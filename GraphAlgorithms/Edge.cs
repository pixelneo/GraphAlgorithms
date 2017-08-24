using System;
namespace GraphAlgorithms
{
    public class Edge : IEquatable<Edge>, IEdge<Edge>
    {
        private int weight;
		public Node<Edge> Start { get; set; }
		public Node<Edge> End { get; set; }
        public int Weight { 
            get{
                return weight;
            } 
            set{
                if (value >= 0)
                    weight = value;
            } 
        }
        //pouzit jako: u kazdeho vrcholu bude seznam incidentnich hran
        //nejak chytre zjistit kdo je na konci hrany

        public Node<Edge> GetNeighbour(Node<Edge> me){
            if(me.Equals(Start))
                return End;
            return Start; 
        }
        public Edge(Node<Edge> start, Node<Edge> end, int weight){
            Start = start;
            End = end;
            Weight = weight;
        }
        public Edge(OrientedEdge orientedEdge){
            Start = new Node<Edge>(orientedEdge.Start.Key, orientedEdge.Start.Value);
            End = new Node<Edge>(orientedEdge.End.Key, orientedEdge.End.Value);
            Weight = orientedEdge.Weight;
        }

        public Edge(Edge e){
			Start = e.Start;
			End = e.End;
			Weight = e.Weight;
        }
        public int GetNeighbourKey(int meKey){
            if(meKey == Start.Key)
                return End.Key;
            return Start.Key; 
        }

        public bool Equals(Edge other)
        {
            if((this.Start.Equals(other.Start) && this.End.Equals(other.End)) || (this.Start.Equals(other.End) && this.End.Equals(other.Start)))
                return true;
            return false;
        }

        public override int GetHashCode(){
            if (Start.Key < End.Key){
                return (int)((((Start.Key + End.Key) * (Start.Key + End.Key + 1)) / 2) + End.Key); //cantor pairing function, viz. wikipedie
            }
            else {
                return (int)((((End.Key + Start.Key) * (End.Key + Start.Key + 1)) / 2) + Start.Key);
            }
            
        }
    }
}
