using System;

namespace BuildHouseApp.Models.Construction;

public interface IConstructionEquipment
{
    string Name { get; }
    bool IsOperational { get; }
    void Operate();
    void Stop();
    event Action<string> EquipmentStatusChanged;
}