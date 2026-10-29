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
    public partial class peminjaman_agt : Form
    {
        public SqlConnection connection = new SqlConnection(koneksi.conn);
        public SqlCommand command;
        public SqlDataAdapter adapter;
        public DataTable tabel;
        public SqlDataReader reader;
        List<string> keranjangBuku = new List<string>();
        public peminjaman_agt()
        {
            InitializeComponent();
            label4.Text = Model.name;
            this.Resize += peminjaman_agt_Resize;
            this.WindowState = FormWindowState.Maximized;
            AturLayout();

            showIdBuku();

            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "ID Buku";
            dataGridView1.Columns[1].Name = "Judul Buku";
        }

        private void peminjaman_agt_Resize(object sender, EventArgs e)
        {
            AturLayout();
        }

        private void AturLayout()
        {
            bool maximized = this.WindowState == FormWindowState.Maximized;
            int headerHeight = maximized ? 87 : 65;
            int sidebarWidth = maximized ? 220 : 130;

            pHeader.Height = headerHeight;
            label1.Font = maximized
                ? new Font("Arial", 21.75F, FontStyle.Bold)
                : new Font("Arial", 18F, FontStyle.Bold);
            label1.Location = new Point(
                Math.Max(0, (pHeader.ClientSize.Width - label1.Width) / 2),
                Math.Max(0, (headerHeight - label1.Height) / 2)
            );

            panel1.Width = sidebarWidth;
            panel1.Padding = maximized
                ? new Padding(25, 23, 25, 23)
                : new Padding(10);

            label2.Font = maximized
                ? new Font("Calibri", 14.25F, FontStyle.Bold)
                : new Font("Calibri", 10F, FontStyle.Bold);
            label4.Font = maximized
                ? new Font("Calibri", 14.25F, FontStyle.Bold)
                : new Font("Calibri", 10F, FontStyle.Bold);

            int contentLeft = sidebarWidth + (maximized ? 10 : 7);
            int contentRight = this.ClientSize.Width - (maximized ? 15 : 10);
            int contentWidth = Math.Max(0, contentRight - contentLeft);
            int welcomeTop = headerHeight + (maximized ? 12 : 10);
            label2.Location = new Point(contentLeft, welcomeTop);
            label4.Location = new Point(label2.Right + (maximized ? 8 : 5), welcomeTop);
            label4.AutoSize = false;
            label4.AutoEllipsis = true;
            label4.Size = new Size(
                Math.Max(0, this.ClientSize.Width - label4.Left - 15),
                label2.Height
            );

            lLine.Location = new Point(sidebarWidth, label2.Bottom + 9);
            lLine.Size = new Size(
                Math.Max(0, this.ClientSize.Width - sidebarWidth),
                2
            );

            int horizontalMargin = maximized ? 15 : 10;
            int verticalMargin = maximized ? 20 : 10;
            int navigationGap = maximized ? 12 : 7;
            int availableHeight =
                panel1.ClientSize.Height - (verticalMargin * 2) - (navigationGap * 3);
            int navigationButtonHeight = Math.Min(
                maximized ? 52 : 43,
                Math.Max(0, availableHeight / 4)
            );
            int navigationButtonWidth = Math.Max(
                0,
                panel1.ClientSize.Width - (horizontalMargin * 2)
            );
            Button[] navigationButtons = { button1, button2, button3, button4 };
            for (int i = 0; i < navigationButtons.Length; i++)
            {
                navigationButtons[i].Font = maximized
                    ? new Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
                    : new Font("Microsoft Sans Serif", 10F);
                navigationButtons[i].Location = new Point(
                    horizontalMargin,
                    verticalMargin + (i * (navigationButtonHeight + navigationGap))
                );
                navigationButtons[i].Size = new Size(
                    navigationButtonWidth,
                    navigationButtonHeight
                );
            }

            label3.Location = new Point(
                contentLeft + Math.Max(0, (contentWidth - label3.Width) / 2),
                lLine.Bottom + (maximized ? 18 : 16)
            );

            int panelGap = 18;
            int columnWidth = Math.Max(0, (contentWidth - panelGap) / 2);
            int panelTop = label3.Bottom + (maximized ? 14 : 12);
            int panelBottom = this.ClientSize.Height - (maximized ? 20 : 12);
            int actionPanelHeight = maximized ? 64 : 58;
            int actionPanelTop = panelBottom - actionPanelHeight;
            int bodyPanelHeight = Math.Max(
                0,
                actionPanelTop - (maximized ? 14 : 12) - panelTop
            );
            int leftColumn = contentLeft;
            int rightColumn = contentLeft + columnWidth + panelGap;

            panel2.Location = new Point(leftColumn, panelTop);
            panel2.Size = new Size(columnWidth, bodyPanelHeight);
            panel3.Location = new Point(leftColumn, actionPanelTop);
            panel3.Size = new Size(columnWidth, actionPanelHeight);
            panel4.Location = new Point(rightColumn, panelTop);
            panel4.Size = new Size(columnWidth, bodyPanelHeight);
            panel5.Location = new Point(rightColumn, actionPanelTop);
            panel5.Size = new Size(columnWidth, actionPanelHeight);

            idbuku.Width = Math.Max(0, panel2.ClientSize.Width - idbuku.Left - 12);
            label9.AutoSize = false;
            label10.AutoSize = false;
            label11.AutoSize = false;
            label9.AutoEllipsis = true;
            label10.AutoEllipsis = true;
            label11.AutoEllipsis = true;
            label9.Size = new Size(Math.Max(0, panel2.ClientSize.Width - label9.Left - 12), 20);
            label10.Size = new Size(Math.Max(0, panel2.ClientSize.Width - label10.Left - 12), 20);
            label11.Size = new Size(Math.Max(0, panel2.ClientSize.Width - label11.Left - 12), 20);

            int actionButtonMargin = 10;
            int actionButtonGap = 6;
            int actionButtonWidth = Math.Min(
                75,
                Math.Max(0, (panel3.ClientSize.Width - (actionButtonMargin * 2) - (actionButtonGap * 2)) / 3)
            );
            Button[] cartButtons = { button5, button7, button6 };
            for (int i = 0; i < cartButtons.Length; i++)
            {
                cartButtons[i].Size = new Size(actionButtonWidth, 28);
                cartButtons[i].Location = new Point(
                    actionButtonMargin + (i * (actionButtonWidth + actionButtonGap)),
                    Math.Max(0, (panel3.ClientSize.Height - cartButtons[i].Height) / 2)
                );
            }

            int gridMargin = 12;
            dataGridView1.Location = new Point(gridMargin, gridMargin);
            dataGridView1.Size = new Size(
                Math.Max(0, panel4.ClientSize.Width - (gridMargin * 2)),
                Math.Max(0, panel4.ClientSize.Height - (gridMargin * 2))
            );

            int saveButtonWidth = Math.Min(220, Math.Max(0, panel5.ClientSize.Width - 24));
            button8.Size = new Size(saveButtonWidth, 32);
            button8.Location = new Point(
                Math.Max(0, (panel5.ClientSize.Width - button8.Width) / 2),
                Math.Max(0, (panel5.ClientSize.Height - button8.Height) / 2)
            );
        }

        public void showIdBuku()
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open(); // Buka koneksi

                    string sql = "SELECT buku_id FROM buku"; // Query SQL untuk mengambil data
                    command = new SqlCommand(sql, connection);

                    reader = command.ExecuteReader(); // Eksekusi query dan ambil data

                    idbuku.Items.Clear(); // Bersihkan ComboBox sebelum mengisi data

                    while (reader.Read()) // Mengisi ComboBox dengan data dari database
                    {
                        // Mengambil data dari kolom "buku_id"
                        idbuku.Items.Add(reader["buku_id"].ToString());
                    }
                    reader.Close(); // Tutup reader
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at:" + ex);
                }
            }
        }

        private void peminjaman_agt_Load(object sender, EventArgs e)
        {

        }

        private void idbuku_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (idbuku.SelectedItem != null)
            {
                using (SqlConnection connection = new SqlConnection(koneksi.conn))
                {
                    try
                    {
                        connection.Open(); // Buka koneksi

                        // Query untuk mengambil data
                        string sql = "SELECT buku.buku_judul, penulis.penulis_nama, penerbit.penerbit_nama " +
                                     "FROM buku " +
                                     "JOIN penulis ON penulis.penulis_id = buku.buku_penulis_id " +
                                     "JOIN penerbit ON penerbit.penerbit_id = buku.buku_penerbit_id " +
                                     "WHERE buku_id = @bukuid";

                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@bukuid", idbuku.Text);

                        reader = command.ExecuteReader(); // Eksekusi query dan ambil data

                        if (reader.Read()) // Jika data ditemukan, tampilkan di label
                        {
                            label9.Text = reader["buku_judul"].ToString();
                            label10.Text = reader["penulis_nama"].ToString();
                            label11.Text = reader["penerbit_nama"].ToString();
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (idbuku.SelectedItem == null)
            {
                MessageBox.Show("Silakan pilih buku terlebih dahulu!");
                return;
            }

            string idBuku = idbuku.SelectedItem.ToString();
            string judulBuku = label7.Text;

            if (!keranjangBuku.Contains(idBuku))
            {
                keranjangBuku.Add(idBuku);
                dataGridView1.Rows.Add(idBuku, judulBuku);
            }
            else
            {
                MessageBox.Show("Buku sudah ada di keranjang!");    
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                idbuku.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih buku yang mau diedit!");
                return;
            }

            if (idbuku.SelectedItem == null)
            {
                MessageBox.Show("Silakan pilih buku baru untuk mengganti!");
                return;
            }

            string bukuBaru = idbuku.SelectedItem.ToString();
            int selectedIndex = dataGridView1.SelectedRows[0].Index;

            // Buku lama
            string bukuLama = dataGridView1.Rows[selectedIndex].Cells[0].Value.ToString();

            // Jika buku baru sudah ada di keranjang → tolak
            if (keranjangBuku.Contains(bukuBaru) && bukuBaru != bukuLama)
            {
                MessageBox.Show("Buku sudah ada di keranjang!");
                return;
            }

            // Update ke list
            keranjangBuku[selectedIndex] = bukuBaru;

            // Update tampilan DataGridView
            dataGridView1.Rows[selectedIndex].Cells[0].Value = bukuBaru;

            MessageBox.Show("Buku berhasil diubah!");

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih data yang mau dihapus!");
                return;
            }

            // Ambil ID buku dari baris yang dipilih
            string idBuku = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

            // Hapus dari list & datagrid
            keranjangBuku.Remove(idBuku);
            dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);

        }

        private void button8_Click(object sender, EventArgs e)
        {
           
            // TEMPEL DI DALAM method klik tombol "Save Peminjaman" (isi kurung kurawalnya saja)

            if (keranjangBuku.Count == 0)
            {
                MessageBox.Show("Keranjang masih kosong!");
                return;
            }
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                connection.Open();  // Buka koneksi
                try
                {
                    // insert ke tabel peminjaman
                    string sqlPinjam = @"INSERT INTO peminjaman
        (peminjaman_user_id, peminjaman_tglpinjam, peminjaman_tglkembali, peminjaman_statuskembali, peminjaman_note, peminjaman_denda)
        VALUES (@user, @tglpinjam, @tglkembali, @status, @note, @denda)";
                    command = new SqlCommand(sqlPinjam, connection);
                    command.Parameters.AddWithValue("@user", Model.iduser);
                    command.Parameters.AddWithValue("@tglpinjam", DateTime.Today);
                    command.Parameters.AddWithValue("@tglkembali", "");
                    command.Parameters.AddWithValue("@status", 0);   // 0 = belum dikembalikan (kolom bertipe bit, jobsheet: "Dipinjam")
                    command.Parameters.AddWithValue("@note", "");
                    command.Parameters.AddWithValue("@denda", 0);
                    command.ExecuteNonQuery();

                    // ambil peminjaman_id terakhir (ID dari trigger)
                    // diurutkan berdasarkan ANGKA-nya, karena kalau urut teks, PJ9 dianggap lebih besar dari PJ10
                    string sqlGetId = "SELECT TOP 1 peminjaman_id FROM peminjaman ORDER BY TRY_CAST(SUBSTRING(peminjaman_id, 3, 15) AS INT) DESC";
                    string peminjamanId;
                    using (SqlCommand cmdGet = new SqlCommand(sqlGetId, connection))
                    {
                        peminjamanId = cmdGet.ExecuteScalar().ToString();
                    }

                    // insert detail (banyak buku)
                    string sqlDetail = @"INSERT INTO peminjaman_detail
        (peminjaman_detail_peminjaman_id, peminjaman_detail_buku_id)
        VALUES (@pinjamid, @bukuid)";
                    foreach (string idBuku in keranjangBuku) // ambil dari List
                    {
                        command = new SqlCommand(sqlDetail, connection);
                        command.Parameters.AddWithValue("@pinjamid", peminjamanId);
                        command.Parameters.AddWithValue("@bukuid", idBuku);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Peminjaman berhasil disimpan!");
                    keranjangBuku.Clear();
                    dataGridView1.Rows.Clear();

                    // === REFRESH RIWAYAT SECARA OTOMATIS ===
                    // Dikomentari dulu: blok ini untuk versi UserControl (this.Parent) dan butuh class Riwayat.
                    // Di aplikasimu tiap halaman berupa Form, dan form riwayat belum ada, jadi akan error saat build.
                    // Aktifkan lagi kalau nanti memang pakai UserControl.
                    //
                    var parent = this.Parent;
                    if (parent != null)
                    {
                        riwayat_agt r = parent.Controls.OfType<riwayat_agt>().FirstOrDefault();
                        if (r != null)
                        {
                            r.showData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            riwayat_agt panggil = new riwayat_agt();
            panggil.Show();
            this.Hide();
        }
    }
}
