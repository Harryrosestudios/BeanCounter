namespace BeanCounter
{
    public class Recipe
    {
        public string? BlendName { get; set; }
        public string? CoffeeType { get; set; }
        public string? RoastName { get; set; }
        public decimal RoastPrice { get; set; }
        public double? RoastUnitWeight { get; set; }
        public string? MilkType { get; set; }
        public decimal? MilkPrice { get; set; }
        public double? MilkUnitWeight { get; set; }
        public List<Syrup>? Syrups { get; set; }
    }

    public class Syrup
    {
        public string? Name { get; set; }
        public int? Pumps { get; set; }
        public decimal? Price { get; set; }
        public int? UnitWeight { get; set; }
    }
}

