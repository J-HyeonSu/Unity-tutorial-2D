using UnityEngine;

public class StudyLog : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Debug.Log("Start 함수 실행");
        Debug.LogWarning("Start 함수 실행");
        Debug.LogError("Start 함수 실행");
        
    }

    // Update is called once per frame
    private void Update()
    {
    }
}