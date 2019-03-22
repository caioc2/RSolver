using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MLAgents;
using System;
using System.IO;

public class RubiksCubeAgentPerfTest : Agent
{

    [Header("Specific to Rubiks Cube")]
    private RubiksCubePrefab rcp;
    public int maxScramble = 100;
    public int interval = 1000;
    
    private int solveCount = 0;
    private int resetCount = 0;

    private int curScramble = 1;
    private int minMoves = Int32.MaxValue;
    private int maxMoves = 0;
    private int totalMoves = 0;

    public override void InitializeAgent()
    {
        rcp = transform.gameObject.GetComponent<RubiksCubePrefab>() as RubiksCubePrefab;
        using (StreamWriter writer = new StreamWriter(File.Open(Application.dataPath + "/stats.txt", FileMode.Append, FileAccess.Write)))
        {
            writer.WriteLine("ScrambleDepth  SolveRate AvgMoves MinMoves MaxMoves");
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(rcp.getCubeState());
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        if (vectorAction.Length != 1)
        {
            Debug.LogError("Action vector length != 1");
        }
        else
        {
            RubiksCube.move m = (RubiksCube.move)vectorAction[0];
            rcp.runMove(m);
            if (rcp.isSolved())
            {
                Done();
                incSolvedCount();
            }
        }
    }

    public override void AgentReset()
    {
        int moveCount = rcp.turnCount();
        moveCount = moveCount == 0 ? 1 : moveCount;//Fix first time run;

        incResetCount();
        rcp.resetCube();
        rcp.scramble(curScramble);

        
        
        if(moveCount <= agentParameters.maxStep)
        {
            totalMoves += moveCount;
            minMoves = Mathf.Min(minMoves, moveCount);
            maxMoves = Mathf.Max(maxMoves, moveCount);
        }
        moveCount = 0;


        

        if (getResetCount() >= interval)
        {
            float solveRate = getSolveRate();
            float avgMoves = solveCount > 0 ? 1.0f * totalMoves / (1.0f * solveCount): 0;

           using (StreamWriter writer = new StreamWriter(File.Open(Application.dataPath + "/stats.txt", FileMode.Append, FileAccess.Write)))
            {
                writer.WriteLine(curScramble + " " + solveRate + " " + avgMoves + " " + minMoves + " " + maxMoves);
            }
            Debug.Log(curScramble + " " + solveRate + " " + avgMoves + " " + minMoves + " " + maxMoves);

            minMoves = Int32.MaxValue;
            maxMoves = 0;
            totalMoves = 0;
            resetCounters();
            curScramble++;

            if (curScramble > maxScramble)
            {
                agentParameters.resetOnDone = false;
                Debug.Log("Finished generating stats!");
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
        resetCount = solveCount = 0;
    }

}
