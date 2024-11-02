using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using ProtoBuf;
using RPGEngine;

namespace BasicRPGTest_Mono.Engine.Data
{
    [ProtoContract(SkipConstructor = true)]
    public class Vector2i
    {
        [ProtoMember(1)]
        public int X { get; set; }
        [ProtoMember(2)]
        public int Y { get; set; }

        public Vector2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2(Vector2i vec)
        {
            return new Vector2(vec.X, vec.Y);
        }
        public static implicit operator Vector2i(Vector2 vec)
        {
            return new Vector2i((int)vec.X, (int)vec.Y);
        }

        public override bool Equals(object other)
        {
            var otherVec = other as Vector2i;
            return X == otherVec.X && Y == otherVec.Y;
        }
        public override int GetHashCode()
        {
            return 0;
        }
    }
}
