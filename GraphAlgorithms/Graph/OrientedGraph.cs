﻿using System;
using System.Collections.Generic;
namespace GraphAlgorithms
{
    public class OrientedGraph : Graph<OrientedEdge>
    {
        public OrientedGraph()
        {
        }

        public bool AddEdge(OrientedEdge edge){
            if(base.AddEdgeAndEndNodes(edge)){
                var e = Edges[edge];
                Nodes[e.Start.Key].AddIncidentEdge(e);
                return true;
            }
            return false;
        }
        public List<Node<OrientedEdge>> FindTopologicalOrder(){
            
        } 
        public List<OrientedGraph> FindStronglyConnectedComponents(){

        }

    }
}
