using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrazySwarmTracker : MonoBehaviour, IMQTTSubscriber
{
    public const string TOPIC_REQUEST = "crazyflie/positions";
    public GameObject MQTTManagerGO;
    public MQTTManager manager;  

    public ConcurrentDictionary<string, Entry> MessageDictionary { get; private set; } = new ConcurrentDictionary<string, Entry>();
    public Dictionary<string, GameObject> Drones = new Dictionary<string, GameObject>();

    public bool error = false;
    public string errorMessage = "";

    SwarmCreator swarmCreator;

    // Start is called before the first frame update
    void Start()
    {
        MQTTManagerGO = GameObject.Find("MQTTManager");
        manager = MQTTManagerGO.GetComponent<MQTTManager>();
        manager.Subscribe(TOPIC_REQUEST, this);
        swarmCreator = GetComponent<SwarmCreator>();
    }

    // Update is called once per frame
    void Update()
    {
        //create drones that are in MQTT but not in renderer and that have fresh timestamp
        foreach (Entry msgEntry in MessageDictionary.Select(e => e.Value).Where(e => e.Data.id.StartsWith("cf")))
        {
            if (!IsOld(msgEntry.Timestamp, Time.time))
            {
                if (!Drones.ContainsKey(msgEntry.Data.id))
                {
                    var numberIdStr = msgEntry.Data.id.Remove(0, 2);
                    if (Int32.TryParse(numberIdStr, out var id))
                    {
                        Debug.Log($"Adding drone {msgEntry.Data.id}");
                        Drones.Add(msgEntry.Data.id, swarmCreator.CreateDrone(id, true));
                    }
                }
            }
            else
            {
                //delete drones that are in not fresh anymore, i.e. were not updated for longer than a second
                if (Drones.ContainsKey(msgEntry.Data.id))
                {
                    Debug.Log($"Removing drone {msgEntry.Data.id}");
                    var drone = Drones[msgEntry.Data.id];
                    Drones.Remove(msgEntry.Data.id);
                    Destroy(drone);
                }
            }
        }
    }
    private bool IsOld(float timestamp, float currenttime)
    {
        return currenttime - timestamp > 1;
    }


    public void Receive(string topic, string message)
    {
        try
        {
            Entry entry = new Entry();
            MQTTData data = JsonConvert.DeserializeObject<MQTTData>(message);
            entry.Data = data;
            entry.Timestamp = Time.time;
            MessageDictionary.AddOrUpdate(entry.Data.id, entry, (key, oldValue) => entry);

        }
        catch (JsonSerializationException ex)
        {
            if (!error)
            {
                errorMessage = ex.Message;
                error = true;
            }
        }
    }


    public class Entry
    {
        public MQTTData Data { get; set; }
        public float Timestamp { get; set; } = 0;
    }

    public class MQTTData
    {
        public string id { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

}
