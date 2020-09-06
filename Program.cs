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

        static void basicRun()
        {
            var sw = Stopwatch.StartNew();
            basicCalc(new int[] { 0, a.Length, 1});
            sw.Stop();
            Console.Write("Basic run basicCalc = ");
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            sw.Restart();
            increaseCalc(new int[] { 0, a.Length, 1 });
            sw.Stop();
            Console.Write("Basic run increaseCalc = ");
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        static void parallelRun(int m, string text, ParameterizedThreadStart func)
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
            Console.Write("    ");
            Console.Write(text);
            Console.Write(" ");
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        static void circularRun(int m)
        {
            var threads = new Thread[m];
            for (int i = 0; i < m; i++)
            {
                threads[i] = new Thread(basicCalc);
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
            Console.Write("    Circular run basicCalc = ");
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            for (int i = 0; i < m; i++)
            {
                threads[i] = new Thread(increaseCalc);
            }
            sw.Restart();
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
            Console.Write("    Circular run increaseCalc = ");
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }
        static void Main(string[] args)
        {
            int[] ns = { 10, 100, 1000, 100000 };
            int[] ms = { 2, 3, 4, 5, 10 };
            foreach (var n in ns)
            {
                fillData(n);
                Console.Write("N = ");
                Console.WriteLine(n);
                basicRun();
                foreach (var m in ms)
                {
                    Console.Write("    M = ");
                    Console.WriteLine(m);
                    parallelRun(m, "Parallel run basicCalc = ", basicCalc);
                    parallelRun(m, "Parallel run increaseCalc = ", increaseCalc);
                    circularRun(m);
                }
            }
         
        }
    }
}
