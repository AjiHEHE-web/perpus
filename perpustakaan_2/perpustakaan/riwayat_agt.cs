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
    public partial class riwayat_agt : Form
    {
        public SqlConnection connection = new SqlConnection(koneksi.conn); // untuk koneksi
        public SqlCommand command; // untuk eksekusi perintah sql
        public SqlDataAdapter adapter; // ambil data dari sqlcommand
        public DataTable tabel; // untuk tabel menyimpan hasil query

        public riwayat_agt()
        {
            InitializeComponent();
            label4.Text = Model.name;
            this.Resize += riwayat_agt_Resize;
            this.WindowState = FormWindowState.Maximized;
            AturLayout();
            showData();
        }

        private void riwayat_agt_Resize(object sender, EventArgs e)
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
            int contentWidth = Math.Max(
                0,
                this.ClientSize.Width - contentLeft - (maximized ? 15 : 10)
            );
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
            int gap = maximized ? 12 : 7;
            int availableHeight =
                panel1.ClientSize.Height - (verticalMargin * 2) - (gap * 3);
            int buttonHeight = Math.Min(
                maximized ? 52 : 43,
                Math.Max(0, availableHeight / 4)
            );
            int buttonWidth = Math.Max(
                0,
                panel1.ClientSize.Width - (horizontalMargin * 2)
            );
            Button[] buttons = { button1, button2, button3, button4 };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Font = maximized
                    ? new Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
                    : new Font("Microsoft Sans Serif", 10F);
                buttons[i].Location = new Point(
                    horizontalMargin,
                    verticalMargin + (i * (buttonHeight + gap))
                );
                buttons[i].Size = new Size(buttonWidth, buttonHeight);
            }

            label3.Location = new Point(
                contentLeft + Math.Max(0, (contentWidth - label3.Width) / 2),
                lLine.Bottom + (maximized ? 18 : 16)
            );

            int searchGap = 8;
            int searchWidth = Math.Min(
                maximized ? 420 : 300,
                Math.Max(0, contentWidth - label5.Width - searchGap)
            );
            int searchRowWidth = label5.Width + searchGap + searchWidth;
            int searchLeft = contentLeft + Math.Max(0, (contentWidth - searchRowWidth) / 2);
            int searchTop = label3.Bottom + (maximized ? 20 : 16);
            label5.Location = new Point(searchLeft, searchTop);
            cari.Location = new Point(
                label5.Right + searchGap,
                searchTop + Math.Max(0, (label5.Height - cari.Height) / 2)
            );
            cari.Width = searchWidth;

            int gridTop = searchTop + Math.Max(label5.Height, cari.Height) + (maximized ? 16 : 14);
            int gridRight = this.ClientSize.Width - (maximized ? 15 : 10);
            int gridBottom = this.ClientSize.Height - (maximized ? 20 : 12);
            dataGridView1.Location = new Point(contentLeft, gridTop);
            dataGridView1.Size = new Size(
                Math.Max(0, gridRight - contentLeft),
                Math.Max(0, gridBottom - gridTop)
            );
        }

        public void showData()
        {
            using (SqlConnection connection = new SqlConnection(koneksi.conn))
            {
                try
                {
                    connection.Open();

                    string sql = "SELECT pd.peminjaman_detail_peminjaman_id, buku.buku_judul, p.peminjaman_tglpinjam, " +
                        "CASE WHEN p.peminjaman_statuskembali = 1 THEN 'Dikembalikan' ELSE 'Dipinjam' END AS status_kembali, p.peminjaman_note, p.peminjaman_denda " +
                        "FROM peminjaman_detail pd " +
                        "JOIN peminjaman p ON p.peminjaman_id = pd.peminjaman_detail_peminjaman_id " +
                        "JOIN buku ON buku.buku_id = pd.peminjaman_detail_buku_id " +
                        "WHERE p.peminjaman_user_id = @userid";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@userid", Model.iduser);

                    adapter = new SqlDataAdapter(command);
                    tabel = new DataTable();
                    adapter.Fill(tabel);

                    dataGridView1.DataSource = tabel;
                    dataGridView1.Columns[0].HeaderText = "Id Peminjaman";
                    dataGridView1.Columns[1].HeaderText = "Judul Buku";
                    dataGridView1.Columns[2].HeaderText = "Tanggal Pinjam";
                    dataGridView1.Columns[3].HeaderText = "Status Kembali";
                    dataGridView1.Columns[4].HeaderText = "Note";
                    dataGridView1.Columns[5].HeaderText = "Denda";
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
                    string sql = "SELECT pd.peminjaman_detail_peminjaman_id, buku.buku_judul, p.peminjaman_tglpinjam, " +
                        "p.peminjaman_statuskembali, p.peminjaman_note, p.peminjaman_denda " +
                        "FROM peminjaman_detail pd " +
                        "JOIN peminjaman p ON p.peminjaman_id = pd.peminjaman_detail_peminjaman_id " +
                        "JOIN buku ON buku.buku_id = pd.peminjaman_detail_buku_id " +
                        "WHERE (" +
                            "p.peminjaman_id LIKE '%' + @search + '%' OR " +
                            "buku.buku_judul LIKE '%' + @search + '%' OR " +
                            "p.peminjaman_tglpinjam LIKE '%' + @search + '%' OR " +
                            "(CASE WHEN p.peminjaman_statuskembali = 1 THEN 'Dikembalikan' ELSE 'Dipinjam' END) LIKE '%' + @search + '%' OR " +
                            "p.peminjaman_note LIKE '%' + @search + '%' OR " +
                            "p.peminjaman_denda LIKE '%' + @search + '%'" +
                        ") AND p.peminjaman_user_id = @userid";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@userid", Model.iduser);
                    command.Parameters.AddWithValue("@search", cari.Text);

                    adapter = new SqlDataAdapter(command);
                    tabel = new DataTable();
                    adapter.Fill(tabel);

                    dataGridView1.DataSource = tabel;
                    dataGridView1.Columns[0].HeaderText = "Id Peminjaman";
                    dataGridView1.Columns[1].HeaderText = "Judul Buku";
                    dataGridView1.Columns[2].HeaderText = "Tanggal Pinjam";
                    dataGridView1.Columns[3].HeaderText = "Status Kembali";
                    dataGridView1.Columns[4].HeaderText = "Note";
                    dataGridView1.Columns[5].HeaderText = "Denda";
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



        private void riwayat_agt_Load(object sender, EventArgs e)
        {

        }

        private void cari_TextChanged(object sender, EventArgs e)
        {
            searchData();
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
    }
}
