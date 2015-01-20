using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using C3.XNA;

namespace Test_Game_3 {
    public class Ship {
        //<Physics Variables>
        public Vector2 pos;
        public Vector2 vel;
        public Vector2 targetVel;
        public Vector2 targetPt;
        public Vector2 maxThrust;
        public float rotThrust;
        public float rotVel;
        public Vector2 thrust;
        public Vector2 thrustState;
        public int velEaseBuffer;
        public int intervalBuffer;
        public float mass;
        public float relmass;
        public float rotation;
        //
        List<Vector2> thrustList;
        Vector2 prevTPt;
        //</Physics Variables>

        //<Graphics Variables>
        public int spriteState = 0;
        public Texture2D shipTexture;
        public List<Texture2D> shipTextureSet;
        public SpriteEffects flipState;
        public int width { get { return shipTextureSet[0].Width; } }
        public int height { get { return shipTextureSet[0].Height; } }
        public Rectangle bounds;
        //</Graphics Variables>

        //<Game Variables>
        public Boolean selected = false;
        public Boolean isAdmiral = false;
        //</Game Variables>
        public Ship() { }

        public void Initialize(List<Texture2D> inTexture, Vector2 inPos, float inRot) {
            shipTextureSet = inTexture;
            pos = inPos;
            rotation = inRot;
            rotThrust = 0.02f;
            maxThrust.X = 0.2f;
            maxThrust.Y = 0.2f;
            thrustState.X = 1;
            thrustState.Y = 1;
            velEaseBuffer = 1;
            intervalBuffer = 10;
            mass = 1f;
            relmass = mass;
            targetPt = pos;
        }
        public void Update() {
            spriteState = 0;
            flipState = SpriteEffects.None;
            bounds = new Rectangle((int)(pos.X - width / 2), (int)(pos.Y - height / 2), (int)width, (int)height);
            float distToTarget = (float)Dist(targetPt, pos);
            //<Thrusters and Movement>
            //Set thrust Values
            if
                (selected) {
                //thrustList = stepPhysics(100);
            }
            thrust = stepPhysics(1)[0];






            //Slow down if slow enough and close enough
            if (distToTarget < width) {
                if (Math.Abs(vel.X) < maxThrust.X) {
                    thrust.X = -vel.X / velEaseBuffer;
                }

                if (Math.Abs(vel.Y) < maxThrust.Y) {
                    thrust.Y = -vel.Y /velEaseBuffer;
                }
            }



            vel.X += thrust.X / relmass;
            vel.Y += thrust.Y / relmass;

            vel.X = (float)(Math.Floor(vel.X * 1000) / 1000);
            vel.Y = (float)(Math.Floor(vel.Y * 1000) / 1000);
            //set mass
            relmass = mass + Math.Abs((vel.X + vel.Y) / 100);
            //</Thrusters and Movement>

            // get final position
            pos.X += vel.X;
            pos.Y += vel.Y;


            //<rotation Control>
            double rotDiff = (Math.Atan2(targetPt.Y - pos.Y, targetPt.X - pos.X) - rotation);
            //Ineffecient rotation fix
            if (rotDiff > Math.PI) {
                rotDiff -= Math.PI * 2;
            }
            if (rotDiff < -Math.PI) {
                rotDiff += Math.PI * 2;
            }
            //Rotation Thrusters
            int rotAccInts = (int)Math.Abs(rotVel / rotThrust);
            int rotVelInts = (int)Math.Abs(rotDiff / rotVel);
            if (rotAccInts < rotVelInts || rotVel == 0) {
                if (Math.Floor(rotDiff) < 0 - rotThrust) {
                    rotVel -= rotThrust;
                }
                else if (Math.Ceiling(rotDiff) > 0 + rotThrust) {
                    rotVel += rotThrust;
                }
            }
            else {
                if (rotVel > 0) {
                    rotVel -= rotThrust;
                }
                else if (rotVel < 0) {
                    rotVel += rotThrust;
                }
            }
            if (Math.Abs(vel.X) < maxThrust.X && Math.Abs(vel.Y) < maxThrust.Y && Math.Abs(rotVel) / relmass < rotThrust) {
                rotVel = 0;
            }
            //Perform rotation movement
            rotation += rotVel / relmass;
            //normalize rotation value
            rotation = rotation % (float)(Math.PI * 2);
            //</Rotation Control>

            prevTPt = targetPt;
        }

