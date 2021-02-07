using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    static class FontLibrary
    {

        private static Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();

        public static SpriteFont getFont(string name)
        {
            if (name != null && fonts.ContainsKey(name))
                return fonts[name];

            return null;
        }
        public static void addFont(string name, SpriteFont font)
        {
            if (name == null) return;
            if (fonts.ContainsKey(name))
                fonts.Remove(name);

            fonts.Add(name, font);
        }


    }
}
