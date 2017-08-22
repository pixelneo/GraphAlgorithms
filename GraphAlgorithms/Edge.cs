using System;
namespace GraphAlgorithms
{
    public class Edge : IEquatable<Edge>, IEdge<Edge>
    {
		public Node<Edge> Start { get; set; }
		public Node<Edge> End { get; set; }
		public uint Weight { get; set; }
        //pouzit jako: u kazdeho vrcholu bude seznam incidentnich hran
        //nejak chytre zjistit kdo je na konci hrany

        public Node<Edge> GetNeighbour(Node<Edge> me){
            if(me.Equals(Start))
                return End;
            return Start; 
        }
        public Edge(Node<Edge> start, Node<Edge> end, uint weight){
            Start = start;
            End = end;
            Weight = weight;
        }
        public uint GetNeighbourKey(uint meKey){
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
