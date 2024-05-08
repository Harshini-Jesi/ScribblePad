namespace CADye.Lib {
   #region struct Point ---------------------------------------------------------------------------
   public struct Point {
      public double X, Y;
      public Point (double x, double y) { X = x; Y = y; }
   };
   #endregion

   #region struct Bound ---------------------------------------------------------------------------
   readonly public struct Bound { // Bound in drawing space
      #region Constructors
      public Bound (Point cornerA, Point cornerB) {
         MinX = Math.Min (cornerA.X, cornerB.X);
         MaxX = Math.Max (cornerA.X, cornerB.X);
         MinY = Math.Min (cornerA.Y, cornerB.Y);
         MaxY = Math.Max (cornerA.Y, cornerB.Y);
      }

      public Bound (IEnumerable<Point> pts) {
         MinX = pts.Min (p => p.X);
         MaxX = pts.Max (p => p.X);
         MinY = pts.Min (p => p.Y);
         MaxY = pts.Max (p => p.Y);
      }

      public Bound (IEnumerable<Bound> bounds) {
         MinX = bounds.Min (b => b.MinX);
         MaxX = bounds.Max (b => b.MaxX);
         MinY = bounds.Min (b => b.MinY);
         MaxY = bounds.Max (b => b.MaxY);
      }

      public Bound () {
         this = Empty;
      }

      public static readonly Bound Empty = new () { MinX = double.MaxValue, MinY = double.MaxValue, MaxX = double.MinValue, MaxY = double.MinValue };
      #endregion

      #region Properties
      public double MinX { get; init; }
      public double MaxX { get; init; }
      public double MinY { get; init; }
      public double MaxY { get; init; }
      public double Width => MaxX - MinX;
      public double Height => MaxY - MinY;
      public Point Mid => new ((MaxX + MinX) / 2, (MaxY + MinY) / 2);
      public bool IsEmpty => MinX > MaxX || MinY > MaxY;
      #endregion

      #region Methods
      public Bound Inflated (Point ptAt, double factor) {
         if (IsEmpty) return this;
         var minX = ptAt.X - (ptAt.X - MinX) * factor;
         var maxX = ptAt.X + (MaxX - ptAt.X) * factor;
         var minY = ptAt.Y - (ptAt.Y - MinY) * factor;
         var maxY = ptAt.Y + (MaxY - ptAt.Y) * factor;
         return new () { MinX = minX, MaxX = maxX, MinY = minY, MaxY = maxY };
      }
      #endregion
   }
   #endregion

   #region interface IDrawable --------------------------------------------------------------------
   public interface IDrawable {
      void DrawCircle (List<Point> Points);
      void DrawConLine (List<Point> Points);
      void DrawLine (List<Point> Points);
      void DrawRectangle (List<Point> Points);
   }
   #endregion

   #region class DrawingSheet ---------------------------------------------------------------------
   public class DrawingSheet {
      public void AddShape (Shape shape) {
         Shapes.Add (shape);
         shape.Bound = new Bound (shape.Points);
         Bound = new Bound (Shapes.Select (shape => shape.Bound));
      }

      public Bound Bound { get; private set; }
      public List<Shape> Shapes = new ();
   }
   #endregion

   #region class Shape ----------------------------------------------------------------------------
   public abstract class Shape {
      public int ShapeId { get; set; }
      public Bound Bound { get; set; }
      public List<Point> Points { get => mPoints; set => mPoints = value; }
      List<Point> mPoints = new ();

      #region Methods -----------------------------------------------
      public abstract void Draw (IDrawable drawing);

      public void Load (BinaryReader br) {
         int pointsCount = br.ReadInt32 ();
         for (int j = 0; j < pointsCount; j++) {
            double x = br.ReadDouble ();
            double y = br.ReadDouble ();
            Points.Add (new Point (x, y));
         }
      }

      public void Save (BinaryWriter bw) {
         bw.Write (ShapeId);
         bw.Write (Points.Count);
         foreach (Point pt in Points) {
            bw.Write (pt.X);
            bw.Write (pt.Y);
         }
      }

      public void UpdateEndPoint (Point pt) => Points[^1] = pt;
      #endregion
   }
   #endregion

   #region class Line -----------------------------------------------------------------------------
   public class Line : Shape {
      public Line () => ShapeId = 1;

      public override void Draw (IDrawable drawing) {
         drawing.DrawLine (Points);
      }
   }
   #endregion

   #region class Rectangle ------------------------------------------------------------------------
   public class Rectangle : Shape {
      public Rectangle () => ShapeId = 2;

      public override void Draw (IDrawable drawing) {
         drawing.DrawRectangle (Points);
      }
   }
   #endregion

   #region class Circle ---------------------------------------------------------------------------
   public class Circle : Shape {
      public Circle () => ShapeId = 3;

      public override void Draw (IDrawable drawing) {
         drawing.DrawCircle (Points);
      }
   }
   #endregion

   #region class ConnectedLine --------------------------------------------------------------------
   public class ConnectedLine : Shape {
      public ConnectedLine () => ShapeId = 4;

      public override void Draw (IDrawable drawing) {
         drawing.DrawConLine (Points);
      }
   }
   #endregion
}