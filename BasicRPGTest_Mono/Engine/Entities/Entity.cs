using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RPGEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public class Entity
    {
        public string name { get; set; }
        public int id { get; set; }
        public int instanceId { get; set; }
        private Graphic v_Graphic { get; set; }
        public Rectangle boundingBox { get; set; }
        public Vector2 Position { get; set; }

        private Vector2 v_CenteredPosition = new Vector2();

        private Rectangle v_HitBox;
        public int HitBoxSize { get; set; } = 24;
        public bool DrawBoundingBox { get; set; } = true;
        public bool DrawCenterBox { get; set; } = true;
        public bool DrawHitBox { get; set; } = false;


        //====================================================================================
        // PROPERTIES
        //====================================================================================

        public Vector2 CenteredPosition
        {
            get 
            {
                v_CenteredPosition.X = Position.X + (graphic.texture.Width / 2);
                v_CenteredPosition.Y = Position.Y + (graphic.texture.Height / 2);
                
                return v_CenteredPosition;
            }
        }


        public Rectangle HitBox
        {
            get
            {
                v_HitBox.X = (int)CenteredPosition.X - HitBoxSize;
                v_HitBox.Y = (int)CenteredPosition.Y - HitBoxSize;
                v_HitBox.Width = HitBoxSize;
                v_HitBox.Height = HitBoxSize;

                return v_HitBox;
            }
        }


        public Graphic graphic
        {
            get { return v_Graphic; }
            set { v_Graphic = value; }
        }


        private Color _tintColor = Color.White;
        public Color tintColor
        {
            get { return _tintColor; }
            set
            {
                _tintColor = value;
            }
        }

        public GraphicsDeviceManager graphicsManager { get; set; }


        //====================================================================================
        // CONSTRUCTORS
        //====================================================================================

        public Entity(Graphic graphic, Rectangle box, GraphicsDeviceManager graphicsDevice)
        {
            this.graphic = graphic;
            boundingBox = box;
            if (GetType() == typeof(Entity)) id = EntityManager.entities.Count;
            graphicsManager = graphicsDevice;


            //EntityManager.add(this);
        }


        public bool isColliding(Rectangle box)
        {
            if (MapManager.activeMap == null) return true;
            ConcurrentDictionary<int, Rectangle> pairs = MapManager.activeMap.collidables;
            foreach (KeyValuePair<int, Rectangle> pair in pairs)
            {
                if (box.Intersects(pair.Value))
                    return true;
            }


            return false;
        }



        //====================================================================================
        // FUNCTIONS
        //====================================================================================

        public virtual Rectangle getBox(Vector2 pos)
        {
            int x = (int)(pos.X - (boundingBox.Width - (boundingBox.Width / 2)));
            int y = (int)(pos.Y - (boundingBox.Height - (v_Graphic.texture.Height / 2)));

            Rectangle box = new Rectangle(x, y, boundingBox.Width, boundingBox.Height);

            return box;
        }


        public bool MoveTo(Vector2 mPosition)
        {

            // If CANNOT move to sent Position
            
            // Otherwise...
            // Move Entity position



            return true;
        }


        //====================================================================================
        // FUNCTIONS - Update & Draw
        //====================================================================================

        public virtual void update()
        {
        }

        public virtual void draw(SpriteBatch batch)
        {
            if (!Camera.camera.BoundingRectangle.Intersects(boundingBox)) return;
            // Otherwise...

            // Draw THIS Entity's texture
            graphic.draw(batch, Position, tintColor);

            // Draw helper Boxes
            batch.Begin(transformMatrix: Camera.camera.Transform);

            // Draw Entity Center point
            if (DrawCenterBox) { batch.DrawRectangle(new Rectangle((int)CenteredPosition.X, (int)CenteredPosition.Y, 5, 5), Color.AliceBlue); }

            // If Draw Bounding Box = TRUE. Draw it.
            if (DrawBoundingBox) { batch.DrawRectangle(boundingBox, Microsoft.Xna.Framework.Color.White); }

            // If Draw Hit Box = TRUE. Draw it.
            if (DrawHitBox) { batch.DrawRectangle(HitBox, Microsoft.Xna.Framework.Color.PaleVioletRed); }

            batch.End();
        }

    }


    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
}
