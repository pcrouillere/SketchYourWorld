========What's New\Fixes v1.3========
Added Skybox shaders
Fixed problem with Shield shaders.
Fixed problem with rimlit shaders.
Fixed problem with reflective shaders.
Memory Leak fixed
Fixed bug in 4 extra light system
Few optimizations.
Many minor bug fixes.

========What's New\Fixes v1.2========
•   Eye follow class 
•   GUI Shaders
•   Emmisive Shaders
•   Ramp/Toon Shaders that work with unity shadows.
•   New Lighting System with many options to customize lit shaders!
    a) Choose any combination of main directional, main point or 4 extra point lights
    b) New Lighting System lets user have infinite lights. Framework uses 4 closest to camera 
    b) Choose to add Beast lightmapping to lit shaders.
    c) Choose the main light that affects specular/rim/bump-mapping/ramp shaders.
    d) Option to turn main point light falloff on and off.
    e) Option to double light in frag shader.
    f) Option to tint shadow color  ( Pro Only ).
    g) Option to make main directional light a spotlight\flashlight for all lit shaders.
    h) Each group of lights has an intensity multiply value.
    i) Compile feature to make lit shaders only contain the code from the options you pick.
    
•   _EchoCoreManager now works with DontDestroyOnLoad().
•   Light probes now give off proper light.
•   Fixed problem with how shadows were rendered.
•   Fixed bug in cutout shaders.
•   Fixed a bug in lit shaders that only affected certain gpu's
•   Fixed a few bugs in Outline/Overlay system.
•   Fixed a bug in EchoFXEvent.
•	Fixed bug in EchoGameObject for _Color property.
•   Fixed bug in EchoLinked List.
•   Fixed bug in transparent shaders when using Beast.
•   EchoGameObject now uses less memory in most cases.
•   Rim lighting on planet shaders was improved.
•   More options on Asteroid Belt class.
•   Changed some shader property names to match what unity shaders use.
•   Many optimizations. 

//============================
Getting Started
//============================

1.  If you are upgrading from older version or a beta version. Follow steps below before importing 1.2 asset
	a. Try out new lit shader options in empty project before upgrading your main app.
	   You must now press compile on _EchoCoreManager before running any sample projects that use lit shaders.
	b. Backup your project.
	c. Remove echoLogin folder.
	d. Remove plugins\echoLogin folder.
	e. Import v1.3 of framework.
	f. IMPORTANT - All files in CORE Framework must be in the default install location!
	g. Any questions about this contact me. (info at bottom)

2.	Make sure forward rendering path is selected.

3.	Add _EchoCoreManager prefab to the scene 
		a. This is only needed if you are using EchoGameObject, Lit Shaders or EchoFXEvent.
		b. Delete any old EchoCoreManager' and replace with _EchoCoreManager

