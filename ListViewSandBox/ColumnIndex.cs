using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ListViewSandBox
{
    public class ColumnIndex
    {
        private string name;
        private int index;
        private string goalName;

        public string Name
        {
            get { return this.name; }
            set { name = value; }
        }

        public int Index
        {
            get { return this.index; }
            set { index = value; }
        }

        public string GoalName
        {
            get { return this.goalName; }
            set { goalName = value; }
        }
    }
}
