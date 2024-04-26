using CADye.Lib;
using System.Windows.Controls;
using System.Windows.Input;

namespace CADye;
public abstract class Widget {
   public Widget (Editor editor) {
      mEditor = editor;
      mEditor.Shape = new Line ();
   }

   public abstract string PromptText { get; }

   public abstract string[] Labels { get; }

   public abstract void OnMouseDown (object sender, MouseButtonEventArgs e);

   public abstract void OnMouseMove (object sender, MouseEventArgs e);

   public void Attach () {
      mEditor.MouseDown += OnMouseDown;
      mEditor.MouseMove += OnMouseMove;
      foreach (var label in Labels) {
         mLabel = new () { Content = label };
         mTextBox = new () { Height = 30, Width = 60 };
         mEditor.Window.InputBar.Children.Add (mLabel);
         mEditor.Window.InputBar.Children.Add (mTextBox);
      }
   }

   public void Detach () {
      mEditor.MouseDown -= OnMouseDown;
      mEditor.MouseMove -= OnMouseMove;
      mEditor.Window.InputBar.Children.Clear ();
   }

   public void StopDrawing (KeyEventArgs e) {
      if (e.Key == Key.Escape) {
         if (mEditor.Shape is ConnectedLine) {
            mEditor.Shape.UpdateEndPoint (mStartPoint);
            mEditor.Dwg.Shapes.Add (mEditor.Shape);
            mEditor.Shape = new ConnectedLine ();
         } else mEditor.Shape.Points.Clear ();
      }
   }

   protected Editor mEditor;
   protected Point mStartPoint, mCurrentPoint;
   Label? mLabel;
   TextBox? mTextBox;
}