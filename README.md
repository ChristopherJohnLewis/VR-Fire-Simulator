# VFireLib Virtual Fire Simulator
This visualization has been stripped of all VR and interface scripts to give anyone the ability to easily attach the scripts to their given project. We are also considering adding this as a package to the Unity store, for easier distribution.

Out of the box, this repository provides code that can randomly generate trees across a terrain (using replacable fuel index's), display a moving fire line across a given 3d terrain using shaders, and display a 3D terrain with appropriate textures. 

This code is dynamic by allowing the developer to modify the simulator with their own data. 

Some of the easily customizable options are:
-Burn map
-Vegetation map
-Satellite Map (a top-down texture of your terrain)
-Height map
-Fuel Data
-Start Time
-Step Time
-Tree Block Size
-Tree Scale
-Noise function
-Burn Color
-Tree models

To run this package, simply:
1. Install Unity Hub
2. Install Unity version 2018.3.4 via Unity Hub like so:
  - Run Unity Hub
  - Navigate to "Installs" on the left side
  - Press the blue "Install Editor" button on the upper right side
  - Navigate to the "Archive" menu
  - Press the "download archive" link 
      - A browser tab will launch to the Unity Download Archive
  - Navigate to the Unity 2018.x tab
  - Scroll down to Unity 2018.3.4
  - Select the green "Unity Hub" button
  - On the popup that appears, press "Open Unity Hub"
  - Unity Hub will open
  - This will give you many options to install specific Unity functionality
  - Make sure the checkbox next to your language is selected
  - Every other option is optional and not necessary to run this plugin
3. Clone this repository however you see fit
4. Go into Unity Hub's "Projects" tab and press the dropdown next to "Open"
5. Press "Add project from disk"
6. Navigate to the folder this repository was cloned into and open the folders until you see ".idea" or "Assets"
7. Make sure the folder says "VR-Fire-Simulator" and press "Add Project"
8. Press the newly created project titled "VR-Fire-Simulator" and wait for it to open. This will take a while
9. When the Unity project opens, open the "Scenes" folder and open the "MainScene" asset
10. When this scene opens up, you should see a dark colored environment in the "Scene view"
11. You can press into the "Game" window to watch the fire spread through Kyle Canyon, or go into the scene view and move yourself around.
  - documentation for how to move in the scene view can be found here: https://docs.unity3d.com/Manual/SceneViewNavigation.html
12. If you wish to use this plugin, please use this program to generate and replace the example files in the "FireSimulator" component: https://github.com/ruiwu1990/jessie_fire_simulation/tree/Dockerized
  - I also strongly recommend having a good understanding of Unity and Shaders before diving into this plugin.


This has not been entirely tested with different maps or differently generated maps, use with https://github.com/ruiwu1990/jessie_fire_simulation/tree/Dockerized for the best results
