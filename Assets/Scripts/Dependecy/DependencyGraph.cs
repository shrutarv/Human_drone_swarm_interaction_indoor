using System;
using System.Collections.Generic;

namespace Dependency
{
    public class DependencyGraph<T>
    {
        public HashSet<DependencyNode<T>> Nodes { get; set; }

        public DependencyGraph()
        {
            Nodes = new HashSet<DependencyNode<T>>();
        }

        public DependencyNode<T> CreateNode(T data)
        {
            var node = new DependencyNode<T>(this, data);
            Nodes.Add(node);
            return node;
        }

        public void RemoveNode(DependencyNode<T> node)
        {
            Nodes.Remove(node);
            foreach(var n in Nodes)
            {
                n.Children.Remove(node);
            }
        }

        public List<DependencyNode<T>> TopologicSort(out DeadlockEvent deadlock)
        {
            var results = new List<DependencyNode<T>>();
            var seen = new HashSet<DependencyNode<T>>();
            var pending = new HashSet<DependencyNode<T>>();

            deadlock = Visit(Nodes, results, seen, pending, null, null);

            return results;
        }

        private DeadlockEvent Visit(HashSet<DependencyNode<T>> graph, List<DependencyNode<T>> results, HashSet<DependencyNode<T>> dead, HashSet<DependencyNode<T>> pending, List<DependencyNode<T>> pendingList, DeadlockEvent deadlock)
        {

            // Foreach node in the graph
            foreach (var n in graph)
            {
                // Skip if node has been visited
                if (!dead.Contains(n))
                {
                    if (pendingList == null) pendingList = new List<DependencyNode<T>>();
                    if (!pending.Contains(n))
                    {
                        pending.Add(n);
                        pendingList.Add(n);
                        // recursively call this function for every child of the current node
                        deadlock = Visit(n.Children, results, dead, pending, pendingList, deadlock);
                    }
                    else
                    {
                        deadlock = new DeadlockEvent { Cycle = new List<DependencyNode<T>>(pendingList) };
                    }


                    if (pending.Contains(n))
                    {
                        pending.Remove(n);
                        pendingList.Remove(n);
                    }

                        dead.Add(n);

                    // Made it past the recusion part, so there are no more dependents.
                    // Therefore, append node to the output list.
                    results.Add(n);
                }
            }
            return deadlock;
        }

        public class DeadlockEvent
        {
            public List<DependencyNode<T>> Cycle { get; set; }
        }
    }
}
