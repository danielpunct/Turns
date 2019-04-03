using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    public TMP_Text currentLevel;
    public TMP_Text nextLevel;
    public Slider progressSlider;   
    public TMP_Text pointsText;

    Tween t;

    public void Display()
    {
        currentLevel.text = (Game.Instance.Stage + 1).ToString();
        nextLevel.text = (Game.Instance.Stage + 2).ToString();
        var newValue = (FloorManager.Instance.TilesPassed % Game.Instance.TilesInStage) / (float) Game.Instance.TilesInStage;
        t?.Kill();
        t = progressSlider.DOValue(newValue, newValue > 0 ? 0.3f : 0);
        pointsText.text = (Game.Instance.PerfectPoints + Game.Instance.MovesMade).ToString();
    }
}
