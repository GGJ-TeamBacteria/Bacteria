MixCast VR plugin for Unity - v1.5.0
(c) Blueprint Reality Inc., 2017. All rights reserved



Quickstart:

1) Attach the "MixCastCameras" prefab to your VR Room transform's GameObject ([CameraRig] in default SteamVR setup).
2) Add the "MixCast UI" prefab to your UI GameObject(s) where Mixed Reality controls should be accessible. If you haven't used Unity's UI system yet, you'll probably also need to add its EventSystem component for input to be responded to, by right clicking in the Hierarchy Window and selecting "UI/Event System".

Note: If upgrading from a previous version of MixCast, please delete the old MixCast folder(s) before importing the new package


Dependencies:
	Required
	- Unity 5.5 or above

	Optional
	- SteamVR plugin by Valve Corporation
	- Oculus Utilities for Unity 5

The MixCast VR Studio application must be installed and run prior to enabling MixCast VR output in your application at runtime.


Extras:
MixCast also comes with some extra prefabs and scripts to aid with mixed reality development and production.

Slate UI prefab: Provides a film slate style display that can be called up via keypress to aid in the capture of multiple takes in a row. Inspect the attached script for more details. Drop into any scene.

Player Blob Shadow prefabs: Causes a simple blob shadow to appear in MixCast output at the point on the ground where the player's head is over. Attach to the Head or Eye transform.

SetRenderingForMixCast script: Controls whether the specified Renderer components are visible during regular rendering or MixCast rendering rather than both. By default grabs all Renderers under it in the hierarchy.


Changelist:

v1.5.0
- Added Recordings folder for MixCast-generated content.
- Added optional MixCast Auto-Start on application startup
- Added optional automatic periodic screenshot capture

v1.4.0
- Added optional Player lighting which allows Unity lights to apply to the player
- Added optional Player-relative camera feed cropping
- Added lightweight localization system
- Added user framerate control
- Greatly improved feed synchronization in low framerate situations
- Improved OpenVR tracking device serialization to match by Serial ID
- Fixed bug involving deserializing the static subtraction textures in a linear color space project
- Fixed Oculus space mismatch bug
- Fixed camera HDR warning in Unity 5.6 and above

v1.3.0
- Merged input feed shaders and materials using a multi_compile shader for easier custom effects.
- Added Posterize and HSV Modify (Desaturate, etc) variations on the input feed shader for custom player effects. Apply one of the shaders to the supplied Camera Feed material to activate it.
- Added tracked motion smoothing to reduce tracking and/or hand jitter.
- Improved WebcamFeed/MixCastCamera relationship for separation of concerns.
- Studio has new quick setup for FoV and Alignment.
- Fixed HDR texture allocation.

v1.2.0
- Added support for Oculus SDK based projects. Created Oculus and SteamVR specific code branches and an Editor process to activate the appropriate one(s) using define flags MIXCAST_STEAMVR and MIXCAST_OCULUS. 
- Added automatic update checking.
- Allowing "None" option for input device for a purely virtual camera that can still be tracked by a controller.
- Created additional isolation modes: "None" and "Static Subtraction". 
	None simply disables background removal while still allowing for foreground-based Mixed Reality.
	Static subtraction provides a rudimentary background removal system for fixed cameras which doesn't require a greenscreen. The scene setup can influence the resulting quality greatly.
- Added ability for user to separate the In-Scene Display from the Camera location. As a result the visual representations are now in separate sub-groups of the MixCast Camera prefab.
- Expanded tracked camera capabilities to be configurable to any tracked controller. 
- Created SetRenderingControllerForMixCast to be attached to the default SteamVR_RenderModel object for the controllers if you don't want them to appear in the Mixed Reality output
- Arrow buttons can be used while camera is being tracked by a device

v1.1.0
- Restructured UI for clarity and expandability. 
- Added buffered output mode with configurable game delay for camera latency compensation. 
- Added icon on in-scene display to communicate if the camera is tracked. 
- Added device resolution selection.

v1.0.2
- Added quadrant output mode for recording. 

v1.0.1
- Updated assets to correct logo. Improved initial setup by having MixCast camera copy main camera settings by default.



Additional Info:

MixCast SDK FAQ: https://blueprinttools.com/mixcast-sdk-faq/
MixCast Studio FAQ: https://blueprinttools.com/mixcast-faq/ 