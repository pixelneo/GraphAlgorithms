using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures;

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

        protected int NumberOfNegativeEdges {
            get;
            private set;

        }

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
            NumberOfNegativeEdges = 0;
        }

        /// <summary>
        /// Adds new node to graph
        /// </summary>
        /// <param name="node">Node to add</param>
        /// <returns>True, if node has been added, false otherwise.</returns>
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
                if (edge.Weight < 0) {
                    NumberOfNegativeEdges++;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes edge from the collection of edges
        /// </summary>
        /// <param name="edge">Edge to remove</param>
        /// <returns>True, if successful, false if not.</returns>
        public virtual bool DeleteEdge(E edge) {
            if (edge.Weight < 0) {
                NumberOfNegativeEdges--;
            }
            return Edges.Remove(edge);
        }

        /// <summary>
        /// Finds distance between two nodes and list of predecessors on the path. Chooses appropriate algorithm (dijkstra for non-negative edges, bellman ford otherwise).
        /// </summary>
        /// <param name="start">From this node</param>
        /// <param name="end">To this node</param>
        /// <returns>Tuple of distance and path, if there is a path, null otherwise</returns>
        protected Tuple<int, Dictionary<int, int?>> FindDistanceOfShortestPathAndPredecessors(Node<E> start, Node<E> end) {
            var predecessor = new Dictionary<int, int?>();
            int? distance;
            if (NumberOfNegativeEdges >= 0) {
                distance = Dijkstra(start, end, ref predecessor);
            }
            else {
                distance = BellmanFord(start, end, ref predecessor);
            }
            if (distance == null)
                return null;
            return new Tuple<int, Dictionary<int, int?>>((int)distance, predecessor);
        }


        /// <summary>
        /// Finds distance between two nodes and list of predecessors on the path. Works on graph with non-negative edges.
        /// </summary>
        /// <param name="start">From this node</param>
        /// <param name="end">To this node</param>
        /// <param name="predecessor">Dictionary of predecessors, predecessor[k] is the predecessor of k</param>
        /// <returns>Distance, if there is a path, null otherwise</returns>
        private int? Dijkstra(Node<E> start, Node<E> end, ref Dictionary<int, int?> predecessor) {
            var distance = new Dictionary<int, int>();
            var heap = new MinHeap();

            //Dijskstra algorithm
            foreach (var nodeKey in Nodes.Keys) {
                distance.Add(nodeKey, int.MaxValue);
            }
            var current = new Node<E>(start);

            heap.Insert(0, start.Key);
            distance[start.Key] = 0;

            while (heap.Count > 0) {
                current = Nodes[heap.ExtractMin().Item2];

                foreach (var edge in current.IncidentEdges.Values) {
                    var neighbouringNode = edge.GetNeighbour(current);
                    var key = neighbouringNode.Key;

                    if (distance[key] > distance[current.Key] + edge.Weight) {
                        if (!heap.indexInHeap.ContainsKey(key)) {
                            heap.Insert(distance[current.Key] + edge.Weight, key);
                        }
                        else {
                            heap.DecreaseKey(key, distance[current.Key] + edge.Weight);
                        }

                        distance[key] = distance[current.Key] + edge.Weight;
                        predecessor[key] = current.Key;
                    }
                }
                if (current.Key == end.Key)
                    break;
            }
            if (distance[end.Key] != int.MaxValue)
                return distance[end.Key];
            return null;
        }

        /// <summary>
        /// Finds distance between two nodes and list of predecessors on the path. Works on graph with positive and negative edges.
        /// </summary>
        /// <param name="start">From this node</param>
        /// <param name="end">To this node</param>
        /// <param name="predecessor">Dictionary of predecessors, predecessor[k] is the predecessor of k</param>
        /// <returns>Distance, if there is a path, null otherwise</returns>
        protected int? BellmanFord(Node<E> start, Node<E> end, ref Dictionary<int, int?> predecessor) {
            var distance = new Dictionary<int, int>();
            foreach (var nodeKey in Nodes.Keys) {
                distance[nodeKey] = int.MaxValue;
            }
            distance[start.Key] = 0;

            int startKey, endKey, weight;
            bool changed = false;

            for (int i = 1; i < Nodes.Count; i++) {
                changed = false;
                foreach (var edge in Edges.Values) {
                    startKey = edge.Start.Key;
                    endKey = edge.End.Key;
                    weight = edge.Weight;
                    if (distance[startKey] != int.MaxValue && distance[startKey] + weight < distance[endKey]) {
                        distance[endKey] = distance[startKey] + weight;
                        predecessor[endKey] = startKey;
                        changed = true;
                    }
                }
                if (!changed) //no need for other iterations
                    break;
            }

            if (changed) {
                foreach (var edge in Edges.Values) {
                    startKey = edge.Start.Key;
                    endKey = edge.End.Key;
                    weight = edge.Weight;
                    if (distance[startKey] != int.MaxValue && distance[startKey] + weight < distance[endKey]) {
                        return null; //negative cycle
                    }
                }
            }
            return distance[end.Key];
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

        /// <summary>
        /// Finds distance of shortest path between all vertices
        /// </summary>
        /// <param name="matrixOfEdges">Matrix of edges</param>
        protected void FindShortestPathInMatrix(ref int?[,] matrixOfEdges) {
            for (int l = 0; l < Nodes.Count; l++) {
                matrixOfEdges[l, l] = 0;
            }

            //Floyd Warshall
            for (int k = 0; k < Nodes.Count; k++) {
                for (int i = 0; i < Nodes.Count; i++) {
                    for (int j = 0; j < Nodes.Count; j++) {
                        int a = matrixOfEdges[i, j] ?? int.MaxValue;
                        int b = matrixOfEdges[i, k] == null || matrixOfEdges[k, j] == null ? int.MaxValue : (int)matrixOfEdges[i, k] + (int)matrixOfEdges[k, j];
                        //var c = matrixOfEdges[k + 1, j] ?? int.MaxValue;
                        matrixOfEdges[i, j] = Math.Min(a, b);
                    }
                }
            }
        }

        //Kruskal MST, Union Find
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
