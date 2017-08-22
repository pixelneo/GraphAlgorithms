using System;
using System.Collections.Generic;

namespace GraphAlgorithms
{
    public class Node<E> : IEquatable<Node<E>>
	    where E : IEdge<E> //rozliseni orientovanych a neorientovanych hran

	{
        public uint Key;
        public Object Value = null;

        public Dictionary<E,E> IncidentEdges {
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
        //public bool AddNeighbour(Node<E> neighbour, uint distance)
        //{
        //    if (Neighbours.ContainsKey(neighbour.Key))
        //        return false;
        //    E edge = new E();
        //    edge.Start = this;
        //    edge.End = neighbour;
        //    Neighbours.Add(neighbour.Key,new Tuple<Node<E>, uint>(neighbour, distance));
        //    return true;
        //}

        //public bool DeleteNeighbour(uint key){
        //    return Neighbours.Remove(key);
        //}
        //smazat END
        
        public bool Equals(Node<E> other)
        {
            return this.Key.Equals(other.Key); 
        }

        public override int GetHashCode(){
            return this.Key.GetHashCode();
        }
    }
}
