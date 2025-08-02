# Polymorphism for Unity

Low ceremony package for working with polymorphic types in the unity editor

## Goals

- Non-invasive property drawers
      - Use standard `PropertyDrawer` attributes wherever possible; use custom generic types where not
      - Do not replace standard editors/inspectors
- Well documented and composable set of UI elements to aid in supporting additional polymorphic data structures
- Take advantage of new editor functionality as it becomes available (when they reduce the amount of abstraction needed)
     - Notably Unity 6.0 adds `applyToCollection` to the PropertyAtrribute class which will allow PropertyAttributes to be added to arrays and lists 
- Support for a range of useful generic data-structures
    - Predictable performance characteristics for these types
   - Standard serialization mechanisms
- Support for generic types in the custom type selection dropdown
- Support for generic ScriptableObjects using Roslyn codegen
- Support for complete feature-set from Unity version 2019.3 and above


## Non-Goals
- IMGUI suppport will be added when trivial, but this is a secondary consideration
- A library of Odin/Tri-Inspector style attributes to customise the editor is a non-goal
     - While convenient, this style of editor extension is invasive in that it replaces the default editor and so it does not play nicely with other editor extensions

## Notes

This package is a work in progress. 
Please check back later for v0.1