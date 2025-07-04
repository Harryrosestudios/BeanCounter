```markdown
# BeanCounter

A command-line toolkit for managing and pricing custom coffee blends for RoastRite Ltd.

---

## Project Overview

BeanCounter consists of two main scripts:

- **AddRecipe.cs**: Interactive tool for adding new drink recipes to `recipes.json`.
- **Program.cs**: Main pricing tool that allows you to select, customise, and price drinks based on saved recipes.

Both scripts share their data models in `Models.cs`.

---

## Prerequisites

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
- A terminal or command prompt
- All scripts (`AddRecipe.cs`, `Program.cs`, `Models.cs`) in the same project directory

---

## How to Use

### 1. Adding a New Recipe (`AddRecipe.cs`)

This script lets you create new drink recipes with all ingredient details, including per-unit weights and prices.

**To run:**

```
dotnet clean
dotnet build -p:StartupObject=BeanCounter.AddRecipe
dotnet bin/Debug/net9.0/BeanCounter.dll
```

- Follow the prompts to enter the blend name, coffee type, roast details, milk details, and any number of syrups.
- Syrup input format: `SyrupName NumberOfPumps Price UnitWeight` (e.g., `Vanilla 3 0.50 3`)
- Confirm to save. The recipe is appended to `recipes.json`.

---

### 2. Pricing and Customising Drinks (`Program.cs`)

This script lets you select a saved recipe, choose size and options, and see a full price breakdown.

**To run:**

```
dotnet clean
dotnet build -p:StartupObject=BeanCounter.Program
dotnet bin/Debug/net9.0/BeanCounter.dll
```

- Select a blend from the list.
- Choose a size: Small, Medium (×1.5), Large (×2), or Custom (enter units for each ingredient).
- Optionally change the milk (non-dairy adds £0.40).
- Optionally make it decaf (subtracts £0.40).
- The script displays a detailed ingredient and price breakdown.

---

## Switching Between Scripts

**Important:**  
If you want to switch between `AddRecipe.cs` and `Program.cs`, always run:

```
dotnet clean
dotnet build -p:StartupObject=BeanCounter.[AddRecipe|Program]
dotnet bin/Debug/net9.0/BeanCounter.dll
```

- Replace `[AddRecipe|Program]` with the entry point you want.
- `dotnet clean` ensures the correct entry point is used (otherwise, .NET may run the previous script).

---

## Shared Models

Both scripts use `Models.cs` for the `Recipe` and `Syrup` classes.  
**Do not duplicate these classes in your script files.**

---

## Troubleshooting

- If you see prompts from the wrong script, you likely need to `dotnet clean` and rebuild with the correct `StartupObject`.
- Always use the correct case for class names: `BeanCounter.Program` and `BeanCounter.AddRecipe`.
- If you change your target framework, update the DLL path accordingly.

---

## Example Workflow

**Add a recipe:**
```
dotnet clean
dotnet build -p:StartupObject=BeanCounter.AddRecipe
dotnet bin/Debug/net9.0/BeanCounter.dll
```

**Price a drink:**
```
dotnet clean
dotnet build -p:StartupObject=BeanCounter.Program
dotnet bin/Debug/net9.0/BeanCounter.dll
```

---

## File Structure

```
BeanCounter/
├── AddRecipe.cs
├── Program.cs
├── Models.cs
├── recipes.json
└── README.md
```

---

## Notes

- All monetary values are in GBP and shown to two decimal places.
- All input is validated at the console.
- Recipes are stored in `recipes.json` in the project directory.
```

