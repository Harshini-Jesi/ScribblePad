﻿using Bound = CADye.Lib.Bound;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;

namespace CADye;
static class Util {
   public static Matrix ComputeZoomExtentsProjXfm (double viewWidth, double viewHeight, Bound b) {
      var viewMargin = 30;
      // Compute the scaling, to fit specified drawing extents into the view space
      double scaleX = (viewWidth - 2 * viewMargin) / b.Width, scaleY = (viewHeight - 2 * viewMargin) / b.Height;
      double scale = Math.Min (scaleX, scaleY);
      var scaleMatrix = Matrix.Identity; scaleMatrix.Scale (scale, -scale);
      // translation...
      Point projectedMidPt = scaleMatrix.Transform (new Point (b.Mid.X, b.Mid.Y));
      Point viewMidPt = new (viewWidth / 2, viewHeight / 2);
      var translateMatrix = Matrix.Identity; translateMatrix.Translate (viewMidPt.X - projectedMidPt.X, viewMidPt.Y - projectedMidPt.Y);
      // Final zoom extents matrix, is a product of scale and translate matrices
      scaleMatrix.Append (translateMatrix);
      return scaleMatrix;
   }
}

class PanWidget { // Works in screen space
   #region Constructors ---------------------------------------------
   public PanWidget (UIElement eventSource, Action<Vector> onPan) {
      mOnPan = onPan;
      eventSource.MouseDown += (sender, e) => {
         if (e.ChangedButton == MouseButton.Middle) PanStart (e.GetPosition (eventSource));
      };
      eventSource.MouseUp += (sender, e) => {
         if (IsPanning) PanEnd (e.GetPosition (eventSource));
      };
      eventSource.MouseMove += (sender, e) => {
         if (IsPanning) PanMove (e.GetPosition (eventSource));
      };
      eventSource.MouseLeave += (sender, e) => {
         if (IsPanning) PanCancel ();
      };
   }
   #endregion

   #region Implementation -------------------------------------------
   bool IsPanning => mPrevPt != null;

   void PanStart (Point pt) => mPrevPt = pt;

   void PanMove (Point pt) {
      mOnPan.Invoke (pt - mPrevPt!.Value);
      mPrevPt = pt;
   }

   void PanEnd (Point? pt) {
      if (pt.HasValue) PanMove (pt.Value);
      mPrevPt = null;
   }

   void PanCancel () => PanEnd (null);
   #endregion

   #region Private --------------------------------------------------
   Point? mPrevPt;
   readonly Action<Vector> mOnPan;
   #endregion
}