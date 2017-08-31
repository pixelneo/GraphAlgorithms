using System;
using System.Collections.Generic;
using System.Linq;
namespace GraphAlgorithms
{
    public class DirectedGraph : Graph<DirectedEdge>
    {
        /* START – temporary fields used in algorithms */
        private List<DirectedGraph> graphs;
        private Stack<int> dfsStack;
        /* END */

        public DirectedGraph() : base() {
        }

        /// <summary>
        /// Adds the edge and incident nodes, if they are not in the graph already.
        /// </summary>
        /// <returns><c>true</c>, if edge was added, <c>false</c> otherwise.</returns>
        /// <param name="edge">Edge to add.</param>
        public bool AddEdge(DirectedEdge edge) {
            if (base.AddEdgeAndEndNodes(edge)) {
                var e = Edges[edge];
                Nodes[e.Start.Key].AddIncidentEdge(e);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the node.
        /// </summary>
        /// <returns><c>true</c>, if node was added, <c>false</c> otherwise.</returns>
        /// <param name="node">Node to add.</param>
        public override bool AddNode(Node<DirectedEdge> node) {
            if (base.AddNode(node)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes edge
        /// </summary>
        /// <param name="edge">Edge to remove.</param>
        /// <returns>True, if succesful, false otherwise</returns>
        public override bool DeleteEdge(DirectedEdge edge) {
            if (base.DeleteEdge(edge)) {
                Nodes[edge.Start.Key].DeleteIncidentEdge(edge);
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
        public Tuple<DirectedGraph, int> FindShortestPath(Node<DirectedEdge> Start, Node<DirectedEdge> End) {
            int? distance = null;
            var predecessor = new Dictionary<int, int?>();

            if (NumberOfNegativeEdges <= 0) {
                distance = base.Dijkstra(Start, End, ref predecessor);
            }
            else {
                distance = BellmanFord(Start, End, ref predecessor);
            }
            if (distance == null)
                return null;

            DirectedGraph graph = new DirectedGraph();
            var current = End;
            Node<DirectedEdge> previous;

            while (!current.Equals(Start) && predecessor[current.Key].HasValue) {
                previous = Nodes[(int)predecessor[current.Key]];

                DirectedEdge edge = new DirectedEdge(previous, current, 1);
                graph.AddEdge(Edges[edge]);
                current = previous;
            }
            return new Tuple<DirectedGraph, int>(graph, (int)distance);

        }
        /// <summary>
        /// Finds distance between two nodes and list of predecessors on the path. Works on graph with positive and negative edges.
        /// </summary>
        /// <param name="start">From this node</param>
        /// <param name="end">To this node</param>
        /// <param name="predecessor">Dictionary of predecessors, predecessor[k] is the predecessor of k</param>
        /// <returns>Distance, if there is a path, null otherwise</returns>
        protected int? BellmanFord(Node<DirectedEdge> start, Node<DirectedEdge> end, ref Dictionary<int, int?> predecessor) {
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

            foreach (var edge in Edges.Values) {
                startKey = edge.Start.Key;
                endKey = edge.End.Key;
                weight = edge.Weight;
                if (distance[startKey] != int.MaxValue && distance[startKey] + weight < distance[endKey]) {
                    return null; //negative cycle
                }
            }
            return distance[end.Key];
        }

        /// <summary>
        /// Finds the minimal spanning tree.
        /// </summary>
        /// <returns>MST, if the graph is connected. Null otherwise</returns>
        public DirectedGraph FindMST() {
            edges = Edges.Keys.ToList().OrderBy(edge => edge.Weight).ToList();
            shrub = new Dictionary<int, int?>(Nodes.Count);
            heightOfShrub = new Dictionary<int, int>(Nodes.Count);

            DirectedGraph graph = new DirectedGraph();

            foreach (var node in Nodes) {
                shrub.Add(node.Key, null);
                heightOfShrub.Add(node.Key, 0);
                graph.AddNode(new Node<DirectedEdge>(node.Key, node.Value));
            }

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
        /// Finds topological order of graph iff the graph is DAG.
        /// </summary>
        /// <returns>Topological order iff graph is DAG. Else undefined</returns>
        public List<Node<DirectedEdge>> FindTopologicalOrder() {
            countDFS = 0;
            visited.Clear();
            dfsStack = new Stack<int>();
            foreach (var nodeKey in Nodes.Keys) {
                if (!visited.Contains(nodeKey)) {
                    topologicalOrderDFS(nodeKey);
                }
            }
            var result = new List<Node<DirectedEdge>>();
            while (dfsStack.Count > 0) {
                result.Add(Nodes[dfsStack.Pop()]);
            }
            return result;
        }

        private void topologicalOrderDFS(int nodeKey) {
            if (visited.Contains(nodeKey)) {
                return;
            }
            visited.Add(nodeKey);
            foreach (var neighbour in Nodes[nodeKey].IncidentEdges.Values) {
                topologicalOrderDFS(neighbour.End.Key);
            }
            dfsStack.Push(nodeKey);
        }

        /// <summary>
        /// Finds the strongly connected components.
        /// </summary>
        /// <returns>List of subgraphs representing components.</returns>
        public List<DirectedGraph> FindStronglyConnectedComponents() {
            in1 = new Dictionary<int, int?>(Nodes.Count);
            low = new Dictionary<int, int?>(Nodes.Count);
            graphs = new List<DirectedGraph>();
            foreach (var node in Nodes) {
                in1[node.Key] = null;
                low[node.Key] = null;
            }
            countDFS = 0;
            dfsStack = new Stack<int>();
            foreach (var node in Nodes) {
                if (in1[node.Key] == null) {
                    dfsSCC(node.Key);
                }
            }
            return graphs;
        }

        //uses Tarjan algorithm for strongly connected components
        private void dfsSCC(int nodeKey) {
            in1[nodeKey] = countDFS++;
            dfsStack.Push(nodeKey);
            low[nodeKey] = int.MaxValue;
            foreach (var neighbourEdge in Nodes[nodeKey].IncidentEdges.Values) {
                var neighbourKey = neighbourEdge.End.Key;
                if (!in1[neighbourKey].HasValue) {
                    dfsSCC(neighbourKey);
                    low[nodeKey] = Math.Min((int)low[nodeKey], (int)low[neighbourKey]);
                }
                else {
                    if (!visited.Contains(neighbourKey)) {
                        low[nodeKey] = Math.Min((int)low[nodeKey], (int)in1[neighbourKey]);
                    }
                }
            }
            if (low[nodeKey] >= in1[nodeKey]) {
                graphs.Add(new DirectedGraph());
                while (true) {
                    var nK = dfsStack.Pop();
                    graphs.Last().AddNode(new Node<DirectedEdge>(Nodes[nK]));
                    if (nK == nodeKey)
                        break;
                }
                //Adding edges of the component
                foreach (var node in graphs.Last().Nodes) {
                    foreach (var edge in Nodes[node.Key].IncidentEdges.Values) {
                        if (graphs.Last().Nodes.ContainsKey(edge.End.Key)) {
                            graphs.Last().AddEdge(edge);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the graph is DAG.
        /// </summary>
        /// <returns><c>true</c>, if graph is DAG, <c>false</c> otherwise.</returns>
        public bool IsDAG() {
            visited.Clear();
            visitedNow.Clear();
            foreach (var nodeKey in Nodes.Keys) {
                if (!visited.Contains(nodeKey)) {
                    visitedNow.Clear();
                    if (!DagDfs(nodeKey))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Finds the weakly connected components.
        /// </summary>
        /// <returns>The collection on UndirectedGraph objects representing the components.</returns>
        public IEnumerable<UndirectedGraph> FindWeaklyConnectedComponents() {
            //creates UndirectedGraph, treating oriented edges like unoriented
            var temporaryGraph = new UndirectedGraph();

            foreach (var node in Nodes) {
                temporaryGraph.AddNode(new Node<Edge>(node.Key, node.Value));
            }
            var components = new List<DirectedGraph>();
            var nodeKeys = new Queue<int>();
            foreach (var edge in Edges.Values) {
                var e = new Edge(edge);
                if (!temporaryGraph.Edges.ContainsKey(e))
                    temporaryGraph.AddEdge(e);
            }

            //calls UndirectedGraph method for finding connected components
            foreach (var component in temporaryGraph.FindConnectedComponents()) {
                yield return component;
            }

        }


        private bool DagDfs(int nodeKey) {
            if (visitedNow.Contains(nodeKey))
                return false;
            visited.Add(nodeKey);
            visitedNow.Add(nodeKey);
            foreach (var neighbour in Nodes[nodeKey].IncidentEdges.Values) {
                if (!DagDfs(neighbour.End.Key))
                    return false;
            }
            visitedNow.Remove(nodeKey);
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
            }
            base.FindShortestPathInMatrix(ref matrix);

            //mapping temporary indices to Node keys
            for (int k = 0; k < Nodes.Count; k++) {
                matrix2.Add(keys[k], new Dictionary<int, int?>());
                for (int l = 0; l < Nodes.Count; l++) {
                    matrix2[keys[k]].Add(keys[l], matrix[k, l]);
                }
            }

            return matrix2;
        }

    }
}
