﻿using System.Collections;
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

    public float rate = 0.95f;
    private int count = 0;
    private int count2 = 0;
    private int steps = 0;
    private float sc;


    public override void InitializeAgent()
    {
        rcp = transform.gameObject.GetComponent<RubiksCubePrefab>() as RubiksCubePrefab;
        rcp.scramble((int)Mathf.Floor(startScramble + Random.value * maxScramble));
        sc = rcp.getScore();
        if(animated)
        {
            agentParameters.onDemandDecision = true;
            agentParameters.maxStep = 0;
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(rcp.getCubeState());
    }

    RubiksCube.move last = RubiksCube.move.NOTHING;
    int lc = 0;
    public override void AgentAction(float[] vectorAction, string textAction)
    {

        if(vectorAction.Length != 1)
        {
            Debug.LogError("Action vector length != than 1");
        }
        else
        { 
            RubiksCube.move m = (RubiksCube.move)vectorAction[0];
            if (m == last)
                lc++;
            else
                lc = 0;

            last = m;

            if (lc >= 4)
            {
                AddReward(-0.01f);
                lc = 0;
            }

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
                count++;
            } else
            {
                float score = rcp.getScore();
                if(sc < score)
                {
                    AddReward(0.01f);
                    sc = score;
                }
            }
        }
    }

    public override void AgentReset()
    {
        rcp.resetCube();
        lc = 0;
        rcp.scramble((int)Mathf.Floor(startScramble + Random.value * maxScramble));
        sc = rcp.getScore();
        count2++;

        if(count2 >= 1000)
        {
            float p = (float)count / (float)count2;

            Debug.Log("Current rate: " + p + ", current scramble: " + maxScramble);
            if (p >= rate)
            {
                startScramble = maxScramble = Mathf.Min(maxScramble + 1, 30);
            }
            count = count2 = 0;
        }
    }

    private bool waitRequest = false;
    public void FixedUpdate()
    {
        if (!IsDone() && agentParameters.onDemandDecision) {
            if(!waitRequest)
            {
                RequestDecision();
                waitRequest = true;
            }else if (!rcp.isAnimationInProgress() && rcp.movesToAnim() == 0)
            {
                waitRequest = false;
                if(rcp.isSolved())
                {
                    Done();
                    Debug.Log("Solved");
                }
            }
        }
    }

}
