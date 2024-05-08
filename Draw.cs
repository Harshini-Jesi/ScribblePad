using CADye.Lib;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CADye;
public class Draw : IDrawable {
   public Draw (Pen pen, DrawingContext dwgCntxt,Matrix xfm) =>(mPen,mDwg,mXfm)=(pen,dwgCntxt,xfm);

   public void DrawCircle (List<Point> points) {
      mStart.X = points[0].X; mStart.Y = points[0].Y;
      mEnd.X = points[^1].X; mEnd.Y = points[^1].Y;
      double x = mEnd.X - mStart.X;
      double y = mEnd.Y - mStart.Y;
      double radius = Math.Sqrt ((x * x) + (y * y));
      mDwg.DrawEllipse (null, mPen, mXfm.Transform (mStart), radius, radius);
   }

   public void DrawConLine (List<Point> points) {
      for (int i = 0; i < points.Count - 1; i++) {
         mStart.X = points[i].X; mStart.Y = points[i].Y;
         mEnd.X = points[i + 1].X; mEnd.Y = points[i + 1].Y;
         mDwg.DrawLine (mPen, mXfm.Transform (mStart), mXfm.Transform (mEnd));
      }
   }

   public void DrawLine (List<Point> points) {
      mStart.X = points[0].X; mStart.Y = points[0].Y;
      mEnd.X = points[^1].X; mEnd.Y = points[^1].Y;
      mDwg.DrawLine (mPen, mXfm.Transform(mStart), mXfm.Transform(mEnd));
   }

   public void DrawRectangle (List<Point> points) {
      mStart.X = points[0].X; mStart.Y = points[0].Y;
      mEnd.X = points[^1].X; mEnd.Y = points[^1].Y;
      mDwg.DrawRectangle (null, mPen, new (mXfm.Transform (mStart), mXfm.Transform (mEnd)));
   }

   DrawingContext mDwg;
   Pen mPen;
   Matrix mXfm;
   System.Windows.Point mStart, mEnd;
}