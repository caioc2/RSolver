using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneController : MonoBehaviour {

    public GameObject rubiksCube;
    private RubiksCubePrefab rcp;
    private RubiksCubeAgentPG rca;
    // Use this for initialization
    void Awake () {
        rcp = rubiksCube.GetComponent<RubiksCubePrefab>() as RubiksCubePrefab;
        rca = rubiksCube.GetComponent<RubiksCubeAgentPG>() as RubiksCubeAgentPG;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void L()
    {
        rcp.runAnimatedMove(RubiksCube.move.LEFT);
    }

    public void Lcc()
    {
        rcp.runAnimatedMove(RubiksCube.move.LEFTCC);
    }
    public void R()
    {
        rcp.runAnimatedMove(RubiksCube.move.RIGHT);
    }

    public void Rcc()
    {
        rcp.runAnimatedMove(RubiksCube.move.RIGHTCC);
    }
    public void U()
    {
        rcp.runAnimatedMove(RubiksCube.move.TOP);
    }

    public void Ucc()
    {
        rcp.runAnimatedMove(RubiksCube.move.TOPCC);
    }
    public void D()
    {
        rcp.runAnimatedMove(RubiksCube.move.BOTTOM);
    }

    public void Dcc()
    {
        rcp.runAnimatedMove(RubiksCube.move.BOTTOMCC);
    }
    public void B()
    {
        rcp.runAnimatedMove(RubiksCube.move.BACK);
    }

    public void Bcc()
    {
        rcp.runAnimatedMove(RubiksCube.move.BACKCC);
    }
    public void F()
    {
        rcp.runAnimatedMove(RubiksCube.move.FRONT);
    }

    public void Fcc()
    {
        rcp.runAnimatedMove(RubiksCube.move.FRONTCC);
    }

    public void X()
    {
        rcp.runAnimatedMove(RubiksCube.move.X);
    }

    public void Xcc()
    {
        rcp.runAnimatedMove(RubiksCube.move.XCC);
    }

    public void Y()
    {
        rcp.runAnimatedMove(RubiksCube.move.Y);
    }

    public void Ycc()
    {
        rcp.runAnimatedMove(RubiksCube.move.YCC);
    }

    public void reset()
    {
        rcp.resetCube();
    }


    public void scramble1()
    {
        rcp.scramble(1);
    }

    public void scramble10()
    {
        rcp.scramble(10);
    }

    public void scramble100()
    {
        rcp.scramble(100);
    }

    public void solve()
    {
        rca.Solve();
    }
}
