namespace CADye.lib {
   #region struct Point ---------------------------------------------------------------------------
   public struct Point {
      public double X, Y;
      public Point (double x, double y) { X = x; Y = y; }
   };
   #endregion

   #region class Shapes ---------------------------------------------------------------------------
   public partial class Shapes {
      public int BinId { get; set; }
      public List<Point> pointList = new ();

      #region Methods -----------------------------------------------
      public void Load (BinaryReader br) {
         int pointsCount = br.ReadInt32 ();
         for (int j = 0; j < pointsCount; j++) {
            double x = br.ReadDouble ();
            double y = br.ReadDouble ();
            pointList.Add (new Point (x, y));
         }
      }

      public void Save (BinaryWriter bw) {
         bw.Write (BinId);
         bw.Write (pointList.Count);
         foreach (Point pt in pointList) {
            bw.Write (pt.X);
            bw.Write (pt.Y);
         }
      }

      public virtual void UpdateEndPoint (Point pt) => pointList[^1] = pt;
      #endregion
   }
   #endregion

   #region class Line -----------------------------------------------------------------------------
   public partial class Line : Shapes {
      public Line () => BinId = 2;
   }
   #endregion

   #region class Rectangle ------------------------------------------------------------------------
   public partial class Rectangle : Shapes {
      public Rectangle () => BinId = 3;
   }
   #endregion

   #region class Circle ---------------------------------------------------------------------------
   public partial class Circle : Shapes {
      public Circle () => BinId = 4;
   }
   #endregion

   #region class ConnectedLine --------------------------------------------------------------------
   public partial class ConnectedLine : Shapes {
      public ConnectedLine () => BinId = 5;
   }
   #endregion
}