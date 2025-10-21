namespace Gellee
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Pages.Recipes.RecipeEditPage), typeof(Pages.Recipes.RecipeEditPage));
        }
    }
}
