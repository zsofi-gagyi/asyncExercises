using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RefactoringProofOfConcept
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Task task = null;

            for (int i = 0; i < 5; i++)
            {
                task = Task.Factory.StartNew(() =>
                {
                    Blocking(); // Awaited(); 
                });
                Console.WriteLine($"Round {i} finished");
            }

            task.Wait();

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine($"It took {elapsedTime}"); // 00.05.26 / 00.05.26 / 00.05.27 with Blocking, 
                                                         // 00.00.49 / 00.00.60 / 00.00.48 with Awaited
        }

        private static async Task<string> Blocking()
        {
            for (int i = 0; i < 10; i++)
            {
                var result = SpendTime();
                var output =  result.Result;
                Console.WriteLine($"Output nr {i} was just calculated from Blocking! \n {output} \n");
            }

            return $"Blocking";
        }

        private static async Task<string> Awaited()
        {
            for (int i = 0; i < 10; i++)
            {
                var result = SpendTime();
                var output = await result;
                Console.WriteLine($"Output nr {i} was just calculated from Awaited! \n {output} \n");
            }

            return $"Awaited";
        }

        private static async Task<string> SpendTime()
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(80);
                return "time spent! by" + Task.CurrentId + "\n";
            });
        }
    }
}
