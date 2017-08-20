using System;
namespace GraphAlgorithms
{
    public class Edge : GenericEdge, IEquatable<Edge> 
    {

        //pouzit jako: u kazdeho vrcholu bude seznam incidentnich hran
        //nejak chytre zjistit kdo je na konci hrany

        public override Node<Edge, UnorientedGraph> GetNeighbour(Node<Edge, UnorientedGraph> me){
            if(me.Equals(Start))
                return End;
            return Start; 
        }
        public Node<Edge, UnorientedGraph> GetNeighbourKey(uint meKey){
            if(meKey == Start.Key)
                return End.Key;
            return Start.Key; 
        }

        public override bool Equals(Edge other)
        {
            if((this.Start.Equals(other.form) && this.End.Equals(other.End)) || (this.Start.Equals(other.End) && this.End.Equals(other.Start)))
                return true;
            return false;
        }

        public override int GetHashCode(){
            if (Start.Key < End.Key){
                return (((Start.Key+End.Key)*(Start.Key+End.Key+1))/2)+End.Key; //cantor pairing function, viz. wikipedie
            }
            else {
                return (((End.Key+Start.Key)*(End.Key+Start.Key+1))/2)+Start.Key;
            }
            
        }
    }
}
