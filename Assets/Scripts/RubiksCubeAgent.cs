using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RubiksCubeAgent : Agent {

    [Header("Specific to Rubiks Cube")]
    public GameObject cube;
    private RubiksCubePrefab rcp;
    public int startScramble = 1;
    private int maxScramble = 50;
    private bool firstTime = true;


    public override void InitializeAgent()
    {
        cube = transform.gameObject;
        rcp = cube.GetComponent<RubiksCubePrefab>() as RubiksCubePrefab;
        rcp.RC.Scramble((int)Mathf.Floor(startScramble + Random.value * maxScramble)); 
    }

    public override void CollectObservations()
    {
        //AddVectorObs(rcp.RC.getStateAsVec());
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

    }

    public override void AgentReset()
    {
        //rcp.resetCubePrefabPositions();
        //rcp.RC.Scramble((int)Mathf.Floor(startScramble + Random.value * maxScramble));
    }

}
