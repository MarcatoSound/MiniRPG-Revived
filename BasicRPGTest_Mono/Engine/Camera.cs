using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public static class Camera
    {
        public static OrthographicCamera camera;

        private static Vector2 _camPos;
        public static Vector2 camPos
        {
            get { return _camPos; }
            set
            {
                _camPos = value;
                camera.Position = _camPos;
            }
        }

        static Camera()
        {
        }

        public static void Update()
        {
        }

        public static void reset()
        {
            _camPos = new Vector2();
            camera.Position = _camPos;
        }
    }
}
