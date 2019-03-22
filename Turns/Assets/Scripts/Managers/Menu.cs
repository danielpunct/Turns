using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class Menu : Singleton<Menu>
{
   [Header("Menu")] 
   public GameObject menuUIHolder;
   public GameObject lateHolder;

   [Header("Game")]
   public GameObject gameUIHolder;

   void Awake()
   {
      lateHolder.transform.localScale = Vector3.zero;
   }

   public void ShowMenu(bool init)
   {
      menuUIHolder.SetActive(true);
      gameUIHolder.SetActive(false);
      StartCoroutine(ShowLateUIHolder(init));
   }

   IEnumerator ShowLateUIHolder(bool init)
   {
      if (!init)
      {
         yield return new WaitForSeconds(1.5f);
      }

      lateHolder.transform.localScale = Vector3.zero;
      lateHolder.transform.DOScale(1, 0.6f).SetEase(Ease.OutBack);
      lateHolder.SetActive(true);
      Player.Instance.Reset(); // need to be done before camera
      CameraFollow.Instance.SetForMenu();
      FloorManager.Instance.Reset();
   }


   public void ShowGameMenu()
   {
      menuUIHolder.SetActive(false);
      gameUIHolder.SetActive(true);
      lateHolder.SetActive(false);
   }
   
   public void OnPlayClick()
   {
      GameManager.Instance.StartAnotherGame();
   }
}
