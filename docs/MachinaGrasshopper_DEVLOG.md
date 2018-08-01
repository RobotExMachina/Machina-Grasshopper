```text
//  ███╗   ███╗ █████╗  ██████╗██╗  ██╗██╗███╗   ██╗ █████╗        ██████╗ ██╗  ██╗
//  ████╗ ████║██╔══██╗██╔════╝██║  ██║██║████╗  ██║██╔══██╗      ██╔════╝ ██║  ██║
//  ██╔████╔██║███████║██║     ███████║██║██╔██╗ ██║███████║█████╗██║  ███╗███████║
//  ██║╚██╔╝██║██╔══██║██║     ██╔══██║██║██║╚██╗██║██╔══██║╚════╝██║   ██║██╔══██║
//  ██║ ╚═╝ ██║██║  ██║╚██████╗██║  ██║██║██║ ╚████║██║  ██║      ╚██████╔╝██║  ██║
//  ╚═╝     ╚═╝╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝       ╚═════╝ ╚═╝  ╚═╝

```

# TODO
- [ ] Wrap Machina types as parameters deriving from GH_Goo
- [ ] Watch Long's videos... ;) http://developer.rhino3d.com/videos/
- [ ] Add `ExternalAxes/To`


# REVISIONS
---
# v0.6.4
- [x] Deprecate `SendToBridge`, to be replaced by a new set of dedicated `Connect/Send/Reas` components.


---
# v0.6.3

## BUILD 1403
- [x] Add safe program name check to avoid
- [x] IOs can now be named with `string`
- [x] Remove `Robot.SetIOName()`
- [x] Add `toolPin` option for IOs (UR robots)


---
# v0.6.2

## BUILD 1402
- [x] Add `ToInstruction()` override to Action:
    Generates a string representing a "serialized" instruction representing the Machina-API command that would have generated this action. Useful for generating actions to send to the Bridge.

---
# v0.6.1

## BUILD 1401
- [x] `Speed/To` can now be a `double`
- [x] `Precision/To` can now be a `double`
- [x] Tweaks to ABB compiler to accept the above.
- [x] Add `Acceleration` and `AccelerationTo` actions: add to Actions, Cursor, Settings
  - [x] Add acceleration params to UR compiler
  - [x] Acceleration values of zero or less reset it back to the robot's default.
- [x] Add `RotationSpeed/To()` option
  - [x] Add ABB correct compilation with defaults
  - [x] Add UR warning compilation message
- [x] Add `JointSpeed/To()` and `JointAcceleration/To()` for UR robots
  - [x] Add UR compilation
  - [x] Add ABB compilation warnings
- [x] UR streaming

---
## v.0.5.0
- [x] Update Machina core to 0.5.0
- [x] Hide most pre-0.5.0 components
- [x] Remove all obsolete components, and create new ones with GUID to avoid overwrite
- [x] Make components have the option to choose between abs/rel
- [x] Rename `Motion` to `MotionMode`
- [x] Rename `FeedRate` to `ExtrusionRate`
- [x] ICONS!! :)
- [x] Add GH_MutableComponent middleware class
- [x] Split component classes per file
- [x] Generate new components with new GuiD
- [x] Rename `CreateRobot` to `NewRobot` , same for `NewTool`
- [x] Mutable components now accept default values
- [x] Add namespaces to components
- [x] Add `Temperature`
- [x] Add `Extrude`
- [x] Add `ExtrusionRate`
- [x] Update sample files


## v.0.4.1
- [x] Add 3D Printing sample
- [x] Rename and migrate `Zone` to `Precision`
- [x] Rename components and IOs
    + [x] Component: name and nickname will be the same, and identical to the core Machina function. The purpose here is to get users acquaintance with the Machina syntax, and hopefuly transition smoothly to it.
    + [x] Input name: description of _what_ it is: Point, Vector, Plane... or descriptive words like Direction, Axis, Angle, etc.
    + [x] Input nickname: one or two-letter initials of input name. Components are rather simple and with little inputs, we probably don't need to force long nicknames and can match GH's style
    + [x] Outputs: same
- [x] Think of levels and categories to subdivide Actions tab in
- [x] Add sample files

## v0.4.0
- [x] Improve sample files
- [x] Add Robot.SetIOName()
- [x] Add "human comments" option to compile component
- [x] Add IOs
