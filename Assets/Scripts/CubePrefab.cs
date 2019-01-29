using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubePrefab : MonoBehaviour
{
    public List<GameObject> panels = new List<GameObject>();

    public void refreshPanels(Cube c)
    {
        List<Cube.colorEnum> cc = c.getColors();
        for (int i= 0; i < 6; i++)
        {
            Renderer rend = panels[i].GetComponent<Renderer>();
            rend.enabled = true;
            rend.material.color = Cube.enumToColor(cc[i]);
        }
    }
}
