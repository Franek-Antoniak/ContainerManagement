namespace ContainerManagement;

public class OverfillException(string message) : Exception(message);

public interface IHazardNotifier
{
    void NotifyHazard(string message);
}

public abstract class Container
{
    private static int _nextSerialNumber = 1;
    private readonly string _containerTypeChar;

    protected Container(string containerTypeChar, float height, float ownWeight, float depth, float maxCapacity)
    {
        if (string.IsNullOrWhiteSpace(containerTypeChar))
            throw new ArgumentException("Container type character cannot be empty.", nameof(containerTypeChar));
        if (height <= 0 || ownWeight <= 0 || depth <= 0 || maxCapacity <= 0)
            throw new ArgumentException("Container dimensions, weight, and capacity must be positive values.");

        _containerTypeChar = containerTypeChar.ToUpper();
        SerialNumber = $"KON-{_containerTypeChar}-{_nextSerialNumber++}";
        Height = height;
        OwnWeight = ownWeight;
        Depth = depth;
        MaxCapacity = maxCapacity;
        CargoWeight = 0;
    }

    public string SerialNumber { get; }
    public float CargoWeight { get; protected set; }
    public float Height { get; }
    public float OwnWeight { get; }
    public float Depth { get; }
    public float MaxCapacity { get; }

    public virtual void EmptyCargo()
    {
        CargoWeight = 0;
        Console.WriteLine($"Container {SerialNumber} emptied.");
    }

    public virtual void LoadCargo(float weightToAdd)
    {
        switch (weightToAdd)
        {
            case < 0:
                throw new ArgumentException("Cannot add negative cargo weight.");
            case 0:
                Console.WriteLine(
                    $"Container {SerialNumber}: Attempted to add 0kg. Cargo weight remains {CargoWeight}kg.");
                return;
        }

        if (CargoWeight + weightToAdd > MaxCapacity)
        {
            var availableCapacity = MaxCapacity - CargoWeight;
            throw new OverfillException(
                $"Attempt to add {weightToAdd}kg to container {SerialNumber} (Current: {CargoWeight}kg, Available: {availableCapacity}kg) would exceed its max capacity of {MaxCapacity}kg.");
        }

        CargoWeight += weightToAdd;
        Console.WriteLine(
            $"Container {SerialNumber}: Added {weightToAdd}kg of cargo. Total cargo weight is now: {CargoWeight}kg.");
    }

    public override string ToString()
    {
        return
            $"{SerialNumber} | Type: {_containerTypeChar} | Cargo: {CargoWeight}kg / {MaxCapacity}kg | Own Weight: {OwnWeight}kg | Dimensions: H:{Height}cm, D:{Depth}cm";
    }
}