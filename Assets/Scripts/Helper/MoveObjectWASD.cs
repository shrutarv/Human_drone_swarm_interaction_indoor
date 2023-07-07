using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectWASD : MonoBehaviour
{
    Camera cam;
    public float speed = 10;
    public float spacing = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        var forward = cam.transform.forward;
        var right = cam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(forward * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-right * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-forward * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(right * speed * Time.deltaTime);
        }

        

    }
}
