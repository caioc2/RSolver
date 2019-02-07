using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RubiksCubeAgent : Agent {

    [Header("Specific to Rubiks Cube")]
    private RubiksCubePrefab rcp;
    public int startScramble = 1;
    private int maxScramble = 50;
    private bool firstTime = true;


    public override void InitializeAgent()
    {
        rcp = transform.gameObject.GetComponent<RubiksCubePrefab>() as RubiksCubePrefab;
        List<RubiksCube.move> move = RubiksCube.randMoveSequence((int)Mathf.Floor(startScramble + Random.value * maxScramble));
        rcp.addMove(move);
    }

    public override void CollectObservations()
    {
        AddVectorObs(rcp.RC.getStateAsVec());
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

    }

    public override void AgentReset()
    {
        rcp.resetCubePrefabPositions();
        List<RubiksCube.move> move = RubiksCube.randMoveSequence((int)Mathf.Floor(startScramble + Random.value * maxScramble));
        rcp.addMove(move);
    }

}
