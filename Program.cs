using Microsoft.EntityFrameworkCore;
using BokhandelApp.Model;

namespace BokhandelApp
{
    class Program
    {
        private const string MenuChoice_ListStock = "1";
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
                    case MenuChoice_ListStock:
                        ListStockBalance();
                        break;
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

            Console.WriteLine(" 1 -> Lista lagersaldo för butik");
            Console.WriteLine(" 0 -> Avsluta");

            Console.Write("\nVälj ett alternativ: ");
        }
        
        static void ListStockBalance()
        {
            try
            {
                using var context = new BookstoreLabbContext();
                
                Console.Clear();
                Console.WriteLine("\n <-- BUTIKER -->\n");

                var stores = context.Stores.ToList();

                if (!stores.Any())
                {
                    Console.WriteLine("Inga butiker hittades i databasen.");
                    PressKeyToContinue();
                    return;
                }

                foreach (var store in stores)
                {
                    Console.WriteLine($"{store.StoreId}. {store.StoreName}");
                }

                Console.Write("\nVälj butik: ");
                
                if (!int.TryParse(Console.ReadLine()?.Trim(), out int storeId))
                {
                    Console.WriteLine("Ogiltigt val. Vänligen ange ett numeriskt värde.");
                    PressKeyToContinue();
                    return;
                }

                var selectedStore = context.Stores
                    .Include(store => store.BookBalances)
                        .ThenInclude(bookbalance => bookbalance.IsbnNavigation)
                    .FirstOrDefault(store => store.StoreId == storeId);

                if (selectedStore == null)
                {
                    Console.WriteLine("Butiken hittades inte. Kontrollera nummret och försök igen.");
                    PressKeyToContinue();
                    return;
                }

                // fortsätt här!!!!!!
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod: {ex.Message}");
                Console.WriteLine("Kontrollera databasanslutningen och försök igen.");
            }
            finally
            {
                PressKeyToContinue();
            }
        }


        static void PressKeyToContinue()
        {
            Console.WriteLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
            Console.ReadKey(intercept: true);
        }
    }
}