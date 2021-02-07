using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI.HUD
{
    public static class HudManager
    {
        private static List<HudElement> elements;
        private static Dictionary<string, HudElement> elementsByName;

        public static Texture2D tileset;

        static HudManager()
        {
            elements = new List<HudElement>();
            elementsByName = new Dictionary<string, HudElement>();
        }

        public static void add(HudElement element)
        {
            elements.Add(element);
            elementsByName.Add(element.name, element);
        }

        public static HudElement get(int i)
        {
            if (i > elements.Count - 1) return null;
            return elements[i];
        }
        public static HudElement getByName(string name)
        {
            if (name != null && elementsByName.ContainsKey(name))
                return elementsByName[name];

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
            foreach (HudElement element in elements)
            {
                element.Draw(batch);
            }
        }

        public static void Clear()
        {
            elements.Clear();
        }
    }
}
