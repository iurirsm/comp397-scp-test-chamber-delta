// CHANGE LOG
//
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1
// "Reworked all input to Unity New Input System for gamepad/on-screen joystick support"      || version 1.1.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
    using UnityEditor;
    using System.Net;
#endif

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;

    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool isZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    // Internal Variables
    private bool isWalking = false;

    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    // Internal Variables
    private CanvasGroup sprintBarCG;
    private bool isSprinting = false;
    private bool isSprintToggled = false; // For toggle sprint on Android
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;

    #endregion

    #region Jump

    public bool enableJump = true;
    public float jumpPower = 5f;

    // Internal Variables
    private bool isGrounded = false;

    #endregion

    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    // Internal Variables
    private bool isCrouched = false;
    private Vector3 originalScale;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 jointOriginalPos;
    private float timer = 0;

    #endregion

    // -----------------------------------------------------------------------
    // New Input System – InputAction fields
    // Wire these up in the Inspector (or leave them as-is and use the default
    // bindings below which cover keyboard, mouse, gamepad and on-screen sticks)
    // -----------------------------------------------------------------------
    #region Input Actions

    [Header("Input Actions")]
    [Tooltip("Move action – bind to WASD, Left Stick, and On-Screen Joystick")]
    public InputAction moveAction = new InputAction("Move", InputActionType.Value,
        binding: "<Gamepad>/leftStick",
        interactions: null,
        processors: "StickDeadzone");

    [Tooltip("Look action – bind to Mouse Delta, Right Stick, and On-Screen Look Joystick")]
    public InputAction lookAction = new InputAction("Look", InputActionType.Value,
        binding: "<Mouse>/delta");

    [Tooltip("Sprint action – hold to sprint")]
    public InputAction sprintAction = new InputAction("Sprint", InputActionType.Button,
        binding: "<Keyboard>/leftShift");

    [Tooltip("Jump action")]
    public InputAction jumpAction = new InputAction("Jump", InputActionType.Button,
        binding: "<Keyboard>/space");

    [Tooltip("Crouch action")]
    public InputAction crouchAction = new InputAction("Crouch", InputActionType.Button,
        binding: "<Keyboard>/leftCtrl");

    [Tooltip("Zoom action")]
    public InputAction zoomAction = new InputAction("Zoom", InputActionType.Button,
        binding: "<Mouse>/rightButton");

    // Cached per-frame values
    private Vector2 moveInput;
    private Vector2 lookInput;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        crosshairObject = GetComponentInChildren<Image>();

        playerCamera.fieldOfView = fov;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }

        // Add keyboard + gamepad composite bindings so a single action covers all devices
        AddDefaultBindings();
    }

    private void AddDefaultBindings()
    {
        // Move – WASD composite + gamepad left stick
        moveAction.AddCompositeBinding("2DVector")
            .With("Up",    "<Keyboard>/w")
            .With("Down",  "<Keyboard>/s")
            .With("Left",  "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        // Gamepad left stick already set as primary binding in field initializer

        // Look – gamepad right stick (mouse delta already set)
        lookAction.AddBinding("<Gamepad>/rightStick")
            .WithProcessor("ScaleVector2(x=3,y=3)");

        // Sprint – gamepad left shoulder / south button
        sprintAction.AddBinding("<Gamepad>/leftShoulder");

        // Jump – gamepad south button
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        // Crouch – gamepad right shoulder
        crouchAction.AddBinding("<Gamepad>/rightShoulder");

        // Zoom – gamepad left trigger
        zoomAction.AddBinding("<Gamepad>/leftTrigger");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
        crouchAction.Enable();
        zoomAction.Enable();

        // Subscribe jump & crouch to callbacks so they fire on press/release correctly
        jumpAction.performed   += OnJumpPerformed;
        crouchAction.performed += OnCrouchPerformed;
        crouchAction.canceled  += OnCrouchCanceled;
        
        // On Android, sprint is toggleable instead of hold-to-sprint
#if UNITY_ANDROID || UNITY_IOS
        sprintAction.performed += OnSprintPerformed;
#endif
        
        zoomAction.performed   += OnZoomPerformed;
        zoomAction.canceled    += OnZoomCanceled;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
        zoomAction.Disable();

        jumpAction.performed   -= OnJumpPerformed;
        crouchAction.performed -= OnCrouchPerformed;
        crouchAction.canceled  -= OnCrouchCanceled;
        
#if UNITY_ANDROID || UNITY_IOS
        sprintAction.performed -= OnSprintPerformed;
#endif
        
        zoomAction.performed   -= OnZoomPerformed;
        zoomAction.canceled    -= OnZoomCanceled;
    }

    // -----------------------------------------------------------------------
    // Input callbacks
    // -----------------------------------------------------------------------
    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (enableJump && isGrounded)
            Jump();
    }

    private void OnCrouchPerformed(InputAction.CallbackContext ctx)
    {
        if (!enableCrouch) return;

        if (holdToCrouch)
        {
            isCrouched = false;
            Crouch();
        }
        else
        {
            Crouch(); // toggle
        }
    }

    private void OnCrouchCanceled(InputAction.CallbackContext ctx)
    {
        if (!enableCrouch || !holdToCrouch) return;
        isCrouched = true;
        Crouch();
    }

    // Toggle sprint on mobile platforms (Android/iOS)
    private void OnSprintPerformed(InputAction.CallbackContext ctx)
    {
        isSprintToggled = !isSprintToggled;
    }

    private void OnZoomPerformed(InputAction.CallbackContext ctx)
    {
        if (!enableZoom || isSprinting) return;
        if (holdToZoom)
            isZoomed = true;
        else
            isZoomed = !isZoomed;
    }

    private void OnZoomCanceled(InputAction.CallbackContext ctx)
    {
        if (holdToZoom)
            isZoomed = false;
    }

    void Start()
    {
        if (lockCursor)
        {
            #if !UNITY_ANDROID
            Cursor.lockState = CursorLockMode.Locked;
            #else
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            #endif
        }

        if (crosshair)
        {
            crosshairObject.sprite = crosshairImage;
            crosshairObject.color  = crosshairColor;
        }
        else
        {
            crosshairObject.gameObject.SetActive(false);
        }

        #region Sprint Bar

        sprintBarCG = GetComponentInChildren<CanvasGroup>();

        if (useSprintBar)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth  = Screen.width;
            float screenHeight = Screen.height;

            sprintBarWidth  = screenWidth  * sprintBarWidthPercent;
            sprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth,     sprintBarHeight,     0f);
            sprintBar.rectTransform.sizeDelta   = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

            if (hideBarWhenFull)
                sprintBarCG.alpha = 0;
        }
        else
        {
            sprintBarBG.gameObject.SetActive(false);
            sprintBar.gameObject.SetActive(false);
        }

        #endregion
    }

    private void Update()
    {
        // Read inputs once per frame
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        #region Camera

        if (cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + lookInput.x * mouseSensitivity;

            if (!invertCamera)
                pitch -= mouseSensitivity * lookInput.y;
            else
                pitch += mouseSensitivity * lookInput.y;

            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles          = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        #region Camera Zoom

        if (enableZoom)
        {
            if (isZoomed)
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
            else if (!isSprinting)
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov,     zoomStepTime * Time.deltaTime);
        }

        #endregion
        #endregion

        #region Sprint

        if (enableSprint)
        {
            if (isSprinting)
            {
                isZoomed = false;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                if (!unlimitedSprint)
                {
                    sprintRemaining -= 1 * Time.deltaTime;
                    if (sprintRemaining <= 0)
                    {
                        isSprinting       = false;
                        isSprintToggled   = false; // Reset toggle on Android when out of stamina
                        isSprintCooldown  = true;
                    }
                }
            }
            else
            {
                sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
            }

            if (isSprintCooldown)
            {
                sprintCooldown -= 1 * Time.deltaTime;
                if (sprintCooldown <= 0)
                    isSprintCooldown = false;
            }
            else
            {
                sprintCooldown = sprintCooldownReset;
            }

            if (useSprintBar && !unlimitedSprint)
            {
                float sprintRemainingPercent = sprintRemaining / sprintDuration;
                sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
            }
        }

        #endregion

        CheckGround();

        if (enableHeadBob)
            HeadBob();
    }

    void FixedUpdate()
    {
        #region Movement

        if (playerCanMove)
        {
            // Build target velocity from new input system move values
            Vector3 targetVelocity = new Vector3(moveInput.x, 0, moveInput.y);

            isWalking = (targetVelocity.x != 0 || targetVelocity.z != 0) && isGrounded;

            // On Android/iOS, use toggle sprint. On desktop, use hold-to-sprint
#if UNITY_ANDROID || UNITY_IOS
            bool sprintHeld = enableSprint && isSprintToggled && sprintRemaining > 0f && !isSprintCooldown;
#else
            bool sprintHeld = enableSprint && sprintAction.IsPressed() && sprintRemaining > 0f && !isSprintCooldown;
#endif

            if (sprintHeld)
            {
                targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                Vector3 velocity       = rb.velocity;
                Vector3 velocityChange = targetVelocity - velocity;
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                if (velocityChange.x != 0 || velocityChange.z != 0)
                {
                    isSprinting = true;

                    if (isCrouched)
                        Crouch();

                    if (hideBarWhenFull && !unlimitedSprint)
                        sprintBarCG.alpha += 5 * Time.deltaTime;
                }

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            else
            {
                isSprinting = false;

                if (hideBarWhenFull && sprintRemaining == sprintDuration)
                    sprintBarCG.alpha -= 3 * Time.deltaTime;

                targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                Vector3 velocity       = rb.velocity;
                Vector3 velocityChange = targetVelocity - velocity;
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }

        #endregion
    }

    private void CheckGround()
    {
        Vector3 origin    = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float   distance  = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            isGrounded = false;
        }

        if (isCrouched && !holdToCrouch)
            Crouch();
    }

    private void Crouch()
    {
        if (isCrouched)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            walkSpeed /= speedReduction;
            isCrouched = false;
        }
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed *= speedReduction;
            isCrouched = true;
        }
    }

    private void HeadBob()
    {
        if (isWalking)
        {
            if (isSprinting)
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            else if (isCrouched)
                timer += Time.deltaTime * (bobSpeed * speedReduction);
            else
                timer += Time.deltaTime * bobSpeed;

            joint.localPosition = new Vector3(
                jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x,
                jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y,
                jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            timer = 0;
            joint.localPosition = new Vector3(
                Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed),
                Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed),
                Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }
}



