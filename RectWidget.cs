using CADye.lib;
using Point = CADye.lib.Point;
using System.Windows.Input;

namespace CADye;
public class RectWidget : Widget {
   public RectWidget (Editor editor) : base (editor) {
      mEditor.Shape = new Rectangle ();
      ShowDetails ();
   }

   public override void OnMouseDown (MouseButtonEventArgs e) {
      Point startPoint;
      mEditor.Window.Prompt.Text = "Rectangle: Pick first corner of rectangle";
      startPoint.X = e.GetPosition (mEditor).X;
      startPoint.Y = e.GetPosition (mEditor).Y;
      mEditor.Shape.pointList.Add (startPoint);
      mEditor.Shape.pointList.Add (startPoint);
      if (mEditor.Shape.pointList.Count > 2) {
         mEditor.ShapesList.Add (mEditor.Shape);
         mEditor.Shape = new Rectangle ();
      }
      mEditor.InvalidateVisual ();
   }

   public override void OnMouseMove (MouseEventArgs e) {
      if (mEditor.Shape.pointList.Count > 0) {
         Point currentPoint;
         currentPoint.X = e.GetPosition (mEditor).X;
         currentPoint.Y = e.GetPosition (mEditor).Y;
         mEditor.Window.Prompt.Text = "Rectangle: Pick opposite corner of rectangle";
         mEditor.Shape.UpdateEndPoint (currentPoint);
         mEditor.InvalidateVisual ();
      }
   }

   void ShowDetails () {
      mEditor.Window.Prompt.Text = "Rectangle: Pick first corner of rectangle";
      mEditor.Window.LineDetails.Visibility = System.Windows.Visibility.Collapsed;
      mEditor.Window.CircleDetails.Visibility = System.Windows.Visibility.Collapsed;
      mEditor.Window.RectDetails.Visibility = System.Windows.Visibility.Visible;
   }
}