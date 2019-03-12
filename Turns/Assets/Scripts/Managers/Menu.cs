using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;

public class Menu : Singleton<Menu>
{
   public GameObject lateHolder;

   void Awake()
   {
      lateHolder.transform.localScale = Vector3.zero;
   }

   public void Show()
   {
      gameObject.SetActive(true);
      StartCoroutine(ShowLate());
   }

   IEnumerator ShowLate()
   {
      yield return new WaitForSeconds(1);
      lateHolder.transform.localScale = Vector3.zero;
      lateHolder.transform.DOScale(1, 0.6f).SetEase(Ease.OutBack);
      lateHolder.SetActive(true);
   }

   public void Hide()
   {
      gameObject.SetActive(false);
      lateHolder.SetActive(false);
   }
   
   public void OnPlayClick()
   {
      GameManager.Instance.StartAnotherGame();
   }
}
