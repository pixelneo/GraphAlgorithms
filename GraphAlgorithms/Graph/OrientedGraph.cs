using System;
using System.Collections.Generic;
using System.Linq;
namespace GraphAlgorithms
{
    public class OrientedGraph : Graph<OrientedEdge>
    {
        /* START – temporary fields used in algorithms */
        private List<Node<OrientedEdge>> topologicalOrder;
        private List<OrientedGraph> graphs;
        private Stack<int> dfsStack;
        /* END */

        public OrientedGraph() : base() {
        }

        /// <summary>
        /// Adds the edge and incident nodes, if they are not in the graph already.
        /// </summary>
        /// <returns><c>true</c>, if edge was added, <c>false</c> otherwise.</returns>
        /// <param name="edge">Edge to add.</param>
        public bool AddEdge(OrientedEdge edge) {
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
        public override bool AddNode(Node<OrientedEdge> node) {
            if (base.AddNode(node)) {
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
        public Tuple<OrientedGraph, int> FindShortestPath(Node<OrientedEdge> Start, Node<OrientedEdge> End) {
            var result = base.FindDistanceOfShortestPathAndPredecessors(Start, End);
            if (result == null)
                return null;
            var distance = result.Item1;
            var predecessor = result.Item2;

            OrientedGraph graph = new OrientedGraph();
            var current = End;
            Node<OrientedEdge> previous;

            while (!current.Equals(Start) && predecessor[current.Key].HasValue) {
                previous = Nodes[(int)predecessor[current.Key]];

                OrientedEdge edge = new OrientedEdge(previous, current, 1);
                graph.AddEdge(Edges[edge]);
                current = previous;
            }
            return new Tuple<OrientedGraph, int>(graph, distance);

        }

        /// <summary>
        /// Finds the minimal spanning tree.
        /// </summary>
        /// <returns>MST, if the graph is connected. Null otherwise</returns>
        public OrientedGraph FindMST() {
            edges = Edges.Keys.ToList().OrderBy(edge => edge.Weight).ToList();
            shrub = new Dictionary<int, int?>(Nodes.Count);
            heightOfShrub = new Dictionary<int, int>(Nodes.Count);

            foreach (var node in Nodes) {
                shrub.Add(node.Key, null);
                heightOfShrub.Add(node.Key, 0);
            }
            OrientedGraph graph = new OrientedGraph();

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
        public List<Node<OrientedEdge>> FindTopologicalOrder() {
            countDFS = 0;
            visited.Clear();
            dfsStack = new Stack<int>();
            foreach (var nodeKey in Nodes.Keys) {
                if (!visited.Contains(nodeKey)) {
                    topologicalOrderDFS(nodeKey);
                }
            }
            var result = new List<Node<OrientedEdge>>();
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
        public List<OrientedGraph> FindStronglyConnectedComponents() {
            in1 = new Dictionary<int, int?>(Nodes.Count);
            low = new Dictionary<int, int?>(Nodes.Count);
            graphs = new List<OrientedGraph>();
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
                graphs.Add(new OrientedGraph());
                while (true) {
                    var nK = dfsStack.Pop();
                    graphs.Last().AddNode(new Node<OrientedEdge>(Nodes[nK]));
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
        /// <returns>The collection on UnorientedGraph objects representing the components.</returns>
        public IEnumerable<UnorientedGraph> FindWeaklyConnectedComponents() {
            //creates UnorientedGraph, treating oriented edges like unoriented
            var temporaryGraph = new UnorientedGraph();

            foreach (var node in Nodes) {
                temporaryGraph.AddNode(new Node<Edge>(node.Key, node.Value));
            }
            var components = new List<OrientedGraph>();
            var nodeKeys = new Queue<int>();
            foreach (var edge in Edges.Values) {
                var e = new Edge(edge);
                if (!temporaryGraph.Edges.ContainsKey(e))
                    temporaryGraph.AddEdge(e);
            }

            //calls UnorientedGraph method for finding connected components
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

    }
}
