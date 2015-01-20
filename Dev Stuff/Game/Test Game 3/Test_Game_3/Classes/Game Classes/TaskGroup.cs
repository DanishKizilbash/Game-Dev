using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test_Game_3 {
    class TaskGroup {
        public Ship rearAdmiral;
        public int rearAdmiralLoc;
        public List<Ship> taskUnits = new List<Ship>();
        public List<Vector2> formation = new List<Vector2>();
        public int formLineBuffer = 5;
        public float rotation;
        public Boolean inRot = false;
        private int rotCt = 0;
        private int rotCtMax = 15;
        public int formationBuffer = 10;
        public TaskGroup() {

        }
        public void Update() {
            if (inRot) {
                rotCt++;
                if (rotCt >= rotCtMax) {
                    rotCt = 0;
                    inRot = false;
                }
            }
        }
        public void addShip(Ship tShip) {
            taskUnits.Add(tShip);
        }
        public void clearUnits() {
            taskUnits = new List<Ship>();
        }
        public void clearFormation() {
            formation = new List<Vector2>();
        }
        public void setFormation(string type = "") {
            //get/set formation
            //Make Array with all positions of units.
            //normalize them relative to rear admiral.
            //assign targetPts to the respective units.            
            if (taskUnits.Count > 0) {
                clearFormation();
                if (rearAdmiral == null) { rearAdmiral = taskUnits[0]; rearAdmiralLoc = 0; }
                List<Ship> outShips = taskUnits;
                if (type == "Square") {
                    int curPos = 0;
                    int len = (int)Math.Ceiling(Math.Sqrt(taskUnits.Count));
                    for (int y = 0; y < len; y++) {
                        for (int x = 0; x < len; x++) {
                            if (curPos >= taskUnits.Count - 1) {
                                curPos = taskUnits.Count - 1;
                            }
                            formation.Add(new Vector2(rearAdmiral.targetPt.X + (x - len / 2) * taskUnits[curPos].width, rearAdmiral.targetPt.Y + (y - len / 2) * taskUnits[curPos].height));
                            curPos++;
                        }
                    }
                    int temploc = (int)Math.Round(((double)formation.Count / 2)); ;
                    if (len%2!=0) {
                        formation[temploc] = new Vector2(formation[rearAdmiralLoc].X, formation[rearAdmiralLoc].Y);
                        formation[rearAdmiralLoc] = rearAdmiral.targetPt;                        
                    }
                    else {
                        formation[(int)(temploc + len / 2)] = new Vector2(formation[rearAdmiralLoc].X, formation[rearAdmiralLoc].Y);
                        formation[rearAdmiralLoc] = rearAdmiral.targetPt; 
                        
                    }
                    rearAdmiralLoc = 0;
                    //if (rearAdmiralLoc != temploc) {
                       
                    //}
                }
                else {
                    rearAdmiralLoc = 0;
                    for (int i = 0; i < outShips.Count; i++) {
                        if (rearAdmiral == outShips[i]) { rearAdmiralLoc = i; }
                        formation.Add(outShips[i].targetPt);
                    }
                }
                normalizeFormation(rearAdmiral.targetPt);
                setTarget(rearAdmiral.targetPt);
            }

        }
        private void normalizeFormation(Vector2 relPT) {
            //normalize formation: get ship positions relative to flag ship and adjust formation to match
            for (int i = 0; i < formation.Count; i++) {
                Vector2 formPos = formation[i];
                formPos.X = (formPos.X - relPT.X) / rearAdmiral.width;
                formPos.Y = (formPos.Y - relPT.Y) / rearAdmiral.height;
                formation[i] = new Vector2((float)Math.Round(formPos.X), (float)Math.Round(formPos.Y));

            }
        }

        public void rotateFormation(float rot) {
            if (!inRot) {
                inRot = true;
                rotation += rot;
                for (int i = 0; i < formation.Count; i++) {
                    formation[i] = rotateAboutOrigin(formation[i], formation[rearAdmiralLoc], rot);
                }
                rotation = rotation % (float)(Math.PI * 2);
                setTarget(rearAdmiral.targetPt);
            }
        }
        public Vector2 rotateAboutOrigin(Vector2 point, Vector2 origin, float rotation) {
            return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
        }
        public void setTarget(Vector2 tPt) {

            if (taskUnits.Count > 0) {
                if (rearAdmiral == null) {
                    rearAdmiral = taskUnits[0];
                }
                rearAdmiral.targetPt = tPt;
                rearAdmiral.isAdmiral = true;
                int curPos = 0; // curpos is position in Task Units, i is position in formation
                for (int i = 0; i < formation.Count; i++) {
                    if (taskUnits[curPos] != rearAdmiral) {
                        taskUnits[curPos].isAdmiral = false;
                        Vector2 formOffset = new Vector2((taskUnits[curPos].width) * formation[i].X, (taskUnits[curPos].height) * formation[i].Y);
                        taskUnits[curPos].targetPt = new Vector2((rearAdmiral.targetPt.X + rearAdmiral.width / 2) - (taskUnits[curPos].width / 2) + formOffset.X, (rearAdmiral.targetPt.Y + rearAdmiral.height / 2) - (taskUnits[curPos].height / 2) + formOffset.Y);
                    }
                    curPos++;
                    if (curPos >= taskUnits.Count) {
                        i = formation.Count;
                    }
                }
            }
        }
    }
}
