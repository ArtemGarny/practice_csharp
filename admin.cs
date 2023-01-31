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

namespace автосалон_дизайн
{
    public partial class admin : KryptonForm
    {
        public admin()
        {
            InitializeComponent();
        }

        private void admin_Load(object sender, EventArgs e)
        {

        }

        private void btnClients_Click(object sender, EventArgs e)
        {
            Clients cf = new Clients();
            cf.Owner = this;
            cf.Show();
            this.Hide();
        }

        private void btnProviders_Click(object sender, EventArgs e)
        {
            Providers cf = new Providers();
            cf.Owner = this;
            cf.Show();
            this.Hide();
        }

        private void btnPurchases_Click(object sender, EventArgs e)
        {
            Purchases cf = new Purchases();
            cf.Owner = this;
            cf.Show();
            this.Hide();
        }

        private void btnSupplies_Click(object sender, EventArgs e)
        {
            Supplies cf = new Supplies();
            cf.Owner = this;
            cf.Show();
            this.Hide();
        }

        private void btnCars_Click(object sender, EventArgs e)
        {
            Cars cf = new Cars();
            cf.Owner = this;
            cf.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Owner.Show();
            this.Close();
        }

        private void admin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }
    }
}
