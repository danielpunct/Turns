using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
   public int MaxMoves;
   public int Gems;
   public float RunningTime;
   public int TilesPassed;

   public void Save()
   {
      if (MaxMoves > Game.Instance.MovesMade)
      {
         MaxMoves = Game.Instance.MovesMade;
         PlayerPrefs.SetInt("moves", MaxMoves);
      }
   }

   public void Load()
   {
      MaxMoves = PlayerPrefs.GetInt("moves", 0);
   }

   public void LogRunningTime(float time, int tilesPassed)
   {
      RunningTime = time;
      TilesPassed = tilesPassed;
   }
}
