using DxfEditor_ConsoleApp;

var uiDrawer = new UiDrawer();

try
{
    uiDrawer.StartUi();
}
catch (Exception ex)
{
    uiDrawer.drawMessage(ex.Message, ConsoleMessage.Error);
}

Console.ResetColor();
Console.ReadLine();





