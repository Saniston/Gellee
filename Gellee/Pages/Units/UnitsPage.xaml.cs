using Gellee.Models;
using Gellee.Services.Repositories;
using LiteDB;
using System.Collections.ObjectModel;

namespace Gellee.Pages.Units;

public partial class UnitsPage : ContentPage
{
    readonly UnitOfMeasurementService _unitService;
    readonly ObservableCollection<UnitOfMeasurement> _items = [];
    readonly PageFilter _filter = new() { Take = 10, Page = 1 };
    bool _isLoading;
    bool _hasMore = true;

    public UnitsPage(UnitOfMeasurementService unitService)
    {
        InitializeComponent();

        _unitService = unitService ?? throw new ArgumentNullException(nameof(unitService));

        UnitsCollection.ItemsSource = _items;
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

            if (!_hasMore)
                return;

            var results = _unitService.GetPaginated(_filter)?.ToList() ?? [];

            foreach (var r in results)
                _items.Add(r);

            _hasMore = results.Count >= _filter.Take;
            if (_hasMore)
                _filter.Page++;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível carregar unidades.\n{ex.Message}", "OK");
        }
        finally
        {
            _isLoading = false;
        }
    }

    async void OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        if (_hasMore)
            await LoadItemsAsync(reset: false);
    }

    async void OnSearchBarTextChanged(object? sender, TextChangedEventArgs e)
    {
        _filter.SearchTerm = e.NewTextValue ?? string.Empty;
        await LoadItemsAsync(reset: true);
    }

    async void OnAddUnitClicked(object? sender, EventArgs e)
    {
        try
        {
            string? name = await DisplayPromptAsync("Nova unidade", "Informe o nome da unidade:", "Adicionar", "Cancelar", placeholder: "Ex: Copo", maxLength: 100, keyboard: Keyboard.Text);
            if (string.IsNullOrWhiteSpace(name))
                return;

            var unit = new UnitOfMeasurement
            {
                Name = name.Trim(),
                BaseValue = 1m
            };

            _unitService.Save(unit);

            _items.Insert(0, unit);
            await DisplayAlertAsync("Sucesso", "Unidade adicionada.", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível adicionar a unidade.\n{ex.Message}", "OK");
        }
    }

    async void OnEditSwipeInvoked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is SwipeItem si && si.CommandParameter is Guid id)
            {
                var unit = _items.FirstOrDefault(x => x.Id == id) ?? _unitService.GetById(default);
                if (unit == null)
                {
                    await DisplayAlertAsync("Aviso", "Unidade não encontrada.", "OK");
                    return;
                }

                string? newName = await DisplayPromptAsync("Editar unidade", "Nome da unidade:", "Salvar", "Cancelar", initialValue: unit.Name, maxLength: 100, keyboard: Keyboard.Text);
                if (string.IsNullOrWhiteSpace(newName))
                    return;

                unit.Name = newName.Trim();
                _unitService.Save(unit);

                var idx = _items.IndexOf(unit);
                if (idx >= 0)
                    _items[idx] = unit;

                await DisplayAlertAsync("Sucesso", "Unidade atualizada.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível editar a unidade.\n{ex.Message}", "OK");
        }
    }

    async void OnDeleteSwipeInvoked(object? sender, EventArgs e)
    {
        try
        {
            if (sender is SwipeItem si && si.CommandParameter is Guid id)
            {
                var unit = _items.FirstOrDefault(x => x.Id == id);
                if (unit == null)
                {
                    await DisplayAlertAsync("Aviso", "Unidade não encontrada.", "OK");
                    return;
                }

                bool ok = await DisplayAlertAsync("Confirmar", $"Remover '{unit.Name}'?", "Remover", "Cancelar");
                if (!ok) return;

                _unitService.Delete(unit.Id);
                _items.Remove(unit);

                await DisplayAlertAsync("Sucesso", "Unidade removida.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Erro", $"Não foi possível remover a unidade.\n{ex.Message}", "OK");
        }
    }
}