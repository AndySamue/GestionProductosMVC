using GestionProductosMVC.Models;

namespace GestionProductosMVC.Services;

public interface IPokemonService
{
    Task<IReadOnlyList<PokemonSummary>> GetPokemonListAsync(CancellationToken cancellationToken = default);

    Task<PokemonDetail?> GetPokemonDetailAsync(string idOrName, CancellationToken cancellationToken = default);
}
