using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ListViewSandBox
{
    public class TreeViewEntry
    {
        public TreeViewEntry()
        {
        }

        public TreeViewEntry(string name)
        {
            this.name = name;
        }
        private string name;
        private List<TreeViewEntry> entryList;

        public double Entropie;
        public int Anzahl;
        public bool IsSingleValue;

        public List<TreeViewEntry> EntryList
        {
            get
            {
                if (this.entryList == null)
                    this.entryList = new List<TreeViewEntry>();
                return this.entryList;
            }
            set { this.entryList = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }
}
