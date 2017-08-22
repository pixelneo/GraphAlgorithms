﻿using System;
namespace GraphAlgorithms
{
    public interface IEdge<E>
		where E : IEdge<E>
    {
		Node<E> Start { get; set; }
		Node<E> End { get; set; }
		uint Weight { get; set; }

        Node<E> GetNeighbour(Node<E> me);
    }
}
