namespace ContainerManagement;

public class RefrigeratedContainer : Container
{
    private static readonly Dictionary<string, float> RequiredTemperatures =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Bananas", 13.3f },
            { "Chocolate", 18.0f },
            { "Fish", 2.0f },
            { "Meat", -15.0f },
            { "Ice Cream", -18.0f },
            { "Frozen pizza", -30.0f },
            { "Cheese", 7.2f },
            { "Sausages", 5 },
            { "Butter", 20.5f },
            { "Eggs", 19.0f }
        };

    public RefrigeratedContainer(float height, float ownWeight, float depth, float maxCapacity, string productType,
        float temperature)
        : base("C", height, ownWeight, depth, maxCapacity)
    {
        if (string.IsNullOrWhiteSpace(productType))
            throw new ArgumentException("Product type cannot be empty.", nameof(productType));

        if (RequiredTemperatures.TryGetValue(productType, out var requiredTemperature))
        {
            if (temperature < requiredTemperature)
                throw new ArgumentException(
                    $"Temperature {temperature}°C is too low for product '{productType}'. Required minimum temperature is {requiredTemperature}°C.");
        }
        else
        {
            Console.WriteLine(
                $"Warning: Product type '{productType}' has no defined minimum temperature requirement. Assuming temperature {temperature}°C is acceptable.");
        }


        ProductType = productType;
        Temperature = temperature;
    }

    public string ProductType { get; private set; }
    public float Temperature { get; }

    public static bool IsTemperatureSuitableForProduct(string productType, float temperature)
    {
        if (RequiredTemperatures.TryGetValue(productType, out var requiredTemperature))
            return temperature >= requiredTemperature;
        return true;
    }

	
	public bool ChangeProductType(string newProductType)
{
    if (string.IsNullOrWhiteSpace(newProductType))
    {
        Console.WriteLine($"Error changing product type in {SerialNumber}: New product type cannot be empty.");
        return false;
    }

    if (this.CargoWeight > 0)
    {
        Console.WriteLine($"Error changing product type in {SerialNumber}: Cannot change product type for a non-empty container. Please empty the container first (Current Weight: {this.CargoWeight}kg).");
        return false;
    }

    if (RequiredTemperatures.TryGetValue(newProductType, out var requiredTemperature))
    {
        if (this.Temperature < requiredTemperature)
        {
            Console.WriteLine(
                $"Cannot change product type in {SerialNumber} to '{newProductType}'. The current container temperature ({this.Temperature}°C) is too low. Required minimum temperature is {requiredTemperature}°C.");
            return false;
        }
    }
     else {
           Console.WriteLine(
                $"Warning: New product type '{newProductType}' has no defined minimum temperature requirement. Assuming current temperature {this.Temperature}°C is acceptable.");
     }


    Console.WriteLine(
        $"Container {SerialNumber}: Changing product type from '{ProductType}' to '{newProductType}'.");
    ProductType = newProductType;
    return true;
}	

    public override string ToString()
    {
        return $"{base.ToString()} | Product: {ProductType} | Temp: {Temperature}°C";
    }
}