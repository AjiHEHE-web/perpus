using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace perpustakaan
{
    internal class koneksi
    {
        public static string conn = "Data Source = LAPTOP-3C93U4H6\\SQLEXPRESS; Initial Catalog = perpustakaan; Integrated Security = True";
    }

    public class Model
    {
        public static string name {  get; set; }
    }
}
