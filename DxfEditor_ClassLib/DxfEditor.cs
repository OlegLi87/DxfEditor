using IxMilia.Dxf;
using IxMilia.Dxf.Entities;

namespace DxfEditor_ClassLib;

public static class DxfEditor
{
    public static int OffsetCirlces(string pathOriginal, string pathNew,
                                     Dictionary<double, double>? offsetMap, bool offsetAll = true, double offsetDiameter = 0)
    {
        int diametersChanged = 0;
        var dxfFile = loadDxfFile(pathOriginal);

        var circles = dxfFile.Entities.Where(e => e is DxfCircle).Select(i => (DxfCircle)i);
        if (circles is null) return 0;

        foreach (var circle in circles)
        {

            if (offsetAll)
            {
                circle.Radius += offsetDiameter / 2;
                diametersChanged++;
            }
            else
            {
                if (offsetMap is null) return 0;

                double circleDiameter = Math.Round(circle.Radius * 2, 2);
                if (offsetMap.ContainsKey(circleDiameter))
                {
                    circle.Radius += offsetMap[circleDiameter] / 2;
                    diametersChanged++;
                }
            }
        }

        if (diametersChanged > 0) dxfFile.Save(pathNew);

        return diametersChanged;
    }

    public static (int, int) DeleteSplinesAndDuplicates(string pathOriginal, string pathNew, double range)
    {
        var dxfFile = loadDxfFile(pathOriginal);
        int deletedSplines = deleteSplines(dxfFile);
        int deletedDuplicates = 0;

        var entities = dxfFile.Entities.ToList();
        for (int i = 0; i < entities.Count; i++)
        {
            for (int k = i + 1; k < entities.Count; k++)
            {
                if (areDuplicates((entities[i], entities[k]), range))
                {
                    dxfFile.Entities.Remove(entities[k]);
                    deletedDuplicates++;
                }
            }
        }

        dxfFile.Save(pathNew);
        return (deletedSplines, deletedDuplicates);
    }

    public static void DrawMesh()
    {

    }

    private static int deleteSplines(DxfFile dxfFile)
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

    private static bool areDuplicates((DxfEntity, DxfEntity) entities, double range) =>
         entities switch
         {
             (DxfLine l1, DxfLine l2) => arePointsWithinRange(l1.P1, l2.P1, range) && arePointsWithinRange(l1.P2, l2.P2, range)
                                       || arePointsWithinRange(l1.P2, l2.P1, range) && arePointsWithinRange(l1.P1, l2.P2, range),
             (DxfArc a1, DxfArc a2) => Math.Abs(a1.Radius - a2.Radius) < range && areBoundingBoxesWithinRange(a1.GetBoundingBox(), a2.GetBoundingBox(), range),
             (DxfCircle c1, DxfCircle c2) => Math.Abs(c1.Radius - c2.Radius) < range && arePointsWithinRange(c1.Center, c2.Center, range),
             _ => false
         };

    private static bool arePointsWithinRange(DxfPoint p1, DxfPoint p2, double range) =>
       Math.Abs(p1.X - p2.X) < range && Math.Abs(p1.Y - p2.Y) < range;

    private static bool areBoundingBoxesWithinRange(DxfBoundingBox? b1, DxfBoundingBox? b2, double range)
    {
        if (b1 is null || b2 is null) return false;
        return arePointsWithinRange(b1.Value.MaximumPoint, b2.Value.MaximumPoint, range)
                    && arePointsWithinRange(b1.Value.MinimumPoint, b2.Value.MinimumPoint, range);
    }


    private static DxfFile loadDxfFile(string path)
    {
        if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path value is empty or missing");

        DxfFile? dxfFile = DxfFile.Load(path);
        if (dxfFile is null) throw new ArgumentException("Could not load dxf file");

        return dxfFile;
    }
}