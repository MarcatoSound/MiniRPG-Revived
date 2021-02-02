using BasicRPGTest_Mono.Engine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Items
{
    public class Item
    {
        public ParentItem parent;

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

        public int quantity { get; set; }

        public Item(ParentItem parentItem, int quantity = 1)
        {
            parent = parentItem;

            id = parent.id;
            name = parent.name;
            displayName = parent.displayName;
            graphic = parent.graphic;
            description = parent.description;
            maxStackSize = parent.maxStackSize;

            swingDist = parent.swingDist;
            swingStyle = parent.swingStyle;
            hitbox = parent.hitbox;
            damage = parent.damage;

            this.quantity = quantity;

        }
    }
}
