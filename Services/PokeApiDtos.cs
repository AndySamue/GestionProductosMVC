using System.Text.Json.Serialization;

namespace GestionProductosMVC.Services;

internal sealed class PokemonListResponseDto
{
    [JsonPropertyName("results")]
    public List<PokemonListItemDto> Results { get; set; } = new();
}

internal sealed class PokemonListItemDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

internal sealed class PokemonDetailDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [JsonPropertyName("sprites")]
    public SpritesDto? Sprites { get; set; }

    [JsonPropertyName("types")]
    public List<PokemonTypeSlotDto> Types { get; set; } = new();

    [JsonPropertyName("abilities")]
    public List<PokemonAbilitySlotDto> Abilities { get; set; } = new();

    [JsonPropertyName("stats")]
    public List<PokemonStatSlotDto> Stats { get; set; } = new();
}

internal sealed class SpritesDto
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; set; }

    [JsonPropertyName("other")]
    public OtherSpritesDto? Other { get; set; }
}

internal sealed class OtherSpritesDto
{
    [JsonPropertyName("official-artwork")]
    public OfficialArtworkDto? OfficialArtwork { get; set; }
}

internal sealed class OfficialArtworkDto
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; set; }
}

internal sealed class PokemonTypeSlotDto
{
    [JsonPropertyName("slot")]
    public int Slot { get; set; }

    [JsonPropertyName("type")]
    public NamedApiResourceDto Type { get; set; } = new();
}

internal sealed class PokemonAbilitySlotDto
{
    [JsonPropertyName("ability")]
    public NamedApiResourceDto Ability { get; set; } = new();
}

internal sealed class PokemonStatSlotDto
{
    [JsonPropertyName("base_stat")]
    public int BaseStat { get; set; }

    [JsonPropertyName("stat")]
    public NamedApiResourceDto Stat { get; set; } = new();
}

internal sealed class NamedApiResourceDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
