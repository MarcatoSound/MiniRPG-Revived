using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Entities
{
    public class Spawn
    {
        public LivingEntity entity;
        public int weight;
        public Spawn(LivingEntity entity, int weight)
        {
            this.entity = entity;
            this.weight = weight;
        }
    }
}
