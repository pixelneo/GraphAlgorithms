using System;
namespace GraphAlgorithms
{
    public class DirectedEdge : IEdge<DirectedEdge>, IEquatable<DirectedEdge>
    {
        public Node<DirectedEdge> Start { get; set; }
        public Node<DirectedEdge> End { get; set; }
        public int Weight { get; set; }

        public DirectedEdge(Node<DirectedEdge> start, Node<DirectedEdge> end, int weight) {
            Start = start;
            End = end;
            Weight = weight;
        }

        public DirectedEdge(DirectedEdge DirectedEdge) {
            Start = new Node<DirectedEdge>(DirectedEdge.Start.Key, DirectedEdge.Start.Value);
            End = new Node<DirectedEdge>(DirectedEdge.End.Key, DirectedEdge.End.Value);
            Weight = DirectedEdge.Weight;
        }

        public bool Equals(DirectedEdge other) {
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
        public Node<DirectedEdge> GetNeighbour(Node<DirectedEdge> me) {
            return End;
        }

        public override string ToString() {
            return string.Format("[Edge: Start={0}, End={1}, Weight={2}]", Start, End, Weight);
        }
    }
}
