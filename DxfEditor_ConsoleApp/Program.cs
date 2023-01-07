using DxfEditor_ConsoleApp;

var uiDrawer = new UiDrawer();

try
{
    uiDrawer.StartUi();
}
catch (Exception ex)
{
    uiDrawer.drawErrorMessage(ex.Message);
}

Console.ResetColor();
Console.ReadLine();





