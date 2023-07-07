using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportCreationBehavior : MonoBehaviour
{
    TransportManager transportManager;

    public bool active = false;

    
    public float timeOnGround = 0f;
    public bool orderCreated = true;

    // Start is called before the first frame update
    void Start()
    {
        transportManager = GameObject.Find("Transports").GetComponent<TransportManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(transform.position.y < 0.1f)
        {
            timeOnGround += Time.deltaTime;
        } else
        {
            orderCreated = false;
            timeOnGround = 0f;
        }

        if (timeOnGround > 0.5f && !orderCreated) {
            transportManager.CreateTransportOrder(transform.position);
            orderCreated = true;
        }

    }



}
