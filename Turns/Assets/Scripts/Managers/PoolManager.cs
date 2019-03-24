using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using Lean.Pool;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    public LeanGameObjectPool TilesPool;
}