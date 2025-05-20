using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform target;

    public Vector3 Offset;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (target == null) return;

        var desiredPosition = target.position + Offset;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, desiredPosition, Time.deltaTime);
        mainCamera.transform.LookAt(target.position);
    }
}