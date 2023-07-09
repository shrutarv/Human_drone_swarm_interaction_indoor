using System;
using System.Collections.Generic;
using UnityEngine;
using ViconDataStreamSDK.CSharp;

namespace ClassicConsoleApp1
{
    public class ViconTracker
    {

        public List<ViconTrackingObject> TrackingObjects { get; private set; }
        private ViconDataStreamSDK.CSharp.Client ViconClient;

        string ViconHostName = "192.168.2.221:801";

        public ViconTracker()
        {
            TrackingObjects = new List<ViconTrackingObject>();

        }

        public bool IsConnected()
        {
            if (ViconClient != null)
            {
                return ViconClient.IsConnected().Connected;
            }
            else
            {
                return false;
            }
        }

        public void Connect()
        {
            if(ViconClient == null)
            {
                try
                {
                    ViconClient = new ViconDataStreamSDK.CSharp.Client();
                }
                catch (DllNotFoundException)
                {
                    Debug.Log("Could not connect to Vicon");
                    return;
                }

            
            }



            if (ViconClient.IsConnected().Connected)
            {
                ViconClient.Disconnect();
            }
            
            Console.Write("Vicon: Connecting to {0} ...", ViconHostName);
            while (!ViconClient.IsConnected().Connected)
            {
                // Direct connection
                ViconClient.Connect(ViconHostName);

                // Multicast connection
                // MyClient.ConnectToMulticast( HostName, "224.0.0.0" );

                System.Threading.Thread.Sleep(200);
                //Console.Write(".");
            }
            Console.WriteLine(" done");

            ViconClient.EnableSegmentData();
            ViconClient.SetStreamMode(ViconDataStreamSDK.CSharp.StreamMode.ClientPullPreFetch);

            ViconClient.SetAxisMapping(ViconDataStreamSDK.CSharp.Direction.Forward,
                
                               ViconDataStreamSDK.CSharp.Direction.Left,
                               ViconDataStreamSDK.CSharp.Direction.Up
                               ); // Z-up
        }

        public void Disconnect()
        {
            ViconClient?.Disconnect();
        }

        //private Task<ViconDataStreamSDK.DotNET.Result> viconGetFrameTask = null;

        public void Update()
        {
            if (!ViconClient.IsConnected().Connected)
            {
                Console.Write("Vicon: Reconnecting to {0} ...", ViconHostName);
                if (ViconClient.Connect(ViconHostName).Result == Result.ClientConnectionFailed)
                {
                    return;
                };
            }

            //if (viconGetFrameTask == null)
            //{
            //    viconGetFrameTask = Task<ViconDataStreamSDK.DotNET.Result>.Factory.StartNew(() => ViconClient.GetFrame().Result);
            //}

            //if (!viconGetFrameTask.IsCompleted)
            //{
            //    return;
            //}



            //if (!(viconGetFrameTask.Result == Result.Success))
            if (!(ViconClient.GetFrame().Result == Result.Success))
            {
                return;
            }

            //viconGetFrameTask = null;


            //ViconClient.

            //if (ViconClient.GetFrame().Result != ViconDataStreamSDK.DotNET.Result.Success)
            //{
            //    Console.WriteLine("No Success getting Frame");
            //    return;
            //}

            //Console.WriteLine(ViconClient.GetLatencyTotal().Total);

            Output_GetSegmentGlobalTranslation vicon_pos;
            Output_GetSegmentGlobalRotationEulerXYZ vicon_rot;
            Output_GetSegmentGlobalRotationQuaternion vicon_rot_quat;
            foreach (ViconTrackingObject obj in TrackingObjects)
            {
                if (obj.SegmentName == null)
                {
                    vicon_pos = ViconClient.GetSegmentGlobalTranslation(obj.SubjectName, ViconClient.GetSegmentName(obj.SubjectName, 0).SegmentName);
                    vicon_rot = ViconClient.GetSegmentGlobalRotationEulerXYZ(obj.SubjectName, ViconClient.GetSegmentName(obj.SubjectName, 0).SegmentName);
                    vicon_rot_quat = ViconClient.GetSegmentGlobalRotationQuaternion(obj.SubjectName, ViconClient.GetSegmentName(obj.SubjectName, 0).SegmentName);
                } else
                {
                    vicon_pos = ViconClient.GetSegmentGlobalTranslation(obj.SubjectName, obj.SegmentName);
                    vicon_rot = ViconClient.GetSegmentGlobalRotationEulerXYZ(obj.SubjectName, obj.SegmentName);
                    vicon_rot_quat = ViconClient.GetSegmentGlobalRotationQuaternion(obj.SubjectName, obj.SegmentName);
                }
                if (vicon_pos.Translation[0] != 0f || vicon_pos.Translation[1] != 0f)
                {
                    obj.TrackerX = (float)vicon_pos.Translation[0];
                    obj.TrackerY = (float)vicon_pos.Translation[1];
                    obj.TrackerZ = (float)vicon_pos.Translation[2];

                    obj.RotationX = (float)vicon_rot.Rotation[0];
                    obj.RotationY = (float)vicon_rot.Rotation[1];
                    obj.RotationZ = (float)vicon_rot.Rotation[2];
                    //obj.RotationW = (float)vicon_rot.Rotation[3];

                    obj.Rotation = new Quaternion((float)vicon_rot_quat.Rotation[0], -(float)vicon_rot_quat.Rotation[2], (float)vicon_rot_quat.Rotation[1], (float)vicon_rot_quat.Rotation[3]);

                    obj.Occluded = false;

                } else
                {
                    obj.Occluded = true;
                }
            }
        }

    }
}
