﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RubiksCubePrefab : MonoBehaviour {

    public GameObject CubePrefab;

    public RubiksCube RC;//the actual rubiks cube data structure
    public List<List<List<GameObject>>> cubePrefabMatrix;
    public float spacing = 1.05f;
    public float rotationSpeed = 500;

    // Use this for initialization
    void Start () {
        RC = new RubiksCube();
        
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
    }

    void Update()
    {
        //Not needed?
        //RefreshPanels();
    }

    public void resetCubePrefabPositions()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    cubePrefabMatrix[i][j][k].transform.position = new Vector3((i - 1), (j - 1), (k - 1)) * spacing;
                    cubePrefabMatrix[i][j][k].transform.rotation = Quaternion.identity;
                }
            }
        }
    }
    
    public IEnumerator animateCustomSequence(List<RubiksCube.move> seq)
    {

        int step = 0;


        while (step < seq.Count)
        {
            RubiksCube.move c = seq[step];//get the character of the turn to run
            bool clockwise = RubiksCube.isClockwise(c);

            float totalRotation = 0;
            int dir = 1;
            if (clockwise)
                dir = -1;
            float delta = 0;

            switch(c) {
                case RubiksCube.move.RIGHT:
                case RubiksCube.move.RIGHTCC:
                    while (Mathf.Abs(totalRotation) < 90){
                        delta = -1* dir * rotationSpeed * Time.deltaTime;
                        totalRotation += delta;
                        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[2][i][j].transform.RotateAround(transform.position, transform.right, delta); } }
                        yield return null;
                    }
                    RC.rotateRightFace(clockwise);
                    break;
                case RubiksCube.move.LEFT:
                case RubiksCube.move.LEFTCC:
                    while (Mathf.Abs(totalRotation) < 90)
                    {
                        delta = dir * rotationSpeed * Time.deltaTime;
                        totalRotation += delta;
                        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[0][i][j].transform.RotateAround(transform.position, transform.right, delta); } }
                        yield return null;
                    }
                    RC.rotateLeftFace(clockwise);
                    break;
                case RubiksCube.move.TOP:
                case RubiksCube.move.TOPCC:
                    while (Mathf.Abs(totalRotation) < 90)
                    {
                        delta = -1 * dir * rotationSpeed * Time.deltaTime;
                        totalRotation += delta;
                        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][2][j].transform.RotateAround(transform.position, transform.up, delta); } }
                        yield return null;
                    }
                    RC.rotateTopFace(clockwise);
                    break;
                case RubiksCube.move.BOTTOM:
                case RubiksCube.move.BOTTOMCC:
                    while (Mathf.Abs(totalRotation) < 90)
                    {
                        delta = dir * rotationSpeed * Time.deltaTime;
                        totalRotation += delta;
                        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][0][j].transform.RotateAround(transform.position, transform.up, delta); } }
                        yield return null;
                    }
                    RC.rotateBottomFace(clockwise);
                    break;
                case RubiksCube.move.FRONT:
                case RubiksCube.move.FRONTCC:
                    while (Mathf.Abs(totalRotation) < 90)
                    {
                        delta = dir * rotationSpeed * Time.deltaTime;
                        totalRotation += delta;
                        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][j][0].transform.RotateAround(transform.position, transform.forward, delta); } }
                        yield return null;
                    }
                    RC.rotateFrontFace(clockwise);
                    break;
                case RubiksCube.move.BACK:
                case RubiksCube.move.BACKCC:

                    while (Mathf.Abs(totalRotation) < 90)
                    {
                        delta = -1 * dir * rotationSpeed * Time.deltaTime;
                        totalRotation += delta;
                        for (int i = 0; i < 3; i++) { for (int j = 0; j < 3; j++) { cubePrefabMatrix[i][j][2].transform.RotateAround(transform.position, transform.forward, delta); } }
                        yield return null;
                    }
                    RC.rotateBackFace(clockwise);
                    break;
                case RubiksCube.move.X:
                case RubiksCube.move.XCC:
                    while (Mathf.Abs(totalRotation) < 90)
                    {
                        delta = dir * rotationSpeed * Time.deltaTime;
                        totalRotation += delta;
                        transform.RotateAround(transform.position, transform.right, delta);
                        yield return null;
                    }
                    RC.turnCubeX(clockwise);
                    break;
                case RubiksCube.move.Y:
                case RubiksCube.move.YCC:
                    while (Mathf.Abs(totalRotation) < 90)
                    {
                        delta = dir * rotationSpeed * Time.deltaTime;
                        totalRotation += delta;
                        transform.RotateAround(transform.position, transform.up, delta);
                        yield return null;
                    }
                    RC.turnCubeY(clockwise);
                    break;
                case RubiksCube.move.Z:
                case RubiksCube.move.ZCC:
                    while (Mathf.Abs(totalRotation) < 90)
                    {
                        delta = dir * rotationSpeed * Time.deltaTime;
                        totalRotation += delta;
                        transform.RotateAround(transform.position, transform.forward, delta);
                        yield return null;
                    }
                    RC.turnCubeZ(clockwise);
                    break;
                default:
                    break;
            }

            step++;
            transform.rotation = Quaternion.identity;
            transform.position = Vector3.zero;
            resetCubePrefabPositions();
            RefreshPanels();
        }

        yield return null;
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
