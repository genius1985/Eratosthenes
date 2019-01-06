using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Erathosthenes.Environment;

namespace Erathosthenes.UI {
    public class NodeView : MonoBehaviour
    {
        //BEST PRACTICE --- SEPERATING FUNCTION AND UI
        //The node and graph classes are purely about data and initialising the elements necessary for our graph to work...
        //But they DO NOT CONTAIN ANY UI STUFF, and that is wholly intentional.
        //It's best to seperate that stuff into it's own class, which we will be doing here.


        //THIS OBJECT GETS INITIALISED
        //Now this is an IMPORTANT DISTINCTION...
        //The NODE class was just a data container, and we use a constructor to create it, which is done in the graph.
        //In fact, it creates whole bunch of 'em and stores them inside an array.
        //THIS CLASS, which handles the UI REPRESENTATION of a node, actually needs to be INSTANTIATED at runtime, so we can SEE the node object.
        //That's why THIS CLASS gets the below gameobject, which is the gameobject we spawn at runtime.

        public GameObject tile;

        [Range(0f, 0.5f)]
        public float borderSize = 0.15f;        //This will be an adjustable value which will allow us to SCALE the tile prefab to make it smaller if need be. Just thinking ahead and giving ourselves options.

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void DrawNode(Node _node)
        {
            //IF WE HAVE A TILE PREFAB...
            if(tile != null)
            {
                //...THEN SET A POSITION FOR THIS NODE...
                gameObject.transform.position = _node.nodePosition;

                //...and apply any scale changes to the tile prefab.
                tile.transform.localScale = new Vector3(1f - borderSize, 1f - borderSize, 1f);      //Because borderSize of a float of less than 1, we can perform a subtraction, and will get a % scale reduction in size. No need to multiply, although I think it would do the same thing???
            }

        }//end initialise node
    



        public void SetMaterial(Node _node, Material _mat)
        {

            Renderer ren = tile.GetComponent<Renderer>();
            ren.material = _mat;

        }//end set material



}//end class
}//end namespace UI
