using GestionProductosMVC.Models;
using GestionProductosMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionProductosMVC.Controllers;

public class PokemonController : Controller
{
    private readonly IPokemonService _pokemonService;
    private readonly ILogger<PokemonController> _logger;

    public PokemonController(IPokemonService pokemonService, ILogger<PokemonController> logger)
    {
        _pokemonService = pokemonService;
        _logger = logger;
    }

    // GET: Pokemon
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var pokemons = await _pokemonService.GetPokemonListAsync(cancellationToken);
            return View(pokemons);
        }
        catch (PokeApiException ex)
        {
            _logger.LogError(ex, "Fallo al cargar el catálogo de Pokémon");
            ViewBag.ErrorMessage = ex.Message;
            return View(Array.Empty<PokemonSummary>());
        }
    }

    // GET: Pokemon/Details/pikachu
    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var pokemon = await _pokemonService.GetPokemonDetailAsync(id, cancellationToken);
            if (pokemon is null)
            {
                return NotFound();
            }

            return View(pokemon);
        }
        catch (PokeApiException ex)
        {
            _logger.LogError(ex, "Fallo al cargar el detalle del Pokémon {Id}", id);
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
