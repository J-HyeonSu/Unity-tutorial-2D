using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float jumpForce = 5f;
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.position += Vector3.forward * (Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position += Vector3.back * (Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.position += Vector3.left * (Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += Vector3.right * (Time.deltaTime * moveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.transform.position += Vector3.up * (Time.deltaTime * jumpForce);
        }
        
    }
}
