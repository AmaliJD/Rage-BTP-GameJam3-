using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalData
{
    private static string[] scenes = new string[] { "Main Menu", "Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6", "Level 7", "Level 8", "Level 9", "Level 10", "Level 11", "Level 12" };
    private static int level_index = 1;
    private static int[] stars = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public static string[] Scene
    {
        get
        {
            return scenes;
        }
        set
        {
            scenes = value;
        }
    }

    public static int Index
    {
        get
        {
            return level_index;
        }
        set
        {
            level_index = value;
        }
    }

    public static int[] Stars
    {
        get
        {
            return stars;
        }
        set
        {
            stars = value;
        }
    }
}
