using CADye.Lib;
using System.Windows.Input;

namespace CADye;
public class RectWidget : Widget {
   public RectWidget (Editor editor) : base (editor) {
      mEditor.Shape = new Rectangle ();
      mEditor.Window.Prompt.Text = PromptText;
   }

   public override string PromptText => mEditor.Shape.Points.Count == 0 ? "Rectangle: Pick first corner of rectangle" : "Rectangle: Pick opposite corner of rectangle";
   public override string[] Labels => sLabels;

   public override void OnMouseDown (object sender, MouseButtonEventArgs e) {
      mEditor.Window.Prompt.Text = PromptText;
      mStartPoint.X = e.GetPosition (mEditor).X;
      mStartPoint.Y = e.GetPosition (mEditor).Y;
      mEditor.Shape.Points.Add (mStartPoint);
      mEditor.Shape.Points.Add (mStartPoint);
      if (mEditor.Shape.Points.Count > 2) {
         mEditor.Dwg.Shapes.Add (mEditor.Shape);
         mEditor.Shape = new Rectangle ();
         mEditor.Window.Prompt.Text = PromptText;
      }
      mEditor.InvalidateVisual ();
   }

   public override void OnMouseMove (object sender, MouseEventArgs e) {
      if (mEditor.Shape.Points.Count > 0) {
         mCurrentPoint.X = e.GetPosition (mEditor).X;
         mCurrentPoint.Y = e.GetPosition (mEditor).Y;
         mEditor.Window.Prompt.Text = PromptText;
         mEditor.Shape.UpdateEndPoint (mCurrentPoint);
         mEditor.InvalidateVisual ();
      }
   }

   static string[] sLabels = { "X", "Y", "Width", "Height" };
}