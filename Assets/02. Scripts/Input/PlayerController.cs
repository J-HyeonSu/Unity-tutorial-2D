using System;
using System.Collections.Generic;
using KBCore.Refs;
using Cinemachine;
using Utilities;
using UnityEngine;

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

    [Header("JumpSetting")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float jumpDuration = 0.5f;
    [SerializeField] float jumpCooldown = 0f;
    [SerializeField] float jumpMaxHeight = 2f;
    [SerializeField] float gravityMultiplier = 3f;
    
    const float ZeroF = 0f;
        
    Transform mainCam;

    private float currentSpeed;
    private float velocity;
    private float jumpVelocity;
    
    private Vector3 movement;

    private List<Timer> timers;
    private CountdownTimer jumpTimer;
    private CountdownTimer jumpCooldownTimer;
    
    //애니메이터 값
    private static readonly int Speed = Animator.StringToHash("Speed");

    void Awake()
    {
        mainCam = Camera.main.transform;
        
        //시네머신 카메라가 플레이어를 따라가도록 설정
        freeLookVCam.Follow = transform;
        freeLookVCam.LookAt = transform;
        
        //카메라가 대상의 위치 변경에 따라 위치 재조정
        freeLookVCam.OnTargetObjectWarped(transform, transform.position - freeLookVCam.transform.position - Vector3.forward);

        rb.freezeRotation = true;
        
        //Setup Timer
        jumpTimer = new CountdownTimer(jumpDuration);
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        timers = new List<Timer>(2) { jumpTimer, jumpCooldownTimer };
        jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        
    }

    void Start()
    {
        input.EnablePlayerActions();
    }

    void OnEnable()
    {
        input.Jump += OnJump;
    }

    void OnDisable()
    {
        input.Jump -= OnJump;
        input.DisableInputActions();
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


    void Update()
    {
        //입력방향 (x 좌우 , z앞뒤)
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);

        HandleTimers();
        UpdateAnimator();

    }

    void FixedUpdate()
    {
        //매프레임 이동처리
        HandleMovement();
        HandleJump();
    }

    void HandleTimers()
    {
        foreach (var timer in timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }

    void HandleJump()
    {
        
        //점프를 하고있지않고, 땅위에있는경우
        if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
        {
            jumpVelocity = ZeroF;
            jumpTimer.Stop();
            return;
        }
        
        //점프중일경우
        if (jumpTimer.IsRunning)
        {
            
            float launchPoint = 0.9f;
            
            
            if (jumpTimer.Progress > launchPoint)
            {
                jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * MathF.Abs(Physics.gravity.y));
            }
            else
            {
                jumpVelocity += (1 - jumpTimer.Progress) * jumpForce * Time.fixedDeltaTime;
            }
        }
        else
        {
            jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
        }

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(Speed, currentSpeed);
    }

    private void HandleMovement()
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
        Vector3 velocity = adjustedDirection * (moveSpeed * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        
    }

    private void HandleRotation(Vector3 adjustedDirection)
    {
        // adjustedDirection 방향을 바라보는 회전값(Quaternion)을 계산한다.
        var targetRotation = Quaternion.LookRotation(adjustedDirection);
        
        // 현재 회전(transform.rotation)에서 targetRotation까지 rotationSpeed 속도로 부드럽게 회전시킨다.
        // Time.deltaTime을 곱해 프레임에 맞춘 부드러운 회전을 만든다.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.LookAt(transform.position + adjustedDirection);
    }

    private void SmoothSpeed(float value)
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
}