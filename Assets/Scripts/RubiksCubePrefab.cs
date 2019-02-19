using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class RubiksCubePrefab : MonoBehaviour
{

    public GameObject CubePrefab;

    protected RubiksCube RC;//the actual rubiks cube data structure
    protected List<List<List<GameObject>>> cubePrefabMatrix;
    public float spacing = 1.05f;
    public float rotationTime = 10.0f;
    protected Queue<RubiksCube.move> AnimSeq;
    protected RubiksCube.move current;

    protected abstract void resetCubePrefabPositions();

    protected abstract void RefreshPanels();

    protected abstract void rotateRight(float angle);

    protected abstract void rotateLeft(float angle);

    protected abstract void rotateTop(float angle);

    protected abstract void rotateBottom(float angle);

    protected abstract void rotateFront(float angle);

    protected abstract void rotateBack(float angle);

    bool isAnimInProgress = false;
    void Update()
    {
        if (isAnimInProgress)
        {
            isAnimInProgress = animMove(current);
        }
        else if (AnimSeq.Count > 0)
        {
            current = AnimSeq.Dequeue();
            isAnimInProgress = true;
        }
    }

    //returns current animation time
    private float totalTime = 0.0f;
    private bool animMove(RubiksCube.move m)
    {
        const float angle = 90.0f;
        float delta = Time.deltaTime;
        totalTime += delta;
        if (totalTime < 0.0f || totalTime >= rotationTime || rotationTime <= 0.0f)
        {
            totalTime = 0.0f;
            runMove(m);
            return false;
        }

        float rot = angle * delta / rotationTime;

        float dir = RubiksCube.isClockwise(m) ? -1.0f : 1.0f;
        switch (m)
        {
            case RubiksCube.move.RIGHT:
            case RubiksCube.move.RIGHTCC:
                rot = -dir * rot;
                rotateRight(rot);
                break;
            case RubiksCube.move.LEFT:
            case RubiksCube.move.LEFTCC:
                rot = dir * rot;
                rotateLeft(rot);
                break;
            case RubiksCube.move.TOP:
            case RubiksCube.move.TOPCC:
                rot = -dir * rot;
                rotateTop(rot);
                break;
            case RubiksCube.move.BOTTOM:
            case RubiksCube.move.BOTTOMCC:
                rot = dir * rot;
                rotateBottom(rot);
                break;
            case RubiksCube.move.FRONT:
            case RubiksCube.move.FRONTCC:
                rot = dir * rot;
                rotateFront(rot);
                break;
            case RubiksCube.move.BACK:
            case RubiksCube.move.BACKCC:
                rot = -dir * rot;
                rotateBack(rot);
                break;
            case RubiksCube.move.X:
            case RubiksCube.move.XCC:
                rot = dir * rot;
                for (int x = 0; x < cubePrefabMatrix.Count; x++)
                {
                    for (int y = 0; y < cubePrefabMatrix[x].Count; y++)
                    {
                        for (int z = 0; z < cubePrefabMatrix[x][y].Count; z++)
                        {
                            cubePrefabMatrix[x][y][z].transform.RotateAround(transform.position, transform.right, rot);
                        }
                    }
                }
                //parent rotation is not resetable
                //transform.RotateAround(transform.position, transform.right, rot);
                break;
            case RubiksCube.move.Y:
            case RubiksCube.move.YCC:
                rot = dir * rot;
                for (int x = 0; x < cubePrefabMatrix.Count; x++)
                {
                    for (int y = 0; y < cubePrefabMatrix[x].Count; y++)
                    {
                        for (int z = 0; z < cubePrefabMatrix[x][y].Count; z++)
                        {
                            cubePrefabMatrix[x][y][z].transform.RotateAround(transform.position, transform.up, rot);
                        }
                    }
                }
                //transform.RotateAround(transform.position, transform.up, rot);
                break;
            case RubiksCube.move.Z:
            case RubiksCube.move.ZCC:
                rot = dir * rot;
                for (int x = 0; x < cubePrefabMatrix.Count; x++)
                {
                    for (int y = 0; y < cubePrefabMatrix[x].Count; y++)
                    {
                        for (int z = 0; z < cubePrefabMatrix[x][y].Count; z++)
                        {
                            cubePrefabMatrix[x][y][z].transform.RotateAround(transform.position, transform.forward, rot);
                        }
                    }
                }
                //transform.RotateAround(transform.position, transform.forward, rot);
                break;
            default:
                break;
        }
        return true;
    }

    public void runAnimatedMove(RubiksCube.move m)
    {
        AnimSeq.Enqueue(m);
    }

    public void runAnimatedMove(List<RubiksCube.move> m)
    {
        for (int i = 0; i < m.Count; ++i) runAnimatedMove(m[i]);
    }

    public void runMove(RubiksCube.move m)
    {
        RC.RunCustomSequence(m);
        resetCubePrefabPositions();
        RefreshPanels();
    }

    public void runMove(List<RubiksCube.move> m)
    {
        RC.RunCustomSequence(m);
        resetCubePrefabPositions();
        RefreshPanels();
    }

    public void resetCube()
    {
        RC.setDefaultCubeColor();
        resetCubePrefabPositions();
        RefreshPanels();
    }

    public void scramble(int turns)
    {
        RC.Scramble(turns);
        resetCubePrefabPositions();
        RefreshPanels();
    }

    public bool isAnimationInProgress()
    {
        return isAnimInProgress;
    }

    public int movesToAnim()
    {
        return AnimSeq.Count;
    }

    public float[] getCubeState()
    {
        return RC.getStateAsVec();
    }

    public bool isSolved()
    {
        return RC.isSolved();
    }

    public float getScore()
    {
        return RC.getScore();
    }

    public int turnCount()
    {
        return RC.TurnRecordTokenCount();
    }
}
