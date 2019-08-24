using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var random = new Random();
            var dinnerParty = new CancellationTokenSource();
            var notOnFire = new CancellationTokenSource();

            var cookMeal = CookMeal(random, dinnerParty.Token, notOnFire.Token);
            SometimesStartAFire(random, notOnFire);

            while (!cookMeal.IsCompleted)
            {
                if (Console.KeyAvailable)
                {
                    dinnerParty.Cancel();
                    Console.WriteLine("\n------ Bad news - the dinner party is cancelled! ------\n");
                    break;
                }
            }

            var ending = await cookMeal;
            Console.WriteLine(ending);
        }

        static async Task SometimesStartAFire(Random random, CancellationTokenSource notOnfire)
        {
            while (true)
            {
                if (random.Next(0, 11) > 9)
                {
                    notOnfire.Cancel();
                    Console.WriteLine("\n()()() The kitchen is on fire! ()()()\n");
                    return;
                }
                else
                {
                    await Task.Delay(random.Next(200, 500));
                }
            }
        }

        static async Task<string> CookMeal(Random random, CancellationToken dinnerParty, CancellationToken notOnFire)
        {
            var cook1 = "first cook";
            var cook2 = "second cook";

            var saltAlreadyBought = new CancellationTokenSource();

            var buySalt1 = BuySalt(cook1, random, saltAlreadyBought, notOnFire);
            var buySalt2 = BuySalt(cook2, random, saltAlreadyBought, notOnFire);

            await Task.WhenAll(buySalt1, buySalt2);
            Console.WriteLine();

            try
            {
                var cutMushrooms = CutIngredient(cook1, "mushrooms", random, notOnFire, dinnerParty);
                var cutVegetables = CutIngredient(cook2, "vegetables", random, notOnFire, dinnerParty);
                var cutChicken = cutVegetables
                                    .ContinueWith(o => CutIngredient(cook2, "chicken", random, notOnFire, dinnerParty));
                var preheatTheOven = PreheatTheOven(cook1, notOnFire, dinnerParty);
                await Task.WhenAll(cutChicken, preheatTheOven);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                Console.WriteLine("The other steps weren't even started");
                if (notOnFire.IsCancellationRequested)
                {
                    Console.WriteLine("The cooks have ran away and are now safe");
                }
                return ("Cooking abandoned");
            }


            try
            {
                Console.WriteLine();
                await FinishMeal(cook2, random, notOnFire, dinnerParty);
                return "Dinner is ready!";
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                if (notOnFire.IsCancellationRequested)
                {
                    Console.WriteLine("The cooks have ran away and are now safe");
                }
            }

            return ("Cooking abandoned");
        }

        static async Task BuySalt(string cook, Random random, CancellationTokenSource saltAlreadybought, CancellationToken notOnFire)
        {
            Console.WriteLine($"The {cook} has been notified that the kitchen has run out of salt");
            await Task.Delay(random.Next(400, 900));
            Console.WriteLine($"The {cook} has bought salt in the way to the kitchen");

            if (notOnFire.IsCancellationRequested)
            {
                Console.WriteLine($"Because of the fire, the {cook} will take his packet of salt home with him");
                return;
            }

            if (saltAlreadybought.IsCancellationRequested)
            {
                Console.WriteLine($"Since there is already salt there, our {cook} will take his packet of salt home with him");
            }
            else
            {
                saltAlreadybought.Cancel();
                Console.WriteLine($"We will cook with the salt our {cook} has bought");
            }
        }

        static async Task CutIngredient(string cook, string primeMaterial, Random random, CancellationToken notOnFire, CancellationToken dinnerParty)
        {

            if (notOnFire.IsCancellationRequested)
            {
                throw new OperationCanceledException($"The {primeMaterial} will not be cut, because the {cook} had to run away from the fire");
            }
            else if (dinnerParty.IsCancellationRequested)
            {
                Console.WriteLine($"Our {cook} has decided to cut the {primeMaterial}, even if no dinner guests will come, because they will be still useful for tomorrow");
            }

            Console.WriteLine($"The {cook} has started to cut the {primeMaterial}");
            await Task.Delay(random.Next(800, 1200));

            if (notOnFire.IsCancellationRequested)
            {
                throw new OperationCanceledException($"The cutting of {primeMaterial} has been abandoned midway, because the {cook} had to run away from the fire");
            }
            else
            {
                Console.WriteLine($"The {cook} has finished cutting the {primeMaterial}");
            }
        }

        static async Task PreheatTheOven(string cook, CancellationToken fire, CancellationToken dinnerParty)
        {
            var allCauses = CancellationTokenSource.CreateLinkedTokenSource(fire, dinnerParty);
            if (allCauses.IsCancellationRequested)
            {
                throw new OperationCanceledException("Oven preheating was cancelled");
            }

            Console.WriteLine($"The {cook} has set the oven to preheat");
            await Task.Delay(3000);
            Console.WriteLine("Oven is ready");
        }

        static async Task FinishMeal(string cook, Random random, CancellationToken notOnFire, CancellationToken dinnerParty)
        {
            if (dinnerParty.IsCancellationRequested)
            {
                throw new OperationCanceledException("With no guests in sight, the cooks just put the ingredients in the freezer");

            }

            if (notOnFire.IsCancellationRequested)
            {
                throw new OperationCanceledException("The meal's ingredients have burned to charcoal even without beign put into the oven");
            }

            Console.WriteLine($"The {cook} has mixed the ingredients and has put them in the preheated oven");
            await Task.Delay(random.Next(100, 300));
            Console.WriteLine("Time passes...");

            if (notOnFire.IsCancellationRequested)
            {
                throw new OperationCanceledException("The meal has burned to charcoal because the entire kitchen is on fire");
            }

            Console.WriteLine("The meal is ready");
        }
    }
}
