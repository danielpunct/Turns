using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public TMP_Text movesText;

    void Start()
    {
        Game.Instance.Reset();
        Menu.Instance.Show();
        UpdateUI();
    }

    public void UpdateUI()
    {
        movesText.text = "Moves made: " + Game.Instance.MovesMade.ToString();
    }

    public void StartAnotherGame()
    {
        Game.Instance.Play();
        Menu.Instance.Hide();
        UpdateUI();
    }

    public void GameOver()
    {
        Menu.Instance.Show();
    }
}
