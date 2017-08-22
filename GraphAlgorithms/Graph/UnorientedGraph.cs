﻿using System;
using System.Collections.Generic;
namespace GraphAlgorithms
{
    public class UnorientedGraph : Graph<Edge>
    {
        public UnorientedGraph(){
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

        public Tuple<UnorientedGraph,uint> FindShortestPath(Node<Edge> Start, Node<Edge> End) {
            var result = base.FindDistanceOfShortestPathAndPredecessors(Start, End);
            if (result == null)
                return null;
            uint distance = result.Item1;
            var predecessor = result.Item2;

            UnorientedGraph graph = new UnorientedGraph();
			Node<Edge> previous = new Node<Edge>(End.Key, End.Value);
			graph.AddNode(previous);
            var current = End;
            while (!current.Equals(Start) && predecessor[previous.Key].HasValue)
			{
				current = new Node<Edge>((uint)predecessor[previous.Key], Nodes[previous.Key].Value);

				graph.AddNode(current);
                Edge edge = new Edge(previous, current, 1);
                graph.AddEdge(Edges[edge]);
				previous = current;
			}
            return new Tuple<UnorientedGraph, uint>(graph, distance);

		}
        public List<Edge> FindBridges(){
            List<Edge> bridges = new List<Edge>();
            uint count = 0;
            List<uint?> in1 = new List<uint?>();
            List<uint?> low = new List<uint?>();
            foreach(KeyValuePair<uint,Node> node in Nodes){
                in1[node.Key] = null;
            }
            Node node = Nodes.First();
            bridgesDFS(node.Key, null, ref count, ref in1, ref low, ref bridges);
            return bridges;
        }
        private void bridgesDFS(uint nodeKey, ref uint count, ref List<uint?> in1, ref List<uint?> low, ref List<Edge> bridges, uint? nodeParent = null){
            count++;
            in1[nodeKey] = count;
            low[nodeKey] = int.MaxValue();
            foreach(var edge in Nodes[nodeKey].IncidentEdges.Values){
                var w = edge.getNeighbourKey(nokeKey);
                if(in1[w] == null){
                    bridgesDFS(w, nodeKey, ref count, ref in1, ref low, ref bridges);
                    if(low[w] >= in1[w]){
                        bridges.Add(edge);
                    }
                    low[nodeKey] = Math.Min(low[w],low[nodeKey]);
                }
                else if(w != nodeParent && in1[w] < in1[nodeKey]){
                    low[nodeKey] = Math.Min(low[nodeKey],in1[w]);
                }
            }
        }
        public List<Node<Edge>> FindArticulations(){

        }
        
    }
}
