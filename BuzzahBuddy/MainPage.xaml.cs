using BuzzahBuddy.ViewModels;

namespace BuzzahBuddy;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
