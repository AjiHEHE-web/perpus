using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace perpustakaan
{
    public partial class cariBuku : Form
    {
        public SqlConnection connection = new SqlConnection(koneksi.conn); // untuk koneksi
        public SqlCommand command; // untuk eksekusi perintah sql
        public SqlDataAdapter adapter; // ambil data dari sqlcommand
        public DataTable tabel; // untuk tabel menyimpan hasil query
        public cariBuku()
        {
            InitializeComponent();
            showData();
        }

        private void cariBuku_Load(object sender, EventArgs e)
        {
            searchData();
        }

        public void showData()
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();

                    string sql = "SELECT buku.buku_id, buku.buku_judul, buku.buku_isbn, buku.buku_thnterbit, " +
                                 "penulis.penulis_nama, rak.rak_nama, kategori.kategori_nama, penerbit.penerbit_nama from buku " +
                                 "JOIN penulis ON penulis.penulis_id = buku.buku_penulis_id " +
                                 "JOIN rak ON rak.rak_id = buku.buku_rak_id " +
                                 "JOIN kategori ON kategori.kategori_id = buku.buku_kategori_id " +
                                 "JOIN penerbit ON penerbit.penerbit_id = buku.buku_penerbit_id";

                    command = new SqlCommand(sql, connection);
                    adapter = new SqlDataAdapter(command);
                    tabel = new DataTable();
                    adapter.Fill(tabel);

                    dataGridView1.DataSource = tabel;
                    dataGridView1.Columns[0].HeaderText = "Id Buku";
                    dataGridView1.Columns[1].HeaderText = "Judul Buku";
                    dataGridView1.Columns[2].HeaderText = "ISBN";
                    dataGridView1.Columns[3].HeaderText = "Tahun Terbit";
                    dataGridView1.Columns[4].HeaderText = "Penulis";
                    dataGridView1.Columns[5].HeaderText = "Rak";
                    dataGridView1.Columns[6].HeaderText = "Kategori";
                    dataGridView1.Columns[7].HeaderText = "Penerbit";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at:" + ex);
                }
            }
        }

        public void searchData()
        {
            if (!string.IsNullOrEmpty(cari.Text))
            {
                try
                {
                    string sql = "SELECT buku.buku_id, buku.buku_judul, buku.buku_isbn, buku.buku_thnterbit, " +
                                 "penulis.penulis_nama, rak.rak_nama, kategori.kategori_nama, penerbit.penerbit_nama " +
                                 "FROM buku " +
                                 "JOIN penulis ON penulis.penulis_id = buku.buku_penulis_id " +
                                 "JOIN rak ON rak.rak_id = buku.buku_rak_id " +
                                 "JOIN kategori ON kategori.kategori_id = buku.buku_kategori_id " +
                                 "JOIN penerbit ON penerbit.penerbit_id = buku.buku_penerbit_id " +
                                 "WHERE " +
                                 "buku_id LIKE '%' + @search + '%' OR " +
                                 "buku_judul LIKE '%' + @search + '%' OR " +
                                 "buku_isbn LIKE '%' + @search + '%' OR " +
                                 "buku_thnterbit LIKE '%' + @search + '%' OR " +
                                 "penulis_nama LIKE '%' + @search + '%' OR " +
                                 "rak_nama LIKE '%' + @search + '%' OR " +
                                 "kategori_nama LIKE '%' + @search + '%' OR " +
                                 "penerbit_nama LIKE '%' + @search + '%'";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@search", cari.Text);
                    adapter = new SqlDataAdapter(command);
                    tabel = new DataTable();
                    adapter.Fill(tabel);

                    dataGridView1.DataSource = tabel;
                    dataGridView1.Columns[0].HeaderText = "Id Buku";
                    dataGridView1.Columns[1].HeaderText = "Judul Buku";
                    dataGridView1.Columns[2].HeaderText = "ISBN";
                    dataGridView1.Columns[3].HeaderText = "Tahun Terbit";
                    dataGridView1.Columns[4].HeaderText = "Penulis";
                    dataGridView1.Columns[5].HeaderText = "Rak";
                    dataGridView1.Columns[6].HeaderText = "Kategori";
                    dataGridView1.Columns[7].HeaderText = "Penerbit";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at:" + ex);
                }
            }
            else
            {
                showData();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            peminjaman_agt panggil = new peminjaman_agt();
            panggil.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            riwayat_agt panggil = new riwayat_agt();
            panggil.Show();
            this.Hide();
        }
    }
}
