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

namespace CompanyManagement
{
    public partial class frmShow : Form
    {
        BindingSource bsC = new BindingSource();
        BindingSource bsP = new BindingSource();
        DataSet ds;
        public frmShow()
        {
            InitializeComponent();
        }

        private void frmShow_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadDataBindingSource();
        }

        public void LoadDataBindingSource()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM Companies", con))
                {
                    ds = new DataSet();
                    sda.Fill(ds, "Companies");
                    sda.SelectCommand.CommandText = "SELECT * FROM Products";
                    sda.Fill(ds, "Products");
                    ds.Tables["Companies"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for (int i = 0; i < ds.Tables["Companies"].Rows.Count; i++)
                    {
                        ds.Tables["Companies"].Rows[i]["image"] = File.ReadAllBytes($@"..\..\Images\{ds.Tables["Companies"].Rows[i]["logo"]}");
                    }
                        DataRelation rel = new DataRelation("FK_S_S", ds.Tables["Companies"].Columns["companyId"], ds.Tables["Products"].Columns["companyId"]);
                        ds.Relations.Add(rel);
                        bsC.DataSource = ds;
                        bsC.DataMember = "Companies";

                        bsP.DataSource = bsC;
                        bsP.DataMember = "FK_S_S";
                        dataGridView1.DataSource = bsP;
                        AddDataBindings();
                }
            }
        }
        

        private void AddDataBindings()
        {
            lblCompanyId.DataBindings.Clear();
            lblCompanyId.DataBindings.Add("Text", bsC, "companyId");

            lblCompanyName.DataBindings.Clear();
            lblCompanyName.DataBindings.Add("Text", bsC, "companyName");

            lblCategory.DataBindings.Clear();
            lblCategory.DataBindings.Add("Text", bsC, "category");

            lblPhone.DataBindings.Clear();
            lblPhone.DataBindings.Add("Text", bsC, "phone");

            lblEmail.DataBindings.Clear();
            lblEmail.DataBindings.Add("Text", bsC, "email");

            lblEstablishedDate.DataBindings.Clear();
            lblEstablishedDate.DataBindings.Add("Text", bsC, "establishedDate");

            Binding bm = new Binding("Text", bsC, "establishedDate", true);
            bm.Format += Bm_Format;
            lblEstablishedDate.DataBindings.Clear();
            lblEstablishedDate.DataBindings.Add(bm);

            cbIsD.DataBindings.Clear();
            cbIsD.DataBindings.Add("Checked", bsC, "insideDhaka", true);

            pbLogoShow.DataBindings.Clear();
            pbLogoShow.DataBindings.Add(new Binding("Image", bsC, "image", true));
        }
        private void Bm_Format(object sender, ConvertEventArgs e)
        {
            DateTime d = (DateTime)e.Value;
            e.Value = d.ToString("dd-MM-yyyy");
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            bsC.MoveFirst();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (bsC.Position > 0)
            {
                bsC.MovePrevious();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (bsC.Position < bsC.Count - 1)
            {
                bsC.MoveNext();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            bsC.MoveLast();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int v = int.Parse((bsC.Current as DataRowView).Row[0].ToString());
            new frmEdit
            {
                TheForm = this,
                IdToEdit = v
            }.ShowDialog();
        }
    }
}
