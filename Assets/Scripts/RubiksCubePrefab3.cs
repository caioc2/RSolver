﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RubiksCubePrefab3 : RubiksCubePrefab
{
    // Use this for initialization
    void Awake()
    {
        RC = new RubiksCube3();  
        initCube();
    }
}