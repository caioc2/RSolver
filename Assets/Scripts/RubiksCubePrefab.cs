using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RubiksCubePrefab : MonoBehaviour {

    public GameObject CubePrefab;

    private RubiksCube RC;//the actual rubiks cube data structure
    private List<List<List<GameObject>>> cubePrefabMatrix;
    public float spacing = 1.05f;
    public float rotationTime = 10.0f;
    private Queue<RubiksCube.move> AnimSeq;
    private RubiksCube.move current;

    // Use this for initialization
    void Awake () {
        RC = new RubiksCube();
        AnimSeq = new Queue<RubiksCube.move>();
        current = RubiksCube.move.NOTHING;
        cubePrefabMatrix = new List<List<List<GameObject>>>();
        for (int x = 0; x < 3; x++)
        {
            List<List<GameObject>> PrefabRow = new List<List<GameObject>>();
            for (int y = 0; y < 3; y++)
            {
                List<GameObject> PrefabColumn = new List<GameObject>();
                for (int z = 0; z < 3; z++)
                {
                    GameObject cubePrefab = Instantiate(CubePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    cubePrefab.transform.SetParent(transform);
                    cubePrefab.transform.position = new Vector3((x - 1), (y - 1), (z - 1)) * spacing + transform.position;
                    cubePrefab.GetComponent<CubePrefab>().refreshPanels(RC.cubeMatrix[x][y][z]);
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

    private void updateCube(RubiksCube.move m)
    {
        resetCubePrefabPositions();
        RC.RunCustomSequence(m);
        RefreshPanels();
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
            updateCube(m);
            return false;
        }

        float rot = angle * delta / rotationTime;

        float dir = RubiksCube.isClockwise(m) ? -1.0f : 1.0f;
        switch (m)
        {
            case RubiksCube.move.RIGHT:
            case RubiksCube.move.RIGHTCC:
                    rot = -dir * rot;
                    for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[2][i][j].transform.RotateAround(transform.position, transform.right, rot); } }
                break;
            case RubiksCube.move.LEFT:
            case RubiksCube.move.LEFTCC:
                    rot = dir * rot;
                    for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[0][i][j].transform.RotateAround(transform.position, transform.right, rot); } }
                break;
            case RubiksCube.move.TOP:
            case RubiksCube.move.TOPCC:
                    rot = -dir * rot;
                    for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][2][j].transform.RotateAround(transform.position, transform.up, rot); } }
                break;
            case RubiksCube.move.BOTTOM:
            case RubiksCube.move.BOTTOMCC:
                    rot = dir * rot;
                    for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][0][j].transform.RotateAround(transform.position, transform.up, rot); } }
                break;
            case RubiksCube.move.FRONT:
            case RubiksCube.move.FRONTCC:
                    rot = dir * rot;
                for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][j][0].transform.RotateAround(transform.position, transform.forward, rot); } }
                break;
            case RubiksCube.move.BACK:
            case RubiksCube.move.BACKCC:
                    rot = -dir * rot;
                for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][j][2].transform.RotateAround(transform.position, transform.forward, rot); } }
                break;
            case RubiksCube.move.X:
            case RubiksCube.move.XCC:
                    rot = dir * rot;
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 3; z++)
                        {
                            cubePrefabMatrix[x][y][z].transform.RotateAround(transform.position, transform.right, rot);
                        }
                    }
                }
                //parent rotation is not resetable
                //transform.RotateAround(transform.position, transform.right, rot);
                break;
            case RubiksCube.move.Y:
            case RubiksCube.move.YCC:
                    rot = dir * rot;
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 3; z++)
                        {
                            cubePrefabMatrix[x][y][z].transform.RotateAround(transform.position, transform.up, rot);
                        }
                    }
                }
                //transform.RotateAround(transform.position, transform.up, rot);
                break;
            case RubiksCube.move.Z:
            case RubiksCube.move.ZCC:
                    rot = dir * rot;
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 3; z++)
                        {
                            cubePrefabMatrix[x][y][z].transform.RotateAround(transform.position, transform.forward, rot);
                        }
                    }
                }
                //transform.RotateAround(transform.position, transform.forward, rot);
                break;
            default:
                break;
        }
        return true;
    }

    private void resetCubePrefabPositions()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    cubePrefabMatrix[i][j][k].transform.position = new Vector3((i - 1), (j - 1), (k - 1)) * spacing + transform.position;
                    cubePrefabMatrix[i][j][k].transform.rotation = Quaternion.identity;
                }
            }
        }
    }

    private void RefreshPanels()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    cubePrefabMatrix[x][y][z].GetComponent<CubePrefab>().refreshPanels(RC.cubeMatrix[x][y][z]);
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
        RC.setDefaultCubeColor();
        resetCubePrefabPositions();
        RefreshPanels();
    }

    public void scramble(int turns)
    {
        RC.Scramble(turns);
        resetCubePrefabPositions();
        RefreshPanels();
    }

    public bool isAnimationInProgress()
    {
        return isAnimInProgress;
    }

    public float[] getCubeState()
    {
        return RC.getStateAsVec();
    }

    public bool isSolved()
    {
        return RC.isSolved();
    }
}
