using System;
namespace GraphAlgorithms
{
    public class Edge : IEquatable<Edge>, IEdge<Edge>
    {
        private int weight;
        public Node<Edge> Start { get; set; }
        public Node<Edge> End { get; set; }
        public int Weight {
            get {
                return weight;
            }
            set {
                if (value >= 0)
                    weight = value;
            }
        }

        /// <summary>
        /// Gets the opposite node on the edge to the one provided.
        /// </summary>
        /// <returns>Opposite node</returns>
        /// <param name="me"></param>
        public Node<Edge> GetNeighbour(Node<Edge> me) {
            if (me.Equals(Start))
                return End;
            return Start;
        }

        public Edge(Node<Edge> start, Node<Edge> end, int weight) {
            Start = start;
            End = end;
            Weight = weight;
        }
        public Edge(OrientedEdge orientedEdge) {
            Start = new Node<Edge>(orientedEdge.Start.Key, orientedEdge.Start.Value);
            End = new Node<Edge>(orientedEdge.End.Key, orientedEdge.End.Value);
            Weight = orientedEdge.Weight;
        }

        public Edge(Edge e) {
            Start = e.Start;
            End = e.End;
            Weight = e.Weight;
        }

        /// <summary>
        /// Gets the key of the opposite node on the edge to the one provided.
        /// </summary>
        /// <returns>Key of the opposite node.</returns>
        /// <param name="meKey"></param>
        public int GetNeighbourKey(int meKey) {
            if (meKey == Start.Key)
                return End.Key;
            return Start.Key;
        }

        public bool Equals(Edge other) {
            return (this.Start.Equals(other.Start) && this.End.Equals(other.End)) || (this.Start.Equals(other.End) && this.End.Equals(other.Start));
        }

        public override int GetHashCode() {
            if (Start.Key < End.Key) {
                return (int)((((Start.Key + End.Key) * (Start.Key + End.Key + 1)) / 2) + End.Key); //cantor pairing function, viz. wikipedie
            }
            else {
                return (int)((((End.Key + Start.Key) * (End.Key + Start.Key + 1)) / 2) + Start.Key);
            }

        }

        public override string ToString() {
            return string.Format("[Edge: Start={0}, End={1}, Weight={2}]", Start, End, Weight);
        }
    }
}
