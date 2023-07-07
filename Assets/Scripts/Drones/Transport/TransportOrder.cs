using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportOrder : MonoBehaviour
{
    public int id;
    public GameObject source;
    public GameObject sink;
    public GameObject load;

    // Start is called before the first frame update
    void Start()
    {
        source = transform.Find($"Source{id}").gameObject;
        sink = transform.Find($"Sink{id}").gameObject;
        load = transform.Find($"Load{id}").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Finished()
    {
        Destroy(gameObject);
    }
}
