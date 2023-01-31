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
    public partial class Providers : KryptonForm
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
            adapter = new OleDbDataAdapter("SELECT * FROM Providers", conn);
            conn.Open();
            adapter.Fill(dt);
            dgwProviders.DataSource = dt;
            conn.Close();
        }

        public Providers()
        {
            InitializeComponent();
        }

        private void Providers_Load(object sender, EventArgs e)
        {
            GetData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtProvider_ID.Text == "" || txtName.Text == "" || txtOtchestvo.Text == "" || txtSurname.Text == "")
            {
                DialogResult dr = MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else if (r.IsMatch(txtName.Text) != true || r.IsMatch(txtSurname.Text) != true || r.IsMatch(txtOtchestvo.Text) != true)
            {
                DialogResult dr = MessageBox.Show("Ошибка формата введенных данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                string querry = "INSERT INTO Providers(Familia, Imya, Otchestvo) VALUES" +
                "(@Surname,@Imya,@Otchestvo)";
                cmd = new OleDbCommand(querry, conn);

                cmd.Parameters.AddWithValue("@Surname", txtSurname.Text);
                cmd.Parameters.AddWithValue("@Imya", txtName.Text);
                cmd.Parameters.AddWithValue("@Otchestvo", txtOtchestvo.Text);

                conn.Open();
                DataView dv = dt.DefaultView;
                dv.RowFilter = "Familia LIKE '" + txtSurname.Text + "' AND Imya LIKE '" + txtName.Text + "' AND Otchestvo LIKE '" + txtOtchestvo.Text + "'";
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtProvider_ID.Text == "" || txtName.Text == "" || txtOtchestvo.Text == "" || txtSurname.Text == "")
            {
                DialogResult dr = MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else if (r.IsMatch(txtName.Text) != true || r.IsMatch(txtSurname.Text) != true || r.IsMatch(txtOtchestvo.Text) != true)
            {
                DialogResult dr = MessageBox.Show("Ошибка формата введенных данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                string querry = "UPDATE Providers SET Familia=@Surname, Imya=@Imya, Otchestvo=@Otchestvo " +
                "Where Provider_ID=@id";
                cmd = new OleDbCommand(querry, conn);

                cmd.Parameters.AddWithValue("@Surname", txtSurname.Text);
                cmd.Parameters.AddWithValue("@Imya", txtName.Text);
                cmd.Parameters.AddWithValue("@Otchestvo", txtOtchestvo.Text);
                cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtProvider_ID.Text));
                conn.Open();
                DataView dv = dt.DefaultView;
                dv.RowFilter = "Familia LIKE '" + txtSurname.Text + "' AND Imya LIKE '" + txtName.Text + "' AND Otchestvo LIKE '" + txtOtchestvo.Text + "'";
                if (dv.Count != 0)
                {
                    DialogResult dr = MessageBox.Show("Такой человек уже есть в базе", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }

                else
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                GetData();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (txtProvider_ID.Text == "" || r2.IsMatch(txtProvider_ID.Text)!=true)
            {
                DialogResult dr = MessageBox.Show("Введите ID !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = "Provider_ID = '"+txtProvider_ID.Text+"'";
                if (dv.Count != 0)
                {
                    DialogResult dr = MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (dr != DialogResult.Cancel)
                    {
                        string querry = "DELETE FROM Providers WHERE Provider_ID=@id";
                        cmd = new OleDbCommand(querry, conn);
                        cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtProvider_ID.Text));
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        
                    }
                }
                else
                {
                    DialogResult dr = MessageBox.Show("Поставщик не найден !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
            GetData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = "Familia LIKE '%" + txtSearch.Text + "%' OR Imya LIKE '%" + txtSearch.Text + "%' OR Otchestvo LIKE '%" + txtSearch.Text + "%'";
            dgwProviders.DataSource = dv;
        }

        private void dgwProviders_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtProvider_ID.Text = dgwProviders.CurrentRow.Cells[0].Value.ToString();
            txtSurname.Text = dgwProviders.CurrentRow.Cells[1].Value.ToString();
            txtName.Text = dgwProviders.CurrentRow.Cells[2].Value.ToString();
            txtOtchestvo.Text = dgwProviders.CurrentRow.Cells[3].Value.ToString();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Owner.Show();
            this.Close();
        }

        private void Providers_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }
    }
}
