Latest builds of Stop the Pop can be downloaded on [Google Play](https://play.google.com/store/apps/details?id=com.stopthepopgame.stopthepop)
and on the [App Store](https://itunes.apple.com/us/app/stop-the-pop/id1166315634?ls=1&mt=8).

## Dependencies

### GUI Animator
Purchase and download [GUI Animator for Unity UI](https://assetstore.unity.com/packages/tools/gui/gui-animator-for-unity-ui-28709)

Import the following files into the project
```
Common/Plugins/GETween.dll
GUI Animator for Unity UI/Plugins/Editor/GUIAnimatorEditor.dll
GUI Animator for Unity UI/Plugins/GUIAnimator.dll
GUI Animator for Unity UI/Prefabs/GSui_Object.prefab
GUI Animator for Unity UI/Scripts/Editor/GAuiEditor.cs
GUI Animator for Unity UI/Scripts/GAui.cs
GUI Animator for Unity UI/Scripts/GSui.cs
```

### iTween
Import [iTween](https://assetstore.unity.com/packages/tools/animation/itween-84) into the project.

### Font 
This project uses the [Bangers font](https://fonts.google.com/specimen/Bangers). License for this font is described [here](https://github.com/buckning/stop-the-pop/blob/master/Assets/Fonts/OFL10.txt)

### Unity Ads
Import [Unity Ads](https://assetstore.unity.com/packages/add-ons/services/unity-ads-66123) from the Asset Store.

### Download and Configure Google Play Game Services
Download the current build from the [Google Play Game Services for Unity Github repository](https://github.com/playgameservices/play-games-plugin-for-unity/tree/master/current-build)

Build for Android:
In the menu bar, click on File -> Build Settings... -> Click on Android -> Click on Switch Platform 

In the menu bar, click "Assests" -> Play Services Resolver -> Android Resolver -> Resolve

### Download Linear Unlit Gradient (Optional)
Download [Linear Unlit Gradients](https://assetstore.unity.com/packages/vfx/shaders/linear-unlit-gradients-51733) from the Asset Store. 
Just import Shaders/LinearGradient.shader and LinearGradient#12.mat files.
This is required for the title screen, game complete screen and some levels.

### Download Toon Fx (Optional)
Download [Toon Fx](https://assetstore.unity.com/packages/vfx/particles/toon-fx-25601) from the Asset Store. 

These are optional since the game will run without them. These partical effects are used in the level compelete screen and game complete screen.

Import the following files:
```
Materials(Mobile)/Recolorable/Confetti2x2(Mobile).mat
Materials(Mobile)/Recolorable/Sparkle(Mobile).mat
Prefabs(Mobile)/Confetti/Confetti2DBlastGreen(Mobile).prefab
Prefabs(Mobile)/Confetti/Confetti2DBlastPink(Mobile).prefab
Prefabs(Mobile)/Sparkles/Sparkle(Mobile).prefab
Textures(Mobile)/Greyscale/confetti2x2(Mobile).png
Textures(Mobile)/Greyscale/sparkle(Mobile).png
```
Some minor changes are required to get these particle systems to work as expected.

In Unity, navigate to Prefabs(Mobile)/Confetti/ and select both Confetti prefabs. Click on Add Component in the inspector and type UnscaledTimeParticles C# script. Next change Particle System settings in the inspector, expand the Renderer section and change the Sorting Layer to UI. These changes are needed to properly display and run the particle effects in the level complete screen.

Click on the Sparkle(Mobile) prefab, in the inspector click on the Particle System component and expand the Shape section. Change the X and Z scale settings to fit your screen.

## Directory Structure
game-art directory contains SVG files for all the artwork within the game
