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
            if(base.AddEdge(edge)){
                var e = Edges[edge];
                Nodes[e.from.Key].AddIncidentEdge(e);
                return true;
            }
            return false;
        }
        public List<Node<OrientedEdge,OrientedGraph>> FindTopologicalOrder(){
            
        } 
        public List<OrientedGraph> FindStronglyConnectedComponents(){

        }

    }
}
