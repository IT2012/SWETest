using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using Binding = System.Windows.Data.Binding;
using DataGridCell = System.Windows.Controls.DataGridCell;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using TreeView = System.Windows.Controls.TreeView;

namespace ListViewSandBox
{
    //LALAAA
    //TEST2 von benni
	//TEST3 von benni
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public double _testEntropie = 0;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private DataTable _myDataTable;
        public DataTable MyDataTable
        {
            get { return _myDataTable; }
            private set
            {
                _myDataTable = value;
                OnPropertyChanged("MyDataTable");
            }
        }

        public List<List<DataTable>> SplittetTablesList = new List<List<DataTable>>();
        public List<double> BufferDoubles = new List<double>();

        public MainWindow()
        {
            InitializeComponent();

            MyDataTable = new DataTable();

            DGrid.ItemsSource = MyDataTable.DefaultView;

            List<TreeNode> testList = new List<TreeNode>();

            Tree.ItemsSource = testList;

            TreeNode treeNode = new TreeNode();

            treeNode.DTable = MyDataTable;
        }

        private void MiCsvSaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "CSV Files|*.csv" };
            saveFileDialog.ShowDialog();

            SaveCSV(MyDataTable, saveFileDialog.FileName, ";");

        }

        private void SaveCSV(DataTable csvData, string csvFile, string seperator)
        {
            StringBuilder sb = new StringBuilder();

            var columnNames = csvData.Columns.Cast<DataColumn>().Select(column => "\"" + column.ColumnName.Replace("\"", "\"\"") + "\"").ToArray();
            sb.AppendLine(string.Join(seperator, columnNames));

            foreach (DataRow row in csvData.Rows)
            {
                var fields = row.ItemArray.Select(field => "\"" + field.ToString().Replace("\"", "\"\"") + "\"").ToArray();
                sb.AppendLine(string.Join(seperator, fields));
            }

            File.WriteAllText(csvFile, sb.ToString(), Encoding.Default);

        }

        private static DataTable LoadDataTabletFromCSVFile(string csvFilePath)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csvFilePath))
                {
                    csvReader.Delimiters = new string[] { ";" };
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    csvReader.CommentTokens = new String[] { "" };
                    string[] singleRowStrings = csvReader.ReadFields();
                    if (singleRowStrings == null)
                        return csvData;
                    foreach (string column in singleRowStrings)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        singleRowStrings = csvReader.ReadFields();
                        if (singleRowStrings == null)
                            return csvData;
                        for (int i = 0; i < singleRowStrings.Length; i++)
                        {
                            if (string.IsNullOrEmpty(singleRowStrings[i]) || string.IsNullOrWhiteSpace(singleRowStrings[i]))
                            {
                                singleRowStrings[i] = null;
                            }
                        }
                        csvData.Rows.Add(singleRowStrings);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return csvData;
        }

        private void MiCsvLoadClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "CSV Files|*.csv" };
            openFileDialog.ShowDialog();

            MyDataTable = LoadDataTabletFromCSVFile(openFileDialog.FileName);

            DGrid.SetBinding(ItemsControl.ItemsSourceProperty, new Binding { Source = MyDataTable });
        }

        private TreeNode GetTree(TreeNode treeNode, DataTable dataTable, int columnIndex)
        {
            treeNode.Name = treeNode.FlowName;
            treeNode.Name += dataTable.Columns[columnIndex].ColumnName;
            treeNode.DTable = dataTable;
            foreach (DataRow row in dataTable.Rows)
            {
                if (treeNode.Children.All(c => c.FlowName != row.ItemArray[columnIndex].ToString()))
                {
                    TreeNode childNode = new TreeNode()
                    {
                        ParentTreeNode = treeNode as TreeNode,
                        FlowName = row.ItemArray[columnIndex].ToString(),
                        DTable = new DataTable()
                    };
                    childNode.DTable = dataTable.Clone();
                    treeNode.Children.Add(childNode);
                }
                if (treeNode.Children.Any(c => c.FlowName == row.ItemArray[columnIndex].ToString()))
                {
                    TreeNode childNode = treeNode.Children.FirstOrDefault(c => c.FlowName == row.ItemArray[columnIndex].ToString());
                    //TreeNode childNode = treeNode.Children.Where(c => c.FlowName == row.ItemArray[columnIndex].ToString()).FirstOrDefault();
                    if (childNode != null)
                    {
                        childNode.DTable.ImportRow(row);
                    }
                }
            }
            //foreach (TreeNode child in treeNode.Children)
            //{
            //    child.DTable.Columns.RemoveAt(columnIndex);
            //}
            //if (BufferDoubles.Any(bd => (!double.IsNaN(bd))))
            if (_testEntropie != 0)
            {

                for (int i = 0; i < treeNode.Children.Count; i++)
                {
                    TreeNode childNode = treeNode.Children[i];

                    childNode.DTable.Columns.RemoveAt(columnIndex);

                    string trigger = string.Empty;
                    bool triggerChanged = _doubleEliminatedDataTable(childNode.DTable, childNode.DTable.Columns.Count - 1).Rows.Count == 1;


                    childNode.IsLast = false;
                    if (!triggerChanged)
                    {
                        int newColumnIndex = _smallesColumnIndex(childNode.DTable, childNode);
                        if (newColumnIndex == childNode.ClassifiedColumn)
                        {
                            childNode.IsClassified = true;
                        }
                        childNode = GetTree(childNode, childNode.DTable, newColumnIndex);

                        //foreach (TreeNode child in treeNode.Children)
                        //{
                        //    child.DTable.Columns.RemoveAt(newColumnIndex);
                        //}

                    }
                    else
                    {
                        setRectangle(treeNode);

                    }
                    if (!(childNode.Children != null))
                    {
                        childNode.IsLast = true;
                    }
                }

            }
            else
            {
                setRectangle(treeNode);
            }

            return treeNode;
        }

        private static void setRectangle(TreeNode treeNode)
        {
            if (treeNode.IsClassified)
            {
                treeNode.Children = new List<TreeNode>();
                for (int i = 0; i < treeNode.DTClassified.Rows.Count; i++)
                {
                    TreeNode childNode = new TreeNode();
                    childNode.FlowName = treeNode.DTClassified.Rows[i][0].ToString();
                    if (treeNode.DTClassified.Rows[i][2].ToString() == "0")
                    {
                        childNode.Name = "n";
                    }
                    else
                    {
                        childNode.Name = "j";
                    }
                    childNode.Name += "=";
                    childNode.Name += treeNode.DTClassified.Rows[i][1].ToString();
                    treeNode.Children.Add(childNode);
                    childNode.IsLast = true;
                }
            }
            else
            {
                for (int i = 0; i < treeNode.Children.Count; i++)
                {
                    TreeNode childNode = treeNode.Children[i];
                    //childNode.DTable.Columns.RemoveAt(columnIndex);
                    //int newColumnIndex = KnotenFinden(treeNode.DTable,treeNode);
                    childNode.Name = treeNode.Children[i].DTable.Rows[0].ItemArray[treeNode.Children[i].DTable.Columns.Count - 1].ToString();
                    childNode.Name += "=";
                    childNode.Name += treeNode.Children[i].DTable.Rows.Count;
                    childNode.IsLast = true;
                }
            }
        }


        private List<DataTable> GetChildTable(int index, DataTable dataTable)
        {
            List<DataTable> dBuffrtDataTables = new List<DataTable>();
            foreach (DataRow row in dataTable.Rows)
            {

                if (!dBuffrtDataTables.Any(c => c.TableName.Contains(row.ItemArray[index].ToString())))
                {
                    DataTable childTable = new DataTable();
                    childTable = dataTable.Clone();
                    childTable.TableName = row.ItemArray[index].ToString();
                    dBuffrtDataTables.Add(childTable);
                }
                if (dBuffrtDataTables.Any(c => c.TableName.Contains(row.ItemArray[index].ToString())))
                {
                    DataTable childTable = dBuffrtDataTables.FirstOrDefault(c => c.TableName.Contains(row.ItemArray[index].ToString()));
                    //DataTable childTable = dBuffrtDataTables.Where(c => c.TableName.Contains(row.ItemArray[index].ToString())).FirstOrDefault();
                    if (childTable != null)
                    {
                        childTable.ImportRow(row);
                    }
                }
            }
            return dBuffrtDataTables;
        }

        private void GenTreeOnClick(object sender, RoutedEventArgs e)
        {
            int columnIndex = _smallesColumnIndex(MyDataTable, new TreeNode());
            TreeNode testerNode = GetTree(new TreeNode(), MyDataTable, columnIndex);
            List<TreeNode> testList = new List<TreeNode> { testerNode };
            Tree.ItemsSource = testList;
            BaumTabItem.IsSelected = true;
        }

        private bool _containsNumbers(DataTable table, int spalte)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                double dummy = 0;

                if (!double.TryParse(table.Rows[i][spalte].ToString(), out dummy))
                    return false;

            }

            return true;
        }

        private int _smallesColumnIndex(DataTable quellDaten, TreeNode parent)
        {
            DataTable anzahlAttribute = new DataTable();
            anzahlAttribute.Columns.Add("Bezeichnung");
            anzahlAttribute.Columns.Add("AnzahlGesamt");
            anzahlAttribute.Columns.Add("AnzahlAntwortJa");

            int kleinsteSpalte = 0;
            double kleinsteEntropie = 1;
            double aktuelleEntropie = 0;

            for (int i = 0; i < quellDaten.Columns.Count - 1; i++)
            {

                anzahlAttribute = _doubleEliminatedDataTable(quellDaten, i);
                anzahlAttribute.DefaultView.Sort = anzahlAttribute.Columns[0].ColumnName + " ASC";
                anzahlAttribute = anzahlAttribute.DefaultView.ToTable();

                DataTable klassifizierteDaten = new DataTable();

                if (_containsNumbers(anzahlAttribute, 0))
                {
                    klassifizierteDaten = _classifiedDataTable(anzahlAttribute);
                    parent.DTClassified = klassifizierteDaten;
                    parent.ClassifiedColumn = i;
                    if (klassifizierteDaten.Rows.Count < 3)
                    {
                        aktuelleEntropie = CalculatedEntropy(klassifizierteDaten);
                    }
                    else
                    {
                        aktuelleEntropie = 1;
                    }
                }
                else
                {

                    aktuelleEntropie = CalculatedEntropy(anzahlAttribute);
                }

                if (aktuelleEntropie < kleinsteEntropie)
                {
                    kleinsteEntropie = aktuelleEntropie;
                    kleinsteSpalte = i;
                }

            }
            _testEntropie = kleinsteEntropie;

            return kleinsteSpalte;
        }


        private DataTable _classifiedDataTable(DataTable table)
        {
            DataTable rueckgabeTabelle = new DataTable();
            rueckgabeTabelle.Columns.Add("Bezeichnung");
            rueckgabeTabelle.Columns.Add("AnzahlGesamt");
            rueckgabeTabelle.Columns.Add("AnzahlAntwortJa");

            int zaehler = 0;
            double vonWert = 0;
            double bisWert = 0;

            for (int i = 0; i < table.Rows.Count - 1; i++)
            {
                if (table.Rows[i][2].ToString() == table.Rows[i + 1][2].ToString())
                {
                    zaehler += int.Parse(table.Rows[i][1].ToString());
                }
                else
                {
                    rueckgabeTabelle.Rows.Add();

                    bisWert = (double.Parse(table.Rows[i][0].ToString()) + double.Parse(table.Rows[i + 1][0].ToString())) / 2;
                    zaehler += int.Parse(table.Rows[i][1].ToString());

                    if (vonWert == 0)
                        rueckgabeTabelle.Rows[rueckgabeTabelle.Rows.Count - 1][0] = "<= " + bisWert;
                    else
                        rueckgabeTabelle.Rows[rueckgabeTabelle.Rows.Count - 1][0] = "> " + vonWert + " , <= " + bisWert;

                    rueckgabeTabelle.Rows[rueckgabeTabelle.Rows.Count - 1][1] = zaehler;
                    rueckgabeTabelle.Rows[rueckgabeTabelle.Rows.Count - 1][2] = table.Rows[i][2].ToString() == "0" ? 0 : zaehler;

                    vonWert = bisWert;
                    zaehler = 0;
                }

            }
            #region Für die letzte Zeile
            if (table.Rows[table.Rows.Count - 1][2].ToString() == table.Rows[table.Rows.Count - 2][2].ToString())
                zaehler += int.Parse(table.Rows[table.Rows.Count - 1][1].ToString());
            else
                zaehler = 1;

            rueckgabeTabelle.Rows.Add();

            rueckgabeTabelle.Rows[rueckgabeTabelle.Rows.Count - 1][0] = "> " + bisWert;
            rueckgabeTabelle.Rows[rueckgabeTabelle.Rows.Count - 1][1] = zaehler;
            rueckgabeTabelle.Rows[rueckgabeTabelle.Rows.Count - 1][2] = table.Rows[table.Rows.Count - 1][2].ToString() == "0" ? 0 : zaehler;
            #endregion


            return rueckgabeTabelle;
        }

        private DataTable _doubleEliminatedDataTable(DataTable csvData, int spaltePruefen)
        {

            DataTable ergebnis = new DataTable();
            ergebnis.Columns.Add("Bezeichnung");
            ergebnis.Columns.Add("AnzahlGesamt");
            ergebnis.Columns.Add("AnzahlAntwortJa");

            bool schon_vorhanden = false;

            for (int i = 0; i < csvData.Rows.Count; i++)
            {
                for (int j = 0; j < ergebnis.Rows.Count; j++)
                {
                    if (csvData.Rows[i][spaltePruefen].ToString() == ergebnis.Rows[j][0].ToString())
                    {
                        schon_vorhanden = true;
                        break;

                    }

                }
                if (schon_vorhanden == false)
                {
                    ergebnis.Rows.Add(csvData.Rows[i][spaltePruefen].ToString(), 0);

                }
                else
                {
                    schon_vorhanden = false;
                }
            }


            for (int i = 0; i < ergebnis.Rows.Count; i++)
            {
                int anzahlGesamt = 0;
                int anzahlAntwortJa = 0;
                for (int j = 0; j < csvData.Rows.Count; j++)
                {
                    if (csvData.Rows[j][spaltePruefen].ToString() == ergebnis.Rows[i][0].ToString())
                    {
                        anzahlGesamt += 1;
                    }

                    if (csvData.Rows[j][spaltePruefen].ToString() == ergebnis.Rows[i][0].ToString()
                        && csvData.Rows[j][csvData.Columns.Count - 1].ToString() == "j")
                    {
                        anzahlAntwortJa += 1;
                    }

                }
                ergebnis.Rows[i][1] = anzahlGesamt;
                ergebnis.Rows[i][2] = anzahlAntwortJa;
            }
            return ergebnis;
        }


        double CalculatedEntropy(DataTable table)
        {

            double wert = 0;
            double gesamt = 0;

            for (int i = 0; i < table.Rows.Count; i++)
                gesamt += double.Parse(table.Rows[i][1].ToString());


            for (int j = 0; j < table.Rows.Count; j++)
            {
                double anzahl = 0;
                double antwortJa = 0;
                double antwortNein = 0;

                anzahl = double.Parse(table.Rows[j][1].ToString());
                antwortJa = double.Parse(table.Rows[j][2].ToString());
                antwortNein = anzahl - antwortJa;

                wert += ((anzahl / gesamt) * ((-antwortJa / anzahl * CalculatedLog(antwortJa, anzahl)) - ((antwortNein / anzahl) * CalculatedLog(antwortNein, anzahl))));
                if (double.IsNaN(wert))
                {
                    string a = "";
                }
            }

            return wert;
        }

        double CalculatedLog(double wert1, double wert2)
        {
            double value = 0;

            value = Math.Log(wert1 / wert2, 2);

            if (double.IsInfinity(value))
            {
                return 0;
            }
            return value;

        }
    }
}
