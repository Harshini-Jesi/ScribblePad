using CADye.Lib;
using System;
using System.Windows.Media;

namespace CADye;
public class ConLineBuilder : Widget {
   #region Constructor ----------------------------------------------
   public ConLineBuilder (Editor editor) : base (editor) => ShowPrompt ();
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
      if (mFirstPt != null) {
         var firstPt = mFirstPt.Value;
         (Dx, Dy) = (drawingPt.X - firstPt.X, drawingPt.Y - firstPt.Y);
         Length = Math.Sqrt (Dx * Dx + Dy * Dy);
         Angle = Math.Atan2 (Dy, Dx) * (180 / Math.PI);
      }
      mPoints.Add (drawingPt);
      (mFirstPt, X, Y) = (drawingPt, drawingPt.X, drawingPt.Y);
      return null;
   }

   public override void PointHover (Point drawingPt) {
      ShowPrompt ();
      mHoverPt = drawingPt;
   }

   public override void ShowPrompt () {
      if (mEditor.Window != null)
         mEditor.Window.Prompt.Text = mFirstPt == null ?
            "ConnectedLine: Pick beginning point" : "ConnectedLine: Pick end point";
   }

   public override void StopDrawing () {
      mEditor.Dwg.AddPline (Pline.CreateConnectedLine (mPoints));
      mPoints.Clear (); mFirstPt = null;
   }

   #region DrawableBase implementation ------------------------------
   public override void Draw (DrawingCommands drawingCommands) {
      if (mFirstPt == null || mHoverPt == null) return;
      drawingCommands.Brush = Brushes.Red;
      drawingCommands.DrawLine (mFirstPt.Value, mHoverPt.Value);
      drawingCommands.Brush = Brushes.Black;
      drawingCommands.DrawLines (mPoints);
   }
   #endregion
   #endregion

   #region Private --------------------------------------------------
   static string[] sLabels = { "X", "Y", "Dx", "Dy", "Length", "Angle" };
   double mX, mY, mDx, mDy, mLength, mAngle;
   Point? mFirstPt, mHoverPt;
   #endregion
}
