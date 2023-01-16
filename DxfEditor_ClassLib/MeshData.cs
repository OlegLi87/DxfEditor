namespace DxfEditor_ClassLib;

public class MeshData
{
    internal void Deconstruct(out Rectangle sheet, out Rectangle item, out double overcut, out int amount)
    {
        sheet = SheetSize;
        item = ItemSize;
        overcut = Overcut;
        amount = Amount;
    }

    public Rectangle SheetSize { get; init; }
    public Rectangle ItemSize { get; init; }
    public double Overcut { get; init; }
    public int Amount { get; init; }
}