using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using C3.XNA;
using UserInterface;
using Cameras;

namespace Test_Game_3 {

    /// <summary>
    /// This is the main type for your game
    /// </summary>

    public class Game1 : Microsoft.Xna.Framework.Game {

        //Camera
        Camera2D cam = new Camera2D();
        Vector2 cameraPos;
        int cameraEaseFrames = 10;
        int cameraSpeed = 20;
        float cameraScale = 1;
        //Graphics      
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont font;
        List<Texture2D> shipTexList = new List<Texture2D>();
        List<Texture2D> btnTexture1 = new List<Texture2D>();
        List<Button> buttonList = new List<Button>();
        Boolean drawGrid = false;
        //Input
        MouseState mouseState = Mouse.GetState();
        public Vector2 relMousePt;
        KeyboardState keyboardState = Keyboard.GetState();
        int prevMouseScrollValue = Mouse.GetState().ScrollWheelValue;
        Boolean selectMode = false;
        Vector2 selectBoxOrig;
        Vector2 selectBoxEnd;
        //Objects
        //Ships
        List<Ship> ShipList = new List<Ship>();
        Fleet Fleet1 = new Fleet();
        TaskGroup TG1 = new TaskGroup();
        TaskGroup TG2 = new TaskGroup();
        Fleet SelectedFleet;


        //Misc
        private FrameCounter frameCounter = new FrameCounter();
        public Game1() {
            cam.Pos = new Vector2(0, 0);
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            SelectedFleet = Fleet1;



        }
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            //<UI>

            //</UI>
            //<Ships>
            // Add Ships

            //</Ships>
            base.Initialize();
        }
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //<GUI>
            //fonts
            font = Content.Load<SpriteFont>("Arial");
            //buttons
            btnTexture1.Add(Content.Load<Texture2D>("GUI/Buttons/Button1"));
            btnTexture1.Add(Content.Load<Texture2D>("GUI/Buttons/Button2"));
            btnTexture1.Add(Content.Load<Texture2D>("GUI/Buttons/Button3"));
            buttonList.Add(new Button(btnTexture1, new Vector2(cam.pos.X - GraphicsDevice.Viewport.Width / 2 + 150, cam.pos.Y + GraphicsDevice.Viewport.Height / 2 - 50), font, "Make TG", makeSelectedTaskGroup));
            buttonList.Add(new Button(btnTexture1, new Vector2(cam.pos.X - GraphicsDevice.Viewport.Width / 2 + 300, cam.pos.Y + GraphicsDevice.Viewport.Height / 2 - 50), font, "Make Square TG", makeSelectedTaskGroup, "Square"));
            //</GUI>
            //<Ships>
            //Load Ship Content
            shipTexList.Add(Content.Load<Texture2D>("Ships/Ship4"));
            shipTexList.Add(Content.Load<Texture2D>("Ships/Ship4 fwdThrust"));
            shipTexList.Add(Content.Load<Texture2D>("Ships/Ship4 revThrust"));
            //</Ships>

            for (int i = 0; i < 25; i++) {
                addShip();
            }


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            // TODO: Add your update logic here
            //<Misc>
            relMousePt = new Vector2((mouseState.X / cam.Zoom - (cam.width / 2)) + cam.pos.X, (mouseState.Y / cam.Zoom - (cam.height / 2)) + cam.pos.Y);
            //</Misc>
            //<Ships>
            for (int i = 0; i < ShipList.Count; i++) {
                ShipList[i].Update();
            }
            TG1.Update();

            //for (int b = 0; b < 10; b++) {
            //    addShip();
            //}
            //Ship2.pos = Ship.targetPt;
            //</Ships>



            //Update Camera
            cam.pos.X += (float)Math.Floor(((cameraPos.X - cam.Pos.X) / cameraEaseFrames));
            cam.pos.Y += (float)Math.Floor(((cameraPos.Y - cam.Pos.Y) / cameraEaseFrames));

