using UnityEngine;

public class Study_Class
{
    public int number;

    public Study_Class(int num)
    {
        this.number = num;
    }
}

public struct Study_Struct
{
    public int number;
    
    public Study_Struct(int num)
    {
        this.number = num;
    }
}
public class Study_ClassStruct : MonoBehaviour
{
    void Start()
    {
        Debug.Log("클래스---------");
        Study_Class c1 = new Study_Class(10);
        Study_Class c2 = c1;
        Debug.Log($"c1 : {c1.number} / c2 : {c2.number}");
        
        Debug.Log("구조체----------");
        Study_Struct s1 = new Study_Struct(10);
        Study_Struct s2 = s1;
        Debug.Log($"s1 : {s1.number} / s2 : {s2.number}");


    }
    
}
