using BasicRPGTest_Mono.Engine.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Inventories
{
    public class PlayerInventory : Inventory
    {
        public Item mainhand;
        public Item offhand;
        public Hotbar hotbarPrimary;
        public Hotbar hotbarSecondary;
    }
}
