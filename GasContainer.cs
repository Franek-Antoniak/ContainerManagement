namespace ContainerManagement;

public class GasContainer : Container, IHazardNotifier
{
    public GasContainer(float height, float ownWeight, float depth, float maxCapacity, float pressure)
        : base("G", height, ownWeight, depth, maxCapacity)
    {
        if (pressure <= 0)
            throw new ArgumentException("Pressure must be positive.", nameof(pressure));
        Pressure = pressure;
    }

    private float Pressure { get; }

    public void NotifyHazard(string message)
    {
        Console.WriteLine($"!!! HAZARD NOTIFICATION !!! Container: {SerialNumber}, Message: {message}");
    }


    public override void EmptyCargo()
    {
        var residualCargo = CargoWeight * 0.05f;
        CargoWeight = residualCargo;
        Console.WriteLine($"Gas container {SerialNumber}: Emptied, maintaining {CargoWeight:F2}kg (5%) residual gas.");
    }

    public override string ToString()
    {
        return $"{base.ToString()} | Pressure: {Pressure} atm";
    }
}