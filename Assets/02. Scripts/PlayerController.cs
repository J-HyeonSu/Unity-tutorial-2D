using System;
using System.Collections.Generic;
using KBCore.Refs;
using Cinemachine;
using DG.Tweening;
using Utilities;
using UnityEngine;



namespace Platformer
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        //자신의 CharacterController, Animator 컴포넌트 참조
        //외부 Cinemacine 카메라 , InputReader(ScriptableObject)
        [Header("References")] 
        [SerializeField, Self] Animator animator;
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
        [SerializeField, Anywhere] InputReader input;

        //설정
        [Header("Settings")] 
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;

        [Header("Jump Setting")] 
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        //[SerializeField] float jumpMaxHeight = 2f;
        [SerializeField] float gravityMultiplier = 3f;

        [Header("Dash Setting")] 
        [SerializeField] private float dashForce = 5f;
        [SerializeField] private float dashDuration = 1f;
        [SerializeField] private float dashCooldown = 2f;

        [Header("Attack Setting")] 
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackDistance = 1f;
        [SerializeField] private int attackDamage = 10;
        
        
        

        const float ZeroF = 0f;

        Transform mainCam;

        private float currentSpeed;
        private float velocity;
        private float jumpVelocity;
        private float dashVelocity = 1f;
        

        private Vector3 movement;

        private List<Timer> timers;
        private CountdownTimer jumpTimer;
        private CountdownTimer jumpCooldownTimer;
        private CountdownTimer dashTimer;
        private CountdownTimer dashCooldownTimer;
        private CountdownTimer attackTimer;
        

        private StateMachine stateMachine;

        //애니메이터 값
        private static readonly int Speed = Animator.StringToHash("Speed");

        void Awake()
        {
            mainCam = Camera.main.transform;

            //시네머신 카메라가 플레이어를 따라가도록 설정
            freeLookVCam.Follow = transform;
            freeLookVCam.LookAt = transform;

            //카메라가 대상의 위치 변경에 따라 위치 재조정
            freeLookVCam.OnTargetObjectWarped(transform,
                transform.position - freeLookVCam.transform.position - Vector3.forward);

            rb.freezeRotation = true;

            SetupTimers();

            SetupStateMachine();
        }

        private void SetupStateMachine()
        {
            //State Machine
            stateMachine = new StateMachine();

            //Declare states
            var locomotionState = new LocomotionState(this, animator);
            var jumpState = new JumpState(this, animator);
            var dashState = new DashState(this, animator);
            var attackState = new AttackState(this, animator);
            var deadState = new DeadState(this, animator);
            

            //Define transitions
            At(locomotionState, jumpState, new FuncPredicate(()=>jumpTimer.IsRunning));
            At(locomotionState, dashState, new FuncPredicate(()=> dashTimer.IsRunning));
            At(locomotionState, attackState, new FuncPredicate(()=> attackTimer.IsRunning));
            At(attackState, locomotionState, new FuncPredicate(()=> !attackTimer.IsRunning));
            Any(deadState, new FuncPredicate(()=>GetComponent<Health>().IsDead));
            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));
            
            
            //Set initial state
            stateMachine.SetState(locomotionState);
        }

        bool ReturnToLocomotionState()
        {
            return groundChecker.IsGrounded 
                   && !jumpTimer.IsRunning 
                   && !dashTimer.IsRunning 
                   && !attackTimer.IsRunning;
        }

        private void SetupTimers()
        {
            //Setup Timer
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            
            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
            
            
            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);

            dashTimer.OnTimerStart += () => dashVelocity = dashForce;
            dashTimer.OnTimerStop += () =>
            {
                dashVelocity = 1f;
                dashCooldownTimer.Start();
            };

            attackTimer = new CountdownTimer(attackCooldown);
            
            
            timers = new List<Timer>(5) { jumpTimer, jumpCooldownTimer , dashTimer, dashCooldownTimer, attackTimer};
        }

        void At(IState from, IState to, IPredicate condition)
        {
            stateMachine.AddTransition(from, to, condition);
        }

        void Any(IState to, IPredicate condition)
        {
            stateMachine.AddAnyTransition(to, condition);
        }

        void Start()
        {
            input.EnablePlayerActions();
        }

        void OnEnable()
        {
            input.Jump += OnJump;
            input.Dash += OnDash;
            input.Attack += OnAttack;
        }

        void OnDisable()
        {
            input.Jump -= OnJump;
            input.Dash -= OnDash;
            input.Attack -= OnAttack;
            input.DisableInputActions();
        }

        void OnAttack()
        {
            if (!attackTimer.IsRunning)
            {
                attackTimer.Start();
            }
        }

        public void Attack()
        {
            Vector3 attackPos = transform.position + transform.forward;
            Collider[] hitEnemies = Physics.OverlapSphere(attackPos, attackDistance);

            foreach (var enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<Health>().TakeDamage(attackDamage);
                }
            }
        }
        void OnJump(bool performed)
        {
            if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded)
            {
                //true , 점프타이머 false, 쿨다운타이머 false, 그라운드체커true
                jumpTimer.Start();
            }
            else if (!performed && jumpTimer.IsRunning)
            {
                //false true
                jumpTimer.Stop();
            }
        }

        void OnDash(bool performed)
        {
            if (performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning)
            {
                dashTimer.Start();
            }
            else if (!performed && dashTimer.IsRunning)
            {
                dashTimer.Stop();
            }
        }


        void Update()
        {
            //입력방향 (x 좌우 , z앞뒤)
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
            stateMachine.Update();

            HandleTimers();
            UpdateAnimator();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        public void HandleJump()
        {
            //점프를 하고있지않고, 땅위에있는경우
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = ZeroF;
                jumpTimer.Stop();
                return;
            }
            
            if (!jumpTimer.IsRunning)
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }
        

        private void UpdateAnimator()
        {
            animator.SetFloat(Speed, currentSpeed);
        }

        public void HandleMovement()
        {
            //카메라 방향 기준으로 방향 보정
            //y축 회전량에 따라 설정 y축회전은 수평회전임
            //adjustedDirection -> 카메라가 보고있는 방향기준으로 입력된 이동방향을 회전시킨 결과
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;


            if (adjustedDirection.magnitude > ZeroF)
            {
                //캐릭터 회전처리
                HandleRotation(adjustedDirection);

                //캐릭터 이동처리
                HandleHorizontalMovement(adjustedDirection);

                //속도 보간
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                //입력이 없을경우 속도 0으로 보간
                SmoothSpeed(ZeroF);

                rb.linearVelocity = new Vector3(ZeroF, rb.linearVelocity.y, ZeroF);
            }
        }

        private void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            Vector3 velocity = adjustedDirection * (moveSpeed * dashVelocity *Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }

        private void HandleRotation(Vector3 adjustedDirection)
        {
            // adjustedDirection 방향을 바라보는 회전값(Quaternion)을 계산한다.
            var targetRotation = Quaternion.LookRotation(adjustedDirection);

            // 현재 회전(transform.rotation)에서 targetRotation까지 rotationSpeed 속도로 부드럽게 회전시킨다.
            // Time.deltaTime을 곱해 프레임에 맞춘 부드러운 회전을 만든다.
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.LookAt(transform.position + adjustedDirection);
        }

        private void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }

        
    }
}