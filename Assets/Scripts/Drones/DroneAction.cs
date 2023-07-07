using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface DroneAction
{
    Guid Id();
    void Execute();
    void Trigger(Collider col);
    Vector3? TargetPoint();
    GameObject TargetGameObject();
}
