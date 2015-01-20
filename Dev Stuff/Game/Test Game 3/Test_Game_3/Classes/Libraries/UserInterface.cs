using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using C3.XNA;
namespace UserInterface {
    class Button {
        List<Texture2D> btnTexture;
        public int currentTexture = 0;
        public int width;
        public int height;
        public Vector2 pos;
        private Vector2 posOffset;
        public bool isDown;
        private int isDownCt;
        private int isDownCtFrames;
        public Boolean visible;
        private Rectangle bounds;
        public string text;
        private SpriteFont font;
        private float scale;
        public Rectangle mouseRect;
        public string methodVar;
        public Func<string, Boolean> pressMethod;
        public Button(List<Texture2D> Texture, Vector2 Position, SpriteFont ifont, string itext, Func<string, Boolean> PressMethod, string MethodVar = "", Boolean isVisible = true) {
            btnTexture = Texture;
            pos = Position;
            width = Texture[0].Width;
            height = Texture[0].Height;
            isDown = false;
            visible = isVisible;
            bounds = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            font = ifont;
            text = itext;
            scale = 1f;
            methodVar = MethodVar;
            pressMethod = PressMethod;
            isDownCtFrames = 15;
        }
        public Boolean Update(MouseState mState, Vector2 relMousePt, Vector2 camPos, float camZoom) {
            
            posOffset = pos / scale + camPos;
            scale = camZoom;
            bounds = new Rectangle((int)posOffset.X , (int)posOffset.Y, (int)(width/scale), (int)(height/scale));
            checkMouse(mState, relMousePt/scale);
            if (isDown && isDownCt <= 0) {
                pressMethod(methodVar);
                isDownCt = isDownCtFrames;
            }
            else {
                if (isDownCt>0) isDownCt--;
            }
            if (currentTexture != 0) {
                return true;
            }
            return false;
        }

        private void checkMouse(MouseState mState, Vector2 relMousePt) {
            mouseRect = new Rectangle((int)(relMousePt.X*scale), (int)(relMousePt.Y*scale), 1, 1);
            if (mouseRect.Intersects(bounds)) {
                if (mState.LeftButton == ButtonState.Pressed) {
                    currentTexture = 2;
                    isDown = true;
                }
                else {
                    isDown = false;
                    currentTexture = 1;
                }
            }
            else {
                isDown = false;
                currentTexture = 0;
            }
        }
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(btnTexture[currentTexture], new Vector2(bounds.X, bounds.Y), new Rectangle(0, 0, btnTexture[currentTexture].Width, btnTexture[currentTexture].Height), Color.White, 0f, new Vector2(0, 0), 1 / scale, SpriteEffects.None, 1);
            Vector2 textSize = font.MeasureString(text);
            float textScale = (textSize.X/bounds.Width)+0.2f;

            spriteBatch.DrawString(font, text, new Vector2(bounds.X +bounds.Width / 2 - textSize.X / 2 / textScale, bounds.Y + bounds.Height / 2 - textSize.Y / 2 / textScale), Color.Red, 0f, new Vector2(0, 0), 1 / textScale, SpriteEffects.None, 0);

        }
    }
}
