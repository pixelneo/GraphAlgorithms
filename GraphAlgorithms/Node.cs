using System;
using System.Collections.Generic;

namespace GraphAlgorithms
{
    public class Node<E> : IEquatable<Node<E>>
        where E : IEdge<E> //differencing between oriented and unoriented edges

    {
        private int key;
        public int Key {
            get {
                return key;
            }
            set {
                if (value >= 0) {
                    key = value;
                }
            }
        }
        public Object Value = null;

        /// <summary>
        /// Incident edges to the node.
        /// </summary>
        public Dictionary<E, E> IncidentEdges {
            get;
            private set;
        }

        public Node(int key, Object value = null, Dictionary<E, E> incidentEdges = null) {
            IncidentEdges = incidentEdges ?? new Dictionary<E, E>();
            Value = value;
            Key = key;
        }

        public Node(Node<E> node) {
            IncidentEdges = node.IncidentEdges;
            Key = node.Key;
            Value = node.Value;
        }

        public bool AddIncidentEdge(E edge) {
            if (IncidentEdges.ContainsKey(edge))
                return false;
            IncidentEdges.Add(edge, edge);
            return true;
        }

        public bool DeleteIncidentEdge(E edge) {
            return IncidentEdges.Remove(edge);
        }

        public bool Equals(Node<E> other) {
            return this.Key.Equals(other.Key);
        }

        public override int GetHashCode() {
            return this.Key.GetHashCode();
        }
        public override string ToString() {
            return string.Format("[Node: Key={0}]", Key);
        }
    }
}
