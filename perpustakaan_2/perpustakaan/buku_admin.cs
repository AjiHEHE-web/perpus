using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace perpustakaan
{
    public partial class buku_admin : Form
    {
        SqlCommand command;
        SqlDataReader reader;
        string idBuku = ""; // Variabel untuk menyimpan ID buku yang sedang dipilih

        public buku_admin()
        {
            InitializeComponent();

            label4.Text = Model.name;

            this.Resize += buku_admin_Resize;

            AturLayout();
        }

        // ========================== METHOD DATA ==========================

        public void showData()
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();
                    // Menggunakan JOIN agar DataGridView menampilkan nama bukan hanya ID
                    string sql = @"
                        SELECT b.buku_id, b.buku_judul, p.penulis_nama, k.kategori_nama, 
                               pn.penerbit_nama, r.rak_nama, b.buku_isbn, b.buku_thnterbit,
                               b.buku_penulis_id, b.buku_kategori_id, b.buku_penerbit_id, b.buku_rak_id
                        FROM buku b
                        LEFT JOIN penulis p ON b.buku_penulis_id = p.penulis_id
                        LEFT JOIN kategori k ON b.buku_kategori_id = k.kategori_id
                        LEFT JOIN penerbit pn ON b.buku_penerbit_id = pn.penerbit_id
                        LEFT JOIN rak r ON b.buku_rak_id = r.rak_id";

                    command = new SqlCommand(sql, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;

                    // Menyembunyikan kolom ID agar tidak membingungkan pengguna
                    if (dataGridView1.Columns.Count > 0)
                    {
                        dataGridView1.Columns["buku_penulis_id"].Visible = false;
                        dataGridView1.Columns["buku_kategori_id"].Visible = false;
                        dataGridView1.Columns["buku_penerbit_id"].Visible = false;
                        dataGridView1.Columns["buku_rak_id"].Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error tampil data: " + ex.Message);
                }
            }
        }

        public void insertData()
        {
            if (cPenulis.SelectedIndex == -1 || cKategori.SelectedIndex == -1 ||
                cPenerbit.SelectedIndex == -1 || cRak.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(tJudul.Text) || string.IsNullOrWhiteSpace(tIsbn.Text) ||
                string.IsNullOrWhiteSpace(tThnterbit.Text))
            {
                MessageBox.Show("Data buku belum lengkap!");
                return;
            }

            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                        INSERT INTO buku (buku_id, buku_penulis_id, buku_kategori_id, buku_penerbit_id, buku_rak_id, buku_judul, buku_isbn, buku_thnterbit) 
                        VALUES (@buku_id, @penulis_id, @kategori_id, @penerbit_id, @rak_id, @judul, @isbn, @thnterbit)";

                    command = new SqlCommand(sql, connection);
                    // Mengirim string kosong untuk buku_id karena akan dioverride otomatis oleh Trigger yang dibuat
                    command.Parameters.AddWithValue("@buku_id", "");
                    command.Parameters.AddWithValue("@penulis_id", cPenulis.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@kategori_id", cKategori.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@penerbit_id", cPenerbit.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@rak_id", cRak.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@judul", tJudul.Text);
                    command.Parameters.AddWithValue("@isbn", tIsbn.Text);
                    command.Parameters.AddWithValue("@thnterbit", tThnterbit.Text);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Data buku berhasil ditambahkan!");
                    BersihkanInput();
                    showData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error insert buku: " + ex.Message);
                }
            }
        }

        public void editData()
        {
            if (string.IsNullOrEmpty(idBuku))
            {
                MessageBox.Show("Pilih data yang ingin diedit dari tabel terlebih dahulu!");
                return;
            }

            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                        UPDATE buku SET 
                            buku_penulis_id = @penulis_id, 
                            buku_kategori_id = @kategori_id, 
                            buku_penerbit_id = @penerbit_id, 
                            buku_rak_id = @rak_id, 
                            buku_judul = @judul, 
                            buku_isbn = @isbn, 
                            buku_thnterbit = @thnterbit 
                        WHERE buku_id = @id";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@id", idBuku);
                    command.Parameters.AddWithValue("@penulis_id", cPenulis.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@kategori_id", cKategori.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@penerbit_id", cPenerbit.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@rak_id", cRak.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@judul", tJudul.Text);
                    command.Parameters.AddWithValue("@isbn", tIsbn.Text);
                    command.Parameters.AddWithValue("@thnterbit", tThnterbit.Text);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Data buku berhasil diupdate!");
                    BersihkanInput();
                    showData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error edit buku: " + ex.Message);
                }
            }
        }

        public void deleteData()
        {
            if (string.IsNullOrEmpty(idBuku))
            {
                MessageBox.Show("Pilih data yang ingin dihapus dari tabel terlebih dahulu!");
                return;
            }

            DialogResult dialog = MessageBox.Show("Apakah Anda yakin ingin menghapus buku ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialog == DialogResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(koneksi.conn))
                {
                    try
                    {
                        connection.Open();
                        string sql = "DELETE FROM buku WHERE buku_id = @id";
                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@id", idBuku);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Data buku berhasil dihapus!");
                        BersihkanInput();
                        showData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error hapus buku: " + ex.Message);
                    }
                }
            }
        }

        public void searchData()
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                        SELECT b.buku_id, b.buku_judul, p.penulis_nama, k.kategori_nama, 
                               pn.penerbit_nama, r.rak_nama, b.buku_isbn, b.buku_thnterbit,
                               b.buku_penulis_id, b.buku_kategori_id, b.buku_penerbit_id, b.buku_rak_id
                        FROM buku b
                        LEFT JOIN penulis p ON b.buku_penulis_id = p.penulis_id
                        LEFT JOIN kategori k ON b.buku_kategori_id = k.kategori_id
                        LEFT JOIN penerbit pn ON b.buku_penerbit_id = pn.penerbit_id
                        LEFT JOIN rak r ON b.buku_rak_id = r.rak_id
                        WHERE b.buku_judul LIKE @cari OR b.buku_isbn LIKE @cari";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@cari", "%" + cariBuku.Text + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error cari data: " + ex.Message);
                }
            }
        }

        // ======================= METHOD COMBOBOX & INPUT =======================

        public void showNamaPenulis()
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT penulis_id, penulis_nama FROM penulis";
                    command = new SqlCommand(sql, connection);
                    reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    cPenulis.DataSource = null;
                    cPenulis.DataSource = dt;
                    cPenulis.DisplayMember = "penulis_nama";
                    cPenulis.ValueMember = "penulis_id";
                    cPenulis.SelectedIndex = -1;
                }
                catch { cPenulis.DataSource = null; }
            }
        }

        public void showNamaKategori()
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT kategori_id, kategori_nama FROM kategori";
                    command = new SqlCommand(sql, connection);
                    reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    cKategori.DataSource = null;
                    cKategori.DataSource = dt;
                    cKategori.DisplayMember = "kategori_nama";
                    cKategori.ValueMember = "kategori_id";
                    cKategori.SelectedIndex = -1;
                }
                catch { cKategori.DataSource = null; }
            }
        }

        public void showNamaPenerbit()
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT penerbit_id, penerbit_nama FROM penerbit";
                    command = new SqlCommand(sql, connection);
                    reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    cPenerbit.DataSource = null;
                    cPenerbit.DataSource = dt;
                    cPenerbit.DisplayMember = "penerbit_nama";
                    cPenerbit.ValueMember = "penerbit_id";
                    cPenerbit.SelectedIndex = -1;
                }
                catch { cPenerbit.DataSource = null; }
            }
        }

        public void showNamaRak()
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT rak_id, rak_nama FROM rak";
                    command = new SqlCommand(sql, connection);
                    reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    cRak.DataSource = null;
                    cRak.DataSource = dt;
                    cRak.DisplayMember = "rak_nama";
                    cRak.ValueMember = "rak_id";
                    cRak.SelectedIndex = -1;
                }
                catch { cRak.DataSource = null; }
            }
        }

        private void BersihkanInput()
        {
            idBuku = ""; // Kosongkan ID saat clear input
            cPenulis.SelectedIndex = -1;
            cKategori.SelectedIndex = -1;
            cPenerbit.SelectedIndex = -1;
            cRak.SelectedIndex = -1;

            tJudul.Clear();
            tIsbn.Clear();
            tThnterbit.Clear();
        }

        // ============================ EVENT HANDLER ============================

        private void buku_admin_Load(object sender, EventArgs e)
        {
            label4.Text = Model.name;

            AturLayout();
            showData(); // Panggil Tampil Data

            showNamaPenulis();
            showNamaKategori();
            showNamaPenerbit();
            showNamaRak();
        }

        private void buku_admin_Resize(object sender, EventArgs e)
        {
            AturLayout();
        }

        private void bInsert_Click(object sender, EventArgs e)
        {
            insertData();
        }

        // Handle Event tambahan bila kamu tak sengaja klik tombolnya 2x membuat bInsert_Click_1
        private void bInsert_Click_1(object sender, EventArgs e)
        {
            insertData();
        }

        private void bEdit_Click(object sender, EventArgs e)
        {
            editData();
        }

        private void bDelete_Click(object sender, EventArgs e)
        {
            deleteData();
        }

        private void cariBuku_TextChanged(object sender, EventArgs e)
        {
            searchData();
        }

        // Saat Row pada DataGridView diklik, datanya pindah ke form input
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                idBuku = row.Cells["buku_id"].Value.ToString();
                tJudul.Text = row.Cells["buku_judul"].Value.ToString();
                tIsbn.Text = row.Cells["buku_isbn"].Value.ToString();
                tThnterbit.Text = row.Cells["buku_thnterbit"].Value.ToString();

                // Select index berdasarkan Value ID
                cPenulis.SelectedValue = row.Cells["buku_penulis_id"].Value.ToString();
                cKategori.SelectedValue = row.Cells["buku_kategori_id"].Value.ToString();
                cPenerbit.SelectedValue = row.Cells["buku_penerbit_id"].Value.ToString();
                cRak.SelectedValue = row.Cells["buku_rak_id"].Value.ToString();
            }
        }

        // ============================== NAVIGATION ==============================

        private void button1_Click_1(object sender, EventArgs e) { new Admin().Show(); this.Hide(); }
        private void button2_Click_1(object sender, EventArgs e) { new penulis_admin().Show(); this.Hide(); }
        private void button3_Click_1(object sender, EventArgs e) { new penerbit_admin().Show(); this.Hide(); }
        private void button4_Click_1(object sender, EventArgs e) { new kategori_admin().Show(); this.Hide(); }
        private void button5_Click_1(object sender, EventArgs e) { new rak_admin().Show(); this.Hide(); }
        private void button6_Click_1(object sender, EventArgs e) { new buku_admin().Show(); this.Hide(); }
        private void button7_Click_1(object sender, EventArgs e) { new peminjaman_admin().Show(); this.Hide(); }
        private void button8_Click_1(object sender, EventArgs e) { this.Close(); }


        // =============================== LAYOUT =================================

        private void AturLayout()
        {
            SuspendLayout();

            if (this.WindowState == FormWindowState.Maximized)
            {
                pHeader.Height = 87;

                pLogo.Location = new Point(20, 6);
                pLogo.Size = new Size(83, 77);

                label1.Font = new Font("Arial", 21.75F, FontStyle.Bold);
                label1.Location = new Point((this.ClientSize.Width - label1.Width) / 2, 22);

                panel1.Width = 220;
                panel1.Padding = new Padding(25, 23, 25, 23);

                lLine.Location = new Point(220, 87);
                lLine.Size = new Size(this.ClientSize.Width - 220, 48);

                label2.Font = new Font("Calibri", 14.25F, FontStyle.Bold);
                label2.Location = new Point(230, 99);

                label4.Font = new Font("Calibri", 14.25F, FontStyle.Bold);
                label4.Location = new Point(label2.Right + 8, 99);

                button1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
                button2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
                button3.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
                button4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
                button5.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
                button6.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
                button7.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
                button8.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);

                button1.Margin = new Padding(13, 15, 13, 15);
                button2.Margin = new Padding(13, 15, 13, 15);
                button3.Margin = new Padding(13, 15, 13, 15);
                button4.Margin = new Padding(13, 15, 13, 15);
                button5.Margin = new Padding(13, 15, 13, 15);
                button6.Margin = new Padding(13, 15, 13, 15);
                button7.Margin = new Padding(13, 15, 13, 15);
                button8.Margin = new Padding(13, 15, 13, 15);

                int contentLeft = panel1.Width + 18;
                int contentRight = this.ClientSize.Width - 18;
                int contentWidth = contentRight - contentLeft;

                int gap = 16;
                int panel2Width = (int)(contentWidth * 0.36);
                int panel4Width = contentWidth - panel2Width - gap;

                if (panel2Width < 300)
                    panel2Width = 300;

                if (panel4Width < 350)
                    panel4Width = 350;

                int totalWidth = panel2Width + gap + panel4Width;

                if (totalWidth > contentWidth)
                {
                    panel2Width = (int)(contentWidth * 0.35);
                    panel4Width = contentWidth - panel2Width - gap;
                }

                int contentX = contentLeft;

                label3.Location = new Point(
                    contentX + ((panel2Width + gap + panel4Width) - label3.Width) / 2,
                    122
                );

                panel2.Location = new Point(contentX, 145);
                panel2.Size = new Size(panel2Width, 137);

                panel4.Location = new Point(contentX + panel2Width + gap, 145);
                panel4.Size = new Size(panel4Width, 137);

                cPenulis.Location = new Point(63, 11);
                cKategori.Location = new Point(63, 42);
                cPenerbit.Location = new Point(63, 77);
                cRak.Location = new Point(63, 108);

                cPenulis.Width = panel2.Width - 70;
                cKategori.Width = panel2.Width - 70;
                cPenerbit.Width = panel2.Width - 70;
                cRak.Width = panel2.Width - 70;

                tJudul.Location = new Point(94, 12);
                tIsbn.Location = new Point(94, 47);
                tThnterbit.Location = new Point(94, 82);

                tJudul.Width = panel4.Width - 102;
                tIsbn.Width = panel4.Width - 102;
                tThnterbit.Width = panel4.Width - 102;

                panel3.Location = new Point(contentX, 288);
                panel3.Size = new Size(contentWidth, 45);

                bInsert.Location = new Point((panel3.Width / 2) - 180, 7);
                bEdit.Location = new Point((panel3.Width / 2) - 30, 7);
                bDelete.Location = new Point((panel3.Width / 2) + 120, 7);

                label8.Location = new Point(contentX, 365);
                cariBuku.Location = new Point(label8.Right + 8, 363);

                dataGridView1.Location = new Point(contentX, 395);
                dataGridView1.Size = new Size(contentWidth, this.ClientSize.Height - 415);
            }
            else
            {
                pHeader.Height = 65;
                pLogo.Location = new Point(10, 5);
                pLogo.Size = new Size(55, 55);
                label1.Font = new Font("Arial", 18F, FontStyle.Bold);
                label1.Location = new Point((this.ClientSize.Width - label1.Width) / 2, 18);
                panel1.Width = 130;
                panel1.Padding = new Padding(10);
                lLine.Location = new Point(130, 65);
                lLine.Size = new Size(this.ClientSize.Width - 130, 40);
                label2.Font = new Font("Calibri", 10F, FontStyle.Bold);
                label2.Location = new Point(140, 76);
                label4.Font = new Font("Calibri", 10F, FontStyle.Bold);
                label4.Location = new Point(label2.Right + 5, 76);

                button1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
                button2.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
                button3.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
                button4.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
                button5.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
                button6.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
                button7.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
                button8.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);

                button1.Margin = new Padding(5, 6, 5, 6);
                button2.Margin = new Padding(5, 6, 5, 6);
                button3.Margin = new Padding(5, 6, 5, 6);
                button4.Margin = new Padding(5, 6, 5, 6);
                button5.Margin = new Padding(5, 6, 5, 6);
                button6.Margin = new Padding(5, 6, 5, 6);
                button7.Margin = new Padding(5, 6, 5, 6);
                button8.Margin = new Padding(5, 6, 5, 6);
            }
            ResumeLayout();
        }

        // ============================= EVENT LAIN (KOSONG) =========================
        private void label1_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
        private void lLine_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
    }
}