using System;
namespace GraphAlgorithms
{
    public class OrientedEdge : IEdge<OrientedEdge>, IEquatable<OrientedEdge>
    {
        private int weight;
        public Node<OrientedEdge> Start { get; set; }
        public Node<OrientedEdge> End { get; set; }
        public int Weight {
            get { return weight; }
            set {
            	weight = value;
            }
        }

        public OrientedEdge(Node<OrientedEdge> start, Node<OrientedEdge> end, int weight) {
            Start = start;
            End = end;
            Weight = weight;
        }

        public OrientedEdge(OrientedEdge orientedEdge) {
            Start = new Node<OrientedEdge>(orientedEdge.Start.Key, orientedEdge.Start.Value);
            End = new Node<OrientedEdge>(orientedEdge.End.Key, orientedEdge.End.Value);
            Weight = orientedEdge.Weight;
        }

        public bool Equals(OrientedEdge other) {
            return (this.Start.Equals(other.Start) && this.End.Equals(other.End));
        }

        public override int GetHashCode() {
            return ((((Start.Key + End.Key) * (Start.Key + End.Key + 1)) / 2) + End.Key);//cantor pairing function, viz. wikipedie
        }

        /// <summary>
        /// Gets the End node.
        /// </summary>
        /// <returns>End node</returns>
        /// <param name="me"></param>        
        public Node<OrientedEdge> GetNeighbour(Node<OrientedEdge> me) {
            return End;
        }

        public override string ToString() {
            return string.Format("[Edge: Start={0}, End={1}, Weight={2}]", Start, End, Weight);
        }
    }
}
