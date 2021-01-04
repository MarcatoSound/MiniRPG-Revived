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
        public static GuiPlayerInventory playerInv;

        static GuiWindowManager()
        {
            windows = new List<GuiWindow>();
        }

        public static void add(GuiWindow window)
        {
            windows.Add(window);
            if (window is GuiPlayerInventory) playerInv = (GuiPlayerInventory)window;
        }

        public static GuiWindow get(int i)
        {
            if (i > windows.Count - 1) return null;
            return windows[i];
        }
        public static GuiWindow getByName(string name)
        {
            foreach (GuiWindow window in windows)
            {
                if (window.name == name) return window;
            }

            return null;
        }

        public static List<GuiWindow> getWindows()
        {
            return windows;
        }

        public static void openWindow(string name)
        {
            foreach (GuiWindow window in windows)
            {
                if (window.name == name)
                {
                    openWindow(window);
                    break;
                }
            }
        }
        public static void openWindow(int i)
        {
            activeWindow = windows[i];
            Core.paused = true;
        }
        public static void openWindow(GuiWindow window)
        {
            activeWindow = window;
            Core.paused = true;
        }
        public static void closeWindow()
        {
            activeWindow = null;
            Core.paused = false;
        }

        public static void Clear()
        {
            windows.Clear();
        }
    }
}
