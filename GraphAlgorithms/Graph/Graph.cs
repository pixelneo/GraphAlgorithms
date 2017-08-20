using System;
using System.Collections.Generic;
namespace GraphAlgorithms
{
    public abstract class Graph<E> 
        where E : GenericEdge
    {
        //zmenit na hashset
        public Dictionary<uint,Node<E,Graph<E>>> Nodes {
            get;
            private set;
        }
        public Dictionary<E,E> Edges {
            get;
            private set;
        }
        public Graph()
        {
            Nodes = new Dictionary<uint, Node<E,Graph<E>>>();
        }

        public bool AddNode(Node<E,Graph<E>> node)
        {
            if (Nodes.ContainsKey(node.Key))
                return false;
            Nodes.Add(node.Key, node);
            //this
            return true;
        }

        protected bool AddEdge(E edge){
            if(!Edges.Contains(edge)){
                if(Nodes.Contains(edge.from.Key))
                    Nodes.Add(edge.from.Key,edge.from);
                if(Nodes.Contains(edge.to.Key))
                    Nodes.Add(edge.to.Key,edge.to);
                return Edges.Add(edge);
            }
            return false;
        }


        //pro neorientovane, pro orientovane zmenit rekonstrukci cesty
        //TODO: rekontrukci cesty implementuji ve zdedene tride, prvni zavolam predka a pak dopisu rekontrukci.

        /// <summary>
        /// Finds the shortest path between two nodes.
        /// </summary>
        /// <returns>Tuple of Graph, representing path and and the distance.</returns>
        /// <param name="from">Start node</param>
        /// <param name="to">End node</param>
        protected Tuple<Graph<E>,uint> FindShortestPath(Node<E,Graph<E>> start, Node<E, Graph<E>> to) {
            //TODO: minimova halda 
            Dictionary<uint, uint> distance = new Dictionary<uint, uint>();
            Dictionary<uint, Status> status = new Dictionary<uint, Status>();
            Dictionary<uint, uint?> predecessor = new Dictionary<uint, uint?>();

            foreach(KeyValuePair<uint, Node<E,Graph<E>>> node in Nodes){
                status[node.Key] = Status.UnDiscovered;
                distance[node.Key] = uint.MaxValue;
                predecessor[node.Key] = null; //asi nepujde, co kdyby to byl int; mozna je jedno co tam bude
            }
            Node<E, Graph<E>> current = new Node<E, Graph<E>>(start.Key, start.Value);
			uint currentKey = start.Key;
            uint currentValue = 0;
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
                foreach(var edge in current.IncidentEdges.Keys){
                    var neighbouringNode = edge.getNeighbour(current).Key;
                    if(distance[neighbouringNode] > distance[current.Key] + entry.weight){
                         otevrene[neighbouringNode] = distance[current.Key] + entry.weight;
                        status[neighbouringNode] = Status.Open;
                         otevrene++;
                         predecessor[neighbouringNode] = current.Key;
                    }
                }
                /* 
                foreach(var entry in current.Neighbours.Values){
                    if(distance[entry.Item1] > distance[current.Key] + entry.Item2){
                         otevrene[entry.Item1] = distance[current.Key] + entry.Item2;
                         status[entry.Item1] = .Open;
                         otevrene++;
                         predecessor[entry.Item1] = current.Key;
                    }
                }*/
                status[current.Key] = Status.Closed;
                otevrene--;
                if(current.Key == to.Key)
                    break;

                current.Value = int.MaxValue;

            }
            //neni treba preimplementovat, 
            if(distance[to.Key] < uint.MaxValue)
            {
                object result = Activator.CreateInstance(GetType());
                Node<E,Graph<E>> previous = new Node<E,Graph<E>>(to.Key, to.Value);
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
