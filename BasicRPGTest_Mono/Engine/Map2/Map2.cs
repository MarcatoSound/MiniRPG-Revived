using BasicRPGTest_Mono.Engine.Map2;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    class Map2
    {
        //####################################################################################
        // VARIABLES
        //####################################################################################

        public String name { get; set; } = "";
        public String filePath { get; set; } = "";
        public String fileName { get; set; } = "";

        public int width { get; set; } = 0;
        public int height { get; set; } = 0;

        // Layer Stuff
        public List<MapLayer> layers { get; set; } = new List<MapLayer>();


        // Region Stuff
        public bool drawRegionBorders { get; set; } = true;
        public List<MapRegion> regions { get; set; } = new List<MapRegion>();
        public List<MapRegion> regionsVisible { get; set; } = new List<MapRegion>();


        // Tile Stuff
        public int tileSize { get; set; } = 32;
        public int tilesWide { get; set; } = 0;
        public int tilesHigh { get; set; } = 0;

        public List<TileMaster> tileMasters { get; set; } = new List<TileMaster>();

        public Dictionary<Vector2, TileChild> childTilesAll { get; set; } = new Dictionary<Vector2, TileChild>();
        public int[,] childTiles { get; set; }
        public Dictionary<Vector2, TileChild> childTilesVisible { get; set; } = new Dictionary<Vector2, TileChild>();




        //####################################################################################
        // CONSTRUCTORS
        //####################################################################################




        //####################################################################################
        // PROPERTIES
        //####################################################################################

        public String filePathAndName
        {
            get { return filePath + fileName; }
        }



        //####################################################################################
        // FUNCTIONS
        //####################################################################################

        /*
        // Load Map from File
        private void loadMapFile (string world)
        {

            // Track how long this function takes to run
            Utility.CodeTimer codeTimer = new Utility.CodeTimer();
            codeTimer.startTimer();

            TileMaster tileMaster;
            RPGEngine.Tile tileOriginal;

            List<TileLayer> layers = new List<TileLayer>();

            String path = $"save\\{world}\\maps";
            StreamReader reader = new StreamReader($"save\\{world}\\world.json");

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                // TODO: Account for incorrect layer loading order.
                reader = new StreamReader($"{path}\\{file.Name}");
                JObject jsonLayer = JObject.Parse(reader.ReadToEnd());

                TileLayer layer = new TileLayer(jsonLayer.Value<string>("layer"));
                JArray tileArray = jsonLayer.Value<JArray>("tiles");

                foreach (JObject tileJson in tileArray)
                {
                    RPGEngine.Tile template = TileManager.get(tileJson.Value<int>("id"));
                    if (template == null) continue;
                    // Otherwise...

                    int x = tileJson.Value<int>("x");
                    int y = tileJson.Value<int>("y");

                    tileOriginal = new RPGEngine.Tile(template, new Vector2(x, y));

                    layer.setTile(tileOriginal.tilePos, tileOriginal);
                    tileOriginal = null;
                }

                layers.Add(layer);
                System.Diagnostics.Debug.WriteLine("Loaded layer: " + layer.name);

                reader.Close();

                layer = null;

                GC.Collect();

            }

        }
        */

        // Clears all Data from this Map object
        public void clear ()
        {
            this.regions.Clear();
            this.regionsVisible.Clear();

            this.layers.Clear();

            this.childTilesVisible.Clear();
            this.tileMasters.Clear();
            this.childTilesAll.Clear();
            

        }



    }
}
