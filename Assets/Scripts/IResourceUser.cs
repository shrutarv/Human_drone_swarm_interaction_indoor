using System;
using System.Collections.Generic;
using System.Text;
using Dependency;
using Reservation;
using UnityEngine;

public interface IResourceUser
{
    string Name { get; }
    Vector3 Source { get; }
    Vector3 Destination { get; }
    string CurrentState { get; }
    DependencyNode<IResourceUser> DependencyNode { get; }
    ReservationPath ReservationPath { get; }
    void InvalidateReservation(int pathStepIndex, int timeStamp, string reason, StringBuilder debug, string prefix);
    void IncrementTimestampAndPropagate(IResourceUser ru, int pathIndex, int timestamp, StringBuilder debug, string prefix);
    void InformNextSectionFree(ReservationManager origin, ReservationEntry nextEntry);
    void RegisterReservationManager(ReservationManager rm);
}

