using BasicRPGTest_Mono.Engine.Datapacks;
using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace BasicRPGTest_Mono.Engine.Items
{
    public class ParentItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public Graphic graphic { get; set; }
        public string description { get; set; }
        public int maxStackSize { get; set; } = 99;

        // Attack data
        public float swingDist;
        public SwingStyle swingStyle;
        public Rectangle hitbox;
        public double damage;

        public ParentItem(string displayName, Texture2D texture, int maxStack = 99) : this(displayName, new Graphic(texture), maxStack) { }
        public ParentItem(string displayName, Graphic graphic, int maxStack = 99)
        {
            id = ItemManager.getItems().Count;
            name = Util.cleanString(displayName).ToLower();
            this.displayName = displayName;
            this.graphic = graphic;
            this.maxStackSize = maxStack;


            if (GetType() == typeof(ParentItem)) swingDist = 0.785f;
            if (GetType() == typeof(ParentItem)) swingStyle = SwingStyle.Slash;
            if (GetType() == typeof(ParentItem)) hitbox = new Rectangle(0, 0, 24, 24);
            if (GetType() == typeof(ParentItem)) damage = 1;
        }
        public ParentItem(DataPack pack, YamlSection config)
        {
            id = ItemManager.getItems().Count;

            // Easy ones first
            name = config.getName();
            displayName = config.getString("display_name", name);
            maxStackSize = config.getInt("max_stack", 99);
            swingDist = (float)config.getDouble("swing_distance", 0.785f);
            swingStyle = (SwingStyle)Enum.Parse(typeof(SwingStyle), config.getString("", "Slash"));
            hitbox = new Rectangle(0, 0, config.getInt("hitbox.width", 24), config.getInt("hitbox.height", 24));
            damage = config.getDouble("damage", 1);

            // These take a little more processing to validate...
            string imgPath = config.getString("texture");
            Texture2D texture;
            if (!imgPath.Equals(""))
                texture = Util.loadTexture($"{pack.packPath}\\textures\\{imgPath}");
            else
                texture = Util.loadTexture($"{pack.packPath}\\textures\\missing.png");
            graphic = new Graphic(texture);


        }
    }
}
