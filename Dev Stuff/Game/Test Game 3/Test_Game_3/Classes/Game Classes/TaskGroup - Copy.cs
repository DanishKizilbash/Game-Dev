using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test_Game_3 {
    class TaskGroupBack {
        public Ship rearAdmiral;
        public List<Ship> taskUnits = new List<Ship>();
        public List<List<Vector2>> formation = new List<List<Vector2>>();
        public int formLineBuffer = 10;
        public TaskGroupBack() {

        }
        public void addShip(Ship tShip) {
            taskUnits.Add(tShip);
        }
        public void clearUnits() {
            taskUnits = new List<Ship>();
        }
        public void clearFormation() {
            formation = new List<List<Vector2>>();
        }
        public void setFormation() {
            //get formation
            //Make Array with all positions of units.
            //normalize them relative to rear admiral.
            //assign targetPts to the respective units.
            if (taskUnits.Count > 0) {
                clearFormation();
                List<Ship> outShips = taskUnits;
                float prevY = outShips[0].pos.Y;
                int previ = 0;
                for (int i = 0; i < outShips.Count; i++) {
                    Console.WriteLine(outShips[i].pos);
                    if ((float)Math.Abs(prevY - outShips[i].pos.Y) > formLineBuffer|| i>=outShips.Count-1) {
                        
                        List<Ship> yElements = outShips.GetRange(previ, i - previ).ToList<Ship>();
                        Console.WriteLine("Difference found, adding set with length: " + yElements.Count);
                        List<Vector2> posElements = new List<Vector2>();
                        foreach (Ship ele in yElements) {
                            posElements.Add(ele.pos);
                        }
                        formation.Add(posElements);
                        previ = i;
                        prevY = outShips[i].pos.Y;
                    }
                }
                normalizeFormation();
            }

             Console.WriteLine("////////////");
        }
        public void setFormation2() {
            //get formation
            if (taskUnits.Count > 0) {
                clearFormation();
                List<Ship> outShips = taskUnits;
                float prevY = outShips[0].pos.Y;
                int previ = 0;
                for (int i = 0; i < outShips.Count; i++) {
                    Console.WriteLine(outShips[i].pos);
                    if ((float)Math.Abs(prevY - outShips[i].pos.Y) > formLineBuffer || i >= outShips.Count - 1) {

                        List<Ship> yElements = outShips.GetRange(previ, i - previ).ToList<Ship>();
                        Console.WriteLine("Difference found, adding set with length: " + yElements.Count);
                        List<Vector2> posElements = new List<Vector2>();
                        foreach (Ship ele in yElements) {
                            posElements.Add(ele.pos);
                        }
                        formation.Add(posElements);
                        previ = i;
                        prevY = outShips[i].pos.Y;
                    }
                }
                normalizeFormation();
            }

            Console.WriteLine("////////////");
        }
        private void normalizeFormation() {
            //normalize formation: get ship positions relative to flag ship and adjust formation to match
            int y = 0;
            int x = 0;
            for (y = 0; y < formation.Count; y++) {
                for (x = 0; x < formation[y].Count; x++) {
                    Vector2 formPos = formation[y][x];
                    formPos = (formPos-rearAdmiral.pos) / rearAdmiral.width;
                    formation[y][x] = new Vector2((float)Math.Round(formPos.X), (float)Math.Round(formPos.Y));
                   // Console.WriteLine("At loc.  x: " + x + " y: " + y + " position: " + formation[y][x]);
                }
            }
        }
        
        public void sendShipsToFormation() {
            int curPos = 0;
            for (int y = 0; y < formation.Count; y++) {
                for (int x = 0; x < formation[y].Count; x++) {
                    if (taskUnits[curPos] != rearAdmiral) {
                        Vector2 formOffset = new Vector2(taskUnits[curPos].width * formation[y][x].X, taskUnits[curPos].height * formation[y][x].Y);
                        taskUnits[curPos].targetPt = new Vector2((rearAdmiral.targetPt.X - rearAdmiral.width / 2) - (taskUnits[curPos].width / 2) + formOffset.X, (rearAdmiral.targetPt.Y - rearAdmiral.height / 2) - (taskUnits[curPos].height / 2) + formOffset.Y);
                    }
                    curPos++;
                    if (curPos >= taskUnits.Count) {
                        x = formation[y].Count;
                    }
                }
                if (curPos >= taskUnits.Count) {
                    y = formation.Count;
                }
            }

        }
        public void setTarget(Vector2 tPt, String type = "") {
            if (taskUnits.Count > 0) {
                //taskUnits.Sort(sortByShipPos);
                if (rearAdmiral == null) {
                    rearAdmiral = taskUnits[0];
                }
                rearAdmiral.targetPt = tPt;

                if (type == "Square") {
                    clearFormation();
                    int len = (int)Math.Ceiling(Math.Sqrt(taskUnits.Count));
                    Console.WriteLine(len);
                    for (int y = 0; y < len; y++) {
                        formation.Add(new List<Vector2>());
                        for (int x = 0; x < len; x++) {
                            formation[y].Add(new Vector2(x, y));
                        }
                    }

                    //Vector2 formOffset = new Vector2(taskUnits[i].width * (float)Math.Floor(i / len), taskUnits[i].height * ((i / len * len) % len));
                    //taskUnits[i].targetPt = new Vector2(tPt.X + formOffset.X - (len * taskUnits[i].width / 2), tPt.Y + formOffset.Y - (len * taskUnits[i].height / 2));
                }
                else {



                }
                sendShipsToFormation();
            }
        }
    }
}
