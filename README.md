# RSolver

![](/docs/images/rc.png)
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

After training for about 3 hours on a notebook CPU we were able to solve the 2x2 cube with any scramble depth with average solve length of ~26 moves, within a day of training we were able to achieve an average solve length of ~17 moves which is close to the gods number for 2x2 cube which is 14 quarter turn moves.

![2x2 cube result](/docs/images/2x2.png)

The artificial neural network used was pretty simple, 2 hidden layers with 512 units each. More details about training procedures can be found in the following text. You can test the DRL solver online [here](https://caioc2.github.io/RSolver/game/).

[![Video of the game.](http://img.youtube.com/vi/TfZp3LCC-uM/0.jpg)](http://www.youtube.com/watch?v=TfZp3LCC-uM)

## Usage

First you need to clone this repository and also the [ML-Agents](https://github.com/Unity-Technologies/ml-agents)(we use the 0.7 version). Next you need to copy the files from
`ML-Agents/UnitySDK/Assets/*` to `RSolver/Assets/`.

You can train the 2x2 cube with the scene `RubiksCube2` and the 3x3 with the `RubiksCube3` following the [documentation of ML-Agents for training](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Training-ML-Agents.md). Other than that the agents (cubes in the scene) have adjustable parameters which are not implemented in the original ML-Agents agent, they are:

* `Num Scramble` - The starting number of scramble applied to the cube
* `Min Solve Rate` - Minimum solve rate to increase the `Num Scramble`
* `interval` - Number of episodes used to calculate the `Min Solve Rate` parameter
* `Max Move Rate` - Maximum of the average rate of moves (moves/num scramble) used to solve the cube before increasing the `Num Scramble`
* `Animated` - Animate the cube rotations. Must be used with `On Demand Decisions`.

For 2x2 only:

* `Mask Action` - Reduce the state space/actions of the cube by fixing one of the cubelets and letting the others be rotated.

About the cube drawing animation:

* `Draw cubes` - Draw cubes, you can disable the drawing while training.
* `Rotation time` - Time in seconds needed to make a 90 degrees rotation


Finally there is another scene `RubiksCube2PerfTest` which you can run to collect statistics of performance for each scramble depth of a trained model like solve rate, median moves, average moves, maximum and minimum moves.

# The Rubik's Cube Problem

The 2x2 cube is far easier than its 3x3 counterpart. Its state space is approx. 3.6e6 while the 3x3 is aprox. 4.3e19, nevertheless it still an interesting puzzle. Various challenges are presented by these puzzles, among them are:

* Discrete action and state space
* Unknown direct algorithm to optimally solve it, yet there are algorithms using search and other techniques to solve it optimally or very close to it.
* Compact representation of state space is hard, for example our representation of 2x2 space is 2^144, which is approx. 2.2e43 while the state space is much smaller 3.6e6.
* Sparse reward system. It's not trivial to evaluate how good or close to the solution one configuration is. Given that positive reward is only assigned when the cube is solved.

These make it a hard problem, which still needs a good solution. One of the best know solutions using RL is [Solving the Rubik's Cube Without Human Knowledge](https://arxiv.org/abs/1805.07470) which uses a neural network to guide an informed search using Monte Carlo Tree Search(MCTS) and Breadth First Search(BFS). As much as it is interesting by being able to solve almost every 3x3 cube with results close to other search algorithms using various Rubik's cube domain knowledge and group theory, it stills disappointing in one sense: It needs search algorithms after the training, consuming a lot of time and exploration before reaching the solution. Even that the MCTS explore far less states than the traditional search algorithms like DFS and BFS, the need for using it together with BFS points that maybe the search is doing the hard work and not the RL itself.

One attempt of reproducing the results  of [Solving the Rubik's Cube Without Human Knowledge](https://arxiv.org/abs/1805.07470) was made in [Reinforcement Learning to solve Rubik’s cube (and other complex problems!)](https://medium.com/datadriveninvestor/reinforcement-learning-to-solve-rubiks-cube-and-other-complex-problems-106424cf26ff), yet the results presented by the original authors couldn't be reproduced, be it by lack of implementation details in the original paper, by capping the allowed search time/nodes or less training than the original.
So far the later also tried the method on 2x2 cubes and it couldn't consistently solve all of the 2x2 cubes. Its results using MCTS would need 50 moves or more, and using MCTS + BFS around 14 moves. Again it reinforces the idea of the search doing the hard work for solving the cube.

## Our Approach

Solving the cube can be think of 
> "At each step chose the move that decreases the number of following moves needed to get to the solved state".

As much as we don't know how many moves away one state is from solved, this thinking implies that: *Any cube state that is N moves away from the solution necessarily needs to pass through states which are (N-1), (N-2),..., 1 moves away from the solution*.

This is a great thing, we can decompose our solve process backward and create a [learning curriculum](https://qmro.qmul.ac.uk/xmlui/bitstream/handle/123456789/15972/Bengio%2C%202009%20Curriculum%20Learning.pdf?sequence=1&isAllowed=y). The main idea is to teach the agent to solve cubes starting with 1 move away and increasing it till N moves. This solves the problem of sparse rewards, or not being able to give rewards/classify intermediate states until reaching the solution.

Our criteria for this curriculum were:
* Solve rate of the cubes with *k* moves away from solved.
* Solve length rate - *ck* moves needed to solve *k* scramble depth.

We don't know how to generate cubes with **exact** *k* moves away from solution, but applying a random sequence of *k* moves to a solved cube guarantees that this cube is *k* or less moves away from solved, which is good enough for our needs.

## Implementation

The most relevant for RL is the file `/RSolver/Assets/Scripts/RubiksCubeAgent.cs`. It implements the reward system *-0.01* every step and *+1* when solved and also our curriculum, which could not be done with the curriculum framework of ML-Agents. Yet, this file is extremely simple. Everything else is the Rubik's cube representation and animation in Unity.

So far this specific problem doesn't need anything special from Unity, like physics simulation or other complex things, but it reflects a problem that is challenging and a simple way to overcome it (At least in its 2x2 form).

## Experiments

For our experiments we used:

|Program  |Version   |     |Hardware |Model      |     |Parameters    |Value      |
|--------:|---------:|----:|--------:|----------:|----:|-------------:|----------:|
|ML-Agents|0.7       |     |CPU      |i7 7700QH  |     |Learning Rate |5e-5       |
|Unity    |2018.1.9f2|     |GPU      |GTX 1060 6G|     |Batch Size    |32         |
|Windows  |10.0.17134|     |RAM      |16GB       |     |Epoch         |5          |
|         |          |     |         |           |     |Episode Length|100        |
|         |          |     |         |           |     |Interval      |500        |

*All training were made using CPU only, for our network and batch size we hadn't enough data to flood the GPU, hence using CPU alone was faster.*

From our tests using a smaller batch size (32) helps in the convergence and faster completion of the curriculum. Larger batch sizes tends to "average" too much the agent experience and not learning anything at all, given that both our state space and actions are discrete and the representation of two states with one move away from each other can be pretty different.

The learning rate initially could be greater, but after the initial adjustment of the network weights it would make the the training unstable as we want to add knowledge and not replace it with the curriculum.

Another important parameter is the agent `maximum episode length`. We set it to `100` steps per episode. It may sounds inefficient to let an agent do 100 steps while it was for example one move away from the solution, but it is really important to explore both bad and good moves, lowering it made the learning slow down. Last but not least, the `interval` in which is calculated the statistics and increased (or not) the curriculum has some effects on the result. Smaller (50) finishes the curriculum faster but tends to need more moves than larger (500) interval after the same number of iterations. This is mostly because the curriculum was increased before the agent truly completing it because of high variance in the statistics used and being stuck in a sub-optimum.

### 2x2 Cube

The following figure shows the effect of different network layouts in the episode length.

Number of units per layer   |  Number of layers
:--------------------------:|:-------------------------:
![](/docs/images/ep-len.png)|![](/docs/images/ep-len2.png)

For both figures, lower values indicates better result (less steps to solve the cube). The last peak in the episode length indicates the completion of the curriculum, equivalently after that point any cube would be solved consistently. All networks were able to solve the 2x2 cube.
In the left we see the comparison of the number of units per layer using 2 layers. Clearly increasing the number of units improves the training. In the right there is the comparison of the number of layers using 512 units per layer. It also decreases the final episode length but takes more time to complete the curriculum with 8 layers.


The result of training using the aforementioned parameters after 1e7 iterations can be seen in the following figure. Up to 5 moves away from the solution it can solve with the same move count. After that it increases faster than the scramble depth but stabilizes in ~17 moves for scramble depth > 20.
![2x2 cube result](/docs/images/2x2.png)
### 3x3 Cube

*In progress*
