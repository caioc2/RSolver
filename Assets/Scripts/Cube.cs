using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube
{

    public enum sides { FRONT = 0, LEFT = 1, BACK = 2, RIGHT = 3, TOP = 4, BOTTOM = 5 };
    public enum colorEnum { RED = 0, BLUE = 1, GREEN = 2, ORANGE = 3, YELLOW = 4, WHITE = 5, BLACK = 6};

   /*public static Color REDCOLOR { get { return Color.red; } }
    public static Color BLUECOLOR { get { return Color.blue; } }
    public static Color GREENCOLOR { get { return Color.green; } }
    public static Color ORANGECOLOR { get { return new Color(1.0f, 0.5f, 0.0f); } }
    public static Color YELLOWCOLOR { get { return Color.yellow; } }
    public static Color WHITECOLOR { get { return Color.white; } }
    public static Color BLACKCOLOR { get { return Color.black; } }*/
    List<colorEnum> colors = new List<colorEnum>();

    public static Color enumToColor(colorEnum c)
    {
        switch(c)
        {
            case colorEnum.RED:
                return Color.red;
            case colorEnum.BLUE:
                return Color.blue;
            case colorEnum.GREEN:
                return Color.green;
            case colorEnum.ORANGE:
                return new Color(1.0f, 0.5f, 0.0f);
            case colorEnum.YELLOW:
                return Color.yellow;
            case colorEnum.WHITE:
                return Color.white;
            case colorEnum.BLACK:
            default:
                return Color.black;
        }
    }

    public Cube()
    {
        for (int i = 0; i < 6; i++) { colors.Add(colorEnum.BLACK); }
        setAllSideColors(colorEnum.BLACK);
    }

    public Cube(List<colorEnum> c)
    {
        for (int i = 0; i < 6; i++) { colors.Add(colorEnum.BLACK); }
        setSideColors(c);
    }

    public void setSideColors(List<colorEnum> colors)
    {
        for (int i = 0; i < 6; i++)
        {
            setSideColor((sides)i, colors[i]);
        }
    }

    public void setAllSideColors(colorEnum c)
    {
        for (int i = 0; i < colors.Count; i++)
        {
            setSideColor((sides)i, c);
        }
    }

    public List<colorEnum> getColors()
    {
        List<colorEnum> tempcolors = new List<colorEnum>();
        for (int i = 0; i < colors.Count; i++)
        {
            tempcolors.Add(colors[i]); 
        }
        return tempcolors;
    }

    public colorEnum getColor(int i)
    {
        return colors[i];
    }

    public void rotateY()
    {
        List<colorEnum> oldColors = getColors();
        for (int i = 0; i < 4; i++)
        {
            setSideColor((sides)i, oldColors[(i + 1) % 4]);
        }
    }

    public void rotateYCC()
    {
        List<colorEnum> oldColors = getColors();
        for (int i = 0; i < 4; i++)
        {
            setSideColor((sides)i, oldColors[(i + 3) % 4]);
        }
    }

    public void rotateX()
    {
        List<colorEnum> oldColors = getColors();
        
        setSideColor(sides.TOP, oldColors[(int)sides.FRONT]);
        setSideColor(sides.BACK, oldColors[(int)sides.TOP]);
        setSideColor(sides.BOTTOM, oldColors[(int)sides.BACK]);
        setSideColor(sides.FRONT, oldColors[(int)sides.BOTTOM]);
    }

    public void rotateXCC()
    {
        List<colorEnum> oldColors = getColors();

        setSideColor(sides.TOP, oldColors[(int)sides.BACK]);
        setSideColor(sides.BACK, oldColors[(int)sides.BOTTOM]);
        setSideColor(sides.BOTTOM, oldColors[(int)sides.FRONT]);
        setSideColor(sides.FRONT, oldColors[(int)sides.TOP]);
    }

    public void rotateZ()
    {
        List<colorEnum> oldColors = getColors();

        setSideColor(sides.TOP, oldColors[(int)sides.LEFT]);
        setSideColor(sides.RIGHT, oldColors[(int)sides.TOP]);
        setSideColor(sides.BOTTOM, oldColors[(int)sides.RIGHT]);
        setSideColor(sides.LEFT, oldColors[(int)sides.BOTTOM]);
    }

    public void rotateZCC()
    {
        List<colorEnum> oldColors = getColors();

        setSideColor(sides.TOP, oldColors[(int)sides.RIGHT]);
        setSideColor(sides.RIGHT, oldColors[(int)sides.BOTTOM]);
        setSideColor(sides.BOTTOM, oldColors[(int)sides.LEFT]);
        setSideColor(sides.LEFT, oldColors[(int)sides.TOP]);
    }

    public void setSideColor(sides side, colorEnum c)
    {
        colors[(int)side] = c;
    }

    public colorEnum getColor(sides side)
    {
        return colors[(int)side];
    }


    public bool containsColors(params colorEnum[] list)
    {
        int matchedColors = 0;
        for (int i = 0; i < list.Length; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (colors[j] == list[i])
                    matchedColors++;
            }
        }

        return (matchedColors == list.Length);
    }

    public int numColors()//number of colors on this piece that are not black
    {
        int num = 0;
        for (int i = 0; i < 6; i++)
        {
            if (colors[i] != colorEnum.BLACK)
                num++;
        }

        return num;
    }
}
