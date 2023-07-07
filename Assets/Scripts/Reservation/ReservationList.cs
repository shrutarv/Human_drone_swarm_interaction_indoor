using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Reservation
{
    public class ReservationList
    {

        public ReservationManager _manager;

        public int clock = 0;
        public int nextSlot = 1;
        public SortedDictionary<int, ReservationEntry> reservationList = new SortedDictionary<int, ReservationEntry>();
        public List<string> debugReservationList = new List<string>();

        public HashSet<IResourceUser> _currentlyPresentResourceUsers = new HashSet<IResourceUser>();
        public string CurrentlyPresentResourceUsers { get => GetCurrentlyPresentResourceUsers(); }

        private string GetCurrentlyPresentResourceUsers()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var obj in _currentlyPresentResourceUsers)
            {
                if (obj != null)
                {
                    if (((Component)obj)?.gameObject != null)
                    {
                        sb.Append($"{((Component)obj).gameObject.name}, ");
                    }
                }
            }
            return sb.ToString();
        }


        public ReservationList(ReservationManager manager)
        {
            _manager = manager;
        }

        public int ReserveSlot(ReservationManager resource, int pathStepIndex, int timeStamp, IResourceUser ru)
        {
            if (timeStamp >= nextSlot)
            {
                nextSlot = timeStamp + 1;
            }

            _manager.history.Add(clock, $"ReserveSlot for {ru.Name} with pathIndex {pathStepIndex}");

            if (reservationList.Count == 0)
            {
                clock = nextSlot;
            }

            AddListEntry(resource, pathStepIndex, nextSlot, ru);
            return nextSlot++;
        }


        private void AddListEntry(ReservationManager resource, int pathStepIndex, int i, IResourceUser ru)
        {
            var entry = new ReservationEntry(i, ru, resource, pathStepIndex);
            ru.ReservationPath.AddEntry(entry);
            foreach (var prev in reservationList.Reverse())
            {
                if (prev.Value.User == entry.User)
                {
                    break;
                }
                ru.ReservationPath.AddDependency(entry, prev.Value);
            }
            reservationList.Add(i, entry);
            debugReservationList.Add(i + ": " + ru.Name);
        }

        public bool EntryPermitted(IResourceUser ru)
        {

            bool noUsersBeforeMeAreInsideTheCluster = true;
            bool hasReservationEntry = false;
            List<int> timeslots = new List<int>();
            foreach (KeyValuePair<int, ReservationEntry> kvp in reservationList)
            {

                if (kvp.Value.User == ru)
                {
                    hasReservationEntry = true;
                    timeslots.Add(kvp.Key);
                    if (kvp.Key == clock)
                    {
                        if (_currentlyPresentResourceUsers.Count > 0 && !_currentlyPresentResourceUsers.Contains(ru))
                        {
                            //Debug.Log(ru.Name + " is not allowed to enter as there are already users inside Resource " + this.gameObject.name + ". Users inside resource: " + CurrentlyPresentResourceUsers , gameObject);
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (_currentlyPresentResourceUsers.Contains(ru) && noUsersBeforeMeAreInsideTheCluster)
                        {
                            //if the user is already inside the cluster permit movement. This case can appear through merging and should work without deadlocks (hopefully) Moritz 10.03.2019
                            return true;
                        }
                    }
                }
                if (_currentlyPresentResourceUsers.Contains(kvp.Value.User))
                {
                    noUsersBeforeMeAreInsideTheCluster = false;
                }
            }

            if (hasReservationEntry)
            {
                if (reservationList.ContainsKey(clock))
                {
                    //Debug.Log(ru.Name + " tries to enter " + this.gameObject.name + " when clock is at " + clock + " and permitted next agent is " + reservationList[clock].User.Name + ", my timeslot starts at " + timeslots.Min(), gameObject);
                }
                else
                {
                    //Debug.Log(ru.Name + " tries to enter " + this.gameObject.name + " when clock is at " + clock + " and there is NO NEXT AGENT!! , my timeslot starts at " + timeslots.Min(), gameObject);
                }
            }
            else
            {
                Debug.Log(ru.Name + " tries to enter " + _manager.gameObject.name + ", but has no reservation entry.", _manager.gameObject);
            }
            return false;
        }

        public void Entering(IResourceUser ru, int pathIndex)
        {


            if (_currentlyPresentResourceUsers.Count > 0 && !_currentlyPresentResourceUsers.Contains(ru))
            {
                _manager.history.Add(clock, $"{ru.Name} is entering {_manager.name} with path index {pathIndex} while there are already other users inside: {CurrentlyPresentResourceUsers}");
            }
            if (reservationList.Count == 0)
            {
                _manager.history.Add(clock, $"{ru.Name} is entering {_manager.name} with path index {pathIndex} while reservation list is empty. This is NOT GOOD!!");
                throw new Exception($"{ru.Name} is entering {_manager.name} with path index {pathIndex} while reservation list is empty. This is NOT GOOD!!");
            }
            _manager.history.Add(clock, $"{ru.Name} is entering me with path index {pathIndex}");
            if (!_currentlyPresentResourceUsers.Contains(ru))
            {
                ru.RegisterReservationManager(_manager);
                _currentlyPresentResourceUsers.Add(ru);
            }


        }

        public void DeleteSlot(IResourceUser ru) //as soon as a new master is assigned anywhere on a already reserved path all slots that the agent had reserved before need to be deleted (and a new reservation started)
        {


            _manager.history.Add(clock, $"Delete all slots for {ru.Name}");

            List<int> deleteKey = new List<int>();

            foreach (KeyValuePair<int, ReservationEntry> kvp in reservationList)
            {
                if (kvp.Value.User == ru)
                {
                    deleteKey.Add(kvp.Key);
                    _manager.history.Add(clock, $"     with path index {kvp.Value.PathIndex}");
                }
            }

            if (deleteKey.Count > 0)
            {
                foreach (int k in deleteKey)
                {
                    DeleteListEntry(k, ru);
                }
            }
        }


        private void DeleteListEntry(int deletedListIndex, IResourceUser ru)
        {
            var deletedEntry = reservationList[deletedListIndex];
            reservationList.Remove(deletedListIndex);

            foreach (var after in reservationList)
            {
                if (after.Value.User != ru)
                {
                    try
                    {
                        after.Value.User.ReservationPath.RemoveDependency(after.Value, deletedEntry);
                    }
                    catch (Exception)
                    {
                        Debug.Log(_manager.history.Text(), _manager.gameObject);
                        Debug.Break();
                    }
                }
            }

            ru.ReservationPath.RemoveEntry(deletedEntry);

            RecreateDebugReservationList();

            if (deletedListIndex == clock)
            {
                IncrementClock();
            }
        }

        private void RecreateDebugReservationList()
        {
            debugReservationList.Clear();
            foreach (KeyValuePair<int, ReservationEntry> kvp in reservationList)
            {
                if (kvp.Value == null)
                {
                    //Debug.Log(gameObject.name + " reservationList has null entry in slot " + kvp.Key);
                }
                else
                {
                    //Debug.Log(gameObject.name + " reservationList entry key " + kvp.Key);
                    //Debug.Log(gameObject.name + " reservationList entry value " + kvp.Value.Name);
                    debugReservationList.Add($"{kvp.Key}: {kvp.Value.User.Name} at {kvp.Value.Resource.name}");
                }
            }
        }


        public void NotifyExit(IResourceUser ru, int pathIndex)
        {

            _manager.history.Add(clock, $"NotifyExit for {ru.Name} with path index {pathIndex}");
            var entry = FindReservationEntry(ru, pathIndex);
            if (entry.Equals(default)) //if there is no reservation entry
            {
                var message = $"{ru.Name} is exiting with path index {pathIndex} but there is no corresponding reservation entry. This is NOT GOOD!!!";
                _manager.history.Add(clock, message);
                _manager.history.Add(clock, $"     Currently present users: {CurrentlyPresentResourceUsers}");
                Debug.Log(message, _manager.gameObject);
                throw new Exception(message);
            }

            if (_currentlyPresentResourceUsers.Contains(ru))
            {
                var deletedEntry = reservationList[entry.Key];
                reservationList.Remove(entry.Key);

                foreach (var after in reservationList)
                {
                    if (after.Value.User != ru)
                    {
                        after.Value.User.ReservationPath.RemoveDependency(after.Value, deletedEntry);
                    }
                }

                ru.ReservationPath.RemoveEntry(deletedEntry);


                RecreateDebugReservationList();
                if (entry.Key == clock)
                {
                    IncrementClock();
                }
                else
                {
                    _manager.history.Add(clock, $"{ru.Name} is exiting with time value {entry.Key} but the clock is at {clock}, there is another currently present user, so the clock is not incremented");
                    _manager.history.Add(clock, $"     Currently present users: {CurrentlyPresentResourceUsers}");
                }
                var nextEntry = FindReservationEntry(ru, pathIndex + 1);
                if (nextEntry.Equals(default(KeyValuePair<int, ReservationEntry>)))
                {
                    _manager.history.Add(clock, $"{ru.Name} is leaving the cluster and removed from currently present users");
                    _currentlyPresentResourceUsers.Remove(ru);
                }
                else
                {
                    _manager.history.Add(clock, $"{ru.Name} is not leaving the cluster since there is another reservation entry.");
                    _manager.history.Add(clock, $"     Currently present users: {CurrentlyPresentResourceUsers}");
                    _manager.history.Add(clock, ReservationListAsText("     "));
                }
                if (reservationList.ContainsKey(clock))
                {
                    var nextReservation = reservationList[clock];
                    _manager.history.Add(clock, $"InformNextSectionFree for {nextReservation.User.Name} with pathIndex {nextReservation.PathIndex}");
                    nextReservation.User.InformNextSectionFree(_manager, nextReservation);
                }
                else
                {
                    _manager.history.Add(clock, $"Incremented clock after departure of last user in reservation list, waiting for new reservations.");
                }
            }
            else
            {
                var message = $"{ru.Name} is exiting with path index {pathIndex} but is not registered as currently present in this resource. This is NOT GOOD!!!";
                _manager.history.Add(clock, message);
                _manager.history.Add(clock, $"     Currently present users: {CurrentlyPresentResourceUsers}");
                Debug.Log(message, _manager.gameObject);
                throw new Exception(message);
            }


        }

        public void IncrementTimestampAndPropagate(IResourceUser ru, int pathIndex, int timestamp, StringBuilder debug, string prefix)
        {
            var unchanged = new SortedDictionary<int, ReservationEntry>();
            var changed = new SortedDictionary<int, ReservationEntry>();
            var propagate = new SortedDictionary<IResourceUser, ReservationEntry>();
            bool found = false;
            int time = timestamp;
            debug.Append($"{prefix}IncrementTimestampAndPropagate for {ru.Name}, {pathIndex} with timeStamp {timestamp}\n");
            debug.Append($"{prefix}Before: Reservation list at {_manager.name} with (clock: {clock}, nextSlot: {nextSlot})\n");
            reservationList.ToList().ForEach(kvp => debug.Append(prefix + "     " + kvp.Key + ": " + kvp.Value.User.Name + ", " + kvp.Value.PathIndex + "\n"));
            debug.Append("\n");

            foreach (var entry in reservationList)
            {
                if (entry.Value.User == ru && entry.Value.PathIndex == pathIndex)
                {
                    found = true;
                }
                if (found)
                {
                    if (time > entry.Key)
                    {
                        var path = entry.Value.User.ReservationPath;
                        path.RemoveEntry(entry.Value);
                        entry.Value.Time = time;
                        changed.Add(time, entry.Value);
                        time++;
                    }
                    else
                    {
                        unchanged.Add(entry.Key, entry.Value);
                    }
                }
                else
                {
                    unchanged.Add(entry.Key, entry.Value);
                }
            }
            reservationList.Clear();
            foreach (var entry in unchanged)
            {
                reservationList.Add(entry.Key, entry.Value);
            }
            foreach (var entry in changed.Reverse())
            {
                if (entry.Value.User != null && !propagate.ContainsKey(entry.Value.User))
                {
                    propagate.Add(entry.Value.User, entry.Value);
                }
                reservationList.Add(entry.Key, entry.Value);
            }



            if (reservationList.Count > 0)
            {
                clock = reservationList.First().Key;
                nextSlot = reservationList.Last().Key + 1;
            }

            debug.Append($"{prefix}After: Reservation list at {_manager.name} with (clock: {clock}, nextSlot: {nextSlot})\n");
            reservationList.ToList().ForEach(kvp => debug.Append(prefix + "     " + kvp.Key + ": " + kvp.Value.User.Name + ", " + kvp.Value.PathIndex + "\n"));
            debug.Append("\n");

            List<ReservationEntry> previous = new List<ReservationEntry>();
            foreach (var entry in reservationList)
            {
                if (changed.ContainsKey(entry.Key))
                {
                    var path = entry.Value.User.ReservationPath;
                    path.AddEntry(entry.Value);
                    foreach (var dep in previous.Reverse<ReservationEntry>())
                    {
                        if (dep.User == entry.Value.User)
                        {
                            break;
                        }
                        path.AddDependency(entry.Value, dep);
                    }
                }
                previous.Add(entry.Value);
            }
            foreach (var kvp in propagate)
            {
                kvp.Key.IncrementTimestampAndPropagate(kvp.Key, kvp.Value.PathIndex + 1, kvp.Value.Time + 1, debug, prefix + "     ");
            }
        }

        public KeyValuePair<int, ReservationEntry> FindReservationEntry(IResourceUser ru, int pathIndex)
        {
            foreach (KeyValuePair<int, ReservationEntry> kvp in reservationList)
            {
                if (kvp.Value.User == ru && kvp.Value.PathIndex == pathIndex)
                {
                    return kvp;
                }
            }

            return default;
        }

        public List<ReservationEntry> FindPreviousEntries(IResourceUser ru)
        {
            List<ReservationEntry> returnList = new List<ReservationEntry>();

            foreach (KeyValuePair<int, ReservationEntry> kvp in reservationList)
            {

                if (kvp.Value.User == ru)
                {
                    return returnList;
                }
                else
                {
                    returnList.Add(kvp.Value);
                }
            }

            return new List<ReservationEntry>();
        }


        private void IncrementClock()
        {

            foreach (int key in reservationList.Keys)
            {
                if (key > clock)
                {
                    clock = key;
                    return;
                }
            }
            clock = nextSlot;
        }

        public string ReservationListAsText(string prefix)
        {
            StringBuilder debug = new StringBuilder();
            debug.Append(prefix + "Reservation list (clock: " + clock + ", nextSlot: " + nextSlot + ")\n");
            reservationList.ToList().ForEach(kvp => debug.Append(prefix + "     " + kvp.Key + ": " + kvp.Value.User.Name + ", " + kvp.Value.PathIndex + "\n"));
            return debug.ToString();
        }

        public void MergeReservationList(ReservationManager client, SortedDictionary<int, ReservationEntry> clientReservationList, int clientClock, int clientNextSlot)
        {
            //Debug.Log("MergeReservationList for client " + client.gameObject.name + " and master " + gameObject.name, gameObject);
            //if one of the reservation lists is empty then just use the other, non-empty list

            StringBuilder debug = new StringBuilder();
            debug.Append("MergeReservationList from " + client.name + " to master " + _manager.name + "\n");
            if (_currentlyPresentResourceUsers.Count > 0)
            {
                debug.Append("     Master has current user list: " + CurrentlyPresentResourceUsers + "\n");
            }
            if (client.reservationList._currentlyPresentResourceUsers.Count > 0)
            {
                debug.Append("     Client has current user list: " + client.reservationList.CurrentlyPresentResourceUsers + "\n");
            }


            if (clientReservationList.Count == 0)
            {
                debug.Append("     Client reservation list is empty.\n");
                _currentlyPresentResourceUsers.UnionWith(client.reservationList._currentlyPresentResourceUsers);
                foreach (IResourceUser ru in _currentlyPresentResourceUsers)
                {
                    ru.RegisterReservationManager(_manager);
                }
                client.reservationList._currentlyPresentResourceUsers = new HashSet<IResourceUser>();
                _manager.history.Add(clock, debug.ToString());
                return;
            }
            if (reservationList.Count == 0)
            {
                debug.Append("     Master reservation list is empty. Using Client list.\n");
                _currentlyPresentResourceUsers.UnionWith(client.reservationList._currentlyPresentResourceUsers);
                foreach (IResourceUser ru in _currentlyPresentResourceUsers)
                {
                    ru.RegisterReservationManager(_manager);
                }

                client.reservationList._currentlyPresentResourceUsers = new HashSet<IResourceUser>();
                //Debug.Log("Master reservation list is empty, using the client reservation list", gameObject);
                reservationList = new SortedDictionary<int, ReservationEntry>(clientReservationList);
                clock = clientClock;
                nextSlot = clientNextSlot;
                RecreateDebugReservationList();
                _manager.history.Add(clock, debug.ToString());
                return;
            }

            //var masterCurrentUser = currentlyPresentResourceUser;
            //var clientCurrentUser = client.currentlyPresentResourceUser;




            //For each user, record the highest clock value that can be found in both lists.
            //This will be used after merging to determine the users that have to update the following resources in their reservation chain
            //(if the highest clock value is not exceeded after merging then there is no need for an update)
            Dictionary<IResourceUser, int> existingHighestClockValueMap = new Dictionary<IResourceUser, int>();
            foreach (KeyValuePair<int, ReservationEntry> kvp in reservationList)
            {
                if (existingHighestClockValueMap.ContainsKey(kvp.Value.User))
                {
                    var oldValue = existingHighestClockValueMap[kvp.Value.User];
                    if (kvp.Value.Time > oldValue)
                    {
                        existingHighestClockValueMap[kvp.Value.User] = kvp.Value.Time;
                    }
                }
                else
                {
                    existingHighestClockValueMap[kvp.Value.User] = kvp.Value.Time;
                }
            }

            foreach (KeyValuePair<int, ReservationEntry> kvp in clientReservationList)
            {
                if (existingHighestClockValueMap.ContainsKey(kvp.Value.User))
                {
                    var oldValue = existingHighestClockValueMap[kvp.Value.User];
                    if (kvp.Value.Time > oldValue)
                    {
                        existingHighestClockValueMap[kvp.Value.User] = kvp.Value.Time;
                    }
                }
                else
                {
                    existingHighestClockValueMap[kvp.Value.User] = kvp.Value.Time;
                }
            }


            //Remove the values from the ReservationPath maps, so that they can be updated after merging
            foreach (KeyValuePair<int, ReservationEntry> kvp in reservationList)
            {
                kvp.Value.User.ReservationPath.RemoveEntry(kvp.Value);
            }

            foreach (KeyValuePair<int, ReservationEntry> kvp in clientReservationList)
            {
                kvp.Value.User.ReservationPath.RemoveEntry(kvp.Value);
            }



            debug.Append("     Highest clock value map:\n ");
            existingHighestClockValueMap.ToList().ForEach(kvp => debug.Append($"          {kvp.Key.Name}: {kvp.Value}\n"));
            debug.Append("\n");

            debug.Append("     Client reservation list (clock: " + clientClock + ", nextSlot: " + clientNextSlot + ")\n");
            clientReservationList.ToList().ForEach(kvp => debug.Append("         " + kvp.Key + ": " + kvp.Value.User.Name + ", " + kvp.Value.PathIndex + "\n"));

            debug.Append("\n");

            debug.Append("     Master reservation list (clock: " + clock + ", nextSlot: " + nextSlot + ")\n");
            reservationList.ToList().ForEach(kvp => debug.Append("         " + kvp.Key + ": " + kvp.Value.User.Name + ", " + kvp.Value.PathIndex + "\n"));



            //Create an intermediate datastructure called mergeList that represents an indexed ordering of unique users
            //First, the master list is processed so that any existing order of users is taken from here
            //Then, the client list is merged and any conflicting ordering of users in both lists is resolved by using the master order
            //Additionally, the merge function tries to preserve any clock values that are non-conflicting to keep updates in the reservation chain low
            SortedDictionary<int, MergeEntry> mergeList = new SortedDictionary<int, MergeEntry>();
            Dictionary<IResourceUser, MergeEntry> userDict = new Dictionary<IResourceUser, MergeEntry>();




            SortedDictionary<int, ReservationEntry> currentUserList = new SortedDictionary<int, ReservationEntry>();

            int currentUserListClock = clock;

            int masterCurrentUserClock = clock;
            if (_currentlyPresentResourceUsers.Count > 0)
            {

                while (reservationList.ContainsKey(masterCurrentUserClock))
                {
                    var entry = reservationList[masterCurrentUserClock];
                    if (_currentlyPresentResourceUsers.Contains(entry.User))
                    {
                        currentUserList.Add(currentUserListClock++, entry);
                        reservationList.Remove(masterCurrentUserClock);
                    }
                    else
                    {
                        break;
                    }
                    masterCurrentUserClock++;
                }
            }


            int clientCurrentUserClock = clientClock;

            if (client.reservationList._currentlyPresentResourceUsers.Count > 0)
            {
                currentUserListClock = Math.Max(currentUserListClock, clientClock);
                while (clientReservationList.ContainsKey(clientCurrentUserClock))
                {
                    var entry = clientReservationList[clientCurrentUserClock];
                    if (client.reservationList._currentlyPresentResourceUsers.Contains(entry.User))
                    {
                        currentUserList.Add(currentUserListClock++, entry);
                        clientReservationList.Remove(clientCurrentUserClock);
                    }
                    else
                    {
                        break;
                    }
                    clientCurrentUserClock++;
                }
            }

            int baseMergeClockValue = clock;

            foreach (KeyValuePair<int, ReservationEntry> kvp in currentUserList)
            {
                MergeReservationEntry(kvp, mergeList, userDict, baseMergeClockValue, debug);
            }


            baseMergeClockValue = currentUserListClock;


            foreach (KeyValuePair<int, ReservationEntry> kvp in reservationList)
            {
                MergeReservationEntry(kvp, mergeList, userDict, baseMergeClockValue, debug);
            }

            foreach (KeyValuePair<int, ReservationEntry> kvp in clientReservationList)
            {
                MergeReservationEntry(kvp, mergeList, userDict, baseMergeClockValue, debug);
            }

            if (client.reservationList._currentlyPresentResourceUsers.Count > 0)
            {
                _currentlyPresentResourceUsers.UnionWith(client.reservationList._currentlyPresentResourceUsers);
                foreach (IResourceUser ru in _currentlyPresentResourceUsers)
                {
                    ru.RegisterReservationManager(_manager);
                }
            }


            //From the mergeList, the new reservation list is constructed by expanding the unique user entries into reservation entries that represent a single resource
            SortedDictionary<int, ReservationEntry> merged = new SortedDictionary<int, ReservationEntry>();

            //since the master is chosen by having the lowest clock, set the merge clock value to the existing clock of the master 
            //this guarantees that that this is the lowest clock value in both origin lists
            int mergeClock = clock;
            foreach (KeyValuePair<int, MergeEntry> kvp in mergeList)
            {

                mergeClock = Math.Max(mergeClock, kvp.Value.FirstClock); //use the max of next mergeClock value and first clock value of merge entry to preserve the logic time order of the reservation chain

                //expand the merge entry into several reservation entries depending on the count
                foreach (var entry in kvp.Value.PathIndexedReservations)
                {
                    entry.Value.Time = mergeClock;
                    kvp.Value.LastClock = mergeClock; //write back the mergeClock to merge entry, so that the highest clock value after expansion is saved for the update user check
                    merged.Add(mergeClock++, entry.Value);
                }
            }

            debug.Append("\n");
            debug.Append("     Merged template list:\n");
            mergeList.ToList().ForEach(kvp => debug.Append("         " + kvp.Key + ": " + kvp.Value.User.Name + ", count: " + kvp.Value.PathIndexedReservations.Count + ", first: " + kvp.Value.FirstClock + ", last: " + kvp.Value.LastClock + ", path: " + kvp.Value.LowestPathEntryIndex() + "\n"));



            nextSlot = mergeClock; //the next slot is the incremented mergeClock value after the last expansion
            reservationList = merged; //set the merged list as the new reservation list
            clock = reservationList.First().Key;


            debug.Append("\n");

            debug.Append("     Merged reservation list (clock: " + clock + ", nextSlot: " + nextSlot + ")\n");
            merged.ToList().ForEach(kvp => debug.Append("         " + kvp.Key + ": " + kvp.Value.User.Name + ", " + kvp.Value.PathIndex + "\n"));


            //create an ordered list of users that have to be updated
            //a user that has a higher clock value than before needs to update the subsequent resources in the reservation chain
            //for each user, the previously recorded highest clock value is compared to the highest clock value after merging
            //if the new highest value was incremented by a collision in the merging process then the user is added to the update list
            SortedDictionary<int, MergeEntry> usersThatHaveToBeUpdated = new SortedDictionary<int, MergeEntry>();

            foreach (KeyValuePair<int, MergeEntry> kvp in mergeList)
            {
                var oldHighestClockValue = existingHighestClockValueMap[kvp.Value.User];
                if (oldHighestClockValue < kvp.Value.LastClock)
                {
                    usersThatHaveToBeUpdated.Add(kvp.Value.LastClock, kvp.Value);
                }
            }

            debug.Append("\n");
            debug.Append("     Users that have to be updated: \n");
            if (usersThatHaveToBeUpdated.Count == 0)
            {
                debug.Append("          none");
            }

            //invalidate all subsequent reservations for user that have to be updated
            //this will trigger a new reservation process for the subsequent reservation chain
            //the invalidation will be in the order of users in the new merge list, so that this order is preserved in any subsequent reservations
            StringBuilder debugInvalidate = new StringBuilder();
            foreach (KeyValuePair<int, MergeEntry> kvp in usersThatHaveToBeUpdated)
            {
                debug.Append("         " + kvp.Key + ": " + kvp.Value.User.Name + ", nextPathIndex: " + (kvp.Value.HighestPathEntryIndex() + 1) + ", nextTimeStamp: " + (kvp.Value.LastClock + 1) + "\n");
                kvp.Value.User.InvalidateReservation(kvp.Value.HighestPathEntryIndex() + 1, kvp.Value.LastClock + 1, $"InvalidateReservation for {kvp.Value.User.Name} because of merge from {client.name} to {_manager.name}", debugInvalidate, "");
            }





            //Update the ReservationPath Maps for all entries
            foreach (var kvp in merged)
            {
                var entry = kvp.Value;
                try
                {
                    entry.User.ReservationPath.AddEntry(entry);
                }
                catch (Exception e)
                {
                    Debug.Log(debug.ToString(), _manager.gameObject);
                    throw e;
                }
                foreach (var prev in reservationList.Where(obj => obj.Key < kvp.Key).Reverse())
                {
                    if (prev.Value.User == entry.User)
                    {
                        break;
                    }
                    entry.User.ReservationPath.AddDependency(entry, prev.Value);
                }
            }

            _manager.history.Add(clock, debug.ToString());

            RecreateDebugReservationList();

            if (usersThatHaveToBeUpdated.Count > 0) 
            {
                Debug.Log("WARNING: usersThatHaveToBeUpdated.Count > 0");
                Debug.Log(_manager.history.Text(), _manager.gameObject);
                Debug.Log(debugInvalidate, _manager.gameObject);
                Debug.Break();
            }

        }

        /**
         * This method adds a reservation entry into the mergeList which is used as an intermediate datastructure to construct the newly merged reservation list.
         * The mergeList represents an indexed ordering of the unique user entries of both lists. These entries can later be expanded into a regular resevation list.
         */
        private void MergeReservationEntry(KeyValuePair<int, ReservationEntry> kvp, SortedDictionary<int, MergeEntry> mergeList, Dictionary<IResourceUser, MergeEntry> userDict, int baseClockValue, StringBuilder debug)
        {
            //if the user is already known then update the MergeEntry if necessary and increment the counter
            //the merge entry should always contain the lowest path index and the lowest clock value of all merged entries of a user
            //these lowest values will be used as the starting values when expanding into reservation entries again
            if (userDict.ContainsKey(kvp.Value.User))
            {
                if (!userDict[kvp.Value.User].PathIndexedReservations.ContainsKey(kvp.Value.PathIndex))
                {
                    userDict[kvp.Value.User].PathIndexedReservations.Add(kvp.Value.PathIndex, kvp.Value);
                }
                else
                {
                    Debug.Log($"{_manager.name}->{kvp.Value.User.Name}: Trying to merge the same path index {kvp.Value.PathIndex} again");
                    Debug.Log(debug.ToString(), _manager.gameObject);
                    Debug.Log(_manager.history.Text(), _manager.gameObject);
                    Debug.Break();
                }
                var oldFirstClock = userDict[kvp.Value.User].FirstClock;
                var newClock = kvp.Key;
                if (oldFirstClock > newClock)
                {
                    userDict[kvp.Value.User].FirstClock = newClock;
                }

            }
            //if the user is not yet known create a new MergeEntry
            else
            {
                //try to use the original clock value as an index or increment it until a free index has been found
                //make sure that every index is higher or equal to the given base clock value
                int index = Math.Max(kvp.Key, baseClockValue);
                while (mergeList.ContainsKey(index))
                {
                    index++;
                }

                var entry = new MergeEntry(kvp.Key, kvp.Value.User, kvp.Value.PathIndex, 1);
                entry.PathIndexedReservations.Add(kvp.Value.PathIndex, kvp.Value);

                mergeList.Add(index, entry);
                userDict.Add(kvp.Value.User, entry);

            }
        }
    }
}
