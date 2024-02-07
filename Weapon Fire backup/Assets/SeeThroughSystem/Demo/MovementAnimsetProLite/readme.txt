Movement Animset Pro Lite v.1.15
--------------------


This is a sample of Movement Animset Pro - a complete pack of motion capture animations, to build a seamless third person perspective character movement for your game. The animations are universal, so you can use them for any setting you like - SciFi, Fantasy etc.


V.1.15 changes:
--------------------
- changed the rig to Humanoid
- added MECANIM graph
- added example Scene showing how to do different stopping motions

V.1.1 changes:
--------------------
- Tweaked WalkFwdLoop, made lenght of steps more even
- Tweaked transition animations to fit the WalkFwdLoop
--------------------


It consists of 8 animations:


Idle
WalkFwdLoop
WalkFwdStart
WalkFwdStop_LU
WalkFwdStop_RU
JumpIdleStart
JumpIdleLand
ButtonPush_RH

--------------------

To get full animation set, consisting of over 120 animations, get full version of Movement Animset Pro at Unity Asset Store!
It covers:

- Standing
- Walking
- Running
- Crouching / Sneaking
- Jumping
- Falling
- Interactions (button pushing, picking up objects etc.)

It also contains:
- Skinned model of a modern day soldier (26,536 tris, 13,813 verts)
- Skinned model of a dummy, to preview animations


--------------------
KNOWN ISSUES:

- MECANIM takes Root Transform Position (XZ) from both Axis at the same time, when using Humanoid Rig. It takes the transform from the Hips (pelvis) bone and not the Root bone, like it should be. Therefore, all animations, that have pelvis go outside the Center Of Mass (for example crouching, or swaying when stopping), will appear like the feet are sliding, when instead the pelvis should be moving and the feet should be stable. This occurs ONLY in MECANIM Humanoid retarget, Generic animations look properly.

Few ways to go around this:
1) Use Generic rig
2) If the animations doesn't need a Root Motion in local X Axis (like strafing), Make a constraint on X axis, so it doesn't move.
3) Use IK feet stabilisation

A simple solution to that problem would be separating Axis of Root Transform Position per animation by Unity (so not XZ together, but optionally X or/and Z)


--------------------
Created by Kubold
kuboldgames@gmail.com
http://www.kubold.com
https://www.facebook.com/kuboldgames