using System.Collections.Generic;
using UnityEngine;

public interface IDroneConnection
{
    GameObject GetFloor();
    void CreateTrajectory(int id, double vmax, double amax, string groupMask, List<Vector3> waypoints);
    void Land(int id, double starttime, double duration);
    void LandWithHeight(int id, double starttime, double duration, double height);
    void MoveHome(int id, double starttime, double duration);
    void MoveRelative(int id, double starttime, double duration, double x, double y, double z, double yaw);
    void MoveTo(int id, double starttime, double duration, double x, double y, double z, double yaw);
    void Start(int id, double starttime, double duration, double height);
    void StartTrajectory(int id, double starttime, double timescale, string groupMask);
}