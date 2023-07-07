using System;
namespace Reservation
{
    public class ReservationEntry
    {
        public int Time;
        public IResourceUser User;
        public ReservationManager Resource;
        public int PathIndex;
        public bool CurrentlyPresent = false;

        public ReservationEntry(int time, IResourceUser user, ReservationManager resource,  int pathIndex)
        {
            Time = time;
            User = user ?? throw new ArgumentNullException(nameof(user));
            Resource = resource;
            PathIndex = pathIndex;
        }
    }
}
