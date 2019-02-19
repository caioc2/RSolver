using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RubiksCubePrefab3 : RubiksCubePrefab
{
    // Use this for initialization
    void Awake()
    {
        RC = new RubiksCube3();
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

    protected override void resetCubePrefabPositions()
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

    protected override void RefreshPanels()
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

    protected override void rotateRight(float angle)
    {
        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[2][i][j].transform.RotateAround(transform.position, transform.right, angle); } }
    }

    protected override void rotateLeft(float angle)
    {
        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[0][i][j].transform.RotateAround(transform.position, transform.right, angle); } }
    }

    protected override void rotateTop(float angle)
    {
        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][2][j].transform.RotateAround(transform.position, transform.up, angle); } }
    }

    protected override void rotateBottom(float angle)
    {
        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][0][j].transform.RotateAround(transform.position, transform.up, angle); } }
    }

    protected override void rotateFront(float angle)
    {
        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][j][0].transform.RotateAround(transform.position, transform.forward, angle); } }
    }

    protected override void rotateBack(float angle)
    {
        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][j][2].transform.RotateAround(transform.position, transform.forward, angle); } }
    }
}