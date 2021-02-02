using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Items
{
    public class Tool : Item
    {
        public Dictionary<DamageType, double> damageTypes;
        public Tool(ParentTool parentTool, int quantity = 1) : base(parentTool, quantity)
        {
            damageTypes = new Dictionary<DamageType, double>(parentTool.damageTypes);
        }

    }
}
