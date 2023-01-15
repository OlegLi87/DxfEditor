using DxfEditor_ClassLib;

namespace DxfEditor_ConsoleApp;

internal class UiDrawer
{
    public void StartUi()
    {
        Console.Clear();
        drawMenu();
    }

    private void drawMenu()
    {
        char selectedOption = InputCollectorAndValidator.AskForOptionInput<MenuOptions>(Enum.GetValues<MenuOptions>(), drawMessage);

        switch (selectedOption)
        {
            case '1':
                drawOffsetDiametersOption();
                break;
            case '2':
                drawDeleteDuplicatesOptions();
                break;
        }
    }

    private void drawOffsetDiametersOption()
    {
        // Asking from user for original file path and for a new one.
        string[] paths = InputCollectorAndValidator.AskForFilesPaths(drawMessage);
        var dxfEditor = new DxfEditor(paths[1], paths[0]);

        // Asking for user to choose which diameters to offset
        string[] options = { "Offset all diameters", "Choose separately which diameters to offset" };
        char selectedOption = InputCollectorAndValidator.AskForOptionInput<string>(options, drawMessage);
        double offset;

        //draw ui based on which option was selected
        int diametersChangedCount;
        if (selectedOption == '1')
        {
            offset = InputCollectorAndValidator.AskForNumericalInput("__Type diameter offset size__", drawMessage, false);
            diametersChangedCount = dxfEditor.OffsetCirlces(null, true, offset);
        }
        else
        {
            Dictionary<double, double> diameterToOffsetMap = new();

            do
            {
                double diameter = InputCollectorAndValidator.AskForNumericalInput("__Type diameter size__", drawMessage);
                offset = InputCollectorAndValidator.AskForNumericalInput("__Type diameter offset size__", drawMessage, false);
                diameterToOffsetMap.Add(diameter, offset);

                selectedOption = InputCollectorAndValidator.AskForOptionInput<string>(new string[] { "Add more diameters", "Stop adding" }, drawMessage);
            }
            while (selectedOption != '2');

            diametersChangedCount = dxfEditor.OffsetCirlces(diameterToOffsetMap, false);
        }

        string howMany = diametersChangedCount == 1 ? "diameter was" : "diameters were";
        drawMessage($"{diametersChangedCount} {howMany} changed.", ConsoleMessageStatus.Success);
    }

    private void drawDeleteDuplicatesOptions()
    {
        string[] paths = InputCollectorAndValidator.AskForFilesPaths(drawMessage);
        var dxfEditor = new DxfEditor(paths[1], paths[0]);

        double range = InputCollectorAndValidator.AskForNumericalInput("__Type range value to delete duplicates within__", drawMessage);

        (int deletedSplines, int deletedDuplicates) = dxfEditor.DeleteSplinesAndDuplicates(range);

        string howMany = deletedSplines == 1 ? "spline was" : "splines were";
        drawMessage($"{deletedSplines} {howMany} deleted.", ConsoleMessageStatus.Success);

        howMany = deletedDuplicates == 1 ? "duplicate was" : "duplicates were";
        drawMessage($"{deletedDuplicates} {howMany} deleted.", ConsoleMessageStatus.Success);
    }

    internal void drawMessage(string message, ConsoleMessageStatus status)
    {
        if (status == ConsoleMessageStatus.Success)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (status == ConsoleMessageStatus.Error)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
        }

        Console.WriteLine(message);
        Console.WriteLine();

        Console.ResetColor();
    }
}