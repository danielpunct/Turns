using Gamelogic.Extensions;
using UnityEngine;

public class RunnerController_CH : Singleton<RunnerController_CH>
{
   public Animator _anim;
   public Rigidbody _rb;

   float forwardCache = -1;
   bool groundedCache = true;
   
   [SerializeField] float m_GroundCheckDistance = 0.1f;


   float m_OrigGroundCheckDistance;
   float m_ForwardAmount;
   bool m_IsGrounded;
   float m_TurnAmount;
   bool m_Crouching;
   
   void Start()
   {
      m_OrigGroundCheckDistance = m_GroundCheckDistance;
   }

   void Update()
   {
      if (Runner.Instance.IsFalling)
      {
         m_IsGrounded = false;
      }

      if (Runner.Instance.IsRunning)
      {
         m_ForwardAmount = Runner.Instance.Speed * 10;
         m_GroundCheckDistance = 0.1f;
         
         
//         m_TurnAmount = Mathf.Atan2(Player.Instance.Direction.x, Player.Instance.Direction.z);
      }

      if (!m_IsGrounded)
      {
         HandleAirborneMovement();
      }

      if (Runner.Instance.IsRunning || Runner.Instance.IsFalling)
      {
         UpdateAnimator();
      }
   }

   public void Reset()
   {
      m_ForwardAmount = 0;
      m_TurnAmount = 0;
      m_Crouching = false;
      m_IsGrounded = false;
      UpdateAnimator();
      _anim.Rebind();
   }


//   void HandleGroundedMovement(bool crouch, bool jump)
//   {
//      // check whether conditions are right to allow a jump:
//      if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
//      {
//         // jump!
//         m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
//         m_IsGrounded = false;
//         m_Animator.applyRootMotion = false;
//         m_GroundCheckDistance = 0.1f;
//      }
//   }
   
   void HandleAirborneMovement()
   {
      // apply extra gravity from multiplier:
      Vector3 extraGravityForce = (Physics.gravity * 2) - Physics.gravity;
      _rb.AddForce(extraGravityForce);

      if (Mathf.Abs( _rb.velocity.y) >0.01f)
      {
         m_GroundCheckDistance = m_OrigGroundCheckDistance;
         m_IsGrounded = false;
      }
      else
      {
         m_GroundCheckDistance = 0.01f;
         m_IsGrounded = true;
      }
   }
   
   void UpdateAnimator()
   {
      // update the animator parameters
      _anim.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
//      _anim.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
      _anim.SetBool("Crouch", m_Crouching);
      _anim.SetBool("OnGround", m_IsGrounded);
      if (!m_IsGrounded)
      {
         _anim.SetFloat("Jump", Mathf.Min(-5,_rb.velocity.y));
      }
   }
}
