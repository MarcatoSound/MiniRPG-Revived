using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Graphics
{
    public class GraphicSet : Graphic
    {

        private GraphicAnimated _active;
        public GraphicAnimated active
        {
            get { return _active; }
            set
            {
                _active = value;
                texture = value.texture;
            }
        }

        public GraphicAnimated idleUp;
        public GraphicAnimated idleDown;
        public GraphicAnimated idleLeft;
        public GraphicAnimated idleRight;

        public GraphicAnimated moveUp;
        public GraphicAnimated moveDown;
        public GraphicAnimated moveLeft;
        public GraphicAnimated moveRight;

        public GraphicAnimated attackUp;
        public GraphicAnimated attackDown;
        public GraphicAnimated attackLeft;
        public GraphicAnimated attackRight;

        public GraphicSet(Texture2D spriteSet)
        {
            idleUp = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 0, 256, 64)), 1, 4, 3);
            idleDown = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 64, 256, 64)), 1, 4, 3);
            idleLeft = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 128, 256, 64)), 1, 4, 3);
            idleRight = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 192, 256, 64)), 1, 4, 3);

            moveUp = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 256, 256, 64)), 1, 4, 5);
            moveDown = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 320, 256, 64)), 1, 4, 5);
            moveLeft = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 384, 256, 64)), 1, 4, 5);
            moveRight = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 448, 256, 64)), 1, 4, 5);

            attackUp = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 512, 256, 64)), 1, 4, 9);
            attackDown = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 576, 256, 64)), 1, 4, 9);
            attackLeft = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 640, 256, 64)), 1, 4, 9);
            attackRight = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, new Rectangle(0, 704, 256, 64)), 1, 4, 9);

            active = idleDown;

        }

        public void setSprite(GraphicType type, Direction direction)
        {
            switch (type)
            {
                case GraphicType.Idle:
                    switch (direction)
                    {
                        case Direction.Up:
                            active = idleUp;
                            break;
                        case Direction.Down:
                            active = idleDown;
                            break;
                        case Direction.Left:
                            active = idleLeft;
                            break;
                        case Direction.Right:
                            active = idleRight;
                            break;
                    }
                    break;
                case GraphicType.Move:
                    switch (direction)
                    {
                        case Direction.Up:
                            active = moveUp;
                            break;
                        case Direction.Down:
                            active = moveDown;
                            break;
                        case Direction.Left:
                            active = moveLeft;
                            break;
                        case Direction.Right:
                            active = moveRight;
                            break;
                    }
                    break;
                case GraphicType.Attack:
                    switch (direction)
                    {
                        case Direction.Up:
                            active = attackUp;
                            break;
                        case Direction.Down:
                            active = attackDown;
                            break;
                        case Direction.Left:
                            active = attackLeft;
                            break;
                        case Direction.Right:
                            active = attackRight;
                            break;
                    }
                    break;
            }
        }

    }


    public enum GraphicType
    {
        Idle,
        Move,
        Attack
    }
}
