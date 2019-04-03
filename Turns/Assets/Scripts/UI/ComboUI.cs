using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    public TMP_Text text;

    Transform tr;
    GameObject go;
    Sequence seq;

    void Awake()
    {
        tr = text.transform;
        go = text.gameObject;
    }

    public void Hide()
    {
        seq?.Kill();
        text.gameObject.SetActive(false);
    }

    public void Show(string value)
    {
        text.text = value;
        text.alpha = 0;
        tr.localPosition = new Vector3(-400, 0, 0);
        tr.localScale = Vector3.zero;
        go.SetActive(true);
        
        seq?.Kill();
        seq = DOTween.Sequence()
            .Insert(0, tr.DOLocalMoveX(100, 1.3f).SetEase(Ease.OutCirc))
            .Insert(0, tr.DOScale(1, 0.3f))
            .Insert(0.8f, tr.DOScale(0, 0.6f))
            .Insert(0, text.DOFade(1, 0.6f).SetLoops(2, LoopType.Yoyo));
    }
}
