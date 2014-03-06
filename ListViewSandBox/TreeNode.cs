using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace ListViewSandBox
{
    public class TreeNode
    {
        public TreeNode()
        {

        }

        private TreeNode _parentTreeNode;

        public TreeNode ParentTreeNode
        {
            get { return _parentTreeNode; }
            set { _parentTreeNode = value; }
        }

        private int _currentColumn;

        public int CurrentColumn
        {
            get { return _currentColumn; }
            set { _currentColumn = value; }
        }

        private List<TreeNode> _children;

        public List<TreeNode> Children
        {
            get
            {
                if (_children == null)
                    _children = new List<TreeNode>();
                return _children;
            }
            set { _children = value; }
        }

        private DataTable _dataTable;

        public DataTable DTable
        {
            get { return _dataTable; }
            set { _dataTable = value; }
        }

        private DataTable _classifiedDataTable;

        public DataTable DTClassified
        {
            get { return _classifiedDataTable; }
            set { _classifiedDataTable = value; }
        }

        private string _flowName;

        public string FlowName
        {
            get { return _flowName; }
            set { _flowName = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private bool _isLast;

        public bool IsLast
        {
            get { return _isLast; }
            set { _isLast = value; }
        }

        private bool _isClassified;

        public bool IsClassified
        {
            get { return _isClassified; }
            set { _isClassified = value; }
        }

        public int ClassifiedColumn { get; set; }

        public Point CurrentPosition { get; set; }

        public Point ParentsPosition { get; set; }

        public double Entropy { get; set; }


    }
}
