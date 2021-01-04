using Microsoft.Xna.Framework.Graphics;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Graphics
{
    public class GraphicSet : Graphic
    {

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
            idleUp = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 0, 0, 64), 1, 1);
            idleDown = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 1, 0, 64), 1, 1);
            idleLeft = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 2, 0, 64), 1, 1);
            idleRight = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 3, 0, 64), 1, 1);

            moveUp = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 0, 0, 64), 1, 1);
            moveDown = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 1, 0, 64), 1, 1);
            moveLeft = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 2, 0, 64), 1, 1);
            moveRight = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 3, 0, 64), 1, 1);

            attackUp = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 0, 0, 64), 1, 1);
            attackDown = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 1, 0, 64), 1, 1);
            attackLeft = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 2, 0, 64), 1, 1);
            attackRight = new GraphicAnimated(Utility.Util.getSpriteFromSet(spriteSet, 3, 0, 64), 1, 1);

            texture = idleDown.texture;

        }

        public void setSprite(GraphicType type, Direction direction)
        {
            switch (type)
            {
                case GraphicType.Idle:
                    switch (direction)
                    {
                        case Direction.Up:
                            texture = idleUp.texture;
                            break;
                        case Direction.Down:
                            texture = idleDown.texture;
                            break;
                        case Direction.Left:
                            texture = idleLeft.texture;
                            break;
                        case Direction.Right:
                            texture = idleRight.texture;
                            break;
                    }
                    break;
                case GraphicType.Move:
                    switch (direction)
                    {
                        case Direction.Up:
                            texture = moveUp.texture;
                            break;
                        case Direction.Down:
                            texture = moveDown.texture;
                            break;
                        case Direction.Left:
                            texture = moveLeft.texture;
                            break;
                        case Direction.Right:
                            texture = moveRight.texture;
                            break;
                    }
                    break;
                case GraphicType.Attack:
                    switch (direction)
                    {
                        case Direction.Up:
                            texture = attackUp.texture;
                            break;
                        case Direction.Down:
                            texture = attackDown.texture;
                            break;
                        case Direction.Left:
                            texture = attackLeft.texture;
                            break;
                        case Direction.Right:
                            texture = attackRight.texture;
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
