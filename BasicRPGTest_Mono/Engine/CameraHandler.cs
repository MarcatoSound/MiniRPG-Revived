using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    // INCREDIBLY powerful camera handler provided by Khalid Abuhakmeh from Stackoverflow!

    public interface IFocusable
    {
        Vector2 Position { get; }
    }

    public interface ICamera2D
    {
        /// <summary>
        /// Gets or sets the position of the camera
        /// </summary>
        /// <value>The position.</value>
        Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the move speed of the camera.
        /// The camera will tween to its destination.
        /// </summary>
        /// <value>The move speed.</value>
        float MoveSpeed { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the camera.
        /// </summary>
        /// <value>The rotation.</value>
        float Rotation { get; set; }

        /// <summary>
        /// Gets the origin of the viewport (accounts for Scale)
        /// </summary>        
        /// <value>The origin.</value>
        Vector2 Origin { get; }

        /// <summary>
        /// Gets or sets the scale of the Camera
        /// </summary>
        /// <value>The scale.</value>
        float Scale { get; set; }

        /// <summary>
        /// Gets the screen center (does not account for Scale)
        /// </summary>
        /// <value>The screen center.</value>
        Vector2 ScreenCenter { get; }

        /// <summary>
        /// Gets the transform that can be applied to 
        /// the SpriteBatch Class.
        /// </summary>
        /// <see cref="SpriteBatch"/>
        /// <value>The transform.</value>
        Matrix Transform { get; }

        /// <summary>
        /// Gets or sets the focus of the Camera.
        /// </summary>
        /// <seealso cref="IFocusable"/>
        /// <value>The focus.</value>
        IFocusable Focus { get; set; }

        /// <summary>
        /// Determines whether the target is in view given the specified position.
        /// This can be used to increase performance by not drawing objects
        /// directly in the viewport
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///     <c>true</c> if the target is in view at the specified position; otherwise, <c>false</c>.
        /// </returns>
        bool IsInView(Vector2 position, Texture2D texture);
    }

    public class Camera2D : GameComponent, ICamera2D
    {
        private Vector2 _position;
        protected float _viewportHeight;
        protected float _viewportWidth;
        private float _scale;
        private float _oldScale;

        public Camera2D(Game game)
            : base(game)
        { }

        #region Properties

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public float Scale
        {
            get { return _scale; }
            set
            {
                _oldScale = _scale;

                if (value > maxZoom)
                {
                    _scale = maxZoom;
                    return;
                }
                if (value < minZoom)
                {
                    _scale = minZoom;
                    return;
                }
                _scale = value;
            }
        }
        public float maxZoom { get; set; }
        public float minZoom { get; set; }
        public Vector2 ScreenCenter { get; protected set; }
        public Matrix Transform { get; set; }
        public IFocusable Focus { get; set; }
        public float MoveSpeed { get; set; }
        public Rectangle BoundingRectangle { get; set; }
        public Rectangle CameraLimits { get; set; }

        #endregion

        /// <summary>
        /// Called when the GameComponent needs to be initialized. 
        /// </summary>
        public override void Initialize()
        {
            _viewportWidth = Game.GraphicsDevice.Viewport.Width;
            _viewportHeight = Game.GraphicsDevice.Viewport.Height;
            BoundingRectangle = new Rectangle(Convert.ToInt32(Position.X), Convert.ToInt32(Position.Y), Convert.ToInt32(_viewportWidth), Convert.ToInt32(_viewportHeight));

            ScreenCenter = new Vector2(_viewportWidth / 2, _viewportHeight / 2);
            maxZoom = 1.5f;
            minZoom = 0.8f;
            //minZoom = 0.3f;
            Scale = 1.25f;
            _oldScale = 1.25f;
            MoveSpeed = 5f;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Move the Camera to the position that it needs to go
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float newScale = Scale;
            newScale -= (Scale - _oldScale) * 5 * (delta * 10);
            _oldScale = newScale;

            Origin = ScreenCenter / newScale;

            // Create the Transform used by any
            // spritebatch process
            Transform = Matrix.Identity *
                        Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
                        Matrix.CreateScale(new Vector3(newScale, newScale, newScale));



            Vector2 newPos = new Vector2(_position.X, _position.Y);
            newPos.X += (Focus.Position.X - Position.X) * MoveSpeed * delta;
            newPos.Y += (Focus.Position.Y - Position.Y) * MoveSpeed * delta;
            if (newPos.X + (_viewportWidth / 2 / newScale) > CameraLimits.Width || newPos.X - (_viewportWidth / 2 / newScale) < 0) 
                newPos.X = _position.X;
            if (newPos.Y + (_viewportHeight / 2 / newScale) > CameraLimits.Height || newPos.Y - (_viewportHeight / 2 / newScale) < 0)
                newPos.Y = _position.Y;

            _position.X = newPos.X;
            _position.Y = newPos.Y;

            BoundingRectangle = new Rectangle(Convert.ToInt32(_position.X - (_viewportWidth / 2 / newScale)), Convert.ToInt32(_position.Y - (_viewportHeight / 2 / newScale)), Convert.ToInt32(_viewportWidth / newScale), Convert.ToInt32(_viewportHeight / newScale));

            base.Update(gameTime);
        }

        /// <summary>
        /// Determines whether the target is in view given the specified position.
        /// This can be used to increase performance by not drawing objects
        /// directly in the viewport
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///     <c>true</c> if [is in view] [the specified position]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInView(Vector2 position, Texture2D texture)
        {
            // If the object is not within the horizontal bounds of the screen

            if ((position.X + texture.Width) < (Position.X - Origin.X) || (position.X) > (Position.X + Origin.X))
                return false;

            // If the object is not within the vertical bounds of the screen
            if ((position.Y + texture.Height) < (Position.Y - Origin.Y) || (position.Y) > (Position.Y + Origin.Y))
                return false;

            // In View
            return true;
        }
    }
}
