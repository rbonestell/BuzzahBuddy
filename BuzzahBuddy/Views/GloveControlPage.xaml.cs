using BuzzahBuddy.ViewModels;

namespace BuzzahBuddy.Views;

public partial class GloveControlPage : ContentPage
{
    public GloveControlPage(GloveControlViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
