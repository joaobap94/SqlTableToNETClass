
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SQLToClass
{
    public partial class Form1 : Form
    {
        string coolsql = "declare @TableName sysname = '{0}' declare @Result varchar(max) = 'public class ' + @TableName + ' {'  select @Result = @Result + ' public ' + ColumnType + NullableSign + ' ' + ColumnName + ' { get; set; } ' from(select replace(col.name, ' ', '_') ColumnName, column_id ColumnId, case typ.name when 'bigint' then 'long' when 'binary' then 'byte[]' when 'bit' then 'bool' when 'char' then 'string' when 'date' then 'DateTime' when 'datetime' then 'DateTime' when 'datetime2' then 'DateTime' when 'datetimeoffset' then 'DateTimeOffset' when 'decimal' then 'decimal' when 'float' then 'double' when 'image' then 'byte[]' when 'int' then 'int' when 'money' then 'decimal' when 'nchar' then 'string' when 'ntext' then 'string' when 'numeric' then 'decimal' when 'nvarchar' then 'string' when 'real' then 'float' when 'smalldatetime' then 'DateTime' when 'smallint' then 'short' when 'smallmoney' then 'decimal' when 'text' then 'string' when 'time' then 'TimeSpan' when 'timestamp' then 'long' when 'tinyint' then 'byte' when 'uniqueidentifier' then 'Guid' when 'varbinary' then 'byte[]' when 'varchar' then 'string' else 'UNKNOWN_' + typ.name end ColumnType, case when col.is_nullable = 1 and typ.name in ('bigint', 'bit', 'date', 'datetime', 'datetime2', 'datetimeoffset', 'decimal', 'float', 'int', 'money', 'numeric', 'real', 'smalldatetime', 'smallint', 'smallmoney', 'time', 'tinyint', 'uniqueidentifier') then '?' else '' end NullableSign from sys.columns col join sys.types typ on col.system_type_id = typ.system_type_id AND col.user_type_id = typ.user_type_id where object_id = object_id(@TableName)) t order by ColumnId  set @Result = @Result + ' }' select @Result as RetVal ";
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
                string table = item;

                if (item.Contains("."))
                {
                    schema = item.Split('.')[0];
                    table = item.Split('.')[1];
                }

                using (IDbConnection cn = new SqlConnection(textBox2.Text))
                {

                    var res = cn.Query<string>(coolsql.Replace("{0}", item)).FirstOrDefault();

                    textBox1.Text = res.Replace($"public class {schema}.{table}", $"public class {table}");

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

