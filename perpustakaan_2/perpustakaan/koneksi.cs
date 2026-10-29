using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace perpustakaan
{
    internal class koneksi
    {
        public static string conn = "Data Source = LAPTOP-3C93U4H6\\SQLEXPRESS; Initial Catalog = perpustakaan; Integrated Security = True";

        public static void TampilkanHasilHapus(int jumlahBaris, string namaData)
        {
            if (jumlahBaris == 0)
            {
                MessageBox.Show(
                    "Data " + namaData + " tidak ditemukan atau sudah dihapus.",
                    "Informasi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            MessageBox.Show(
                "Data " + namaData + " berhasil dihapus.",
                "Informasi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        public static void TampilkanKesalahanHapus(SqlException exception, string namaData)
        {
            string pesan = exception.Number == 547
                ? "Data " + namaData + " tidak dapat dihapus karena masih digunakan oleh data lain. Hapus data terkait terlebih dahulu."
                : "Data " + namaData + " gagal dihapus: " + exception.Message;

            MessageBox.Show(
                pesan,
                "Penghapusan gagal",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }

    public class Model
    {
        public static string name {  get; set; }

        public static string iduser { get; set; }
        public static string username { get; set; }
    }
}
