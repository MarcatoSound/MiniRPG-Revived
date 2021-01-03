﻿using Microsoft.Xna.Framework;
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

        public static Player player;

        public static bool paused;
    }
}
