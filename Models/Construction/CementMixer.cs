using System;
using System.Threading.Tasks;

namespace BuildHouseApp.Models.Construction;

public class CementMixer : IConstructionEquipment
{
    public string Name { get; } = "Cement Mixer";
    public bool IsOperational { get; private set; } = true;
    
    public event Action<string> EquipmentStatusChanged;

    private readonly Random _random = new();
    private bool _isRunning;

    public void Operate()
    {
        if (!IsOperational)
        {
            EquipmentStatusChanged?.Invoke($"{Name} is broken and cannot operate");
            return;
        }

        _isRunning = true;
        Task.Run(async () =>
        {
            while (_isRunning)
            {
                EquipmentStatusChanged?.Invoke($"{Name} is mixing cement");
                await Task.Delay(2000);
                
                // Random chance of failure
                if (_random.NextDouble() < 0.05) // 5% chance of failure
                {
                    IsOperational = false;
                    EquipmentStatusChanged?.Invoke($"{Name} has broken down!");
                    _isRunning = false;
                    return;
                }
            }
        });
    }

    public void Stop()
    {
        _isRunning = false;
        EquipmentStatusChanged?.Invoke($"{Name} has stopped");
    }
}