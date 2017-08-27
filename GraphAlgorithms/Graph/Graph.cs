using System;
using System.Collections.Generic;
using System.Linq;
namespace GraphAlgorithms
{
    public abstract class Graph<E>
        where E : IEdge<E>
    {
        /* START – temporary fields used in algorithms */
        protected List<E> edges;
        protected Dictionary<int, int?> shrub;
        protected Dictionary<int, int> heightOfShrub;
        protected HashSet<int> visited;
        protected HashSet<int> visitedNow;
        protected Dictionary<int, int?> low;
        protected Dictionary<int, int?> in1;
        protected Dictionary<int, int> out1;
        protected List<E> bridges;
        protected List<Node<E>> articulations;
        protected int countDFS;
        /* END */


        /// <summary>
        /// Dictionary of Nodes list by their keys
        /// </summary>
        public Dictionary<int, Node<E>> Nodes {
            get;
            private set;
        }
        /// <summary>
        /// Dictionary of Edges
        /// </summary>
        /// <value>Values and key are the same.</value>
        public Dictionary<E, E> Edges {
            get;
            private set;
        }

        public Graph() {
            Nodes = new Dictionary<int, Node<E>>();
            Edges = new Dictionary<E, E>();
            visited = new HashSet<int>();
            visitedNow = new HashSet<int>();
        }

        public virtual bool AddNode(Node<E> node) {
            if (Nodes.ContainsKey(node.Key))
                return false;
            Nodes.Add(node.Key, node);
            return true;
        }

        protected bool AddEdgeAndEndNodes(E edge) {
            if (!Edges.ContainsKey(edge)) {
                AddNode(edge.Start);
                AddNode(edge.End);
                Edges.Add(edge, edge);
                return true;
            }
            return false;
        }

        //TODO smazat i referenci v IndidentEdges
        public virtual bool DeleteEdge(E edge) {
            return Edges.Remove(edge);
        }


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


            //Dijskstra algorithm
            foreach (var node in Nodes) {
                status[node.Key] = Status.UnDiscovered;
                distance[node.Key] = int.MaxValue;
                predecessor[node.Key] = null;
            }
            var current = new Node<E>(start);
            status[start.Key] = Status.Open;
            distance[start.Key] = 0;
            var newOpen = true;
            uint otevrene = 1;
            while (otevrene > 0) {

                foreach (var node in status) {
                    if (node.Value == Status.Open && (distance[node.Key] < distance[current.Key] || !newOpen)) {
                        current = Nodes[node.Key];
                        newOpen = true;
                    }
                }

                foreach (var edge in current.IncidentEdges.Values) {
                    var neighbouringNode = edge.GetNeighbour(current);
                    var key = neighbouringNode.Key;
                    if (distance[key] > distance[current.Key] + edge.Weight) {
                        distance[key] = distance[current.Key] + edge.Weight;
                        status[key] = Status.Open;
                        predecessor[key] = current.Key;
                        otevrene++;
                    }
                }

                status[current.Key] = Status.Closed;
                otevrene--;
                if (current.Key == to.Key)
                    break;
                newOpen = false;

            }
            if (distance[to.Key] != int.MaxValue)
                return new Tuple<int, Dictionary<int, int?>>((int)distance[to.Key], predecessor);
            return null;

        }


        protected bool FindAMinimalSpanningTreeShrub(ref Graph<E> graph) {
            //Kruskal algorthm
            edges = Edges.Keys.ToList().OrderBy(edge => edge.Weight).ToList();
            shrub = new Dictionary<int, int?>(Nodes.Count);
            heightOfShrub = new Dictionary<int, int>(Nodes.Count);
            foreach (var n in Nodes) {
                shrub.Add(n.Key, null);
                heightOfShrub.Add(n.Key, 0);
            }
            foreach (var edge in edges) {
                if (!Find(edge.Start.Key, edge.End.Key)) {
                    Union(edge.Start.Key, edge.End.Key);
                }
            }

            return true;
        }

        //Kruskal MST, Union find
        private int RootOf(int nodeKey) {
            while (shrub[nodeKey].HasValue) {
                nodeKey = (int)shrub[nodeKey];
            }
            return nodeKey;
        }

        protected void Union(int nodeKey1, int nodeKey2) {
            var root1 = RootOf(nodeKey1);
            var root2 = RootOf(nodeKey2);
            if (root1 == root2) return;
            if (heightOfShrub[root1] < heightOfShrub[root2]) {
                shrub[root1] = root2;
            }
            else if (heightOfShrub[root1] > heightOfShrub[root2]) {
                shrub[root2] = root1;
            }
            else {
                shrub[root1] = root2;
                heightOfShrub[root2] = heightOfShrub[root2] + 1;
            }

        }

        protected bool Find(int nodeKey1, int nodeKey2) {
            if (RootOf(nodeKey1) == RootOf(nodeKey2)) {
                return true;
            }
            return false;
        }

        public override string ToString() {
            return string.Format("[Graph: Nodes={0}, Edges={1}]", Nodes, Edges);
        }


        protected void ClearTemp() {
            edges.Clear();
            shrub.Clear();
            heightOfShrub.Clear();
            visited.Clear();
            low.Clear();
            in1.Clear();
            out1.Clear();
            bridges.Clear();
            articulations.Clear();
            countDFS = 0;

        }
    }
    enum Status
    {
        Open,
        Closed,
        UnDiscovered
    }
}
