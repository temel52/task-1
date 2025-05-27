using System;
using System.Threading.Tasks;

namespace BuildHouseApp.Models.Construction;

public class Bricklayer
{
    public string Name { get; }
    public bool IsWorking { get; private set; }
    
    public event Action<string> ActivityChanged;

    public Bricklayer(string name)
    {
        Name = name;
    }

    public async Task WorkOnHouseAsync(House house)
    {
        IsWorking = true;
        ActivityChanged?.Invoke($"{Name} started working on foundation");
        await house.BuildFoundationAsync();
        
        ActivityChanged?.Invoke($"{Name} started working on walls");
        await house.BuildWallsAsync();
        
        ActivityChanged?.Invoke($"{Name} started working on roof");
        await house.BuildRoofAsync();
        
        IsWorking = false;
        ActivityChanged?.Invoke($"{Name} finished work");
    }
}