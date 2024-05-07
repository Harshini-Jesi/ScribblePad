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
      mStartPoint.X = e.GetPosition (mEditor).X;
      mStartPoint.Y = e.GetPosition (mEditor).Y;
      mEditor.Shape.Points.Add (mStartPoint);
      mEditor.Shape.Points.Add (mStartPoint);
      mEditor.InvalidateVisual ();
   }

   public override void OnMouseMove (object sender, MouseEventArgs e) {
      if (mEditor.Shape.Points.Count > 0) {
         mCurrentPoint.X = e.GetPosition (mEditor).X;
         mCurrentPoint.Y = e.GetPosition (mEditor).Y;
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
