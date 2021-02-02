using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public static class Core
    {
        public static Main game;
        public static GameTime globalTime;
        public static GraphicsDevice graphics;

        public static ContentManager content;
        public static SpriteFont mainFont;
        public static SpriteFont dmgFont;
        public static SpriteFont critFont;

        public static Player player;

        public static bool paused;

        public static List<PopupText> popupTexts;
        public static Dictionary<Vector2, ItemEntity> items;

        static Core()
        {
            popupTexts = new List<PopupText>();
            items = new Dictionary<Vector2, ItemEntity>();
        }
    }
}
