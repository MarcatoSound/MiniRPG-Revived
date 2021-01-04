using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI.HUD
{
    public static class HudManager
    {
        public static List<HudElement> elements;
        public static Texture2D tileset;

        static HudManager()
        {
            elements = new List<HudElement>();
        }

        public static void add(HudElement element)
        {
            elements.Add(element);
        }

        public static HudElement get(int i)
        {
            if (i > elements.Count - 1) return null;
            return elements[i];
        }
        public static HudElement getByName(string name)
        {
            foreach (HudElement element in elements)
            {
                if (element.name == name) return element;
            }

            return null;
        }

        public static List<HudElement> getWindows()
        {
            return elements;
        }


        public static void Update()
        {
            foreach (HudElement element in elements)
            {
                element.Update();
            }
        }
        public static void Draw(SpriteBatch batch)
        {
            batch.Begin();
            foreach (HudElement element in elements)
            {
                element.Draw(batch);
            }
            batch.End();
        }
    }
}
