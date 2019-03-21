# RSolver
An experiment with Rubiks cube 2x2 and 3x3 solver using deep reinforcement learning in Unity with ML-Agents.

The base repo was forked from https://github.com/stuartsoft/RSolver which implements a deterministic solver.

From this base we use the cube visual modeling and rotation operations. All the code was reworked/optmized, we removed the deterministic solver and implemented everything else to be used with DRL.

Among the changes made are:
* Representation of actions and colors as ENUM
* Implementation of anti-clockwise X' actions which were used as XXX or X3
* Reworked the code to work with both 3x3 and 2x2 cubes
* Reworked the animation to fit the DRL implementation
* Implemented everything else about DRL
