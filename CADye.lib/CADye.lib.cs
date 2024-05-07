namespace CADye.Lib {
   #region struct Point ---------------------------------------------------------------------------
   public struct Point {
      public double X, Y;
      public Point (double x, double y) { X = x; Y = y; }
   };
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
      public List<Shape> Shapes = new ();
   }
   #endregion

   #region class Shape ----------------------------------------------------------------------------
   public abstract class Shape {
      public int ShapeId { get; set; }

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