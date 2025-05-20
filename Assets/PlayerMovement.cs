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
        var moveX = Input.GetAxis("Horizontal");
        var moveY = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveX, 0f, moveY).normalized;
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded()) rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * (groundCheckDistance + 0.1f));
    }

    private bool isGrounded()
    {
        var hit = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.3f, groundLayer);
        Debug.Log("IsGrounded: " + hit);
        return hit;
    }
}