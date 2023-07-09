using BeyondApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class LaserRenderer : MonoBehaviour
{

	public enum State
	{
		Idle,
		Connecting,
		Connected,
		FatalError
	}

	private string ZoneImageName = "BeyondConnector";
	public int Scanrate = 60;
	private readonly byte[] _zones = new byte[256];
	private static bool IsBeyondStartedAndReady
	{
		get
		{
			try
			{
				return Beyond.ldbBeyondExeStarted() == 1 && Beyond.ldbBeyondExeReady() == 1;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}

	public Dictionary<string, LaserBehaviour> LaserBehaviours = new Dictionary<string, LaserBehaviour>();

	NamedPipeClientStream clientStream;
	FramePoints frame = new FramePoints();
	TSdkImagePoint[] temp = new TSdkImagePoint[8192];

	public int LaserPointCount { get; private set; } = 0;

	public int points = 0;

	public State ConnectionState;

	public void AddLaserBehaviour(LaserBehaviour lb)
	{
		//Debug.Log("Added LaserBehaviour for " + lb.gameObject.name);
		LaserBehaviours.Add(lb.gameObject.name, lb);
	}

	public void RemoveLaserBehaviour(LaserBehaviour lb)
	{
		LaserBehaviours.Remove(lb.gameObject.name);
	}

	// Start is called before the first frame update
	void Start()
	{
		ConnectionState = State.Idle;
		frame.Points = new TSdkImagePoint[8192];
		frame.Count = 0;
		_zones[0] = 1;
		_zones[1] = 0;
	}

	// Update is called once per frame
	void Update()
	{
		if (ConnectionState == State.Connecting && IsBeyondStartedAndReady)
		{
			ConnectionState = State.Connected;
			CreateBeyondZoneImage();
		}
		if (ConnectionState == State.Connected && !IsBeyondStartedAndReady)
		{
			Debug.Log("Lost Connection to Beyond", this);
			ConnectionState = State.Idle;
		}
	}



	// LateUpdate is called once per frame, after Update has finished. Any calculations that are performed in Update will have completed when LateUpdate begins.
	void LateUpdate()
	{
		if (ConnectionState == State.Connected && IsBeyondStartedAndReady)
		{
			Array.Clear(frame.Points, 0, frame.Points.Length);

			int pointCount = 0;
			int index = 0;

			foreach (LaserBehaviour obj in LaserBehaviours.Values)
			{
				if (obj.visible)
				{
					foreach (LaserGraphicsShape shape in obj.Shapes)
					{
						if (shape.Active)
						{
							shape.Update();

							if (shape.LaserPath.Length > 0)
							{

								pointCount += shape.LaserPath.Length + 1;

								frame.Points[index++] = TSdkImagePoint.CreatePoint(shape.LaserPath[0].X, shape.LaserPath[0].Y, Color.black, 1);
								foreach (PointWithColor point in shape.LaserPath)
								{
									frame.Points[index++] = TSdkImagePoint.CreatePoint(point.X, point.Y, point.Color, 1);
								}
							}
						}
					}
				}
			}

			//Console.WriteLine("pointCount: {0}", pointCount);

			frame.Count = pointCount;
			LaserPointCount = pointCount;
			points = pointCount;

			Beyond.ldbSendFrameToImage(ZoneImageName, frame.Count, frame.Points, _zones, Scanrate);

			/*
            byte[] buffer = getBytes(frame);
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            clientStream.WaitForPipeDrain();
			*/
		}
	}

	public void Connect()
	{
		/*Debug.Log("Connecting to beyond pipe... ");
        clientStream = new NamedPipeClientStream("beyondconnector");
        clientStream.Connect();

        byte[] buffer = ASCIIEncoding.ASCII.GetBytes("Connected");
        Debug.Log("done");
        Debug.Log("Can Write: " + clientStream.CanWrite);
        clientStream.Write(buffer, 0, buffer.Length);
        clientStream.Flush();*/
		if (ConnectionState == State.Idle || ConnectionState == State.FatalError)
		{
            try
            {
				InitializeBeyond();
			}
			catch (DllNotFoundException e)
			{
				ConnectionState = State.FatalError;
				Debug.Log(e.Message, this);
			}
			catch (Exception)
            {
				// Just try to connect.
			}
			ConnectionState = State.Connecting;
		}
	}

	public void Disconnect()
	{
		if (ConnectionState == State.Connected)
		{
			CleanupBeyond();
			ConnectionState = State.Idle;
		}
	}

	private void InitializeBeyond()
	{
		try
		{
			Debug.Log($"Initializing Beyond Library, Result: {Beyond.ldbCreate()}", this);
			Debug.Log($"DLL Version: {Beyond.ldbGetDllVersion()}", this);
			Debug.Log($"Beyond is {(Beyond.ldbBeyondExeStarted() == 1 ? "" : "not ")}started", this);
			Debug.Log($"Beyond is {(Beyond.ldbBeyondExeReady() == 1 ? "" : "not ")}ready", this);
		}
		catch (Exception e)
		{
			Debug.Log($"couldn't initialize beyond: {e.GetType().Name}", this);
			throw;
		}
	}

	private void CreateBeyondZoneImage()
	{
		try
		{
			Debug.Log($"Projector Count: {Beyond.ldbGetProjectorCount()}", this);
			Debug.Log($"Zone Count: {Beyond.ldbGetZoneCount()}", this);
			Debug.Log($"Create Zone Image \"{ZoneImageName}\", Result: {Beyond.ldbCreateZoneImage(0, ZoneImageName)}", this);
		}
		catch (Exception e)
		{
			Debug.Log($"couldn't create zone image: {e.GetType().Name}", this);
		}
	}

	private void CleanupBeyond()
	{
		try
		{
			Beyond.ldbSendFrameToImage(ZoneImageName, 0, null, _zones, Scanrate);
			Debug.Log($"Closing Beyond Library, Result: {Beyond.ldbDestroy()}", this);
		}
		catch (Exception e)
		{
			Debug.Log($"Couldn't clean up Beyond: {e.GetType().ToString()}", this);
		}
	}


	void OnDisable()
	{
		/*if (clientStream != null && clientStream.IsConnected)
        {
            LateUpdate();
            clientStream.Dispose();
        }*/

		Disconnect();


	}

	byte[] getBytes(FramePoints points)
	{
		int size = Marshal.SizeOf(points);
		byte[] arr = new byte[size];

		IntPtr ptr = Marshal.AllocHGlobal(size);
		Marshal.StructureToPtr(points, ptr, true);
		Marshal.Copy(ptr, arr, 0, size);
		Marshal.FreeHGlobal(ptr);
		return arr;
	}
}
