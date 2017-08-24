﻿using System;
using System.Collections.Generic;
using System.Linq;
namespace GraphAlgorithms
{
    public class OrientedGraph : Graph<OrientedEdge>
    {
        private List<Node<OrientedEdge>> topologicalOrder;
        public OrientedGraph() : base()
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
        public override bool AddNode(Node<OrientedEdge> node){
            if(base.AddNode(node)){
                rootNode.AddIncidentEdge(new OrientedEdge(rootNode,node,0));
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
		public Tuple<OrientedGraph, int> FindShortestPath(Node<OrientedEdge> Start, Node<OrientedEdge> End)
		{
			var result = base.FindDistanceOfShortestPathAndPredecessors(Start, End);
			if (result == null)
				return null;
			var distance = result.Item1;
			var predecessor = result.Item2;

            OrientedGraph graph = new OrientedGraph();
            Node<OrientedEdge> previous = new Node<OrientedEdge>(End.Key, End.Value);
			graph.AddNode(previous);
			var current = End;
			while (!current.Equals(Start) && predecessor[previous.Key].HasValue)
			{
                current = new Node<OrientedEdge>((int)predecessor[previous.Key], Nodes[previous.Key].Value);

				graph.AddNode(current);
                OrientedEdge edge = new OrientedEdge(previous, current, 1);
				graph.AddEdge(Edges[edge]);
				previous = current;
			}
            return new Tuple<OrientedGraph, int>(graph, distance);

		}

        public OrientedGraph FindMST()
		{
            edges = Edges.Keys.ToList().OrderBy(edge => edge.Weight).ToList();
			shrub = new List<int?>(new int?[Nodes.Count]);
			heightOfShrub = new List<int>(new int[Nodes.Count]);
			OrientedGraph graph = new OrientedGraph();
			
			int edgeCount = 0;
			foreach (var edge in edges)
			{
				if (!Find(edge.Start.Key, edge.End.Key))
				{
					graph.AddEdge(edge);
					Union(edge.Start.Key, edge.End.Key);
					edgeCount++;
				}
			}
			if (edgeCount + 1 < Nodes.Count)
				return null;
			return graph;
		}
		
		public List<Node<OrientedEdge>> FindTopologicalOrder(){
            out1 = new List<int>();
            in1 = new List<int?>();
            DFS(rootNode.Key);
            var result = new List<Node<OrientedEdge>>();
            for (int i = Nodes.Count; i > 0; i++){
                result.Add(Nodes[i]);
            }
            return result;
        } 
        public List<OrientedGraph> FindStronglyConnectedComponents(){
            return null;
        }

        public bool IsDAG(){
			visited = new List<bool>(new bool[Nodes.Count]);
            return DagDfs(rootNode.Key);
        }

        //TODO tohle jsou slabe spojene komponenty, predelat na to!
        public IEnumerable<UnorientedGraph> FindWeaklyConnectedComponents(){
			visited = new List<bool>(new bool[Nodes.Count]);
            var components = new List<OrientedGraph>();
			var nodeKeys = new Queue<int>();
            var temporaryGraph = new UnorientedGraph();
            foreach(var edge in Edges.Values){
                var e = new Edge(edge);
                if(!temporaryGraph.Edges.ContainsKey(e))
                    temporaryGraph.AddEdge(e);
            }

            return temporaryGraph.FindConnectedComponents();
			
        }
       

        private void DFS(int nodeKey){
			if (visited[nodeKey])
				return;
			visited[nodeKey] = true;
            in1.Add(nodeKey);
			foreach (var neighbour in Nodes[nodeKey].IncidentEdges.Values)
			{
				DFS(neighbour.End.Key);
			}
			out1.Add(nodeKey);
        }

        private bool DagDfs(int nodeKey){
            if (visited[nodeKey])
                return false;
            visited[nodeKey] = true;
            foreach(var neighbour in Nodes[nodeKey].IncidentEdges.Values){
                if (!DagDfs(neighbour.End.Key)) 
                    return false;
            }
            return true;
        }

    }
}
