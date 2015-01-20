using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test_Game_3 {
    class Fleet {
        public Ship flagShip;
        public List<TaskGroup> taskGroups = new List<TaskGroup>();
        public Fleet() { }
        public void addTaskGroup(TaskGroup tTaskGroup) {
            taskGroups.Add(tTaskGroup);
        }
        public void clear() {
            taskGroups = new List<TaskGroup>();
        }
    }
}
