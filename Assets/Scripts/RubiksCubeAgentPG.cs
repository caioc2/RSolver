using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MLAgents;

public class RubiksCubeAgentPG : Agent
{

    [Header("Specific to Rubiks Cube")]
    private RubiksCubePrefab rcp;
    public override void InitializeAgent()
    {
        rcp = transform.gameObject.GetComponent<RubiksCubePrefab>() as RubiksCubePrefab;

        agentParameters.onDemandDecision = true;
        agentParameters.resetOnDone = false;
        agentParameters.maxStep = 0;
    }

    public override void CollectObservations()
    {
        AddVectorObs(rcp.getCubeState());
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (vectorAction.Length != 1)
        {
            Debug.LogError("Action vector length != than 1");
        }
        else
        {
            RubiksCube.move m = (RubiksCube.move)vectorAction[0];
            rcp.runAnimatedMove(m);
        }
    }

    private bool toSolve = false;
    public void Solve()
    {
        toSolve = true;
    }

    private bool waitRequest = false;
    public void FixedUpdate()
    {
        if (toSolve)
        {
            if (!waitRequest)
            {
                RequestDecision();
                waitRequest = true;
            }
            else if (!rcp.isAnimationInProgress() && rcp.movesToAnim() == 0)
            {
                waitRequest = false;
                if (rcp.isSolved())
                {
                    toSolve = false;
                }
            }
        }
    }
}
