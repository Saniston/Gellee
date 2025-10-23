using Gellee.Models;
using Gellee.Services.Repositories;
using System.Collections.ObjectModel;

namespace Gellee.Pages.Recipes;

[QueryProperty(nameof(RecipeIdRaw), "recipeId")]
public partial class RecipeEditPage : ContentPage
{
    readonly RecipeService _recipeService;
    readonly IngredientService _ingredientService;
    readonly UnitOfMeasurementService _unitService;

    Recipe _current = new();
    readonly ObservableCollection<RecipeIngredient> _ingredients = new();

    public string? RecipeIdRaw
    {
        set
        {
            if (Guid.TryParse(value, out var g))
            {
                _ = LoadRecipeAsync(g);
            }
        }
    }

    public RecipeEditPage(RecipeService recipeService, IngredientService ingredientService, UnitOfMeasurementService unitService)
    {
        InitializeComponent();
        _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
        _ingredientService = ingredientService ?? throw new ArgumentNullException(nameof(ingredientService));
        _unitService = unitService ?? throw new ArgumentNullException(nameof(unitService));

        RecipeIngredientsCollection.ItemsSource = _ingredients;
    }

    async Task LoadRecipeAsync(Guid id)
    {
        try
        {
            var r = _recipeService.GetById(id);
            if (r != null)
            {
                _current = r;
                NameEntry.Text = r.Name;
                DescriptionEditor.Text = r.Description;

                _ingredients.Clear();
                if (r.Ingredients is not null)
                {
                    foreach (var ri in r.Ingredients)
                        _ingredients.Add(ri);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível carregar a receita.\n{ex.Message}", "OK");
        }
    }

    async void OnAddIngredientClicked(object? sender, EventArgs e)
    {
        try
        {
            var ingredients = _ingredientService.GetPaginated(null).ToList();
            if (!ingredients.Any())
            {
                await DisplayAlertAsync("Aviso", "Nenhum ingrediente cadastrado. Cadastre ingredientes antes.", "OK");
                return;
            }

            var ingredientNames = ingredients.Select(i => i.Name).ToArray();
            string action = await DisplayActionSheetAsync("Selecione ingrediente", "Cancelar", null, ingredientNames);
            if (string.IsNullOrWhiteSpace(action) || action == "Cancelar") return;

            var selectedIngredient = ingredients.FirstOrDefault(i => i.Name == action);
            if (selectedIngredient == null) return;

            var units = _unitService.GetPaginated(null).ToList();
            string unitChoice = "Sem unidade";
            if (units.Any())
            {
                var unitNames = units.Select(u => u.Name).ToArray();
                unitChoice = await DisplayActionSheetAsync("Selecione unidade", "Cancelar", null, unitNames);
                if (string.IsNullOrWhiteSpace(unitChoice) || unitChoice == "Cancelar") return;
            }

            string qtyRaw = await DisplayPromptAsync("Quantidade", "Informe a quantidade (ex: 1.5):", "OK", "Cancelar", placeholder: "0.0", maxLength: 20, keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(qtyRaw)) return;
            if (!decimal.TryParse(qtyRaw.Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var qty))
            {
                if (!decimal.TryParse(qtyRaw, out qty))
                {
                    await DisplayAlertAsync("Erro", "Quantidade inválida.", "OK");
                    return;
                }
            }

            var recipeIngredient = new RecipeIngredient
            {
                IngredientId = selectedIngredient.Id,
                Ingredient = selectedIngredient,
                Quantity = qty
            };

            if (units.Any())
            {
                var selectedUnit = units.FirstOrDefault(u => u.Name == unitChoice);
                if (selectedUnit != null)
                {
                    recipeIngredient.UnitOfMeasurementId = selectedUnit.Id;
                    recipeIngredient.UnitOfMeasurement = selectedUnit;
                }
            }

            _ingredients.Add(recipeIngredient);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível adicionar ingrediente.\n{ex.Message}", "OK");
        }
    }

    async void OnEditIngredientInvoked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is SwipeItem si && si.CommandParameter is Guid ingId)
            {
                var item = _ingredients.FirstOrDefault(x => x.IngredientId == ingId);
                if (item == null)
                {
                    await DisplayAlertAsync("Aviso", "Ingrediente não encontrado.", "OK");
                    return;
                }

                string qtyRaw = await DisplayPromptAsync("Quantidade", "Informe a quantidade:", "OK", "Cancelar", initialValue: item.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture), keyboard: Keyboard.Numeric);
                if (string.IsNullOrWhiteSpace(qtyRaw)) return;
                if (!decimal.TryParse(qtyRaw.Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var qty))
                {
                    if (!decimal.TryParse(qtyRaw, out qty))
                    {
                        await DisplayAlertAsync("Erro", "Quantidade inválida.", "OK");
                        return;
                    }
                }
                item.Quantity = qty;

                var units = _unitService.GetPaginated(null).ToList();
                if (units.Any())
                {
                    var unitNames = units.Select(u => u.Name).ToArray();
                    string unitChoice = await DisplayActionSheetAsync("Selecione unidade", "Cancelar", null, unitNames);
                    if (!string.IsNullOrWhiteSpace(unitChoice) && unitChoice != "Cancelar")
                    {
                        var selectedUnit = units.FirstOrDefault(u => u.Name == unitChoice);
                        if (selectedUnit != null)
                        {
                            item.UnitOfMeasurementId = selectedUnit.Id;
                            item.UnitOfMeasurement = selectedUnit;
                        }
                    }
                }

                var idx = _ingredients.IndexOf(item);
                if (idx >= 0)
                {
                    _ingredients[idx] = item;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível editar ingrediente.\n{ex.Message}", "OK");
        }
    }

    async void OnDeleteIngredientInvoked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is SwipeItem si && si.CommandParameter is Guid ingId)
            {
                var item = _ingredients.FirstOrDefault(x => x.IngredientId == ingId);
                if (item == null)
                {
                    await DisplayAlertAsync("Aviso", "Ingrediente não encontrado.", "OK");
                    return;
                }

                bool ok = await DisplayAlertAsync("Confirmar", $"Remover '{item.Name}'?", "Remover", "Cancelar");
                if (!ok) return;

                _ingredients.Remove(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível remover ingrediente.\n{ex.Message}", "OK");
        }
    }

    async void OnSaveClicked(object? sender, EventArgs e)
    {
        try
        {
            _current.Name = NameEntry.Text?.Trim() ?? string.Empty;
            _current.Description = DescriptionEditor.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(_current.Name))
            {
                await DisplayAlertAsync("Validação", "Nome é obrigatório.", "OK");
                return;
            }

            _current.Ingredients = _ingredients.ToList();
            _recipeService.Save(_current);
            await DisplayAlertAsync("Sucesso", "Receita salva.", "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível salvar a receita.\n{ex.Message}", "OK");
        }
    }

    async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}