# SCP: Test Chamber Delta - Updated GDD
## Project Status: Part 1 Implementation Complete

---

## 1. Game Overview
**Title:** SCP: Test Chamber Delta  
**Genre:** 3D First-Person Survival / Stealth  
**Platform:** WebGL, Android (Mobile Support)  
**Engine:** Unity 2022.3.62f3 (Built-in Render Pipeline)  
**Target Playtime:** 5–8 minutes  

**Game Summary:**
SCP: Test Chamber Delta is a 3D single-level survival experience set inside a controlled containment experiment. The player assumes the role of Class-D personnel placed inside a large, abandoned containment chamber for testing purposes. The objective is to survive an encounter with SCP-173 and reach the designated extraction point.

The game includes a Main Menu, Options Screen, Game-Over Screen, Pause functionality, and an inventory system with pickable items.

---

## 2. Core Gameplay (Planned for Future Parts)
- **SCP-173 Detection:** SCP-173 moves only when it is not being directly observed
- **Strategic Movement:** Player must maintain visual awareness and navigate toward extraction point
- **Item System:** Collectible items provide gameplay enhancements (healing, speed boost, vision, effects)

---

## 3. Level Design
Utilizes the **Abandoned Asylum** environment pack with:
- Starting holding cell (Zone A)
- Multiple corridors and rooms
- Environmental obstacles
- Extraction zone (Zone B)

---

## 4. Player Mechanics

### Movement & Controls (New Input System Implementation)
- **Desktop (PC):** WASD + Mouse Look
- **Android/iOS:** On-screen joystick + touch controls
- **Gamepad:** Left Stick movement + Right Stick look

### Sprint System
- **Desktop:** Hold Left Shift to sprint (continuous)
- **Android/iOS:** Tap Sprint button to toggle sprint ON/OFF
- Includes stamina bar with cooldown mechanic

### Camera Control
- Mouse/Gamepad right stick for look
- Strategic camera control required to manage SCP-173

### Inventory Items (IMPLEMENTED)
Available pickable items:
1. **SCP-500** - Panacea (red pills, healing item)
2. **SCP-178** - 3D Glasses (cyan, vision enhancement)
3. **SCP-198** - Cup of Joe (yellow, stamina boost)
4. **SCP-1025** - Pulp Periodical (magenta, random effects)
5. **Flashlight** - Toggle with F key, only usable after pickup

### UI Features (IMPLEMENTED)
- **Inventory Display:** Shows collected items with icons and names
- **Flashlight System:** Toggle with F key when in inventory
- **Pause Menu:** Press Escape to pause (hiding mobile UI except pause button)
- **Game Over Screen:** Displays when SCP-173 reaches player (hides all mobile UI)

---

## 5. Win / Lose Conditions

**Win Condition:**
Reach the designated extraction zone.

**Lose Condition:**
SCP-173 reaches the player.

---

## 6. Required UI Screens (IMPLEMENTED)

### Main Menu
- Start Game
- Options
- Exit

### Options Screen
- Volume Controls
- Sensitivity Controls

### Pause Screen
- Resume Game
- Options
- Main Menu
- Retry (Optional)

### Game Over Screen
- Retry Level
- Main Menu

### In-Game UI
- **Inventory Bar:** Shows collected items (top-left or custom position)
- **Minimap:** Player and enemy position indicators
- **Sprint Bar:** Displays stamina remaining
- **Crosshair:** Center screen aiming indicator
- **Mobile Controls:** Platform-specific (shows on Android/iOS, hidden on Desktop)

---

## 7. Audio Design
- Ambient containment chamber audio
- SCP-173 movement sound cues
- Environmental sound effects
- Menu and Game Over background music
- Pause/Resume audio handling

---

## 8. Asset Plan

### Primary Environment
- **Abandoned Asylum** (Unity Asset Store)

### 3D Models
- **SCP-173** (Creative Commons Attribution licensed model)

### Additional Assets
- PBR Lamp pack
- Ambient audio
- SCP logo textures
- Item sprites/icons (SCP500, SCP178, SCP198, SCP1025, Flashlight)

### Attribution
Proper attribution included for all third-party assets.

---

