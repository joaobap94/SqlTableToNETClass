
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SQLToClass
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                Clipboard.SetText(textBox1.Text);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string item = listBox1.SelectedItem.ToString();


            try
            {
                string schema = "";
                string tableName = item;

                if (item.Contains("."))
                {
                    schema = item.Split('.')[0];
                    tableName = item.Split('.')[1];
                }

                using (IDbConnection cn = new SqlConnection(textBox2.Text))
                {

                        var columnInfo = cn.Query<ColumnData>($"SELECT column_name as ColumnName, data_type as DataType FROM information_schema.columns WHERE table_name = '{tableName}'", cn);
                        

                        // Generate C# class
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"public class {item}");
                        sb.AppendLine("{");
                        foreach (var column in columnInfo)
                        {
                            sb.AppendLine($"    public {GetCSharpType(column.DataType)} {column.ColumnName} {{ get; set; }}");
                        }
                        sb.AppendLine("}");


                    textBox1.Text = sb.ToString().Replace($"public class {schema}.{tableName}", $"public class {tableName}");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error obtaining class from table: {ex.Message}", "Error");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            var list = PopulateTables();
            if (list != null && list.Any())
            {
                foreach (var item in list)
                {
                    listBox1.Items.Add(item);
                }
            }
        }

        public List<string> PopulateTables()
        {
            try
            {
                using (IDbConnection cn = new SqlConnection(textBox2.Text))
                {
                    List<string> tableNames = cn.Query<string>("SELECT CONCAT(SCHEMA_NAME(schema_id), '.', s.name) as tblname from sys.tables s ORDER BY name").ToList();

                    return tableNames;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error obtaining list from connection string: {ex.Message}", "Error");
                return null;
            }
        }

        public string GetCSharpType(string sqlType)
        {
            switch (sqlType.ToLower())
            {
                case "int":
                    return "int";
                case "bigint":
                    return "long";
                case "smallint":
                    return "short";
                case "tinyint":
                    return "byte";
                case "bit":
                    return "bool";
                case "decimal":
                    return "decimal";
                case "numeric":
                    return "decimal";
                case "money":
                    return "decimal";
                case "smallmoney":
                    return "decimal";
                case "float":
                    return "float";
                case "real":
                    return "float";
                case "date":
                    return "DateTime";
                case "datetime":
                    return "DateTime";
                case "datetime2":
                    return "DateTime";
                case "smalldatetime":
                    return "DateTime";
                case "char":
                    return "string";
                case "varchar":
                    return "string";
                case "text":
                    return "string";
                case "nchar":
                    return "string";
                case "nvarchar":
                    return "string";
                case "ntext":
                    return "string";
                case "binary":
                    return "byte[]";
                case "varbinary":
                    return "byte[]";
                case "image":
                    return "byte[]";
                case "uniqueidentifier":
                    return "Guid";
                default:
                    return "object";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "class|*.cs";
                save.Title = "Save Class";

                string item = listBox1.SelectedItem.ToString();

                if (item.Contains(".")) {
                    string table = item.Split(".")[1];


                    save.FileName = table;
                }
                else
                {
                    save.FileName = item;

                }

                save.ShowDialog();



                if (save.FileName != "")
                {
                    StreamWriter writer = new StreamWriter(save.OpenFile());
                    writer.Write(textBox1.Text);

                    writer.Dispose();

                    writer.Close();
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

