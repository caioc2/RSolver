using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RubiksCube
{
    // assumes clockwise moves to have binary as ???0 and counter-clockwise to be ???1 comparing (moveA >> 1) == (moveACC >> 1)
    public enum move { RIGHT = 0, RIGHTCC = 1, LEFT = 2, LEFTCC = 3, TOP = 4, TOPCC = 5, BOTTOM = 6, BOTTOMCC = 7, FRONT = 8, FRONTCC = 9, BACK = 10, BACKCC = 11,
                       X = 12, XCC = 13,  Y = 14, YCC = 15, Z = 16, ZCC = 17, NOTHING = 18 };
    public static int moveCount = 19; //total move count
    public static int changeMoveCount = 12; //total moves which change cube configuration
    public List<List<List<Cube>>> cubeMatrix;

    public List<List<move>> sequences;
    public List<move> turnRecord;

    // Use this for initialization
    public RubiksCube()
    {
        sequences = new List<List<move>>();
        turnRecord = new List<move>();
        sequences.Add(stringToMove("RiUFiUi"));//fix middle in stage 2
        sequences.Add(stringToMove("RiDiRD"));//fix corner in stage 3
        sequences.Add(stringToMove("URUiRiUiFiUF"));
        sequences.Add(stringToMove("UiLiULUFUiFi"));
        sequences.Add(stringToMove("FURUiRiFi"));
        sequences.Add(stringToMove("FRURiUiFi"));
        sequences.Add(stringToMove("RURiURUURi"));
        sequences.Add(stringToMove("RiFRiBBRFiRiBBRRUi"));
        sequences.Add(stringToMove("FFULRiFFLiRUFF"));
        sequences.Add(stringToMove("FFUiLRiFFLiRUiFF"));

        sequences.Add(stringToMove("LLRRFFBBUUDD"));//checkerboard
        sequences.Add(stringToMove("UDiRLiFBiUDi"));//six spots*/
        /*sequences.Add(new List<move> { move.RIGHTCC, move.TOP, move.FRONTCC, move.TOPCC });//fix middle in stage 2
        sequences.Add(new List<move> { move.RIGHTCC, move.BOTTOMCC, move.RIGHT, move.BOTTOM });//fix corner in stage 3
        sequences.Add(new List<move> { move.TOP, move.RIGHT, move.TOPCC, move.RIGHTCC, move.TOPCC, move.FRONTCC, move.TOP, move.FRONT });
        sequences.Add(new List<move> { move.TOPCC, move.LEFTCC, move.TOP, move.LEFT, move.TOP, move.FRONT, move.TOPCC, move.FRONTCC });
        sequences.Add(new List<move> { move.FRONT, move.TOP, move.RIGHT, move.TOPCC, move.RIGHTCC, move.FRONTCC });
        sequences.Add(new List<move> { move.FRONT, move.RIGHT, move.TOP, move.RIGHTCC, move.TOPCC, move.FRONTCC });
        sequences.Add(new List<move> { move.RIGHT, move.TOP, move.RIGHTCC, move.TOP, move.RIGHT, move.TOP, move.TOP, move.RIGHTCC });
        sequences.Add(new List<move> { move.RIGHTCC, move.FRONT, move.RIGHTCC, move.BACK, move.BACK, move.RIGHT, move.FRONTCC, move.RIGHTCC, move.BACK, move.BACK, move.RIGHT, move.RIGHT, move.TOPCC });
        sequences.Add(new List<move> { move.FRONT, move.FRONT, move.TOP, move.LEFT, move.RIGHTCC, move.FRONT, move.FRONT, move.LEFTCC, move.RIGHT, move.TOP, move.FRONT, move.FRONT });
        sequences.Add(new List<move> { move.FRONT, move.FRONT, move.TOPCC, move.LEFT, move.RIGHTCC, move.FRONT, move.FRONT, move.LEFTCC, move.RIGHT, move.TOPCC, move.FRONT, move.FRONT });

        sequences.Add(new List<move> { move.LEFT, move.LEFT, move.RIGHT, move.RIGHT, move.FRONT, move.FRONT, move.BACK, move.BACK, move.TOP, move.TOP, move.BOTTOM, move.BOTTOM });//checkerboard
        sequences.Add(new List<move> { move.TOP, move.BOTTOMCC, move.RIGHT, move.LEFTCC, move.FRONT, move.BACKCC, move.TOP, move.BOTTOMCC });//six spots*/

        cubeMatrix = new List<List<List<Cube>>>();

        for (int x = 0; x < 3; x++)
        {
            List<List<Cube>> CubeRow = new List<List<Cube>>();
            for (int y = 0; y < 3; y++)
            {
                List<Cube> CubeColumn = new List<Cube>();
                for (int z = 0; z < 3; z++)
                {
                    Cube tempcube = new Cube();
                    tempcube.setAllSideColors(Cube.colorEnum.BLACK);
                    CubeColumn.Add(tempcube);
                }
                CubeRow.Add(CubeColumn);
            }
            cubeMatrix.Add(CubeRow);
        }
        //transform.rotation = Quaternion.Euler(new Vector3(45,0,45));
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cubeMatrix[i][j][0].setSideColor(Cube.sides.FRONT, Cube.colorEnum.RED);
                cubeMatrix[2][i][j].setSideColor(Cube.sides.RIGHT, Cube.colorEnum.BLUE);
                cubeMatrix[0][i][j].setSideColor(Cube.sides.LEFT, Cube.colorEnum.GREEN);
                cubeMatrix[i][j][2].setSideColor(Cube.sides.BACK, Cube.colorEnum.ORANGE);
                cubeMatrix[i][2][j].setSideColor(Cube.sides.TOP, Cube.colorEnum.WHITE);
                cubeMatrix[i][0][j].setSideColor(Cube.sides.BOTTOM, Cube.colorEnum.YELLOW);
            }
        }
        cubeMatrix[1][1][1].setAllSideColors(Cube.colorEnum.BLACK);
    }

    public List<move> stringToMove(string seq)
    {
        List<move> m = new List<move>();
        int i = 0;
        while(i < seq.Length)
        {
            char c = seq[i];
            if(c == 'R')
            {
                m.Add((i + 1 < seq.Length && seq[i + 1] == 'i') ? move.RIGHTCC : move.RIGHT);
            }else if(c == 'L')
            {
                m.Add((i + 1 < seq.Length && seq[i + 1] == 'i') ? move.LEFTCC : move.LEFT);
            }
            else if(c == 'U')
            {
                m.Add((i + 1 < seq.Length && seq[i + 1] == 'i') ? move.TOPCC : move.TOP);
            }
            else if(c == 'D')
            {
                m.Add((i + 1 < seq.Length && seq[i + 1] == 'i') ? move.BOTTOMCC : move.BOTTOM);
            }
            else if(c == 'B')
            {
                m.Add((i + 1 < seq.Length && seq[i + 1] == 'i') ? move.BACKCC : move.BACK);
            }
            else if(c == 'F')
            {
                m.Add((i + 1 < seq.Length && seq[i + 1] == 'i') ? move.FRONTCC : move.FRONT);
            }
            i++;
        }
        return m;
    }

    public static move getInverse(move a)
    {
        return (move)((uint)a ^ 1);

    }

    public static bool isClockwise(move a)
    {
        return (((uint)a & 1) == 0);
    }

    public List<List<Cube>> getCubeXYFace(int r, bool reference)
    {
        List<List<Cube>> face = new List<List<Cube>>();
        for (int i = 0; i < 3; i++)
        {
            List<Cube> row = new List<Cube>();
            for (int j = 0; j < 3; j++)
            {
                if (reference)
                    row.Add(cubeMatrix[i][j][r]);
                else
                {
                    Cube tempcube = new Cube(cubeMatrix[i][j][r].getColors());
                    row.Add(tempcube);
                }
            }
            face.Add(row);
        }

        return face;
    }

    public List<List<Cube>> getCubeYZFace(int r, bool reference)
    {
        List<List<Cube>> face = new List<List<Cube>>();
        for (int i = 0; i < 3; i++)
        {
            List<Cube> row = new List<Cube>();
            for (int j = 0; j < 3; j++)
            {
                if (reference)
                    row.Add(cubeMatrix[r][i][j]);
                else
                {
                    Cube tempcube = new Cube(cubeMatrix[r][i][j].getColors());
                    row.Add(tempcube);
                }
            }
            face.Add(row);
        }

        return face;
    }

    public List<List<Cube>> getCubeXZFace(int r, bool reference)
    {
        List<List<Cube>> face = new List<List<Cube>>();
        for (int i = 0; i < 3; i++)
        {
            List<Cube> row = new List<Cube>();
            for (int j = 0; j < 3; j++)
            {
                if (reference)
                    row.Add(cubeMatrix[i][r][j]);
                else
                {
                    Cube tempcube = new Cube(cubeMatrix[i][r][j].getColors());
                    row.Add(tempcube);
                }
            }
            face.Add(row);
        }

        return face;
    }

    List<Cube> getOutline(List<List<Cube>> face)
    {
        //converts a 2d matrix of cubes to a linear list of the outline cubes of the provided face
        //the list is ordered in a clockwise direction, starting with the lower left cube [0][0]
        //getOutline returns a list of references to the origional matrix
        List<Cube> outline = new List<Cube>();
        for (int i = 0; i < 3; i++)
        {
            outline.Add(face[0][i]);
        }
        outline.Add(face[1][2]);
        for (int i = 2; i >= 0; i--)
        {
            outline.Add(face[2][i]);
        }
        outline.Add(face[1][0]);

        return outline;
    }

    public void rotateFrontFace(bool clockwise, bool record = true)
    {
        int iterations = 1;
        if (!clockwise) iterations = 3;
        for (int j = 0; j < iterations; j++)
        {
            List<Cube> oldFrontOutline = getOutline(getCubeXYFace(0, false));
            List<Cube> currentFrontOutline = getOutline(getCubeXYFace(0, true));

            for (int i = 0; i < 8; i++)
            {
                currentFrontOutline[(i + 2) % 8].setSideColors(oldFrontOutline[i].getColors());
                currentFrontOutline[(i + 2) % 8].rotateZ();
            }
        }

        if (record)
        {
            turnRecord.Add((clockwise ? move.FRONT : move.FRONTCC));
        }

    }

    void rotateMiddleXYFace(bool clockwise)//clockwise is relative to front face in this case
    {
        int iterations = 1;
        if (!clockwise) iterations = 3;
        for (int j = 0; j < iterations; j++)
        {
            List<Cube> oldFrontOutline = getOutline(getCubeXYFace(1, false));
            List<Cube> currentFrontOutline = getOutline(getCubeXYFace(1, true));

            for (int i = 0; i < 8; i++)
            {
                currentFrontOutline[(i + 2) % 8].setSideColors(oldFrontOutline[i].getColors());
                currentFrontOutline[(i + 2) % 8].rotateZ();

            }
        }
    }

    public void rotateBackFace(bool clockwise, bool record = true)
    {
        int iterations = 1;
        if (!clockwise) iterations = 3;
        for (int j = 0; j < iterations; j++)
        {
            List<Cube> oldFrontOutline = getOutline(getCubeXYFace(2, false));
            List<Cube> currentFrontOutline = getOutline(getCubeXYFace(2, true));

            for (int i = 0; i < 8; i++)
            {
                currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
                currentFrontOutline[i].rotateZ();
                currentFrontOutline[i].rotateZ();
                currentFrontOutline[i].rotateZ();
            }
        }

        if (record)
        {
            turnRecord.Add((clockwise ? move.BACK : move.BACKCC));
        }
    }

    public void rotateRightFace(bool clockwise, bool record = true)
    {
        int iterations = 1;
        if (!clockwise) iterations = 3;
        for (int j = 0; j < iterations; j++)
        {
            List<Cube> oldFrontOutline = getOutline(getCubeYZFace(2, false));
            List<Cube> currentFrontOutline = getOutline(getCubeYZFace(2, true));

            for (int i = 0; i < 8; i++)
            {
                currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
                currentFrontOutline[i].rotateX();
            }
        }

        if (record)
        {
            turnRecord.Add((clockwise ? move.RIGHT : move.RIGHTCC));
        }
    }

    void rotateMiddleYZFace(bool clockwise)//clockwise is relative to left face
    {
        int iterations = 1;
        if (!clockwise) iterations = 3;
        for (int j = 0; j < iterations; j++)
        {
            List<Cube> oldFrontOutline = getOutline(getCubeYZFace(1, false));
            List<Cube> currentFrontOutline = getOutline(getCubeYZFace(1, true));

            for (int i = 0; i < 8; i++)
            {
                currentFrontOutline[(i + 2) % 8].setSideColors(oldFrontOutline[i].getColors());
                currentFrontOutline[(i + 2) % 8].rotateX();
                currentFrontOutline[(i + 2) % 8].rotateX();
                currentFrontOutline[(i + 2) % 8].rotateX();

            }
        }
    }

    public void rotateLeftFace(bool clockwise, bool record = true)
    {
        int iterations = 1;
        if (!clockwise) iterations = 3;
        for (int j = 0; j < iterations; j++)
        {
            List<Cube> oldFrontOutline = getOutline(getCubeYZFace(0, false));
            List<Cube> currentFrontOutline = getOutline(getCubeYZFace(0, true));

            for (int i = 0; i < 8; i++)
            {
                currentFrontOutline[(i + 2) % 8].setSideColors(oldFrontOutline[i].getColors());
                currentFrontOutline[(i + 2) % 8].rotateX();
                currentFrontOutline[(i + 2) % 8].rotateX();
                currentFrontOutline[(i + 2) % 8].rotateX();

            }
        }

        if (record)
        {
            turnRecord.Add((clockwise ? move.LEFT : move.LEFTCC));
        }
    }

    public void rotateTopFace(bool clockwise, bool record = true)
    {
        int iterations = 1;
        if (!clockwise) iterations = 3;
        for (int j = 0; j < iterations; j++)
        {
            List<Cube> oldFrontOutline = getOutline(getCubeXZFace(2, false));
            List<Cube> currentFrontOutline = getOutline(getCubeXZFace(2, true));

            for (int i = 0; i < 8; i++)
            {
                currentFrontOutline[(i+2)%8].setSideColors(oldFrontOutline[i].getColors());
                currentFrontOutline[(i + 2) % 8].rotateY();
                currentFrontOutline[(i + 2) % 8].rotateY();
                currentFrontOutline[(i + 2) % 8].rotateY();
            }
        }

        if (record)
        {
            turnRecord.Add((clockwise ? move.TOP : move.TOPCC));
        }
    }

    void rotateMiddleXZFace(bool clockwise)//clockwise is relative to bottom face
    {
        int iterations = 1;
        if (!clockwise) iterations = 3;
        for (int j = 0; j < iterations; j++)
        {
            List<Cube> oldFrontOutline = getOutline(getCubeXZFace(1, false));
            List<Cube> currentFrontOutline = getOutline(getCubeXZFace(1, true));

            for (int i = 0; i < 8; i++)
            {
                currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
                currentFrontOutline[i].rotateY();
            }
        }
    }

    public void rotateBottomFace(bool clockwise, bool record = true)
    {
        int iterations = 1;
        if (!clockwise) iterations = 3;
        for (int j = 0; j < iterations; j++)
        {
            List<Cube> oldFrontOutline = getOutline(getCubeXZFace(0, false));
            List<Cube> currentFrontOutline = getOutline(getCubeXZFace(0, true));

            for (int i = 0; i < 8; i++)
            {
                currentFrontOutline[i].setSideColors(oldFrontOutline[(i+2)%8].getColors());
                currentFrontOutline[i].rotateY();
            }
        }

        if (record)
        {
            turnRecord.Add((clockwise ? move.BOTTOM : move.BOTTOMCC));
        }
    }

    public void turnCubeZ(bool clockwise)
    {
        rotateFrontFace(clockwise, false);
        rotateMiddleXYFace(clockwise);
        rotateBackFace(!clockwise, false);

        turnRecord.Add((clockwise ? move.Z : move.ZCC));
    }

    public void turnCubeX(bool clockwise)
    {
        rotateLeftFace(clockwise, false);
        rotateMiddleYZFace(clockwise);
        rotateRightFace(!clockwise, false);

        turnRecord.Add((clockwise ? move.X : move.XCC));
    }

    public void turnCubeY(bool clockwise)
    {
        rotateBottomFace(clockwise, false);
        rotateMiddleXZFace(clockwise);
        rotateTopFace(!clockwise, false);

        turnRecord.Add((clockwise ? move.Y : move.YCC));
    }

    public Vector3 cornerCubeWithColors(Cube.colorEnum a, Cube.colorEnum b, Cube.colorEnum c)
    {
        if (a == b || b == c || c == a)
            throw new System.ArgumentException("No two colors in query can be the same");

        for (int i= 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (cubeMatrix[i][j][k].containsColors(a, b, c))
                    {
                        return new Vector3(i, j, k);
                    }
                }
            }
        }

        //cube was not found. Something is wrong if you're looking for a cube that doesn't exist
        throw new System.ArgumentException("No such cube exists with colors provided");

    }

    public Vector3 sideCubeWithColors(Cube.colorEnum a, Cube.colorEnum b)
    {
        if (a == b)
            throw new System.ArgumentException("No two colors in query can be the same");

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    if (cubeMatrix[i][j][k].numColors()==2 && cubeMatrix[i][j][k].containsColors(a, b))
                    {
                        return new Vector3(i, j, k);
                    }
                }
            }
        }

        //cube was not found. Something is wrong if you're looking for a cube that doesn't exist
        throw new System.ArgumentException("No such cube exists with colors provided");
    }
   
    public bool isYellowCrossOnTop()
    {
        bool valid = true;
        List<List<Cube>> plane =  getCubeXZFace(2, false);

        if (plane[0][1].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;
        if (plane[1][0].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;
        if (plane[2][1].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;
        if (plane[1][2].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;

        //check center
        if (plane[1][1].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;

        return valid;
    }

    public bool isYellowHorizontalLineOnTop()
    {
        bool valid = true;
        List<List<Cube>> plane = getCubeXZFace(2, false);

        if (plane[0][1].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;
        if (plane[2][1].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;

        //check center
        if (plane[1][1].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;

        return valid;
    }

    public bool isYellowBackwardsLOnTop()
    {
        bool valid = true;
        List<List<Cube>> plane = getCubeXZFace(2, false);

        if (plane[0][1].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;
        if (plane[1][2].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;

        //check center
        if (plane[1][1].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
            valid = false;

        return valid;
    }

    public bool isTopAllYellow()
    {
        bool valid = true;
        List<List<Cube>> plane = getCubeXZFace(2, false);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (plane[i][j].getColor(Cube.sides.TOP) != Cube.colorEnum.YELLOW)
                    valid = false;
            }
        }

        return valid;
    }

    public bool allTopCornersSolved()
    {
        bool iscorrect = true;
        for (int i = 0; i < 4; i++)
        {
            Cube TopLeftFrontCube = cubeMatrix[0][2][0];
            Cube MiddleLeftFrontCube = cubeMatrix[0][1][0];

            if (TopLeftFrontCube.getColor(Cube.sides.FRONT) != MiddleLeftFrontCube.getColor(Cube.sides.FRONT) || TopLeftFrontCube.getColor(Cube.sides.LEFT) != MiddleLeftFrontCube.getColor(Cube.sides.LEFT))
                iscorrect = false;
            turnCubeY(true);
        }

        return iscorrect;
    }

    public bool isSolved()
    {
        List<List<Cube>> Side;
        Cube.colorEnum c;
        bool valid = true;

        for (int z = 0; z < 4; z++)//check perimeter
        {
            Side = getCubeXYFace(0, true);
            c = Side[1][1].getColor(Cube.sides.FRONT);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Side[i][j].getColor(Cube.sides.FRONT) != c)
                        valid = false;
                }
            }

            turnCubeY(true);
        }

        //check top side
        Side = getCubeXZFace(2, true);
        c = Side[1][1].getColor(Cube.sides.TOP);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (Side[i][j].getColor(Cube.sides.TOP) != c)
                    valid = false;
            }
        }

        //if 5 of the 6 sides are correct, the last side must be correct

        return valid;
    }

    public int RunSequence(int s)
    {
        //Debug.Log("Running sequence: " + s);
        List<move> seq = sequences[s];
        int cost = RunCustomSequence(seq);
        return cost;
    }

    public void RunCustomSequence(move m)
    {
        switch (m)
        {
            case move.RIGHT:
                rotateRightFace(true);
                break;
            case move.RIGHTCC:
                rotateRightFace(false);
                break;
            case move.LEFT:
                rotateLeftFace(true);
                break;
            case move.LEFTCC:
                rotateLeftFace(false);
                break;
            case move.TOP:
                rotateTopFace(true);
                break;
            case move.TOPCC:
                rotateTopFace(false);
                break;
            case move.BOTTOM:
                rotateBottomFace(true);
                break;
            case move.BOTTOMCC:
                rotateBottomFace(false);
                break;
            case move.FRONT:
                rotateFrontFace(true);
                break;
            case move.FRONTCC:
                rotateFrontFace(false);
                break;
            case move.BACK:
                rotateBackFace(true);
                break;
            case move.BACKCC:
                rotateBackFace(false);
                break;
            case move.X:
                turnCubeX(true);
                break;
            case move.XCC:
                turnCubeX(false);
                break;
            case move.Y:
                turnCubeY(true);
                break;
            case move.YCC:
                turnCubeY(false);
                break;
            case move.Z:
                turnCubeZ(true);
                break;
            case move.ZCC:
                turnCubeZ(false);
                break;
            case move.NOTHING:
            default:
                break;
        }
    }

    public int RunCustomSequence(List<move> seq)
    {
        for(int i = 0; i < seq.Count; ++i) {
            RunCustomSequence(seq[i]);
        }

        return seq.Count;
    }

    public move randChangeMove()
    {
        return (move)Mathf.Round(Random.value * changeMoveCount - 0.499999f);
    }

    public void Scramble(int turns)
    {

        List<move> seq = new List<move>();
        for (int i = 0; i < turns; i++)
        {
            seq.Add(randChangeMove());
        }

        RunCustomSequence(seq);

        clearTurnRecord();
    }

    public void clearTurnRecord()
    {
        turnRecord.Clear();
        turnRecord = new List<move>();
    }

    public RubiksCube cloneCube()
    {
        RubiksCube RC = new RubiksCube();
        RC.turnRecord = new List<move>(turnRecord);
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0;y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    Cube tempCube = new Cube();
                    for (int i = 0; i < 6; i++)
                    {
                        Cube.colorEnum tempc = cubeMatrix[x][y][z].getColor((Cube.sides)i);
                        tempCube.setSideColor((Cube.sides)i, tempc);
                    }
                    RC.cubeMatrix[x][y][z] = tempCube;
                }
            }
        }

        return RC;
    }

    public int TurnRecordTokenCount()
    {
        return turnRecord.Count;
    }

    public void turnCubeToFaceRGBOColorWithYellowOrWhiteOnTop(Cube.colorEnum c)
    {
        if (cubeMatrix[1][1][0].getColor(Cube.sides.FRONT) == c)
            return;//do nothing
        else if (cubeMatrix[0][1][1].getColor(Cube.sides.LEFT) == c)
            turnCubeY(true);
        else if (cubeMatrix[2][1][1].getColor(Cube.sides.RIGHT) == c)
            turnCubeY(false);
        else
        {
            turnCubeY(true);
            turnCubeY(true);
        }
    }

}
