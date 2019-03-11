using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class RunnerController : MonoBehaviour
{
   public Animator animator;

   void Update()
   {
      if (Game.Instance.PlayerRunning)
      {
         animator.SetFloat("Forward", Player.Instance.Speed * 8);
      }
   }
}
