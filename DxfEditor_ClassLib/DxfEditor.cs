using IxMilia.Dxf.Entities;

namespace DxfEditor_ClassLib;

public class DxfEditor
{
    private string _pathNew;
    private string? _pathOriginal;

    public DxfEditor(string pathNew, string? pathOriginal = null)
    {
        _pathNew = pathNew;
        _pathOriginal = pathOriginal;
    }

    public int OffsetCirlces(Dictionary<double, double>? offsetMap, bool offsetAll = true, double offsetDiameter = 0)
    {
        int diametersChanged = 0;
        var dxfFile = DxfUtils.LoadDxfFile(_pathOriginal);

        var circles = dxfFile.Entities.OfType<DxfCircle>();
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

        if (diametersChanged > 0) dxfFile.Save(_pathNew);
        return diametersChanged;
    }

    public (int, int) DeleteSplinesAndDuplicates(double range)
    {
        var dxfFile = DxfUtils.LoadDxfFile(_pathOriginal);
        int deletedSplines = DxfUtils.DeleteSplines(dxfFile);
        int deletedDuplicates = 0;

        var entities = dxfFile.Entities.ToList();
        for (int i = 0; i < entities.Count; i++)
        {
            for (int k = i + 1; k < entities.Count; k++)
            {
                if (DxfUtils.AreDuplicates((entities[i], entities[k]), range))
                {
                    dxfFile.Entities.Remove(entities[k]);
                    deletedDuplicates++;
                }
            }
        }

        dxfFile.Save(_pathNew);
        return (deletedSplines, deletedDuplicates);
    }

    public void CreateMesh()
    {

    }
}