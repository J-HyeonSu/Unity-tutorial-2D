using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public LayerMask groundLayer;
    public float groundCheckDistance = .5f;
    private Vector3 moveDirection;

    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // ad
        float moveY = Input.GetAxis("Vertical"); // ws
        
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * moveY + right * moveX).normalized;
        

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * (moveSpeed * Time.deltaTime));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * (groundCheckDistance));
    }

    private bool IsGrounded()
    {
        bool hit = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        Debug.Log("IsGrounded: " + hit);
        return hit;
    }
}