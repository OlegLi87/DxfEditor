using System.Text;
using System.Text.RegularExpressions;

namespace DxfEditor_ConsoleApp;

internal static class InputCollectorAndValidator
{
    private static readonly char[] _numericalChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', };

    internal static char AskForOptionInput<T>(IEnumerable<T> options, Action<string, ConsoleMessageStatus> messageDrawer)
    {
        bool toStopAskForInput = false;
        char selectedOption = 'X';

        while (!toStopAskForInput)
        {
            Console.WriteLine("__Choose from options bellow__");
            int counter = 1;
            foreach (T opt in options)
                Console.WriteLine($"[{counter++}]:{opt}");

            if (tryGetSelectedOptionInput(Enumerable.Range(1, options.Count()), out selectedOption)) toStopAskForInput = true;
            else messageDrawer("Invalid option selected!Try once again.", ConsoleMessageStatus.Error);
        }

        Console.WriteLine();
        return selectedOption;
    }

    internal static string AskForTextualInput(string message, Action<string, ConsoleMessageStatus> messageDrawer,
                                                bool checkForPath = false)
    {
        bool toStopAskForInput = false;
        string input = string.Empty;

        while (!toStopAskForInput)
        {
            Console.WriteLine(message);
            if (tryGetTextualInput(out input, checkForPath)) toStopAskForInput = true;
            else
            {
                var strBuilder = new StringBuilder();
                strBuilder.Append("Invalid Input!Must not be empty");
                if (checkForPath) strBuilder.Append(" and begin with \"C:\\\" or \"c:\\\"");

                messageDrawer(strBuilder.ToString(), ConsoleMessageStatus.Error);
            }
        }

        return input;
    }

    internal static double AskForNumericalInput(string message, Action<string, ConsoleMessageStatus> messageDrawer, bool largerThanZero = true)
    {
        bool toStopAskForInput = false;
        double input = 0;

        while (!toStopAskForInput)
        {
            Console.WriteLine(message);
            if (tryGetNumericalInput(out input, largerThanZero)) toStopAskForInput = true;
            else messageDrawer("Invalid input!Try once again.", ConsoleMessageStatus.Error);
        }

        return input;
    }

    internal static string[] AskForFilesPaths(Action<string, ConsoleMessageStatus> messageDrawer)
    {
        string[] messages = { "__Type path to target DXF file,relative to root folder __", "__Type path for new DXF file,relative to root folder" };
        string[] paths = new string[messages.Length];

        for (int i = 0; i < messages.Length; i++)
            paths[i] = AskForTextualInput(messages[i], messageDrawer, true);

        var regex = new Regex(@".dxf$");

        for (int i = 0; i < paths.Length; i++)
            if (!regex.IsMatch(paths[i])) paths[i] += ".dxf";

        return paths;
    }

    private static bool tryGetSelectedOptionInput(IEnumerable<int> allowedOptions, out char input)
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey();
        input = keyInfo.KeyChar;

        foreach (int opt in allowedOptions)
            if (_numericalChars[opt] == input) return true;

        return false;
    }

    private static bool tryGetTextualInput(out string input, bool checkForPath = false)
    {
        input = Console.ReadLine() ?? string.Empty;
        if (input.Length == 0 || string.IsNullOrWhiteSpace(input)) return false;
        if (checkForPath)
        {
            var regex = new Regex(@"^(C|c):\\");
            return regex.IsMatch(input);
        }

        return true;
    }

    private static bool tryGetNumericalInput(out double input, bool largerThanZero = true)
    {
        string textualInput;
        input = 0;

        return tryGetTextualInput(out textualInput) && Double.TryParse(textualInput, out input)
                  && (!largerThanZero || input > 0);
    }
}