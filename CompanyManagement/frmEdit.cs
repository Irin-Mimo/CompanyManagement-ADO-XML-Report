using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CompanyManagement
{

    public partial class frmEdit : Form
    {
        List<ProductDetails>details = new List<ProductDetails>();
        string currentFile = "";
        string oldFile = "";
        public frmEdit()
        {
            InitializeComponent();
        }
        public frmShow TheForm { get; set; }
        public int IdToEdit { get; set; }

        private void frmEdit_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadInform();
        }

        private void LoadInform()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Companies WHERE companyId=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtCompanyName.Text = dr.GetString(1);
                        txtCategory.Text = dr.GetString(2);
                        txtPhone.Text = dr.GetString(3);
                        txtEmail.Text = dr.GetString(4);
                        dtpEstablishedDate.Value = dr.GetDateTime(5).Date;
                        
                        checkBox1.Checked = dr.GetBoolean(6);
                        pbLogo.Image = Image.FromFile(@"..\..\Images\" + dr.GetString(7));
                        oldFile = dr.GetString(7);
                    }
                    dr.Close();

                    cmd.CommandText = "SELECT * FROM Products WHERE companyId=@i";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    while (dr2.Read())
                    {
                        details.Add(new ProductDetails
                        {
                            productName = dr2.GetString(1),
                            price=dr2.GetDecimal(2),
                            stockQuantity = dr2.GetInt32(3),
                            releaseDate = dr2.GetDateTime(4).Date,
            
                        });
                    }
                    StDataSource();
                    con.Close();
                }
            }
        }

        private void StDataSource()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                details.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = "";
                dataGridView1.DataSource = details;

            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                pbLogo.Image = Image.FromFile(currentFile);
            }
        }

       

        private void btnAdd_Click(object sender, EventArgs e)
        {
            details.Add(new ProductDetails
            {
                productName = txtProductName.Text,
                price = nudPrice.Value,
                stockQuantity = (int)nudStockQuantity.Value,
                releaseDate = dtpReleaseDate.Value.Date,

            });
            dataGridView1.DataSource = "";
            dataGridView1.DataSource = details;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;
                        string f = oldFile;
                        if (currentFile != "")
                        {
                            //for image
                            string ext = Path.GetExtension(currentFile);
                             f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                            string savePath = @"..\..\Images\" + f;
                            MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                            byte[] bytes = ms.ToArray();
                            FileStream fs = new FileStream(savePath, FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }
                        cmd.CommandText = "UPDATE Companies SET companyName=@cn,category=@c,phone=@p,email=@e,establishedDate=@ed,insideDhaka=@isd,logo=@logo WHERE companyId=@i";
                        cmd.Parameters.AddWithValue("@i", IdToEdit);
                        cmd.Parameters.AddWithValue("@cn", txtCompanyName.Text);

                        cmd.Parameters.AddWithValue("@c", txtCategory.Text);

                        cmd.Parameters.AddWithValue("@p", txtPhone.Text);

                        cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@ed", dtpEstablishedDate.Value);

                        cmd.Parameters.AddWithValue("@isd", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@logo", f);

                        try
                        {
                            //delete
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "DELETE FROM Products WHERE companyId=@id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();

                            //add
                            foreach (var s in details)
                            {
                                cmd.CommandText = "INSERT INTO Products(productName,price,stockQuantity,releaseDate,companyId) VALUES(@pn, @p, @sq, @rd, @i)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@pn", s.productName);
                                cmd.Parameters.AddWithValue("@p", s.price);
                                cmd.Parameters.AddWithValue("@sq", s.stockQuantity);
                                cmd.Parameters.AddWithValue("@rd", s.releaseDate);
                                cmd.Parameters.AddWithValue("@i", IdToEdit);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            MessageBox.Show("Data Updated successfully!!");
                            TheForm.LoadDataBindingSource();
                            AllClear();

                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                    con.Close();
            }
         
        }

        public void AllClear()
        {
            details.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
            txtCompanyName.Clear();
            txtCategory.Clear();
            dtpEstablishedDate.Value = DateTime.Now;
            txtPhone.Clear();
            txtEmail.Clear();
            checkBox1.Checked = false;
            pbLogo.Image = Image.FromFile(@"..\..\Images\logo.png");
            txtProductName.Clear();
            nudPrice.Value = 0;
            nudStockQuantity.Value = 0;
            dtpReleaseDate.Value = DateTime.Now;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    string sql = @"DELETE FROM Products where companyId=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, con, trx))
                    {
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            cmd.CommandText = "DELETE FROM Companies WHERE companyId=@id";
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            trx.Commit();
                            MessageBox.Show("Data Deleted Successfully!!");
                            AllClear();
                            TheForm.LoadDataBindingSource();

                        }
                        catch
                        {
                            trx.Rollback();
                            MessageBox.Show("Failed to Delete Data !!");
                        }
                        con.Close();
                    }
                }
            }
        }
    }
}
