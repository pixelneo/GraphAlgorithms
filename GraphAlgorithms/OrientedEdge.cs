using System;
namespace GraphAlgorithms
{
    public class OrientedEdge : GenericEdge
    {
        public override Node<OrientedEdge,OrientedGraph> GetNeighbour(Node<OrientedEdge, OrientedGraph> me){
            return End;
        }
        public override int GetHashCode(){
            return (((Start.Key+End.Key)*(Start.Key+End.Key+1))/2)+End.Key;
        }
    }
}
