# GDPlotter
is a lightweight math plot library.

## Overview
The project consists of the following parts, in ascending order of importance:
  - WFA_example
    - A working WF-based program which allows a user to input a piece of code representing a Func<double, double> and have the function rendered on screen, in an overly interactive window
  - Core/Drawing/Pixelmap32 + ARGB
    - A wrapper of System.Drawing.Bitmap which allows fast R/W access to single pixels
  - Core/RuntimeCompiler
    - A minimalistic mechanism which compiles (user input) code to an invokable Func<double, double> delegate during runtime
  - Core/Rendering
    - A small library of methods which handle transformation logic neccessary for correct rendering of mathematical primitives
  - Controllers/_PlotController
    - The main reason why this repo exists
    - Manages the current state of UI
    - Handles user input
    - Provides rendering methods for various plot primitives
   
## Details
### _PlotController
Most of this will make more sense once you fiddle around with the WFA example for 20 seconds.

_PlotController fulfills the following roles:
  - Tracks ControlArea and PlotArea
    - ControlArea represents a region in the target element to which we shall render
    - PlotArea represents a region of a cartesian plane which we shall render
  - Provides support for several modes of user interaction
    - Zoom
      - Zooms in/out the ControlArea or the PlotArea rectangle
        - either in real time by continuous updates of the most recent pair of points of interaction
          - ZoomMode property
          - ?_ActivateZoomMode
          - UpdateZoomModeCoord (or UpdateCoords which fixates the first point)
          - DeactivateZoomMode
        - or discretely, by a direct call
          - ?_?_Zoom methods
    - Shift
      - Shifts the ControlArea or the PlotArea rectangle
        - Either in real time by continuous updates of the most recent point of interaction
          - ShiftMode property
          - ?_ActivateShiftMode
          - UpdateShiftModeCoord (or UpdateCoords)
          - DeactivateShiftMode
        - Or discretely, by a direct call
          - ?_?_Shift methods
    - Select
      - Either tracks a selection preview of the ControlArea or the PlotArea rectangle in real time based on update of the most recent point of interaction
        - SelectionMode property
        - ?_ActivateSelectionMode
        - UpdateSelectionModeCoord (or UpdateCoords)
        - RefreshSelection
        - DeactivateSelectionMode
      - Or selects a new ControlArea or PlotArea by a direct call, according to a fixed pair of points
        - ?_?_Select
  - Provides methods to render the plot primitives
    - RefreshAxes
    - RefreshGrid
    - RefreshFrame
    - RefreshPlotBoundCoords
    - RefreshFx in the case of FxPlotController
  - Tracks parameters used to render the plot primitives
    - Grid spacing
    - Coordinate value rendering parameters
  - Tracks rendering flags for
    - The x and y axes
    - A coordinate grid
    - Control frame
    - Coordinate values displayed near ControlArea bounds
    - A selection mode preview rectangle
    - The Func<double, double> to plot in the case of FxPlotController
### Rendering
Is a singular static class, mostly self-explanatory
### RuntimeCompiler
Ditto
### Pixelmap32
Is a functional extension of a regular bitmap. It turns out that System.Drawing.Graphics is faster than what's possible by looping over memory when it comes to anything more complex than R/W of a singular pixel, and so Pixelmap32 exposes that functionality as a 2D indexer + Lock/Unlock methods, alongside a GetGraphics for everything else.
Manual locking is also neccessary for parallelism. The order of bytes in ARGB matters.
### WFA_example
PlotDisplay is a reusable control (assuming anyone still uses WFA), the rest of the UI code is mostly stitched together to show off the controller.

UI logic for PlotDisplay:
  - Regular actions target PlotArea, shift-key modifier switches them to ControlArea
    - MouseDown activates a mode, MouseUp deactivates it, MouseMove updates coordinates
      - Left click commands SelectionMode
      - Right click commands ShiftMode
      - Middle click commands ZoomMode
        - ZoomMode requires an additional point, the capture of which is bound to any mouse button other than the three mentioned
    - Mouse wheel up/down performs zoom
    - WASD keys pad an area
    - IJKL keys shift an area
    - V flips horizontal orientation of an area, around mouse cursor
    - B flips vertical orientation of an area, around mouse cursor
- Esc aborts selection mode

## License

MIT License

Copyright (c) 2023 GateDigger

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
