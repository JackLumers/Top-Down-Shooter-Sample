# TopDownShooterSample
A sample game that I did as a test task project. Done in 3 days.

# Project Setup
* Unity version: 2021.3.26f

# Keybinds 
* **WASD/Arrows** - Player movement
* **Left mouse button** - Fire
* **Space** - Change weapon

# Configs
All configs (like weapons, enemies spawning config, e.t.c) can be found in **Configuration** folder.

# Possible Improvements?
* Rewriting own pooling solution.
*There was a recommendation to not use 3d party assets for code, so I took my own old pooling solution.*
* Additional weapon logic. Like autimatic weapons that can shot automatically after cooldown when player holds **Fire Button**.
* Player looking direction mark jitter fix.
* Animations, fancy models.
* Camera adjustments, minor refactoring, additional abstraction layers.

# Used assets
* [Cinemachine](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.3/manual/index.html) package for simple camera setup.
* [UniTask](https://github.com/Cysharp/UniTask). This is technically a 3d party solution, but default asynchronous code in Unity is quite painful to write. And UniTask is kinda industral standard so why not ¯\_(ツ)_/¯
 
