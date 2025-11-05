using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompanyManagement
{
    public partial class frmAddEmployee : Form
    {
        string conString = "Data Source=DESKTOP-IOJGJEN;Initial Catalog=CompanyManagement;Trusted_Connection=True";
        SqlConnection con;
        SqlCommand cmd;
        string employeeId = "";
        public frmAddEmployee()
        {
            InitializeComponent();
            con = new SqlConnection(conString);
            con.Open();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please Enter Employee Name!!");
                txtName.Select();
            }
            else if (string.IsNullOrWhiteSpace(txtCity.Text))
            {
                MessageBox.Show("Please Enter City Name!!");
                txtCity.Select();
            }
            else if (string.IsNullOrWhiteSpace(txtDepartment.Text))
            {
                MessageBox.Show("Please Enter Department Name!!");
                txtDepartment.Select();
            }
            else if (comboBox1.SelectedIndex<=-1)
            {
                MessageBox.Show("Select Gender!!");
                comboBox1.Select();
            }
            else
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    DataTable dt = new DataTable();
                    cmd = new SqlCommand("spEmployee", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@actionType", "SaveData");
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    cmd.Parameters.AddWithValue("@name", txtName.Text); 
                    cmd.Parameters.AddWithValue("@city", txtCity.Text); 
                    cmd.Parameters.AddWithValue("@department", txtDepartment.Text);
                    cmd.Parameters.AddWithValue("@gender", comboBox1.Text);
                    int runRes = cmd.ExecuteNonQuery();
                    if(runRes>0)
                    {
                        MessageBox.Show("Data Saved Successfully!!");
                        dataGridView1.DataSource = LoadData();
                        ClearAll();
                    }

                    else
                    {
                        MessageBox.Show("Please try again!!");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:-ex.massage");
                }
            }

        }

        private void ClearAll()
        {
            txtName.Clear();
            txtCity.Clear();
            txtDepartment.Clear();
            comboBox1.SelectedIndex = -1;
            txtName.Focus();
            btnSave.Text = "Save";
        }

        private void frmAddEmployee_Load(object sender, EventArgs e)
        {
           dataGridView1.DataSource= LoadData();

        }

        private DataTable LoadData()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            DataTable dt = new DataTable();
            cmd = new SqlCommand("spEmployee", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@actionType", "GetAllData");
            cmd.Parameters.AddWithValue("@employeeId", employeeId);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            return dt;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSave.Text = "Update";
                employeeId = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                DataTable dt = LoadRecordById(employeeId);
                if (dt.Rows.Count > 0)
                {
                    employeeId = dt.Rows[0][0].ToString();
                    txtName.Text = dt.Rows[0][1].ToString();
                    txtCity.Text = dt.Rows[0][2].ToString();
                    txtDepartment.Text = dt.Rows[0][3].ToString();
                    comboBox1.Text = dt.Rows[0][4].ToString();
                }
                else
                {
                    ClearAll();
                }
            }
        }
        private DataTable LoadRecordById(string empId)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            DataTable dt = new DataTable();
            cmd = new SqlCommand("spEmployee", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@actionType", "GetDataById");
            cmd.Parameters.AddWithValue("@employeeId", employeeId);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            return dt;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(employeeId))
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    DataTable dt = new DataTable();
                    cmd = new SqlCommand("spEmployee", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@actionType", "DeleteData");
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    int numRes = cmd.ExecuteNonQuery();
                    if (numRes > 0)
                    {
                        MessageBox.Show("Data deleted!!");
                        dataGridView1.DataSource = LoadData();
                        ClearAll();
                    }
                    else
                    {
                        MessageBox.Show("Please try again!!");

                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Eror:- " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a record!!");
            }

        }
    }
}
