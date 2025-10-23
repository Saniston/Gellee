using Gellee.Models;
using Gellee.Services.Repositories;
using System.Collections.ObjectModel;

namespace Gellee.Pages.Recipes;

public partial class RecipesPage : ContentPage
{
    readonly RecipeService _recipeService;
    readonly ObservableCollection<Recipe> _items = [];
    readonly PageFilter _filter = new() { Take = 10, Page = 1 };
    bool _isLoading;
    bool _hasMore = true;

    public RecipesPage(RecipeService recipeService)
    {
        InitializeComponent();

        _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));

        RecipesCollection.ItemsSource = _items;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_items.Count == 0)
            await LoadItemsAsync(reset: true);
    }

    async Task LoadItemsAsync(bool reset = false)
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            if (reset)
            {
                _filter.Page = 1;
                _items.Clear();
                _hasMore = true;
            }

            if (!_hasMore) return;

            var results = _recipeService.GetPaginated(_filter)?.ToList() ?? [];

            foreach (var r in results)
                _items.Add(r);

            _hasMore = results.Count >= _filter.Take;
            if (_hasMore) _filter.Page++;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível carregar receitas.\n{ex.Message}", "OK");
        }
        finally
        {
            _isLoading = false;
        }
    }

    async void OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        if (_hasMore) await LoadItemsAsync(reset: false);
    }

    async void OnSearchBarTextChanged(object? sender, TextChangedEventArgs e)
    {
        _filter.SearchTerm = e.NewTextValue ?? string.Empty;
        await LoadItemsAsync(reset: true);
    }

    async void OnAddRecipeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("RecipeEditPage");
    }

    async void OnEditSwipeInvoked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is SwipeItem si && si.CommandParameter is Guid id)
            {
                await Shell.Current.GoToAsync($"RecipeEditPage?recipeId={id}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível iniciar edição.\n{ex.Message}", "OK");
        }
    }

    async void OnDeleteSwipeInvoked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is SwipeItem si && si.CommandParameter is Guid id)
            {
                var recipe = _items.FirstOrDefault(x => x.Id == id) ?? _recipeService.GetById(id);
                if (recipe == null)
                {
                    await DisplayAlertAsync("Aviso", "Receita não encontrada.", "OK");
                    return;
                }

                bool ok = await DisplayAlertAsync("Confirmar", $"Remover '{recipe.Name}'?", "Remover", "Cancelar");
                if (!ok) return;

                _recipeService.Delete(recipe.Id);
                _items.Remove(recipe);

                await DisplayAlertAsync("Sucesso", "Receita removida.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível remover a receita.\n{ex.Message}", "OK");
        }
    }

    async void OnRecipeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection?.FirstOrDefault() is not Recipe selected) return;

            await Shell.Current.GoToAsync($"RecipeCalculatePage?recipeId={selected.Id}");

            RecipesCollection.SelectedItem = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível abrir a receita.\n{ex.Message}", "OK");
        }
    }
}