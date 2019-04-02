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
    public  const int moveCount = 19; //total move count
    public  const int changeMoveCount = 12; //total moves which change cube configuration
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

    public abstract int getNCubes();

    protected abstract void rotateRightFace(bool record = true);

    protected abstract void rotateLeftFace(bool record = true);

    protected abstract void rotateTopFace(bool record = true);

    protected abstract void rotateBottomFace(bool record = true);

    protected abstract void rotateFrontFace(bool record = true);

    protected abstract void rotateBackFace(bool record = true);

    protected abstract void turnCubeX();

    protected abstract void turnCubeY();

    protected abstract void turnCubeZ();

    protected abstract void rotateRightFaceCC(bool record = true);

    protected abstract void rotateLeftFaceCC(bool record = true);

    protected abstract void rotateTopFaceCC(bool record = true);

    protected abstract void rotateBottomFaceCC(bool record = true);

    protected abstract void rotateFrontFaceCC(bool record = true);

    protected abstract void rotateBackFaceCC(bool record = true);

    protected abstract void turnCubeXCC();

    protected abstract void turnCubeYCC();

    protected abstract void turnCubeZCC();

    public abstract float getScore();

    public void setDefaultCubeColor()
    {
        int n = getNCubes();
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                cubeMatrix[i][j][0].setSideColor(Cube.sides.FRONT, Cube.colorEnum.RED);
                cubeMatrix[n-1][i][j].setSideColor(Cube.sides.RIGHT, Cube.colorEnum.BLUE);
                cubeMatrix[0][i][j].setSideColor(Cube.sides.LEFT, Cube.colorEnum.GREEN);
                cubeMatrix[i][j][n-1].setSideColor(Cube.sides.BACK, Cube.colorEnum.ORANGE);
                cubeMatrix[i][n-1][j].setSideColor(Cube.sides.TOP, Cube.colorEnum.WHITE);
                cubeMatrix[i][0][j].setSideColor(Cube.sides.BOTTOM, Cube.colorEnum.YELLOW);
            }
        }

        //cubeMatrix[1][1][1].setAllSideColors(Cube.colorEnum.BLACK);
    }

    public List<List<Cube>> getFace(Cube.sides which)
    {
        int n = getNCubes();
        switch (which)
        {
            case Cube.sides.FRONT:
                return getCubeXYFace(0, true);
            case Cube.sides.BACK:
                return getCubeXYFace(n-1, true);
            case Cube.sides.LEFT:
                return getCubeYZFace(0, true);
            case Cube.sides.RIGHT:
                return getCubeYZFace(n-1, true);
            case Cube.sides.BOTTOM:
                return getCubeXZFace(0, true);
            case Cube.sides.TOP:
                return getCubeXZFace(n-1, true);
            default:
                return new List<List<Cube>>();
        }
    }

    public Cube.colorEnum[,] getFaceColors(Cube.sides which)
    {
        int n = getNCubes();
        Cube.colorEnum[,] colors = new Cube.colorEnum[n, n];
        List<List<Cube>> side = getFace(which);
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                colors[i, j] = side[i][j].getColor(which);
            }
        }

        return colors;
    }

    public bool isSolved()
    {
        int n = getNCubes();
        foreach (Cube.sides s in System.Enum.GetValues(typeof(Cube.sides)))
        {
            Cube.colorEnum[,] c = getFaceColors(s);
            for (int k = 0; k < n; ++k)
            {
                for (int l = 0; l < n; ++l)
                {
                    if (c[l, k] != c[1, 1])
                        return false;
                }
            }
        }
        return true;
    }

    public float[] getStateAsVec()
    {
        int n = getNCubes();
        // 6 faces, with n*n cubes faces, with possible 6 colors per cube
        float[] state = new float[6 * n * n * 6];

        //pre-process, first map current sides to colors

        int[] c2s = new int[6];//Not safe if change color/side enum values
        //int[] s2c = new int[6];

        foreach (Cube.sides s in System.Enum.GetValues(typeof(Cube.sides)))
        {
            Cube.colorEnum c = getFaceColors(s)[1, 1];
            c2s[(int)c] = (int)s;
            //s2c[(int)s] = (int)c;
        }

        int i = 0;
        foreach (Cube.sides s in System.Enum.GetValues(typeof(Cube.sides)))
        {
            foreach (Cube.colorEnum c in getFaceColors(s))
            {
                state[i + c2s[(int)c]] = 1.0f;
                i += 6;
            }
        }
        return state;
    }
    
    public void RunCustomSequence(move m)
    {
        switch (m)
        {
            case move.RIGHT:
                rotateRightFace();
                break;
            case move.RIGHTCC:
                rotateRightFaceCC();
                break;
            case move.LEFT:
                rotateLeftFace();
                break;
            case move.LEFTCC:
                rotateLeftFaceCC();
                break;
            case move.TOP:
                rotateTopFace();
                break;
            case move.TOPCC:
                rotateTopFaceCC();
                break;
            case move.BOTTOM:
                rotateBottomFace();
                break;
            case move.BOTTOMCC:
                rotateBottomFaceCC();
                break;
            case move.FRONT:
                rotateFrontFace();
                break;
            case move.FRONTCC:
                rotateFrontFaceCC();
                break;
            case move.BACK:
                rotateBackFace();
                break;
            case move.BACKCC:
                rotateBackFaceCC();
                break;
            case move.X:
                turnCubeX();
                break;
            case move.XCC:
                turnCubeXCC();
                break;
            case move.Y:
                turnCubeY();
                break;
            case move.YCC:
                turnCubeYCC();
                break;
            case move.Z:
                turnCubeZ();
                break;
            case move.ZCC:
                turnCubeZCC();
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

    public static move randChangeMove(int count = changeMoveCount)
    {
        return (move)Mathf.Round(Random.value * count - 0.499999f);
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

    public void Scramble(int turns, int[] mask)
    {
        RunCustomSequence(randMoveSequence(turns, mask));
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

    public static List<move> randMoveSequence(int turns, int[] mask)
    {
        List<move> seq = new List<move>(turns);

        move last = move.NOTHING;
        while (seq.Count < turns)
        {
            move m = (move)mask[Mathf.Min((int)randChangeMove(mask.Length), mask.Length - 1)];
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

    public List<List<Cube>> getCubeXYFace(int r, bool reference)
    {
        int n = getNCubes();
        List<List<Cube>> face = new List<List<Cube>>();
        for (int i = 0; i < n; i++)
        {
            List<Cube> row = new List<Cube>();
            for (int j = 0; j < n; j++)
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
        int n = getNCubes();
        List<List<Cube>> face = new List<List<Cube>>();
        for (int i = 0; i < n; i++)
        {
            List<Cube> row = new List<Cube>();
            for (int j = 0; j < n; j++)
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
        int n = getNCubes();
        List<List<Cube>> face = new List<List<Cube>>();
        for (int i = 0; i < n; i++)
        {
            List<Cube> row = new List<Cube>();
            for (int j = 0; j < n; j++)
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
}

public class RubiksCube3 : RubiksCube
{
    protected const int nCubes = 3;
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

    public override int getNCubes()
    {
        return nCubes;
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


    void rotateMiddleXYFace()//clockwise is relative to front face in this case
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXYFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXYFace(1, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 6) % 8].getColors());
            currentFrontOutline[i].rotateZ();
        }
    }

    void rotateMiddleXYFaceCC()//clockwise is relative to front face in this case
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXYFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXYFace(1, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
            currentFrontOutline[i].rotateZCC();
        }
    }

    void rotateMiddleYZFace()//clockwise is relative to left face
    {
        List<Cube> oldFrontOutline = getOutline(getCubeYZFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeYZFace(1, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 6) % 8].getColors());
            currentFrontOutline[i].rotateXCC();
        }
    }

    void rotateMiddleYZFaceCC()//clockwise is relative to left face
    {
        List<Cube> oldFrontOutline = getOutline(getCubeYZFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeYZFace(1, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
            currentFrontOutline[i].rotateX();
        }
    }

    void rotateMiddleXZFace()//clockwise is relative to bottom face
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXZFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXZFace(1, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
            currentFrontOutline[i].rotateY();
        }
    }

    void rotateMiddleXZFaceCC()//clockwise is relative to bottom face
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXZFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXZFace(1, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 6) % 8].getColors());
            currentFrontOutline[i].rotateYCC();
        }
    }

    protected override void rotateFrontFace(bool record = true)
    {

        List<Cube> oldFrontOutline = getOutline(getCubeXYFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXYFace(0, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 6) % 8].getColors());
            currentFrontOutline[i].rotateZ();
        }
        if (record) turnRecord.Add(move.FRONT);
    }

    protected override void rotateFrontFaceCC(bool record = true)
    {

        List<Cube> oldFrontOutline = getOutline(getCubeXYFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXYFace(0, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
            currentFrontOutline[i].rotateZCC();
        }
        if (record) turnRecord.Add(move.FRONTCC);
    }

    protected override void rotateBackFace(bool record = true)
    {

        List<Cube> oldFrontOutline = getOutline(getCubeXYFace(2, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXYFace(2, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
            currentFrontOutline[i].rotateZCC();
        }
        if (record) turnRecord.Add(move.BACK);
    }

    protected override void rotateBackFaceCC(bool record = true)
    {

        List<Cube> oldFrontOutline = getOutline(getCubeXYFace(2, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXYFace(2, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 6) % 8].getColors());
            currentFrontOutline[i].rotateZ();
        }
        if (record) turnRecord.Add(move.BACKCC);
    }

    protected override void rotateRightFace(bool record = true)
    {

        List<Cube> oldFrontOutline = getOutline(getCubeYZFace(2, false));
        List<Cube> currentFrontOutline = getOutline(getCubeYZFace(2, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
            currentFrontOutline[i].rotateX();
        }
        if (record) turnRecord.Add(move.RIGHT);
    }

    protected override void rotateRightFaceCC(bool record = true)
    {

        List<Cube> oldFrontOutline = getOutline(getCubeYZFace(2, false));
        List<Cube> currentFrontOutline = getOutline(getCubeYZFace(2, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 6) % 8].getColors());
            currentFrontOutline[i].rotateXCC();
        }
        if (record) turnRecord.Add(move.RIGHTCC);
    }

    protected override void rotateLeftFace(bool record = true)
    {

        List<Cube> oldFrontOutline = getOutline(getCubeYZFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeYZFace(0, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 6) % 8].getColors());
            currentFrontOutline[i].rotateXCC();

        }
        if (record) turnRecord.Add(move.LEFT);
    }

    protected override void rotateLeftFaceCC(bool record = true)
    {

        List<Cube> oldFrontOutline = getOutline(getCubeYZFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeYZFace(0, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
            currentFrontOutline[i].rotateX();

        }
        if (record) turnRecord.Add(move.LEFT);
    }

    protected override void rotateTopFace(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXZFace(2, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXZFace(2, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 6) % 8].getColors());
            currentFrontOutline[i].rotateYCC();
        }
        if (record) turnRecord.Add(move.TOP);
    }

    protected override void rotateTopFaceCC(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXZFace(2, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXZFace(2, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
            currentFrontOutline[i].rotateY();
        }
        if (record) turnRecord.Add(move.TOPCC);
    }

    protected override void rotateBottomFace(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXZFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXZFace(0, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 2) % 8].getColors());
            currentFrontOutline[i].rotateY();
        }
        if (record) turnRecord.Add(move.BOTTOM);
    }

    protected override void rotateBottomFaceCC(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXZFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXZFace(0, true));

        for (int i = 0; i < 8; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 6) % 8].getColors());
            currentFrontOutline[i].rotateYCC();
        }
        if (record) turnRecord.Add(move.BOTTOMCC);
    }

    protected override void turnCubeZ()
    {
        rotateFrontFace(false);
        rotateMiddleXYFace();
        rotateBackFaceCC(false);
        turnRecord.Add(move.Z);
    }

    protected override void turnCubeZCC()
    {
        rotateFrontFaceCC(false);
        rotateMiddleXYFaceCC();
        rotateBackFace(false);
        turnRecord.Add(move.ZCC);
    }

    protected override void turnCubeX()
    {
        rotateLeftFace(false);
        rotateMiddleYZFace();
        rotateRightFaceCC(false);
        turnRecord.Add(move.X);
    }

    protected override void turnCubeXCC()
    {
        rotateLeftFaceCC(false);
        rotateMiddleYZFaceCC();
        rotateRightFace(false);
        turnRecord.Add(move.XCC);
    }

    protected override void turnCubeY()
    {
        rotateBottomFace(false);
        rotateMiddleXZFace();
        rotateTopFaceCC(false);
        turnRecord.Add(move.Y);
    }

    protected override void turnCubeYCC()
    {
        rotateBottomFaceCC(false);
        rotateMiddleXZFaceCC();
        rotateTopFace(false);
        turnRecord.Add(move.YCC);
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

//2x2 rubiks cube
public class RubiksCube2 : RubiksCube
{
    // Use this for initialization
    protected const int nCubes = 2;

    public RubiksCube2()
    {
        turnRecord = new List<move>();
        cubeMatrix = new List<List<List<Cube>>>();

        for (int x = 0; x < nCubes; x++)
        {
            List<List<Cube>> CubeRow = new List<List<Cube>>();
            for (int y = 0; y < nCubes; y++)
            {
                List<Cube> CubeColumn = new List<Cube>();
                for (int z = 0; z < nCubes; z++)
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

    public override int getNCubes()
    {
        return nCubes;
    }

    List<Cube> getOutline(List<List<Cube>> face)
    {
        //converts a 2d matrix of cubes to a linear list of the outline cubes of the provided face
        //the list is ordered in a clockwise direction, starting with the lower left cube [0][0]
        //getOutline returns a list of references to the origional matrix
        List<Cube> outline = new List<Cube>();
        outline.Add(face[0][0]);
        outline.Add(face[0][1]);
        outline.Add(face[1][1]);
        outline.Add(face[1][0]);

        return outline;
    }

    protected override void rotateFrontFace(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXYFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXYFace(0, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 3) % 4].getColors());
            currentFrontOutline[i].rotateZ();
        }
        if (record) turnRecord.Add(move.FRONT);
    }

    protected override void rotateFrontFaceCC(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXYFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXYFace(0, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 1) % 4].getColors());
            currentFrontOutline[i].rotateZCC();
        }
        if (record) turnRecord.Add(move.FRONTCC);
    }

    protected override void rotateBackFace(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXYFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXYFace(1, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 1) % 4].getColors());
            currentFrontOutline[i].rotateZCC();
        }
        if (record) turnRecord.Add(move.BACK);
    }

    protected override void rotateBackFaceCC(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXYFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXYFace(1, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 3) % 4].getColors());
            currentFrontOutline[i].rotateZ();
        }
        if (record) turnRecord.Add(move.BACKCC);
    }

    protected override void rotateRightFace(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeYZFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeYZFace(1, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 1) % 4].getColors());
            currentFrontOutline[i].rotateX();
        }
        if (record) turnRecord.Add(move.RIGHT);
    }

    protected override void rotateRightFaceCC(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeYZFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeYZFace(1, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 3) % 4].getColors());
            currentFrontOutline[i].rotateXCC();
        }
        if (record) turnRecord.Add(move.RIGHTCC);
    }

    protected override void rotateLeftFace(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeYZFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeYZFace(0, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 3) % 4].getColors());
            currentFrontOutline[i].rotateXCC();

        }
        if (record) turnRecord.Add(move.LEFT);
    }

    protected override void rotateLeftFaceCC(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeYZFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeYZFace(0, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 1) % 4].getColors());
            currentFrontOutline[i].rotateX();

        }
        if (record) turnRecord.Add(move.LEFT);
    }

    protected override void rotateTopFace(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXZFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXZFace(1, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 3) % 4].getColors());
            currentFrontOutline[i].rotateYCC();
        }
        if (record) turnRecord.Add(move.TOP);
    }

    protected override void rotateTopFaceCC(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXZFace(1, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXZFace(1, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 1) % 4].getColors());
            currentFrontOutline[i].rotateY();
        }
        if (record) turnRecord.Add(move.TOPCC);
    }

    protected override void rotateBottomFace(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXZFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXZFace(0, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 1) % 4].getColors());
            currentFrontOutline[i].rotateY();
        }
        if (record) turnRecord.Add(move.BOTTOM);
    }

    protected override void rotateBottomFaceCC(bool record = true)
    {
        List<Cube> oldFrontOutline = getOutline(getCubeXZFace(0, false));
        List<Cube> currentFrontOutline = getOutline(getCubeXZFace(0, true));

        for (int i = 0; i < 4; i++)
        {
            currentFrontOutline[i].setSideColors(oldFrontOutline[(i + 3) % 4].getColors());
            currentFrontOutline[i].rotateYCC();
        }
        if (record) turnRecord.Add(move.BOTTOMCC);
    }

    protected override void turnCubeZ()
    {
        rotateFrontFace(false);
        rotateBackFaceCC(false);
        turnRecord.Add(move.Z);
    }

    protected override void turnCubeZCC()
    {
        rotateFrontFaceCC(false);
        rotateBackFace(false);
        turnRecord.Add(move.ZCC);
    }

    protected override void turnCubeX()
    {
        rotateLeftFace(false);
        rotateRightFaceCC(false);
        turnRecord.Add(move.X);
    }

    protected override void turnCubeXCC()
    {
        rotateLeftFaceCC(false);
        rotateRightFace(false);
        turnRecord.Add(move.XCC);
    }

    protected override void turnCubeY()
    {
        rotateBottomFace(false);
        rotateTopFaceCC(false);
        turnRecord.Add(move.Y);
    }

    protected override void turnCubeYCC()
    {
        rotateBottomFaceCC(false);
        rotateTopFace(false);
        turnRecord.Add(move.YCC);
    }

    public override float getScore()
    {
        float sc = 0.0f;
        foreach (Cube.sides s in System.Enum.GetValues(typeof(Cube.sides)))
        {
            Cube.colorEnum[,] c = getFaceColors(s);
            for (int k = 0; k < 2; ++k)
            {
                for (int l = 0; l < 2; ++l)
                {
                    if (c[l, k] != c[1, 1])
                        sc -= 1.0f;
                }
            }
        }
        return sc;
    }

    public RubiksCube2 cloneCube()
    {
        RubiksCube2 RC = new RubiksCube2();
        RC.turnRecord = new List<move>(turnRecord);
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    Cube tempCube = new Cube();
                    for (int i = 0; i < 2; i++)
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
