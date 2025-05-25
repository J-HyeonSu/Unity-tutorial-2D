using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Platformer
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity
    {
        [SerializeField, Self] private NavMeshAgent agent;
        [SerializeField, Self] private PlayerDetector playerDetector;
        [SerializeField, Child] private Animator animator;
        [SerializeField] private float wanderRadius = 10;
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private float deadTime = 10f;

        private StateMachine stateMachine;
        private CountdownTimer attackTimer;
        private CountdownTimer deadTimer;
        

        void OnValidate() => this.ValidateRefs();

        void Start()
        {
            stateMachine = new StateMachine();
            attackTimer = new CountdownTimer(timeBetweenAttacks);
            deadTimer = new CountdownTimer(deadTime);

            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
            var deadState = new EnemyDeadState(this, animator, agent, playerDetector.Player);
            
            At(wanderState, chaseState, new FuncPredicate(()=> playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(()=> !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(()=> playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(()=> !playerDetector.CanAttackPlayer()));
            Any(deadState, new FuncPredicate(()=> GetComponent<Health>().IsDead));
            stateMachine.SetState(wanderState);
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void Update()
        {
            stateMachine.Update();
            attackTimer.Tick(Time.deltaTime);
            deadTimer.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void Attack()
        {
            if (attackTimer.IsRunning) return;

            attackTimer.Start();
            playerDetector.PlayerHealth.TakeDamage(attackDamage);
            
        }

        
        public void Dead()
        {
            if (deadTimer.IsRunning) return;
            
            
            Destroy(gameObject);
        }

        public void DeadTimerStart()
        {
            deadTimer.Start();
        }
        
    }
}