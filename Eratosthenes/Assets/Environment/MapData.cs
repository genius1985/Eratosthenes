using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;       //This is needed for when we translate single chars to ints
using UnityEngine.SceneManagement;      //needed for linking resources load path to scene name

namespace Erathosthenes.Environment
{
    public class MapData : MonoBehaviour
    {
        public int mapWidth = 7;
        public int mapHeight = 7;       //These control the overall size of the map, which is adjustable.
        //HOWEVER, if using a text or image asset to design a map, the value will be set by the map design via the below methods.


        private void Start()
        {
            //(USING RESOURCES)
            string levelName = SceneManager.GetActiveScene().name;        //THIS CACHES THE LEVEL NAME, AND WILL AUTO ASSIGN ANY MAPDATA THAT MATCHES THE NAME, AT RUNTIME.

            if (textureMaps == null)
            {
                //textureMaps = Resources.Load(resourcePath + "/" + levelName) as Texture2D;
            }

            if (textAsset == null)
            {
                textAsset = Resources.Load(resourcePath + "/" + levelName) as TextAsset;
            }
        }//end start



        //============ TEXT ASSET STUFF ========== (methods and variables grouped together for now)
        public TextAsset textAsset;
        public string resourcePath = "MapData";     //This references a folder in Resources that we can use at runtime to auto- assign the text asset field.


        //CONVERTING A TXT FILE TO STRING CHUNKS
        //The text asset is pretty much one long string, and we need to chop it up into equal lengths that fit our map width and height.
        //We can do that by adding invisible 'line break' strings into the long string to force carriage returns.
        public List<string> GetTextFromFile(TextAsset _textAsset)
        {
            List<string> lines = new List<string>();

            //FIRST, CACHE THE ENTIRE TEXT FILE AS ONE STRING...
            if(_textAsset != null)
            {
                string textData = _textAsset.text;      //One of the properties of a text asset type is the entire text inside it. It's in string form.

                //...NEXT, WE NEED TO CREATE OUR 'RETURN' CHARACTERS...
                string[] delimiters = {"\r\n", "\n" };      //<----- One for windows, one for mac. A 'delimiter' is just something that seperates text, for example a comma.

                //...THEN PHYSICALLY SPLIT THE TEXT UP BY USING THE SPLIT FUNCTION ON OUR LONG STRING, AND STORE THOSE SPLIT CHUNKS IN OUT LIST!
                lines.AddRange(textData.Split(delimiters, System.StringSplitOptions.None));     //Go research SPLIT, but the options bit just forces this method to leave blank spaces in, which we may wish to use in our designs.

                //we use lines.AddRange() because split actually returns an array, and we want a list. By bunging it into the list with AddRange it is converted for us.

                //THE NEXT PROBLEM IS REVERSING THE LIST OF STRINGS...
                //When we create the maze, (0,0) is the bottom left, but the string goes from top left to bottom right when we read it.
                //By reversing the string order, we can make the list match the maze layout.
                lines.Reverse();
            }
            else
            {
                //Debug.LogError("MAPDATA: GetTextFromFile(): text asset is null --- does the mapdata file in resources match the scene name?");
            }

            return lines;

        }//end get text from file 



        //THE BELOW METHOD is just an overloaded version, so we don't have to pass in the text asset every time we call it.
        public List<string> GetTextFromFile()
        {
            return GetTextFromFile(textAsset);
        }




        //NOW WE NEED TO MAKE THE STRING'S DIMENSIONS MATCH THE MAZE...
        public void SetDimensions(List<string> _textLines)
        {
            //FIRST, LET'S SET THE MAP HEIGHT TO MATCH THE LINES COUNT...
            mapHeight = _textLines.Count;

            //WIDTH IS NEXT BUT IT'S HARDER...(we need to set the width to allow for added or lopped off characters, and fill any blank spaces with zeroes...)

            //LOOP THROUGH EACH TEXT CHUNK...
            foreach(string line in _textLines)
            {

                //... AND IF THE LINE GOES OUT PAST THE WIDTH, SET THE WIDTH TO BE THAT LONGEST LINE VALUE...
                if(line.Length > mapWidth)
                {
                    mapWidth = line.Length;
                }

                //NEXT, WE HAVE TO MODIFY THE BELOW MAKE MAP METHOD...

            }

        }//end set dimensions






        //==================================    TEXTURE BASED STUFF     ===================================================





        public Texture2D textureMaps;    //!!!!!!!!! -----   this would normally go up top, I just put it here to group everything







        public List<string> GetMapFromTexture(Texture2D _texture)
        {
            List<string> lines = new List<string>();


            if (_texture != null)
            {
                //FIRST, SET THE HEIGHT DIMENSION...
                for (int y = 0; y < _texture.height; y++)
                {
                    //...THEN CREATE A STRING FOR EACH LINE...
                    string newLine = "";

                    //...AND LOOP THROUGH EACH PIXEL ON THE HORIZONTAL ROW...
                    for (int x = 0; x < _texture.width; x++)
                    {
                        //...AND THEN POPULATE newLine BASED ON PIXEL COLOUR AT THAT INDEX!
                        if (_texture.GetPixel(x, y) == Color.black)
                        {
                            newLine += "1";                                         //note that we are using += to create a string, and not just + to add a single char...
                        }
                        else if (_texture.GetPixel(x, y) == Color.white)
                        {
                            newLine += "0";                                         //also note that we are dealing with letters, and not integers here. This string will still be converted to ints using the below methods.
                        }
                        else
                        {
                            newLine += " ";     //This is just an error catcher that will account for missed pixels.
                        }
                    }

                    //FINALLY, ADD THE COMPLETED STRING newLine TO THE LIST OF STRINGS
                    lines.Add(newLine);
                }
            }
            else
            {
                Debug.LogError("MAPDATA: GetMapFromTexture: no texture2D asset found");
            }

            return lines;

        }//end get map from texture
            
            
        //=====================================================================================

        //--------------------------------------------------------------------------------------------------------------------------------------------------

        public int[,] MakeMap()        //This method returns a 2D integer array
        {
            //===== text asset stuff 
            List<string> lines = new List<string>();

                //===== PRIORITISING TEXTURES OVER TEXT FILES               //this is basically a simple if statement to make textures get used first in the event we have both a texture and a text file.
                if(textureMaps != null)
                {
                    lines = GetMapFromTexture(textureMaps);
                }
                else
                {
                //lines = GetTextFromFile();
                Debug.LogError("MAPDATA: MakeMap(): texture map null");
                }
                //=====

            SetDimensions(lines);
            //=====


            int[,] map = new int[mapWidth, mapHeight];      //We pass the map dimensions into the integer array, making the array the size of the map.

            for(int y = 0; y < mapHeight; y++)              //Starting from 0, for each vertical space...
            {
                for(int x = 0; x < mapWidth; x++)           //...take the horizontal space...
                {
                    //map[x,y] = 0;                                          //...and assign the values of the map array at that index to be 0.    

                    if(lines[y].Length > x) //(This prevents out of index errors when a line is shorter than mapwidth)
                    {
                        map[x, y] = (int)char.GetNumericValue(lines[y][x]);     //TEXT ASSET VERSION - this one line grabs the CHAR "0" or "1" at that index, casts it to the int 0 or 1, and adds it to the map array. 
                    }
                }
            }//Basically, this loops through each grid space. Simple enough, but we also ASSIGN VALUE TO THE ARRAY INDEXES, citing both coordinates to assign value to a specific grid space.

            return map;
        }//end make map



    }//end class
}//end namespace environment
