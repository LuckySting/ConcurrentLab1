using System;
using System.Diagnostics;
using System.Threading;

namespace ConcurrentLab1
{
    class Program
    {
        static int[] a = null;
        static int[] b = null;
        static int complexity = (int)Math.Pow(10, 2);

        static void basicCalc(object o)
        {
            var payload = (int[])o;
            int start = payload[0];
            int end = payload[1];
            int step = payload[2];
            for(int i = start; i < end; i+=step)
            {
                int temp = a[i];
                for(int j=0; j<complexity; j++)
                {
                    temp = temp * j - j;
                }
                b[i] = temp;
            }
        }
        static void increaseCalc(object o)
        {
            var payload = (int[])o;
            int start = payload[0];
            int end = payload[1];
            int step = payload[2];
            for (int i = start; i < end; i += step)
            {
                int temp = a[i];
                for (int j = 0; j < a[i]; j++)
                {
                    temp = temp * j - j;
                }
                b[i] = temp;
            }
        }

        static void fillData(int n)
        {
            a = new int[n];
            b = new int[n];
            for(int i = 0; i < n; i++)
            {
                a[i] = i;
            }
        }

        static void basicRun(ParameterizedThreadStart func)
        {
            var sw = Stopwatch.StartNew();
            func(new int[] { 0, a.Length, 1});
            sw.Stop();
            Console.Write(sw.Elapsed.TotalMilliseconds);
            Console.Write("; ");
        }

        static void parallelRun(int m, ParameterizedThreadStart func)
        {
            var threads = new Thread[m];
            for (int i = 0; i < m; i++)
            {
                    threads[i] = new Thread(func);
            }
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < m; i++)
            {
                int start = (i * a.Length) / m;
                int stop = ((i + 1) * a.Length) / m;
                int step = 1;
                threads[i].Start(new int[] { start, stop, step });
            }
            for (int i = 0; i < m; i++)
            {
                threads[i].Join();
            }
            sw.Stop();
            Console.Write(sw.Elapsed.TotalMilliseconds);
            Console.Write("; ");
        }

        static void circularRun(int m, ParameterizedThreadStart func)
        {
            var threads = new Thread[m];
            for (int i = 0; i < m; i++)
            {
                threads[i] = new Thread(func);
            }
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < m; i++)
            {
                int start = i;
                int stop = a.Length;
                int step = m;
                threads[i].Start(new int[] { start, stop, step });
            }
            for (int i = 0; i < m; i++)
            {
                threads[i].Join();
            }
            sw.Stop();
            Console.Write(sw.Elapsed.TotalMilliseconds);
            Console.Write("; ");
        }
        static void Main(string[] args)
        {
            int[] ns = { 10, 100, 1000, 100000 };
            int[] ms = { 2, 3, 4, 5, 10 };
            foreach (var m in ms)
            {
                foreach (var n in ns)
                {
                    fillData(n);
                    circularRun(m, increaseCalc);
                }
                Console.WriteLine();
            }
         
        }
    }
}
