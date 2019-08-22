using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsyncExercise
{
    public static class Exercise
    {
        public static void Run()
        {
            var t1 = new Task(Write);
            t1.Start();

            var t2 = new Task(() => Write("string"));
            t2.Start();

            var t3 = new Task(() => Write(000));
        }

        private static void Write(string s)
        {
            for(int i = 0; i < 100; i++)
            {
                Console.WriteLine(s);
            }
        }

        private static void Write(int s)
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(s);
            }
        }

        private static void Write()
        {
           for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("----");
            }
        }
    }
}
