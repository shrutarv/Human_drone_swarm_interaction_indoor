using System;
using System.Collections.Generic;

namespace Dependency
{
    public class DependencyNode<T>

    {
        public DependencyGraph<T> Graph { get; }
        public T Data { get; set; }
        public HashSet<DependencyNode<T>> Children { get; set; }

        public DependencyNode(DependencyGraph<T> graph, T data)
        {
            Children = new HashSet<DependencyNode<T>>();
            Graph = graph;
            Data = data;
        }

        public void AddDependency(DependencyNode<T> node)
        {
            Children.Add(node);
        }

        public void RemoveDependency(DependencyNode<T> node)
        {
            Children.Remove(node);
        }
    }
}
