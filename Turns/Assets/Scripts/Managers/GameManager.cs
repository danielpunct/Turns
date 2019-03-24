using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    void Start()
    {
        Application.targetFrameRate = 60;
        Game.Instance.Reset();
        Menu.Instance.ShowMenu(true);
        Menu.Instance.UpdateUI();
    }


    public void StartAnotherGame()
    {
        Game.Instance.Play();
        Menu.Instance.ShowGameMenu();
        Menu.Instance.UpdateUI();
    }

    public void GameOver()
    {
        Menu.Instance.ShowMenu(false);
    }
}
