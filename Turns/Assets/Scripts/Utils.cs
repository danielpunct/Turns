using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static bool GetRand100Pondere(int pondere)
    {
        return Random.Range(0, 100) <= pondere;
    }
}
