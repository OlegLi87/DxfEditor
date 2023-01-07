using System.Text.RegularExpressions;

namespace DxfEditor_ConsoleApp;

internal static class InputCollectorAndValidator
{
    private static readonly char[] _numericalChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', };

    internal static bool TryGetSelectedOptionInput(IEnumerable<int> allowedOptions, out char input)
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey();
        input = keyInfo.KeyChar;

        foreach (int opt in allowedOptions)
            if (_numericalChars[opt] == input) return true;

        return false;
    }

    internal static bool TryGetTextualInput(out string input, bool checkForPath = false)
    {
        input = Console.ReadLine() ?? string.Empty;
        if (input.Length == 0 || string.IsNullOrWhiteSpace(input)) return false;
        if (checkForPath)
        {
            var regex = new Regex(@"^C:\\");
            return regex.IsMatch(input);
        }

        return true;
    }

    internal static bool TryGetNumericalInput(out double input)
    {
        string textualInput;
        input = 0;

        return TryGetTextualInput(out textualInput) && Double.TryParse(textualInput, out input);
    }
}