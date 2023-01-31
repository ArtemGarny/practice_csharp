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
    public partial class user : KryptonForm
    {
        OleDbConnection conn;
        OleDbDataAdapter adapter;
        DataTable dt;

        void GetData()
        {
            conn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.12.0; Data Source=auto_bd.accdb");
            dt = new DataTable();
            adapter = new OleDbDataAdapter("SELECT * FROM Cars", conn);
            conn.Open();
            adapter.Fill(dt);
            dgwCars.DataSource = dt;
            conn.Close();
        }

        public user()
        {
            InitializeComponent();
        }

        private void user_Load(object sender, EventArgs e)
        {
            GetData();
        }

        private void dgwCars_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtCar_ID.Text = dgwCars.CurrentRow.Cells[0].Value.ToString();
            txtMarka.Text = dgwCars.CurrentRow.Cells[1].Value.ToString();
            txtModel.Text = dgwCars.CurrentRow.Cells[2].Value.ToString();
            txtPrice.Text = dgwCars.CurrentRow.Cells[3].Value.ToString();
            txtCountry.Text = dgwCars.CurrentRow.Cells[4].Value.ToString();
            txtAmount.Text = dgwCars.CurrentRow.Cells[5].Value.ToString();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Owner.Show();
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = "Marka LIKE '%" + txtSearch.Text + "%' OR Model LIKE '%" + txtSearch.Text + "%'"
                + "OR Country LIKE '%" + txtSearch.Text + "%'";
            dgwCars.DataSource = dv;
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }

        private void user_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }
    }
}
