﻿using System;
using System.Collections.Generic;
using System.Linq;
namespace GraphAlgorithms
{
    public class UnorientedGraph : Graph<Edge>
    {
        private int countBridges; //helper field
		public UnorientedGraph() : base(){
        }

        public bool AddEdge(Edge edge){
            if(base.AddEdgeAndEndNodes(edge)){
                var e = Edges[edge];
                Nodes[e.Start.Key].AddIncidentEdge(e);
                Nodes[e.End.Key].AddIncidentEdge(e);
                return true;
            }
            return false;
        }
        /* 
        public bool AddEdge(Node node1, Node node2, uint weight = 0){
            Edge<K> edge = new Edge<K>;
            edge.Start = node1;
            edge.End = node2;
            edge.weight = weight;
            return AddEdge(edge);
        }*/

        /// <summary>
        /// Finds the shortest path between two nodes
        /// </summary>
        /// <returns>Null if path does not exist, Tuple of the path and distance.</returns>
        /// <param name="Start">Start node</param>
        /// <param name="End">End node</param>
        public Tuple<UnorientedGraph,int> FindShortestPath(Node<Edge> Start, Node<Edge> End) {
            var result = base.FindDistanceOfShortestPathAndPredecessors(Start, End);
            if (result == null)
                return null;
            var distance = result.Item1;
            var predecessor = result.Item2;

            UnorientedGraph graph = new UnorientedGraph();
			Node<Edge> previous = new Node<Edge>(End.Key, End.Value);
            var current = End;
            while (!current.Equals(Start) && predecessor[previous.Key].HasValue)
			{
				current = new Node<Edge>((int)predecessor[previous.Key], Nodes[previous.Key].Value);

                Edge edge = new Edge(previous, current, 1);
                graph.AddEdge(Edges[edge]);
				previous = current;
			}
            return new Tuple<UnorientedGraph, int>(graph, distance);

		}

