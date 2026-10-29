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

            this.Resize += Anggota_Resize;
            this.WindowState = FormWindowState.Maximized;
            AturLayout();
        }

        private void Anggota_Resize(object sender, EventArgs e)
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
