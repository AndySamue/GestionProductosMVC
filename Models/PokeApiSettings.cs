namespace GestionProductosMVC.Models;

public class PokeApiSettings
{
    public const string SectionName = "PokeApi";

    public string BaseUrl { get; set; } = "https://pokeapi.co/api/v2/";

    public int ListLimit { get; set; } = 20;
}
