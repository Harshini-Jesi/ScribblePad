using CADye.lib;
using System;
using System.Windows.Media;

namespace CADye;
public class Drawing {
   Pen mPen = new (Brushes.White, 2);

   public void Draw (DrawingContext dc, Line line) {
      System.Windows.Point start, end;
      start.X = line.pointList[0].X; start.Y = line.pointList[0].Y;
      end.X = line.pointList[^1].X; end.Y = line.pointList[^1].Y;
      dc.DrawLine (mPen, start, end);
   }

   public void Draw (DrawingContext dc, Rectangle rect) {
      System.Windows.Point start, end;
      start.X = rect.pointList[0].X; start.Y = rect.pointList[0].Y;
      end.X = rect.pointList[^1].X; end.Y = rect.pointList[^1].Y;
      dc.DrawRectangle (null, mPen, new (start, end));
   }

   public void Draw (DrawingContext dc, Circle circle) {
      System.Windows.Point start, end;
      start.X = circle.pointList[0].X; start.Y = circle.pointList[0].Y;
      end.X = circle.pointList[^1].X; end.Y = circle.pointList[^1].Y;
      double x = end.X - start.X;
      double y = end.Y - start.Y;
      double radius = Math.Sqrt ((x * x) + (y * y));
      dc.DrawEllipse (null, mPen, start, radius, radius);
   }
}