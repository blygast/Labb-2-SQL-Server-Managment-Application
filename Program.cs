using Microsoft.EntityFrameworkCore;
using BokhandelApp.Model;

namespace BokhandelApp
{
    class Program
    {
        private const string MenuChoice_ListStock = "1";
        private const string MenuChoice_AddBook = "2";
        private const string MenuChoice_Exit = "0";
        private const int TitleMaxLength = 30;
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
                    case MenuChoice_AddBook:
                        AddBookToStore();
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
            Console.WriteLine(" 2 -> Lägg till bok i butik");
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

                DisplayStockForStore(selectedStore);
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

        static void DisplayStockForStore(Store selectedStore)
        {
            Console.Clear();
            Console.WriteLine($"\n <-- Lagersaldo för: {selectedStore.StoreName} -->\n");
            Console.WriteLine($"{"ISBN",-15} {"Titel",-30} {"Antal",-10}");
            Console.WriteLine(new string('-', 60));

            if (!selectedStore.BookBalances.Any())
            {
                Console.WriteLine("Lagerstatus saknas för denna butik.");
                return;
            }

            var orderedBalances = selectedStore.BookBalances
                .OrderBy(b => b.IsbnNavigation.Title);

            foreach (var balance in orderedBalances)
            {
                string title = TruncateString(balance.IsbnNavigation.Title, TitleMaxLength);
                Console.WriteLine($"{balance.Isbn,-15} {title,-30} {balance.AmountInStock,-10}");
            }
        }

        static string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Length <= maxLength)
                return value;

            return value[..(maxLength - 3)] + "...";
        }
        
        static void AddBookToStore()
        {
            try
            {
                using var context = new BookstoreLabbContext();
                
                Console.Clear();
                Console.WriteLine("\n <-- LÄGG TILL BOK I BUTIK -->\n");

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

                var selectedStore = context.Stores.FirstOrDefault(s => s.StoreId == storeId);

                if (selectedStore == null)
                {
                    Console.WriteLine("Butiken hittades inte. Kontrollera nummret och försök igen.");
                    PressKeyToContinue();
                    return;
                }

                Console.Clear();
                Console.WriteLine($"\n <-- BÖCKER -->\n");
                Console.WriteLine($"{"ISBN",-15} {"Titel",-40}");
                Console.WriteLine(new string('-', 60));

                var allBooks = context.Books.OrderBy(b => b.Title).ToList();

                if (!allBooks.Any())
                {
                    Console.WriteLine("Inga böcker hittades i databasen.");
                    PressKeyToContinue();
                    return;
                }

                foreach (var book in allBooks)
                {
                    string title = TruncateString(book.Title, 40);
                    Console.WriteLine($"{book.Isbn,-15} {title,-40}");
                }

                Console.Write("\nAnge ISBN för boken du vill lägga till: ");
                string? isbn = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(isbn))
                {
                    Console.WriteLine("Ogiltigt ISBN.");
                    PressKeyToContinue();
                    return;
                }

                var selectedBook = context.Books.FirstOrDefault(b => b.Isbn == isbn);

                if (selectedBook == null)
                {
                    Console.WriteLine("Boken hittades inte. Kontrollera ISBN och försök igen.");
                    PressKeyToContinue();
                    return;
                }

                var existingBalance = context.BookBalances
                    .FirstOrDefault(bb => bb.StoreId == storeId && bb.Isbn == isbn);

                if (existingBalance != null)
                {
                    existingBalance.AmountInStock++;
                    context.SaveChanges();
                    Console.WriteLine($"\nBoken '{TruncateString(selectedBook.Title, 40)}' har lagts till i {selectedStore.StoreName}.");
                    Console.WriteLine($"Nytt antal i lager: {existingBalance.AmountInStock}");
                }
                else
                {
                    Console.Write("\nBoken finns inte i butiken. Ange antal att lägga till: ");
                    
                    if (!int.TryParse(Console.ReadLine()?.Trim(), out int quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Ogiltigt antal. Måste vara ett positivt heltal.");
                        PressKeyToContinue();
                        return;
                    }

                    var newBalance = new BookBalance
                    {
                        StoreId = storeId,
                        Isbn = isbn,
                        AmountInStock = quantity
                    };

                    context.BookBalances.Add(newBalance);
                    context.SaveChanges();
                    Console.WriteLine($"\nBoken '{TruncateString(selectedBook.Title, 40)}' har lagts till i {selectedStore.StoreName}.");
                    Console.WriteLine($"Antal i lager: {quantity}");
                }
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