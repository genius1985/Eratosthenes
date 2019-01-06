using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Erathosthenes.Environment;
using Erathosthenes.UI;                 //This is needed because we actually draw the graph from here.

namespace Erathosthenes.Core
{
    public class GameController : MonoBehaviour
    {
        public MapData mapData;     //TODO: set in a method??
        public Graph graph;

        //------------------------------------------------------------------------------------------------------------------------------------------------

        void Start()
        {
            if(mapData != null && graph != null)
            {
                CreateMapInstance();
                DrawMap();
                PositionCamera(mapData.mapWidth, mapData.mapHeight);
            }

        }//end start

        //--------------------------------------------------------------------------------------------------------------------------------------------------

        void CreateMapInstance()
        {

            //STORE ALL THE ZEROES AND ONES (generated when we ask mapData to make a map) IN A 2D INTEGER ARRAY...
            int[,] mapInstance = mapData.MakeMap();                                                                     //HOW COME we can use this without referencing MapData? We don't 'get component' anywhere... is it because most of it's fields and methods are public?

            //...THEN TAKE THAT INTEGER ARRAY AND MAKE A GRAPH FROM IT...
            graph.InitializeGraph(mapInstance);

        }//end create map instance



        void DrawMap()
        {
            //...THEN DRAW THE GRAPH, WHICH WILL IN TURN DRAW THE NODES
            //(note that we don't store the GraphView anywhere up top, but just reference it here, because it's on the Graph object anyway. No point in caching the same object twice. REMEMBER THAT!)
            GraphView graphView = graph.GetComponent<GraphView>();
            graphView.DrawGraph(graph);

            //AGAIN, it's seperated, so it;s not at all obvious here, but we set all the shit in graph when we run InitialseGraph().
            //That data is sitting in there, and we don't directly access it.
            //Instead, we run DrawGraph() which utilizes that just stored data.
        }//end draw map


        void PositionCamera(int _mapWidth, int _mapHeight)
        {

            Vector3 newPos = new Vector3((_mapWidth * 0.5f), 10f, (_mapHeight * 0.5f));
            Camera.main.transform.position = newPos;

        }//end position camera

    }//end class

}//end namespace core
