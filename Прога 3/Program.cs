
using System;
using System.IO;
using Newtonsoft.Json;

class Point
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}

class Triangle
{
    public Point A { get; set; }
    public Point B { get; set; }
    public Point C { get; set; }

    public Triangle(Point a, Point b, Point c)
    {
        A = a;
        B = b;
        C = c;
    }

    private double Distance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }

    public double Perimeter()
    {
        return Distance(A, B) + Distance(B, C) + Distance(C, A);
    }

    public double Area()
    {
        double s = Perimeter() / 2;
        return Math.Sqrt(s * (s - Distance(A, B)) * (s - Distance(B, C)) * (s - Distance(C, A)));
    }

    public double Height(double side)
    {
        return (2 * Area()) / side;
    }

    public double Median(double a, double b, double c)
    {
        return 0.5 * Math.Sqrt(2 * b * b + 2 * c * c - a * a);
    }

    public double Bisektrisa(double a, double b, double c)
    {
        return (2 * b * c) / (b + c) * Math.Sqrt(1 - Math.Pow((b * b + c * c - a * a) / (2 * b * c), 2));
    }

    public double InscribedCircleRadius()
    {
        return Area() / (Perimeter() / 2);
    }

    public double CircumscribedCircleRadius()
    {
        return (Distance(A, B) * Distance(B, C) * Distance(C, A)) / (4 * Area());
    }

    public string TriangleType()
    {
        double a = Distance(A, B);
        double b = Distance(B, C);
        double c = Distance(C, A);

        if (Math.Abs(a - b) < 1e-6 && Math.Abs(b - c) < 1e-6)
            return "Рівносторонній";

        if (Math.Abs(a - b) < 1e-6 || Math.Abs(b - c) < 1e-6 || Math.Abs(a - c) < 1e-6)
            return "Рівнобедрений";

        double[] sides = { a, b, c };
        Array.Sort(sides);
        double x = sides[0], y = sides[1], z = sides[2];

        if (Math.Abs(x * x + y * y - z * z) < 1e-6)
            return "Прямокутний";

        if (x * x + y * y > z * z)
            return "Гострокутний";

        return "Тупокутний";
    }
    public void Rotate(double angleDegrees, Point center)
    {
        double angleRadians = angleDegrees * Math.PI / 180;
        A = RotatePoint(A, center, angleRadians);
        B = RotatePoint(B, center, angleRadians);
        C = RotatePoint(C, center, angleRadians);
    }

    private Point RotatePoint(Point p, Point center, double angle)
    {
        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);
        double newX = center.X + (p.X - center.X) * cos - (p.Y - center.Y) * sin;
        double newY = center.Y + (p.X - center.X) * sin + (p.Y - center.Y) * cos;
        return new Point(newX, newY);
    }

    public Point CircumCenter()
    {
        double D = 2 * (A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y));
        double Xc = ((Math.Pow(Distance(A, B), 2) * (B.Y - C.Y)) +
                     (Math.Pow(Distance(B, C), 2) * (C.Y - A.Y)) +
                     (Math.Pow(Distance(C, A), 2) * (A.Y - B.Y))) / D;
        double Yc = ((Math.Pow(Distance(A, B), 2) * (C.X - B.X)) +
                     (Math.Pow(Distance(B, C), 2) * (A.X - C.X)) +
                     (Math.Pow(Distance(C, A), 2) * (B.X - A.X))) / D;
        return new Point(Xc, Yc);
    }

    public void SaveToJson(string filename)
    {
        File.WriteAllText(filename, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public static Triangle LoadFromJson(string filename)
    {
        return JsonConvert.DeserializeObject<Triangle>(File.ReadAllText(filename));
    }

    static void Main()
    {
        Triangle t1 = new Triangle(new Point(0, 0), new Point(4, 0), new Point(0, 3));

        Console.WriteLine($"Периметр: {t1.Perimeter():F2}");
        Console.WriteLine($"Площа: {t1.Area():F2}");
        Console.WriteLine($"Вписане коло: {t1.InscribedCircleRadius():F2}");
        Console.WriteLine($"Описане коло: {t1.CircumscribedCircleRadius():F2}");
        Console.WriteLine($"Tип трикутника: {t1.TriangleType()}");
        Console.WriteLine("До повороту:");
        Console.WriteLine($"A: ({t1.A.X}, {t1.A.Y}), B: ({t1.B.X}, {t1.B.Y}), C: ({t1.C.X}, {t1.C.Y})");

        t1.Rotate(30, t1.A);
        Console.WriteLine("Після повороту навколо A на 30 градусів:");
        Console.WriteLine($"A: ({t1.A.X:F2}, {t1.A.Y:F2}), B: ({t1.B.X:F2}, {t1.B.Y:F2}), C: ({t1.C.X:F2}, {t1.C.Y:F2})");

        Point circumcenter = t1.CircumCenter();
        t1.Rotate(45, circumcenter);
        Console.WriteLine("Після повороту навколо центру описаного кола на 45 градусів:");
        Console.WriteLine($"A: ({t1.A.X:F2}, {t1.A.Y:F2}), B: ({t1.B.X:F2}, {t1.B.Y:F2}), C: ({t1.C.X:F2}, {t1.C.Y:F2})");

        t1.SaveToJson("triangle.json");

        string json = File.ReadAllText("triangle.json");
        Triangle t2 = JsonConvert.DeserializeObject<Triangle>(json);
        if (t2 != null)
        {
            Console.WriteLine("\nДані після зчитування з JSON:");
            Console.WriteLine($"Периметр: {t2.Perimeter():F2}");
            Console.WriteLine($"Площа: {t2.Area():F2}");
            Console.WriteLine($"Вписане коло: {t2.InscribedCircleRadius():F2}");
            Console.WriteLine($"Описане коло: {t2.CircumscribedCircleRadius():F2}");
            Console.WriteLine($"Зчитаний тип трикутника: {t2.TriangleType()}");
            t2.Rotate(30, t2.A);
            Console.WriteLine("Зчитане після повороту навколо A на 30 градусів:");
            Console.WriteLine($"A: ({t2.A.X}, {t2.A.Y}), B: ({t2.B.X}, {t2.B.Y}), C: ({t2.C.X}, {t2.C.Y})");
            Point circumCenter = t2.CircumCenter();
            t2.Rotate(45, circumCenter);
            Console.WriteLine("Зчитане після повороту навколо центру описаного кола на 45 градусів:");
            Console.WriteLine($"A: ({t2.A.X}, {t2.A.Y}), B: ({t2.B.X}, {t2.B.Y}), C: ({t2.C.X}, {t2.C.Y})");
        }
    }
}
