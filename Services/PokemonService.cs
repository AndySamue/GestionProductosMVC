using System.Net.Http.Json;
using System.Text.Json;
using GestionProductosMVC.Models;
using Microsoft.Extensions.Options;

namespace GestionProductosMVC.Services;

public class PokemonService : IPokemonService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly IOptions<PokeApiSettings> _options;
    private readonly ILogger<PokemonService> _logger;

    public PokemonService(HttpClient httpClient, IOptions<PokeApiSettings> options, ILogger<PokemonService> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PokemonSummary>> GetPokemonListAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var limit = _options.Value.ListLimit;
            var listResponse = await _httpClient.GetFromJsonAsync<PokemonListResponseDto>(
                $"pokemon?limit={limit}&offset=0", JsonOptions, cancellationToken);

            if (listResponse?.Results is null || listResponse.Results.Count == 0)
            {
                return Array.Empty<PokemonSummary>();
            }

            var detailTasks = listResponse.Results.Select(item => GetDetailDtoAsync(item.Name, cancellationToken));
            var details = await Task.WhenAll(detailTasks);

            return details
                .Where(dto => dto is not null)
                .Select(dto => MapToSummary(dto!))
                .OrderBy(p => p.Id)
                .ToList();
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException)
        {
            _logger.LogError(ex, "No se pudo obtener el listado de Pokemon desde PokeAPI");
            throw new PokeApiException("No se pudo cargar el catálogo de Pokémon en este momento. Intenta más tarde.", ex);
        }
    }

    public async Task<PokemonDetail?> GetPokemonDetailAsync(string idOrName, CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = await GetDetailDtoAsync(idOrName, cancellationToken);
            return dto is null ? null : MapToDetail(dto);
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException)
        {
            _logger.LogError(ex, "No se pudo obtener el detalle del Pokemon {IdOrName}", idOrName);
            throw new PokeApiException("No se pudo cargar el detalle del Pokémon solicitado. Intenta más tarde.", ex);
        }
    }

    private async Task<PokemonDetailDto?> GetDetailDtoAsync(string idOrName, CancellationToken cancellationToken)
    {
        var normalized = idOrName.Trim().ToLowerInvariant();
        var response = await _httpClient.GetAsync($"pokemon/{Uri.EscapeDataString(normalized)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<PokemonDetailDto>(JsonOptions, cancellationToken);
    }

    private static PokemonSummary MapToSummary(PokemonDetailDto dto) => new()
    {
        Id = dto.Id,
        Name = Capitalize(dto.Name),
        ImageUrl = dto.Sprites?.Other?.OfficialArtwork?.FrontDefault ?? dto.Sprites?.FrontDefault,
        Types = dto.Types.OrderBy(t => t.Slot).Select(t => Capitalize(t.Type.Name)).ToList()
    };

    private static PokemonDetail MapToDetail(PokemonDetailDto dto) => new()
    {
        Id = dto.Id,
        Name = Capitalize(dto.Name),
        ImageUrl = dto.Sprites?.Other?.OfficialArtwork?.FrontDefault ?? dto.Sprites?.FrontDefault,
        HeightMeters = dto.Height / 10.0,
        WeightKilograms = dto.Weight / 10.0,
        Types = dto.Types.OrderBy(t => t.Slot).Select(t => Capitalize(t.Type.Name)).ToList(),
        Abilities = dto.Abilities.Select(a => Capitalize(a.Ability.Name)).ToList(),
        Stats = dto.Stats.Select(s => new PokemonStat { Name = Capitalize(s.Stat.Name), BaseValue = s.BaseStat }).ToList()
    };

    private static string Capitalize(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var withSpaces = value.Replace('-', ' ');
        return char.ToUpperInvariant(withSpaces[0]) + withSpaces[1..];
    }
}
