using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompanyManagement
{
    public partial class frmBuyerInfo : Form
    {
        public frmBuyerInfo()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(openFileDialog1.FileName);
                this.pbBuyerPic.Image = img;
                this.txtPicture.Text = openFileDialog1.FileName;
            }
        }

        private void frmBuyerInfo_Load(object sender, EventArgs e)
        {
            LoadGride();
            GenerateAutoId();
            LoadCombo();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column is DataGridViewImageColumn imageColumn)
                {
                    imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                }
            }

            // Set the row height for proper image display
            dataGridView1.RowTemplate.Height = 30; // Adjust this value as needed
        }

        private void LoadCombo()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT productItemId,productItemName FROM productItem", con);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                comboBox1.DataSource = ds.Tables[0];
                comboBox1.DisplayMember = "productItemName";
                comboBox1.ValueMember = "productItemId";
                con.Close();
            }
        }

        private void GenerateAutoId()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(buyerId),0)+1 FROM buyer", con))
                {
                    con.Open();
                    int newId = (int)cmd.ExecuteScalar();
                    lblBuyerId.Text = newId.ToString();
                    con.Close();
                }
            }
        }

        private void LoadGride()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using(SqlDataAdapter sda=new SqlDataAdapter("SELECT buyerId,buyerName,buyerContact,buyerEmail,picture FROM buyer", con))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                //for Image
                Image img = Image.FromFile(txtPicture.Text);
                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);

                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO buyer(buyerId,buyerName,buyerContact,buyerEmail,picture,productItemId) VALUES(@bi,@bn,@bc,@be,@pic,@pi)", con)) 
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@bi", lblBuyerId.Text);


                    cmd.Parameters.AddWithValue("@bn", txtName.Text);


                    cmd.Parameters.AddWithValue("@bc", txtContact.Text);


                    cmd.Parameters.AddWithValue("@be", txtEmail.Text);


                    cmd.Parameters.Add(new SqlParameter("@pic", SqlDbType.VarBinary) { Value = ms.ToArray() });

                    cmd.Parameters.AddWithValue("@pi", comboBox1.SelectedValue);
                    if(cmd.ExecuteNonQuery()>0)
                    {
                        MessageBox.Show("Data inserted successfully!!");
                        LoadGride();
                    }
                    con.Close();
                }
            }
        }


        
    }
}
