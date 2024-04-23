using System.Windows.Input;

namespace CADye;
public abstract class Widget {
   public Widget (Editor editor) => mEditor = editor;
   public Editor mEditor;

   public abstract void OnMouseDown (MouseButtonEventArgs e);

   public abstract void OnMouseMove (MouseEventArgs e);
}