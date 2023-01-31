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
    public partial class Clients : KryptonForm
    {
        OleDbConnection conn;
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        DataTable dt;
        Regex r = new Regex(@"^[А-ЯЁ][а-яё]*$");
        Regex r2 = new Regex(@"^[0-9]*$");

        void GetData()
        {
            conn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.12.0; Data Source=auto_bd.accdb");
            dt = new DataTable();
            adapter = new OleDbDataAdapter("SELECT * FROM Clients", conn);
            conn.Open();
            adapter.Fill(dt);
            dgwClients.DataSource = dt;
            dgwClients.Sort(dgwClients.Columns[0], ListSortDirection.Ascending);
            conn.Close();
        }

        public Clients()
        {
            InitializeComponent();
        }

        private void Clients_Load(object sender, EventArgs e)
        {
            GetData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtClient_ID.Text == "" || txtName.Text == "" || txtOtchestvo.Text == "" || txtLogin.Text == "" || txtPass.Text == "" || txtSurname.Text == "")
            {
                DialogResult dr = MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else if(r.IsMatch(txtName.Text)!=true|| r.IsMatch(txtSurname.Text) != true|| r.IsMatch(txtOtchestvo.Text) != true)
            {
                DialogResult dr = MessageBox.Show("Ошибка формата введенных данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                string querry = "UPDATE Clients SET Surname=@Surname, Imya=@Imya, Otchestvo=@Otchestvo, acc_login=@log,acc_pass=@pass " +
                "Where Client_ID=@id";
                cmd = new OleDbCommand(querry, conn);

                cmd.Parameters.AddWithValue("@Surname", txtSurname.Text);
                cmd.Parameters.AddWithValue("@Imya", txtName.Text);
                cmd.Parameters.AddWithValue("@Otchestvo", txtOtchestvo.Text);
                cmd.Parameters.AddWithValue("@log", txtLogin.Text);
                cmd.Parameters.AddWithValue("@pass", txtPass.Text);
                cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtClient_ID.Text));

                conn.Open();

                DataView dv = dt.DefaultView;
                dv.RowFilter = "Surname LIKE '" + txtSurname.Text + "' AND Imya LIKE '" + txtName.Text + "' AND Otchestvo LIKE '" + txtOtchestvo.Text + "'";
                if (dv.Count != 0)
                {
                    DialogResult dr = MessageBox.Show("Такой человек уже есть в базе", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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

        private void dgwClients_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtClient_ID.Text = dgwClients.CurrentRow.Cells[0].Value.ToString();
            txtSurname.Text = dgwClients.CurrentRow.Cells[1].Value.ToString();
            txtName.Text = dgwClients.CurrentRow.Cells[2].Value.ToString();
            txtOtchestvo.Text = dgwClients.CurrentRow.Cells[3].Value.ToString();
            txtLogin.Text = dgwClients.CurrentRow.Cells[4].Value.ToString();
            txtPass.Text = dgwClients.CurrentRow.Cells[5].Value.ToString();
            txtType.Text = dgwClients.CurrentRow.Cells[6].Value.ToString();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (txtClient_ID.Text == "" || r2.IsMatch(txtClient_ID.Text) != true)
            {
                DialogResult dr = MessageBox.Show("Введите ID !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = "Client_ID = '" + txtClient_ID.Text + "'";
                if (dv.Count != 0)
                {

                    DialogResult dr = MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (dr != DialogResult.Cancel)
                    {
                        string querry = "DELETE FROM Clients WHERE Client_ID=@id";
                        cmd = new OleDbCommand(querry, conn);
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtClient_ID.Text));
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                    }
                }
                else
                {
                    DialogResult dr = MessageBox.Show("Клиент не найден !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            GetData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = "Surname LIKE '%" + txtSearch.Text + "%' OR Imya LIKE '%" + txtSearch.Text + "%' OR Otchestvo LIKE '%" + txtSearch.Text + "%'";
            dgwClients.DataSource = dv;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Owner.Show();
            this.Close();
        }

        private void Clients_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }
    }
}
