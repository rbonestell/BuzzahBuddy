using BuzzahBuddy.ViewModels;

namespace BuzzahBuddy.Views;

public partial class DeviceListPage : ContentPage
{
    public DeviceListPage(DeviceListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
