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
using System.Text.RegularExpressions;

namespace автосалон_дизайн
{
    public partial class Cars : KryptonForm
    {
        OleDbConnection conn;
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        DataTable dt;
        Regex r3 = new Regex(@"^[0-9]*$");

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

        public Cars()
        {
            InitializeComponent();
        }

        private void Cars_Load(object sender, EventArgs e)
        {
            GetData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtMarka.Text == "" || txtModel.Text == "" || txtCountry.Text == "" || txtPrice.Text == "" )
            {
                DialogResult dr = MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else if (r3.IsMatch(txtAmount.Text) != true ||  r3.IsMatch(txtPrice.Text) != true)
            {
                DialogResult dr = MessageBox.Show("Ошибка формата введенных данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                string querry = "INSERT INTO Cars( Marka, Model, Price, Country) VALUES (@marka,@model,@price,@country)";
                cmd = new OleDbCommand(querry, conn);
                cmd.Parameters.AddWithValue("@marka", txtMarka.Text);
                cmd.Parameters.AddWithValue("@model", txtModel.Text);
                cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                cmd.Parameters.AddWithValue("@country", txtCountry.Text);
                conn.Open();

                DataView dv = dt.DefaultView;
                dv.RowFilter = "Marka LIKE '" + txtMarka.Text + "' AND Model LIKE '" + txtModel.Text + "'";
                if (dv.Count != 0)
                {
                    DialogResult dr = MessageBox.Show("Данный автомобиль уже есть в базе", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    GetData();
                }

                else
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    GetData();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtMarka.Text == "" || txtModel.Text == "" || txtCountry.Text == "" || txtPrice.Text == "" )
            {
                DialogResult= MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                string querry = "Update Cars Set Marka=@marka,Model=@model,Price=@price,Country=@country " +
                "Where Car_ID=@carid";
                cmd = new OleDbCommand(querry, conn);

                cmd.Parameters.AddWithValue("@marka", txtMarka.Text);
                cmd.Parameters.AddWithValue("@model", txtModel.Text);
                cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                cmd.Parameters.AddWithValue("@country", txtCountry.Text);
                cmd.Parameters.AddWithValue("@carid", Convert.ToInt32(txtCar_ID.Text));
                conn.Open();


                DataView dv = dt.DefaultView;
                dv.RowFilter = "Marka LIKE '" + txtMarka.Text + "' AND Model LIKE '" + txtModel.Text + "'";
                if (dv.Count != 0)
                {
                    DialogResult dr = MessageBox.Show("Данный автомобиль уже есть в базе", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    GetData();
                }

                else
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    GetData();
                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (txtCar_ID.Text == "")
            {
                DialogResult dr = MessageBox.Show("Введите ID", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = "Car_ID = '" + txtCar_ID.Text + "'";
                if (dv.Count != 0)
                {
                    DialogResult dr = MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (dr != DialogResult.Cancel)
                    {
                        string querry = "DELETE FROM Cars WHERE Car_ID=@id";
                        cmd = new OleDbCommand(querry, conn);
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtCar_ID.Text));
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        
                    }
                }
                else
                {
                    DialogResult dr = MessageBox.Show("Машина не найдена !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            GetData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = "Marka LIKE '%" + txtSearch.Text + "%' OR Model LIKE '%" + txtSearch.Text + "%'"
                + "OR Country LIKE '%" + txtSearch.Text + "%'";
            dgwCars.DataSource = dv;
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

        private void Cars_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }
    }
}
