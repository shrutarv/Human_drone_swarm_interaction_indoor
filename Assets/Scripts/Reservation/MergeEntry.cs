using System;
using System.Collections.Generic;
using System.Linq;

namespace Reservation
{
    public class MergeEntry
    {
        public int FirstClock;
        public IResourceUser User;
        public SortedDictionary<int, ReservationEntry> PathIndexedReservations { get; }
        public int LastClock = 0;

        public MergeEntry(int firstClock, IResourceUser user, int pathIndex, int count)
        {
            FirstClock = firstClock;
            User = user ?? throw new ArgumentNullException(nameof(user));
            PathIndexedReservations = new SortedDictionary<int, ReservationEntry>();
        }

        public int LowestPathEntryIndex()
        {
            return PathIndexedReservations.First().Key;
        }

        public int HighestPathEntryIndex()
        {
            return PathIndexedReservations.Last().Key;
        }

    }
}
