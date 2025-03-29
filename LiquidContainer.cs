namespace ContainerManagement;

public class LiquidContainer : Container, IHazardNotifier
{
    public LiquidContainer(float height, float ownWeight, float depth, float maxCapacity, bool isHazardous)
        : base("L", height, ownWeight, depth, maxCapacity)
    {
        IsHazardous = isHazardous;
    }

    public bool IsHazardous { get; }

    public void NotifyHazard(string message)
    {
        Console.WriteLine($"!!! HAZARD NOTIFICATION !!! Container: {SerialNumber}, Message: {message}");
    }

    public override void LoadCargo(float weightToAdd)
    {
        var maxAllowedCapacityPercent = IsHazardous ? 0.5f : 0.9f;
        var maxAllowedTotalWeight = MaxCapacity * maxAllowedCapacityPercent;

        if (CargoWeight + weightToAdd > maxAllowedTotalWeight)
        {
            var availableCapacity = maxAllowedTotalWeight - CargoWeight;
            if (availableCapacity < 0) availableCapacity = 0;

            NotifyHazard(
                $"Attempt to add {weightToAdd}kg to container {SerialNumber} (Current: {CargoWeight}kg, Allowed Total: {maxAllowedTotalWeight}kg) would exceed the {maxAllowedCapacityPercent * 100}% capacity limit.");
            throw new OverfillException(
                $"Attempt to add {weightToAdd}kg to container {SerialNumber} failed. {(IsHazardous ? "Hazardous" : "Non-hazardous")} liquid containers can only hold a total of {maxAllowedTotalWeight}kg ({maxAllowedCapacityPercent * 100}% of capacity). Currently holds {CargoWeight}kg, only {availableCapacity:F2}kg more can be added.");
        }

        base.LoadCargo(weightToAdd);
    }

    public override string ToString()
    {
        return $"{base.ToString()}, Type: Liquid, Hazardous: {IsHazardous}";
    }
}