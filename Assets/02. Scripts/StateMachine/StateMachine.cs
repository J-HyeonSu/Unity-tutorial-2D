using System;
using System.Collections.Generic;
 
namespace Platformer
{
    public class StateMachine 
    {
        /*
         StateMachine
            │
            ├── Dictionary<Type, StateNode> nodes
            │   └── StateNode
            │       ├── IState State
            │       └── HashSet<ITransition> Transitions
            │           └── Transition : ITransition
            │               ├── IState To
            │               └── IPredicate Condition
            │
            ├── HashSet<ITransition> anyTransitions
            │   └── Transition (조건만 맞으면 어떤 상태에서든 이동)
            │
            ├── 현재 상태: StateNode current
            │   └── StateNode.State : IState
            │       └── BaseState : IState
            │           ├── LocomotionState
            │           ├── JumpState
            │           └── DashState
            │
            └── 상태 전이 조건: IPredicate
                └── FuncPredicate(Func<bool>)
         
         */
        
        //현재 활성상태 노드
        StateNode current;
        //모든 상태 노드를 타입기반 저장
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransitions = new();

        public void Update() 
        {
            //매 프레임마다 상태 확인
            var transition = GetTransition();
            if (transition != null) 
                ChangeState(transition.To);
            
            current.State?.Update();
        }
        
        public void FixedUpdate() 
        {
            current.State?.FixedUpdate();
        }

        
        
        //외부에서 직접 상태를 설정할 때 사용
        public void SetState(IState state) 
        {
            current = nodes[state.GetType()];
            current.State?.OnEnter();
        }
        
        
        void ChangeState(IState state) 
        {
            
            if (state == current.State) return;
            
            var previousState = current.State;
            var nextState = nodes[state.GetType()].State;
            
            previousState?.OnExit();
            nextState?.OnEnter();
            //현재상태갱신
            current = nodes[state.GetType()];
        }

        ITransition GetTransition() 
        {
            foreach (var transition in anyTransitions)
                if (transition.Condition.Evaluate())
                    return transition;
            
            foreach (var transition in current.Transitions)
                if (transition.Condition.Evaluate())
                    return transition;
            
            return null;
        }

        
        public void AddTransition(IState from, IState to, IPredicate condition) 
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }
        
        public void AddAnyTransition(IState to, IPredicate condition) 
        {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }

        
        StateNode GetOrAddNode(IState state) 
        {
            var node = nodes.GetValueOrDefault(state.GetType());
            
            if (node == null) {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }
            
            return node;
        }

        class StateNode 
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }
            
            public StateNode(IState state) {
                State = state;
                Transitions = new HashSet<ITransition>();
            }
            
            public void AddTransition(IState to, IPredicate condition) {
                Transitions.Add(new Transition(to, condition));
            }
        }
    }
}