namespace CADye.Lib {
   #region struct Point ---------------------------------------------------------------------------
   public struct Point {
      public Point (double x, double y) => (X, Y) = (x, y);

      public double X, Y;
   };
   #endregion

   #region struct Bound ---------------------------------------------------------------------------
   readonly public struct Bound { // Bound in drawing space
      #region Constructors ------------------------------------------
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

      #region Properties --------------------------------------------
      public double MinX { get; init; }
      public double MaxX { get; init; }
      public double MinY { get; init; }
      public double MaxY { get; init; }
      public double Width => MaxX - MinX;
      public double Height => MaxY - MinY;
      public Point Mid => new ((MaxX + MinX) / 2, (MaxY + MinY) / 2);
      public bool IsEmpty => MinX > MaxX || MinY > MaxY;
      #endregion

      #region Methods -----------------------------------------------
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

   #region class Pline ----------------------------------------------------------------------------
   public class Pline {
      #region Constructors ------------------------------------------
      public Pline () { }

      public Pline (IEnumerable<Point> pts) => (mPoints, Bound) = (pts.ToList (), new Bound (pts));
      #endregion

      #region Properties --------------------------------------------
      public Bound Bound { get; }

      public IEnumerable<Point> GetPoints () => mPoints;
      #endregion

      #region Methods -----------------------------------------------
      public static Pline CreateLine (Point startPt, Point endPt) {
         return new Pline (Enum (startPt, endPt));

         static IEnumerable<Point> Enum (Point a, Point b) {
            yield return a;
            yield return b;
         }
      }

      public static Pline CreateRectangle (Point startCornerPt, Point endCornerPt) {
         return new Pline (Enum (startCornerPt, endCornerPt));

         static IEnumerable<Point> Enum (Point a, Point b) {
            Point c = new (a.X, b.Y), d = new (b.X, a.Y);
            yield return a;
            yield return d;
            yield return b;
            yield return c;
            yield return a;
         }
      }

      public static Pline CreateConnectedLine (List<Point> points) {
         return new Pline (Enum (points));

         static IEnumerable<Point> Enum (List<Point> points) {
            for (int i = 0; i < points.Count; i++) yield return points[i];
         }
      }

      public void Load (BinaryReader br) {
         int pointsCount = br.ReadInt32 ();
         for (int j = 0; j < pointsCount; j++) {
            double x = br.ReadDouble ();
            double y = br.ReadDouble ();
            mPoints.Add (new Point (x, y));
         }
      }

      public void Save (BinaryWriter bw) {
         bw.Write (mPoints.Count);
         foreach (Point pt in mPoints) {
            bw.Write (pt.X);
            bw.Write (pt.Y);
         }
      }
      #endregion

      #region Private -----------------------------------------------
      readonly List<Point> mPoints = new ();
      #endregion
   }
   #endregion
}