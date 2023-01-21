using IxMilia.Dxf;
using IxMilia.Dxf.Entities;

namespace DxfEditor_ClassLib;

public class DxfEditor
{
    private string _pathNew;
    private string? _pathOriginal;

    public DxfEditor(string pathNew, string? pathOriginal = null)
    {
        ArgumentNullException.ThrowIfNull(pathNew);
        _pathNew = pathNew;
        _pathOriginal = pathOriginal;
    }

    public int OffsetCirlces(Dictionary<double, double>? offsetMap, bool offsetAll = true, double? offsetDiameter = null)
    {
        int diametersChanged = 0;
        var dxfFile = DxfUtils.LoadDxfFile(_pathOriginal);

        var circles = dxfFile.Entities.Where(e => e is DxfCircle && e is not DxfArc).Cast<DxfCircle>();
        if (circles is null) return 0;

        foreach (var circle in circles)
        {

            if (offsetAll)
            {
                ArgumentNullException.ThrowIfNull(offsetDiameter);
                circle.Radius += offsetDiameter.Value / 2;
                diametersChanged++;
            }
            else
            {
                ArgumentNullException.ThrowIfNull(offsetMap);

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

    public void CreateMeshes(IEnumerable<MeshData> meshDatas)
    {
        var dxfFile = new DxfFile();
        var origin = new DxfPoint(0, 0, 0);

        foreach (var meshData in meshDatas)
        {
            var currentData = meshData;
            while (currentData.SheetSize.IsAbleToContain(currentData.ItemSize))
            {
                (List<DxfLine> hLines, List<DxfLine> vLines) lines = DxfUtils.CreateLines(currentData);
                createMesh(lines.hLines.Concat(lines.vLines), dxfFile);

                currentData = DxfUtils.CreateAnotherMeshData(currentData, lines);
                if (currentData is null) break;
            }
        }
    }

    private void createMesh(IEnumerable<DxfLine> lines, DxfFile dxfFile)
    {
        foreach (var line in lines)
            dxfFile.Entities.Add(line);

        dxfFile.Save(_pathNew);
    }
}