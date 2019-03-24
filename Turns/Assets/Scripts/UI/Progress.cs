using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    public TMP_Text currentLevel;
    public TMP_Text nextLevel;
    public Slider progressSlider;   
    public TMP_Text movesText;


    public void Display()
    {
        currentLevel.text = Game.Instance.Stage.ToString();
        nextLevel.text = (Game.Instance.Stage + 1).ToString();
        var newValue = (Game.Instance.MovesMade % Game.Instance.MovesInStage) / (float) Game.Instance.MovesInStage;
        progressSlider.DOValue(newValue, newValue > 0 ? 0.3f : 0);
        movesText.text = Game.Instance.MovesMade.ToString();

    }
}
