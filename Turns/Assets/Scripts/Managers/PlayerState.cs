using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
   public int MaxMoves;
   public int LastMoves;
   public int LastPoints;
   public int MaxPoints;
   public int Gems;
   public float RunningTime;
   public int TilesPassed;

   public void Load()
   {
      MaxMoves = PlayerPrefs.GetInt("moves", 0);
      MaxPoints = PlayerPrefs.GetInt("points", 0);
   }

   public void SaveRun(int movesMade, float time, int tilesPassed, int points)
   {
      RunningTime = time;
      TilesPassed = tilesPassed;
      LastMoves = movesMade;
      LastPoints = points;
      if (MaxMoves < movesMade)
      {
         MaxMoves = movesMade;
         PlayerPrefs.SetInt("moves", MaxMoves);
      }
      if (MaxPoints < points)
      {
         MaxPoints = points;
         PlayerPrefs.SetInt("points", MaxPoints);
      }
   }
}
