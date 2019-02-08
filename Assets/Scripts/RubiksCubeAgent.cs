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
            return;
        }
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

    public override void AgentReset()
    {
        rcp.scramble((int)Mathf.Floor(startScramble + Random.value * maxScramble));
    }

}

/*[CustomEditor(typeof(RubiksCubeAgent))]
public class RubiksCubeAgentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myRubiksCubeAgent = target as RubiksCubeAgent;

        myRubiksCubeAgent.agentParameters.onDemandDecision = GUILayout.Toggle(myRubiksCubeAgent.animated);

        if (myRubiksCubeAgent.animated)
            myRubiksCubeAgent.agentParameters.onDemandDecision = EditorGUILayout.Toggle(false);
    }
}*/
