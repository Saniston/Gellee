using Gellee.Models;
using Gellee.Services;
using Gellee.Services.Repositories;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Gellee.Pages.Recipes;

[QueryProperty(nameof(RecipeId), "recipeId")]
public partial class RecipeCalculatePage : ContentPage
{
    readonly RecipeService _recipeService;
    Recipe? _recipe;
    readonly ObservableCollection<RecipeIngredient> _ingredients = new();

    string _recipeId = string.Empty;
    public string RecipeId
    {
        get => _recipeId;
        set
        {
            _recipeId = value;
            _ = LoadRecipeAsync(value);
        }
    }

    public RecipeCalculatePage(RecipeService recipeService)
    {
        InitializeComponent();
        _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
        IngredientsCollection.ItemsSource = _ingredients;
    }

    async Task LoadRecipeAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (!Guid.TryParse(id, out var guid)) return;

        try
        {
            _recipe = _recipeService.GetById(guid);
            if (_recipe == null)
            {
                await DisplayAlertAsync("Erro", "Receita n�o encontrada.", "OK");
                return;
            }

            RecipeNameLabel.Text = _recipe.Name;

            _ingredients.Clear();
            foreach (var ri in _recipe.Ingredients)
            {
                // clonar itens para edi��o independente
                _ingredients.Add(new RecipeIngredient
                {
                    IngredientId = ri.IngredientId,
                    Ingredient = ri.Ingredient,
                    Quantity = ri.Quantity,
                    UnitOfMeasurementId = ri.UnitOfMeasurementId,
                    UnitOfMeasurement = ri.UnitOfMeasurement
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"N�o foi poss�vel carregar a receita.\n{ex.Message}", "OK");
        }
    }

    void OnQuantityUnfocused(object? sender, FocusEventArgs e)
    {
        try
        {
            if (!(sender is Entry entry)) return;
            if (!(entry.BindingContext is RecipeIngredient edited)) return;

            var text = entry.Text ?? "0";
            if (!decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out var newQty)
                && !decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out newQty))
            {
                return;
            }

            // recalcula usando o nome do ingrediente editado como base
            var recalculated = RecipeCalculator.Recalculate(_ingredients.Select(i => new RecipeIngredient
            {
                IngredientId = i.IngredientId,
                Ingredient = i.Ingredient,
                Quantity = i.Quantity,
                UnitOfMeasurementId = i.UnitOfMeasurementId,
                UnitOfMeasurement = i.UnitOfMeasurement
            }).ToList(), edited.Ingredient.Name, newQty);

            // atualiza a collection exibida
            _ingredients.Clear();
            foreach (var ri in recalculated)
                _ingredients.Add(ri);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    async void OnSaveAsCopyClicked(object? sender, EventArgs e)
    {
        try
        {
            if (_recipe == null) return;

            var copy = new Recipe
            {
                Name = _recipe.Name + " (c�pia)",
                Description = _recipe.Description,
                Ingredients = _ingredients.Select(i => new RecipeIngredient
                {
                    IngredientId = i.IngredientId,
                    Ingredient = i.Ingredient,
                    Quantity = i.Quantity,
                    UnitOfMeasurementId = i.UnitOfMeasurementId,
                    UnitOfMeasurement = i.UnitOfMeasurement
                }).ToList()
            };

            copy.GenerateId();
            _recipeService.Save(copy);

            await DisplayAlertAsync("Sucesso", "Receita salva como c�pia.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"N�o foi poss�vel salvar a c�pia.\n{ex.Message}", "OK");
        }
    }

    async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}