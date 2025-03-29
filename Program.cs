namespace ContainerManagement;

internal abstract class Program
{
    private static void Main()
    {
        Console.WriteLine("--- Container Management System Demo (Additive Loading) ---");

        Console.WriteLine("\n>>> Creating container ships...");
        var ship1 = new ContainerShip(25, 5, 50);
        var ship2 = new ContainerShip(20, 10, 80);
        Console.WriteLine($"Created ship: {ship1.Name}...");
        Console.WriteLine($"Created ship: {ship2.Name}...");

        // ----- Tworzenie Kontenerów -----
        Console.WriteLine("\n>>> Creating various containers...");
        try
        {
            var liquidContainer = new LiquidContainer(200, 1500, 200, 10000, false); // 10t capacity
            var
                hazardousLiquidContainer =
                    new LiquidContainer(200, 1600, 200, 10000, true); // 10t capacity, 5t limit
            var gasContainer = new GasContainer(180, 1200, 180, 8000, 15); // 8t capacity
            var refrigeratedContainerFish =
                new RefrigeratedContainer(220, 2000, 210, 12000, "Fish", 2); // 12t capacity
            var refrigeratedContainerBananas =
                new RefrigeratedContainer(220, 2000, 210, 12000, "Bananas", 14); // 12t capacity

            Console.WriteLine($"Created: {liquidContainer}");
            Console.WriteLine($"Created: {hazardousLiquidContainer}");
            Console.WriteLine($"Created: {gasContainer}");
            Console.WriteLine($"Created: {refrigeratedContainerFish}");
            Console.WriteLine($"Created: {refrigeratedContainerBananas}");


            // ----- Ładowanie Ładunku do Kontenerów (Logika Dodawania) -----
            Console.WriteLine("\n>>> Loading cargo into containers (additive logic)...");

            Console.WriteLine(
                $"\n--- Loading {liquidContainer.SerialNumber} (Max: {liquidContainer.MaxCapacity * 0.9f}kg) ---");
            liquidContainer.LoadCargo(4000); // Add 4000kg. Total: 4000kg
            liquidContainer.LoadCargo(4500); // Add 4500kg. Total: 8500kg (OK, < 9000kg limit)
            try
            {
                Console.WriteLine("Attempting to add more cargo (should exceed 90% limit)...");
                liquidContainer.LoadCargo(600); // Try to add 600kg. 8500 + 600 = 9100kg > 9000kg limit -> Error
            }
            catch (OverfillException ex)
            {
                Console.WriteLine($"Caught expected exception: {ex.Message}");
            }

            Console.WriteLine($"Final state: {liquidContainer}");


            Console.WriteLine(
                $"\n--- Loading {hazardousLiquidContainer.SerialNumber} (Max: {hazardousLiquidContainer.MaxCapacity * 0.5f}kg) ---");
            hazardousLiquidContainer.LoadCargo(2000); // Add 2000kg. Total: 2000kg
            hazardousLiquidContainer.LoadCargo(3000); // Add 3000kg. Total: 5000kg (OK, <= 5000kg limit)
            try
            {
                Console.WriteLine("Attempting to add more cargo (should exceed 50% limit)...");
                hazardousLiquidContainer.LoadCargo(1); // Try to add 1kg. 5000 + 1 = 5001kg > 5000kg limit -> Error
            }
            catch (OverfillException ex)
            {
                Console.WriteLine($"Caught expected exception: {ex.Message}");
            }

            Console.WriteLine($"Final state: {hazardousLiquidContainer}");


            Console.WriteLine($"\n--- Loading {gasContainer.SerialNumber} (Max: {gasContainer.MaxCapacity}kg) ---");
            gasContainer.LoadCargo(7000); // Add 7000kg. Total: 7000kg
            try
            {
                Console.WriteLine("Attempting to add more cargo (should exceed max capacity)...");
                gasContainer.LoadCargo(1001); // Try to add 1001kg. 7000 + 1001 = 8001kg > 8000kg limit -> Error
            }
            catch (OverfillException ex)
            {
                Console.WriteLine($"Caught expected exception: {ex.Message}");
            }

            // Opróżnienie i ponowne załadowanie, aby pokazać inną ścieżkę
            Console.WriteLine("Emptying and reloading gas container...");
            gasContainer.EmptyCargo(); // Leaves 5% -> 7000 * 0.05 = 350kg
            Console.WriteLine($"State after emptying: {gasContainer}");
            gasContainer.LoadCargo(7600); // Add 7600kg. Total 350 + 7600 = 7950kg (OK)
            Console.WriteLine($"Final state: {gasContainer}");


            Console.WriteLine(
                $"\n--- Loading {refrigeratedContainerFish.SerialNumber} (Max: {refrigeratedContainerFish.MaxCapacity}kg) ---");
            refrigeratedContainerFish.LoadCargo(5000); // Add 5000kg. Total: 5000kg
            refrigeratedContainerFish.LoadCargo(6000); // Add 6000kg. Total: 11000kg (OK < 12000kg)
            Console.WriteLine($"Final state: {refrigeratedContainerFish}");


            // ----- Operacje na Statku 1 (teraz z kontenerami załadowanymi addytywnie) -----
            Console.WriteLine("\n>>> Operations on Ship 1...");
            ship1.DisplayShipInfo();

            // Kontenery mają teraz następujące wagi ładunku:
            // liquidContainer: 8500kg
            // hazardousLiquidContainer: 5000kg
            // gasContainer: 7950kg (po opróżnieniu i doładowaniu)
            // refrigeratedContainerFish: 11000kg
            // refrigeratedContainerBananas: 0kg (nie ładowany)

            ship1.LoadContainer(liquidContainer); // Waga kontenera: 1500 + 8500 = 10000 kg
            ship1.LoadContainer(hazardousLiquidContainer); // Waga kontenera: 1600 + 5000 = 6600 kg
            // Suma: 16600 kg (16.6t) < 50t OK. Liczba: 2 < 5 OK.
            ship1.DisplayShipInfo();

            var containersForList = new List<Container> { gasContainer, refrigeratedContainerFish };
            // Wagi: (1200 + 7950) + (2000 + 11000) = 9150 + 13000 = 22150 kg
            // Suma całkowita: 16600 + 22150 = 38750 kg (38.75t) < 50t OK. Liczba kontenerów: 2+2 = 4 < 5 OK.
            ship1.LoadContainers(containersForList);
            ship1.DisplayShipInfo();

            // Próba załadowania 5-tego kontenera (Bananas, pusty)
            Console.WriteLine("Attempting to load 5th container (Bananas, empty)...");
            ship1.LoadContainer(refrigeratedContainerBananas); // Waga: 2000 + 0 = 2000kg
            // Suma: 38750 + 2000 = 40750 kg (40.75t) < 50t OK. Liczba: 5 <= 5 OK.
            ship1.DisplayShipInfo(); // Powinien mieć 5 kontenerów

            // Próba załadowania 6-tego kontenera (przekroczenie liczby)
            Console.WriteLine("Attempting to load 6th container (should exceed count limit)...");
            var extraLiquid = new LiquidContainer(100, 1000, 100, 5000, false);
            extraLiquid.LoadCargo(1000); // Total weight: 1000kg
            ship1.LoadContainer(extraLiquid); // Liczba: 6 > 5 -> BŁĄD

            // ... (Reszta operacji na statkach: Remove, UnloadCargo, Transfer, Replace, DisplayContainerInfo)
            // Te operacje nie wymagały zmian, ale ich efekt będzie widoczny na kontenerach
            // z wagami ustawionymi przez logikę dodawania. Można je tutaj umieścić jak poprzednio.

            Console.WriteLine("\n>>> Other ship operations demo...");
            // Wyświetlenie informacji o kontenerze
            ship1.DisplayContainerInfo(liquidContainer.SerialNumber);

            // Opróżnienie ładunku kontenera na statku
            ship1.UnloadContainerCargo(liquidContainer.SerialNumber);
            ship1.DisplayContainerInfo(liquidContainer.SerialNumber); // Pokaże 0kg ładunku

            // Usunięcie kontenera
            ship1.RemoveContainer(hazardousLiquidContainer.SerialNumber);
            ship1.DisplayShipInfo();

            // Transfer
            Console.WriteLine("\nTransferring container...");
            ship1.TransferContainer(ship2, gasContainer.SerialNumber);
            ship1.DisplayShipInfo();
            ship2.DisplayShipInfo();

            // Zastąpienie
            Console.WriteLine("\nReplacing container...");
            var replacement = new LiquidContainer(180, 1400, 180, 9000, false);
            replacement.LoadCargo(500); // Ładujemy trochę do nowego
            replacement.LoadCargo(1500); // Dodajemy więcej. Total: 2000kg
            // Waga: 1400 + 2000 = 3400kg
            // Zastępujemy Bananas (waga 2000kg) 
            ship1.ReplaceContainer(refrigeratedContainerBananas.SerialNumber, replacement);
            ship1.DisplayShipInfo();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n!!! An unexpected error occurred during the demo: {ex.Message} !!!");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine("\n--- Demo Completed ---");
    }
}