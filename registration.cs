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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;

namespace автосалон_дизайн
{
    public partial class registration : KryptonForm
    {
        OleDbConnection conn;
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        DataTable dt;
        Regex r = new Regex(@"^[А-ЯЁ][а-яё]*$");

        void GetData()
        {
            conn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.12.0; Data Source=auto_bd.accdb");
            dt = new DataTable();
            adapter = new OleDbDataAdapter("SELECT * FROM Clients", conn);
            conn.Open();
            adapter.Fill(dt);
            conn.Close();
        }

        public registration()
        {
            InitializeComponent();
        }

        private void registration_Load(object sender, EventArgs e)
        {
            GetData();
        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            conn.Open();
            if (txtSurname.Text == "" || txtName.Text == "" || txtotchestvo.Text == "" || txtLogin.Text == "" || txtPass.Text == "" || txtPassRepeat.Text == "")
            { DialogResult dr = MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1); }
            else if(r.IsMatch(txtSurname.Text)!=true|| r.IsMatch(txtName.Text) != true || r.IsMatch(txtotchestvo.Text) != true)
            {
                DialogResult dr = MessageBox.Show("Ошибка формата введенных данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                
                DataView dv = dt.DefaultView;
                dv.RowFilter = "acc_login LIKE '" + txtLogin.Text + "'";
                if (dv.Count != 0)
                {
                    DialogResult dr = MessageBox.Show("Логин занят", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    txtLogin.Clear();

                }

                else
                {

                    if (txtPass.Text != txtPassRepeat.Text)
                    {
                        DialogResult dr = MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        txtLogin.Clear();
                        txtPass.Clear();
                    }
                    else
                    {
                        string querry = "INSERT INTO Clients(Surname, Imya, Otchestvo, acc_login, acc_pass, acc_type) VALUES" +
                        "(@Surname,@Imya,@Otchestvo,@login,@password,@type)";
                        cmd = new OleDbCommand(querry, conn);

                        cmd.Parameters.AddWithValue("@Surname", txtSurname.Text);
                        cmd.Parameters.AddWithValue("@Imya", txtName.Text);
                        cmd.Parameters.AddWithValue("@Otchestvo", txtotchestvo.Text);
                        cmd.Parameters.AddWithValue("@login", txtLogin.Text);
                        cmd.Parameters.AddWithValue("@password", txtPass.Text);
                        if (txtSecret.Text.ToLower() == "автосалон")
                            cmd.Parameters.AddWithValue("@type", "admin");
                        else
                            cmd.Parameters.AddWithValue("@type", "user");

                        cmd.ExecuteNonQuery();
                        DialogResult dr = MessageBox.Show("Успешная регистрация !", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                        GetData();
                        Owner.Show();
                        this.Close();

                    }

                }
                
            }
            conn.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Owner.Show();
            this.Close();
        }

        private void registration_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }
    }
}
