using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalData
{
    private static string[] scenes = new string[] { "Level 5", "Level 1", "Level 2", "Level 3", "Level 4", "Level 5" };
    private static int level_index = 1;
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
}
