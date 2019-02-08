using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MLAgents;

public class RubiksCubeAgent : Agent {

    [Header("Specific to Rubiks Cube")]
    private RubiksCubePrefab rcp;
    public int startScramble = 1;
    public int maxScramble = 50;
    public bool animated = false;


    public override void InitializeAgent()
    {
        rcp = transform.gameObject.GetComponent<RubiksCubePrefab>() as RubiksCubePrefab;
        rcp.scramble((int)Mathf.Floor(startScramble + Random.value * maxScramble));
        if(animated)
        {
            agentParameters.onDemandDecision = true;
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(rcp.getCubeState());
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if(vectorAction.Length != 1)
        {
            Debug.LogError("Action vector length != than 1");
        }
        else
        { 
            RubiksCube.move m = (RubiksCube.move)vectorAction[0];

            if(animated)
            {
                rcp.runAnimatedMove(m);
            } else
            {
                rcp.runMove(m);
            }
            AddReward(-0.005f);

            if(rcp.isSolved())
            {
                AddReward(1.0f);
                Done();
            }
        }
        waitRequest = false;
    }

    public override void AgentReset()
    {
        rcp.resetCube();
        rcp.scramble((int)Mathf.Floor(startScramble + Random.value * maxScramble));

    }

    private bool waitRequest = false;
    public void FixedUpdate()
    {
        if(agentParameters.onDemandDecision && !rcp.isAnimationInProgress() && !waitRequest)
        {
            RequestDecision();
            waitRequest = true;
        }
    }

}
