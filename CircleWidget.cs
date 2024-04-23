using CADye.lib;
using Point = CADye.lib.Point;
using System.Windows.Input;

namespace CADye;
public class CircleWidget : Widget {
   public CircleWidget (Editor editor) : base (editor) {
      mEditor.Shape = new Circle ();
      ShowDetails ();
   }

   public override void OnMouseDown (MouseButtonEventArgs e) {
      Point startPoint;
      mEditor.Window.Prompt.Text = "Circle: Pick center point";
      startPoint.X = e.GetPosition (mEditor).X;
      startPoint.Y = e.GetPosition (mEditor).Y;
      mEditor.Shape.pointList.Add (startPoint);
      mEditor.Shape.pointList.Add (startPoint);
      if (mEditor.Shape.pointList.Count > 2) {
         mEditor.ShapesList.Add (mEditor.Shape);
         mEditor.Shape = new Circle ();
      }
      mEditor.InvalidateVisual ();
   }

   public override void OnMouseMove (MouseEventArgs e) {
      if (mEditor.Shape.pointList.Count > 0) {
         Point currentPoint;
         currentPoint.X = e.GetPosition (mEditor).X;
         currentPoint.Y = e.GetPosition (mEditor).Y;
         mEditor.Window.Prompt.Text = "Circle: Pick a point on circle";
         mEditor.Shape.UpdateEndPoint (currentPoint);
         mEditor.InvalidateVisual ();
      }
   }

   void ShowDetails () {
      mEditor.Window.Prompt.Text = "Circle: Pick center point";
      mEditor.Window.RectDetails.Visibility = System.Windows.Visibility.Collapsed;
      mEditor.Window.LineDetails.Visibility = System.Windows.Visibility.Collapsed;
      mEditor.Window.CircleDetails.Visibility = System.Windows.Visibility.Visible;
   }
}