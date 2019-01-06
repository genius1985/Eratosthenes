using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Erathosthenes.Environment
{
    public enum NodeType           //We make the node type a public enum out of scope, so it can be refrenced in the class.
    {
        Open = 0,
        Blocked = 1                 //We use an enum over a bool so we can assign a binary 0 or 1 to each option.
    }

    public class Node                        //node is a data container and needn't inherit from Monobehaviour
    {
        public NodeType nodeType = NodeType.Open;     


        [SerializeField] bool startNode;
        [SerializeField] bool goalNode;     //shifts camera one frame
        [SerializeField] bool endNode;      //ends level


        public int xIndex = -1;
        public int yIndex = -1;             //Both of these get set to be invalid values initially.


        public Vector3 nodePosition;


        public List<Node> neighbourNodes = new List<Node>();
        public Node previousNode = null;                      //This goes after the list becase it will one of the 4 nodes in the list.


        //NODE CONSTRUCTOR
        public Node(int _xIndex, int _yIndex, NodeType _nodeType)
        {
            this.xIndex = _xIndex;
            this.yIndex = _yIndex;
            this.nodeType = _nodeType;
        }   //We can use a constructor BECAUSE THIS CLASS DOES NOT INHERIT FROM MONO!
            //Constructors have no return type.
            //Constructors can do a bunch of stuff, but all we are using it for is to set the values of 'this' Node to be whatever we pass in.
            //If we WERE using Monobehaviour, it would complain about you cannot create a 'new' anything, but must instead use AddComponent().

        //-------------------------------------------------------------------------------------------------------------------------------------------------

        public void Reset()     //RESET just enables us to clear pathfinding data by clearing the previous node stored. That's all pathfinding is, really. A reference to a previous node.
        {   

            previousNode = null;

        }//end reset


    }//end class

}//end namespace environment

