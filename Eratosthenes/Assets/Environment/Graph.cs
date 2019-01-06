using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Erathosthenes.Environment
{
    public class Graph : MonoBehaviour
    {
        public Node[,] nodes;       //Wilmer mentions that this is a SECOND 2D array, which some would say in unnecessary as we already store indexes for each node in mapadata. 
                                    //He says that despite the overead, it's actually more flexible in the long term BUT I DON'T KNOW WHY YET???
        
        public List<Node> wallNodes = new List<Node>();        //This just stores all of the nodes we assign as having nodeType.blocked.      

        //CACHED VARIABLES
        int[,] m_mapData;
        int m_mapWidth;
        int m_mapHeight;

        //The above variables are all from the MapData class, but require caching here for use as member variables that we can loop through when we initialise the graph.
        //The height and width will be found by running 'GETLENGTH' on the integer array.

        //STATIC VARIABLES
        public static readonly Vector2[] allDirections =        //READONLY enables a public and even static variable to be 'unsettable'. Only 'gettable'.
        {
            new Vector2(0f, 1f),        //ahead
            new Vector2(1f, 0f),        //right
            new Vector2(0f, -1f),       //back
            new Vector2(-1f, 0f),        //left
        };

        //-------------------------------------------------------------------------------------------------------------------------------------------------


        //INITIALISING THE GRAPH DOES THE FOLLOWING:
            //  - sets up all necessary member variables
            //  - get the nodetype to add all blocked nodes to the walls list
            //  - get each node's neighbours, and effectively join all of the nodes together


        public void InitializeGraph(int[,] _mapData)        //This will 'create' a graph of nodes. It needs the integer array as such. We have not actually craeed one yet, that gets called in.....(give callback location)
        {
            //CACHE MEMBER VARIABLES UP TOP
            m_mapData = _mapData;
            m_mapWidth = _mapData.GetLength(0);
            m_mapHeight = _mapData.GetLength(1);            //GETLENGTH works by returning the length of a given index. Here, index 0 is width, and index 1 is height, because we have a 2D coordinate system.

            //ALSO ASSIGN THE NODE ARRAY UP TOP SOME SIZES BASED ON THE INCOMING MAPDATA LENGTHS
            nodes = new Node[m_mapWidth, m_mapHeight];

            //=========

            //GET THE NODE'S TYPE (OPEN OR BLOCKED) FROM IT'S VALUES IN THE MAPDATA ARRAY (eg, [3,4])
            for(int y = 0; y < m_mapHeight; y++)
            {
                for(int x = 0; x < m_mapWidth; x++)
                {
                    NodeType type = (NodeType)_mapData[x, y];
                    //NODETYPE is available without reference because it was declared in the Node class outside of scope.
                    //We have to CAST to a nodetype, because that is what casts the 0 or 1 to open/ blocked.

                    //HOW ARE WE GETTING A 0 OR 1 FROM A COORDINATE?? LIKE HOW DOES [7,9] EQUATE TO A 0 OR 1???
                    //AFAIK, it's not actually set at this point...
                    //I am guessing here, but I think it defaults to nodeType.open when unassigned...                           TODO: verify this further down the line. When is this actually set so it can be extracted from here?

                    //CREATE A NEW NODE BASED THE NEWLY FOUND NODETYPE DATA, THE X INDEX, AND Y INDEX    
                    Node newNode = new Node(x, y, type);        //You can see how we pass in the values here to correspond with the parameters required by our constructor over in the Node class.

                    //SET THAT NEWLY CREATED NODE TO TAKE IT'S CORRECT PLACE IN THE NODE ARRAY ('nodes' up top)
                    nodes[x, y] = newNode;

                    //SET THAT NEW AND NOW INDEXED NODE TO HAVE A POSITION IN THE WORLD BASED ON IT'S INDEXES
                    newNode.nodePosition = new Vector3(x, 0, y);                                                          //GO LOOK A THE NODE CLASS AND SEE IF WE CAN REMOVE MONOBEHAVIOUR AND STILL RETAIN THIS LINE. (SOLVED: The problem was that Mono needs transform.position to work, so just abstract it by setting up a placeholder vector 3 in the node class that can do the exact same job.)

                    //IF THE TYPE OF NODE IS A WALL, WE SHOULD ADD IT TO THE LIST OF WALL NODES                                 //TODO: super confused now, as I still don't know how we are able to get the type before the node was actually created up above? Is it because this will eventually use a painted map to read data from?
                    if(type == NodeType.Blocked)
                    {
                        wallNodes.Add(newNode);
                    }
                }

            }//By looping through our newly cached member variables (up top), we can look at the specific index values of each grid space and extract a value that will correspond with the nodetype enum (0 or 1).

            //==========

            //LOOP THROUGH EACH NODE AND GET THAT NODE'S NEIGHBOURS...
            for (int y = 0; y < m_mapHeight; y++)
            {
                for (int x = 0; x < m_mapWidth; x++)
                {
                    //...AS LONG AS THAT NODE TO CHECK IS NOT BLOCKED...
                    if(nodes[x,y].nodeType != NodeType.Blocked)
                    {
                        nodes[x, y].neighbourNodes = GetNeighbours(x,y);        //If you type this method call out, you will see have access to two versions, the original and the overloaded.
                    }
                }
            }



        }//end initialize graph






        //BOUNDS CHECKING WHEN LOOKING AT NEIGHBOUR NODES
        //If we look for a neighbour that is outside the 'allDirections' array, we get an out of index array. This handles that for usm by 'checking for permisiion'.
        //It will only return 'true' and allow the ensuing neighbours to be looked at if the conditions are met.
        public bool IsWithinBounds(int x, int y)
        {

            return (x >= 0 && x < m_mapWidth        &&      y >= 0 && y < m_mapHeight );
            //I have never used return with just a pair of braces like this. Normally I would use an if statement, so I MUST REMEMBER THIS!

        }//end is within bounds




        //GETTING A NODE'S NEIGHBOURS
        //By passing in the below parameters, we have all the things we need to check and store a node's position.
        //REMEMBER: If you have a variable in scope somewhere else, you can just pass it in here, so maaaaaybe it needn't be global? All of these are global anyway. Best practice.
        List<Node> GetNeighbours(int _x, int _y, Node[,] _nodeArray, Vector2[] _allDirections)
        {

            //CREATE A TEMPORARY LIST OF NODES TO CACHE THE NEIGHBOURS ON
            List<Node> neighbours = new List<Node>();

            //LOOP THROUGH EACH DIRECTION...
            foreach (Vector2 dir in _allDirections)
            {
                //CREATE A VARIABLE THAT REPRESENTS THE X AND Y POSITIONS TO CHECK... (we can work out the spot to check by adding x to the direction.x, which gives us an offset)
                int xToCheck = _x + (int)dir.x;
                int yToCheck = _y + (int)dir.y;

                //CHECK IF THAT CREATED SPOT IS WITHIN BOUNDS... THAT THE NODE THERE IS NOT NULL... AND THAT THE NODE THERE IS NOT A WALL...
                if(IsWithinBounds(xToCheck, yToCheck) && 
                    _nodeArray[xToCheck, yToCheck] != null &&
                    _nodeArray[xToCheck, yToCheck].nodeType != NodeType.Blocked)
                {
                    //IF ALL THAT'S COOL, THEN ADD THIS NODE TO THE NEIGHBOUR NODES LIST...
                    neighbours.Add(_nodeArray[xToCheck, yToCheck]);
                }

            }//end foreach

            return neighbours;

        }//end get neighbours


        //OVERLOADED VERSION OF THE ABOVE GETNEIGHBOURS METHOD
        //To reiterate, this is done because the last TWO parameters of that method (the node and vector 2 arrays) will ALWAYS BE THE SAME.
        //Because the only two parameters that will change every method call are the first two (x and y), we can create a more condensed version of the method to simplify things.
        List<Node> GetNeighbours(int _x, int _y)
        {

            return GetNeighbours(_x, _y, nodes, allDirections);     //See? all we have to do is return the other version, but pass in the constantly changing parameters.
            //It's a bit like filtering a method.

            //SO WHY WOULD WE BOTHER KEEPING BOTH?
            //Having access to 2 vesrions of this when we call it means that if we decide to extend the game functionality at some point and need this, we have it. It's planning ahead.

        }//end get neighbours


    }//end class

}//end namespace environment
