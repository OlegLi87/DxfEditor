using DxfEditor_ClassLib;

namespace DxfEditor_ConsoleApp;

internal class UiDrawer
{
    public void StartUi()
    {
        Console.SetWindowSize(UiConstants.Logo[0].Length, UiConstants.CONSOLE_HEIGHT);
        setColors(true);

        drawLogo();
        drawMenu();
    }

    private void setColors(bool toClear = false)
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        if (toClear) Console.Clear();

        Console.ForegroundColor = ConsoleColor.White;
    }

    private void drawLogo()
    {
        Console.Clear();

        for (int i = 0; i < UiConstants.Logo.Length; i++)
        {
            if (i == UiConstants.Logo.Length - 1) Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(UiConstants.Logo[i]);
        }

        setColors();
    }

    private void drawMenu()
    {
        char selectedOption = InputCollectorAndValidator.AskForOptionInput<MenuOptions>(Enum.GetValues<MenuOptions>(), drawMessage);
        drawLogo();

        switch (selectedOption)
        {
            case '1':
                drawOffsetDiametersOption();
                break;
            case '2':
                drawDeleteDuplicatesOption();
                break;
            case '3':
                drawCreateMeshesOption();
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
        drawLogo();
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

                selectedOption = InputCollectorAndValidator.AskForOptionInput<string>(new[] { "Add more diameters", "Stop adding" }, drawMessage);
                drawLogo();
            }
            while (selectedOption != '2');

            diametersChangedCount = dxfEditor.OffsetCirlces(diameterToOffsetMap, false);
        }

        string howMany = diametersChangedCount == 1 ? "diameter was" : "diameters were";
        drawMessage($"{diametersChangedCount} {howMany} changed.", ConsoleMessageStatus.Success);
    }

    private void drawDeleteDuplicatesOption()
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

    private void drawCreateMeshesOption()
    {
        string path = InputCollectorAndValidator.AskForFilesPaths(drawMessage, true)[0];
        var dxfEditor = new DxfEditor(path);
        var meshDatas = new List<MeshData>();
        char selectedOption;

        do
        {
            double sheetWidth = InputCollectorAndValidator.AskForNumericalInput("__Type sheet width in millimeters__", drawMessage);
            double sheetHeight = InputCollectorAndValidator.AskForNumericalInput("__Type sheet height in millimeters__", drawMessage);
            double itemWidth = InputCollectorAndValidator.AskForNumericalInput("__Type item width in millimeters__", drawMessage);
            double itemHeight = InputCollectorAndValidator.AskForNumericalInput("__Type item height in millimeters__", drawMessage);
            double overcut = InputCollectorAndValidator.AskForNumericalInput("__Type overcut in millimeters__", drawMessage, false);
            int amount = (int)(InputCollectorAndValidator.AskForNumericalInput("__Type items amount__", drawMessage));

            var meshData = new MeshData
            {
                SheetSize = new Rectangle(sheetWidth - overcut * 2, sheetHeight - overcut * 2),
                ItemSize = new Rectangle(itemWidth, itemHeight),
                Overcut = overcut,
                Amount = amount
            };
            meshDatas.Add(meshData);

            selectedOption = InputCollectorAndValidator.AskForOptionInput<string>(new[] { "Add more meshes", "Stop adding" }, drawMessage);
            drawLogo();
        }
        while (selectedOption != '2');

        dxfEditor.CreateMeshes(meshDatas);
    }

    internal void drawMessage(string message, ConsoleMessageStatus status)
    {
        if (status == ConsoleMessageStatus.Success)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
        }
        else if (status == ConsoleMessageStatus.Error)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
        }

        Console.WriteLine();
        Console.WriteLine(" " + message);
        Console.WriteLine();

        setColors();
    }
}