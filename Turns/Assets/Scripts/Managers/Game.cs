﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Game : Singleton<Game>
{
    public TMP_Text _debug;
    public bool IsStarted { get; private set; }

    // time player takes to pass a tile
    public float initialTilePassTime = 0.4f;
    public float fastestTilePassTime = 0.1f;
    public int InitialTiles = 6;
    public int PlayerPassTilesBuffer = 1; // initial player wait
    public int StateChangeChances = 7;
    [Range(0,100)]
    public int HoleChangePondere = 50;
    [Range(0,100)]
    public int StairePondere = 50;

    public int MaxStage = 10;
    public int MovesInStage = 4;
    public int DirChageMinDistance = 1;
    public int HoleLength = 2;
    public int HolesMinDistance = 2;
    public int StairsLength = 2;
    public int StairsMinDistance = 2;
    public int MovesMade { get; private set; }

    float _startTime;
    public int Stage { get; private set; }

    public float TilePassTime =>
        Mathf.Lerp(fastestTilePassTime, initialTilePassTime, (MaxStage - Stage) / (float) MaxStage);

    public void Reset()
    {
        MovesMade = 0;
        Stage = 0;
        FloorManager.Instance.Reset();
        Player.Instance.Reset();
    }

    public void Play()
    {
        Reset();

        StartCoroutine(BeginAfterCountdown());
        IsStarted = true;
    }

    IEnumerator BeginAfterCountdown()
    {
        FloorManager.Instance.Play();
        Player.Instance.Play();
        CameraFollow.Instance.SetForGame();
        yield return new WaitForSeconds(0.6f);
        _startTime = Time.fixedTime;
    }

    public void UserTap()
    {
        if (Player.Instance.IsRunning && !Player.Instance.IsJumping)
        {
            var tile = FloorManager.Instance.PeekTile(Player.Instance.CurrentTilePosition.Value);
            if (tile == null || tile.IsHole)
            {
                return;
            }

            OperationsManager.Instance.DoNextAction();
            MovesMade++;
            Stage = MovesMade / MovesInStage;
            Menu.Instance.UpdateUI();
        }
    }

    public void PlayerDie()
    {
        Player.Instance.SlowDownAndDie();
        GameManager.Instance.GameOver();
        IsStarted = false;
    }
}