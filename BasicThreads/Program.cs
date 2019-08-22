using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncExercise
{
    class Program
    {
        static void Main(string[] args)
        {
            var planned = new CancellationTokenSource();
            planned.Token.Register(() =>
            {
                Console.WriteLine("\n timed out \n");
            });

            var interrupted = new CancellationTokenSource();
            interrupted.Token.Register(() =>
            {
                Console.WriteLine("\n interrupted \n");
            });

            var allCauses = CancellationTokenSource.CreateLinkedTokenSource(planned.Token, interrupted.Token);
            var token = allCauses.Token;
            token.Register(() =>
            {
                Console.WriteLine("\n stopped for one reason or another\n");
            });

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1200);
                planned.Cancel();
            });

            Task.Factory.StartNew(() =>
            {
                Console.ReadLine(); //"press any key" plus enter
                interrupted.Cancel();
            });

            //-----------------------------
            var slowCounter = new Task(() => DrawBarCancellaby(2000, interrupted.Token, "cancelous"));
            slowCounter.Start();

            var slowCounter2 = new Task(() => DrawBarCancellaby(3000, planned.Token, "planned"));
            slowCounter2.Start();

            var slowNumber = new Task<int>(SlowlyReturnANumber, "u");
            slowNumber.Start();

            var t1 = new Task(Write);
            t1.Start();

            var t2 = new Task(() => Write("string"));
            t2.Start();

            var t3 = new Task(Write, "?"); // only for an object overload
            t3.Start();

            Task.Factory.StartNew(() => Write("...."));
            //------------------------------

     //       Task.WaitAll(new[] { slowCounter }, 2000, token);

            var slowCountedResult = slowNumber.Result;
            Console.WriteLine("Bye World!" + slowCountedResult);


        }

        private static void Write(string s)
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.Write(s);
            }
        }

        private static void Write(object s)
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.Write(s);
            }
        }

        private static void Write()
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.Write("----");
            }
        }

        private static int SlowlyReturnANumber(object j)
        {
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("\n**********" + i + "**********\n");
                Thread.Sleep(1007);
            }

            return 7;
        }

        private static void DrawBarCancellaby(int time, CancellationToken token, string s)
        {
            Thread.Sleep(time);
            for (int i = 0; i < 1000; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("\n/////////.'.'.'.'.'cancelled'.'.'.'.'.'.'.'.'.'" + s + ".'.'.'.'.'.'.'.'.'.'.'.'.'.'.'/////////\n");
                    break;
                }
                else
                {
                    Console.WriteLine("\n/////////.'.'.'.'.'NOT cancelled .'.'.'.'.'.'.'.'.'.'" + s + ".'.'.'.'.'.'.'.'.'.'.'.'.'.'.'/////////\n");

                }
            }
        }
    }
}
