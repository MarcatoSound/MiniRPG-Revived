using BasicRPGTest_Mono.Engine.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine.Entities
{
    public class ItemSwing
    {
        public Timer swingTimer;
        public Direction direction;
        public Player player;
        public Item item;
        public Graphic graphic;

        public SwingStyle style;
        public Vector2 swingPos;
        public float swingDist;

        public int stabOffset;
        public int stabFrame;
        public float rotation;
        public int rotationFrame;

        public Rectangle hitBox;
        public Rectangle itemBox;

        public ItemSwing(Direction direction, int swingTime, Player player, Item item, Vector2 position, SwingStyle style = SwingStyle.Slash, float swingDist = 1.57f)
        {
            this.player = player;
            this.direction = direction;
            this.item = item;
            graphic = item.graphic;
            itemBox = item.hitbox;
            itemBox.X = itemBox.Width / 2;
            itemBox.Y = itemBox.Height / 2;

            this.style = style;
            this.swingDist = swingDist;
            swingTimer = new Timer(swingTime / 7);
            swingTimer.Elapsed += Update;
            swingTimer.Start();

            swingPos = new Vector2(position.X, position.Y);
            int offset = itemBox.Height - itemBox.Y - 16;
            if (offset < 0) offset = 0;
            if (style == SwingStyle.Slash)
            {
                switch (direction)
                {
                    case Direction.Up:
                        swingPos.Y -= 36;
                        hitBox = new Rectangle((int)swingPos.X - itemBox.X, (int)swingPos.Y - itemBox.Y - offset, itemBox.Width, itemBox.Height);
                        break;
                    case Direction.Left:
                        swingPos.X -= 28;
                        hitBox = new Rectangle((int)swingPos.X - itemBox.Y - offset, (int)swingPos.Y - itemBox.X, itemBox.Width - (itemBox.Width - itemBox.Height), itemBox.Height + (itemBox.Width - itemBox.Height));
                        break;
                    case Direction.Down:
                        swingPos.Y += 36;
                        hitBox = new Rectangle((int)swingPos.X - itemBox.X, (int)swingPos.Y - itemBox.Y + offset, itemBox.Width, itemBox.Height);
                        break;
                    case Direction.Right:
                        swingPos.X += 28;
                        hitBox = new Rectangle((int)swingPos.X - itemBox.Y + offset, (int)swingPos.Y - itemBox.X, itemBox.Width - (itemBox.Width - itemBox.Height), itemBox.Height + (itemBox.Width - itemBox.Height));
                        break;
                }
            }
        }

        public void Update(Object source, ElapsedEventArgs args)
        {
            if (style == SwingStyle.Slash)
            {
                if (rotationFrame == 6)
                {
                    Stop();
                    return;
                }
                rotationFrame++;
                rotation += swingDist / 7;
            }
            if (style == SwingStyle.Stab)
            {
                if (stabFrame == 6)
                {
                    Stop();
                    return;
                }
                stabFrame++;
                if (stabFrame <= 2)
                    stabOffset += 4;
                else
                    stabOffset -= 4;
            }
        }
        public void Draw(SpriteBatch batch, Vector2 position)
        {
            swingPos = new Vector2(position.X, position.Y);

            int offset = itemBox.Height - itemBox.Y - 16;
            if (offset < 0) offset = 0;

            float angle = 0;
            Vector2 origin = new Vector2(-(graphic.texture.Width / (graphic.texture.Width / 16)), graphic.texture.Height + (graphic.texture.Height / (graphic.texture.Height / 16)));
            int modx = 0;
            int mody = 0;

            if (style == SwingStyle.Slash)
            {
                switch (direction)
                {
                    case Direction.Up:
                        angle = -1.57f + swingDist + ((1.57f - swingDist) / 2);
                        swingPos.Y -= 36;
                        //modx = -(graphic.texture.Width / 4);
                        mody = 40;
                        hitBox = new Rectangle((int)swingPos.X - itemBox.X, (int)swingPos.Y - itemBox.Y - offset, itemBox.Width, itemBox.Height);
                        break;
                    case Direction.Left:
                        angle = -3.14f + swingDist + ((1.57f - swingDist) / 2);
                        swingPos.X -= 28;
                        modx = 40;
                        //mody = graphic.texture.Width / 4;
                        hitBox = new Rectangle((int)swingPos.X - itemBox.Y - offset, (int)swingPos.Y - itemBox.X, itemBox.Width - (itemBox.Width - itemBox.Height), itemBox.Height + (itemBox.Width - itemBox.Height));
                        break;
                    case Direction.Down:
                        angle = -4.71f + swingDist + ((1.57f - swingDist) / 2);
                        swingPos.Y += 36;
                        //modx = graphic.texture.Width / 4;
                        mody = -40;
                        hitBox = new Rectangle((int)swingPos.X - itemBox.X, (int)swingPos.Y - itemBox.Y + offset, itemBox.Width, itemBox.Height);
                        break;
                    case Direction.Right:
                        angle = -6.28f + swingDist + ((1.57f - swingDist) / 2);
                        swingPos.X += 28;
                        modx = -40;
                        //mody = -(graphic.texture.Width / 4);
                        hitBox = new Rectangle((int)swingPos.X - itemBox.Y + offset, (int)swingPos.Y - itemBox.X, itemBox.Width - (itemBox.Width - itemBox.Height), itemBox.Height + (itemBox.Width - itemBox.Height));
                        break;
                }
                batch.Begin(transformMatrix: Camera.camera.Transform);
                batch.DrawRectangle(hitBox, Color.White);
                batch.End();

                angle -= rotation;

                graphic.draw(batch, new Vector2(swingPos.X + modx, swingPos.Y + mody), angle, origin);
            }

            if (style == SwingStyle.Stab)
            {
                switch (direction)
                {
                    case Direction.Up:
                        angle = -0.785f;
                        swingPos.Y -= 28;
                        //modx = -(graphic.texture.Width / 4);
                        mody = 40 - stabOffset;
                        hitBox = new Rectangle((int)swingPos.X - itemBox.X, (int)swingPos.Y - itemBox.Y - offset, itemBox.Width, itemBox.Height);
                        break;
                    case Direction.Left:
                        angle = -2.36f;
                        swingPos.X -= 28;
                        modx = 40 - stabOffset;
                        //mody = graphic.texture.Width / 4;
                        hitBox = new Rectangle((int)swingPos.X - itemBox.Y - offset, (int)swingPos.Y - itemBox.X, itemBox.Width - (itemBox.Width - itemBox.Height), itemBox.Height + (itemBox.Width - itemBox.Height));
                        break;
                    case Direction.Down:
                        angle = -3.93f;
                        swingPos.Y += 28;
                        //modx = graphic.texture.Width / 4;
                        mody = -40 + stabOffset;
                        hitBox = new Rectangle((int)swingPos.X - itemBox.X, (int)swingPos.Y - itemBox.Y + offset, itemBox.Width, itemBox.Height);
                        break;
                    case Direction.Right:
                        angle = -5.5f;
                        swingPos.X += 28;
                        modx = -40 + stabOffset;
                        //mody = -(graphic.texture.Width / 4);
                        hitBox = new Rectangle((int)swingPos.X - itemBox.Y + offset, (int)swingPos.Y - itemBox.X, itemBox.Width - (itemBox.Width - itemBox.Height), itemBox.Height + (itemBox.Width - itemBox.Height));
                        break;
                }
                batch.Begin(transformMatrix: Camera.camera.Transform);
                batch.DrawRectangle(hitBox, Color.White);
                batch.End();

                graphic.draw(batch, new Vector2(swingPos.X + modx, swingPos.Y + mody), angle, origin);
            }
        }
        public void Stop()
        {
            swingTimer.Stop();
            player.itemSwing = null;
        }
    }


    public enum SwingStyle
    {
        Slash,
        Stab
    }
}
