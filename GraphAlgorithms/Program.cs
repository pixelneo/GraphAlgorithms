using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace GraphAlgorithms
{
    class MainClass
    {
        public static void Main(string[] args)
        {
			/*
            var graph = new UnorientedGraph();
			graph.AddEdge(new Edge(new Node<Edge>(1), new Node<Edge>(2), 2));
			graph.AddEdge(new Edge(new Node<Edge>(2), new Node<Edge>(3), 2));
			graph.AddEdge(new Edge(new Node<Edge>(3), new Node<Edge>(4), 3));
			graph.AddEdge(new Edge(new Node<Edge>(4), new Node<Edge>(7), 5));
			graph.AddEdge(new Edge(new Node<Edge>(7), new Node<Edge>(5), 1));
			graph.AddEdge(new Edge(new Node<Edge>(7), new Node<Edge>(8), 5));
			graph.AddEdge(new Edge(new Node<Edge>(5), new Node<Edge>(6), 11));
			graph.AddEdge(new Edge(new Node<Edge>(1), new Node<Edge>(6), 10));

            graph.AddNode(new Node<Edge>(9));
            graph.AddEdge(new Edge(graph.Nodes[1],graph.Nodes[9],5));


            var start = graph.Nodes[1];
            var end = graph.Nodes[8];

			//var shortestPath = graph.FindShortestPath(start,end);
			//var mst = graph.FindMST();
			//var br = graph.FindBridges();
			//var com = graph.FindConnectedComponents();
			//var isCon = graph.IsConnected();
			//var art = graph.FindArticulations();
			//var  = graph.();
			//Console.WriteLine(art);
*/
			var graph2 = new OrientedGraph();

			graph2.AddEdge(new OrientedEdge(new Node<OrientedEdge>(1), new Node<OrientedEdge>(2), 2));
			graph2.AddEdge(new OrientedEdge(new Node<OrientedEdge>(2), new Node<OrientedEdge>(3), 1));
			graph2.AddEdge(new OrientedEdge(new Node<OrientedEdge>(3), new Node<OrientedEdge>(4), 3));
			graph2.AddEdge(new OrientedEdge(new Node<OrientedEdge>(4), new Node<OrientedEdge>(7), 5));
			graph2.AddEdge(new OrientedEdge(new Node<OrientedEdge>(7), new Node<OrientedEdge>(5), 1));
			graph2.AddEdge(new OrientedEdge(new Node<OrientedEdge>(7), new Node<OrientedEdge>(8), 5));
			graph2.AddEdge(new OrientedEdge(new Node<OrientedEdge>(5), new Node<OrientedEdge>(6), 11));
			graph2.AddEdge(new OrientedEdge(new Node<OrientedEdge>(1), new Node<OrientedEdge>(6), 10));
            graph2.AddEdge(new OrientedEdge(new Node<OrientedEdge>(1), new Node<OrientedEdge>(3), 1));
            graph2.AddEdge(new OrientedEdge(graph2.Nodes[3],graph2.Nodes[1],4));
            graph2.AddNode(new Node<OrientedEdge>(9));
			var start2 = graph2.Nodes[5];
			var end2 = graph2.Nodes[8];

            //var shortestPath2 = graph2.FindShortestPath(start2, end2);
            //var mst = graph2.FindMST();
            //var bla = graph2.FindWeaklyConnectedComponents();
            var scc = graph2.FindStronglyConnectedComponents();
            Console.WriteLine(scc);

            


		}
    }
}
