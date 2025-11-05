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
    public partial class frmAddCompany : Form
    {
        List<ProductDetails> details = new List<ProductDetails>();
        string currentFile = "";
        //DataSet ds;
        public frmAddCompany()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                pbLogo.Image = Image.FromFile(currentFile);
            }
        }


        private void frmAddCompany_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            //LoadGrid();
        }

        //private void LoadGrid()
        //{
        //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
        //    {
        //        using (SqlDataAdapter sda = new SqlDataAdapter("Select productId,productName,price,stockQuantity,releaseDate,picture from Products", con))
        //        {
        //            ds = new DataSet();
        //            sda.Fill(ds, "Products"); ds.Tables["Products"].Columns.Add(new DataColumn("img", typeof(byte[])));
        //            for (int i = 0; i < ds.Tables["Products"].Rows.Count; i++)
        //            {
        //                ds.Tables["Products"].Rows[i]["img"] = File.ReadAllBytes(Path.Combine(@"..\..\Images", ds.Tables["Products"].Rows[i]["picture"].ToString()));
        //            }
        //            dataGridView1.DataSource = ds.Tables["Products"];
        //        }
        //    }
        //}


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

            productClearAfterAdd();
        }

        private void productClearAfterAdd()
        {
            txtProductName.Clear();
            nudPrice.Value = 0;
            nudStockQuantity.Value = 0;
            dtpReleaseDate.Value = DateTime.Now;
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==4)
            {
                details.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = "";
                dataGridView1.DataSource = details;

            }

        }

        private void btnSaveAll_Click(object sender, EventArgs e)
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
                        //for image
                        string ext = Path.GetExtension(currentFile);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = @"..\..\Images\" + f;
                        MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                        byte[] bytes = ms.ToArray();
                        FileStream fs = new FileStream(savePath, FileMode.Create);
                        fs.Write(bytes, 0, bytes.Length);
                        //fs.Close();

                        //for another Data
                        cmd.CommandText = "Insert Into Companies(companyName,category,phone,email,establishedDate,insideDhaka,logo) values(@cn,@c,@p,@e,@ed,@ind,@logo) SELECT SCOPE_IDENTITY()";
                        cmd.Parameters.AddWithValue("@cn", txtCompanyName.Text);
                        cmd.Parameters.AddWithValue("@c", txtCategory.Text);
                        cmd.Parameters.AddWithValue("@p", nudPrice.Value);
                        cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@ed", dtpEstablishedDate.Value.Date);
                        cmd.Parameters.AddWithValue("@ind", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@logo", f);
                        try
                        {
                            var cId = cmd.ExecuteScalar();
                            foreach (var s in details)
                            {
                                cmd.CommandText = "Insert Into Products(productName,price,stockQuantity,releaseDate, companyId) values(@pn,@p,@sq,@rd,@i) SELECT SCOPE_IDENTITY()";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@pn", s.productName);
                                cmd.Parameters.AddWithValue("@p", s.price);
                                cmd.Parameters.AddWithValue("@sq", s.stockQuantity);
                                cmd.Parameters.AddWithValue("@rd", s.releaseDate);
                                cmd.Parameters.AddWithValue("@i", cId);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            MessageBox.Show("Data saved successfully!!!");
                            AllClearAfterSaveAll();
                        }
                        catch
                        {
                            trx.Rollback();
                        }
                        fs.Close();
                    }
                }
            }
        }

        private void AllClearAfterSaveAll()
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
    }

    public class ProductDetails
    {
        public string productName { get; set; }
        public decimal price { get; set; }
        public int stockQuantity { get; set; }
        public DateTime releaseDate { get; set; }
        //public Image picture { get; set; }


    }
}
