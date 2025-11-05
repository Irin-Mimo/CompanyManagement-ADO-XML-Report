using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompanyManagement
{
    public partial class homePage : Form
    {
        public homePage()
        {
            InitializeComponent();
        }

        private void addNewCompanyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddCompany fac = new frmAddCompany();
            fac.Show();
        }

        private void showEditDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShow fs = new frmShow();
            fs.Show();
        }

        private void companyProductionReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            productionReport pr = new productionReport();
            pr.Show();
        }

        private void addEmployeeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddEmployee ae = new frmAddEmployee();
            ae.Show();
        }

        private void buyerInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBuyerInfo bi = new frmBuyerInfo();
            bi.Show();
        }

        private void buyerReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buyerReport br = new buyerReport();
            br.Show();

        }
    }
}
