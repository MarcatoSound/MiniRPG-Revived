using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI
{
    public static class GuiWindowManager
    {
        public static List<GuiWindow> windows;
        public static Texture2D tileset;
        public static GuiWindow activeWindow;

        static GuiWindowManager()
        {
            windows = new List<GuiWindow>();
        }

        public static void add(GuiWindow window)
        {
            windows.Add(window);
        }

        public static GuiWindow get(int i)
        {
            if (i > windows.Count - 1) return null;
            return windows[i];
        }

        public static List<GuiWindow> getTiles()
        {
            return windows;
        }

        public static void openWindow(int i)
        {
            activeWindow = windows[i];
            Core.player.paused = true;
        }
        public static void openWindow(GuiWindow window)
        {
            activeWindow = window;
            Core.player.paused = true;
        }
        public static void closeWindow()
        {
            activeWindow = null;
            Core.player.paused = false;
        }

        public static void Clear()
        {
            windows.Clear();
        }
    }
}
