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
        DisplayMoves();
    }

    public void DisplayMoves()
    {
        movesText.text = "Moves made: " + Game.Instance.MovesMade.ToString();
    }

    public void StartAnotherGame()
    {
        Game.Instance.Play();
        Menu.Instance.Hide();
        DisplayMoves();
    }

    public void GameOver()
    {
        Menu.Instance.Show();
    }
}
