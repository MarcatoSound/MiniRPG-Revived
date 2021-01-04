using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI.HUD
{
    public abstract class HudElement
    {
        public string name;
        public Vector2 screenPos;

        public HudElement(string name, Vector2 screenPos)
        {
            this.name = name;
            this.screenPos = screenPos;
        }


        public abstract void Update();
        public abstract void Draw(SpriteBatch batch);

    }
}
