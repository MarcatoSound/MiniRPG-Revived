using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Menus
{
    public class MenuItem
    {
        public string label { get; set; }
        public virtual Action run { get; set; }

        public MenuItem(string label)
        {
            this.label = label;
            run = () =>
            {

            };
        }

    }
}
