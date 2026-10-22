using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace perpustakaan
{

    public partial class Anggota : Form
    {
        
        public Anggota()
        {
            InitializeComponent();

            label4.Text = Model.name;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login panggil = new Login();
            panggil.Show();
        }

        private void Anggota_Load(object sender, EventArgs e)
        {

        }

        

        private void cari_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            cariBuku panggil = new cariBuku();
            panggil.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            peminjaman_agt panggil = new peminjaman_agt();
            panggil.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            riwayat_agt panggil =new riwayat_agt();
            panggil.Show();
            this.Hide();
        }
    }
}