4.	Click the _EchoCoreManager object to select options:

	a.	EchoFXEvents
	 	Pre-Allocate Events - will pre allocate this number of events on startup
	 						  if your not using any echoFXEvents this can be set to zero.
	 						  
	b. 	Add Events As Needed - when set the system will dynamically add new events if the 
	                           pre allocated limit has been reached.


	c.	Use Unity Lights 
		Enabled -	This needs to be on if your are using non Core-Framework shaders or realtime shadows.
					Turning this off will disable all unity lights, which may improve performance.
					
	d.	Ambient Light
		Multiply Intensity - Multiplies the ambient light by this value.

	e.  Add Beast Code
		Enabled 	- When set, this will add Beast light-mapping code to all lit shaders.
		
	f.  Double Light In Frag Shader:
		Enabled - This should be enabled when using non Core Framework shaders to
				  keep the light intensity the same. Lit shaders will be faster when this is off.
		
	g.	Main Directional Light - One always on directional light. This lights rendermode should be set to auto or important.
			Enabled - Add code to shader to use this light. 
			Multiply Intensity - multiplies this lights color by this value.
			Use As Spotlight - sets all lit shaders to use this light as a spotlight.
			Spot Beam Size - size of spot light beam.
			Enabled Shadow Tint - if set will tint shadows.
			Shadow Color - color of shadow tint.
				Affect Shaders Of Type
					Specular/Rim/Ramp - this light will affect shaders of type Specular, Rim or Ramp.
					BumpMap - When set this light will affect bumpmap shaders.

	h.	Main Point Light - One always on point light. This lights rendermode should be set to auto or important.
			Enabled - Add code to shader to use this light.
			Multiply Intensity - multiplies this lights color by this value.
			Light Falloff - when set light will fall off linearly with lights range.
			Ramp Fade - When checked will fade the light value and the ramp value for ramp shaders only.
			          - When unchecked will only fade light value.
				Affect Shaders Of Type
					Specular/Rim/Ramp - this light will affect shaders of type Specular, Rim or Ramp.
					BumpMap - When set this light will affect bumpmap shaders.


	i.	4 Extra Point Lights - picks the 4 closest lights to main camera that have render mode set to "not important".
			Enabled - Add code to shader to use the 4 light system.
			Multiply Intensity - multiplies this lights color by this value.
			Look Ahead - how far to look in front of main camera to find the 4 closest lights.
				Affect Shaders Of Type
					Specular - this light will affect shaders of type Specular.

	j.  LightProbes/SH - Add code to lit shaders to support LightProbes\Spherical Harmonics.
	
	k.	Compile Shaders - Press this to generate shaders that only include the code from the options above.
	    


Class Extension Examples

C#:

// Default Method
class MyGameObjectScript : MonoBehaviour
{
}

// CoreFramework Method
class MyGameObjectScript : EchoGameObject
{
}


JS:

// Default Method ( by default in JS the class is not specified )
class MyGameObjectScript
{
}

// CoreFramework Method
class  MyGameObjectScript extends EchoGameObject
{
}


EchoGameObject Options

All EchoGameObject(s) in the scene must be active in order for CoreFramework to register them.

The following options will appear in the inspector for all objects that include EchoGameObject or scripts derived from EchoGameObject

NOTE: You only need to use EchoGameObject if an object is using some part of the framework.
      It is not needed for shaders to work.

The EchoGameObject options are as follows:

•   Alternate Material: Material set here can be swapped by using SwapMaterial().
                        (See Dissolve sample project)

•	Active at Start:	Use this to activate or deactivate the GameObject.
	
•	Renderer Enabled:	Use this to enable the mesh renderer.

•	Fix Scale:			Corrects mesh import floating point scale errors to 1,1,1 -OR- corrects manually scaled GameObjects by .
						resizing the mesh and colliders to the current scale and setting the localScale to 1,1,1. (This is essential for batching)

•	Add Children:		Use this to add an EchoGameObject to children that do not already have an EchoGameObject or derivative attached.

•	Outline Material:	Use this to add an outline material from echoLogin/OutlineMaterials folder. 

•	Outline Color:		Use this to set the color of the outline.

•	Overlay Material:	Use this to add an overlay material from echoLogin/OutlineMaterials folder.

•	Outline Children:	Toggle this on or off to include children in outline\overlay.

•	Outline On:			Toggle this to set if outline is ON or OFF at start.

Be sure to check out sample projects in the SampleProjects folder.

Full reference is available at http://www.echologin.com/coreframework.html


========SUPPORT========

Email/Skype/IRC/PM me any questions!

Support email: core@echologin.com

Skype: echoLogin

Please include #invoice number in subject of email or in msg when adding me to skype.


NOTE: 	When running Space Demo, be sure to add scene1 and Intermission to build settings for it to run properly.
		To add a level to the build settings use the menu File->Build Settings...
		
		You must also re-bake lightmapped sample projects in order for them to look correct.
			Space Demo 		- Intermission scene
			LightMapTest 	- scene1



