using System;
using System.Collections.Generic;
using System.Linq;
namespace GraphAlgorithms
{
    public abstract class Graph<E>
        where E : IEdge<E>
    {
        protected List<E> edges;
        protected Dictionary<int,int?> shrub;
        protected Dictionary<int,int> heightOfShrub;
        protected Dictionary<int,bool> visited;
		protected Dictionary<int, int?> low;
		protected Dictionary<int, int?> in1;
		protected Dictionary<int, int> out1;
        protected List<E> bridges;
        protected List<Node<E>> articulations;
		protected int countDFS; // helper field for counting edges in DFS



		public Dictionary<int, Node<E>> Nodes
        {
            get;
            private set;
        }
        public Dictionary<E, E> Edges
        {
            get;
            private set;
        }
		protected Node<OrientedEdge> rootNode
		{
			get;
			set;
		}

        public Graph()
        {
            Nodes = new Dictionary<int, Node<E>>();
            Edges = new Dictionary<E, E>();
            rootNode = new Node<OrientedEdge>(0);
        }

        public virtual bool AddNode(Node<E> node)
        {
            if (Nodes.ContainsKey(node.Key))
                return false;
            Nodes.Add(node.Key, node);
            return true;
        }

        protected bool AddEdgeAndEndNodes(E edge){
            if(!Edges.ContainsKey(edge)){
                AddNode(edge.Start);
                AddNode(edge.End);
                Edges.Add(edge, edge);
                return true;
            }
            return false;
        }

        //TODO smazat i referenci v IndidentEdges
        public virtual bool DeleteEdge(E edge){
            return Edges.Remove(edge);
        }



        //pro neorientovane, pro orientovane zmenit rekonstrukci cesty
        //TODO: rekontrukci cesty implementuji ve zdedene tride, prvni zavolam predka a pak dopisu rekontrukci.

        /// <summary>
        /// Finds the shortest path between two nodes.
        /// </summary>
        /// <returns>Tuple of distance and dictionary of predecessors</returns>
        /// <param name="start">Start node</param>
        /// <param name="to">End node</param>
        protected Tuple<int, Dictionary<int, int?>> FindDistanceOfShortestPathAndPredecessors(Node<E> start, Node<E> to) {
            //TODO: minimova halda 
            var distance = new Dictionary<int, int>();
            var status = new Dictionary<int, Status>();
            var predecessor = new Dictionary<int, int?>();

            foreach(var node in Nodes){
                status[node.Key] = Status.UnDiscovered;
                distance[node.Key] = int.MaxValue;
                predecessor[node.Key] = null; //asi nepujde, co kdyby to byl int; mozna je jedno co tam bude
            }
            var current = new Node<E>(start);
            status[start.Key] = Status.Open;
            distance[start.Key] = 0;
            var newOpen = true;
            uint otevrene = 1;
            while(otevrene > 0){

                foreach(var node in status){
                    if(node.Value == Status.Open && (distance[node.Key] < distance[current.Key] || !newOpen )){
                        current = Nodes[node.Key];
                        newOpen = true;
                    }
                }

                //s verzi s Edge - budu resit vrchol na konci incidentni hrany. vsechno ulozeno stejne
                foreach(var edge in current.IncidentEdges.Values){
                    var neighbouringNode = edge.GetNeighbour(current);
                    var key = neighbouringNode.Key;
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
                newOpen = false;
               // current.Value = int.MaxValue;

            }
            if (distance[to.Key] != int.MaxValue)
                return new Tuple<int, Dictionary<int, int?>>((int)distance[to.Key], predecessor);
            return null;

        }


        protected bool FindAMinimalSpanningTreeShrub(ref Graph<E> graph) {
            edges = Edges.Keys.ToList().OrderBy(edge=>edge.Weight).ToList();
            shrub = new Dictionary<int, int?>(Nodes.Count);  //new List<int?>(Nodes.Count);
            heightOfShrub = new Dictionary<int, int>(Nodes.Count);
			foreach (var n in Nodes)
			{
                shrub.Add(n.Key, null);
				heightOfShrub.Add(n.Key, 0);
			}
            foreach(var edge in edges){
                if(!Find(edge.Start.Key, edge.End.Key)){
                    Union(edge.Start.Key, edge.End.Key);
                    //tady pridani hrany, a overeni ze existuje
                }
            }

            return true;
        }
        //Kruskal MST, Union find
        private int RootOf(int nodeKey){
            while(shrub[nodeKey].HasValue){
                nodeKey = (int)shrub[nodeKey];
            }
            return nodeKey;
        }

        protected void Union(int nodeKey1, int nodeKey2){
            var root1 = RootOf(nodeKey1);
            var root2 = RootOf(nodeKey2);
            if (root1 == root2) return;
            if(heightOfShrub[root1] < heightOfShrub[root2]){
                shrub[root1] = root2;
            }
            else if(heightOfShrub[root1] > heightOfShrub[root2]){
                shrub[root2] = root1;
            }
            else {
                shrub[root1] = root2;
                heightOfShrub[root2] = heightOfShrub[root2] + 1; 
            }

        }

        protected bool Find(int nodeKey1, int nodeKey2){
            if (RootOf(nodeKey1) == RootOf(nodeKey2)){
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return string.Format("[Graph: Nodes={0}, Edges={1}]", Nodes, Edges);
        }

        
    }
    enum Status {
        Open, 
        Closed,
        UnDiscovered
    }
}
