using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private float groundDistance = 0.08f;
    [SerializeField] private LayerMask groundLayer;
    
    public bool IsGrounded { get; private set; }

    void Update()
    {
        IsGrounded = Physics.SphereCast(transform.position, groundDistance, Vector3.down, out _, groundDistance, groundLayer);
    }
}