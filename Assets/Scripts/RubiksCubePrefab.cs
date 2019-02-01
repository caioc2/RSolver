using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RubiksCubePrefab : MonoBehaviour {

    public GameObject CubePrefab;

    public RubiksCube RC;//the actual rubiks cube data structure
    public List<List<List<GameObject>>> cubePrefabMatrix;
    public float spacing = 1.05f;
    public float rotationTime = 10.0f;
    private Queue<RubiksCube.move> AnimSeq;
    private RubiksCube.move current;

    // Use this for initialization
    void Start () {
        RC = new RubiksCube();
        //AnimSeq = new Queue<RubiksCube.move>();
        AnimSeq = new Queue<RubiksCube.move>(new List<RubiksCube.move> { RubiksCube.move.RIGHT, RubiksCube.move.TOP, RubiksCube.move.FRONT });
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

        Debug.Log(transform.right);
        Debug.Log(transform.up);
        Debug.Log(transform.forward);
    }

    private float totalTime = 0.0f;
    void Update()
    {
        if (totalTime >= rotationTime || rotationTime <= 0.0f)
        {
            RC.RunCustomSequence(current);
            resetCubePrefabPositions();
            RefreshPanels();

            if (AnimSeq.Count > 0)
            {
                current = AnimSeq.Dequeue();
                totalTime = 0.0f;
            } else
            {
                current = RubiksCube.move.NOTHING;
            }
        }

        if (current == RubiksCube.move.NOTHING)
        {
            if(AnimSeq.Count > 0)
            {
                current = AnimSeq.Dequeue();
                totalTime = 0.0f;
            }
        }
        else
        { 
            totalTime = animMove(totalTime, current);
        }

    }

    //returns current animation time
    private float animMove(float t, RubiksCube.move m)
    {
        float dir = RubiksCube.isClockwise(m) ? -1.0f : 1.0f;
        const float angle = 90.0f;

        if (t < 0.0f) return float.MaxValue;

        //If current time > than animTime, stop anim and update cube faces
        if(t >=  rotationTime || rotationTime <= 0.0f)
        {
            return float.MaxValue;
        }

        //Else, animate
        float rot;
        float delta = Time.deltaTime;
        delta = delta + t > rotationTime ? rotationTime - t : delta;
        switch (m)
        {
            case RubiksCube.move.RIGHT:
            case RubiksCube.move.RIGHTCC:
                    rot = -1 * dir * angle * delta / rotationTime;
                    for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[2][i][j].transform.RotateAround(transform.position, transform.right, rot); } }
                break;
            case RubiksCube.move.LEFT:
            case RubiksCube.move.LEFTCC:
                    rot = dir * angle * delta / rotationTime;
                    for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[0][i][j].transform.RotateAround(transform.position, transform.right, rot); } }
                break;
            case RubiksCube.move.TOP:
            case RubiksCube.move.TOPCC:
                    rot = -1 * dir * angle * delta / rotationTime;
                    for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][2][j].transform.RotateAround(transform.position, transform.up, rot); } }
                break;
            case RubiksCube.move.BOTTOM:
            case RubiksCube.move.BOTTOMCC:
                    rot = dir * angle * delta / rotationTime;
                    for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][0][j].transform.RotateAround(transform.position, transform.up, rot); } }
                break;
            case RubiksCube.move.FRONT:
            case RubiksCube.move.FRONTCC:
                    rot = dir * angle * delta / rotationTime;
                for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][j][0].transform.RotateAround(transform.position, transform.forward, rot); } }
                break;
            case RubiksCube.move.BACK:
            case RubiksCube.move.BACKCC:
                    rot = -1 * dir * angle * delta / rotationTime;
                for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][j][2].transform.RotateAround(transform.position, transform.forward, rot); } }
                break;
            case RubiksCube.move.X:
            case RubiksCube.move.XCC:
                    rot = dir * angle * delta / rotationTime;
                transform.RotateAround(transform.position, transform.right, rot);
                break;
            case RubiksCube.move.Y:
            case RubiksCube.move.YCC:
                    rot = dir * angle * delta / rotationTime;
                transform.RotateAround(transform.position, transform.up, rot);
                break;
            case RubiksCube.move.Z:
            case RubiksCube.move.ZCC:
                    rot = dir * angle * delta / rotationTime;
                transform.RotateAround(transform.position, transform.forward, rot);
                break;
            default:
                break;
        }
        return t + delta;
    }

    public void resetCubePrefabPositions()
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

    public void RefreshPanels()
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


}
