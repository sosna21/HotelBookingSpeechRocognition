using System.Windows;
using System.Windows.Controls;
using SWPSD_PROJEKT.UI.ViewModels;

namespace SWPSD_PROJEKT.UI.Views;

public partial class FacilitiesSelection : UserControl
{
    public FacilitiesSelection()
    {
        InitializeComponent();
        Loaded += (_, _) => RadioBtn.IsChecked = true;
    }

    private void ContinueBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var viewModel = (FacilitiesSelectionViewModel) DataContext;
        if (viewModel.NavigateSummaryOrderCommand.CanExecute(null) &&
            viewModel.SaveFacilities.CanExecute(null))
        {
            viewModel.SaveFacilities.Execute(null);
            viewModel.NavigateSummaryOrderCommand.Execute(null);
        }
        
        //TODO save to database
    }
}