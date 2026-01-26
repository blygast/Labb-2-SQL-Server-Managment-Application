using Microsoft.EntityFrameworkCore;
using BokhandelApp.Model;

namespace BokhandelApp
{
    class Program
    {
        private const string MenuChoice_Exit = "0";

        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                DisplayMainMenu();

                string? choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case MenuChoice_Exit:
                        running = false;
                        Console.WriteLine("Avslutar programmet...");
                        break;
                    default:
                        Console.WriteLine("Ogiltigt val. Vänligen förösk igen.");
                        PressKeyToContinue();
                        break;
                }

            }
        }

        static void DisplayMainMenu()
        {
            Console.Clear();

            Console.WriteLine("\n <-- BOKHANDEL MENY -->\n");

            Console.WriteLine(" 0 -> Avsluta");

            Console.Write("\nVälj ett alternativ: ");
        }
        
        static void PressKeyToContinue()
        {
            Console.WriteLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
            Console.ReadKey(intercept: true);
        }
    }
}