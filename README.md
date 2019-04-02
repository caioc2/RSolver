# RSolver
An experiment with Rubiks cube 2x2 and 3x3 solver using deep reinforcement learning in Unity with [ML-Agents](https://github.com/Unity-Technologies/ml-agents). It's my first attempt with RL and those experiments helped me understanding the theory and practice.

The base repository was forked from https://github.com/stuartsoft/RSolver which implements a deterministic solver.

From this base we use the cube visual modeling and rotation operations with some modifications. In general all the code was reworked/optimized, we removed the deterministic solver and implemented everything else to be used with DRL.

Among the changes made are:
* Changed actions representation.
* Implementation of anti-clockwise X' actions instead of using three times XXX equivalent.
* Reworked the code to work with both 3x3 original and the new implementation  of 2x2 cube.
* Reworked the animation to fit the DRL implementation
* Implemented everything else about DRL to be used with [ML-Agents](https://github.com/Unity-Technologies/ml-agents)

## Results

After training for about 3 hours on a notebook CPU we are able to solve the 2x2 cube with any scramble depth with average solve length of ~26 moves, within a day of training we were able to achieve an average solve length of ~17 moves which is close to the gods number for 2x2 cube which is 14 quarter turn moves.

![2x2 cube result](/docs/images/2x2.png)

The artificial neural network used was pretty simple, 2 hidden layers with 512 units each. More details about training procedures can be found in the following text.

## Usage

First you need to clone this repository and also the [ML-Agents](https://github.com/Unity-Technologies/ml-agents). Next you need copy the files
`ML-Agents/UnitySDK/Assets/*` to `RSolver/Assets/`.

You can train the 2x2 cube with the scene `RubiksCube2` and the 3x3 with the `RubiksCube3` following the [https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Training-ML-Agents.md](documentation of ML-Agents for training). Other than that the agents (cubes in the scene) have adjustable parameters, they are:

* `Num Scramble` - The starting number of scramble applied to the cube
* `Min Solve Rate` - Minimum solve rate to increase the `Num Scramble`
* `interval` - Number of cubes used to calculate the `Min Solve Rate` parameter
* `Max Move Rate` - Maximum of the average rate of moves (moves/num scramble) used to solve the cube before increasing the `Num Scramble`
* `Animated` - Animate the cube rotations. Must be used with `On Demand Decisions`.

For 2x2 only:

* `Mask Action` - Reduce the state space/actions of the cube by fixing one of the cubelets and letting the others be rotated.

About the cube drawing animation:

* `Draw cubes` - Draw cubes, you can disable the drawing while training.
* `Rotation time` - Time in seconds needed to make a 90 degrees rotation


Finally there is another scene `RubiksCube2PerfTest` which you can run to collect statistics of performance for each scramble depth of a trained model like solve rate, median moves, average moves, maximum and minimum moves.



