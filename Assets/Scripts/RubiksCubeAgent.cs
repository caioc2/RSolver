using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MLAgents;

public class RubiksCubeAgent : Agent
{

    [Header("Specific to Rubiks Cube")]
    private RubiksCubePrefab rcp;
    public int numScramble = 1;
    public int interval = 100;
    public bool animated = false;
    public bool maskAction = false;

    public float minSolveRate = 0.95f;
    public float maxMoveRate = 1.10f;
    private int solveCount = 0;
    private int resetCount = 0;
    private int totalMoves = 0;

    private RubiksCubeAcademy academy;


    public override void InitializeAgent()
    {
        rcp = transform.gameObject.GetComponent<RubiksCubePrefab>() as RubiksCubePrefab;
        if(maskAction)
        {
            rcp.scramble(numScramble, new int[] { 0, 1, 6, 7, 10, 11 });
        } else
        {
            rcp.scramble(numScramble);
        }
        
        if (animated)
        {
            agentParameters.onDemandDecision = true;
            agentParameters.resetOnDone = false;
            agentParameters.maxStep = 0;
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(rcp.getCubeState());
        if(maskAction && rcp.GetType() == typeof(RubiksCubePrefab2))
        {
            SetActionMask(0, new int[] { 2, 3, 4, 5, 8, 9 });
        }
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

            if (animated)
            {
                rcp.runAnimatedMove(m);
            }
            else
            {
                rcp.runMove(m);
            }
            AddReward(-0.01f);

            if (rcp.isSolved())
            {
                AddReward(1.0f);
                Done();
                incSolvedCount();
            }
        }
    }

    public override void AgentReset()
    {
        totalMoves += rcp.turnCount();

        rcp.resetCube();
        if (maskAction)
        {
            rcp.scramble(numScramble, new int[] { 0, 1, 6, 7, 10, 11 });
        }
        else
        {
            rcp.scramble(numScramble);
        }
        incResetCount();
        

        if (getResetCount() >= interval)
        {
            float p = getSolveRate();
            float l = 1.0f * totalMoves / (1.0f * interval);

            Debug.Log("Solve rate: " + p + " , Avg Len: " + l + " Scramble: " + numScramble);
            if (l < numScramble * maxMoveRate && minSolveRate < p)
            {
                numScramble = Mathf.Min(numScramble + 1,  50);
            }
            resetCounters();
        }
    }

    private bool waitRequest = false;
    public void FixedUpdate()
    {
        if (!IsDone() && agentParameters.onDemandDecision)
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
                    Done();
                    Debug.Log("Solved in: " + rcp.turnCount() + " moves.");
                }
            }
        }
    }

    private void incResetCount()
    {
        resetCount++;
    }
    private void incSolvedCount()
    {
        solveCount++;
    }

    private int getResetCount()
    {
        return resetCount;
    }
    private int getSolvedCount()
    {
        return solveCount;
    }

    private float getSolveRate()
    {
        return 1.0f * solveCount / (1.0f * resetCount);
    }

    private void resetCounters()
    {
        resetCount = solveCount = totalMoves = 0;
    }
}
