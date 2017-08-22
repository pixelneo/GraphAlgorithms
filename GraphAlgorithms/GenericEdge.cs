using System;
namespace GraphAlgorithms
{
    public abstract class GenericEdge
    {



        public abstract Node<GenericEdge, Graph<GenericEdge>> GetNeighbour(Node<GenericEdge, Graph<GenericEdge>> me);
    }
}