## 9. Implementation Details (Part 1 - Completed)

### Input System Refactoring
- ✅ Migrated from old Input Manager to **New Input System**
- ✅ Supports Keyboard, Gamepad, and Mobile Input
- ✅ Unified input handling across all player controls

### Inventory System (IMPLEMENTED)
- ✅ `SimpleInventory.cs` - Core inventory management with events
- ✅ `InventoryItemType.cs` - Enum of collectible items
- ✅ `InventoryPickup.cs` - Pickup trigger logic
- ✅ `InventoryItemDatabase.cs` - Centralized item data (name, icon)
- ✅ `InventoryDisplay.cs` - UI display system
- ✅ `InventoryItemUI.cs` - Individual item UI rendering

### Flashlight System (IMPLEMENTED)
- ✅ `FlashlightController.cs` - Toggle with F key
- ✅ Only activates when Flashlight item in inventory
- ✅ Uses New Input System for mobile compatibility

### Mobile Support (IMPLEMENTED)
- ✅ `MobileUIController.cs` - Shows/hides mobile UI based on platform
- ✅ Toggle sprint on Android/iOS
- ✅ Platform-specific cursor handling
- ✅ Mobile UI responsive layout

### UI Integration (IMPLEMENTED)
- ✅ Pause button shows when game paused (mobile UI hides except pause button)
- ✅ All mobile UI hides on game over
- ✅ Inventory disappears on pause/game over
- ✅ Canvas Scaler for responsive UI

### Managers Updated (IMPLEMENTED)
- ✅ `PauseManager.cs` - New Input System with Escape key, mobile UI integration
- ✅ `GameOverManager.cs` - Mobile UI hiding
- ✅ `GameManager.cs` - Scene loading with cursor reset
- ✅ `FirstPersonController.cs` - Sprint toggle for mobile, New Input System

---

## 10. Bug Fixes (Completed)

### Cursor Lock Issue
- **Problem:** Cursor locked and disappeared when starting new game from Bootstrap scene
- **Solution:** Added `Cursor.lockState = CursorLockMode.None` and `Cursor.visible = true` in `GameManager.LoadMenu()`
- **Status:** ✅ Fixed

### Platform-Specific Cursor Handling
- **Android:** Cursor lock disabled (touch input)
- **Desktop:** Cursor lock enabled when not paused
- **Status:** ✅ Implemented

---

## 11. Project Management

### Version Control
- GitHub repository with structured commits
- `.gitignore` configured for Unity projects

### Development Tracking
- GitHub Projects (Kanban Board) for task organization
- Stages: Backlog → In Progress → Completed

### Build Targets
- **Desktop (WebGL/PC):** Full keyboard/mouse controls
- **Android:** Mobile UI, touch controls, toggle sprint
- **iOS:** Mobile UI, touch controls, toggle sprint (future)

---

## 12. Next Steps (Future Implementation)

### Part 2 - Gameplay Mechanics
- [ ] Implement SCP-173 AI with line-of-sight detection
- [ ] Detection system (player must keep looking at SCP-173)
- [ ] Extraction zone trigger and win condition
- [ ] Game Over logic when SCP-173 reaches player

### Part 3 - Polish & Optimization
- [ ] Visual effects and particle systems
- [ ] Advanced audio mixing
- [ ] Performance optimization for WebGL/Android
- [ ] Save/Load system (mentioned in GDD)

### Optional Enhancements
- [ ] Difficulty settings
- [ ] Additional SCP items with special abilities
- [ ] Leaderboard system
- [ ] Accessibility options

---

## 13. Technical Stack

### Frameworks & Libraries
- Unity 2022.3.62f3
- **New Input System** (InputAction-based)
- TextMesh Pro (UI text)
- UnityEngine.UI

### Code Architecture
- Event-based system (UnityEvents for loose coupling)
- Singleton pattern (GameManager, GameOverManager, InventoryItemDatabase, SimpleInventory)
- MonoBehaviour-based component architecture

### Scripting Languages
- C# (.NET Framework)

---

## Document History
**Last Updated:** April 21, 2026  
**Status:** Part 1 Complete - Core Systems Implemented  
**Next Review:** Before Part 2 Implementation
