// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Masukkan sebuah angka");
            int angka = int.Parse(Console.ReadLine());
            Console.WriteLine("Anda memasukkan angka: " + angka.ToString());
            Console.ReadLine();
        }
    }
}
