using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public class World
    {
        public World(string name)
        {

        }

        public void save()
        {

        }
        public void load()
        {



            Core.activeWorld = this;
        }
    }
}
