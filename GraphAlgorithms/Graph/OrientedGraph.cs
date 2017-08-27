﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
namespace GraphAlgorithms
{
    public class OrientedGraph : Graph<OrientedEdge>
    {
        private List<Node<OrientedEdge>> topologicalOrder;
        private List<OrientedGraph> graphs;
        private Stack<int> dfsStack;
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
            in1 = new Dictionary<int, int?>(Nodes.Count);
            low = new Dictionary<int, int?>(Nodes.Count);
            visited = new Dictionary<int, bool>(Nodes.Count); 
            graphs = new List<OrientedGraph>();
            foreach(var node in Nodes){
                in1[node.Key] = null;
                low[node.Key] = null;
                visited[node.Key] = false;
            }
            countDFS = 0;
            dfsStack = new Stack<int>();
            foreach(var node in Nodes){
                if(in1[node.Key] == null){
                    dfsSCC(node.Key);
                }
            }
            return graphs;
        }
        private void dfsSCC(int nodeKey){
            in1[nodeKey] = countDFS++;
            dfsStack.Push(nodeKey);
            low[nodeKey] = int.MaxValue;
            foreach(var neighbourEdge in Nodes[nodeKey].IncidentEdges.Values){
                var neighbourKey = neighbourEdge.End.Key;
                if(!in1[neighbourKey].HasValue){
                    dfsSCC(neighbourKey);
                    low[nodeKey] = Math.Min((int)low[nodeKey], (int)low[neighbourKey]);
                }
                else{
                    if(!visited[neighbourKey]){
                        low[nodeKey] = Math.Min((int)low[nodeKey], (int)in1[neighbourKey]);
                    }
                }
            }
            if(low[nodeKey] >= in1[nodeKey]){
                graphs.Add(new OrientedGraph());
                while(true)
                {
                    var nK = dfsStack.Pop();
                    graphs.Last().AddNode(new Node<OrientedEdge>(Nodes[nK]));
                    if (nK == nodeKey)
                        break;
                } 
                //Adding edges of the component
                foreach(var node in graphs.Last().Nodes){
                    foreach(var edge in Nodes[node.Key].IncidentEdges.Values){
                        if(graphs.Last().Nodes.ContainsKey(edge.End.Key)){
                            graphs.Last().AddEdge(edge);
                        }
                    }
                }
            }
        }

        public bool IsDAG(){
            visited = new Dictionary<int, bool>(Nodes.Count);
			foreach (var node in Nodes)
			{
                visited.Add(node.Key, false);
			}
            return DagDfs(rootNode.Key);
        }

        /// <summary>
        /// Finds the weakly connected components.
        /// </summary>
        /// <returns>The collection on UnorientedGraph objects representing the components.</returns>
        public IEnumerable<UnorientedGraph> FindWeaklyConnectedComponents(){
            visited = new Dictionary<int, bool>(Nodes.Count);
			var temporaryGraph = new UnorientedGraph();

			foreach (var node in Nodes)
			{
				visited.Add(node.Key, false);
                temporaryGraph.AddNode(new Node<Edge>(node.Key,node.Value));
			}
            var components = new List<OrientedGraph>();
			var nodeKeys = new Queue<int>();
            foreach(var edge in Edges.Values){
                var e = new Edge(edge);
                if(!temporaryGraph.Edges.ContainsKey(e))
                    temporaryGraph.AddEdge(e);
            }

            foreach(var component in temporaryGraph.FindConnectedComponents()){
                yield return component;
            }
			
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
