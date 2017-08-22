﻿using System;
using System.Collections.Generic;
namespace GraphAlgorithms
{
    public class OrientedGraph : Graph<OrientedEdge>
    {
        public OrientedGraph()
        {
        }

        public bool AddEdge(OrientedEdge edge){
            if(base.AddEdgeAndEndNodes(edge)){
                var e = Edges[edge];
                Nodes[e.Start.Key].AddIncidentEdge(e);
                return true;
            }
            return false;
        }

		/// <summary>
		/// Finds the shortest path between two nodes
		/// </summary>
		/// <returns>Null if path does not exist, Tuple of the path and distance.</returns>
		/// <param name="Start">Start node</param>
		/// <param name="End">End node</param>
		public Tuple<OrientedGraph, uint> FindShortestPath(Node<OrientedEdge> Start, Node<OrientedEdge> End)
		{
			var result = base.FindDistanceOfShortestPathAndPredecessors(Start, End);
			if (result == null)
				return null;
			uint distance = result.Item1;
			var predecessor = result.Item2;

            OrientedGraph graph = new OrientedGraph();
            Node<OrientedEdge> previous = new Node<OrientedEdge>(End.Key, End.Value);
			graph.AddNode(previous);
			var current = End;
			while (!current.Equals(Start) && predecessor[previous.Key].HasValue)
			{
                current = new Node<OrientedEdge>((uint)predecessor[previous.Key], Nodes[previous.Key].Value);

				graph.AddNode(current);
                OrientedEdge edge = new OrientedEdge(previous, current, 1);
				graph.AddEdge(Edges[edge]);
				previous = current;
			}
            return new Tuple<OrientedGraph, uint>(graph, distance);

		}

		public List<Node<OrientedEdge>> FindTopologicalOrder(){
            
        } 
        public List<OrientedGraph> FindStronglyConnectedComponents(){

        }

    }
}
