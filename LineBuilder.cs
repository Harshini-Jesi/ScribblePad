using CADye.Lib;
using System;
using System.Windows.Media;

namespace CADye;
public class LineBuilder : Widget {
   #region Constructor ----------------------------------------------
   public LineBuilder (Editor canvas) : base (canvas) => ShowPrompt ();
   #endregion

   #region Properties -----------------------------------------------
   public double X { get => mX; set { mX = value; OnPropertyChanged (nameof (X)); } }

   public double Y { get => mY; set { mY = value; OnPropertyChanged (nameof (Y)); } }

   public double Dx { get => mDx; set { mDx = value; OnPropertyChanged (nameof (Dx)); } }

   public double Dy { get => mDy; set { mDy = value; OnPropertyChanged (nameof (Dy)); } }

   public double Length { get => mLength; set { mLength = value; OnPropertyChanged (nameof (Length)); } }

   public double Angle { get => mAngle; set { mAngle = value; OnPropertyChanged (nameof (Angle)); } }
   #endregion

   #region Overrides ------------------------------------------------
   public override string[] Labels => sLabels;

   public override Pline? PointClicked (Point drawingPt) {
      ShowPrompt ();
      if (mFirstPt is null) {
         (mFirstPt, X, Y) = (drawingPt, drawingPt.X, drawingPt.Y);
         return null;
      } else {
         var firstPt = mFirstPt.Value;
         mFirstPt = null;
         ShowPrompt ();
         (Dx, Dy) = (drawingPt.X - firstPt.X, drawingPt.Y - firstPt.Y);
         Length = Math.Sqrt (Dx * Dx + Dy * Dy);
         Angle = Math.Atan2 (Dy, Dx) * (180 / Math.PI);
         return Pline.CreateLine (firstPt, drawingPt);
      }
   }

   public override void PointHover (Point drawingPt) {
      ShowPrompt ();
      mHoverPt = drawingPt;
   }

   public override void ShowPrompt () {
      if (mEditor.Window != null)
         mEditor.Window.Prompt.Text = mFirstPt == null ?
            "Line: Pick beginning point" : "Line: Pick end point";
   }

   public override void StopDrawing () => mFirstPt = null;

   #region DrawableBase implementation ------------------------------
   public override void Draw (DrawingCommands drawingCommands) {
      if (mFirstPt == null || mHoverPt == null) return;
      drawingCommands.Brush = Brushes.Red;
      drawingCommands.DrawLine (mFirstPt.Value, mHoverPt.Value);
   }
   #endregion
   #endregion

   #region Private --------------------------------------------------
   static string[] sLabels = { "X", "Y", "Dx", "Dy", "Length", "Angle" };
   double mX, mY, mDx, mDy, mLength, mAngle;
   Point? mFirstPt, mHoverPt;
   #endregion
}