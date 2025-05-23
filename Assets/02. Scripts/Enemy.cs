using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

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

        private StateMachine stateMachine;

        void OnValidate() => this.ValidateRefs();

        void Start()
        {
            stateMachine = new StateMachine();

            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            
            At(wanderState, chaseState, new FuncPredicate(()=> playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(()=> !playerDetector.CanDetectPlayer()));
            //Any(wanderState, new FuncPredicate(()=> true));
            stateMachine.SetState(wanderState);
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void Update()
        {
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }
        
    }
}