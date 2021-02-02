using BasicRPGTest_Mono.Engine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

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
            name = Utility.Util.cleanString(displayName).ToLower();
            this.displayName = displayName;
            this.graphic = graphic;
            this.maxStackSize = maxStack;


            if (GetType() == typeof(ParentItem)) swingDist = 0.785f;
            if (GetType() == typeof(ParentItem)) swingStyle = SwingStyle.Slash;
            if (GetType() == typeof(ParentItem)) hitbox = new Rectangle(0, 0, 24, 24);
            if (GetType() == typeof(ParentItem)) damage = 1;
        }
    }
}
