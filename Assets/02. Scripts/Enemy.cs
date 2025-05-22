using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : Entity
    {
        [SerializeField, Self] private NavMeshAgent agent;
        [SerializeField, Child] private Animator animator;

        private StateMachine stateMachine;

        void OnValidate() => this.ValidateRefs();

        void Start()
        {
            stateMachine = new StateMachine();

            var wanderState = new EnemyWanderState(this, animator, agent, 5f);
            
            Any(wanderState, new FuncPredicate(()=> true));
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