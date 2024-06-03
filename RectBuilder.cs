using CADye.Lib;
using System.Windows.Media;

namespace CADye;
public class RectBuilder : Widget {
   #region Constructor ----------------------------------------------
   public RectBuilder (Editor editor) : base (editor) => ShowPrompt ();
   #endregion

   #region Properties -----------------------------------------------
   public double X { get => mX; set { mX = value; OnPropertyChanged (nameof (X)); } }

   public double Y { get => mY; set { mY = value; OnPropertyChanged (nameof (Y)); } }

   public double Width { get => mWidth; set { mWidth = value; OnPropertyChanged (nameof (Width)); } }

   public double Height { get => mHeight; set { mHeight = value; OnPropertyChanged (nameof (Height)); } }
   #endregion

   #region Overrides ------------------------------------------------
   public override string[] Labels => sLabels;

   public override Pline? PointClicked (Point diagPt) {
      ShowPrompt ();
      if (mPtA is null) {
         (mPtA, X, Y) = (diagPt, diagPt.X, diagPt.Y);
         return null;
      } else {
         var firstPt = mPtA.Value;
         mPtA = null;
         ShowPrompt ();
         (Width, Height) = (diagPt.X - firstPt.X, diagPt.Y - firstPt.Y);
         return Pline.CreateRectangle (firstPt, diagPt);
      }
   }

   public override void PointHover (Point diagPt) {
      ShowPrompt ();
      mPtB = diagPt;
   }

   public override void ShowPrompt () {
      if (mEditor.Window != null)
         mEditor.Window.Prompt.Text = mPtA == null ?
            "Rectangle: Pick first corner of rectangle" : "Rectangle: Pick opposite corner of rectangle";
   }

   public override void StopDrawing () => mPtA = null;

   #region DrawableBase implementation ------------------------------
   public override void Draw (DrawingCommands drawingCommands) {
      if (mPtA == null || mPtB == null) return;
      mPtC = new (mPtA.Value.X, mPtB.Value.Y);
      mPtD = new (mPtB.Value.X, mPtA.Value.Y);
      mPoints.Add (mPtA.Value);
      mPoints.Add (mPtD.Value);
      mPoints.Add (mPtB.Value);
      mPoints.Add (mPtC.Value);
      mPoints.Add (mPtA.Value);
      drawingCommands.Brush = Brushes.Red;
      drawingCommands.DrawLines (mPoints);
      mPoints.Clear ();
   }
   #endregion
   #endregion

   #region Private --------------------------------------------------
   static string[] sLabels = { "X", "Y", "Width", "Height" };
   double mX, mY, mWidth, mHeight;
   Point? mPtA, mPtB, mPtC, mPtD;
   #endregion
}