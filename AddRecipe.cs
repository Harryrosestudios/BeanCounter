using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BeanCounter
{
    class AddRecipe
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var recipe = new Recipe();

                // 1. Blend Name
                Console.Write("Enter Blend Name: ");
                recipe.BlendName = ReadNonEmptyString();

                // 2. Coffee Type
                var coffeeTypes = new List<string> { "Beans", "Instant", "Decaf", "Cold Brew" };
                recipe.CoffeeType = SelectFromMenu("Select Coffee Type:", coffeeTypes);

                // 3. Roast Name and Price
                Console.Write("Enter Roast Name and Price (e.g., \"Italian 06.24\"): ");
                (string roastName, decimal roastPrice) = ReadRoastNameAndPrice();
                recipe.RoastName = roastName;
                recipe.RoastPrice = roastPrice;

                // 4. Milk Type
                var milkTypes = new List<string> { "Whole", "Skim", "Oat", "Almond" };
                recipe.MilkType = SelectFromMenu("Select Milk Type:", milkTypes);

                // 5. Syrups
                recipe.Syrups = new List<Syrup>();
                while (true)
                {
                    Console.Write("Add a syrup (\"SyrupName NumberOfPumps Price\") or \"done\": ");
                    var syrupInput = Console.ReadLine();
                    if (syrupInput == null)
                    {
                        Console.WriteLine("Input cannot be null. Please try again.");
                        continue;
                    }
                    syrupInput = syrupInput.Trim();
                    if (string.Equals(syrupInput, "done", StringComparison.OrdinalIgnoreCase))
                        break;

                    if (TryParseSyrup(syrupInput, out Syrup? syrup))
                    {
                        recipe.Syrups.Add(syrup!);
                    }
                    else
                    {
                        Console.WriteLine("Invalid syrup format. Please try again.");
                    }
                }

                // 6. Print Summary
                PrintSummary(recipe);

                // 7. Confirm or Restart
                Console.Write("Confirm and save? (Y/N): ");
                var confirm = Console.ReadLine()?.Trim().ToUpper();
                if (confirm == "Y")
                {
                    SaveRecipe(recipe);
                    Console.WriteLine("Recipe saved to recipes.json!");
                    break;
                }
                else
                {
                    Console.WriteLine("Restarting entry...\n");
                }
            }
        }

        static string ReadNonEmptyString()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();
                Console.Write("Input cannot be empty. Please try again: ");
            }
        }

        static string SelectFromMenu(string prompt, List<string> options)
        {
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($"{i + 1}. {options[i]}");

            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (int.TryParse(line, out int choice)
                    && choice >= 1 && choice <= options.Count)
                {
                    return options[choice - 1];
                }
                Console.WriteLine("Invalid selection. Please enter a number from the list.");
            }
        }

        static (string, decimal) ReadRoastNameAndPrice()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.Write("Input cannot be empty. Try again: ");
                    continue;
                }

                var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    Console.Write("Invalid format. Use \"RoastName Price\" (e.g., Italian 06.24): ");
                    continue;
                }

                if (decimal.TryParse(parts[1], out decimal price) && price >= 0)
                {
                    return (parts[0], price);
                }

                Console.Write("Invalid price. Please enter a non-negative decimal number: ");
            }
        }

        static bool TryParseSyrup(string input, out Syrup? syrup)
        {
            syrup = null;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                return false;

            string name = parts[0];
            if (!int.TryParse(parts[1], out int pumps) || pumps < 0)
                return false;

            if (!decimal.TryParse(parts[2], out decimal price) || price < 0)
                return false;

            syrup = new Syrup { Name = name, Pumps = pumps, Price = price };
            return true;
        }

        static void PrintSummary(Recipe recipe)
        {
            Console.WriteLine("\n--- Recipe Summary ---");
            Console.WriteLine($"Blend: {recipe.BlendName}");
            Console.WriteLine($"Coffee Type: {recipe.CoffeeType}");
            Console.WriteLine($"Roast: {recipe.RoastName} (£{recipe.RoastPrice:0.00})");
            Console.WriteLine($"Milk: {recipe.MilkType}");
            Console.WriteLine("Syrups:");
            if (recipe.Syrups == null || recipe.Syrups.Count == 0)
            {
                Console.WriteLine("  (none)");
            }
            else
            {
                foreach (var s in recipe.Syrups)
                    Console.WriteLine($"  - {s.Name}, {s.Pumps} pump{(s.Pumps != 1 ? "s" : "")}, £{s.Price:0.00}");
            }
            Console.WriteLine();
        }

        static void SaveRecipe(Recipe recipe)
        {
            const string file = "recipes.json";
            List<Recipe> recipes = new List<Recipe>();

            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    try
                    {
                        recipes = JsonSerializer.Deserialize<List<Recipe>>(json) ?? new List<Recipe>();
                    }
                    catch
                    {
                        // If JSON is corrupted or invalid, overwrite with new list
                        recipes = new List<Recipe>();
                    }
                }
            }

            recipes.Add(recipe);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var newJson = JsonSerializer.Serialize(recipes, options);
            File.WriteAllText(file, newJson);
        }
    }

    public class Recipe
    {
        public string? BlendName { get; set; }
        public string? CoffeeType { get; set; }
        public string? RoastName { get; set; }
        public decimal RoastPrice { get; set; }
        public string? MilkType { get; set; }
        public List<Syrup>? Syrups { get; set; }
    }

    public class Syrup
    {
        public string? Name { get; set; }
        public int Pumps { get; set; }
        public decimal Price { get; set; }
    }
}

