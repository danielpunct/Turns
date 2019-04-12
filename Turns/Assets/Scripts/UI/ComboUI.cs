using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
    public TMP_Text text;
    public AnimationCurve textPopCurve;

    Transform tr;
    GameObject go;
    Sequence seq;

    void Awake()
    {
        tr = text.transform;
        go = text.gameObject;
    }

    void Start()
    {
        Hide();
    }

    public void Hide()
    {
        seq?.Kill();
        text.gameObject.SetActive(false);
    }

    public void Show(string value)
    {
        text.text = value;
        tr.localPosition = Vector3.zero;
        tr.localScale = Vector3.zero;
        text.alpha = 1;
        go.SetActive(true);
        
        seq?.Kill();
        seq = DOTween.Sequence()
            .Insert(0, tr.DOScale(1, 0.75f).SetEase(textPopCurve))
            .Insert(0.5f, tr.DOLocalMoveY(0.6f, 0.6f))
            .Insert(0.5f, text.DOFade(0, 0.6f).SetEase(Ease.InCubic));
    }
}
