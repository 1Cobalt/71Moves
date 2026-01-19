A 2D arcade shooter game where the player progresses through levels by popping balloons with unique properties. Features multiple weapon types, an achievement system, and optimized performance for mobile devices.

Gameplay Features
* **Progression System:** Complete levels by reaching score targets to unlock new stages.
* **Diverse Mechanics:**
    * **Weapons:** Unlock different bows/weapons as you progress.
    * **Balloon Types:** Strategy is key—avoid "Don't Touch" balloons, focus on "Healers" to restore lives, and break "Normal" and "Armored" ones.
* **Achievements:** In-game tracking of milestones (e.g., "Pop 100 balloons").

Technical Highlights
* **Object Pooling:** Implemented a custom pooling system for arrows and balloons to prevent Garbage Collection spikes and ensure smooth 60 FPS.
* **OOP Architecture:** Used inheritance for balloon logic (`Baloon` -> `HealingBalloon`, `DurableBaloon`), making it easy to add new types.
* **Data Persistence:** Saving player progress (levels unlocked, achievements).
* **UI/UX:** Dynamic HUD showing live score, arrow count, and health status.
