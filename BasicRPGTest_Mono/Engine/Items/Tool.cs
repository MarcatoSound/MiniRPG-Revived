using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Items
{
    public class Tool : Item
    {
        public Dictionary<DamageType, double> damageTypes;
        public Tool(string displayName, Texture2D texture, Rectangle hitbox, double damage) : base(displayName, texture)
        {
            if (hitbox == null) this.hitbox = new Rectangle(0, 0, 24, 24);

            this.damage = damage;
            damageTypes = new Dictionary<DamageType, double>();
        }

    }


    public enum DamageType
    {
        Slashing,
        Piercing,
        Bashing,
        Mining,
        Chopping,
        Digging
    }
}
