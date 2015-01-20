using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cameras {
    public class Camera2D {
         protected float zoom; // Camera Zoom
        public Matrix transform; // Matrix Transform
        public Vector2 pos; // Camera Position
        protected float rotation; // Camera Rotation
        public int width;
        public int height;
        public Vector2 origPos;
        public Rectangle bounds;
 
        public Camera2D()
        {
            zoom = 1.0f;
            rotation = 0.0f;
            pos = Vector2.Zero;
        }
        // Sets and gets zoom
        public float Zoom {
            get { return zoom; }
            set { zoom = value; if (zoom < 0.1f) zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation {
            get { return rotation; }
            set { rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount) {
            pos += amount;
        }
        // Get set position
        public Vector2 Pos {
            get { return pos; }
            set { pos = value; }
        }
        //Perform Transformations
        public Rectangle Bounds {
            get { return new Rectangle((int)origPos.X, (int)origPos.Y, width, height); }
        }
        public Matrix getTransformation(GraphicsDevice graphicsDevice) {
            width = (int)(graphicsDevice.Viewport.Width / zoom);
            height = (int)(graphicsDevice.Viewport.Height / zoom);
            transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));

            origPos.X = pos.X - width / 2;
            origPos.Y = pos.Y - height / 2;
            
            return transform;
        }
    }
}
