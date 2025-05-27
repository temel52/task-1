using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BuildHouseApp.Models.Construction;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BuildHouseApp.ViewModels;

public partial class ConstructionViewModel : ViewModelBase
{
    [ObservableProperty] 
    private string _statusMessage = "Construction not started";
    
    [ObservableProperty]
    private bool _isConstructionInProgress;
    
    [ObservableProperty]
    private bool _materialsNeeded;
    
    [ObservableProperty]
    private bool _roofNeeded;
    
    [ObservableProperty]
    private bool _isHouseComplete;
    
    public ObservableCollection<string> ActivityLog { get; } = new();
    public ObservableCollection<IConstructionEquipment> Equipment { get; } = new();
    
    private readonly House _house = new();
    private readonly Bricklayer _bricklayer;

    public ConstructionViewModel(string bricklayerName)
    {
        _bricklayer = new Bricklayer(bricklayerName);
        
        // Setup equipment
        Equipment.Add(new CementMixer());
        Equipment.Add(new Crane());
        
        // Subscribe to events
        _house.ConstructionEvent += OnConstructionEvent;
        _house.ProgressChanged += OnProgressChanged;
        _bricklayer.ActivityChanged += OnActivityChanged;
        
        foreach (var equipment in Equipment)
        {
            equipment.EquipmentStatusChanged += OnEquipmentStatusChanged;
        }
    }
public int HouseProgress => 
    (_house.FoundationProgress + _house.WallsProgress + _house.RoofProgress) / 3;
    private void OnEquipmentStatusChanged(string message)
    {
        AddToLog(message);
    }

    private void OnActivityChanged(string activity)
    {
        AddToLog(activity);
    }

    private void OnProgressChanged(string progress)
    {
        StatusMessage = progress;
        AddToLog(progress);
    }

    private void OnConstructionEvent(ConstructionEventType eventType)
    {
        switch (eventType)
        {
            case ConstructionEventType.MaterialsDepleted:
                MaterialsNeeded = true;
                AddToLog("Materials depleted! Need more materials.");
                break;
            case ConstructionEventType.RoofNeeded:
                RoofNeeded = true;
                AddToLog("Time to build the roof!");
                break;
            case ConstructionEventType.ConstructionComplete:
                IsHouseComplete = true;
                IsConstructionInProgress = false;
                AddToLog("House construction complete!");
                break;
            case ConstructionEventType.EquipmentFailure:
                AddToLog("Equipment failure occurred!");
                break;
        }
    }

    private void AddToLog(string message)
    {
        ActivityLog.Add($"{DateTime.Now:T}: {message}");
    }

    [RelayCommand]
    private async Task StartConstructionAsync()
    {
        if (IsConstructionInProgress) return;
        
        IsConstructionInProgress = true;
        IsHouseComplete = false;
        MaterialsNeeded = false;
        RoofNeeded = false;
        
        AddToLog("Starting construction...");
        
        // Start all equipment
        foreach (var equipment in Equipment)
        {
            equipment.Operate();
        }
        
        // Start construction in background
        await Task.Run(async () =>
        {
            await _bricklayer.WorkOnHouseAsync(_house);
            
            // Stop all equipment when done
            foreach (var equipment in Equipment)
            {
                equipment.Stop();
            }
        });
    }

    [RelayCommand]
    private void DeliverMaterials()
    {
        _house.DeliverMaterials();
        MaterialsNeeded = false;
        AddToLog("Materials delivered to site");
    }

    [RelayCommand]
    private void RepairEquipment()
    {
        foreach (var equipment in Equipment)
        {
            if (equipment is ObservableObject observableEquipment)
            {
                var type = equipment.GetType();
                var isOperationalProperty = type.GetProperty(nameof(IConstructionEquipment.IsOperational));
                
                if (isOperationalProperty != null && !(bool)isOperationalProperty.GetValue(equipment)!)
                {
                    isOperationalProperty.SetValue(equipment, true);
                    AddToLog($"{equipment.Name} has been repaired");
                }
            }
        }
    }
}