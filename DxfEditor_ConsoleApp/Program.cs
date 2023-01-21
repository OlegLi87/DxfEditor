using System.Diagnostics;
using DxfEditor_ConsoleApp;

string path = Path.Combine(Environment.CurrentDirectory, @"exceptionLogs.txt");
using var streamWriter = new StreamWriter(path, true);

Trace.Listeners.Add(new TextWriterTraceListener(streamWriter));
Trace.AutoFlush = true;

bool toContinue = true;
while (toContinue)
{
    var uiDrawer = new UiDrawer();
    try
    {
        uiDrawer.StartUi();
    }
    catch (Exception ex)
    {
        uiDrawer.drawMessage(ex.Message, ConsoleMessageStatus.Error);
        Trace.WriteLine(DateTime.Now.ToString("dd/M/yy H:mm") + " : " + ex.ToString());
        Trace.Flush();
    }

    Console.WriteLine("Press \"y\" to reapeat from the main menu.");
    toContinue = Console.ReadKey().KeyChar == 'y';
}

Trace.Close();







