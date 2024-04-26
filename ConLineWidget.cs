using CADye.Lib;
using System.Windows.Input;

namespace CADye;
public class ConLineWidget : Widget {
   public ConLineWidget (Editor editor) : base (editor) {
      mEditor.Shape = new ConnectedLine ();
      mEditor.Window.Prompt.Text = "ConnectedLine: Pick beginning point";
   }

   public override string PromptText => throw new System.NotImplementedException ();

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
         mEditor.Window.Prompt.Text = "ConnectedLine: Pick end point";
         mEditor.Shape.UpdateEndPoint (mCurrentPoint);
         mEditor.InvalidateVisual ();
      }
   }

   static string[] sLabels = { "X", "Y", "dX", "dY", "Length", "Angle" };
}
