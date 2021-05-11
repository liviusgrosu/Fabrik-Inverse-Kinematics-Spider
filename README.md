 <p float="left">
  <img src="https://raw.githubusercontent.com/liviusgrosu/Fabrik-Inverse-Kinematics-Spider/main/Pictures/diagram_4.png" width="450" height="420">
  <img src="https://raw.githubusercontent.com/liviusgrosu/Fabrik-Inverse-Kinematics-Spider/main/Pictures/diagram_5.png" width="450" height="420">
 </p>

# Inverse Kinematics Spider

##### Table of Contents  
* [Algorithm](#algorithm)
* [Controls](#controls)
## Algorithm:

The inverse kinemetics of each leg is calculated using the FABRIK algorithm. In general the starting point (root or leaf point) is placed
at its ancor point (root socket or goal point respectively), then the following points position is calculated based off a vector between itself and 
the previous point. The process goes from leaf to root and vice versa. The more iterations here are, the more accurate the arm placement to the
goal will be.

<p align="center">
 <img src="https://raw.githubusercontent.com/liviusgrosu/Fabrik-Inverse-Kinematics-Spider/main/Pictures/diagram_1.png">
</p>
The use of a pole also dictates the rotational arch of the arm itself. Each point is projected onto a plane that starts at the root node which
allows for the calcuation of minimal distance required to reach said pole. This is done so by drawing out a circle around each points projection
which then a simple distance function is used to determine that minimal distance to the previous point. After all this, it leaves us with a natural
arch of a limb that imitates closey to its real world counter part

<p align="center">
 <img src="https://raw.githubusercontent.com/liviusgrosu/Fabrik-Inverse-Kinematics-Spider/main/Pictures/diagram_2.png">
</p>

The target prediction is quite trivial where a vertical raycast with respect to the bodies rotation is casted in front to determine a sutable new
arm placement. The body also takes into consideration of the average height and rotation of each arm as well. This adds a little bit more realism to the overall
movement of the spider.

<p align="center">
 <img src="https://raw.githubusercontent.com/liviusgrosu/Fabrik-Inverse-Kinematics-Spider/main/Pictures/diagram_3.png">
</p>

## Controls:

| Actions            | Key                                                               |
| ------------------ | ----------------------------------------------------------------- |
| W                  | Move Forwards                                                     |
| S                  | Move Backwards                                                    |
| A                  | Move Leftwards                                                    |
| A                  | Move Rightwards                                                   |
| Q                  | Turn Counter Clockwise                                            |
| E                  | Turn Clockwise                                                    |
