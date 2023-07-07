using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Dependency.DependencyGraph<IResourceUser>;

namespace Dependency
{
    public class DependencyManager : MonoBehaviour
    {
        DeadlockEvent deadlockEvent = null;
        DependencyGraph<IResourceUser> graph;
        int size = 0;

        public List<string> GlobalOrder;

        // Start is called before the first frame update
        void Start()
        {
            graph = new DependencyGraph<IResourceUser>();
            UpdateGlobalOrder();

        }

        // Update is called once per frame
        void Update()
        {
            if(graph.Nodes.Count != size)
            {
                UpdateGlobalOrder();
                size = graph.Nodes.Count();
            }
        }

        public void UpdateGlobalOrder()
        {
            GlobalOrder = graph.TopologicSort(out var deadlock).Select(s => $"{s.Data.Name} -> {s.Children.Count} ({string.Join(", ", s.Children.Select(c => c.Data.Name))})").ToList();
            if(deadlock != null && deadlockEvent == null)
            {
                Debug.Log($"Cycle detected during global order update", gameObject);
                foreach(var pc in deadlock.Cycle.Select(n => ((PlayerControllerSM)n.Data))){
                    pc.ToggleHighlight();
                }
                deadlockEvent = deadlock;
            }
        }


        public List<IResourceUser> ComputeDependencies()
        {
            return graph.TopologicSort(out var deadlock).Select(s => s.Data).ToList();
        }

        public DependencyNode<IResourceUser> CreateDependencyNode(IResourceUser ru)
        {
            var node = graph.CreateNode(ru);
            UpdateGlobalOrder();
            return node;
        }

        public void RemoveResourceUserNodes(IResourceUser pc)
        {
            graph.RemoveNode(pc.DependencyNode);
            UpdateGlobalOrder();
        }
    }
}
