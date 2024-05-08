using CADye.Lib;
using System.Windows.Input;

namespace CADye;
public class ConLineWidget : Widget {
   public ConLineWidget (Editor editor) : base (editor) {
      mEditor.Shape = new ConnectedLine ();
      ShowPrompt ();
   }

   public override string[] Labels => sLabels;

   public override void OnMouseDown (object sender, MouseButtonEventArgs e) {
      mXfmStartPt = mEditor.InvProjXfm.Transform (e.GetPosition (mEditor));
      (mStartPoint.X, mStartPoint.Y) = (mXfmStartPt.X, mXfmStartPt.Y);
      mEditor.Shape.Points.Add (mStartPoint);
      mEditor.Shape.Points.Add (mStartPoint);
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
            "ConnectedLine: Pick beginning point": "ConnectedLine: Pick end point";
   }

   static string[] sLabels = { "X", "Y", "dX", "dY", "Length", "Angle" };
}
