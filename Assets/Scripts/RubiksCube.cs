using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class RubiksCube
{
    // assumes clockwise moves to have binary as ???0 and counter-clockwise to be ???1 comparing (moveA >> 1) == (moveACC >> 1)
    public enum move
    {
        RIGHT = 0, RIGHTCC = 1, LEFT = 2, LEFTCC = 3, TOP = 4, TOPCC = 5, BOTTOM = 6, BOTTOMCC = 7, FRONT = 8, FRONTCC = 9, BACK = 10, BACKCC = 11,
        X = 12, XCC = 13, Y = 14, YCC = 15, Z = 16, ZCC = 17, NOTHING = 18
    };
    public static int moveCount = 19; //total move count
    public static int changeMoveCount = 12; //total moves which change cube configuration
    public List<List<List<Cube>>> cubeMatrix;
    public List<move> turnRecord;

    public static move getInverse(move a)
    {
        return (move)((uint)a ^ 1);

    }

    public static bool isClockwise(move a)
    {
        return (((uint)a & 1) == 0);
    }

    public int TurnRecordTokenCount()
    {
        return turnRecord.Count;
    }

    protected abstract void rotateRightFace(bool cloclwise, bool record = true);

    protected abstract void rotateLeftFace(bool cloclwise, bool record = true);

    protected abstract void rotateTopFace(bool cloclwise, bool record = true);

    protected abstract void rotateBottomFace(bool cloclwise, bool record = true);

    protected abstract void rotateFrontFace(bool cloclwise, bool record = true);

    protected abstract void rotateBackFace(bool cloclwise, bool record = true);

    protected abstract void turnCubeX(bool cloclwise);

    protected abstract void turnCubeY(bool cloclwise);

    protected abstract void turnCubeZ(bool cloclwise);

    public abstract void setDefaultCubeColor();

    public abstract bool isSolved();

    public abstract float[] getStateAsVec();

    public abstract float getScore();

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
        for (int i = 0; i < seq.Count; ++i)
        {
            RunCustomSequence(seq[i]);
        }

        return seq.Count;
    }

    public static move randChangeMove()
    {
        return (move)Mathf.Round(Random.value * changeMoveCount - 0.499999f);
    }

    public static move randMove()
    {
        return (move)Mathf.Round(Random.value * moveCount - 0.499999f);
    }

    public void Scramble(int turns)
    {
        RunCustomSequence(randMoveSequence(turns));

        clearTurnRecord();
    }

    public static List<move> randMoveSequence(int turns)
    {
        List<move> seq = new List<move>(turns);

        move last = move.NOTHING;
        while (seq.Count < turns)
        {
            move m = randChangeMove();
            if (last != getInverse(m))
            {
                seq.Add(m);
                last = m;
            }

        }
        return seq;
    }

    public void clearTurnRecord()
    {
        turnRecord.Clear();
    }
}

public class RubiksCube3 : RubiksCube
{
    // Use this for initialization
    public RubiksCube3()
    {
        turnRecord = new List<move>();
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
        setDefaultCubeColor();
    }

