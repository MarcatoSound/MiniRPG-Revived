using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Entities
{
    public class Spawn
    {
        public LivingEntity entity;
        public double weight;
        public Spawn(LivingEntity entity, double weight)
        {
            this.entity = entity;
            this.weight = weight;
        }
    }
}
