Mobile GUI system
Gerald Coggins

An easy-to-use game user interface for smart phones and tablets. Includes a diverse variety of input types: joystick-style press and slide, swipe recognition, movement-based controls and supports gamepads. Simple to set up and easy to modify.

==============================================
TO DO LIST
==============================================

Legend
	[ ] uncompleted
	[.] partially complete
	[x] completed
	[|] always needs updating


GENERAL
[ ] Handle Screen Rotation
[ ] handle changing screen sizes
	[x] scale ui
	[ ] static size
[ ] free foating in-game windows
[.] timers
	[.] game timer/frame counter
[ ] input buffer
[ ] shaders

COMPONENTS

GuiHandler
[|] manage and control game state
[|] control screen touch state
[ ] dev can set starting state
[ ] read and display fonts

Basic Window (start screen/system menu/text box)
[.] Graphics
	[x] Backgound
	[ ] Text
	[ ] Effects
[.] Buttons
	[x] Button State
	[x] Up+Down+Disabled+Hidden Image
	[.] Image changes when pressed/disabled
	[x] Able to be pressed and released
	[x] dragging touch off the button prevents pressing
	[ ] dragging onto a button causes it to be able to be pressed

Main Gameplay
[ ] HUD system
	[ ] Graphics - can be enabled/disabled by dev
		[ ] Image - devs decide the position and image
		[ ] Text to Image - devs decide the position, text and image to use for text
[ ] Camera Input - can be enabled/disabled by dev
	[ ] Object tracking
	[ ] Gyroscope - devs control movement ratio
	[ ] Compass - devs control movement ratio
[.] Movement input - any can be enabled/disabled by dev
	[ ] Gamepad
		[ ] Button Mapping
			[ ] Record what buttons the user wants to use
			[ ] Convert input to match mapped buttons
		[ ] Accept Input
			[ ] support common controller types
			[ ] axis and button
		[ ] Give Output
			[ ] axis and button
	[.] TouchScreen
		[ ] Graphics
			[.] Touch point, End point and inbetween
		[x] Accept Input
			[x] use initial point as the center of a joystick
			[x] dev sets min and max distance from the center that will have an effect
			[x] get percent from center to max distance the current point is
			[.] move center point ot follow current point if out of range (can be disabled)
		[x] Give Output
			[x] return direction form center and the % distance from center to end
[.] Action input - can be enabled/disabled by dev
	[ ] Gamepad
		[ ] Button Mapping
			[ ] Record what buttons the user wants to use
			[ ] Convert input to match mapped buttons
		[ ] Accept Input
			[ ] support common controller types
			[ ] axis and button
		[ ] Give Output
			[ ] axis and button
	[.] TouchScreen
		[.] Swipe
			[ ] Graphics
				[ ] initial point, current point and inbetween
			[.] Accept Input
				[.] Simple directional (start and end point only)
					[x] Class
						[x] get angle from start to end point
						[x] check angle against preset angle ranges
						[x] return id for range that the angle is in
						[x] dev modifiable min/max distance
						[x] allow devs to easily add and modify angle ranges
				[ ] Advanced directional (start point, distance to complete swipe, progress through swipe)
					[ ] Graphic - Initial point, End point and progress
				[ ] Shape recognition (create image as swipe progresses and compare it to shape library)
					[ ] Graphic - image of created shape as it is made
			[.] Give Output
	[ ] Screen shake - can be enabled/disabled by dev
		[ ] Graphics
		[ ] direction and intensity - adjustable by dev

Cutscene
[ ] In-Game
	[ ] Remove control from player
	[ ] Load required objects
	[ ] Support scripted control of all objects
[ ] Pre-rendered
	[ ] Remove control from player
	[ ] Load required objects


Start Menu - Prefab
Game Menu - Prefab
[ ] Graphics
	[ ] Image Box
	[ ] Text Box
[ ] Status display
	[ ] Image Box
	[ ] Text Box
[ ] Item display
	[ ] Interactive elements
	[ ] Tooltips
	[ ] Image Box
	[ ] Text Box
System Menu - Prefab



==============================================
CHANGELOG
==============================================

05/15/2017 - v0.0.0 - State Tracking Update + Timers - 0001
===============================

-added temporary way to exit if touchState breaks
-added timer to track time program has been running
-added control over wether or not input is accepted
--boolean allow/disallow
--add to timer to block input for an amount of time
-made it possible to drag a touch off a button to not press it
-improved touchstate tracking
-renamed SystemMenu class to GameWindow
-changed enums to uint type


05/12/2017 - v0.0.0 - Initial Commit
===============================

-multiple touches are handled
-gamestate is tracked
-joystick style control accepts input and gives output as expected
-swipe control accepts input and gives output as expected
-system menu added
|-continue and quit options function
|-acts as expected with multiple touches
