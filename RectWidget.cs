using CADye.Lib;
using System.Windows.Input;

namespace CADye;
public class RectWidget : Widget {
   public RectWidget (Editor editor) : base (editor) {
      mEditor.Shape = new Rectangle ();
      ShowPrompt ();
   }

   public override string[] Labels => sLabels;

   public override void OnMouseDown (object sender, MouseButtonEventArgs e) {
      ShowPrompt ();
      mXfmStartPt = mEditor.InvProjXfm.Transform (e.GetPosition (mEditor));
      (mStartPoint.X, mStartPoint.Y) = (mXfmStartPt.X, mXfmStartPt.Y);
      mEditor.Shape.Points.Add (mStartPoint);
      mEditor.Shape.Points.Add (mStartPoint);
      if (mEditor.Shape.Points.Count > 2) {
         mEditor.Dwg.AddShape (mEditor.Shape);
         mEditor.Shape = new Rectangle ();
         ShowPrompt ();
      }
      mEditor.InvalidateVisual ();
   }

   public override void OnMouseMove (object sender, MouseEventArgs e) {
      if (mEditor.Shape.Points.Count > 0) {
         mXfmCurrentPt = mEditor.InvProjXfm.Transform (e.GetPosition (mEditor));
         (mCurrentPoint.X, mCurrentPoint.Y) = (mXfmCurrentPt.X, mXfmCurrentPt.Y);
         ShowPrompt ();
         mEditor.Shape.UpdateEndPoint (mCurrentPoint);
         mEditor.InvalidateVisual ();
      }
   }

   public override void ShowPrompt () {
      if (mEditor.Window != null)
         mEditor.Window.Prompt.Text = mEditor.Shape.Points.Count == 0 ? 
            "Rectangle: Pick first corner of rectangle" : "Rectangle: Pick opposite corner of rectangle";
   }

   static string[] sLabels = { "X", "Y", "Width", "Height" };
}