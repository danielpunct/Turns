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

    Tween _t;

    public void Display()
    {
        currentLevel.text = (Game.Instance.CurrentStage).ToString();
        nextLevel.text = (Game.Instance.CurrentStage + 1).ToString();
        var newValue = Game.Instance.StageFinishable
            ? 1
            : (FloorManager.Instance.TilesPassed % Game.Instance.TilesInStage) / (float) Game.Instance.TilesInStage;
        
        _t?.Kill();
        _t = progressSlider.DOValue(newValue, newValue > 0 ? 0.3f : 0);
        pointsText.text = (Game.Instance.PerfectPoints + Game.Instance.MovesMade).ToString();
    }
}
