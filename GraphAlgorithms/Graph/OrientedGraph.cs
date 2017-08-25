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
			var current = End;
            Node<OrientedEdge> previous;

            while (!current.Equals(Start) && predecessor[current.Key].HasValue)
			{
                previous = Nodes[(int)predecessor[current.Key]];

                OrientedEdge edge = new OrientedEdge(previous, current, 1);
				graph.AddEdge(Edges[edge]);
                current = previous;
			}
            return new Tuple<OrientedGraph, int>(graph, distance);

		}

        public OrientedGraph FindMST()
		{
            edges = Edges.Keys.ToList().OrderBy(edge => edge.Weight).ToList();
			shrub = new Dictionary<int, int?>(Nodes.Count);
			heightOfShrub = new Dictionary<int, int>(Nodes.Count);

			foreach (var node in Nodes)
			{
				shrub.Add(node.Key, null);
				heightOfShrub.Add(node.Key, 0);
			}
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
			if (edgeCount + 1 != Nodes.Count)
				return null;
			return graph;
		}
		
		public List<Node<OrientedEdge>> FindTopologicalOrder(){
            out1 = new Dictionary<int, int>();
            in1 = new Dictionary<int, int?>();
            countDFS = 0;
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
            visited = new Dictionary<int, bool>(Nodes.Count);
			foreach (var node in Nodes)
			{
                visited.Add(node.Key, false);
			}
            return DagDfs(rootNode.Key);
        }

        //TODO tohle jsou slabe spojene komponenty, predelat na to!
        public IEnumerable<UnorientedGraph> FindWeaklyConnectedComponents(){
            visited = new Dictionary<int, bool>(Nodes.Count);
			foreach (var node in Nodes)
			{
				visited.Add(node.Key, false);
			}
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
            in1.Add(nodeKey, countDFS++);
			foreach (var neighbour in Nodes[nodeKey].IncidentEdges.Values)
			{
				DFS(neighbour.End.Key);
			}
            out1.Add(nodeKey,countDFS++);
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
