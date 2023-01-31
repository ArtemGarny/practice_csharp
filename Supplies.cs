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
using System.Xml.Linq;
using System.Text.RegularExpressions;


namespace автосалон_дизайн
{
    public partial class Supplies : KryptonForm
    {
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.12.0; Data Source=auto_bd.accdb");
        OleDbCommand cmd;
        OleDbCommand cmd1;
        OleDbDataAdapter adapter;
        DataTable dt;
        OleDbDataAdapter adapter2;
        DataTable dt2;
        Regex r = new Regex(@"^[А-ЯЁ][а-яё]*$");
        Regex r2 = new Regex(@"^[0-9]{2}.[0-9]{2}.[0-9]{4}$");
        Regex r3 = new Regex(@"^[0-9]*$");

        public Supplies()
        {
            InitializeComponent();
        }

        void GetData()
        {
            conn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.12.0; Data Source=auto_bd.accdb");
            dt = new DataTable();
            adapter = new OleDbDataAdapter("SELECT * FROM Supplies", conn);
            conn.Open();
            adapter.Fill(dt);
            dgwSupplies.DataSource = dt;
            conn.Close();
            dgwSupplies.Sort(dgwSupplies.Columns[0], ListSortDirection.Ascending);
        }

        void GetData2()
        {

            dt2 = new DataTable();
            adapter2 = new OleDbDataAdapter("SELECT * FROM Cars", conn);
            conn.Open();
            adapter2.Fill(dt2);

            conn.Close();
        }

        private void Supplies_Load(object sender, EventArgs e)
        {
            GetData();
            GetData2();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtMarka.Text == "" || txtModel.Text == "" || txtProvider_ID.Text == "" || txtPrice.Text == "" || txtAmount.Text == "" || txtSupply_ID.Text == ""||txtDate.Text=="")
            {
                DialogResult dr = MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else if(r3.IsMatch(txtAmount.Text) != true || r2.IsMatch(txtDate.Text) != true || r3.IsMatch(txtPrice.Text) != true)
            {
                DialogResult dr = MessageBox.Show("Ошибка формата введенных данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                conn.Open();
                DataRow[] datrow = dt2.Select($"Marka='{txtMarka.Text}' and Model='{txtModel.Text}'");
                if (datrow.Length == 0)
                {
                    DialogResult dr = MessageBox.Show("Автомобиль не найден !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                else if (Convert.ToDouble(txtAmount.Text) % 1 != 0)
                {
                    DialogResult dr = MessageBox.Show("Количество машин вводите целыми числами !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    int price = Convert.ToInt32(datrow[0][3].ToString());
                    int amount = Convert.ToInt32(datrow[0][5].ToString()) + Convert.ToInt32(txtAmount.Text);
                    string querry = "INSERT INTO Supplies(Provider_ID,Marka, Model, Amount, Supply_Date, Price) VALUES (@provid,@marka,@model,@amount,@datapost,@price)";
                    cmd = new OleDbCommand(querry, conn);

                    cmd.Parameters.AddWithValue("@provid", txtProvider_ID.Text);
                    cmd.Parameters.AddWithValue("@marka", txtMarka.Text);
                    cmd.Parameters.AddWithValue("@model", txtModel.Text);
                    cmd.Parameters.AddWithValue("@amount", txtAmount.Text);
                    cmd.Parameters.AddWithValue("@datapost", txtDate.Text);
                    cmd.Parameters.AddWithValue("@price", price);

                    if (Convert.ToInt32(txtAmount.Text) <= 0)
                    {
                        DialogResult dr = MessageBox.Show("Вы должны получить хотя бы одну машину !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                    }
                    else
                    {
                        if (datrow.Length != 0)
                        {

                            if (amount <= 20)
                            {
                                string querry1 = "Update Cars Set Amount= " + amount + " Where  Marka LIKE '" + txtMarka.Text + "' AND Model LIKE '" + txtModel.Text + "'";
                                cmd1 = new OleDbCommand(querry1, conn);

                                cmd1.ExecuteNonQuery();
                                cmd.ExecuteNonQuery();


                            }
                            else if (amount >= 20)
                            { DialogResult dr = MessageBox.Show("В автосалоне нет столько места !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1); }
                        }

                        else
                        {
                            DialogResult dr = MessageBox.Show("Данного автомобиля нет в базе !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            conn.Close();
                            GetData();
                        }

                    }
                }
                conn.Close();
                GetData();
                GetData2();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (txtSupply_ID.Text == "")
            {
                DialogResult dr = MessageBox.Show("Введите ID !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = "Supply_ID = '" + txtSupply_ID.Text + "'";
                if (dv.Count != 0)
                {

                    DialogResult dr = MessageBox.Show("Удалить запись ?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (dr == DialogResult.OK)
                    {
                        DataRow[] datrow = dt2.Select($"Marka='{txtMarka.Text}' and Model='{txtModel.Text}'");
                        int amount = Convert.ToInt32(datrow[0][5].ToString()) - Convert.ToInt32(txtAmount.Text);
                        if (datrow.Length == 0)
                        {
                            DialogResult dr2 = MessageBox.Show("Автомобиль не найден !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }
                        else if(amount<0)
                        {
                            DialogResult dr2 = MessageBox.Show("Нельзя отменить ! Авто уже проданы", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }
                        else
                        {
                            conn.Open();
                            
                            string querry1 = "Update Cars Set Amount= " + amount + " Where  Marka LIKE '" + txtMarka.Text + "' AND Model LIKE '" + txtModel.Text + "'";
                            cmd1 = new OleDbCommand(querry1, conn);
                            cmd1.ExecuteNonQuery();
                            string querry = "DELETE FROM Supplies WHERE Supply_ID=@id";
                            cmd = new OleDbCommand(querry, conn);
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtSupply_ID.Text));
                            cmd.ExecuteNonQuery();
                            conn.Close();

                        }
                    }
                }
                else
                {
                    DialogResult dr = MessageBox.Show("Поставка не найдена !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            GetData();
        }

        private void dgwSupplies_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtSupply_ID.Text = dgwSupplies.CurrentRow.Cells[0].Value.ToString();
            txtProvider_ID.Text = dgwSupplies.CurrentRow.Cells[1].Value.ToString();
            txtMarka.Text = dgwSupplies.CurrentRow.Cells[2].Value.ToString();
            txtModel.Text = dgwSupplies.CurrentRow.Cells[3].Value.ToString();
            txtAmount.Text = dgwSupplies.CurrentRow.Cells[4].Value.ToString();
            txtDate.Text = dgwSupplies.CurrentRow.Cells[5].Value.ToString();
            txtPrice.Text = dgwSupplies.CurrentRow.Cells[6].Value.ToString();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = "Marka LIKE '%" + txtSearch.Text + "%' OR Model LIKE '%" + txtSearch.Text + "%'"
                + "OR Supply_Date LIKE '%" + txtSearch.Text + "%'";
            dgwSupplies.DataSource = dv;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Owner.Show();
            this.Close();
        }

        private void btnRashod_Click(object sender, EventArgs e)
        {
            double rashod = 0;
            foreach (DataGridViewRow row in dgwSupplies.Rows)
            {
                double price;

                price = Convert.ToInt32(row.Cells[4].Value) * Convert.ToInt32(row.Cells[6].Value);
                rashod += price;
            }
            txtRashod.Text = rashod.ToString();
        }

        private void Supplies_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }
    }
}
