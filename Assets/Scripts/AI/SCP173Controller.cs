using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SCP173Controller : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your FPS PlayerCamera here (the actual Camera component).")]
    public Camera playerCamera;

    [Tooltip("Optional.")]
    public Renderer scpRenderer;

    [Header("Chase Settings")]
    public float maxChaseDistance = 60f;
    public float killDistance = 1.4f;

    [Header("Vision Settings")]
    [Tooltip("How far the camera can 'see' SCP-173 for the raycast check.")]
    public float visionRayDistance = 80f;

    [Tooltip("Layers that can block vision (Walls, props, etc). Exclude Player layer.")]
    public LayerMask occlusionMask = ~0;

    private NavMeshAgent agent;
    private Transform playerTransform;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (scpRenderer == null)
            scpRenderer = GetComponentInChildren<Renderer>();

        // Try to find player if camera not set
        if (playerCamera != null)
            playerTransform = playerCamera.transform;
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        if (playerTransform == null)
            playerTransform = playerCamera.transform;

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Don’t chase forever 
        if (distToPlayer > maxChaseDistance)
        {
            agent.isStopped = true;
            return;
        }

        bool beingLookedAt = IsBeingLookedAtByCamera(playerCamera);

        // SCP-173 moves ONLY when not observed
        agent.isStopped = beingLookedAt;

        if (!beingLookedAt)
        {
            agent.SetDestination(playerTransform.position);

            // Simple “touch kill” fallback 
            if (distToPlayer <= killDistance)
            {
                KillPlayer();
            }
        }
    }

    bool IsBeingLookedAtByCamera(Camera cam)
    {
        if (scpRenderer == null) return false;

        // 1) Is it inside the camera view frustum?
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (!GeometryUtility.TestPlanesAABB(planes, scpRenderer.bounds))
            return false;

        // 2) Is it in front of the camera?
        Vector3 toScp = (scpRenderer.bounds.center - cam.transform.position);
        if (Vector3.Dot(cam.transform.forward, toScp.normalized) <= 0.01f)
            return false;

        // 3) Raycast: is there a wall between camera and SCP?
        float distance = Mathf.Min(toScp.magnitude, visionRayDistance);

        if (Physics.Raycast(cam.transform.position, toScp.normalized, out RaycastHit hit, distance, occlusionMask))
        {
            // If the ray hits SCP (or its children), we can see it
            if (hit.transform == scpRenderer.transform || hit.transform.IsChildOf(transform))
                return true;

            // Hit something else first = blocked
            return false;
        }

        // If raycast hits nothing, assume not visible 
        return false;
    }

    void KillPlayer()
    {
        Debug.Log("SCP-173 killed the player.");

        if (GameOverManager.Instance != null)
            GameOverManager.Instance.TriggerGameOver();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            KillPlayer();
        }
    }
}