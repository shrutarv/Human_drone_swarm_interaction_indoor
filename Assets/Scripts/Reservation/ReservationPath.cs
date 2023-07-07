using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Reservation
{
    public class ReservationPath
    {
        public SortedDictionary<ReservationEntry, List<ReservationEntry>> reservationList = new SortedDictionary<ReservationEntry, List<ReservationEntry>>(new ReservationEntryComparer());

        public Dictionary<IResourceUser, int> resourceUsers = new Dictionary<IResourceUser, int>();

        public IResourceUser NextDependency { get; private set; }

        public event EventHandler<DependencyResourceUserAddedEventArgs> ResourceUserAdded;
        public event EventHandler<DependencyResourceUserRemovedEventArgs> ResourceUserRemoved;

        public ReservationPath()
        {
            NextDependency = null;
        }


        private IResourceUser ComputeNextDependency()
        {
            int counter = 0;
            foreach(var kvp in reservationList)
            {
                if(counter > 1)
                {
                    return null;
                }
                else if (kvp.Value.Count > 0)
                {
                    return kvp.Value.First().User;
                }
                counter++;
            }

            return null;
        }

        public void UpdateNextDependency()
        {
            var user = ComputeNextDependency();
            if (NextDependency != null)
            {
                OnResourceUserRemoved(new DependencyResourceUserRemovedEventArgs { ResourceUser = NextDependency });
                if (user != null)
                {
                    OnResourceUserAdded(new DependencyResourceUserAddedEventArgs { ResourceUser = user });
                }
            }
            else if(user != null)
            {
                OnResourceUserAdded(new DependencyResourceUserAddedEventArgs { ResourceUser = user });
            }
            NextDependency = user;
        }

        public void AddDependency(ReservationEntry entry, ReservationEntry dependecy)
        {
            if (!reservationList.ContainsKey(entry))
            {
                reservationList.Add(entry, new List<ReservationEntry>());
            }

            reservationList[entry].Add(dependecy);

            if (resourceUsers.ContainsKey(dependecy.User))
            {
                resourceUsers[dependecy.User] = resourceUsers[dependecy.User] + 1;
            }
            else
            {
                resourceUsers[dependecy.User] = 1;
                //OnResourceUserAdded(new DependencyResourceUserAddedEventArgs { ResourceUser = dependecy.User });
            }
            UpdateNextDependency();
        }

        public void RemoveDependency(ReservationEntry entry, ReservationEntry dependecy)
        {
            if (reservationList.ContainsKey(entry))
            {
                reservationList[entry].Remove(dependecy);

                if(resourceUsers.ContainsKey(dependecy.User) && resourceUsers[dependecy.User] > 0)
                {
                    resourceUsers[dependecy.User] =  resourceUsers[dependecy.User] - 1;
                }
                else
                {
                    resourceUsers.Remove(dependecy.User);
                    //OnResourceUserRemoved(new DependencyResourceUserRemovedEventArgs { ResourceUser = dependecy.User });
                }

            }
            else
            {
                throw new Exception($"Could not find key time for {entry.User.Name} at {entry.Time}");
            }
            UpdateNextDependency();
        }

        public void AddEntry(ReservationEntry entry)
        {
            if (!reservationList.ContainsKey(entry))
            {
                //Debug.Log($"AddEntry {entry.Time} for {entry.User.Name}, Thread {Thread.CurrentThread.ManagedThreadId}");
                reservationList.Add(entry, new List<ReservationEntry>());
            }
            else
            {
                //Debug.Log($"AddEntry {entry.Time} for {entry.User.Name}, Thread {Thread.CurrentThread.ManagedThreadId}");
                Debug.Log(Text());
                Debug.Break();
                throw new Exception($"Have already a reservation entry with key time for {entry.User.Name} at {entry.Time}");
            }
        }

        public void RemoveEntry(ReservationEntry entry)
        {
            if (reservationList.ContainsKey(entry))
            {
                //Debug.Log($"RemoveEntry {entry.Time} for {entry.User.Name}, Thread {Thread.CurrentThread.ManagedThreadId}");

                foreach (var dep in reservationList[entry])
                {
                    if (resourceUsers[dep.User] > 1)
                    {
                        resourceUsers[dep.User] = --resourceUsers[dep.User];
                    }
                    else
                    {
                        resourceUsers.Remove(dep.User);
                        OnResourceUserRemoved(new DependencyResourceUserRemovedEventArgs { ResourceUser = dep.User });
                    }
                }
                reservationList.Remove(entry);
            }
            UpdateNextDependency();
        }

        public string Text()
        {
            var sb = new StringBuilder();


            foreach(var kvp in reservationList)
            {
                var entryList = kvp.Value.Select(e => $"{e.Time}: {e.User.Name} at {e.Resource.name}");
                sb.Append($"{kvp.Key.Time} at {kvp.Key.Resource.name}:\n     {string.Join("\n     ", entryList)}\n"); 
            }


            return sb.ToString();
        }


        public class ReservationEntryComparer : IComparer<ReservationEntry>
        {
            public int Compare(ReservationEntry x, ReservationEntry y)
            {
                return x.Time.CompareTo(y.Time);
            }
        }

        protected virtual void OnResourceUserAdded(DependencyResourceUserAddedEventArgs e)
        {
            ResourceUserAdded?.Invoke(this, e);
        }

        protected virtual void OnResourceUserRemoved(DependencyResourceUserRemovedEventArgs e)
        {
            ResourceUserRemoved?.Invoke(this, e);
        }

        public class DependencyResourceUserAddedEventArgs : EventArgs
        {
            public IResourceUser ResourceUser { get; set; }
        }

        public class DependencyResourceUserRemovedEventArgs : EventArgs
        {
            public IResourceUser ResourceUser { get; set; }
        }
    }
}
