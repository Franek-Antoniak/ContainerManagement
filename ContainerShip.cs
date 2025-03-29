namespace ContainerManagement;

public class ContainerShip
{
    private static int _nextShipId = 1;


    private ContainerShip(string name, float maxSpeed, int maxContainerCount, float maxWeightCapacityTons)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ship name cannot be empty", nameof(name));
        if (maxSpeed <= 0 || maxContainerCount <= 0 || maxWeightCapacityTons <= 0)
            throw new ArgumentException("Ship max speed, container count, and weight capacity must be positive.");

        Name = name;
        MaxSpeed = maxSpeed;
        MaxContainerCount = maxContainerCount;
        MaxWeightCapacityTons = maxWeightCapacityTons;
    }

    public ContainerShip(float maxSpeed, int maxContainerCount, float maxWeightCapacityTons)
        : this($"Ship-{_nextShipId++}", maxSpeed, maxContainerCount, maxWeightCapacityTons)
    {
    }

    private float MaxSpeed { get; } // in knots
    private int MaxContainerCount { get; }
    private float MaxWeightCapacityTons { get; }
    private List<Container> Containers { get; } = [];
    public string Name { get; }


    public bool LoadContainer(Container container)
    {
        if (Containers.Count >= MaxContainerCount)
        {
            Console.WriteLine(
                $"Cannot load container {container.SerialNumber} onto {Name}: Ship is at maximum container capacity ({MaxContainerCount}).");
            return false;
        }

        var currentTotalWeightKg = GetCurrentTotalWeightKg();
        var newContainerTotalWeightKg = container.OwnWeight + container.CargoWeight;
        var projectedTotalWeightTons = (currentTotalWeightKg + newContainerTotalWeightKg) / 1000f;

        if (projectedTotalWeightTons > MaxWeightCapacityTons)
        {
            Console.WriteLine(
                $"Cannot load container {container.SerialNumber} onto {Name}: Ship's maximum weight capacity of {MaxWeightCapacityTons} tons would be exceeded (Projected: {projectedTotalWeightTons:F2}t).");
            return false;
        }

        Containers.Add(container);
        Console.WriteLine($"Container {container.SerialNumber} successfully loaded onto ship {Name}.");
        return true;
    }

    public void LoadContainers(List<Container> containersToLoad)
    {
        if (containersToLoad.Count == 0)
        {
            Console.WriteLine($"Warning on {Name}: No containers provided to load.");
            return;
        }

        Console.WriteLine($"Attempting to load {containersToLoad.Count} containers onto ship {Name}...");
        var successCount = 0;
        var failCount = 0;
        foreach (var container in containersToLoad)
            if (LoadContainer(container))
                successCount++;
            else
                failCount++;

        Console.WriteLine(
            $"Loading process for ship {Name} completed. Successfully loaded: {successCount}, Failed to load: {failCount}.");
    }

    public bool RemoveContainer(string serialNumber)
    {
        var container = FindContainerBySerial(serialNumber);
        if (container == null)
        {
            Console.WriteLine($"Cannot remove container {serialNumber} from {Name}: Not found on ship.");
            return false;
        }

        Containers.Remove(container);
        Console.WriteLine($"Container {serialNumber} removed from ship {Name}.");
        return true;
    }

    public bool UnloadContainerCargo(string serialNumber)
    {
        var container = FindContainerBySerial(serialNumber);
        if (container == null)
        {
            Console.WriteLine($"Cannot unload cargo from {serialNumber} on {Name}: Container not found.");
            return false;
        }

        container.EmptyCargo();
        return true;
    }
    
    public bool ReplaceContainer(string oldSerialNumber, Container newContainer)
    {
        if (FindContainerBySerial(newContainer.SerialNumber) != null)
        {
            Console.WriteLine(
                $"Error on {Name}: Cannot replace with container {newContainer.SerialNumber}, as a container with this serial already exists on the ship.");
            return false;
        }

        var oldContainer = FindContainerBySerial(oldSerialNumber);
        if (oldContainer == null)
        {
            Console.WriteLine($"Cannot replace container {oldSerialNumber} on {Name}: Not found on ship.");
            return false;
        }

        var oldContainerWeightKg = oldContainer.OwnWeight + oldContainer.CargoWeight;
        var newContainerWeightKg = newContainer.OwnWeight + newContainer.CargoWeight;
        var currentWeightKg = GetCurrentTotalWeightKg();
        var projectedWeightTons = (currentWeightKg - oldContainerWeightKg + newContainerWeightKg) / 1000f;

        if (projectedWeightTons > MaxWeightCapacityTons)
        {
            Console.WriteLine(
                $"Cannot replace container {oldSerialNumber} with {newContainer.SerialNumber} on {Name}: Ship's maximum weight capacity of {MaxWeightCapacityTons} tons would be exceeded (Projected: {projectedWeightTons:F2}t).");
            return false;
        }

        var index = Containers.IndexOf(oldContainer);
        Console.WriteLine(
            $"Container {oldSerialNumber} on ship {Name} is being replaced with {newContainer.SerialNumber}.");
        Containers[index] = newContainer;
        
        return true;
    }

    public bool TransferContainer(ContainerShip targetShip, string serialNumber)
    {
        if (targetShip == this)
        {
            Console.WriteLine($"Error on {Name}: Cannot transfer container {serialNumber} to the same ship.");
            return false;
        }


        var container = FindContainerBySerial(serialNumber);
        if (container == null)
        {
            Console.WriteLine($"Cannot transfer container {serialNumber}: Not found on source ship {Name}.");
            return false;
        }

        if (targetShip.LoadContainer(container))
        {
            Containers.Remove(container);
            Console.WriteLine(
                $"Container {serialNumber} successfully transferred from ship {Name} to ship {targetShip.Name}.");
            return true;
        }

        Console.WriteLine($"Transfer of container {serialNumber} from {Name} to {targetShip.Name} failed.");
        return false;
    }

    public void DisplayContainerInfo(string serialNumber)
    {
        var container = FindContainerBySerial(serialNumber);
        if (container == null)
        {
            Console.WriteLine($"Container {serialNumber} not found on ship {Name}.");
        }
        else
        {
            Console.WriteLine($"Information for container {serialNumber} on ship {Name}:");
            Console.WriteLine(container.ToString());
        }
    }

    public void DisplayShipInfo()
    {
        Console.WriteLine($"\n--- Ship Information: {Name} ---");
        Console.WriteLine($"- Max Speed: {MaxSpeed} knots");
        Console.WriteLine($"- Max Containers: {MaxContainerCount}");
        Console.WriteLine($"- Max Weight: {MaxWeightCapacityTons} tons");
        Console.WriteLine($"- Current Containers: {Containers.Count}");
        var currentWeightKg = GetCurrentTotalWeightKg();
        Console.WriteLine($"- Current Total Weight: {currentWeightKg / 1000f:F2} tons");

        if (Containers.Count == 0)
        {
            Console.WriteLine("- Load: Ship is empty.");
        }
        else
        {
            Console.WriteLine("- Containers on board:");
            foreach (var container in Containers) Console.WriteLine($"  - {container}");
        }

        Console.WriteLine($"--- End of Ship Information: {Name} ---\n");
    }

    private Container? FindContainerBySerial(string serialNumber)
    {
        ArgumentNullException.ThrowIfNull(serialNumber);

        return Containers.FirstOrDefault(c =>
            c.SerialNumber.Equals(serialNumber, StringComparison.OrdinalIgnoreCase));
    }

    private float GetCurrentTotalWeightKg()
    {
        return Containers.Sum(c => c.OwnWeight + c.CargoWeight);
    }
}