using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Erathosthenes.Environment;

namespace Erathosthenes.UI
{
    [RequireComponent(typeof(Graph))]
    public class GraphView : MonoBehaviour
    {
        //THIS CLASS can be stored in the hierarchy on the SAME OBJECT as the graph class. One way to take care of this across scenes, etc is to just use REQUIRECOMPONENT.
        //It's actually a good idea anyway as you can't draw a graph, without a , uh... graph to draw.

        public GameObject nodeViewPrefab;       //<----- THIS is a the ENTIRE NODEVIEW PREFAB including the gameobject it spawns, it is not just a mesh. It has a script attached which constitutes part of the UI framework we are creating.

        public Transform frameHolder;

        //MATERIALS - note that these are handled here instead of in the NodeView class. While that class SETS the material, the materials are stored in here.
        public Material openMat;
        public Material closedMat;

        //--------------------------------------------------------------------------------------------------------------------------------------------------

        public void DrawGraph(Graph _graph)
        {
            //INSTANTIATE A NODEVIEW OBJECT FOR EACH IN THE GRAPH'S NODE ARRAY...   
            foreach(Node n in _graph.nodes)
            {
                GameObject instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity);
                //WHY DON'T WE USE THE STORED NODE POSITION???
                //Because we are drawing a NODE VIEW, which then positions the node inside itself.
                //It's a level of seperation I don;t normally employ, but interesting. The NODEVIEW handles everything for that node, but it itself gets drawn by the next object up the chain, this GraphView class.


                //SET PARENT
                instance.transform.SetParent(frameHolder);

                //GRAB THAT INSTANTIATED NODEVIEW COMPONENT...
                NodeView nv = instance.GetComponent<NodeView>();

                if(nv != null)
                {
                    //...AND THEN DRAW THAT NODE...
                    nv.DrawNode(n);     //THIS is another distinction point. We are sending a NODE from the graph to the nodeVIEW class. 
                                        //That class requires a node to actually work, but because it's UI based, it never ncludes them.
                                        //Here is where we actually pass them in.
                                        //CONSIDER HOW ALL OF THIS SEPERATION IS WORKING TO MAKE DIFFERENT SYSTEMS DISTINCT.
                                        //It also means we have to be very specific about what types of data we are passing to where, which makes lines easier to follow, and forces us to question our approach.

                    //...AND THEN SET THE NODE'S MATERIAL APPROPRIATELY
                    if(n.nodeType == NodeType.Blocked)
                    {
                        nv.SetMaterial(n, closedMat);
                    }
                    else
                    {
                        nv.SetMaterial(n, openMat);
                    }


                   


                }
            }//end foreach

        }//end initialse


    }//end class
}//end namespace UI
