using IxMilia.Dxf;

namespace DxfEditor_ClassLib;

public class MeshData
{
    internal void Deconstruct(out Rectangle sheet, out Rectangle item,
                                 out double overcut, out int amount, out DxfPoint origin)
    {
        sheet = SheetSize;
        item = ItemSize;
        overcut = Overcut;
        amount = Amount;
        origin = Origin;
    }

    public Rectangle SheetSize { get; init; }
    public Rectangle ItemSize { get; init; }
    public double Overcut { get; init; }
    public int Amount { get; init; }
    internal DxfPoint Origin { get; set; }
}