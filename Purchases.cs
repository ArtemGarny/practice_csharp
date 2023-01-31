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
    public partial class Purchases : KryptonForm
    {
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.12.0; Data Source=auto_bd.accdb");
        OleDbCommand cmd;
        OleDbCommand cmd1;
        OleDbDataAdapter adapter;
        DataTable dt;
        OleDbDataAdapter adapter2;
        DataTable dt2;
        Regex r2 = new Regex(@"^[0-9]{2}.[0-9]{2}.[0-9]{4}$");
        Regex r3 = new Regex(@"^[0-9]*$");

        public Purchases()
        {
            InitializeComponent();
        }

        void GetData()
        {
            conn.Open();
            dt = new DataTable();
            adapter = new OleDbDataAdapter("SELECT * FROM Purchases", conn);
            adapter.Fill(dt);
            dgwPurchases.DataSource = dt;
            conn.Close();
            dgwPurchases.Sort(dgwPurchases.Columns[0], ListSortDirection.Ascending);
        }

        void GetData2()
        {
            conn.Open();
            dt2 = new DataTable();
            adapter2 = new OleDbDataAdapter("SELECT * FROM Cars", conn);
            adapter2.Fill(dt2);
            conn.Close();
        }

        private void Purchases_Load(object sender, EventArgs e)
        {
            GetData();
            GetData2();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtMarka.Text == "" || txtModel.Text == "" || txtClient_ID.Text == "" || txtPrice.Text == "" || txtAmount.Text == "" || txtPurchase_ID.Text == "" || txtDate.Text == "")
            {
                DialogResult dr = MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else if (r3.IsMatch(txtAmount.Text) != true || r2.IsMatch(txtDate.Text) != true || r3.IsMatch(txtPrice.Text) != true)
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
                
                else
                {
                    int price = Convert.ToInt32(datrow[0][3].ToString());
                    int amount = Convert.ToInt32(datrow[0][5].ToString()) - Convert.ToInt32(txtAmount.Text);
                    string querry = "INSERT INTO Purchases(Client_ID, Marka, Model, Amount, Purchase_Date, Price) VALUES (@clid,@marka,@model,@amount,@datapok, @price)";
                    cmd = new OleDbCommand(querry, conn);

                    cmd.Parameters.AddWithValue("@clid", txtClient_ID.Text);
                    cmd.Parameters.AddWithValue("@marka", txtMarka.Text);
                    cmd.Parameters.AddWithValue("@model", txtModel.Text);
                    cmd.Parameters.AddWithValue("@amount", txtAmount.Text);
                    cmd.Parameters.AddWithValue("@datapok", txtDate.Text);
                    cmd.Parameters.AddWithValue("@price", price);

                    if (Convert.ToInt32(txtAmount.Text) <= 0)
                    {
                        DialogResult dr = MessageBox.Show("Вы должны продать хотя бы одну машину !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                    }
                    else
                    {
                        if (datrow.Length != 0)
                        {

                            if (amount >= 0)
                            {
                                string querry1 = "Update Cars Set Amount= " + amount + " Where  Marka LIKE '" + txtMarka.Text + "' AND Model LIKE '" + txtModel.Text + "'";
                                cmd1 = new OleDbCommand(querry1, conn);
                                cmd1.ExecuteNonQuery();
                                cmd.ExecuteNonQuery();


                            }
                            else if (amount < 0)
                            { DialogResult dr = MessageBox.Show("В автосалоне недостаточно авто данной модели !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1); }
                        }

                        else
                        {
                            DialogResult dr = MessageBox.Show("Данного автомобиля нет в базе !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
            if (txtPurchase_ID.Text == "")
            {
                DialogResult dr = MessageBox.Show("Введите ID !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = "Purchase_ID = '" + txtPurchase_ID.Text + "'";
                if (dv.Count != 0)
                {
                    DialogResult dr = MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (dr != DialogResult.Cancel)
                    {
                        DataRow[] datrow = dt2.Select($"Marka='{txtMarka.Text}' and Model='{txtModel.Text}'");
                        if (datrow.Length == 0)
                        {
                            DialogResult dr2 = MessageBox.Show("Автомобиль не найден !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }
                        else
                        {
                            conn.Open();
                            int amount = Convert.ToInt32(datrow[0][5].ToString()) + Convert.ToInt32(txtAmount.Text);
                            string querry1 = "Update Cars Set Amount= " + amount + " Where  Marka LIKE '" + txtMarka.Text + "' AND Model LIKE '" + txtModel.Text + "'";
                            cmd1 = new OleDbCommand(querry1, conn);
                            cmd1.ExecuteNonQuery();
                            
                            string querry = "DELETE FROM Purchases WHERE Purchase_ID=@id";
                            cmd = new OleDbCommand(querry, conn);
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtPurchase_ID.Text));
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                else
                {
                    DialogResult dr = MessageBox.Show("Покупка не найдена !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            GetData();
        }

        private void dgwPurchases_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtPurchase_ID.Text = dgwPurchases.CurrentRow.Cells[0].Value.ToString();
            txtClient_ID.Text = dgwPurchases.CurrentRow.Cells[1].Value.ToString();
            txtMarka.Text = dgwPurchases.CurrentRow.Cells[2].Value.ToString();
            txtModel.Text = dgwPurchases.CurrentRow.Cells[3].Value.ToString();
            txtAmount.Text = dgwPurchases.CurrentRow.Cells[4].Value.ToString();
            txtDate.Text = dgwPurchases.CurrentRow.Cells[5].Value.ToString();
            txtPrice.Text = dgwPurchases.CurrentRow.Cells[6].Value.ToString();
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
                + "OR Purchase_Date LIKE '%" + txtSearch.Text + "%'";
            dgwPurchases.DataSource = dv;
        }

        private void btnDoxod_Click(object sender, EventArgs e)
        {
            double doxod = 0;
            foreach (DataGridViewRow row in dgwPurchases.Rows)
            {
                double incom;

                incom = Convert.ToInt32(row.Cells[4].Value) * Convert.ToInt32(row.Cells[6].Value);
                doxod += incom;
            }
            txtDoxod.Text = doxod.ToString();
        }

        private void Purchases_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }
    }
}
