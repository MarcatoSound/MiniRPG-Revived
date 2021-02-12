using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    public class RegionManager
    {
        public Map map { get; private set; }

        public Dictionary<Vector2, Region> regions { get; private set; } = new Dictionary<Vector2, Region>();
        public Dictionary<Vector2, Region> changedRegions { get; private set; } = new Dictionary<Vector2, Region>();

        public RegionManager(Map map)
        {
            this.map = map;
        }

        public void updateRegion(Region region)
        {
            if (changedRegions.ContainsKey(region.regionPos))
                return;

            changedRegions.Add(region.regionPos, region);
        }
    }
}
