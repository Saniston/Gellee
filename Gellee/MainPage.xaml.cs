namespace Gellee
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnUnitsClicked(object? sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("///UnitsPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                await DisplayAlertAsync("Erro", $"Não foi possível abrir 'Unidades de Medida'.\n{ex.Message}", "OK");
            }
        }

        private async void OnIngredientsClicked(object? sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("///IngredientsPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                await DisplayAlertAsync("Erro", $"Não foi possível abrir 'Ingredientes'.\n{ex.Message}", "OK");
            }
        }

        private async void OnRecipesClicked(object? sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("///RecipesPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                await DisplayAlertAsync("Erro", $"Não foi possível abrir 'Receitas'.\n{ex.Message}", "OK");
            }
        }
    }
}