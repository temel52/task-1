using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BuildHouseApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _bricklayerName = "John the Builder";
    
    public ObservableCollection<ConstructionViewModel> ConstructionSites { get; } = new();

    [RelayCommand]
    private void AddConstructionSite()
    {
        ConstructionSites.Add(new ConstructionViewModel(BricklayerName));
    }

    [RelayCommand]
    private void RemoveConstructionSite(ConstructionViewModel site)
    {
        ConstructionSites.Remove(site);
    }
}