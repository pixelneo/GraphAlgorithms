using System;
namespace GraphAlgorithms
{
    public interface IEdge<E>
        where E : IEdge<E>
    {
        Node<E> Start { get; set; }
        Node<E> End { get; set; }
        int Weight { get; set; }

        /// <summary>
        /// Gets the opposite node on the edge to the one provided.
        /// </summary>
        /// <returns>Opposite node</returns>
        /// <param name="me">Node to which you want to find the opposite.</param>
        Node<E> GetNeighbour(Node<E> me);
    }
}
