using IxMilia.Dxf;
using IxMilia.Dxf.Entities;

namespace DxfEditor_ClassLib;

internal static class DxfExtensions
{
    internal static DxfLine GetOffsetLine(this DxfLine line, double offset, InsertionDirection direction)
    {
        DxfPoint p1, p2;

        if (direction == InsertionDirection.WidthAlongWidth)
        {
            p1 = new DxfPoint(line.P1.X + offset, line.P1.Y, 0);
            p2 = new DxfPoint(line.P2.X + offset, line.P2.Y, 0);
        }
        else
        {
            p1 = new DxfPoint(line.P1.X, line.P1.Y + offset, 0);
            p2 = new DxfPoint(line.P2.X, line.P2.Y + offset, 0);
        }

        return new DxfLine(p1, p2);
    }
}