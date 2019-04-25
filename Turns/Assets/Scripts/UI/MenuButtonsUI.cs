using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MenuButtonsUI : MonoBehaviour
{
    public CanvasGroup buttonHolder1;
    public CanvasGroup buttonHolder2;
    public CanvasGroup buttonHolder3;
    public CanvasGroup buttonHolder4;


    public void _Reset()
    {
        buttonHolder1.alpha = 0;
        buttonHolder2.alpha = 0;
        buttonHolder3.alpha = 0;
        buttonHolder4.alpha = 0;

        buttonHolder1.transform.DOLocalMoveY(-40, 0);
        buttonHolder2.transform.DOLocalMoveY(-40, 0);
        buttonHolder3.transform.DOLocalMoveY(-40, 0);
        buttonHolder4.transform.DOLocalMoveY(-40, 0);
    }
    
    public void SwitchButtons(Sequence seq, float offset, bool on, bool farther)
    {
        const float buttonsDuration = 0.35f;
        var eachOffset = on ? 0.07f : 0;
        var fadeoffset = on ? 0 : buttonsDuration / 2f;

        seq.Insert(offset + fadeoffset + eachOffset * 0.5f * 0, buttonHolder1.DOFade(on ? 1 : 0, buttonsDuration))
            .Insert(offset + fadeoffset + eachOffset * 0.5f * 1, buttonHolder2.DOFade(on ? 1 : 0, buttonsDuration))
            .Insert(offset + fadeoffset + eachOffset * 0.5f * 2, buttonHolder3.DOFade(on ? 1 : 0, buttonsDuration))
            .Insert(offset + fadeoffset + eachOffset * 0.5f * 3, buttonHolder4.DOFade(on ? 1 : 0, buttonsDuration))
            .Insert(offset + eachOffset * 0,
                buttonHolder1.transform.DOLocalMoveY(on ? 0 : (farther ? -530 : -40), buttonsDuration)
                    .SetEase(Ease.OutBack))
            .Insert(offset + eachOffset * 1,
                buttonHolder2.transform.DOLocalMoveY(on ? 0 : (farther ? -530 : -40), buttonsDuration)
                    .SetEase(Ease.OutBack))
            .Insert(offset + eachOffset * 2,
                buttonHolder3.transform.DOLocalMoveY(on ? 0 : (farther ? -530 : -40), buttonsDuration)
                    .SetEase(Ease.OutBack))
            .Insert(offset + eachOffset * 3,
                buttonHolder4.transform.DOLocalMoveY(on ? 0 : (farther ? -530 : -40), buttonsDuration)
                    .SetEase(Ease.OutBack));
    }
}