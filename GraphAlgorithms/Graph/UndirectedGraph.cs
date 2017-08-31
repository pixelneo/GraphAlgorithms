using System;
using System.Collections.Generic;
using System.Linq;
namespace GraphAlgorithms
{
    public class UndirectedGraph : Graph<Edge>
    {
        public UndirectedGraph() : base() {
        }

        /// <summary>
        /// Adds the edge and incident nodes, if they are not in the graph already.
        /// </summary>
        /// <returns><c>true</c>, if edge was added, <c>false</c> otherwise.</returns>
        /// <param name="edge">Edge to add.</param>
        public bool AddEdge(Edge edge) {
            if (base.AddEdgeAndEndNodes(edge)) {
                var e = Edges[edge];
                Nodes[e.Start.Key].AddIncidentEdge(e);
                Nodes[e.End.Key].AddIncidentEdge(e);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes Edge
        /// </summary>
        /// <param name="edge">Edge to remove.</param>
        /// <returns>True, if successful, false otherwise.</returns>
        public override bool DeleteEdge(Edge edge) {
            if (base.DeleteEdge(edge)) {
                Nodes[edge.Start.Key].DeleteIncidentEdge(edge);
                Nodes[edge.End.Key].DeleteIncidentEdge(edge);
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
        public Tuple<UndirectedGraph, int> FindShortestPath(Node<Edge> Start, Node<Edge> End) {
            var predecessor = new Dictionary<int, int?>();
            int? distance;
            if (NumberOfNegativeEdges <= 0) {
                distance = base.Dijkstra(Start, End, ref predecessor);
            }
            else {
                throw new NotImplementedException("Graph contains negative edges.");
            }
            if (distance == null)
                return null;

            UndirectedGraph graph = new UndirectedGraph();
            Node<Edge> previous = new Node<Edge>(End.Key, End.Value);
            var current = End;
            while (!current.Equals(Start) && predecessor[previous.Key].HasValue) {
                current = new Node<Edge>((int)predecessor[previous.Key], Nodes[previous.Key].Value);

                Edge edge = new Edge(previous, current, 1);
                graph.AddEdge(Edges[edge]);
                previous = current;
            }
            return new Tuple<UndirectedGraph, int>(graph, (int)distance);

        }

        /// <summary>
        /// Finds the minimal spanning tree.
        /// </summary>
        /// <returns>MST, if the graph is connected. Null otherwise</returns>
        public UndirectedGraph FindMST() {
            edges = Edges.Keys.ToList().OrderBy(edge => edge.Weight).ToList();
            shrub = new Dictionary<int, int?>(Nodes.Count);
            heightOfShrub = new Dictionary<int, int>(Nodes.Count);

            foreach (var node in Nodes) {
                shrub.Add(node.Key, null);
                heightOfShrub.Add(node.Key, 0);
            }

            UndirectedGraph graph = new UndirectedGraph();

            int edgeCount = 0;
            foreach (var edge in edges) {
                if (!Find(edge.Start.Key, edge.End.Key)) {
                    graph.AddEdge(edge);
                    Union(edge.Start.Key, edge.End.Key);
                    edgeCount++;
                }
            }
            if (edgeCount + 1 != Nodes.Count)
                return null;
            return graph;
        }

        /// <summary>
        /// Finds the bridges
        /// </summary>
        /// <returns>Collection of bridges.</returns>
        public IEnumerable<Edge> FindBridges() {
            if (Nodes.Count == 0)
                return null;
            bridges = new List<Edge>();
            countDFS = 0;
            in1 = new Dictionary<int, int?>();
            low = new Dictionary<int, int?>();
            foreach (var n in Nodes) {
                in1[n.Key] = null;
            }
            var node = Nodes.First();
            bridgesDFS(node.Key, null);
            return bridges;
        }

        private void bridgesDFS(int nodeKey, int? nodeParent = null) {
            countDFS++;
            in1[nodeKey] = countDFS;
            low[nodeKey] = int.MaxValue;
            foreach (var edge in Nodes[nodeKey].IncidentEdges.Values) {
                var w = edge.GetNeighbourKey(nodeKey);
                if (in1[w] == null) {
                    bridgesDFS(w, nodeKey);
                    if (low[w] >= in1[w]) {
                        if (!bridges.Contains(edge))
                            bridges.Add(edge);
                    }
                    low[nodeKey] = Math.Min(low[w].Value, low[nodeKey].Value);
                }
                else if (nodeParent.HasValue && w != nodeParent.Value) {
                    low[nodeKey] = Math.Min(low[nodeKey].Value, in1[w].Value);
                }
            }
        }

        /// <summary>
        /// Finds articulation nodes.
        /// </summary>
        /// <returns>Collection of articulations</returns>
        public IEnumerable<Node<Edge>> FindArticulations() {
            if (Nodes.Count == 0)
                return null;
            articulations = new List<Node<Edge>>();
            countDFS = 0;
            in1 = new Dictionary<int, int?>();
            low = new Dictionary<int, int?>();
            foreach (var n in Nodes) {
                in1[n.Key] = null;
            }
            var node = Nodes.First();
            articulationsDFS(node.Key, null);
            return articulations;
        }

        private void articulationsDFS(int nodeKey, int? nodeParent = null) {
            countDFS++;
            in1[nodeKey] = countDFS;
            low[nodeKey] = int.MaxValue;
            foreach (var edge in Nodes[nodeKey].IncidentEdges.Values) {
                var w = edge.GetNeighbourKey(nodeKey);
                if (in1[w] == null) {
                    articulationsDFS(w, nodeKey);
                    if (!nodeParent.HasValue && Nodes[nodeKey].IncidentEdges.Count > 1) {
                        if (!articulations.Contains(Nodes[nodeKey])) {
                            articulations.Add(Nodes[nodeKey]);
                        }
                    }
                    else if (nodeParent.HasValue && low[w] >= in1[nodeKey]) {
                        if (!articulations.Contains(Nodes[nodeKey])) {
                            articulations.Add(Nodes[nodeKey]);
                        }
                    }
                    low[nodeKey] = Math.Min(low[w].Value, low[nodeKey].Value);
                }
                else if (nodeParent.HasValue && w != nodeParent.Value) {
                    low[nodeKey] = Math.Min(low[nodeKey].Value, in1[w].Value);
                }
            }
        }

        /// <summary>
        /// Finds the connected components.
        /// </summary>
        /// <returns>Collection of instances of UndirectedGraph repsesenting list of connected components.</returns>
        public IEnumerable<UndirectedGraph> FindConnectedComponents() {

            var nodeKeys = new Queue<int>();
            var graph = new UndirectedGraph();
            foreach (var node in Nodes.Values) {
                if (visited.Contains(node.Key))
                    continue;
                nodeKeys.Enqueue(node.Key);

                //BFS, which finds all reachable nodes
                while (nodeKeys.Count > 0) {
                    var nodeKey = nodeKeys.Dequeue();
                    visited.Add(nodeKey);
                    graph.AddNode(Nodes[nodeKey]);
                    foreach (var nodeNeighbourEdge in Nodes[nodeKey].IncidentEdges.Values) {
                        graph.AddEdge(nodeNeighbourEdge);
                        if (!visited.Contains(nodeNeighbourEdge.GetNeighbourKey(nodeKey))) {
                            nodeKeys.Enqueue(nodeNeighbourEdge.GetNeighbourKey(nodeKey));
                            visited.Add(nodeNeighbourEdge.GetNeighbourKey(nodeKey));
                        }
                    }
                }
                yield return graph;
                graph = new UndirectedGraph();
            }
        }

        /// <summary>
        /// Is graph connected. 
        /// </summary>
        /// <returns><c>true</c>, if connected is connected, <c>false</c> otherwise.</returns>
		public bool IsConnected() {
            if (Nodes.Count == 0)
                return false;
            countDFS = 0;
            var nodeKeys = new Queue<int>();
            nodeKeys.Enqueue(Nodes.First().Key);

            //BFS
            while (nodeKeys.Count > 0) {
                var nodeKey = nodeKeys.Dequeue();
                visited.Add(nodeKey);
                countDFS++;
                foreach (var nodeNeighbourEdge in Nodes[nodeKey].IncidentEdges.Values) {
                    if (!visited.Contains(nodeNeighbourEdge.GetNeighbourKey(nodeKey))) {
                        nodeKeys.Enqueue(nodeNeighbourEdge.GetNeighbourKey(nodeKey));
                        visited.Add(nodeNeighbourEdge.GetNeighbourKey(nodeKey));
                    }
                }
            }
            if (countDFS != Nodes.Count)
                return false;
            return true;
        }



        /// <summary>
        /// Finds distance shortest path between all nodes using Floyd Warshall algorithm.
        /// </summary>
        /// <returns>"2D" Dictionary representing matrix of distances</returns>
        public Dictionary<int, Dictionary<int, int?>> FindShortestPathInMatrix() {
            var matrix2 = new Dictionary<int, Dictionary<int, int?>>();
            var keys = new Dictionary<int, int>();
            var indices = new Dictionary<int, int>();

            int i = 0, j = 0;
            var matrix = new int[Nodes.Count, Nodes.Count];

            //mapping Nodes keys to ints, starting from 0
            foreach (int nodeKey in Nodes.Keys) {
                keys.Add(i, nodeKey);
                indices.Add(nodeKey, i);
                j = 0;
                foreach (int nodeKey2 in Nodes.Keys) {
                    matrix[i, j++] = int.MaxValue;
                }
                i++;
            }

            //creating matrix of edges
            foreach (var edge in Edges.Values) {
                matrix[indices[edge.Start.Key], indices[edge.End.Key]] = edge.Weight;
                matrix[indices[edge.End.Key], indices[edge.Start.Key]] = edge.Weight;
            }
            base.FindShortestPathInMatrix(ref matrix);

            //mapping temporary indices to Node keys
            for (int k = 0; k < Nodes.Count; k++) {
                matrix2.Add(keys[k], new Dictionary<int, int?>());
                for (int l = 0; l < Nodes.Count; l++) {
                    matrix2[keys[k]].Add(keys[l], matrix[k, l]);
                    // matrix2[keys[l]][keys[k]] = matrix[l, k];
                }
            }

            return matrix2;
        }

        /// <summary>
        /// Checks whether the graph is bipartite and if so, returns the two parts.
        /// </summary>
        /// <returns>Null, if graph is not bipartite. Parts is it is bipartite.</returns>
        public Tuple<List<Node<Edge>>, List<Node<Edge>>> IsBipartite() {
            if (Nodes.Count == 0)
                return null;
            var color = new Dictionary<int, bool?>();
            var queue = new Queue<int>();
            visited.Clear();
            Tuple<List<Node<Edge>>, List<Node<Edge>>> result = new Tuple<List<Node<Edge>>, List<Node<Edge>>>(new List<Node<Edge>>(), new List<Node<Edge>>());

            foreach (var nodeKey in Nodes.Keys) {
                color[nodeKey] = null;
            }

            queue.Enqueue(Nodes.First().Key);
            color[Nodes.First().Key] = true;
            int item, neighbour;
            bool nowColor = true;
            while (queue.Count > 0) {
                item = queue.Dequeue();
                nowColor = (bool)color[item];
                foreach (var edge in Nodes[item].IncidentEdges.Values) {
                    neighbour = edge.GetNeighbourKey(item);
                    if (color[neighbour].HasValue && (bool)color[neighbour] == nowColor) {
                        return null;
                    }
                    if (!visited.Contains(neighbour)) {
                        queue.Enqueue(neighbour);
                        color[neighbour] = !nowColor;
                    }
                }
                visited.Add(item);
                if (nowColor) {
                    result.Item1.Add(Nodes[item]);
                }
                else {
                    result.Item2.Add(Nodes[item]);
                }
            }
            return result;
        }


    }
}
