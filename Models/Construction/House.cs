using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildHouseApp.Models.Construction;

public class House
{
    public int FoundationProgress { get; private set; }
    public int WallsProgress { get; private set; }
    public int RoofProgress { get; private set; }
    public bool IsComplete => RoofProgress >= 100;
    
    public event Action<ConstructionEventType> ConstructionEvent;
    public event Action<string> ProgressChanged;

    private readonly Random _random = new();
    private bool _hasMaterials = true;

    public async Task BuildFoundationAsync()
    {
        while (FoundationProgress < 100)
        {
            if (!_hasMaterials)
            {
                ConstructionEvent?.Invoke(ConstructionEventType.MaterialsDepleted);
                await Task.Delay(1000);
                continue;
            }

            await Task.Delay(_random.Next(100, 500));
            FoundationProgress += _random.Next(1, 5);
            FoundationProgress = Math.Min(FoundationProgress, 100);
            ProgressChanged?.Invoke($"Foundation progress: {FoundationProgress}%");
            
            if (_random.NextDouble() < 0.01) // 1% chance to run out of materials
            {
                _hasMaterials = false;
            }
        }
    }

    public async Task BuildWallsAsync()
    {
        while (WallsProgress < 100)
        {
            if (!_hasMaterials)
            {
                ConstructionEvent?.Invoke(ConstructionEventType.MaterialsDepleted);
                await Task.Delay(1000);
                continue;
            }

            await Task.Delay(_random.Next(100, 500));
            WallsProgress += _random.Next(1, 5);
            WallsProgress = Math.Min(WallsProgress, 100);
            ProgressChanged?.Invoke($"Walls progress: {WallsProgress}%");
            
            if (_random.NextDouble() < 0.01) // 1% chance to run out of materials
            {
                _hasMaterials = false;
            }
        }
    }

    public async Task BuildRoofAsync()
    {
        ConstructionEvent?.Invoke(ConstructionEventType.RoofNeeded);
        
        while (RoofProgress < 100)
        {
            if (!_hasMaterials)
            {
                ConstructionEvent?.Invoke(ConstructionEventType.MaterialsDepleted);
                await Task.Delay(1000);
                continue;
            }

            await Task.Delay(_random.Next(100, 500));
            RoofProgress += _random.Next(1, 5);
            RoofProgress = Math.Min(RoofProgress, 100);
            ProgressChanged?.Invoke($"Roof progress: {RoofProgress}%");
            
            if (_random.NextDouble() < 0.01) // 1% chance to run out of materials
            {
                _hasMaterials = false;
            }
        }
        
        ConstructionEvent?.Invoke(ConstructionEventType.ConstructionComplete);
    }

    public void DeliverMaterials()
    {
        _hasMaterials = true;
        ProgressChanged?.Invoke("Materials delivered");
    }
}