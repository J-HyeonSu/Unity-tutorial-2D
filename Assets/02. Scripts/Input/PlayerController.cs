using System;
using KBCore.Refs;
using Cinemachine;
using UnityEngine;

public class PlayerController : ValidatedMonoBehaviour
{
    //자신의 CharacterController, Animator 컴포넌트 참조
    //외부 Cinemacine 카메라 , InputReader(ScriptableObject)
    [Header("References")]
    [SerializeField, Self] CharacterController controller;
    [SerializeField, Self] Animator animator;
    [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
    [SerializeField, Anywhere] InputReader input;
    
    //설정
    [Header("Settings")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float smoothTime = 0.2f;

    const float ZeroF = 0f;
        
    Transform mainCam;

    private float currentSpeed;
    private float velocity;
    
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

    }

    void Start()
    {
        input.EnablePlayerActions();
    }

    void OnDisable()
    {
        input.DisableInputActions();
    }


    void Update()
    {
        //매프레임 이동처리
        HandleMovement();
        UpdateAnimator();

    }

    private void UpdateAnimator()
    {
        animator.SetFloat(Speed, currentSpeed);
    }

    private void HandleMovement()
    {
        //입력방향 (x 좌우 , z앞뒤)
        var movementDirection = new Vector3(input.Direction.x, 0f, input.Direction.y).normalized;
        
        //카메라 방향 기준으로 방향 보정
        //y축 회전량에 따라 설정 y축회전은 수평회전임
        //adjustedDirection -> 카메라가 보고있는 방향기준으로 입력된 이동방향을 회전시킨 결과
        var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movementDirection;
        
        
        if (adjustedDirection.magnitude > ZeroF)
        {
            //캐릭터 회전처리
            HandleRotation(adjustedDirection);
            
            //캐릭터 이동처리
            HandleCharacterController(adjustedDirection);

            //속도 보간
            SmoothSpeed(adjustedDirection.magnitude);
            
        }
        else
        {
            //입력이 없을경우 속도 0으로 보간
            SmoothSpeed(ZeroF);
        }
    }

    private void HandleCharacterController(Vector3 adjustedDirection)
    {
        var adjustedMovement = adjustedDirection * (moveSpeed * Time.deltaTime);
        controller.Move(adjustedMovement);
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