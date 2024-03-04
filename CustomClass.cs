using System.Windows.Media;

namespace BasicScrawls {
   public class Shapes {
      public PointCollection PointList = new ();
   }

   public class Scrawls : Shapes { }

   public class Line : Shapes { }

   public class Rect : Shapes { }

   public class Circle : Shapes { }

   public class ConnectedLine : Shapes { }
}