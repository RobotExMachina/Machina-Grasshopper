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

# REVISIONS
## v.0.5.0
- [x] Update Machina core to 0.5.0
- [x] Hide all pre-0.5.0 components
- [x] Add GH_MutableComponent middleware class
- [ ] Split component classes per file
- [ ] Generate new components with new GuiD, incorporating the abs/rel option 



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
