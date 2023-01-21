using System.Data;
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

        var circles = dxfFile.Entities.Where(e => e.GetType() == typeof(DxfCircle)).Cast<DxfCircle>();
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
        int offsetBetweenMeshes = 50;

        foreach (var meshData in meshDatas)
        {
            if (meshData.SheetSize.IsAbleToContain(meshData.ItemSize) && meshData.Amount > 0)
            {
                (IEnumerable<DxfLine> hLines, IEnumerable<DxfLine> vLines) lines;

                meshData.Origin = origin;
                DxfUtils.PopulateWithLines(meshData);

                var additionalMeshData = DxfUtils.CreateAnotherMeshData(meshData);
                if (additionalMeshData is not null)
                {
                    DxfUtils.PopulateWithLines(additionalMeshData);
                    lines = meshData + additionalMeshData;
                }
                else lines = (meshData.HorizontalLines, meshData.VerticalLines);

                string text = meshData.ItemSize.Width + " X " + meshData.ItemSize.Height;
                createMesh(lines.hLines.Concat(lines.vLines), text, dxfFile);
                origin = new DxfPoint(lines.hLines.Max(l => l.P2.X) + offsetBetweenMeshes, 0, 0);
            }
        }
    }

    private void createMesh(IEnumerable<DxfLine> lines, string sizeText, DxfFile dxfFile)
    {
        double maxY = lines.Max(l => l.P2.Y);
        double minX = lines.Min(l => l.P2.X);
        var text = new DxfText(new DxfPoint(minX, maxY + 10, 0), 15, sizeText);

        dxfFile.Entities.Add(text);

        foreach (var line in lines)
            dxfFile.Entities.Add(line);

        dxfFile.Save(_pathNew);
    }
}