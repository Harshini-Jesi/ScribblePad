namespace BackEnd {
   #region struct Point ---------------------------------------------------------------------------
   public struct Point {
      public double X, Y;
      public Point (double x, double y) { X = x; Y = y; }
   };
   #endregion

   #region class Shapes ---------------------------------------------------------------------------
   public class Shapes {
      public int BinId { get; set; }
      public List<Point> pointList = new ();

      #region Methods -----------------------------------------------
      public virtual void Load (BinaryReader br) {
         int pointsCount = br.ReadInt32 ();
         for (int j = 0; j < pointsCount; j++) {
            double x = br.ReadDouble ();
            double y = br.ReadDouble ();
            pointList.Add (new Point (x, y));
         }
      }

      public virtual void Save (BinaryWriter bw) {
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

   #region class Scrawl ---------------------------------------------------------------------------
   public class Scrawls : Shapes {
      public Scrawls () => BinId = 1;

      public override void UpdateEndPoint (Point pt) => pointList.Add (pt);
   }
   #endregion

   #region class Line -----------------------------------------------------------------------------
   public class Line : Shapes {
      public Line () => BinId = 2;
   }
   #endregion

   #region class Rectangle ------------------------------------------------------------------------
   public class Rectangle : Shapes {
      public Rectangle () => BinId = 3;
   }
   #endregion

   #region class Circle ---------------------------------------------------------------------------
   public class Circle : Shapes {
      public Circle () => BinId = 4;
   }
   #endregion

   #region class ConnectedLine --------------------------------------------------------------------
   public class ConnectedLine : Shapes {
      public ConnectedLine () => BinId = 5;
   }
   #endregion
}