    public override void setDefaultCubeColor()
    {
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


    

    List<Cube> getOutline(List<List<Cube>> face)
    {
        //converts a 2d matrix of cubes to a linear list of the outline cubes of the provided face
        //the list is ordered in a clockwise direction, starting with the lower left cube [0][0]
        //getOutline returns a list of references to the origional matrix
        List<Cube> outline = new List<Cube>();
        for (int i = 0; i< 3; i++)
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


    protected override void rotateFrontFace(bool clockwise, bool record = true)
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

    protected override void rotateBackFace(bool clockwise, bool record = true)
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

    protected override void rotateRightFace(bool clockwise, bool record = true)
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

    protected override void rotateLeftFace(bool clockwise, bool record = true)
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

    protected override void rotateTopFace(bool clockwise, bool record = true)
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

    protected override void rotateBottomFace(bool clockwise, bool record = true)
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

    protected override void turnCubeZ(bool clockwise)
    {
        rotateFrontFace(clockwise, false);
        rotateMiddleXYFace(clockwise);
        rotateBackFace(!clockwise, false);

        turnRecord.Add((clockwise ? move.Z : move.ZCC));
    }

    protected override void turnCubeX(bool clockwise)
    {
        rotateLeftFace(clockwise, false);
        rotateMiddleYZFace(clockwise);
        rotateRightFace(!clockwise, false);

        turnRecord.Add((clockwise ? move.X : move.XCC));
    }

    protected override void turnCubeY(bool clockwise)
    {
        rotateBottomFace(clockwise, false);
        rotateMiddleXZFace(clockwise);
        rotateTopFace(!clockwise, false);

        turnRecord.Add((clockwise ? move.Y : move.YCC));
    }

    public override bool isSolved()
    {
        foreach (Cube.sides s in System.Enum.GetValues(typeof(Cube.sides)))
        {
            Cube.colorEnum[,] c = getFaceColors(s);
            for (int k = 0; k < 3; ++k)
            {
                for (int l = 0; l < 3; ++l)
                {
                    if (c[l, k] != c[1, 1])
                        return false;
                }
            }
        }
        return true;
    }

    public override float getScore()
    {
        float sc = 0.0f;
        foreach (Cube.sides s in System.Enum.GetValues(typeof(Cube.sides)))
        {
            Cube.colorEnum[,] c = getFaceColors(s);
            for (int k = 0; k < 3; ++k)
            {
                for (int l = 0; l < 3; ++l)
                {
                    if (c[l, k] != c[1, 1])
                        sc -= 1.0f;
                }
            }
        }
        return sc;
    }

    public List<List<Cube>> getFace(Cube.sides which)
    {
        switch(which)
        {
            case Cube.sides.FRONT:
                return getCubeXYFace(0, true);
            case Cube.sides.BACK:
                return getCubeXYFace(2, true);
            case Cube.sides.LEFT:
                return getCubeYZFace(0, true);
            case Cube.sides.RIGHT:
                return getCubeYZFace(2, true);
            case Cube.sides.BOTTOM:
                return getCubeXZFace(0, true);
            case Cube.sides.TOP:
                return getCubeXZFace(2, true);
            default:
                return new List<List<Cube>>();
        }
    }

    public Cube.colorEnum[,] getFaceColors(Cube.sides which)
    {
        Cube.colorEnum c = Cube.colorEnum.WHITE;
        Cube.colorEnum[,] colors = { { c, c, c }, { c, c, c }, { c, c, c } };
        List<List<Cube>> side = getFace(which);
        for(int i = 0; i < 3; ++i)
        {
            for(int j = 0; j < 3; ++j)
            {
                colors[i, j] = side[i][j].getColor(which);
            }
        }

        return colors;
    }

    public override float[] getStateAsVec()
    {
        // 6 faces, with 9 cubes, with possible 6 colors per cube
        float[] state = new float[6 * 9 * 6];

        //pre-process, first map current sides to colors

        int[] c2s = new int[6];//Not safe if change color/side enum values
        //int[] s2c = new int[6];

        foreach(Cube.sides s in System.Enum.GetValues(typeof(Cube.sides)))
        {
            Cube.colorEnum c = getFaceColors(s)[1, 1];
            c2s[(int)c] = (int)s;
            //s2c[(int)s] = (int)c;
        }

        int i = 0;
        foreach (Cube.sides s in System.Enum.GetValues(typeof(Cube.sides)))
        {
            foreach(Cube.colorEnum c in getFaceColors(s))
            {
                state[i + c2s[(int)c]] = 1.0f;
                i += 6;
            }
        }
        return state;
    }

    public RubiksCube3 cloneCube()
    {
        RubiksCube3 RC = new RubiksCube3();
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
}