        public List<Vector2> stepPhysics(int steps) {
            //Physics step returns vector movements following standard movement as Vector2 List
            List<Vector2> thrustList = new List<Vector2>();
            Vector2 tempVel = vel;
            Vector2 tempPos = pos;
            Vector2 tempThrust = thrust;
            Vector2 tempThrustState = thrustState;
            float tempRelmass = relmass;
            for (int curStep = 0; curStep < steps; curStep++) {
                //<Physics Step>
                //
                //<Determine Thrust State>
                //Thrust state 0 = Deccelerate
                //Thrust state 1 = Accelerate
                float distToTarget = (float)Dist(targetPt, tempPos);
                Vector2 accIntervalsToNill = new Vector2(Math.Abs(tempVel.X / (maxThrust.X / tempRelmass)) + intervalBuffer, Math.Abs(tempVel.Y / (maxThrust.Y / tempRelmass)) + intervalBuffer);
                Vector2 velIntervalsToPt = new Vector2(Math.Abs((targetPt.X - tempPos.X) / (float)(Math.Abs(tempVel.X)+0.01)), Math.Abs((targetPt.Y - tempPos.Y) / (float)(Math.Abs(tempVel.Y)+0.01)));
                //get Decceleration point
                //X
                if (accIntervalsToNill.X > velIntervalsToPt.X) {
                    if (targetPt.X > tempPos.X) {
                        if (tempVel.X > 0) {
                            tempThrustState.X = 0;
                        }
                    }
                    else {
                        if (tempVel.X < 0) {
                            tempThrustState.X = 0;
                        }
                    }
                }
                else {
                    tempThrustState.X = 1;
                }


                //Y
                if (accIntervalsToNill.Y > velIntervalsToPt.Y) {
                    if (targetPt.Y > tempPos.Y) {
                        if (tempVel.Y > 0) {
                            tempThrustState.Y = 0;
                        }
                    }
                    else {
                        if (tempVel.Y < 0) {
                            tempThrustState.Y = 0;
                        }
                    }

                }
                else {
                    tempThrustState.Y = 1;
                }
                //</Determine Thrust State>
                // <Manage Thrust state vectors>
                float angleToTarget = (float)Math.Atan2(targetPt.Y - tempPos.Y, targetPt.X - tempPos.X);
                double velAngle = Math.Atan2(tempVel.Y, tempVel.X);
                //If deccelerate, thrust opposite to velocity vector otherwise thrust towards target
                if (tempThrustState.X == 0) {
                    targetVel.X = -tempVel.X;
                }
                else {
                    //targetVel.X = (float)Math.Cos(angleToTarget);
                    targetVel.X = (float)Math.Cos(angleToTarget) + (float)(Math.Cos(angleToTarget) - Math.Cos(velAngle)) / (velIntervalsToPt.X+1);
                }
                if (tempThrustState.Y == 0) {
                    targetVel.Y = -tempVel.Y;
                }
                else {
                    //targetVel.Y = (float)Math.Sin(angleToTarget);
                    targetVel.Y = (float)Math.Sin(angleToTarget) + (float)(Math.Sin(angleToTarget) - Math.Sin(velAngle)) / (velIntervalsToPt.Y+1);
                }
                targetVel.Normalize();
                //
                //</ Manage Thrust state vectors>
                //                             
                //  </Physics Step>  
                //Add Thrust Vector to List
                tempThrust.X = maxThrust.X * targetVel.X;
                tempThrust.Y = maxThrust.Y * targetVel.Y;

                //Slow down if slow enough and close enough
                if (distToTarget < width) {
                    if (Math.Abs(tempVel.X) < maxThrust.X) {
                        tempThrust.X = -tempVel.X / ((distToTarget / maxThrust.X) + velEaseBuffer);
                    }

                    if (Math.Abs(tempVel.Y) < maxThrust.Y) {
                        tempThrust.Y = -tempVel.Y / ((distToTarget / maxThrust.Y) + velEaseBuffer);
                    }
                }



                tempVel.X += tempThrust.X / tempRelmass;
                tempVel.Y += tempThrust.Y / tempRelmass;

                tempVel.X = (float)(Math.Floor(tempVel.X * 1000) / 1000);
                tempVel.Y = (float)(Math.Floor(tempVel.Y * 1000) / 1000);
                //set mass
                tempRelmass = mass + Math.Abs((tempVel.X + tempVel.Y) / 100);
                //</Thrusters and Movement>

                // get final position
                tempPos.X += tempVel.X;
                tempPos.Y += tempVel.Y;

                thrustList.Add(tempThrust);
            }
            return thrustList;
        }

        public void Draw(SpriteBatch spriteBatch) {
            shipTexture = shipTextureSet[spriteState];
            spriteBatch.Draw(shipTexture, pos, new Rectangle(0, 0, width, height), Color.White, rotation, new Vector2(width / 2, height / 2), 0.8f, flipState, 0);
            if (selected) {
                Color col = Color.SpringGreen;
                if (isAdmiral) col = Color.Red;
                spriteBatch.DrawRectangle(new Rectangle((int)(pos.X - width / 2), (int)(pos.Y - height / 2), (int)width, (int)height), col, 2.0f);
                /*
                  Vector2 curPos = pos;                
                  Vector2 curVel = vel;
                  for (int i = 0; i < thrustList.Count; i++) {
                      Vector2 tempthrust = thrustList[i];
                      curVel += tempthrust;
                      spriteBatch.DrawLine(curPos, curPos + curVel, Color.Red);
                      curPos += curVel;                
              }
               */

            };



        }
        private double Dist(Vector2 pt1, Vector2 pt2) {
            return Math.Sqrt((pt2.Y - pt1.Y) * (pt2.Y - pt1.Y) + (pt2.X - pt1.X) * (pt2.X - pt1.X));
        }

    }
}

