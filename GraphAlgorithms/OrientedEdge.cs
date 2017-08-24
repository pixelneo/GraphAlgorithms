using System;
namespace GraphAlgorithms
{
    public class OrientedEdge : IEdge<OrientedEdge>
    {
        private int weight;
        public Node<OrientedEdge> Start { get; set; }
		public Node<OrientedEdge> End { get; set; }
        public int Weight { 
            get { return weight; }  
            set {
                if(value >= 0)
                    weight = value; 
            } 
        }

        public OrientedEdge(Node<OrientedEdge> start, Node<OrientedEdge> end, int weight){
            Start = start;
            End = end;
            Weight = weight;
        }
		public OrientedEdge(Edge orientedEdge)
		{
			Start = new Node<OrientedEdge>(orientedEdge.Start.Key, orientedEdge.Start.Value);
            End = new Node<OrientedEdge>(orientedEdge.End.Key, orientedEdge.End.Value);
			Weight = orientedEdge.Weight;
		}

		public override int GetHashCode(){
            return (int)((((Start.Key + End.Key) * (Start.Key + End.Key + 1)) / 2) + End.Key);
        }
		
        public Node<OrientedEdge> GetNeighbour(Node<OrientedEdge> me){
            return End;
        }
    }
}
