using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
    public int lifeExpectancy = 15;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Kill", lifeExpectancy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
