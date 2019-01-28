using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Solver {

    RubiksCube RCube;

    public Solver(RubiksCube RC)
    {
        RCube = RC;
    }

    public List<RubiksCube.move> Solution()
    {
        RCube.clearTurnRecord();

        //reorient the cube so that white is on top
        if (RCube.cubeMatrix[1][2][1].getColor(Cube.sides.TOP) != Cube.colorEnum.WHITE){
            if (RCube.cubeMatrix[1][2][1].getColor(Cube.sides.TOP) == Cube.colorEnum.YELLOW){
                RCube.turnCubeZ(true);
                RCube.turnCubeZ(true);
            }
            else{
                while(RCube.cubeMatrix[1][1][0].getColor(Cube.sides.TOP) != Cube.colorEnum.WHITE)
                {RCube.turnCubeY(true);}//turn the cube until the front is white
                RCube.turnCubeX(false);
            }
        }
        
        Stage2();
        Stage3();
        RCube.turnCubeZ(true);
        RCube.turnCubeZ(true);
        Stage4();
        Stage5();
        Stage6();
        RCube.turnCubeZ(true);
        RCube.turnCubeZ(true);
        return RCube.turnRecord;
    }

    public List<RubiksCube.move> SearchedSolution()
    {
        if (RCube.isSolved())
            return new List<RubiksCube.move>();//cube is already solved. Quit

        //check for easy solutions

        //check if checkerboard pattern solves it
        RubiksCube tempRC = RCube.cloneCube();
        tempRC.RunSequence(10);
        if (tempRC.isSolved())
            return RCube.sequences[10];

        //check if dot patter solves it
        tempRC = RCube.cloneCube();
        List<RubiksCube.move> tempsol = new List<RubiksCube.move>();
        for (int i = 0; i < 3; i++)
        {
            tempRC.RunSequence(11);
            tempsol.AddRange(RCube.sequences[11]);
            if (tempRC.isSolved())
                return tempsol;
        }


        RCube.clearTurnRecord();

        //reorient the cube so that white is on top
        if (RCube.cubeMatrix[1][2][1].getColor(Cube.sides.TOP) != Cube.colorEnum.WHITE)
        {
            if (RCube.cubeMatrix[1][2][1].getColor(Cube.sides.TOP) == Cube.colorEnum.YELLOW)
            {
                RCube.turnCubeZ(true);
                RCube.turnCubeZ(true);
            }
            else
            {
                while (RCube.cubeMatrix[1][1][0].getColor(Cube.sides.TOP) != Cube.colorEnum.WHITE)
                { RCube.turnCubeY(true); }//turn the cube until the front is white
                RCube.turnCubeX(false);
            }
        }

        //run initial dfs check to maybe find easy solutions in 4 turns or less
        List<RubiksCube> dfsTree = new List<RubiksCube>();
        for (int i = 0; i < 3; i++)
        {
            DFS_InitialCheck(0, i, RCube.cloneCube(), dfsTree);
            for (int j = 0; j < dfsTree.Count; j++)
            {
                if (dfsTree[j].isSolved())
                    return dfsTree[j].turnRecord;
            }
        }
        
        //begin running actual solution with intermediate dfs optimization

        dfsTree = new List<RubiksCube>();
        DFS_Stage2(0, null, RCube.cloneCube(), dfsTree);

        int minCost = 9999;
        int minCostIndex = 0;
        for (int i = 0; i < dfsTree.Count; i++)
        {
            int cost = dfsTree[i].TurnRecordTokenCount();
            if (cost < minCost)
            {
                minCost = cost;
                minCostIndex = i;
            }
        }

        RCube = dfsTree[minCostIndex].cloneCube();

        //start stage 3 DFS

        dfsTree = new List<RubiksCube>();
        DFS_Stage3(0, null, RCube.cloneCube(), dfsTree);

        minCost = 9999;
        minCostIndex = 0;
        for (int i = 0; i < dfsTree.Count; i++){
            int cost = dfsTree[i].TurnRecordTokenCount();
            if (cost < minCost)
            {
                minCost = cost;
                minCostIndex = i;
            }
        }

        RCube = dfsTree[minCostIndex].cloneCube();

        //flip cube

        RCube.turnCubeZ(true);
        RCube.turnCubeZ(true);

        //start stage 4 DFS

        dfsTree = new List<RubiksCube>();
        DFS_Stage4(0, null, RCube.cloneCube(), dfsTree);

        minCost = 9999;
        minCostIndex = 0;
        for (int i = 0; i < dfsTree.Count; i++)
        {
            int cost = dfsTree[i].TurnRecordTokenCount();
            if (cost < minCost)
            {
                minCost = cost;
                minCostIndex = i;
            }
        }

        RCube = dfsTree[minCostIndex].cloneCube();

        Stage5();
        Stage6();
        RCube.turnCubeZ(true);
        RCube.turnCubeZ(true);

        return RCube.turnRecord;
    }

    void DFS_Stage2(int depth, List<Cube.colorEnum>RemainingColors, RubiksCube parent, List<RubiksCube> tree)
    {
        //Debug.Log("Depth: " + depth);
        if (depth == 0)//first iteration
        {
            RemainingColors = new List<Cube.colorEnum>();
            RemainingColors.Add(Cube.colorEnum.RED);
            RemainingColors.Add(Cube.colorEnum.GREEN);
            RemainingColors.Add(Cube.colorEnum.ORANGE);
            RemainingColors.Add(Cube.colorEnum.BLUE);
        }
        
        for (int i =0;i< RemainingColors.Count; i++)
        {
            RubiksCube tempRC = parent.cloneCube();
            tempRC.turnCubeToFaceRGBOColorWithYellowOrWhiteOnTop(RemainingColors[i]);
            SolveWhiteSideCube(tempRC);
            List<Cube.colorEnum> newRemainingColors = new List<Cube.colorEnum>();
            for (int j = 0; j < RemainingColors.Count; j++) { newRemainingColors.Add(RemainingColors[j]); }
            newRemainingColors.RemoveAt(i);

            if (newRemainingColors.Count == 0){
                tree.Add(tempRC);
                return;
            }
            else
                DFS_Stage2(depth + 1, newRemainingColors, tempRC,tree);

        }
        return;
    }

    void DFS_Stage3(int depth, List<Cube.colorEnum> RemainingColors, RubiksCube parent, List<RubiksCube> tree)
    {
        //Debug.Log("Depth: " + depth);
        if (depth == 0)//first iteration
        {
            RemainingColors = new List<Cube.colorEnum>();
            RemainingColors.Add(Cube.colorEnum.RED);
            RemainingColors.Add(Cube.colorEnum.GREEN);
            RemainingColors.Add(Cube.colorEnum.ORANGE);
            RemainingColors.Add(Cube.colorEnum.BLUE);
        }

        for (int i = 0; i < RemainingColors.Count; i++)
        {
            RubiksCube tempRC = parent.cloneCube();
            tempRC.turnCubeToFaceRGBOColorWithYellowOrWhiteOnTop(RemainingColors[i]);
            SolveWhiteCornerCube(tempRC);
            List<Cube.colorEnum> newRemainingColors = new List<Cube.colorEnum>();
            for (int j = 0; j < RemainingColors.Count; j++) { newRemainingColors.Add(RemainingColors[j]); }
            newRemainingColors.RemoveAt(i);

            if (newRemainingColors.Count == 0)
            {
                tree.Add(tempRC);
                return;
            }
            else
                DFS_Stage3(depth + 1, newRemainingColors, tempRC, tree);

        }
        return;
    }

    void DFS_Stage4(int depth, List<Cube.colorEnum> RemainingColors, RubiksCube parent, List<RubiksCube> tree)
    {
        //Debug.Log("Depth: " + depth);
        if (depth == 0)//first iteration
        {
            RemainingColors = new List<Cube.colorEnum>();
            RemainingColors.Add(Cube.colorEnum.RED);
            RemainingColors.Add(Cube.colorEnum.GREEN);
            RemainingColors.Add(Cube.colorEnum.ORANGE);
            RemainingColors.Add(Cube.colorEnum.BLUE);
        }

        for (int i = 0; i < RemainingColors.Count; i++)
        {
            RubiksCube tempRC = parent.cloneCube();
            tempRC.turnCubeToFaceRGBOColorWithYellowOrWhiteOnTop(RemainingColors[i]);
            SolveMiddleRowSideCubes(tempRC);
            List<Cube.colorEnum> newRemainingColors = new List<Cube.colorEnum>();
            for (int j = 0; j < RemainingColors.Count; j++) { newRemainingColors.Add(RemainingColors[j]); }
            newRemainingColors.RemoveAt(i);

            if (newRemainingColors.Count == 0)
            {
                tree.Add(tempRC);
                return;
            }
            else
                DFS_Stage4(depth + 1, newRemainingColors, tempRC, tree);

        }
        return;
    }

    void DFS_InitialCheck(int depth, int depthLimit, RubiksCube parent, List<RubiksCube> tree)
    {
        List<RubiksCube.move> operations = new List<RubiksCube.move> { RubiksCube.move.RIGHT, RubiksCube.move.LEFT, RubiksCube.move.TOP, RubiksCube.move.BOTTOM, RubiksCube.move.FRONT, RubiksCube.move.BACK };//each operation also has an inverse
        for (int i = 0; i < operations.Count; i++)
        {
            RubiksCube tempRC = parent.cloneCube();
            tempRC.RunCustomSequence(operations[i]);
            if (depth == depthLimit)
                tree.Add(tempRC);
            else
                DFS_InitialCheck(depth + 1, depthLimit, tempRC, tree);

            RubiksCube tempRC2 = parent.cloneCube();
            tempRC2.RunCustomSequence(RubiksCube.getInverse(operations[i]));
            if (depth == depthLimit)
                tree.Add(tempRC2);
            else
                DFS_InitialCheck(depth + 1, depthLimit, tempRC2, tree);
        }
    }

    public List<RubiksCube.move> trimTurnRecord(List<RubiksCube.move> sol)
    {
        bool changesMade = true;

        int num = 0;

        while (changesMade)
        {
            changesMade = false;
            List<RubiksCube.move> tempSol = new List<RubiksCube.move>();
            RubiksCube.move tokA, tokB, tokC;
            for (int i = 0; i < sol.Count; i ++)
            {
                tokA = sol[i];
                tokB = i + 1 < sol.Count ? sol[i + 1] : RubiksCube.move.X;
                tokC = i + 2 < sol.Count ? sol[i + 2] : RubiksCube.move.X;

                if (RubiksCube.getInverse(tokA) == tokB)
                {//if token A is invers of the next token
                    changesMade = true;
                    i++;
                }
                else if (tokA == tokB && tokA == tokC)
                {
                    changesMade = true;
                    i += 2;
                    tempSol.Add(RubiksCube.getInverse(tokA));
                }
                else//append the token to the tempsol
                {
                    tempSol.Add(tokA);
                }
            }
            sol = tempSol;
            num++;
        }

        return sol;
    }

   /* string getTokenFromSolution(string sol, int i)
    {
        string result = "";
        if (i < sol.Length)
        {
            result += sol[i];
            if (i + 1 < sol.Length && sol[i + 1] == 'i')
                result += 'i';
        }

        return result;
    }

    string inverseToken(string tok)
    {
        if (tok.Length == 2)//is inverted
            return tok[0].ToString();
        else
            return tok[0].ToString() + "i";

    }

    bool isTokenInverseOfToken(string tokA, string tokB)
    {
        return (tokA.Length > 0 && tokB.Length > 0 && tokA[0] == tokB[0] && tokA.Length != tokB.Length);
    }*/

    void Stage2()//Solve the white cross
    {

        for (int i = 0; i < 4; i++)
        {
            SolveWhiteSideCube(RCube);
            RCube.turnCubeY(true);
        }
    }

    void SolveWhiteSideCube(RubiksCube RC)
    {
        Cube.colorEnum TargetColor = RC.cubeMatrix[1][1][0].getColor(Cube.sides.FRONT);
        //Debug.Log("TargetColor: " + TargetColor);
        Vector3 Pos = RC.sideCubeWithColors(TargetColor, Cube.colorEnum.WHITE);
        //Debug.Log("Pos: " + Pos);
        Vector3 TargetPos = new Vector3(1, 2, 0);
        //Debug.Log("TargetPos: " + TargetPos);
        if (TargetPos != Pos)//if the cube is not where it should be
        {
            if (Pos.y == 2)//is on top row
            {
                if (Pos.x == 0)
                {
                    RC.rotateLeftFace(true);
                }
                else if (Pos.x == 2)
                {
                    RC.rotateRightFace(false);
                }
                else if (Pos.x == 1)
                {
                    RC.rotateBackFace(true);
                    RC.rotateBackFace(true);
                }

            }
            else if (Pos.y == 1 && Pos.z == 2)//middle row on the back
            {
                if (Pos.x == 0)
                {
                    RC.rotateBackFace(true);
                    RC.rotateBottomFace(true);
                    RC.rotateBackFace(false);
                }
                if (Pos.x == 2)
                {
                    RC.rotateBackFace(false);
                    RC.rotateBottomFace(true);
                    RC.rotateBackFace(true);
                }
            }

            //cube is now on bottom or front
            //requery pos
            Pos = RC.sideCubeWithColors(TargetColor, Cube.colorEnum.WHITE);
            if (Pos.y == 0)//is on bottom
            {
                while (Pos.z != 0)//while cube is not on the bottom
                {
                    RC.rotateBottomFace(true);
                    Pos = RC.sideCubeWithColors(TargetColor, Cube.colorEnum.WHITE);
                }
            }
            while (Pos.y != 2)
            {
                RC.rotateFrontFace(true);
                Pos = RC.sideCubeWithColors(TargetColor, Cube.colorEnum.WHITE);
            }
        }
        //cube is now where it should be
        Cube temp = RC.cubeMatrix[(int)TargetPos.x][(int)TargetPos.y][(int)TargetPos.z];

        if (temp.getColor(Cube.sides.TOP) != Cube.colorEnum.WHITE)
        {
            RC.turnCubeY(true);
            RC.RunSequence(0);
            RC.turnCubeY(false);
        }
    }

    void Stage3()//sove the white corners
    {
        for (int i = 0; i < 4; i++)
        {
            SolveWhiteCornerCube(RCube);
            RCube.turnCubeY(true);
        }
    }

    void SolveWhiteCornerCube(RubiksCube RC)
    {
        Cube.colorEnum TargetColorA = RC.cubeMatrix[1][1][0].getColor(Cube.sides.FRONT);//front center
        Cube.colorEnum TargetColorB = RC.cubeMatrix[2][1][1].getColor(Cube.sides.RIGHT);//right center
                                                                                  //Debug.Log("TargetColors: " + TargetColorA + TargetColorB);
        Vector3 Pos = RC.cornerCubeWithColors(TargetColorA, TargetColorB, Cube.colorEnum.WHITE);
        //Debug.Log("Pos: " + Pos);
        Vector3 TargetPos = new Vector3(2, 2, 0);
        //Debug.Log("TargetPos: " + TargetPos);

        if (TargetPos != Pos)//not in the right spot
        {
            if (Pos.y == 2)//in the top but in the wrong spot
            {
                if (Pos.z == 0)//top front left pos
                {
                    RC.rotateLeftFace(true);
                    RC.rotateBottomFace(true);
                    RC.rotateLeftFace(false);
                }
                if (Pos.z == 2)//top back left or top back right
                {
                    if (Pos.x == 0)
                    {
                        RC.rotateLeftFace(false);
                        RC.rotateBottomFace(true);
                        RC.rotateLeftFace(true);
                    }
                    if (Pos.x == 2)
                    {
                        RC.rotateRightFace(true);
                        RC.rotateBottomFace(true);
                        RC.rotateRightFace(false);
                    }
                }
            }
            //cube is now on the bottom
            Pos = RC.cornerCubeWithColors(TargetColorA, TargetColorB, Cube.colorEnum.WHITE);
            while (Pos != new Vector3(2, 0, 0))
            {
                RC.rotateBottomFace(true);
                Pos = RC.cornerCubeWithColors(TargetColorA, TargetColorB, Cube.colorEnum.WHITE);
            }
            //cube should now be directly below it's target position or already in target position
        }

        while (true)
        {
            Pos = RC.cornerCubeWithColors(TargetColorA, TargetColorB, Cube.colorEnum.WHITE);
            Cube tempCube = RC.cubeMatrix[(int)Pos.x][(int)Pos.y][(int)Pos.z];
            if (Pos == TargetPos && tempCube.getColor(Cube.sides.FRONT) == TargetColorA && tempCube.getColor(Cube.sides.TOP) == Cube.colorEnum.WHITE)
                break;

            RC.RunSequence(1);
        }
        
    }

    void Stage4()//solve the middle row side cubes...
    {
        for (int i = 0; i < 4; i++) {
            SolveMiddleRowSideCubes(RCube);
            RCube.turnCubeY(true);
        }

    }

    void SolveMiddleRowSideCubes(RubiksCube RC)
    {
        Cube.colorEnum TargetColorA = RC.cubeMatrix[1][1][0].getColor(Cube.sides.FRONT);//front center
        Cube.colorEnum TargetColorB = RC.cubeMatrix[2][1][1].getColor(Cube.sides.RIGHT);//right center
                                                                                  //Debug.Log("TargetColors: " + TargetColorA + TargetColorB);
        Vector3 Pos = RC.sideCubeWithColors(TargetColorA, TargetColorB);
        //Debug.Log("Pos: " + Pos);
        Vector3 TargetPos = new Vector3(2, 1, 0);
        //Debug.Log("TargetPos: " + TargetPos);

        if (Pos != TargetPos || RC.cubeMatrix[(int)TargetPos.x][(int)TargetPos.y][(int)TargetPos.z].getColor(Cube.sides.FRONT) != TargetColorA)
        {//cube is not in the right place or is oriented wrong
            if (Pos.y != 2)//not in the top
            {
                if (Pos.z == 0)//front
                {
                    if (Pos.x == 0)//front left middle row
                        RC.RunSequence(3);//force the cube out of 0,1,0
                    else if (Pos.x == 2)//front right middle row
                        RC.RunSequence(2);//force the cube out of 2,1,0
                }
                else
                {
                    RC.turnCubeY(true);
                    RC.turnCubeY(true);

                    if (Pos.x == 0)//front left middle row
                        RC.RunSequence(2);//force the cube out of 0,1,0
                    else if (Pos.x == 2)//front right middle row
                        RC.RunSequence(3);//force the cube out of 2,1,0

                    RC.turnCubeY(true);
                    RC.turnCubeY(true);
                }
            }

            //cube is now guaranteed to be in top row
            Pos = RC.sideCubeWithColors(TargetColorA, TargetColorB);
            while (Pos != new Vector3(1, 2, 0))
            {
                RC.rotateTopFace(true);
                Pos = RC.sideCubeWithColors(TargetColorA, TargetColorB);
            }

            //cube is now in 1,2,0

            if (RC.cubeMatrix[(int)Pos.x][(int)Pos.y][(int)Pos.z].getColor(Cube.sides.FRONT) != TargetColorA)
            {
                RC.rotateTopFace(false);
                RC.turnCubeY(false);
                RC.RunSequence(3);
                RC.turnCubeY(true);
            }
            else
            {
                RC.RunSequence(2);
            }
        }

        //cube is now in the target pos and is oriented correctly

    }

    void Stage5()//solve the yellow side
    {
        while (!RCube.isYellowCrossOnTop())
        {
            for (int i= 0; i < 4; i++)
            {
                if (RCube.isYellowBackwardsLOnTop())
                    break;
                if (RCube.isYellowHorizontalLineOnTop())
                    break;

                RCube.rotateTopFace(true);
            }
            //cube is now oriented with either a yellow L or horizontal line or just a yellow center

            if (RCube.isYellowHorizontalLineOnTop())
                RCube.RunSequence(5);
            else
                RCube.RunSequence(4);
        }

        //cube now has a yellow cross on top

        while (!RCube.isTopAllYellow())
        {
            int state = 0;

            for (int i = 0; i < 4; i++)
            {
                Cube topleftback = RCube.cubeMatrix[0][2][2];
                Cube toprightback = RCube.cubeMatrix[2][2][2];

                if (topleftback.getColor(Cube.sides.TOP) == Cube.colorEnum.YELLOW && toprightback.getColor(Cube.sides.TOP) == Cube.colorEnum.YELLOW)
                {
                    state = 3;
                    break;
                }
                RCube.turnCubeY(true);
            }

            if (state == 0)//no two adjacent yellow sides were found
            {
                for (int i = 0; i < 4; i++)
                {
                    Cube topleftfront = RCube.cubeMatrix[0][2][0];

                    if (topleftfront.getColor(Cube.sides.TOP) == Cube.colorEnum.YELLOW)
                    {
                        state = 2;
                        break;
                    }
                    RCube.turnCubeY(true);
                }
            }

            if (state == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Cube topleftfront = RCube.cubeMatrix[0][2][0];

                    if (topleftfront.getColor(Cube.sides.LEFT) == Cube.colorEnum.YELLOW)
                    {
                        state = 1;
                        break;
                    }
                    RCube.turnCubeY(true);
                }
            }

            //cube is now oriented and ready for sequence
            RCube.RunSequence(6);
        }
    }

    void Stage6()//solve the rest
    {
        bool AllCornersCorrect = false;

        //checkif all corners are solved in any orientaion of the top face
        for (int i = 0; i < 4; i++){
            if (RCube.allTopCornersSolved()){
                AllCornersCorrect = true;
                break;
            }
            RCube.rotateTopFace(true);
        }

        while (!AllCornersCorrect)
        {

            bool foundTwoAdjacentMatchingCorners = false;
            //try to find to adjacent corners with the same color and place them in the back
            for (int i = 0; i < 4; i++)//checking for adjacent corners
            {
                Cube.colorEnum TopFrontLeft = RCube.cubeMatrix[0][2][0].getColor(Cube.sides.FRONT);
                Cube.colorEnum TopFrontRight = RCube.cubeMatrix[2][2][0].getColor(Cube.sides.FRONT);
                if (TopFrontLeft == TopFrontRight)
                {
                    foundTwoAdjacentMatchingCorners = true;
                    break;
                }
                RCube.turnCubeY(true);
            }

            if (foundTwoAdjacentMatchingCorners)
            {
                Cube.colorEnum TopFrontLeft = RCube.cubeMatrix[0][2][0].getColor(Cube.sides.FRONT);
                Cube.colorEnum MiddleFrontCenter = RCube.cubeMatrix[1][1][0].getColor(Cube.sides.FRONT);

                while (TopFrontLeft != MiddleFrontCenter)
                {
                    RCube.rotateTopFace(true);
                    RCube.turnCubeY(true);
                    TopFrontLeft = RCube.cubeMatrix[0][2][0].getColor(Cube.sides.FRONT);
                    MiddleFrontCenter = RCube.cubeMatrix[1][1][0].getColor(Cube.sides.FRONT);
                }

                RCube.turnCubeY(true);
                RCube.turnCubeY(true);
            }

            //wheather or not we found two corners on the same side, the cube should be in the right orientation now to run the sequence.
            RCube.RunSequence(7);

            //checkif all corners are solved in any orientaion of the top face
            for (int i = 0; i < 4; i++)
            {
                if (RCube.allTopCornersSolved())
                {
                    AllCornersCorrect = true;
                    break;
                }
                RCube.rotateTopFace(true);
            }
        }

        //all yellow corners are now in the right spot
        //now to fix the top side pieces

        while (!RCube.isSolved())
        {
            for (int i = 0; i < 4; i++)
            {
                Cube.colorEnum TopFrontLeft = RCube.cubeMatrix[0][2][0].getColor(Cube.sides.FRONT);
                Cube.colorEnum TopFrontMiddle = RCube.cubeMatrix[1][2][0].getColor(Cube.sides.FRONT);

                if (TopFrontLeft == TopFrontMiddle)
                {
                    RCube.turnCubeY(true);
                    RCube.turnCubeY(true);
                    break;
                }
                RCube.turnCubeY(true);
            }

            //we should now have either a matching face on the back or there are not matching faces
            //now run the last sequence

            Cube.colorEnum TFM = RCube.cubeMatrix[1][2][0].getColor(Cube.sides.FRONT);
            Cube.colorEnum MRM = RCube.cubeMatrix[2][1][1].getColor(Cube.sides.RIGHT);

            if (TFM == MRM)
            {
                RCube.RunSequence(9);
            }
            else
                RCube.RunSequence(8);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}

}
