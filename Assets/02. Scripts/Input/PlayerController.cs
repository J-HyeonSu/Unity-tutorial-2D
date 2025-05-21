using System;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] CharacterController controller;
    [SerializeField, Self] Animator animator;
    [SerializeField, Anywhere] CinemachineVirtualCameraBase freeLookVCam;
    [SerializeField, Anywhere] InputReader input;
    
    [Header("Settings")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float smoothTime = 0.2f;

    const float ZeroF = 0f;
        
    Transform mainCam;

    private float currentSpeed;
    private float velocity;

    void Awake()
    {
        mainCam = Camera.main.transform;
        freeLookVCam.Follow = transform;
        freeLookVCam.LookAt = transform;
        freeLookVCam.OnTargetObjectWarped(transform, transform.position - freeLookVCam.transform.position - Vector3.forward);

    }

    private void OnDisable()
    {
        input.DisableInputActions();
    }


    void Update()
    {
        HandleMovement();
        
    }

    private void HandleMovement()
    {
        var movementDirection = new Vector3(input.Direction.x, 0f, input.Direction.y).normalized;
        var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movementDirection;
        if (adjustedDirection.magnitude > ZeroF)
        {
            HandleRotation(adjustedDirection);

            HandleCharacterController(adjustedDirection);

            SmoothSpeed(adjustedDirection.magnitude);
            
        }
        else
        {
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
        var targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.LookAt(transform.position + adjustedDirection);
    }

    private void SmoothSpeed(float value)
    {
        currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
    }
}