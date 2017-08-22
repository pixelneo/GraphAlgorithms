﻿using System;
using System.Collections.Generic;
namespace GraphAlgorithms
{
    public abstract class Graph<E> 
        where E : IEdge<E>
    {
        //zmenit na hashset
        public Dictionary<uint,Node<E>> Nodes {
            get;
            private set;
        }
        public Dictionary<E,E> Edges {
            get;
            private set;
        }
        public Graph()
        {
            Nodes = new Dictionary<uint, Node<E>>();
        }

        public bool AddNode(Node<E> node)
        {
            if (Nodes.ContainsKey(node.Key))
                return false;
            Nodes.Add(node.Key, node);
            //this
            return true;
        }

        protected bool AddEdgeAndEndNodes(E edge){
            if(!Edges.ContainsKey(edge)){
                if(Nodes.ContainsKey(edge.Start.Key))
                    Nodes.Add(edge.Start.Key,edge.Start);
                if(Nodes.ContainsKey(edge.End.Key))
                    Nodes.Add(edge.End.Key,edge.End);
                Edges.Add(edge, edge);
                return true;
            }
            return false;
        }



        //pro neorientovane, pro orientovane zmenit rekonstrukci cesty
        //TODO: rekontrukci cesty implementuji ve zdedene tride, prvni zavolam predka a pak dopisu rekontrukci.

        /// <summary>
        /// Finds the shortest path between two nodes.
        /// </summary>
        /// <returns>Tuple of distance and dictionary of predecessors</returns>
        /// <param name="start">Start node</param>
        /// <param name="to">End node</param>
        protected Tuple<uint, Dictionary<uint, uint?>> FindDistanceOfShortestPathAndPredecessors(Node<E> start, Node<E> to) {
            //TODO: minimova halda 
            Dictionary<uint, uint?> distance = new Dictionary<uint, uint>();
            Dictionary<uint, Status> status = new Dictionary<uint, Status>();
            Dictionary<uint, uint?> predecessor = new Dictionary<uint, uint?>();

            foreach(KeyValuePair<uint, Node<E>> node in Nodes){
                status[node.Key] = Status.UnDiscovered;
                distance[node.Key] = uint.MaxValue;
                predecessor[node.Key] = null; //asi nepujde, co kdyby to byl int; mozna je jedno co tam bude
            }
            Node<E> current = new Node<E>(start.Key, start.Value);
            status[start.Key] = Status.Open;
            distance[start.Key] = 0;
            uint otevrene = 1;
            while(otevrene > 0){

                foreach(KeyValuePair<uint, Status> node in status){
                    if(node.Value == Status.Open && distance[node.Key] < distance[current.Key]){
                        current = Nodes[node.Key];
                    }
                }

                //s verzi s Edge - budu resit vrchol na konci incidentni hrany. vsechno ulozeno stejne
                foreach(var edge in current.IncidentEdges.Values){
                    Node<E> neighbouringNode = edge.GetNeighbour(current);
                    uint key = neighbouringNode.Key;
                    if(distance[key] > distance[current.Key] + edge.Weight){
                        distance[key] = distance[current.Key] + edge.Weight;
                        status[key] = Status.Open;
                        predecessor[key] = current.Key;
                        otevrene++;
                    }
                }

                status[current.Key] = Status.Closed;
                otevrene--;
                if(current.Key == to.Key)
                    break;

                current.Value = int.MaxValue;

            }
            if (distance[to.Key] != int.MaxValue)
                return new Tuple<uint, Dictionary<uint, uint?>>((uint)distance[to.Key], predecessor);
            else
                return null;

            //neni treba preimplementovat, 
            if(distance[to.Key] < uint.MaxValue)
            {
                object result = Activator.CreateInstance(GetType());
                Node<E> previous = new Node<E>(to.Key, to.Value);
                result.AddNode(previous);
                while(!current.Equals(from)){
                    current = new Node<E,Graph<E>>(predecessor[previous.Key], Nodes[previous.Key].Value);
                   // current.AddNeighbour(previous);
                   
                    result.AddNode(current);
                    result.AddEdge();
                    result.Nodes[previous.Key].AddNeighbour(current);
                    previous = current;
                }
                return new Tuple<Graph<E>,uint>(result, distance[to]);
            }
            else
                return null; // jde to ?
        }

        //najit vsechny nebo pouze jednu?
        //je treba udelat tridu EDGE.!!!!!!
        public Graph<E> FindAMinimalSpanningTree() {

            return null;
        }

        public Dictionary<E,uint?> FindDistanceBetweenAllNodes(){
            Dictionary<E,uint?> matrix = new Dictionary<E,uint?>();
                for(int k = 0; k < Nodes.Count; k++){
                    for(int i = 1; i < Nodes.Count+1; i++){
                        for(int j = 1; j< Nodes.Count+1; j++){

                        }
                    }
                }
        }
    }
    enum Status {
        Open, 
        Closed,
        UnDiscovered
    }
}
