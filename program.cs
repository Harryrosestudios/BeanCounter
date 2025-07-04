using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace BeanCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            var recipes = LoadRecipes("recipes.json");
            if (recipes.Count == 0)
            {
                Console.WriteLine("No recipes found in recipes.json. Please add recipes first.");
                return;
            }

            Console.WriteLine("Select a blend:");
            for (int i = 0; i < recipes.Count; i++)
                Console.WriteLine($"{i + 1}. {recipes[i].BlendName}");

            int recipeIdx = PromptInt(">", 1, recipes.Count) - 1;
            var recipe = recipes[recipeIdx];

            // Size selection
            Console.WriteLine("\nSelect size: [1] Small, [2] Medium, [3] Large, [4] Custom");
            int sizeChoice = PromptInt(">", 1, 4);
            double sizeMultiplier = 1.0;
            Dictionary<string, double> customUnits = new Dictionary<string, double>();

            if (sizeChoice == 1) sizeMultiplier = 1.0;
            else if (sizeChoice == 2) sizeMultiplier = 1.5;
            else if (sizeChoice == 3) sizeMultiplier = 2.0;
            else
            {
                Console.WriteLine("Custom units for each ingredient:");

                // Roast units
                double roastUnits = PromptDouble($"  {recipe.RoastName} units (default 1): ", 0.01, 100, 1);
                customUnits["roast"] = roastUnits;

                // Milk units
                double milkUnits = PromptDouble($"  {recipe.MilkType} units (default 1): ", 0.01, 100, 1);
                customUnits["milk"] = milkUnits;

                // Syrup units
                if (recipe.Syrups != null)
                {
                    for (int i = 0; i < recipe.Syrups.Count; i++)
                    {
                        var s = recipe.Syrups[i];
                        double syrupUnits = PromptDouble($"  {s.Name} syrup units (default 1): ", 0, 100, 1);
                        customUnits[$"syrup_{i}"] = syrupUnits;
                    }
                }
            }

            // Milk change
            string origMilk = recipe.MilkType ?? "";
            string[] milkOptions = { "Whole", "Skim", "Oat", "Almond" };
            Console.WriteLine($"\nChange milk? (current: {origMilk}) [Y/N]");
            bool changeMilk = PromptYesNo();
            string selectedMilk = origMilk;
            bool nonDairySurcharge = false;
            if (changeMilk)
            {
                Console.WriteLine("Select milk:");
                for (int i = 0; i < milkOptions.Length; i++)
                    Console.WriteLine($"{i + 1}. {milkOptions[i]}{(milkOptions[i] == origMilk ? " (original)" : "")}");
                int milkIdx = PromptInt(">", 1, milkOptions.Length) - 1;
                selectedMilk = milkOptions[milkIdx];
                if ((selectedMilk == "Oat" || selectedMilk == "Almond") && (origMilk == "Whole" || origMilk == "Skim"))
                    nonDairySurcharge = true;
            }

            // Decaf
            Console.WriteLine("\nMake it decaf? [Y/N]");
            bool isDecaf = PromptYesNo();

            // --- Price Calculation ---
            double roastUnitsFinal = sizeChoice == 4 ? customUnits["roast"] : sizeMultiplier;
            double milkUnitsFinal = sizeChoice == 4 ? customUnits["milk"] : sizeMultiplier;

            decimal roastPrice = recipe.RoastPrice * (decimal)roastUnitsFinal;
            decimal milkPrice = (recipe.MilkPrice ?? 0) * (decimal)milkUnitsFinal;

            // Syrups
            List<(string name, double units, decimal unitPrice, decimal total)> syrupBreakdown = new();
            if (recipe.Syrups != null)
            {
                for (int i = 0; i < recipe.Syrups.Count; i++)
                {
                    var s = recipe.Syrups[i];
                    double syrupUnits = sizeChoice == 4 ? customUnits[$"syrup_{i}"] : sizeMultiplier;
                    decimal total = (s.Price ?? 0) * (decimal)syrupUnits;
                    syrupBreakdown.Add((s.Name ?? "Syrup", syrupUnits, s.Price ?? 0, total));
                }
            }

            // Extras
            decimal extras = 0;
            if (nonDairySurcharge) extras += 0.40m;
            if (isDecaf) extras -= 0.40m;

            // Total
            decimal totalPrice = roastPrice + milkPrice + extras;
            foreach (var s in syrupBreakdown) totalPrice += s.total;

            // --- Output ---
            Console.WriteLine("\n--- Drink Summary ---");
            Console.WriteLine($"Blend: {recipe.BlendName}");
            Console.WriteLine($"Size: {(sizeChoice == 1 ? "Small" : sizeChoice == 2 ? "Medium" : sizeChoice == 3 ? "Large" : "Custom")}" +
                              $"{(sizeChoice == 4 ? "" : $" ({sizeMultiplier}x)")}");
            Console.WriteLine($"Milk: {selectedMilk}{(nonDairySurcharge ? " (+£0.40)" : "")}");
            Console.WriteLine($"Decaf: {(isDecaf ? "Yes (–£0.40)" : "No")}");
            Console.WriteLine();
            Console.WriteLine("Ingredients:");
            Console.WriteLine("| Ingredient      | Units | Unit Price | Total Price |");
            Console.WriteLine("|-----------------|-------|------------|------------|");
            Console.WriteLine($"| {recipe.RoastName,-15} | {roastUnitsFinal,5} | £{recipe.RoastPrice,8:0.00} | £{roastPrice,9:0.00} |");
            Console.WriteLine($"| {selectedMilk + " Milk",-15} | {milkUnitsFinal,5} | £{recipe.MilkPrice ?? 0,8:0.00} | £{milkPrice,9:0.00} |");
            foreach (var s in syrupBreakdown)
                Console.WriteLine($"| {s.name + " Syrup",-15} | {s.units,5} | £{s.unitPrice,8:0.00} | £{s.total,9:0.00} |");

            if (nonDairySurcharge || isDecaf)
            {
                Console.WriteLine("\nExtras:");
                if (nonDairySurcharge) Console.WriteLine("| Non-dairy milk      | +£0.40 |");
                if (isDecaf) Console.WriteLine("| Decaf               | –£0.40 |");
            }

            Console.WriteLine("\n-------------------------------");
            Console.WriteLine($"Total Price: £{totalPrice:0.00}");
        }

        static List<Recipe> LoadRecipes(string file)
        {
            if (!File.Exists(file)) return new List<Recipe>();
            var json = File.ReadAllText(file);
            return JsonSerializer.Deserialize<List<Recipe>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Recipe>();
        }

        static int PromptInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt + " ");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int value) && value >= min && value <= max)
                    return value;
                Console.WriteLine($"Please enter a number between {min} and {max}.");
            }
        }

        static double PromptDouble(string prompt, double min, double max, double def)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return def;
                if (double.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out double value) && value >= min && value <= max)
                    return value;
                Console.WriteLine($"Please enter a number between {min} and {max}, or press Enter for default ({def}).");
            }
        }

        static bool PromptYesNo()
        {
            while (true)
            {
                string? input = Console.ReadLine()?.Trim().ToUpperInvariant();
                if (input == "Y") return true;
                if (input == "N") return false;
                Console.Write("Please enter Y or N: ");
            }
        }
    }
}