        public UnorientedGraph FindMST(){
			edges = Edges.Keys.ToList().OrderBy(edge => edge.Weight).ToList();
            shrub = new Dictionary<int, int?>(Nodes.Count);
			heightOfShrub = new Dictionary<int, int>(Nodes.Count);

			foreach(var node in Nodes){
				shrub.Add(node.Key, null);
                heightOfShrub.Add(node.Key, 0);
			}

            UnorientedGraph graph = new UnorientedGraph();

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
        public IEnumerable<Edge> FindBridges(){
            bridges = new List<Edge>();
            countDFS = 0;
            in1 = new Dictionary<int, int?>();
            low = new Dictionary<int, int?>();
            foreach(var n in Nodes){
                in1[n.Key] = null;
            }
            var node = Nodes.First();
            bridgesDFS(node.Key, null);
            return bridges;
        }

        private void bridgesDFS(int nodeKey, int? nodeParent = null){          
            countDFS++;
            in1[nodeKey] = countDFS;
            low[nodeKey] = int.MaxValue;
            foreach(var edge in Nodes[nodeKey].IncidentEdges.Values){
                var w = edge.GetNeighbourKey(nodeKey);
                if(in1[w] == null){
                    bridgesDFS(w, nodeKey);
                    if(low[w] >= in1[w]){
                        if(!bridges.Contains(edge))
                            bridges.Add(edge);
                    }
                    low[nodeKey] = Math.Min(low[w].Value,low[nodeKey].Value);
                }
                else if(nodeParent.HasValue && w != nodeParent.Value){ 
					low[nodeKey] = Math.Min(low[nodeKey].Value,in1[w].Value);
                }
            }
        }

        public IEnumerable<Node<Edge>> FindArticulations(){
            articulations = new List<Node<Edge>>();
            countDFS = 0;
			in1 = new Dictionary<int, int?>();
			low = new Dictionary<int, int?>();
			foreach (var n in Nodes)
			{
				in1[n.Key] = null;
			}
			var node = Nodes.First();
            articulationsDFS(node.Key, null);
            return articulations;        
        }

		private void articulationsDFS(int nodeKey, int? nodeParent = null)
		{
			countDFS++;
			in1[nodeKey] = countDFS;
			low[nodeKey] = int.MaxValue;
			foreach (var edge in Nodes[nodeKey].IncidentEdges.Values)
			{
				var w = edge.GetNeighbourKey(nodeKey);
				if (in1[w] == null)
				{
                    articulationsDFS(w, nodeKey);
                    if(!nodeParent.HasValue && Nodes[nodeKey].IncidentEdges.Count > 1){
                        if (!articulations.Contains(Nodes[nodeKey]))
                        {
                            articulations.Add(Nodes[nodeKey]);
                        }
                    }
                    else if (nodeParent.HasValue && low[w] >= in1[nodeKey])
					{
                        if (!articulations.Contains(Nodes[nodeKey]))
                        {
                            articulations.Add(Nodes[nodeKey]);
                        }
					}
					low[nodeKey] = Math.Min(low[w].Value, low[nodeKey].Value);
				}
                else if (nodeParent.HasValue && w != nodeParent.Value)
				{
					low[nodeKey] = Math.Min(low[nodeKey].Value, in1[w].Value);
				}
			}
		}

        /// <summary>
        /// Finds the connected components.
        /// </summary>
        /// <returns>List of instances of UnorientedGraph repsesenting list of conected components.</returns>
        public IEnumerable<UnorientedGraph> FindConnectedComponents()
		{
            visited = new Dictionary<int, bool>(Nodes.Count);
            foreach(var n in Nodes){
                visited.Add(n.Key, false);
            }
            var nodeKeys = new Queue<int>();
            var graph = new UnorientedGraph();
            foreach(var node in Nodes.Values){
                if (visited[node.Key])
                    continue;
                nodeKeys.Enqueue(node.Key);
                //BFS, which finds all reachable nodes
                while(nodeKeys.Count > 0){ 
                    var nodeKey = nodeKeys.Dequeue();
					visited[nodeKey] = true;
                    graph.AddNode(Nodes[nodeKey]);
					foreach(var nodeNeighbourEdge in Nodes[nodeKey].IncidentEdges.Values){
                        graph.AddEdge(nodeNeighbourEdge);
                        if (!visited[nodeNeighbourEdge.GetNeighbourKey(nodeKey)])
                        {
                            nodeKeys.Enqueue(nodeNeighbourEdge.GetNeighbourKey(nodeKey));
                            visited[nodeNeighbourEdge.GetNeighbourKey(nodeKey)] = true; 
                        }
					}
                }
                yield return graph;
                graph = new UnorientedGraph();
            }
		}

        /// <summary>
        /// Is graph connected. 
        /// </summary>
        /// <returns><c>true</c>, if connected is connected, <c>false</c> otherwise.</returns>
		public bool IsConnected()
		{
            visited = new Dictionary<int, bool>(Nodes.Count);
			foreach (var n in Nodes)
			{
				visited.Add(n.Key, false);
			}			
            countDFS = 0;
            var nodeKeys = new Queue<int>();
            nodeKeys.Enqueue(Nodes.First().Key);
            while(nodeKeys.Count > 0){
                var nodeKey = nodeKeys.Dequeue();
                visited[nodeKey] = true;
                countDFS++;
                foreach(var nodeNeighbourEdge in Nodes[nodeKey].IncidentEdges.Values){
                    if (!visited[nodeNeighbourEdge.GetNeighbourKey(nodeKey)]){
                        nodeKeys.Enqueue(nodeNeighbourEdge.GetNeighbourKey(nodeKey));
						visited[nodeNeighbourEdge.GetNeighbourKey(nodeKey)] = true;
					} 
                }
            }
            if (countDFS != Nodes.Count)
                return false;
            return true;
		}


		protected void FindShortestPathInMatrix(ref int?[,] matrixOfEdges)
		{
            if(matrixOfEdges.Length != Nodes.Count)
                return;
			for (int l = 0; l < Nodes.Count; l++)
			{
				matrixOfEdges[l, l] = 0;
			}
			for (int k = 0; k < Nodes.Count; k++)
			{
				for (int i = 0; i < Nodes.Count; i++)
				{
					for (int j = 0; j < Nodes.Count; j++)
					{
						var a = matrixOfEdges[i, j] ?? int.MaxValue;
						var b = matrixOfEdges[i, k + 1] ?? int.MaxValue;
						var c = matrixOfEdges[k + 1, j] ?? int.MaxValue;
						matrixOfEdges[i, j] = Math.Min(a, b + c);
					}
				}
			}
		}


		protected int?[,] FindShortestPathInMatrix()
		{
			var matrix = new int?[Nodes.Count, Nodes.Count];

			FindShortestPathInMatrix(ref matrix);
			return matrix;
		}
        
    }
}