// Custom Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(FirstPersonController)), InitializeOnLoadAttribute]
    public class FirstPersonControllerEditor : Editor
    {
    FirstPersonController fpc;
    SerializedObject SerFPC;

    private void OnEnable()
    {
        fpc    = (FirstPersonController)target;
        SerFPC = new SerializedObject(fpc);
    }

    public override void OnInspectorGUI()
    {
        SerFPC.Update();

        EditorGUILayout.Space();
        GUILayout.Label("Modular First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold,   fontSize = 16 });
        GUILayout.Label("By Jess Case",                   new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 1.1.0",                  new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

        #region Camera Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.playerCamera    = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "Camera attached to the controller."), fpc.playerCamera, typeof(Camera), true);
        fpc.fov             = EditorGUILayout.Slider(new GUIContent("Field of View", "The camera's view angle."), fpc.fov, fpc.zoomFOV, 179f);
        fpc.cameraCanMove   = EditorGUILayout.ToggleLeft(new GUIContent("Enable Camera Rotation", "Determines if the camera is allowed to move."), fpc.cameraCanMove);

        GUI.enabled = fpc.cameraCanMove;
        fpc.invertCamera    = EditorGUILayout.ToggleLeft(new GUIContent("Invert Camera Rotation", "Inverts the up and down movement of the camera."), fpc.invertCamera);
        fpc.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Look Sensitivity", "Determines how sensitive the look movement is."), fpc.mouseSensitivity, .1f, 10f);
        fpc.maxLookAngle    = EditorGUILayout.Slider(new GUIContent("Max Look Angle", "Determines the max and min angle the player camera is able to look."), fpc.maxLookAngle, 40, 90);
        GUI.enabled = true;

        fpc.lockCursor = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "Turns off the cursor visibility and locks it to the middle of the screen."), fpc.lockCursor);
        fpc.crosshair  = EditorGUILayout.ToggleLeft(new GUIContent("Auto Crosshair", "Determines if the basic crosshair will be turned on."), fpc.crosshair);

        if (fpc.crosshair)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Crosshair Image", "Sprite to use as the crosshair."));
            fpc.crosshairImage = (Sprite)EditorGUILayout.ObjectField(fpc.crosshairImage, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.crosshairColor = EditorGUILayout.ColorField(new GUIContent("Crosshair Color", "Determines the color of the crosshair."), fpc.crosshairColor);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        #region Camera Zoom Setup

        GUILayout.Label("Zoom", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        fpc.enableZoom  = EditorGUILayout.ToggleLeft(new GUIContent("Enable Zoom", "Determines if the player is able to zoom in while playing."), fpc.enableZoom);

        GUI.enabled = fpc.enableZoom;
        fpc.holdToZoom   = EditorGUILayout.ToggleLeft(new GUIContent("Hold to Zoom", "Requires the player to hold the zoom input instead of pressing to toggle."), fpc.holdToZoom);
        fpc.zoomFOV      = EditorGUILayout.Slider(new GUIContent("Zoom FOV", "Determines the field of view the camera zooms to."), fpc.zoomFOV, .1f, fpc.fov);
        fpc.zoomStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while zooming in."), fpc.zoomStepTime, .1f, 10f);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Movement Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.playerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), fpc.playerCanMove);

        GUI.enabled = fpc.playerCanMove;
        fpc.walkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player will move while walking."), fpc.walkSpeed, .1f, fpc.sprintSpeed);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #region Sprint

        GUILayout.Label("Sprint", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        fpc.enableSprint    = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprint", "Determines if the player is allowed to sprint."), fpc.enableSprint);

        GUI.enabled = fpc.enableSprint;
        fpc.unlimitedSprint = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Allows for unlimited sprint."), fpc.unlimitedSprint);
        fpc.sprintSpeed     = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."), fpc.sprintSpeed, fpc.walkSpeed, 20f);
        fpc.sprintDuration  = EditorGUILayout.Slider(new GUIContent("Sprint Duration", "Determines how long the player can sprint."), fpc.sprintDuration, 1f, 20f);
        fpc.sprintCooldown  = EditorGUILayout.Slider(new GUIContent("Sprint Cooldown", "Determines the recovery time when the player runs out of sprint."), fpc.sprintCooldown, .1f, fpc.sprintDuration);
        fpc.sprintFOV       = EditorGUILayout.Slider(new GUIContent("Sprint FOV", "Determines the field of view the camera changes to while sprinting."), fpc.sprintFOV, fpc.fov, 179f);
        fpc.sprintFOVStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while sprinting."), fpc.sprintFOVStepTime, .1f, 20f);
        fpc.useSprintBar    = EditorGUILayout.ToggleLeft(new GUIContent("Use Sprint Bar", "Determines if the default sprint bar will appear on screen."), fpc.useSprintBar);

        if (fpc.useSprintBar)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            fpc.hideBarWhenFull = EditorGUILayout.ToggleLeft(new GUIContent("Hide Full Bar", "Hides the sprint bar when sprint duration is full."), fpc.hideBarWhenFull);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar BG", "Object to be used as sprint bar background."));
            fpc.sprintBarBG = (Image)EditorGUILayout.ObjectField(fpc.sprintBarBG, typeof(Image), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar", "Object to be used as sprint bar foreground."));
            fpc.sprintBar = (Image)EditorGUILayout.ObjectField(fpc.sprintBar, typeof(Image), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.sprintBarWidthPercent  = EditorGUILayout.Slider(new GUIContent("Bar Width",  "Determines the width of the sprint bar."),  fpc.sprintBarWidthPercent,  .1f,   .5f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            fpc.sprintBarHeightPercent = EditorGUILayout.Slider(new GUIContent("Bar Height", "Determines the height of the sprint bar."), fpc.sprintBarHeightPercent, .001f, .025f);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Jump

        GUILayout.Label("Jump", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        fpc.enableJump = EditorGUILayout.ToggleLeft(new GUIContent("Enable Jump", "Determines if the player is allowed to jump."), fpc.enableJump);

        GUI.enabled = fpc.enableJump;
        fpc.jumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player will jump."), fpc.jumpPower, .1f, 20f);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Crouch

        GUILayout.Label("Crouch", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        fpc.enableCrouch  = EditorGUILayout.ToggleLeft(new GUIContent("Enable Crouch", "Determines if the player is allowed to crouch."), fpc.enableCrouch);

        GUI.enabled = fpc.enableCrouch;
        fpc.holdToCrouch  = EditorGUILayout.ToggleLeft(new GUIContent("Hold To Crouch", "Requires the player to hold the crouch input instead of pressing to toggle."), fpc.holdToCrouch);
        fpc.crouchHeight  = EditorGUILayout.Slider(new GUIContent("Crouch Height", "Determines the y scale of the player object when crouched."), fpc.crouchHeight, .1f, 1);
        fpc.speedReduction = EditorGUILayout.Slider(new GUIContent("Speed Reduction", "Determines the percent 'Walk Speed' is reduced by while crouched."), fpc.speedReduction, .1f, 1);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Head Bob

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Head Bob Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        fpc.enableHeadBob = EditorGUILayout.ToggleLeft(new GUIContent("Enable Head Bob", "Determines if the camera will bob while the player is walking."), fpc.enableHeadBob);

        GUI.enabled = fpc.enableHeadBob;
        fpc.joint     = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Joint", "Joint object position is moved while head bob is active."), fpc.joint, typeof(Transform), true);
        fpc.bobSpeed  = EditorGUILayout.Slider(new GUIContent("Speed",      "Determines how often a bob rotation is completed."),                    fpc.bobSpeed,  1, 20);
        fpc.bobAmount = EditorGUILayout.Vector3Field(new GUIContent("Bob Amount", "Determines the amount the joint moves on every axis."),           fpc.bobAmount);
        GUI.enabled = true;

        #endregion

        if (GUI.changed)
        {
            EditorUtility.SetDirty(fpc);
            Undo.RecordObject(fpc, "FPC Change");
            SerFPC.ApplyModifiedProperties();
        }
    }
}

#endif