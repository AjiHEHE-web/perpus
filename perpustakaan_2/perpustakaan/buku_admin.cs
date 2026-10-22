using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace perpustakaan
{
    public partial class buku_admin : Form
    {
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable tabel;
        SqlDataReader reader;
        public string id_buku = "";

        private const string NamaKolomPilih = "colPilih";

        public buku_admin()
        {
            InitializeComponent();

            label4.Text = Model.name;

            // Klik di mana pun pada baris (termasuk tombol "Pilih") memilih baris
            dataGridView1.CellClick -= dataGridView1_CellClick;
            dataGridView1.CellClick += dataGridView1_CellClick;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            this.Resize += buku_admin_Resize;

            AturLayout();
        }

        // ===================== LOAD COMBOBOX =====================

        public void showNamaPenulis()
        {
            IsiCombo(cPenulis, "SELECT penulis_nama FROM penulis", "penulis_nama");
        }

        public void showNamaKategori()
        {
            IsiCombo(cKategori, "SELECT kategori_nama FROM kategori", "kategori_nama");
        }

        public void showNamaPenerbit()
        {
            IsiCombo(cPenerbit, "SELECT penerbit_nama FROM penerbit", "penerbit_nama");
        }

        public void showNamaRak()
        {
            IsiCombo(cRak, "SELECT rak_nama FROM rak", "rak_nama");
        }

        private void IsiCombo(ComboBox combo, string sql, string kolom)
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();

                    command = new SqlCommand(sql, connection);
                    reader = command.ExecuteReader();

                    combo.Items.Clear();

                    while (reader.Read())
                    {
                        combo.Items.Add(reader[kolom].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at: " + ex.Message);
                }
            }
        }

        // ===================== TAMPIL DATA =====================

        private readonly string sqlSelectBuku =
            @"SELECT b.buku_id, b.buku_judul, b.buku_isbn, b.buku_thnterbit,
                     p.penulis_nama, k.kategori_nama, pb.penerbit_nama, r.rak_nama
              FROM buku b
              JOIN penulis p ON b.buku_penulis_id = p.penulis_id
              JOIN kategori k ON b.buku_kategori_id = k.kategori_id
              JOIN penerbit pb ON b.buku_penerbit_id = pb.penerbit_id
              JOIN rak r ON b.buku_rak_id = r.rak_id";

        public void showData()
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();

                    command = new SqlCommand(sqlSelectBuku, connection);
                    adapter = new SqlDataAdapter(command);

                    tabel = new DataTable();
                    adapter.Fill(tabel);

                    HapusKolomPilih();
                    dataGridView1.DataSource = tabel;

                    AturGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at: " + ex.Message);
                }
            }
        }

        // Hapus kolom tombol sebelum DataSource diganti,
        // supaya index kolom data (0-7) tidak bergeser
        private void HapusKolomPilih()
        {
            if (dataGridView1.Columns.Contains(NamaKolomPilih))
                dataGridView1.Columns.Remove(NamaKolomPilih);
        }

        private void AturGrid()
        {
            if (dataGridView1.Columns.Count == 0)
                return;

            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Judul";
            dataGridView1.Columns[2].HeaderText = "ISBN";
            dataGridView1.Columns[3].HeaderText = "Tahun Terbit";
            dataGridView1.Columns[4].HeaderText = "Penulis";
            dataGridView1.Columns[5].HeaderText = "Kategori";
            dataGridView1.Columns[6].HeaderText = "Penerbit";
            dataGridView1.Columns[7].HeaderText = "Rak";

            // Cegah edit langsung di dalam sel
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;

            // Tambahkan kolom tombol "Pilih" di paling kanan
            if (!dataGridView1.Columns.Contains(NamaKolomPilih))
            {
                DataGridViewButtonColumn btnPilih = new DataGridViewButtonColumn();
                btnPilih.Name = NamaKolomPilih;
                btnPilih.HeaderText = "Aksi";
                btnPilih.Text = "Pilih";
                btnPilih.UseColumnTextForButtonValue = true;
                btnPilih.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                btnPilih.Width = 70;
                dataGridView1.Columns.Add(btnPilih);
            }
        }

        // ===================== INPUT =====================

        public void clear()
        {
            tJudul.Text = "";
            tIsbn.Text = "";
            tThnterbit.Text = "";

            cPenulis.SelectedIndex = -1;
            cKategori.SelectedIndex = -1;
            cPenerbit.SelectedIndex = -1;
            cRak.SelectedIndex = -1;

            id_buku = "";

            dataGridView1.ClearSelection();
        }

        private bool inputValid()
        {
            if (string.IsNullOrEmpty(tJudul.Text) ||
                string.IsNullOrEmpty(tIsbn.Text) ||
                string.IsNullOrEmpty(tThnterbit.Text) ||
                cPenulis.SelectedIndex == -1 ||
                cKategori.SelectedIndex == -1 ||
                cPenerbit.SelectedIndex == -1 ||
                cRak.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all data!");
                return false;
            }

            if (tThnterbit.Text.Length != 4 ||
                !tThnterbit.Text.All(char.IsDigit))
            {
                MessageBox.Show("Tahun terbit harus 4 digit angka!");
                return false;
            }

            return true;
        }

        private void isiParameter(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@penulis", cPenulis.Text);
            cmd.Parameters.AddWithValue("@kategori", cKategori.Text);
            cmd.Parameters.AddWithValue("@penerbit", cPenerbit.Text);
            cmd.Parameters.AddWithValue("@rak", cRak.Text);
            // Sesuai tipe kolom: varchar(50), char(16), char(4)
            cmd.Parameters.Add("@judul", SqlDbType.VarChar, 50).Value = tJudul.Text.Trim();
            cmd.Parameters.Add("@isbn", SqlDbType.Char, 16).Value = tIsbn.Text.Trim();
            cmd.Parameters.Add("@tahun", SqlDbType.Char, 4).Value = tThnterbit.Text.Trim();
        }

        // buku_id adalah varchar(50) (bukan identity), jadi ID dibuat manual: Buku1, Buku2, ...
        private string buatIdBuku(SqlConnection connection)
        {
            string sql =
                @"SELECT ISNULL(MAX(TRY_CAST(SUBSTRING(buku_id, 5, 50) AS INT)), 0) + 1
                  FROM buku
                  WHERE buku_id LIKE 'Buku%'";

            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                int next = Convert.ToInt32(cmd.ExecuteScalar());
                return "Buku" + next;
            }
        }

        // ===================== CRUD =====================

        public void insertData()
        {
            if (!inputValid())
                return;

            try
            {
                using (SqlConnection connection = new SqlConnection(koneksi.conn))
                {
                    connection.Open();

                    string idBaru = buatIdBuku(connection);

                    string sql =
                        @"INSERT INTO buku
                        (
                            buku_id,
                            buku_penulis_id,
                            buku_kategori_id,
                            buku_penerbit_id,
                            buku_rak_id,
                            buku_judul,
                            buku_isbn,
                            buku_thnterbit
                        )
                        VALUES
                        (
                            @id,
                            (SELECT penulis_id FROM penulis WHERE penulis_nama = @penulis),
                            (SELECT kategori_id FROM kategori WHERE kategori_nama = @kategori),
                            (SELECT penerbit_id FROM penerbit WHERE penerbit_nama = @penerbit),
                            (SELECT rak_id FROM rak WHERE rak_nama = @rak),
                            @judul,
                            @isbn,
                            @tahun
                        )";

                    command = new SqlCommand(sql, connection);

                    command.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = idBaru;
                    isiParameter(command);

                    command.ExecuteNonQuery();
                }

                MessageBox.Show(
                    "Successfully inserted!",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                clear();
                showData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error at: " + ex.Message);
            }
        }

        public void editData()
        {
            if (string.IsNullOrEmpty(id_buku))
            {
                MessageBox.Show(
                    "Please select the data!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            if (!inputValid())
                return;

            DialogResult result = MessageBox.Show(
                "Are you sure to update?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(koneksi.conn))
                    {
                        string sql =
                            @"UPDATE buku SET
                                buku_penulis_id =
                                    (SELECT penulis_id
                                     FROM penulis
                                     WHERE penulis_nama = @penulis),

                                buku_kategori_id =
                                    (SELECT kategori_id
                                     FROM kategori
                                     WHERE kategori_nama = @kategori),

                                buku_penerbit_id =
                                    (SELECT penerbit_id
                                     FROM penerbit
                                     WHERE penerbit_nama = @penerbit),

                                buku_rak_id =
                                    (SELECT rak_id
                                     FROM rak
                                     WHERE rak_nama = @rak),

                                buku_judul = @judul,
                                buku_isbn = @isbn,
                                buku_thnterbit = @tahun

                              WHERE buku_id = @id_buku";

                        command = new SqlCommand(sql, connection);

                        isiParameter(command);

                        command.Parameters.Add("@id_buku", SqlDbType.VarChar, 50).Value = id_buku;

                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        "Successfully updated!",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    clear();
                    showData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void deleteData()
        {
            if (string.IsNullOrEmpty(id_buku))
            {
                MessageBox.Show(
                    "Please select the data!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure to delete?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(koneksi.conn))
                    {
                        string sql = "DELETE FROM buku WHERE buku_id = @id_buku";

                        command = new SqlCommand(sql, connection);

                        command.Parameters.Add("@id_buku", SqlDbType.VarChar, 50).Value = id_buku;

                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        "Successfully deleted!",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    clear();
                    showData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void searchData()
        {
            if (!string.IsNullOrEmpty(cariBuku.Text))
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(koneksi.conn))
                    {
                        string sql =
                            sqlSelectBuku +
                            @" WHERE
                                b.buku_judul LIKE '%' + @search + '%'
                                OR b.buku_isbn LIKE '%' + @search + '%'
                                OR p.penulis_nama LIKE '%' + @search + '%'
                                OR k.kategori_nama LIKE '%' + @search + '%'
                                OR pb.penerbit_nama LIKE '%' + @search + '%'
                                OR r.rak_nama LIKE '%' + @search + '%'";

                        command = new SqlCommand(sql, connection);

                        command.Parameters.AddWithValue("@search", cariBuku.Text);

                        adapter = new SqlDataAdapter(command);

                        tabel = new DataTable();
                        adapter.Fill(tabel);

                        HapusKolomPilih();
                        dataGridView1.DataSource = tabel;

                        AturGrid();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at: " + ex.Message);
                }
            }
            else
            {
                showData();
            }
        }

        // ===================== PILIH BARIS =====================

        private void dataGridView1_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            PilihBaris(e.RowIndex);
        }

        private void PilihBaris(int rowIndex)
        {
            DataGridViewRow row = dataGridView1.Rows[rowIndex];

            dataGridView1.ClearSelection();
            row.Selected = true;

            // Kolom char(16) dan char(4) diberi spasi di belakang oleh SQL Server, jadi di-Trim
            id_buku = Convert.ToString(row.Cells[0].Value).Trim();
            tJudul.Text = Convert.ToString(row.Cells[1].Value).Trim();
            tIsbn.Text = Convert.ToString(row.Cells[2].Value).Trim();
            tThnterbit.Text = Convert.ToString(row.Cells[3].Value).Trim();

            PilihCombo(cPenulis, Convert.ToString(row.Cells[4].Value));
            PilihCombo(cKategori, Convert.ToString(row.Cells[5].Value));
            PilihCombo(cPenerbit, Convert.ToString(row.Cells[6].Value));
            PilihCombo(cRak, Convert.ToString(row.Cells[7].Value));
        }

        private void PilihCombo(ComboBox combo, string nilai)
        {
            combo.SelectedIndex = -1;

            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i].ToString().Trim() == nilai.Trim())
                {
                    combo.SelectedIndex = i;
                    break;
                }
            }
        }

        // ===================== LAYOUT =====================

        private void buku_admin_Resize(object sender, EventArgs e)
        {
            AturLayout();
        }

        private void AturFontMenu(float ukuran, Padding margin)
        {
            Button[] tombol =
            {
                button1, button2, button3, button4,
                button5, button6, button7, button8
            };

            foreach (Button b in tombol)
            {
                b.Font = new Font("Microsoft Sans Serif", ukuran, FontStyle.Bold);
                b.Margin = margin;
            }
        }

        private void AturLayout()
        {
            SuspendLayout();

            if (this.WindowState == FormWindowState.Maximized)
            {
                pHeader.Height = 87;

                pLogo.Location = new Point(20, 6);
                pLogo.Size = new Size(83, 77);

                label1.Font = new Font("Arial", 21.75F, FontStyle.Bold);
                label1.Location = new Point(
                    (this.ClientSize.Width - label1.Width) / 2,
                    22
                );

                panel1.Width = 220;
                panel1.Padding = new Padding(25, 23, 25, 23);

                lLine.Location = new Point(220, 87);
                lLine.Size = new Size(this.ClientSize.Width - 220, 48);

                label2.Font = new Font("Calibri", 14.25F, FontStyle.Bold);
                label2.Location = new Point(230, 99);

                label4.Font = new Font("Calibri", 14.25F, FontStyle.Bold);
                label4.Location = new Point(label2.Right + 8, 99);

                AturFontMenu(12F, new Padding(13, 15, 13, 15));

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
                dataGridView1.Size = new Size(
                    contentWidth,
                    this.ClientSize.Height - 415
                );
            }
            else
            {
                pHeader.Height = 65;

                pLogo.Location = new Point(10, 5);
                pLogo.Size = new Size(55, 55);

                label1.Font = new Font("Arial", 18F, FontStyle.Bold);
                label1.Location = new Point(
                    (this.ClientSize.Width - label1.Width) / 2,
                    18
                );

                panel1.Width = 130;
                panel1.Padding = new Padding(10);

                lLine.Location = new Point(130, 65);
                lLine.Size = new Size(this.ClientSize.Width - 130, 40);

                label2.Font = new Font("Calibri", 10F, FontStyle.Bold);
                label2.Location = new Point(140, 76);

                label4.Font = new Font("Calibri", 10F, FontStyle.Bold);
                label4.Location = new Point(label2.Right + 5, 76);

                AturFontMenu(9F, new Padding(5, 6, 5, 6));
            }

            ResumeLayout();
        }

        // ===================== LOAD FORM =====================

        private void buku_admin_Load(object sender, EventArgs e)
        {
            tJudul.MaxLength = 50;
            tIsbn.MaxLength = 16;
            tThnterbit.MaxLength = 4;

            cPenulis.DropDownStyle = ComboBoxStyle.DropDownList;
            cKategori.DropDownStyle = ComboBoxStyle.DropDownList;
            cPenerbit.DropDownStyle = ComboBoxStyle.DropDownList;
            cRak.DropDownStyle = ComboBoxStyle.DropDownList;

            label4.Text = Model.name;

            AturLayout();

            showNamaPenulis();
            showNamaKategori();
            showNamaPenerbit();
            showNamaRak();

            showData();
        }

        // ===================== NAVIGASI MENU =====================

        private void button1_Click_1(object sender, EventArgs e)
        {
            Admin panggil = new Admin();
            panggil.Show();
            this.Hide();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            penulis_admin panggil = new penulis_admin();
            panggil.Show();
            this.Hide();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            penerbit_admin panggil = new penerbit_admin();
            panggil.Show();
            this.Hide();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            kategori_admin panggil = new kategori_admin();
            panggil.Show();
            this.Hide();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            rak_admin panggil = new rak_admin();
            panggil.Show();
            this.Hide();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            buku_admin panggil = new buku_admin();
            panggil.Show();
            this.Hide();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            peminjaman_admin panggil = new peminjaman_admin();
            panggil.Show();
            this.Hide();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Login panggil = new Login();
            panggil.Show();
        }

        // ===================== EVENT KOSONG (dari Designer) =====================

        private void label1_Click(object sender, EventArgs e) { }

        private void label4_Click(object sender, EventArgs e) { }

        private void panel1_Paint(object sender, PaintEventArgs e) { }

        private void label2_Click(object sender, EventArgs e) { }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) { }

        private void lLine_Click(object sender, EventArgs e) { }

        private void label5_Click(object sender, EventArgs e) { }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }

        private void label8_Click(object sender, EventArgs e) { }

        // ===================== EVENT TOMBOL =====================

        private void cariBuku_TextChanged(object sender, EventArgs e)
        {
            searchData();
        }

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
        private void panel3_Paint(object sender, PaintEventArgs e) { }
    }
}