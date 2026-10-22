using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace perpustakaan
{
    public partial class Register : Form
    {
        public SqlConnection connection = new SqlConnection(koneksi.conn);
        public SqlCommand command;
        public Register()
        {
            InitializeComponent();
        }
        public void clear()
        {
            nama.Text = "";
            alamat.Text = "";
            email.Text = "";
            notlp.Text = "";
            username.Text = "";
            password.Text = "";
        }

        public void insertData()
        {
            if (!string.IsNullOrEmpty(nama.Text) &&
                !string.IsNullOrEmpty(alamat.Text) &&
                !string.IsNullOrEmpty(email.Text) &&
                !string.IsNullOrEmpty(notlp.Text) &&
                !string.IsNullOrEmpty(username.Text) &&
                !string.IsNullOrEmpty(password.Text))
            {
                string sql = "INSERT INTO users (user_nama, user_alamat, user_username, user_email, user_notelp, user_password, user_level) " +
                             "VALUES (@nama, @alamat, @username, @email, @notelp, @password, @level)";

                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nama", nama.Text);
                command.Parameters.AddWithValue("@alamat", alamat.Text);
                command.Parameters.AddWithValue("@username", username.Text);
                command.Parameters.AddWithValue("@email", email.Text);
                command.Parameters.AddWithValue("@notelp", notlp.Text);
                command.Parameters.AddWithValue("@password", password.Text);
                command.Parameters.AddWithValue("@level", "anggota");

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show("Successfully registered!","Information", MessageBoxButtons.OK, MessageBoxIcon.Information
                    );
                    clear();

                    this.Hide(); // menyembunyikan form register
                    Login panggil = new Login();
                    panggil.Show(); // menampilkan form login
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at:" + ex);
                }
                finally 
                { 
                    connection.Close(); 
                }
            
            }
            else
            {
                MessageBox.Show("Please fill all data");
            }
        }
        
        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void Register_Load(object sender, EventArgs e)
        {

        }

        private void bton_Click(object sender, EventArgs e)
        {
            insertData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login panggil = new Login();
            panggil.Show();
        }
    }
}
