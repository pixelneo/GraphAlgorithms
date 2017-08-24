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
			graph.AddNode(previous);
            var current = End;
            while (!current.Equals(Start) && predecessor[previous.Key].HasValue)
			{
				current = new Node<Edge>((int)predecessor[previous.Key], Nodes[previous.Key].Value);

				graph.AddNode(current);
                Edge edge = new Edge(previous, current, 1);
                graph.AddEdge(Edges[edge]);
				previous = current;
			}
            return new Tuple<UnorientedGraph, int>(graph, distance);

		}

        public UnorientedGraph FindMST(){
			edges = Edges.Keys.ToList().OrderBy(edge => edge.Weight).ToList();
            shrub = new List<int?>(new int?[Nodes.Count]);
			heightOfShrub = new List<int>(new int[Nodes.Count]);

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
            if (edgeCount + 1 < Nodes.Count)
                return null;
            return graph;
		}

	

        public IEnumerable<Edge> FindBridges(){
            List<Edge> bridges = new List<Edge>();
            countBridges = 0;
            in1 = new List<int?>(new int?[Nodes.Count]);
            low = new List<int?>();

            var node = Nodes.First();
            return bridgesDFS(node.Key);
        }

        private IEnumerable<Edge> bridgesDFS(int nodeKey, int? nodeParent = null){
            countDFS++;
            in1[nodeKey] = countDFS;
            low[nodeKey] = int.MaxValue;
            foreach(var edge in Nodes[nodeKey].IncidentEdges.Values){
                var w = edge.GetNeighbourKey(nodeKey);
                if(in1[w] == null){
                    bridgesDFS(w, nodeKey);
                    if(low[w] >= in1[w]){
                        yield return edge;
                    }
                    low[nodeKey] = Math.Min(low[w].Value,low[nodeKey].Value);
                }
                else if(w != nodeParent && in1[w] < in1[nodeKey]){
                    low[nodeKey] = Math.Min(low[nodeKey].Value,in1[w].Value);
                }
            }
        }

        public IEnumerable<Node<Edge>> FindArticulations(){
            return null;
        }

        /// <summary>
        /// Finds the connected components.
        /// </summary>
        /// <returns>List of instances of UnorientedGraph repsesenting list of conected components.</returns>
        public IEnumerable<UnorientedGraph> FindConnectedComponents()
		{
            visited = new List<bool>(new bool[Nodes.Count]);
            List<UnorientedGraph> components = new List<UnorientedGraph>();
            Queue<int> nodeKeys = new Queue<int>();
            foreach(var node in Nodes.Values){
                if (visited[node.Key])
                    continue;
                components.Add(new UnorientedGraph());
                nodeKeys.Enqueue(node.Key);
                visited[node.Key] = true;
                //BFS, which finds all reachable nodes
                while(nodeKeys.Count > 0){ 
                    var nodeKey = nodeKeys.Dequeue();
                    foreach(var nodeNeighbourEdge in Nodes[nodeKey].IncidentEdges.Values){
                        components.Last().AddEdge(nodeNeighbourEdge);
                        if(!visited[nodeNeighbourEdge.GetNeighbourKey(nodeKey)])
                            nodeKeys.Enqueue(nodeNeighbourEdge.GetNeighbourKey(nodeKey));
                        visited[nodeNeighbourEdge.GetNeighbourKey(nodeKey)] = true;

					}
                }
            }
            return components;
		}

        /// <summary>
        /// Is graph connected. 
        /// </summary>
        /// <returns><c>true</c>, if connected is connected, <c>false</c> otherwise.</returns>
		public bool IsConnected()
		{
			visited = new List<bool>(new bool[Nodes.Count]);
			countDFS = 0;
            IsConnectedDFS(Nodes.First().Key);
            if (countDFS < Nodes.Count)
                return false;
            return true;
		}

		private void IsConnectedDFS(int nodeKey)
		{
			if (visited[nodeKey])
				return;
			visited[nodeKey] = true;
			countDFS++;
			foreach (var neighbour in Nodes[nodeKey].IncidentEdges.Values)
			{
                IsConnectedDFS(neighbour.End.Key);
			}
		}

		protected void FindShortestPathInMatrix(ref int?[,] matrixOfEdges)
		{
			for (int k = 0; k < Nodes.Count; k++)
			{
				for (int i = 1; i < Nodes.Count + 1; i++)
				{
					for (int j = 1; j < Nodes.Count + 1; j++)
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
			foreach (var edge in Edges.Values)
			{
				//TODO dodelat
			}
			FindShortestPathInMatrix(ref matrix);
			return matrix;
		}
        
    }
}
