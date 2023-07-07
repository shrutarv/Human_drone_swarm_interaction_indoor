using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface DroneActionListener
{
    void NearlyFinished(Guid actionId);
    void FinishedAction(Guid actionId);
    void TimeLeft(Guid actionId, float amount);
}
