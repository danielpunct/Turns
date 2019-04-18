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
   public int MaxLevelPassed;
   public int LevelSelected;


   public void Load()
   {
      MaxMoves = PlayerPrefs.GetInt("moves", 0);
      MaxPoints = PlayerPrefs.GetInt("points", 0);
      MaxLevelPassed = PlayerPrefs.GetInt("level_passed", 0);
      LevelSelected = 0;
   }

   public void SaveRun(int movesMade, float time, int tilesPassed, int points, int levelPassed, bool died)
   {
      RunningTime = time;
      TilesPassed = tilesPassed;
      LastMoves = movesMade;
      LastPoints = points;


      UpdateMaxValue(ref MaxMoves, movesMade, "moves");
      UpdateMaxValue(ref MaxPoints, points, "points");
      UpdateMaxValue(ref MaxLevelPassed, levelPassed, "level_passed");

      UpdateValue(ref LevelSelected, died ? 0 : levelPassed, "level_selected");
   }

   void UpdateMaxValue(ref int value, int newValue, string key)
   {
      if (value < newValue)
      {
         value = newValue;
         PlayerPrefs.SetInt(key, newValue);
      }
   }

   void UpdateValue(ref int value, int newValue, string key)
   {
      value = newValue;
      PlayerPrefs.SetInt(key, newValue);
   }
}
