A competitive 3D digital board game where players race to the finish line. Supports Player vs Player (local) and Player vs AI modes. The game features an inventory system with tactical power-ups to hinder the opponent or boost progress.

 Game Rules
* **Objective:** Be the first to reach the 71st tile.
* **Movement:** Players take turns rolling a D6 die.
* **Items:** Players collect bonuses on the board and store them in an inventory to use strategically during their turn.

Power-Ups & Items
* **Shield:** Blocks the next incoming negative effect (Bomb or Cursed Die).
* **Bomb:** Explodes on the opponent, reducing their next movement by **1d4** (1-4 steps).
* **Cursed Die:** Knocks the opponent back by **1d6** (1-6 steps).
* **Modifier:** Adds or subtracts 1 step from the current roll.

Technical Highlights
* **State Machine:** Implemented a robust turn manager to handle state transitions (PlayerTurn -> Animation -> EventProcessing -> EndTurn).
* **AI Logic:** The bot analyzes the distance to the finish line and the player's status to decide whether to use items or not.
* **Waypoint Movement:** Smooth character movement along the curved path using interpolation/coroutines.
* **Inventory System:** Logic for storing, selecting, and consuming items with UI feedback.
