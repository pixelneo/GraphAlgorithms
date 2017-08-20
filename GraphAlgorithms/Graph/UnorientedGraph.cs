﻿using System;
using System.Collections.Generic;
namespace GraphAlgorithms
{
    public class UnorientedGraph : Graph<Edge> 
    {
        public UnorientedGraph(){
        }

        public bool AddEdge(Edge edge){
            if(base.AddEdge(edge)){
                var e = Edges[edge];
                Nodes[e.from.Key].AddIncidentEdge(e);
                Nodes[e.to.Key].AddIncidentEdge(e);
                return true;
            }
            return false;
        }
        /* 
        public bool AddEdge(Node node1, Node node2, uint weight = 0){
            Edge<K> edge = new Edge<K>;
            edge.from = node1;
            edge.to = node2;
            edge.weight = weight;
            return AddEdge(edge);
        }*/

        public Tuple<UnorientedGraph,uint> FindShortestPath(Node<Edge,UnorientedGraph> from, Node<Edge,UnorientedGraph> to) {
            super.FindShortestPath(from, to);
        }
        public List<Edge> FindBridges(){
            List<Edge>> bridges = new List<Edge>();
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
        private void bridgesDFS(uint nodeKey, uint? nodeParent = null, ref uint count, ref List<uint?> in1, ref List<uint?> low, ref List<Edge> bridges){
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
        public List<Node> FindArticulations(){

        }
        
    }
}
