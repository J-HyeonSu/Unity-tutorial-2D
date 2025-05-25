using UnityEngine;

namespace Platformer
{
    public class DeadState : BaseState
    {
        public DeadState(PlayerController player, Animator animator) : base(player, animator)
        {
        }
        public override void OnEnter()
        {
            animator.CrossFade(DeadHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            
        }
    }
}