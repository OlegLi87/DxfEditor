using IxMilia.Dxf;
using IxMilia.Dxf.Entities;

namespace DxfEditor_ClassLib;

internal static class DxfUtils
{
    internal static DxfFile LoadDxfFile(string? path)
    {
        if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path value is empty or missing");

        DxfFile? dxfFile = DxfFile.Load(path);
        if (dxfFile is null) throw new ArgumentException("Could not load dxf file");

        return dxfFile;
    }

    internal static int DeleteSplines(DxfFile dxfFile)
    {
        int deletedSplines = 0;
        var entities = dxfFile.Entities.ToList();

        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i] is DxfPolyline poly)
            {
                var newEntities = poly.AsSimpleEntities();
                dxfFile.Entities.Remove(poly);

                foreach (var e in newEntities)
                    dxfFile.Entities.Add(e);

                deletedSplines++;
            }
        }

        return deletedSplines;
    }

    internal static bool AreDuplicates((DxfEntity, DxfEntity) entities, double range) =>
         entities switch
         {
             (DxfLine l1, DxfLine l2) => arePointsWithinRange(l1.P1, l2.P1, range) && arePointsWithinRange(l1.P2, l2.P2, range)
                                       || arePointsWithinRange(l1.P2, l2.P1, range) && arePointsWithinRange(l1.P1, l2.P2, range),
             (DxfArc a1, DxfArc a2) => Math.Abs(a1.Radius - a2.Radius) < range && areBoundingBoxesWithinRange(a1.GetBoundingBox(), a2.GetBoundingBox(), range),
             (DxfCircle c1, DxfCircle c2) => Math.Abs(c1.Radius - c2.Radius) < range && arePointsWithinRange(c1.Center, c2.Center, range),
             _ => false
         };

    internal static (List<DxfLine> hLines, List<DxfLine> vLines) CreateLines(MeshData meshData, DxfPoint origin)
    {
        var horizontalLines = new List<DxfLine>();
        var verticalLines = new List<DxfLine>();

        // gathering all necessery data to work with
        (Rectangle sheet, Rectangle item, double overcut, int amount) = meshData;

        InsertionDirection direction = sheet.GetOptimalInsertionDirection(item);
        double incrementAlongWidth = direction == InsertionDirection.WidthAlongWidth ? item.Width : item.Height;
        int itemsNumberAlongWidth = (int)(sheet.Width / incrementAlongWidth);

        double incrementAlongHeight = direction == InsertionDirection.HeightAlongWidth ? item.Height : item.Width;
        int itemsCounter = 0;
        bool isFirstRoundTrip = true;

        // creating lines along width of the sheet aka X axis
        while (itemsCounter < amount && (horizontalLines.Count - 1) * incrementAlongHeight <= sheet.Height - incrementAlongHeight)
        {
            // building a new line
            if (isFirstRoundTrip || amount - itemsCounter - itemsNumberAlongWidth < 0)
            {
                double lineLength = 0;
                while (itemsCounter < amount && lineLength <= sheet.Width - incrementAlongWidth)
                {
                    lineLength += incrementAlongWidth;
                    itemsCounter++;
                }
                var line = new DxfLine(new DxfPoint(origin.X - overcut, origin.Y, 0), new DxfPoint(origin.X + lineLength + overcut, origin.Y, 0));
                horizontalLines.Add(line);

                // at once building the second line if first line was just created
                if (isFirstRoundTrip)
                {
                    line = line.GetOffsetLine(InsertionDirection.HeightAlongWidth, incrementAlongHeight);
                    horizontalLines.Add(line);
                    isFirstRoundTrip = false;
                }
            }
            // creating a new line by offseting the last horizontal line
            else
            {
                var line = horizontalLines[^1].GetOffsetLine(InsertionDirection.HeightAlongWidth, incrementAlongHeight);
                horizontalLines.Add(line);
                itemsCounter += itemsNumberAlongWidth;
            }
        }

        // creating vertical lines
        isFirstRoundTrip = true;
        var ceilLine = horizontalLines[^1];

        while (verticalLines.Count - 1 < itemsNumberAlongWidth)
        {
            DxfLine line;
            // building a new line
            if (isFirstRoundTrip || (verticalLines.Count - 1) * incrementAlongWidth <= ceilLine.P2.X - incrementAlongWidth)
            {
                if (!isFirstRoundTrip) ceilLine = horizontalLines[^2];
                else isFirstRoundTrip = false;

                line = new DxfLine(new DxfPoint(origin.X, origin.Y - overcut, 0), new DxfPoint(origin.X, ceilLine.P1.Y + overcut, 0));
                verticalLines.Add(line);
            }

            line = verticalLines[^1].GetOffsetLine(InsertionDirection.WidthAlongWidth, incrementAlongWidth);
            verticalLines.Add(line);
        }

        return (horizontalLines, verticalLines);
    }

    private static bool arePointsWithinRange(DxfPoint p1, DxfPoint p2, double range) =>
       Math.Abs(p1.X - p2.X) < range && Math.Abs(p1.Y - p2.Y) < range;

    private static bool areBoundingBoxesWithinRange(DxfBoundingBox? b1, DxfBoundingBox? b2, double range)
    {
        if (b1 is null || b2 is null) return false;
        return arePointsWithinRange(b1.Value.MaximumPoint, b2.Value.MaximumPoint, range)
                    && arePointsWithinRange(b1.Value.MinimumPoint, b2.Value.MinimumPoint, range);
    }
}