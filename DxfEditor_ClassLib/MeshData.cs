using IxMilia.Dxf;
using IxMilia.Dxf.Entities;

namespace DxfEditor_ClassLib;

public class MeshData
{
    public Rectangle SheetSize { get; init; }
    public Rectangle ItemSize { get; init; }
    public double Overcut { get; init; }
    public int Amount { get; init; }
    internal DxfPoint Origin { get; set; }
    internal List<DxfLine> HorizontalLines { get; } = new();
    internal List<DxfLine> VerticalLines { get; } = new();

    public static (IEnumerable<DxfLine> hLines, IEnumerable<DxfLine> vLines) operator +(MeshData meshDataLeft, MeshData meshDataRight)
       => (meshDataLeft.HorizontalLines.Concat(meshDataRight.HorizontalLines), meshDataLeft.VerticalLines.Concat(meshDataRight.VerticalLines));

    internal void Deconstruct(out Rectangle sheet, out Rectangle item, out double overcut, out int amount, out DxfPoint origin)
    {
        sheet = SheetSize;
        item = ItemSize;
        overcut = Overcut;
        amount = Amount;
        origin = Origin;
    }

    internal void Deconstruct(out List<DxfLine> hLines, out List<DxfLine> vLines)
    {
        hLines = HorizontalLines;
        vLines = VerticalLines;
    }
}