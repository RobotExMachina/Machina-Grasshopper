```text
//  ███╗   ███╗ █████╗  ██████╗██╗  ██╗██╗███╗   ██╗ █████╗        ██████╗ ██╗  ██╗
//  ████╗ ████║██╔══██╗██╔════╝██║  ██║██║████╗  ██║██╔══██╗      ██╔════╝ ██║  ██║
//  ██╔████╔██║███████║██║     ███████║██║██╔██╗ ██║███████║█████╗██║  ███╗███████║
//  ██║╚██╔╝██║██╔══██║██║     ██╔══██║██║██║╚██╗██║██╔══██║╚════╝██║   ██║██╔══██║
//  ██║ ╚═╝ ██║██║  ██║╚██████╗██║  ██║██║██║ ╚████║██║  ██║      ╚██████╔╝██║  ██║
//  ╚═╝     ╚═╝╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝       ╚═════╝ ╚═╝  ╚═╝

```

# WISHLIST
- [ ] Wrap Machina types as parameters deriving from GH_Goo
- [ ] Watch Long's videos... ;) http://developer.rhino3d.com/videos/
- [ ] Add option to listener to skip messages
- [ ] Add option to listen only to whatever we are subscribed to
- [ ] Add some kind of direct text-based instruction, like `Do("foo")`



# REVISIONS
# v0.8.12
- Updated `Do` to `Issue`
- 

# v0.8.8
- Core update
- Fixes to `Logger`

# v0.8.4
- Update core with bug fixes
- Add `ArmAngle`
- `ExternalAxis` now accepts a third parameter as the target for this action: cartesian targets, joint targets or all.


# v0.8.2
- Update sample files

# v0.8.1
- Deprecate `JointSpeed`, `JointAcceleration` and `RotationSpeed`.
- Add `DefineTool`
- Fixed `AttachTool` and `DetachTool`
- `ActionExecuted` is now replacing `ActionCompleted` and `ExecutionUpdate`
- Add `ActionReleased` and `ActionIssued` listeners
- Add `MotionUpdate` listener
- Add `Logger`

---
# v.0.7.1
## BUILD 1412
- `Compile` now uses a new `Robot` instance to avoid inheriting robot states.
- Massive refactoring.
- New icons (thanks [Hakim](https://github.com/HakimHasan)!)


# v0.7.0
## BUILD 1411
- Improved `Listen` and event listerners, much cleaner "API"
- Add `10_RealTime3DPrinting` sample.


## BUILD 1408
- First stub at `Listen` component with full data ins --> Need to improve outputs to be persistent unless value changes: https://discourse.mcneel.com/t/how-to-trigger-updates-down-only-selected-outputs-of-component/68441
- Fix `ExternalAxis` component


---
# v0.6.4

## BUILD 1407
- [x] Deprecate `SendToBridge`, to be replaced by a new set of dedicated `Connect/Send/Read` components.
- [x] Improve `GH_MutableComponent` architecture to accept optional input parameters
- [x] Add `ExternalAxes/To`
- [x] Split `SendToBridge` into `Connect` and `Send`
- [x] Change `ExternalAxes/To` to new core `ExternalAxis/To`
- [x] Add `CustomCode` component
- [x] Improved socket reliability



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
