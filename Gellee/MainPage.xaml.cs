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
            await Shell.Current.GoToAsync("UnitsPage");
        }

        private async void OnIngredientsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("IngredientsPage");
        }

        private async void OnRecipesClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("RecipesPage");
        }
    }
}