using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestActions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestActionA() {
        Debug.Log("A");
    }

    public void TestActionB() {
        Debug.Log("B");
    }

    public void TestActionC() {
        Debug.Log("C");
    }

    public void TestReturn()
    {
        Debug.Log("-FIN-");
    }

    public void TestPromptReturn(string val)
    {
        Debug.Log(val);
    }
}
