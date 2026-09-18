namespace GestionProductosMVC.Models;

public class PokemonSummary
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public IReadOnlyList<string> Types { get; set; } = new List<string>();
}

public class PokemonDetail
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public double HeightMeters { get; set; }

    public double WeightKilograms { get; set; }

    public IReadOnlyList<string> Types { get; set; } = new List<string>();

    public IReadOnlyList<string> Abilities { get; set; } = new List<string>();

    public IReadOnlyList<PokemonStat> Stats { get; set; } = new List<PokemonStat>();
}

public class PokemonStat
{
    public string Name { get; set; } = string.Empty;

    public int BaseValue { get; set; }
}
