using System;
namespace GraphAlgorithms
{
    public abstract class GenericEdge
    {

        public Node<GenericEdge, Graph<GenericEdge>> Start { get; set; }
        public Node<GenericEdge, Graph<GenericEdge>> End { get; set; }
        public uint Weight { get; set; }

        public abstract Node<GenericEdge, Graph<GenericEdge>> GetNeighbour(Node<GenericEdge, Graph<GenericEdge>> me);
    }
}
