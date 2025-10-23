using Gellee.Models;
using Gellee.Services.Repositories;
using LiteDB;
using System.Collections.ObjectModel;

namespace Gellee.Pages.Ingredients;

public partial class IngredientsPage : ContentPage
{
    readonly IngredientService _ingredientService;
    readonly ObservableCollection<Ingredient> _items = [];
    readonly PageFilter _filter = new() { Take = 10, Page = 1 };
    bool _isLoading;
    bool _hasMore = true;

    public IngredientsPage(IngredientService ingredientService)
    {
        InitializeComponent();

        _ingredientService = ingredientService ?? throw new ArgumentNullException(nameof(ingredientService));

        IngredientsCollection.ItemsSource = _items;
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

            var results = _ingredientService.GetPaginated(_filter)?.ToList() ?? [];

            foreach (var r in results)
                _items.Add(r);

            _hasMore = results.Count >= _filter.Take;
            if (_hasMore) _filter.Page++;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível carregar ingredientes.\n{ex.Message}", "OK");
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

    async void OnAddIngredientClicked(object? sender, EventArgs e)
    {
        try
        {
            string? name = await DisplayPromptAsync("Novo ingrediente", "Informe o nome do ingrediente:", "Adicionar", "Cancelar", placeholder: "Ex: Açúcar", maxLength: 100, keyboard: Keyboard.Text);
            if (string.IsNullOrWhiteSpace(name)) return;

            var ingredient = new Ingredient
            {
                Name = name.Trim()
            };

            _ingredientService.Save(ingredient);

            _items.Insert(0, ingredient);
            await DisplayAlertAsync("Sucesso", "Ingrediente adicionado.", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível adicionar o ingrediente.\n{ex.Message}", "OK");
        }
    }

    async void OnEditSwipeInvoked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is SwipeItem si && si.CommandParameter is Guid id)
            {
                var ingredient = _items.FirstOrDefault(x => x.Id == id);

                ingredient ??= _ingredientService.GetById(id);

                if (ingredient == null)
                {
                    await DisplayAlertAsync("Aviso", "Ingrediente não encontrado.", "OK");
                    return;
                }

                string? newName = await DisplayPromptAsync("Editar ingrediente", "Nome do ingrediente:", "Salvar", "Cancelar", initialValue: ingredient.Name, maxLength: 100, keyboard: Keyboard.Text);
                if (string.IsNullOrWhiteSpace(newName)) return;

                ingredient.Name = newName.Trim();
                _ingredientService.Save(ingredient);

                var idx = _items.IndexOf(ingredient);
                if (idx >= 0) 
                    _items[idx] = ingredient;

                await DisplayAlertAsync("Sucesso", "Ingrediente atualizado.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível editar o ingrediente.\n{ex.Message}", "OK");
        }
    }

    async void OnDeleteSwipeInvoked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is SwipeItem si && si.CommandParameter is Guid id)
            {
                var ingredient = _items.FirstOrDefault(x => x.Id == id);
                if (ingredient == null)
                {
                    await DisplayAlertAsync("Aviso", "Ingrediente não encontrado.", "OK");
                    return;
                }

                bool ok = await DisplayAlertAsync("Confirmar", $"Remover '{ingredient.Name}'?", "Remover", "Cancelar");
                if (!ok) return;

                _ingredientService.Delete(ingredient.Id);
                _items.Remove(ingredient);

                await DisplayAlertAsync("Sucesso", "Ingrediente removido.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível remover o ingrediente.\n{ex.Message}", "OK");
        }
    }
}