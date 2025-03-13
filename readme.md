Godot 2D and 3D Nodes Cheatsheet
================================

This cheatsheet lists all built-in nodes in Godot 4.x for 2D and 3D development, explaining their purpose and practical use cases. Nodes are categorized by their primary context (2D, 3D, or shared), with some overlap where applicable.

2D Nodes
--------

These nodes operate in a 2D coordinate system (X, Y) and are designed for 2D games.

| **Node** | **What It Is** | **When to Use** |
| --- | --- | --- |
| Node2D | Base 2D node with position, rotation, and scale properties. | Use as a root or parent for any 2D object needing spatial transformation (e.g., grouping sprites). |
| AnimatedSprite2D | Plays a sequence of images for animation using a SpriteFrames resource. | Use for animated characters or objects (e.g., a walking player, spinning coin). |
| Area2D | A region that detects collisions or overlaps without physics simulation. | Use for triggers like damage zones, collectibles, or checkpoints. |
| AudioStreamPlayer2D | Plays positional audio in 2D space, with panning based on position relative to the listener. | Use for 2D spatial sound effects (e.g., footsteps or explosions with directional audio). |
| BackBufferCopy | Copies a region of the screen's back buffer for effects like trails or reflections. | Use for advanced rendering effects (e.g., a mirror or ghost trail in 2D). |
| Bone2D | A single bone in a 2D skeleton for skeletal animation. | Use with Skeleton2D for rigging characters (e.g., animating arms or legs). |
| Camera2D | Controls the 2D viewport, allowing zooming, panning, and following objects. | Use to follow a player or focus on specific areas (e.g., scrolling in a large level). |
| CanvasGroup | Groups 2D nodes to render them as a single unit with shared properties (e.g., modulation). | Use for batching 2D objects for performance or applying uniform effects (e.g., tinting a UI group). |
| CanvasLayer | A layer for 2D rendering, unaffected by camera movement or parent transformations. | Use for UI elements or HUDs (e.g., health bars, menus) that stay fixed on screen. |
| CollisionObject2D | Base class for 2D nodes with collision capabilities (not used directly). | Parent class for Area2D, KinematicBody2D, etc.; use its children instead. |
| CollisionPolygon2D | Defines a polygon-shaped collision area for physics or detection. | Use with physics nodes for custom-shaped hitboxes (e.g., irregular platforms). |
| CollisionShape2D | Defines a simple shape (e.g., circle, rectangle) for collision detection. | Pair with Area2D or physics bodies for hitboxes or trigger areas (e.g., a player's hitbox). |
| DampedSpringJoint2D | A 2D physics joint that simulates a spring with damping between two bodies. | Use to connect objects with elastic behavior (e.g., a bouncy bridge or rubber band). |
| GrooveJoint2D | A 2D physics joint that constrains a body to slide along a groove or slot. | Use for sliding mechanics (e.g., a piston or drawer in a 2D game). |
| Joint2D | Base class for 2D physics joints (not used directly). | Parent class for specific joints like PinJoint2D; use its children instead. |
| KinematicBody2D | A 2D physics body you control manually with code (no automatic physics). | Use for player characters or objects needing custom movement (e.g., platformer hero). |
| Light2D | Adds 2D lighting effects like glows or shadows using a texture mask. | Use for atmospheric effects or visibility mechanics (e.g., a torch in a dark level). |
| LightOccluder2D | Defines a shape that blocks Light2D to cast shadows in 2D. | Use with Light2D for shadow effects (e.g., a wall blocking a lamp's light). |
| Line2D | Draws a series of connected 2D lines with customizable width and texture. | Use for visual effects or debugging (e.g., drawing a path, outline, or trajectory). |
| MeshInstance2D | Displays a 2D mesh (geometry) in a 2D scene, typically for custom shapes. | Use for procedurally generated 2D shapes or imported meshes (e.g., a custom polygon object). |
| MultiMeshInstance2D | Renders multiple instances of a 2D mesh efficiently using instancing. | Use for performance-heavy scenes (e.g., hundreds of grass blades or particles). |
| NavigationAgent2D | Handles pathfinding for 2D navigation using a NavigationRegion2D. | Use for AI movement (e.g., enemies following the player on a 2D map). |
| NavigationObstacle2D | Defines an obstacle that affects 2D navigation dynamically. | Use for moving obstacles in pathfinding (e.g., a patrolling enemy blocking a path). |
| NavigationPolygon | Defines a 2D polygon for navigation pathfinding (used with NavigationRegion2D). | Use to create walkable areas for AI (e.g., a level's traversable floor). |
| NavigationRegion2D | A region that enables 2D pathfinding using a NavigationPolygon. | Use for AI navigation in 2D (e.g., a maze or open world with walkable zones). |
| NinePatchRect | A resizable rectangular texture with stretchable borders (like a UI frame). | Use for scalable UI elements (e.g., dialog boxes, buttons with borders). |
| ParallaxBackground | Manages ParallaxLayer nodes for depth effects in scrolling backgrounds. | Use for layered scrolling backgrounds (e.g., moving clouds behind a foreground). |
| ParallaxLayer | A layer within ParallaxBackground with adjustable scroll speed for depth. | Use with ParallaxBackground for multi-layered effects (e.g., distant mountains). |
| Particles2D | Emits 2D particles for visual effects using CPU processing. | Use for effects like smoke, fire, or explosions in 2D games (less performant than GPU alternatives). |
| Path2D | Defines a 2D curve or path for objects to follow. | Use for predefined movement (e.g., enemy patrol routes or a moving platform). |
| PathFollow2D | Moves along a Path2D at a set speed or offset. | Use with Path2D for smooth movement (e.g., a train on tracks or a circling enemy). |
| PhysicalBone2D | A bone in a 2D skeleton that interacts with physics (e.g., ragdoll effects). | Use with Skeleton2D for physics-driven animations (e.g., a collapsing character). |
| PhysicsBody2D | Base class for 2D physics-enabled nodes (not used directly). | Parent class for KinematicBody2D, RigidBody2D, etc.; use its children instead. |
| PinJoint2D | A 2D physics joint that pins two bodies together at a point, allowing rotation. | Use for hinges or pivots (e.g., a swinging door or pendulum). |
| PointLight2D | A 2D light source emitting light from a point (like Light2D with specific behavior). | Use for omnidirectional lighting (e.g., a glowing orb or lamp in 2D). |
| Polygon2D | Draws a filled or outlined 2D polygon with customizable texture and color. | Use for custom 2D shapes (e.g., a star, shield, or irregular UI element). |
| RayCast2D | Casts a 2D ray to detect collisions with objects in a straight line. | Use for line-of-sight checks or aiming (e.g., a laser or enemy detection). |
| RemoteTransform2D | Applies its transform (position, rotation, scale) to another 2D node remotely. | Use to sync transforms between nodes (e.g., linking a camera to a player without parenting). |
| RigidBody2D | A 2D physics body affected by forces like gravity and collisions. | Use for objects with realistic physics (e.g., falling boxes, bouncing balls). |
| Skeleton2D | A hierarchy of Bone2D nodes for 2D skeletal animation. | Use for rigging and animating 2D characters (e.g., a humanoid with moving limbs). |
| Sprite2D | Displays a single 2D texture or image in the scene. | Use for static visuals (e.g., backgrounds, icons, or non-animated objects like a tree). |
| StaticBody2D | A 2D physics body that doesn't move (fixed in place). | Use for immovable level elements (e.g., walls, platforms). |
| TileMap | Creates grids of tiles from a tileset for efficient level design. | Use for building levels or maps (e.g., platformer floors, walls, or repeating backgrounds). |
| TouchScreenButton | A 2D button that responds to touch input, using a texture for visuals. | Use for mobile game controls (e.g., a virtual joystick or attack button). |
| VisibleOnScreenNotifier2D | Emits signals when it enters or exits the screen boundaries. | Use to optimize or trigger events (e.g., spawn enemies only when visible). |

3D Nodes
--------

These nodes operate in a 3D coordinate system (X, Y, Z) for 3D games.

| **Node** | **What It Is** | **When to Use** |
| --- | --- | --- |
| Node3D | Base 3D node with position, rotation, and scale in 3D space. | Use as a root or parent for any 3D object (e.g., grouping models or lights). |
| AnimatedSprite3D | Plays a sequence of 2D images as a billboard or flat object in 3D space. | Use for animated billboards (e.g., a flickering sign or NPC name tag in 3D). |
| Area3D | A 3D region that detects collisions or overlaps without physics simulation. | Use for triggers in 3D (e.g., entering a room, picking up an item). |
| AudioStreamPlayer3D | Plays positional audio in 3D space with attenuation and directionality. | Use for spatial sound effects (e.g., a roaring engine or echoing footsteps in 3D). |
| BoneAttachment3D | Attaches a node to a specific bone in a 3D skeleton for dynamic positioning. | Use with Skeleton3D to attach objects (e.g., a sword to a character's hand). |
| Camera3D | Controls the 3D viewport, defining the player's perspective (e.g., FOV, projection). | Use to view the 3D world (e.g., first-person or third-person camera). |
| CharacterBody3D | A 3D physics body for manual control with built-in movement helpers (replaces KinematicBody3D). | Use for player characters or NPCs with custom movement (e.g., FPS or third-person character). |
| CollisionObject3D | Base class for 3D nodes with collision capabilities (not used directly). | Parent class for Area3D, RigidBody3D, etc.; use its children instead. |
| CollisionPolygon3D | Defines a polygon-shaped collision area in 3D (used for debugging or simple shapes). | Use with physics nodes for custom 3D hitboxes (less common; prefer CollisionShape3D). |
| CollisionShape3D | Defines a 3D shape (e.g., sphere, box) for collision detection. | Pair with Area3D or physics bodies for hitboxes (e.g., a player's collision volume). |
| ConeTwistJoint3D | A 3D physics joint that allows rotation within a cone-shaped constraint. | Use for joints with limited rotation (e.g., a robotic arm or swinging lamp). |
| CSGBox3D | A 3D box shape for constructive solid geometry (CSG) operations. | Use for quick prototyping or level design (e.g., carving out a room). |
| CSGCombiner3D | Combines multiple CSG shapes with union, subtraction, or intersection operations. | Use to build complex 3D geometry from simple shapes (e.g., a hollowed-out structure). |
| CSGCylinder3D | A 3D cylinder shape for CSG operations. | Use for cylindrical objects in prototyping (e.g., a pillar or pipe). |
| CSGMesh3D | Uses an external mesh for CSG operations in 3D. | Use to integrate custom models into CSG (e.g., subtracting a sculpted shape). |
| CSGPolygon3D | A 3D polygon extruded into a shape for CSG operations. | Use for custom extruded shapes (e.g., a star-shaped wall). |
| CSGShape3D | Base class for CSG nodes (not used directly). | Parent class for specific CSG shapes; use its children instead. |
| CSGSphere3D | A 3D sphere shape for CSG operations. | Use for spherical objects in prototyping (e.g., a dome or planet). |
| CSGTorus3D | A 3D torus (donut) shape for CSG operations. | Use for ring-shaped objects (e.g., a decorative arch or tire). |
| Decal | Projects a texture onto 3D surfaces for detail (e.g., bullet holes, stains). | Use for surface details without modifying geometry (e.g., graffiti on a wall). |
| DirectionalLight3D | Simulates sunlight with parallel rays and shadows in 3D. | Use for outdoor lighting (e.g., a sun or moon in an open-world game). |
| FogVolume | Defines a 3D volume with fog effects for atmosphere. | Use for localized fog (e.g., a misty valley or smoky room). |
| Generic6DOFJoint3D | A 3D physics joint with six degrees of freedom, fully customizable. | Use for complex joints (e.g., a fully articulated robotic limb). |
| GPUParticles3D | Emits 3D particles using GPU acceleration for high-performance effects. | Use for complex 3D effects (e.g., rain, fire, or magic spells). |
| HingeJoint3D | A 3D physics joint that allows rotation around a single axis (like a hinge). | Use for doors, levers, or wheels (e.g., a swinging gate). |
| ImporterMeshInstance3D | A temporary node for imported 3D meshes during scene processing. | Used internally by Godot's import system; convert to MeshInstance3D for general use. |
| Joint3D | Base class for 3D physics joints (not used directly). | Parent class for specific joints like HingeJoint3D; use its children instead. |
| Label3D | Displays text in 3D space, often as a billboard or fixed orientation. | Use for floating text (e.g., damage numbers, NPC dialogue in 3D). |
| Light3D | Base class for 3D light nodes (not used directly). | Parent class for DirectionalLight3D, OmniLight3D, etc.; use its children instead. |
| MeshInstance3D | Displays a 3D mesh (geometry) in the scene with materials and transforms. | Use for rendering 3D models (e.g., characters, vehicles, or terrain). |
| MultiMeshInstance3D | Renders multiple instances of a 3D mesh efficiently using instancing. | Use for performance-heavy scenes (e.g., a forest of trees or crowd of enemies). |
| NavigationAgent3D | Handles pathfinding for 3D navigation using a NavigationRegion3D. | Use for AI movement (e.g., enemies chasing the player in 3D space). |
| NavigationObstacle3D | Defines a dynamic obstacle that affects 3D navigation. | Use for moving obstacles in pathfinding (e.g., a rolling boulder or patrolling guard). |
| NavigationRegion3D | A region that enables 3D pathfinding using a navigation mesh. | Use for AI navigation in 3D (e.g., a 3D dungeon or open terrain). |
| OccluderInstance3D | Defines a shape that occludes objects for optimization or effects (e.g., culling). | Use to improve performance by hiding unseen geometry (e.g., behind a large wall). |
| OmniLight3D | Emits light in all directions from a point (like a bulb) in 3D. | Use for local lighting (e.g., a lamp or torch in a room). |
| Path3D | Defines a 3D curve or path for objects to follow. | Use for predefined movement (e.g., a rollercoaster track or enemy patrol in 3D). |
| PathFollow3D | Moves along a Path3D at a set speed or offset. | Use with Path3D for smooth movement (e.g., a car on a track or orbiting satellite). |
| PhysicsBody3D | Base class for 3D physics-enabled nodes (not used directly). | Parent class for CharacterBody3D, RigidBody3D, etc.; use its children instead. |
| PinJoint3D | A 3D physics joint that pins two bodies together at a point, allowing rotation. | Use for hinges or pivots (e.g., a swinging bridge or pendulum in 3D). |
| RayCast3D | Casts a 3D ray to detect collisions with objects in a straight line. | Use for line-of-sight or aiming (e.g., a gun's targeting or enemy vision cone). |
| ReflectionProbe | Captures the environment for real-time reflections in 3D. | Use for reflective surfaces (e.g., a shiny floor or water puddle). |
| RemoteTransform3D | Applies its transform (position, rotation, scale) to another 3D node remotely. | Use to sync transforms between nodes (e.g., linking a camera to a player without parenting). |
| RigidBody3D | A 3D physics body affected by forces like gravity and collisions. | Use for objects with realistic physics (e.g., rolling barrels, falling debris). |
| Skeleton3D | A hierarchy of bones for 3D skeletal animation. | Use for rigging and animating 3D characters (e.g., a humanoid with moving limbs). |
| SkeletonIK3D | Applies inverse kinematics (IK) to a Skeleton3D for realistic posing. | Use for dynamic animations (e.g., a hand reaching for an object in 3D). |
| SliderJoint3D | A 3D physics joint that constrains a body to slide along an axis. | Use for sliding mechanics (e.g., a piston or elevator in 3D). |
| SoftBody3D | A deformable 3D physics body that simulates soft materials. | Use for cloth, jelly, or rubber-like objects (e.g., a bouncing blob or waving flag). |
| SpotLight3D | Emits light in a cone shape (like a flashlight) in 3D. | Use for focused lighting (e.g., a car headlight or spotlight). |
| Sprite3D | Displays a 2D texture in 3D space, often as a billboard facing the camera. | Use for flat visuals in 3D (e.g., signs, decals, or floating icons). |
| StaticBody3D | A 3D physics body that doesn't move (fixed in place). | Use for immovable objects (e.g., walls, floors in a 3D level). |
| VehicleBody3D | A specialized RigidBody3D for simulating vehicles with wheels. | Use for cars, tanks, or bikes (e.g., a drivable car in a racing game). |
| VehicleWheel3D | A wheel component for VehicleBody3D with suspension and friction properties. | Use with VehicleBody3D for vehicle physics (e.g., a car's front wheel). |
| VisibleOnScreenNotifier3D | Emits signals when its 3D bounding box enters or exits the screen. | Use to optimize or trigger events (e.g., spawn enemies only when visible in 3D). |
| VoxelGI | Provides real-time global illumination using voxel-based techniques in 3D. | Use for dynamic lighting in enclosed spaces (e.g., a lit cave or room with soft shadows). |

Shared Nodes (2D and 3D)
------------------------

These nodes work in both 2D and 3D contexts, often depending on their parent or configuration.

| **Node** | **What It Is** | **When to Use** |
| --- | --- | --- |
| Node | The generic base node with no specific functionality, only basic properties like name. | Use as a basic container or for scripting logic without spatial properties (e.g., game manager). |
| AnimationPlayer | Animates properties of any node (position, scale, opacity, etc.) over time. | Use for complex animations (e.g., a door opening, UI fading, or character movement). |
| AnimationTree | Manages complex animation blending and state machines for nodes. | Use for advanced animation control (e.g., blending walk/run cycles or state-based character animations). |
| AudioListener2D / AudioListener3D | Defines the point that hears spatial audio (2D or 3D variant). | Use to set the audio perspective (e.g., tied to the player or camera for positional sound). |
| AudioStreamPlayer | Plays non-positional audio (mono or stereo, not spatial). | Use for background music or UI sounds (e.g., a game soundtrack or button click). |
| CanvasItem | Base class for 2D rendering nodes (not used directly). | Parent class for Node2D, Control, etc.; use its children instead. |
| Control | Base class for UI nodes (not used directly, but 2D-based). | Parent class for UI elements like Button, Label; use its children for UI design. |
| GeometryInstance3D | Base class for 3D nodes that render geometry (not used directly). | Parent class for MeshInstance3D, CSGShape3D, etc.; use its children instead. |
| InstancePlaceholder | A placeholder for an instanced scene that hasn't been fully loaded yet. | Used internally when instancing scenes; replace with actual nodes during runtime. |
| Marker2D / Marker3D | A simple node marking a position in 2D or 3D space with no visuals. | Use for reference points (e.g., spawn locations, waypoints, or pivot points). |
| MissingNode | A placeholder for a node type that's missing (e.g., from an older version). | Appears when loading incompatible scenes; replace with valid nodes. |
| Timer | Counts down a specified time and emits a signal when done. | Use for delays or timed events (e.g., respawn timer, attack cooldown). |
| Tween | Interpolates properties of nodes smoothly over time (replaces older Tween node in Godot 4). | Use for simple animations or transitions (e.g., fading a sprite, moving an object smoothly). |

UI Nodes (2D-Based)
-------------------

These are Control-derived nodes primarily for 2D UI, but they can be used in 3D via CanvasLayer.

| **Node** | **What It Is** | **When to Use** |
| --- | --- | --- |
| Button | A clickable button with text or texture. | Use for basic UI interaction (e.g., start game, options menu). |
| CheckBox | A toggle button with a checkbox style. | Use for on/off settings (e.g., mute audio option). |
| CheckButton | A toggle button with a custom texture style. | Use for stylized toggles (e.g., a power switch in a themed UI). |
| ColorRect | A rectangular area filled with a solid color. | Use for backgrounds or simple UI shapes (e.g., a health bar background). |
| Container | Base class for layout nodes (not used directly). | Parent class for HBoxContainer, VBoxContainer, etc.; use its children for layouts. |
| HBoxContainer | Arranges child nodes horizontally in a row. | Use for horizontal UI layouts (e.g., a row of buttons). |
| VBoxContainer | Arranges child nodes vertically in a column. | Use for vertical UI layouts (e.g., a settings menu list). |
| GridContainer | Arranges child nodes in a grid layout. | Use for grid-based UI (e.g., an inventory screen). |
| FlowContainer | Arranges child nodes in a flow layout (horizontal or vertical wrapping). | Use for dynamic lists (e.g., a gallery of items that wraps to the next line). |
| HSplitContainer | Splits two child nodes horizontally with a draggable divider. | Use for resizable panels (e.g., a split-screen editor UI). |
| VSplitContainer | Splits two child nodes vertically with a draggable divider. | Use for resizable vertical panels (e.g., a top/bottom UI split). |
| Label | Displays static text in the UI. | Use for text display (e.g., scores, instructions). |
| LineEdit | A single-line text input field. | Use for user input (e.g., entering a player name). |
| MenuButton | A button that opens a popup menu when clicked. | Use for dropdown menus (e.g., an options selector). |
| OptionButton | A dropdown menu for selecting one option from a list. | Use for single-choice selections (e.g., difficulty level picker). |
| Panel | A rectangular panel with a customizable style for UI backgrounds. | Use for UI containers (e.g., a dialog box background). |
| PanelContainer | A container that applies a panel style to its single child node. | Use to style a single UI element (e.g., a framed button). |
| ProgressBar | A bar that shows progress (e.g., health, loading). | Use for progress indicators (e.g., a health bar or download bar). |
| RichTextLabel | Displays formatted text with BBCode for styling (e.g., bold, colors). | Use for complex text (e.g., a styled story or chat log). |
| ScrollContainer | A container with scrollbars for content larger than its size. | Use for scrollable lists (e.g., a long menu or chat window). |
| Slider | Base class for sliders (not used directly). | Parent class for HSlider, VSlider; use its children for adjustable values. |
| HSlider | A horizontal slider for selecting a value within a range. | Use for horizontal value adjustment (e.g., volume control). |
| VSlider | A vertical slider for selecting a value within a range. | Use for vertical value adjustment (e.g., brightness control). |
| SpinBox | A numeric input field with up/down arrows. | Use for precise numeric input (e.g., setting a timer value). |
| TabContainer | A container with tabs to switch between child nodes. | Use for tabbed interfaces (e.g., an options menu with multiple categories). |
| TextEdit | A multi-line text input field for editing larger text. | Use for text editing (e.g., a note-taking feature or code editor). |
| TextureButton | A button with customizable textures for different states (e.g., normal, pressed). | Use for stylized buttons (e.g., a graphical play button). |
| TextureProgressBar | A progress bar with custom textures instead of a solid fill. | Use for styled progress (e.g., a health bar with a custom graphic). |
| TextureRect | Displays a texture in the UI with stretching or tiling options. | Use for UI images (e.g., a logo or background graphic). |
| Tree | A hierarchical list with expandable items (like a file explorer). | Use for tree-like data (e.g., a skill tree or folder structure). |

* * * * *

Tips for Using Nodes
--------------------

-   **Hierarchy**: Start with Node2D or Node3D as your scene root, then add specialized nodes as children.
-   **Physics**: Choose CharacterBody3D/KinematicBody2D for controlled movement, RigidBody for physics-driven objects, and StaticBody for fixed scenery.
-   **Rendering**: Use Sprite2D/Sprite3D for flat visuals, AnimatedSprite for animations, and MeshInstance3D for 3D models.
-   **Effects**: Leverage Particles2D/GPUParticles3D for dynamic effects and Light2D/OmniLight3D for lighting.
-   **UI**: Build interfaces with Control nodes under a CanvasLayer for 2D or 3D games.
-   **Navigation**: Use NavigationRegion2D/NavigationRegion3D with agents for AI pathfinding.

