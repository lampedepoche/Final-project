using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ShapeType
{
    Ellipse,
    Rect,
    Star,
}

public class Shape
{
    public Color StrokeColor;
    public float StrokeThickness;
    public float X;
    public float Y;
    public float Width;
    public float Height;
    public Color FillColor;
    public bool Filled;
    public ShapeType Type;

    public static Shape TestSquare(float x, float y)
    {
        return new Shape()
        {
            X = x,
            Y = y,
            Width = 100,
            Height = 100,
            StrokeColor = Color.Blue,
            FillColor = Color.Red,
            Filled = true,
            StrokeThickness = 10,
            Type = ShapeType.Rect
        };
    }
}
