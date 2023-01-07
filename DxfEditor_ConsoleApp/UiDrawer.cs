using DxfEditor_ClassLib;

namespace DxfEditor_ConsoleApp;

internal class UiDrawer
{
    public void StartUi() => drawMenu();

    private void drawMenu()
    {
        char selectedOption = askForOptionInput<MenuOptions>(Enum.GetValues<MenuOptions>());

        switch (selectedOption)
        {
            case '1':
                drawOffsetDiametersOption();
                break;
            case '2':
                drawDeleteDuplicatesOption();
                break;
        }
    }

    private void drawOffsetDiametersOption()
    {
        // Asking from user for original file path and for a new one.
        string[] paths = askForFilesPaths();

        // Asking for user to choose which diameters to offset
        string[] options = { "Offset all diameters", "Choose separately which diameters to offset" };
        char selectedOption = askForOptionInput<string>(options);

        //draw ui based on which option was selected
        int diametersChangedCount;
        if (selectedOption == '1')
        {
            double offsetValue = askForNumericalInput("__Type diameter offset size__");
            diametersChangedCount = DxfEditor.OffsetCirlces(paths[0], paths[1], null, true, offsetValue);
        }
        else
        {
            Dictionary<double, double> diameterToOffsetMap = new();

            do
            {
                double diameter = askForNumericalInput("__Type diameter size__");
                double offset = askForNumericalInput("__Type diameter offset size__");
                diameterToOffsetMap.Add(diameter, offset);

                selectedOption = askForOptionInput<string>(new string[] { "Add more diameters", "Stop adding" });
            }
            while (selectedOption != '2');

            diametersChangedCount = DxfEditor.OffsetCirlces(paths[0], paths[1], diameterToOffsetMap, false);
        }

        string howMany = diametersChangedCount == 1 ? "diameter was" : "diameters were";
        drawSuccessMessage($"{diametersChangedCount} {howMany} changed.");
    }

    private void drawDeleteDuplicatesOption()
    {
        string[] paths = askForFilesPaths();
        double range = askForNumericalInput("__Type range value to delete duplicates within__");

        (int deletedSplines, int deletedDuplicates) = DxfEditor.DeleteSplinesAndDuplicates(paths[0], paths[1], range);

        string howMany = deletedSplines == 1 ? "spline was" : "splines were";
        drawSuccessMessage($"{deletedSplines} {howMany} deleted.");

        howMany = deletedDuplicates == 1 ? "duplicate was" : "duplicates were";
        drawSuccessMessage($"{deletedDuplicates} {howMany} deleted.");
    }

    private char askForOptionInput<T>(IEnumerable<T> options)
    {
        bool toStopAskForInput = false;
        char selectedOption = 'X';

        while (!toStopAskForInput)
        {
            Console.WriteLine("__Choose from options bellow__");
            int counter = 1;
            foreach (T opt in options)
                Console.WriteLine($"[{counter++}]:{opt}");

            if (InputCollectorAndValidator.TryGetSelectedOptionInput
                     (Enumerable.Range(1, options.Count()), out selectedOption)) toStopAskForInput = true;
            else drawErrorMessage("Invalid option selected!Try once again.");
        }

        Console.WriteLine();
        return selectedOption;
    }

    private string askForTextualInput(string message)
    {
        bool toStopAskForInput = false;
        string input = string.Empty;

        while (!toStopAskForInput)
        {
            Console.WriteLine(message);
            if (InputCollectorAndValidator.TryGetTextualInput(out input, true)) toStopAskForInput = true;
            else drawErrorMessage("Invalid input!Must not be empty and begin with \"C:\\\"");
        }

        return input;
    }

    private double askForNumericalInput(string message)
    {
        bool toStopAskForInput = false;
        double input = 0;

        while (!toStopAskForInput)
        {
            Console.WriteLine(message);
            if (InputCollectorAndValidator.TryGetNumericalInput(out input)) toStopAskForInput = true;
            else drawErrorMessage("Invalid input!Try once again.");
        }

        return input;
    }

    private string[] askForFilesPaths()
    {
        string[] paths = new string[2];
        string[] messages = { "__Type path to target DXF file,relative to root folder __", "__Type path for new DXF file,relative to root folder" };

        for (int i = 0; i < messages.Length; i++)
            paths[i] = askForTextualInput(messages[i]);

        return paths;
    }

    internal void drawErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkRed;

        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();

        Console.ResetColor();
    }

    private void drawSuccessMessage(string message)
    {
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);

        Console.ResetColor();
    }
}