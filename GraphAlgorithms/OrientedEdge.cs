using System;
namespace GraphAlgorithms
{
    public class OrientedEdge : IEdge<OrientedEdge>
    {
		public Node<OrientedEdge> Start { get; set; }
		public Node<OrientedEdge> End { get; set; }
		public uint Weight { get; set; }


		public override int GetHashCode(){
            return (int)((((Start.Key + End.Key) * (Start.Key + End.Key + 1)) / 2) + End.Key);
        }
		
        public Node<OrientedEdge> GetNeighbour(Node<OrientedEdge> me){
            return End;
        }
    }
}
