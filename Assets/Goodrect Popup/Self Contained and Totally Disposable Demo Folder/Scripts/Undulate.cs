using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undulate : MonoBehaviour
{
    private float t;
    private float speed = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        t = Random.Range(0, Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        float y = Mathf.Sin(t += speed);
        y += 1;
        y /= 10;

        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
    }
}
