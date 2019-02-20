using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RubiksCubePrefab2 : RubiksCubePrefab
{
    // Use this for initialization
    void Awake()
    {
        RC = new RubiksCube2();
        initCube();
    }
}