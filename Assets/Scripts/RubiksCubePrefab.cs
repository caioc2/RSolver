using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class RubiksCubePrefab : MonoBehaviour
{

    public GameObject CubePrefab;

    protected RubiksCube RC;//the actual rubiks cube data structure
    protected List<List<List<GameObject>>> cubePrefabMatrix;
    public float spacing = 1.05f;
    public float rotationTime = 10.0f;
    public bool DrawCubes = true;
    protected Queue<RubiksCube.move> AnimSeq;
    protected RubiksCube.move current;

    protected int getNCubes()
    {
        return RC.getNCubes();
    }

    protected void initCube()
    {
        AnimSeq = new Queue<RubiksCube.move>();
        current = RubiksCube.move.NOTHING;
        cubePrefabMatrix = new List<List<List<GameObject>>>();

        float displacement = (getNCubes() - 1.0f) / 2.0f;

        for (int x = 0; x < getNCubes(); x++)
        {
            List<List<GameObject>> PrefabRow = new List<List<GameObject>>();
            for (int y = 0; y < getNCubes(); y++)
            {
                List<GameObject> PrefabColumn = new List<GameObject>();
                for (int z = 0; z < getNCubes(); z++)
                {
                    GameObject cubePrefab = Instantiate(CubePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    cubePrefab.transform.SetParent(transform);
                    cubePrefab.transform.localPosition = new Vector3((x - displacement), (y - displacement), (z - displacement)) * spacing;
                    cubePrefab.transform.localScale = Vector3.one;
                    cubePrefab.GetComponent<CubePrefab>().refreshPanels(RC.cubeMatrix[x][y][z]);
                    cubePrefab.SetActive(DrawCubes);
                    PrefabColumn.Add(cubePrefab);
                }
                PrefabRow.Add(PrefabColumn);
            }
            cubePrefabMatrix.Add(PrefabRow);
        }
        resetCubePrefabPositions();
        RefreshPanels();
    }

    bool isAnimInProgress = false;
    void Update()
    {
        if (isAnimInProgress)
        {
            isAnimInProgress = animMove(current);
        }
        else if (AnimSeq.Count > 0)
        {
            current = AnimSeq.Dequeue();
            isAnimInProgress = true;
        }
    }

    //returns current animation time
    private float totalTime = 0.0f;
    private bool animMove(RubiksCube.move m)
    {
        const float angle = 90.0f;
        float delta = Time.deltaTime;
        totalTime += delta;
        if (totalTime < 0.0f || totalTime >= rotationTime || rotationTime <= 0.0f)
        {
            totalTime = 0.0f;
            runMove(m);
            return false;
        }

        float rot = angle * delta / rotationTime;

        float dir = RubiksCube.isClockwise(m) ? -1.0f : 1.0f;
        switch (m)
        {
            case RubiksCube.move.RIGHT:
            case RubiksCube.move.RIGHTCC:
                rot = -dir * rot;
                rotateRight(rot);
                break;
            case RubiksCube.move.LEFT:
            case RubiksCube.move.LEFTCC:
                rot = dir * rot;
                rotateLeft(rot);
                break;
            case RubiksCube.move.TOP:
            case RubiksCube.move.TOPCC:
                rot = -dir * rot;
                rotateTop(rot);
                break;
            case RubiksCube.move.BOTTOM:
            case RubiksCube.move.BOTTOMCC:
                rot = dir * rot;
                rotateBottom(rot);
                break;
            case RubiksCube.move.FRONT:
            case RubiksCube.move.FRONTCC:
                rot = dir * rot;
                rotateFront(rot);
                break;
            case RubiksCube.move.BACK:
            case RubiksCube.move.BACKCC:
                rot = -dir * rot;
                rotateBack(rot);
                break;
            case RubiksCube.move.X:
            case RubiksCube.move.XCC:
                rot = dir * rot;
                rotateX(rot);
                break;
            case RubiksCube.move.Y:
            case RubiksCube.move.YCC:
                rot = dir * rot;
                rotateY(rot);
                break;
            case RubiksCube.move.Z:
            case RubiksCube.move.ZCC:
                rot = dir * rot;
                rotateZ(rot);
                break;
            default:
                break;
        }
        return true;
    }

    protected void RefreshPanels()
    {
        for (int x = 0; x < getNCubes(); x++)
        {
            for (int y = 0; y < getNCubes(); y++)
            {
                for (int z = 0; z < getNCubes(); z++)
                {
                    cubePrefabMatrix[x][y][z].GetComponent<CubePrefab>().refreshPanels(RC.cubeMatrix[x][y][z]);
                }
            }
        }
    }

    protected  void resetCubePrefabPositions()
    {
        float displacement = (getNCubes() - 1.0f) / 2.0f;
        for (int i = 0; i < getNCubes(); i++)
        {
            for (int j = 0; j < getNCubes(); j++)
            {
                for (int k = 0; k < getNCubes(); k++)
                {
                    cubePrefabMatrix[i][j][k].transform.localPosition = new Vector3((i - displacement), (j - displacement), (k - displacement)) * spacing + transform.localPosition;
                    cubePrefabMatrix[i][j][k].transform.localRotation = Quaternion.identity;
                }
            }
        }
    }

    protected void rotateRight(float angle)
    {
        for (int i = 0; i < getNCubes(); i++) { for (int j = 0; j < getNCubes(); j++) { cubePrefabMatrix[getNCubes() - 1][i][j].transform.RotateAround(transform.position, transform.right, angle); } }
    }

    protected void rotateLeft(float angle)
    {
        for (int i = 0; i < getNCubes(); i++) { for (int j = 0; j < getNCubes(); j++) { cubePrefabMatrix[0][i][j].transform.RotateAround(transform.position, transform.right, angle); } }
    }

    protected void rotateTop(float angle)
    {
        for (int i = 0; i < getNCubes(); i++) { for (int j = 0; j < getNCubes(); j++) { cubePrefabMatrix[i][getNCubes() - 1][j].transform.RotateAround(transform.position, transform.up, angle); } }
    }

    protected  void rotateBottom(float angle)
    {
        for (int i = 0; i < getNCubes(); i++) { for (int j = 0; j < getNCubes(); j++) { cubePrefabMatrix[i][0][j].transform.RotateAround(transform.position, transform.up, angle); } }
    }

    protected void rotateFront(float angle)
    {
        for (int i = 0; i < getNCubes(); i++) { for (int j = 0; j < getNCubes(); j++) { cubePrefabMatrix[i][j][0].transform.RotateAround(transform.position, transform.forward, angle); } }
    }

    protected void rotateBack(float angle)
    {
        for (int i = 0; i < getNCubes(); i++) { for (int j = 0; j < getNCubes(); j++) { cubePrefabMatrix[i][j][getNCubes() - 1].transform.RotateAround(transform.position, transform.forward, angle); } }
    }

    protected void rotateX(float angle)
    {
        for (int x = 0; x < getNCubes(); x++)
        {
            for (int y = 0; y < getNCubes(); y++)
            {
                for (int z = 0; z < getNCubes(); z++)
                {
                    cubePrefabMatrix[x][y][z].transform.RotateAround(transform.position, transform.right, angle);
                }
            }
        }
    }

    protected void rotateY(float angle)
    {
        for (int x = 0; x < getNCubes(); x++)
        {
            for (int y = 0; y < getNCubes(); y++)
            {
                for (int z = 0; z < getNCubes(); z++)
                {
                    cubePrefabMatrix[x][y][z].transform.RotateAround(transform.position, transform.up, angle);
                }
            }
        }
    }

    protected void rotateZ(float angle)
    {
        for (int x = 0; x < getNCubes(); x++)
        {
            for (int y = 0; y < getNCubes(); y++)
            {
                for (int z = 0; z < getNCubes(); z++)
                {
                    cubePrefabMatrix[x][y][z].transform.RotateAround(transform.position, transform.forward, angle);
                }
            }
        }
    }

    public void runAnimatedMove(RubiksCube.move m)
    {
        AnimSeq.Enqueue(m);
    }

    public void runAnimatedMove(List<RubiksCube.move> m)
    {
        for (int i = 0; i < m.Count; ++i) runAnimatedMove(m[i]);
    }

    public void runMove(RubiksCube.move m)
    {
        RC.RunCustomSequence(m);
        resetCubePrefabPositions();
        RefreshPanels();
    }

    public void runMove(List<RubiksCube.move> m)
    {
        RC.RunCustomSequence(m);
        resetCubePrefabPositions();
        RefreshPanels();
    }

    public void resetCube()
    {
        AnimSeq.Clear();
        current = RubiksCube.move.NOTHING;
        isAnimInProgress = false;
        RC.setDefaultCubeColor();
        resetCubePrefabPositions();
        RefreshPanels();
        RC.clearTurnRecord();
    }

    public void scramble(int turns)
    {
        RC.Scramble(turns);
        resetCubePrefabPositions();
        RefreshPanels();
    }

    public void scramble(int turns, int []mask)
    {
        RC.Scramble(turns, mask);
        resetCubePrefabPositions();
        RefreshPanels();
    }

    public bool isAnimationInProgress()
    {
        return isAnimInProgress;
    }

    public int movesToAnim()
    {
        return AnimSeq.Count;
    }

    public float[] getCubeState()
    {
        return RC.getStateAsVec();
    }

    public bool isSolved()
    {
        return RC.isSolved();
    }

    public float getScore()
    {
        return RC.getScore();
    }

    public int turnCount()
    {
        return RC.TurnRecordTokenCount();
    }
}
