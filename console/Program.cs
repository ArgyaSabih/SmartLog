using System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BubbleSortExample
{
    // Method utama untuk menjalankan program
    public static void Main(string[] args)
    {
        // 1. Membuat array dari integer yang diisi dengan angka sembarang.
        int[] numbers = { 5, 1, 4, 8, 2, 9, 3 };

        Console.WriteLine("Array sebelum diurutkan:");
        // 3. Memanggil method untuk menampilkan elemen array.
        PrintArray(numbers);

        Console.WriteLine("\nMemulai proses Bubble Sort...");
        // 5. Memanggil method bubble sort.
        BubbleSort(numbers);

        Console.WriteLine("\nArray setelah diurutkan:");
        PrintArray(numbers);
    }

    /// <summary>
    /// 3. Method untuk menampilkan setiap elemen dari sebuah array integer.
    /// </summary>
    /// <param name="arr">Array integer yang akan ditampilkan.</param>
    public static void PrintArray(int[] arr)
    {
        foreach (int number in arr)
        {
            Console.Write(number + " ");
        }
        Console.WriteLine(); // Pindah ke baris baru setelah selesai
    }

    public static void BubbleSort(int[] arr)
    {
        int n = arr.Length;
        bool swapped;
        for (int i = 0; i < n - 1; i++)
        {
            swapped = false;
            for (int j = 0; j < n - i - 1; j++)
            {
                // Jika elemen saat ini lebih besar dari elemen berikutnya
                if (arr[j] > arr[j + 1])
                {
                    // Panggil method untuk menukar posisi
                    Swap(arr, j, j + 1);
                    swapped = true;
                }
            }
             // Jika tidak ada pertukaran dalam satu iterasi penuh, array sudah terurut.
            if (swapped == false)
                break;
        }
    }

    public static void Swap(int[] arr, int index1, int index2)
    {
        // Simpan nilai pertama ke variabel sementara
        int temp = arr[index1];
        // Pindahkan nilai kedua ke posisi pertama
        arr[index1] = arr[index2];
        // Pindahkan nilai dari variabel sementara ke posisi kedua
        arr[index2] = temp;
    }
}