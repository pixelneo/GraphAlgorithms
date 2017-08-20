using System;
using System.Collections.Generic;

namespace GraphAlgorithms
{
    public class Node<E, G> : IEquatable<Node<E,G>>
        where E : GenericEdge //rozliseni orientovanych a neorientovanych hran
        where G : Graph<E> //aby nesli pridat incidentni hrany, jejichz koncove vrcholy nejsou v grafu
    {
        public uint Key;
        public Object Value = null;

        public Dictionary<E,E> IncidentEdges {
            get;
            private set;
        }        
        public Dictionary<uint, Edge> Neighbours {
            get;
            private set;
        }

        public Node(uint key, Object value = null)
        {
            //Neighbours = new Dictionary<uint, Edge>>();
            Value = value;
            Key = key;
        }

        public bool AddIncidentEdge(E edge){
            if (IncidentEdges.ContainsKey(edge))
                return false;
            IncidentEdges.Add(edge,edge);
            return true;
        }
        public bool DeleteEdge(E edge){
            return IncidentEdges.Remove(edge);
        }

        //smazat
        public bool AddNeighbour(Node neighbour, uint distance)
        {
            if (Neighbours.ContainsKey(neighbour.Key))
                return false;
                var edge = Edge;
                edge.Start = neighbour.Key;
                edge.End = 
            Neighbours.Add(neighbour.Key,new Tuple<Node, uint>(neighbour, distance));
            return true;
        }

        public bool DeleteNeighbour(uint key){
            return Neighbours.Remove(key);
        }
        //smazat END
        
        public override bool Equals(Node<E,G> other)
        {
            return this.Key.Equals(other.Key); 
        }

        public override int GetHashCode(){
            return this.Key.GetHashCode();
        }
    }
}
