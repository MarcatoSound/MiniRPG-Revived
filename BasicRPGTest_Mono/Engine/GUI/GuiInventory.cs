using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI
{
    public class GuiInventory : GuiWindow
    {
        public GuiInventory() : 
            base(new Microsoft.Xna.Framework.Rectangle(0, 0, 512, 128), GuiWindowManager.tileset)
        {

        }
    }
}
