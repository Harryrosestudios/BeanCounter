# BeanCounter Project: Running Multiple Entry Points

This guide explains how to build and run the two main scripts in your BeanCounter project:  
- **AddRecipe.cs** (for adding recipes to a JSON file)
- **Program.cs** (the main blend pricing tool described in your brief)

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed
- Terminal/command prompt open in your project directory

## Running the AddRecipe Script

The `AddRecipe.cs` file allows you to interactively add recipes to a `recipes.json` file.

### Steps

1. **Build the Project with AddRecipe as Entry Point**

   ```bash
   dotnet build -p:StartupObject=BeanCounter.AddRecipe
   ```

   - This tells .NET to use the `Main` method in the `AddRecipe` class as the entry point.

2. **Run the Compiled Application**

   ```bash
   dotnet bin/Debug/net9.0/BeanCounter.dll
   ```

   - Adjust `net9.0` if your target framework is different.

3. **Follow the Prompts**

   - Enter blend name, coffee type, roast name and price, milk type, and syrups as prompted.
   - Confirm to save the recipe to `recipes.json`.

## Switching to the Main Blend Pricing Tool (`Program.cs`)

The `Program.cs` file implements the main functionality described in your brief:

- **Inputs:**
  - Blend name (string)
  - Batch size in kilograms (decimal)
  - Up to five bean components:
    - Origin code (e.g., “BRA-Santos”)
    - Weight in the blend (kg or g)

- **Outputs:**
  ```
  <Blend Name> – Batch: <X> kg
  Ingredient cost  : £<…>
  Packaging cost   : —
  Roasting loss    : —
  ----------------------------
  Cost / kg        : £<…>
  Total batch £    : £<…>
  ```

### Steps

1. **Build the Project with Program as Entry Point**

   ```bash
   dotnet build -p:StartupObject=BeanCounter.Program
   ```

   - This sets the entry point to the `Main` method in the `Program` class.

2. **Run the Compiled Application**

   ```bash
   dotnet bin/Debug/net9.0/BeanCounter.dll
   ```

   - Again, adjust `net9.0` if needed.

3. **Follow the Prompts**

   - Enter the blend name, batch size, and up to five bean components as requested.
   - The program will output the cost breakdown as specified.

## Switching Between Scripts

- **To run `AddRecipe.cs`:**  
  Build with `-p:StartupObject=BeanCounter.AddRecipe`  
- **To run `Program.cs`:**  
  Build with `-p:StartupObject=BeanCounter.Program`

You can switch between these tools at any time by rebuilding with the appropriate `StartupObject` parameter.

## Troubleshooting

- If you see an error about multiple entry points, ensure you are specifying the correct `StartupObject` when building.
- If you change the target framework, update the path in the run command accordingly.

## Example Commands

| Task                        | Build Command                                         | Run Command                                      |
|-----------------------------|------------------------------------------------------|--------------------------------------------------|
| Add Recipe Tool             | `dotnet build -p:StartupObject=BeanCounter.AddRecipe`| `dotnet bin/Debug/net9.0/BeanCounter.dll`        |
| Main Blend Pricing Tool     | `dotnet build -p:StartupObject=BeanCounter.Program`  | `dotnet bin/Debug/net9.0/BeanCounter.dll`        |

**Tip:**  
If you frequently switch between these tools, consider creating separate projects for each, or use scripts to automate the build/run process.
