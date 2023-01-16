namespace DxfEditor_ClassLib;

public readonly struct Rectangle
{
    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    internal double Width { get; }
    internal double Height { get; }

    internal bool IsAbleToContain(Rectangle rect) =>
      (Width >= rect.Width && Height >= rect.Height) || (Width >= rect.Height && Height >= rect.Width);

    internal InsertionDirection GetOptimalInsertionDirection(Rectangle rect)
    {
        if ((int)(Width / rect.Height) * (int)(Height / rect.Width)
                      > (int)(Height / rect.Height) * (int)(Width / rect.Width)) return InsertionDirection.HeightAlongWidth;

        return InsertionDirection.WidthAlongWidth;
    }

}