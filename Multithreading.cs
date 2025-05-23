using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Multithreading
{
    class Program
    {
        const int matrixSize = 10;
        static int[,] matrixA = new int[matrixSize, matrixSize];
        static int[,] matrixB = new int[matrixSize, matrixSize];
        static int[,] resultMatrix = new int[matrixSize, matrixSize];
        static CountdownEvent countdown = new CountdownEvent(matrixSize * matrixSize);

        static void Main(string[] args)
        {
            Random rand = new Random();

            for (int i = 0; i < matrixSize; i++)
            {
                for (int j = 0; j < matrixSize; j++)
                {
                    matrixA[i, j] = rand.Next(1, 10);
                    matrixB[i, j] = rand.Next(1, 10);
                }
            }

            Console.WriteLine("Matrix A:");
            PrintMatrix(matrixA);
            Console.WriteLine("\n");

            Console.WriteLine("Matrix B:");
            PrintMatrix(matrixB);
            Console.WriteLine("\n");

            for (int i = 0; i < matrixSize; i++)
            {
                for (int j = 0; j < matrixSize; j++)
                {
                    int row = i;
                    int col = j;

                    Thread thread = new Thread(ComputeElement);
                    thread.Start(new int[] { row, col });
                }
            }

            countdown.Wait();

            Console.WriteLine("\nResult Matrix:");
            PrintMatrix(resultMatrix);

            Console.ReadKey();
        }

        static void ComputeElement(object obj)
        {
            int[] indices = (int[])obj;
            int row = indices[0];
            int col = indices[1];

            int sum = 0;
            for (int k = 0; k < matrixSize; k++)
            {
                sum += matrixA[row, k] * matrixB[k, col];
            }

            resultMatrix[row, col] = sum;

            int threadId = Thread.CurrentThread.ManagedThreadId;
            uint core = GetCurrentProcessorNumber();
            Console.WriteLine($"Thread {threadId} computing element calculated on core {core}");

            countdown.Signal();
        }

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentProcessorNumber();

        static void PrintMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrixSize; i++)
            {
                for (int j = 0; j < matrixSize; j++)
                {
                    Console.Write($"{matrix[i, j],4} ");
                }
                Console.WriteLine();
            }
        }
    }
}
