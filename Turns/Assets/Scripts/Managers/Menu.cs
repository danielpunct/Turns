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

   public void Show(bool init)
   {
      gameObject.SetActive(true);
      StartCoroutine(ShowLate(init));
   }

   IEnumerator ShowLate(bool init)
   {
      if (!init)
      {
         yield return new WaitForSeconds(3);
      }

      lateHolder.transform.localScale = Vector3.zero;
      lateHolder.transform.DOScale(1, 0.6f).SetEase(Ease.OutBack);
      lateHolder.SetActive(true);
      Player.Instance.Reset(); // need to be done before camera
      CameraFollow.Instance.SetForMenu();
      FloorManager.Instance.Reset();
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
