using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using System.Data.OleDb;

namespace автосалон_дизайн
{
    public partial class autorization : KryptonForm
    {
        OleDbConnection conn;
        OleDbDataAdapter adapter;
        DataTable dt;

        void GetData()
        {
            conn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.16.0; Data Source=auto_bd.accdb");
            dt = new DataTable();
            adapter = new OleDbDataAdapter("SELECT * FROM Clients", conn);
            conn.Open();
            adapter.Fill(dt);
            conn.Close();
        }
        public autorization()
        {
            InitializeComponent();
        }

        private void autorization_Load(object sender, EventArgs e)
        {
            GetData();
        }

        private void btnAutorize_Click(object sender, EventArgs e)
        {
            GetData();
            conn.Open();
            DataView dv = dt.DefaultView;
            dv.RowFilter = "acc_pass LIKE '" + txtPass.Text + "' AND acc_login LIKE '" + txtLogin.Text +
                "' AND acc_type LIKE 'admin'";
            if (dv.Count != 0)
            {
                txtLogin.Clear();
                txtPass.Clear();
                admin adminf = new admin();
                adminf.Owner = this;
                adminf.Show();
                this.Hide();
            }

            else
            {
                dv.RowFilter = "acc_pass LIKE '" + txtPass.Text + "' AND acc_login LIKE '" + txtLogin.Text +
                    "' AND acc_type LIKE 'user'";
                if (dv.Count != 0)
                {
                    
                    txtLogin.Clear();
                    txtPass.Clear();
                    user usform = new user();
                    usform.Owner = this;
                    usform.Show();
                    this.Hide();
                }
                else
                {
                    label2.Visible = true;
                    txtLogin.Clear();
                    txtPass.Clear();
                }

            }
            conn.Close();
        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            registration regform = new registration();
            regform.Owner = this;
            regform.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtLogin_Enter(object sender, EventArgs e)
        {
            
            txtLogin.Clear();
            label2.Visible = false;
        }

        private void txtPass_Enter(object sender, EventArgs e)
        {
            txtPass.Clear();
            label2.Visible = false;
        }
    }
}
