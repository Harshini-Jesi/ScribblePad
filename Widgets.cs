using CADye.Lib;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CADye;
public abstract class Widget : DrawableBase, INotifyPropertyChanged {
   public Widget (Editor editor) => mEditor = editor;

   public abstract string[] Labels { get; }

   public abstract Pline? PointClicked (Point drawingPt);

   public abstract void PointHover (Point drawingPt);

   public abstract void ShowPrompt ();

   public abstract void StopDrawing ();

   protected void OnPropertyChanged (string propertyName) {
      PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
   }
   public event PropertyChangedEventHandler? PropertyChanged;

   public void OnMouseDown (object sender, MouseButtonEventArgs e) {
      mXfmStartPt = mEditor.InvProjXfm.Transform (e.GetPosition (mEditor));
      DrawingClicked (new (mXfmStartPt.X, mXfmStartPt.Y));
      mEditor.InvalidateVisual ();
   }

   public void OnMouseMove (object sender, MouseEventArgs e) {
      mXfmCurrentPt = mEditor.InvProjXfm.Transform (e.GetPosition (mEditor));
      DrawingHover (new (mXfmCurrentPt.X, mXfmCurrentPt.Y));
      mEditor.InvalidateVisual ();
   }

   public void Attach () {
      mEditor.Cursor = Cursors.Cross;
      mEditor.MouseLeftButtonDown += OnMouseDown;
      mEditor.MouseMove += OnMouseMove;
      foreach (var label in Labels) {
         mLabel = new () { Content = label };
         var binding = new Binding (label) { Source = this, StringFormat = "F2", UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
         mTextBox = new () { Height = 30, Width = 60, VerticalContentAlignment = System.Windows.VerticalAlignment.Center };
         mTextBox.SetBinding (TextBox.TextProperty, binding);
         mEditor.Window!.InputBar.Children.Add (mLabel);
         mEditor.Window.InputBar.Children.Add (mTextBox);
      }
   }

   public void Detach () {
      mEditor.Cursor = Cursors.Arrow;
      mEditor.MouseLeftButtonDown -= OnMouseDown;
      mEditor.MouseMove -= OnMouseMove;
      mEditor.Window!.InputBar.Children.Clear ();
   }

   /// <summary>Mouse click, in drawing space</summary>
   void DrawingClicked (Point drawingPt) {
      var pline = PointClicked (drawingPt);
      if (pline == null) return;
      mEditor.Dwg.AddPline (pline);
   }

   /// <summary>Mouse hover, in drawing space</summary>
   void DrawingHover (Point drawingPt) => PointHover (drawingPt);


   protected Editor mEditor;
   protected System.Windows.Point mXfmStartPt, mXfmCurrentPt;
   Label? mLabel;
   TextBox? mTextBox;
   protected List<Point> mPoints = new ();
}