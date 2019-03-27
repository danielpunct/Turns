using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
   public int MaxMoves;
   public int LastMoves;
   public int Gems;
   public float RunningTime;
   public int TilesPassed;

   public void Load()
   {
      MaxMoves = PlayerPrefs.GetInt("moves", 0);
   }

   public void SaveRun(int movesMade, float time, int tilesPassed)
   {
      RunningTime = time;
      TilesPassed = tilesPassed;
      LastMoves = movesMade;
      if (MaxMoves < movesMade)
      {
         MaxMoves = movesMade;
         PlayerPrefs.SetInt("moves", MaxMoves);
      }
   }
}