            //Input
            inputControl();
            //
            base.Update(gameTime);
        }
        public Ship addShip() {
            ShipList.Add(new Ship());
            Ship myShip = ShipList[ShipList.Count - 1];
            myShip.Initialize(shipTexList, new Vector2(0, 0), 0f);
            Random rnd = new Random();
            float rndnum = (float)(rnd.NextDouble()) + 0.1f;
            //myShip.maxThrust = new Vector2(5f, 5f);
            TG2.addShip(myShip);
            return myShip;
        }
        public List<Ship> selectShips(Boolean reset) {
            List<Ship> selectedList = new List<Ship>();
            for (int s = 0; s < ShipList.Count; s++) {
                Ship curShip = ShipList[s];
                int selectBoxXmin = 0;
                int selectBoxYmin = 0;
                int selectBoxXmax = 0;
                int selectBoxYmax = 0;

                selectBoxXmin = selectBoxOrig.X < selectBoxEnd.X ? (int)selectBoxOrig.X : (int)selectBoxEnd.X;
                selectBoxYmin = selectBoxOrig.Y < selectBoxEnd.Y ? (int)selectBoxOrig.Y : (int)selectBoxEnd.Y;
                selectBoxXmax = selectBoxOrig.X > selectBoxEnd.X ? (int)selectBoxOrig.X : (int)selectBoxEnd.X;
                selectBoxYmax = selectBoxOrig.Y > selectBoxEnd.Y ? (int)selectBoxOrig.Y : (int)selectBoxEnd.Y;

                if (hitTest(curShip.bounds, new Rectangle(selectBoxXmin, selectBoxYmin, selectBoxXmax - selectBoxXmin, selectBoxYmax - selectBoxYmin))) {
                    curShip.selected = true;
                }
                else {
                    if (reset) {
                        curShip.selected = false;
                    }
                    //else { if (curShip.selected == true) {  curShip.selected = true}else{curShip.selected = false;} }
                }

            }
            return selectedList;

        }
        public Boolean hitTest(Rectangle rec1, Rectangle rec2) {
            Boolean result = false;

            if (rec1.Left < rec2.Right && rec1.Right > rec2.Left && rec1.Top < rec2.Bottom && rec1.Bottom > rec2.Top) {
                result = true;
            }

            return result;
        }
        public void inputControl() {
            //<Get States>
            mouseState = Mouse.GetState();
            keyboardState = Keyboard.GetState();
            Boolean onGUI = checkGUI(mouseState);
            //</Get States>

            if (!onGUI) {
                //<Mouse Control>
                if (mouseState.LeftButton == ButtonState.Released) {
                    if (selectMode) {
                        Boolean resetSelect = true;
                        if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift)) {
                            resetSelect = false;
                        }
                        selectShips(resetSelect);
                        makeSelectedTaskGroup();

                    }
                }
                if (mouseState.LeftButton == ButtonState.Pressed) {
                    //
                    if (selectMode == false) {
                        selectBoxOrig = relMousePt;
                    }
                    selectBoxEnd = relMousePt;
                    selectMode = true;
                }
                else {
                    selectMode = false;
                }


                if (mouseState.RightButton == ButtonState.Pressed) {
                        TG1.setTarget(relMousePt);
                }
                if (mouseState.ScrollWheelValue < prevMouseScrollValue && cameraScale > 0.5f) {
                    cameraScale -= 0.1f;
                }
                if (mouseState.ScrollWheelValue > prevMouseScrollValue && cameraScale < 1.5f) {
                    cameraScale += 0.1f;
                }
                cam.Zoom += (cameraScale - cam.Zoom) / cameraEaseFrames;
                prevMouseScrollValue = mouseState.ScrollWheelValue;
                //</Mouse Control>

                //<Keyboard Control>
                if (keyboardState.IsKeyDown(Keys.A)) {
                    cameraPos.X -= cameraSpeed / cam.Zoom;
                }
                if (keyboardState.IsKeyDown(Keys.D)) {
                    cameraPos.X += cameraSpeed / cam.Zoom;
                }
                if (keyboardState.IsKeyDown(Keys.W)) {
                    cameraPos.Y -= cameraSpeed / cam.Zoom;
                }
                if (keyboardState.IsKeyDown(Keys.S)) {
                    cameraPos.Y += cameraSpeed / cam.Zoom;
                }
                if (keyboardState.IsKeyDown(Keys.Space)) {
                    cameraScale = 1;
                    if (TG1.taskUnits.Count > 0) {
                        cameraPos = TG1.taskUnits[0].pos;
                    }
                }
                if (keyboardState.IsKeyDown(Keys.F)) {
                    addShip();
                }
                if (keyboardState.IsKeyDown(Keys.V)) {
                    TG1.setFormation();
                }
                if (keyboardState.IsKeyDown(Keys.B)) {
                    makeSelectedTaskGroup("Square");
                }
                if (keyboardState.IsKeyDown(Keys.R)) {
                    TG1.rotateFormation((float)Math.PI / 4);
                }
                if (keyboardState.IsKeyDown(Keys.Z)) {
                    makeSelectedTaskGroup();
                }
                //</Keyboard Control>
            }
            else {
                //On GUI

            }
        }
        public Boolean makeSelectedTaskGroup(string formType = "") {
            Boolean success = false;
            TG1.clearUnits();
            for (int i = 0; i < ShipList.Count; i++) {
                if (ShipList[i].selected) {
                    TG1.addShip(ShipList[i]);
                }
            }
            if (TG1.taskUnits.Count > 0) {
                success = true;
                TG1.rearAdmiral = TG1.taskUnits[0];
                if (TG1.formation.Count == 0) formType = "Square";
                TG1.setFormation(formType);
            }
            return success;
        }
        Boolean checkGUI(MouseState mState) {
            Boolean res = false;
            for (int i = 0; i < buttonList.Count; i++) {
                if (buttonList[i].Update(mouseState, relMousePt, cam.pos, cam.Zoom)) {
                    res = true;
                }
            }
            return res;
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            int i = 0;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.getTransformation(GraphicsDevice));

            //Grid
            if (drawGrid) {
                for (int y = (-cam.height / 200) - 1; y < 1 + Math.Ceiling((double)(cam.height / 200)); y++) {
                    for (int x = (-cam.width / 200) - 1; x < 1 + Math.Ceiling((double)(cam.width / 200)); x++) {
                        spriteBatch.DrawRectangle(new Rectangle((int)((x * 100) + cam.pos.X), (int)((y * 100) + cam.pos.Y), 100, 100), Color.Black, 2.0f);
                    }
                }
            }

            //

            //FPS
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            frameCounter.Update(deltaTime);

            var fps = string.Format("FPS: {0}", (int)frameCounter.AverageFramesPerSecond + 1);

            spriteBatch.DrawString(font, fps, new Vector2(cam.origPos.X + cam.width - 100, cam.origPos.Y + 10), Color.White);

            //



            //<Ships>
            for (i = 0; i < ShipList.Count; i++) {

                //spriteBatch.DrawLine(ShipList[i].pos, ShipList[i].pos + (ShipList[i].thrust* -50), Color.White,2.0f);
                //spriteBatch.DrawLine(ShipList[i].pos, ShipList[i].pos + (ShipList[i].targetVel * 10), Color.Tomato);
                if (hitTest(ShipList[i].bounds, cam.Bounds)) {
                    ShipList[i].Draw(spriteBatch);
                }

            }
            if (selectMode) {
                spriteBatch.DrawRectangle(new Rectangle((int)selectBoxOrig.X, (int)selectBoxOrig.Y, (int)(selectBoxEnd.X - selectBoxOrig.X), (int)(selectBoxEnd.Y - selectBoxOrig.Y)), Color.LawnGreen, 1.0f / cam.Zoom);
            }
            //</Ships>
            spriteBatch.DrawString(font, "# Ships: " + (ShipList.Count).ToString() + " | # Ships in TG: " + (TG1.taskUnits.Count).ToString(), new Vector2(cam.origPos.X + 10, cam.origPos.Y + 10), Color.White);
            spriteBatch.DrawString(font, (cam.Zoom).ToString(), new Vector2(cam.origPos.X + 400, cam.origPos.Y + 10), Color.White);
            spriteBatch.DrawString(font, (relMousePt).ToString(), new Vector2(cam.origPos.X + 800, cam.origPos.Y + 10), Color.White);
            //

            //<UI>
            for (i = 0; i < buttonList.Count; i++) {
                if (buttonList[i].visible) buttonList[i].Draw(spriteBatch);
            }
            //</UI>
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
