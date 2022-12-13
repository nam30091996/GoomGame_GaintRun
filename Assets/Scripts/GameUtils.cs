using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static int GetHp(int level)
    {
        return 100 + 28 * level;
    }
    
    public static int GetDame(int level)
    {
        return 10 + 5 * level;
    }
}
