using BackEnd;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Point = BackEnd.Point;

namespace BasicScrawls;
#region class EventHandler ------------------------------------------------------------------------
public class EventHandler {
   #region constructor ----------------------------------------------
   public EventHandler (MainWindow source) => main = source;
   MainWindow main;
   #endregion

   #region Methods --------------------------------------------------
   public void EBinOpen (RoutedEventArgs e) {
      OpenFileDialog binOpen = new ();
      if (binOpen.ShowDialog () == true) {
         if (!mIsSaved && mShapesList.Count > 0) {
            MessageBoxResult result = MessageBox.Show ("Do you want to save changes before opening?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel) {
               e.Handled = true;
               return;
            } else if (result == MessageBoxResult.Yes) {
               if (mSavedFileName == null) SaveNew ();
               else SaveChanges (mSavedFileName);
            }
         }
         mShapesList.Clear ();
         mUndoRedo.Clear ();
         BinaryReader br = new (File.Open (binOpen.FileName, FileMode.Open));
         int shapeCount = br.ReadInt32 ();
         for (int i = 0; i < shapeCount; i++) {
            int num = br.ReadInt32 ();
            switch (num) {
               case 1: mShapes = new Scrawls (); break;
               case 2: mShapes = new Line (); break;
               case 3: mShapes = new Rectangle (); break;
               case 4: mShapes = new Circle (); break;
               case 5: mShapes = new ConnectedLine (); break;
            }
            mShapes.Load (br);
            mShapesList.Add (mShapes);
         }
         main.InvalidateVisual ();
         mSavedFileName = binOpen.FileName;
         mIsSaved = true;
         main.Title = $"{Path.GetFileNameWithoutExtension (mSavedFileName)} - Scribble Pad";
      }
   }

   public void EBinSave () {
      if (mSavedFileName == null) SaveNew ();
      else SaveChanges (mSavedFileName);
      main.Title = $"{Path.GetFileNameWithoutExtension (mSavedFileName)} - Scribble Pad";
   }

   public void EClear () {
      mShapes = null;
      mShapesList.Clear ();
      mUndoRedo.Clear ();
      main.InvalidateVisual ();
   }

   public void EConLine () {
      mShapesList.Add (mShapes);
      main.StatusBar.Visibility = Visibility.Collapsed;
      main.InvalidateVisual ();
      mShapes = new ConnectedLine ();
   }

   public void EDockCheck (RoutedEventArgs e) {
      FrameworkElement fwe = e.Source as FrameworkElement;
      mSelectedItem = fwe as ToggleButton;
      mSelectedItem.Click += ToggleButton_Click;
      e.Handled = true;
   }

   public void ENew (RoutedEventArgs e) {
      if (!mIsSaved && mShapesList.Count > 0) {
         MessageBoxResult result = MessageBox.Show ("Do you want to save changes before switching to a new file?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
         if (result == MessageBoxResult.Cancel) {
            e.Handled = true;
            return;
         } else if (result == MessageBoxResult.Yes) {
            if (mSavedFileName == null) SaveNew ();
            else SaveChanges (mSavedFileName);
         }
      }
      EClear ();
      main.Title = "Untitled - Scribble Pad";
   }

   public void EMouseDown (MouseButtonEventArgs e) {
      if (e.LeftButton == MouseButtonState.Pressed) {
         Point pt;
         pt.X = e.GetPosition (main).X;
         pt.Y = e.GetPosition (main).Y;
         switch (mSelectedItem.Name) {
            case "scrawl":
               mShapes = new Scrawls ();
               break;
            case "line":
               mShapes = new Line ();
               break;
            case "rect":
               mShapes = new Rectangle ();
               break;
            case "circle":
               mShapes = new Circle ();
               break;
            case "conLine":
               main.StatusBar.Visibility = Visibility.Visible;
               break;
         }
         mShapes.pointList.Add (pt);
         mShapes.pointList.Add (pt);
         mUndoRedo.Clear ();
         main.redo.IsEnabled = false;
         mIsSaved = false;
      }
   }

   public void EMouseMove (MouseEventArgs e) {
      if (e.LeftButton == MouseButtonState.Pressed) {
         Point pt;
         pt.X = e.GetPosition (main).X;
         pt.Y = e.GetPosition (main).Y;
         mShapes.UpdateEndPoint (pt);
         main.InvalidateVisual ();
      }
   }

   public void EMouseUp (MouseButtonEventArgs e) {
      if (e.LeftButton == MouseButtonState.Released) {
         Point pt;
         pt.X = e.GetPosition (main).X;
         pt.Y = e.GetPosition (main).Y;
         mShapes.UpdateEndPoint (pt);
         if (mShapes is not ConnectedLine) mShapesList.Add (mShapes);
         main.InvalidateVisual ();
         main.undo.IsEnabled = true;
      }
   }

   public void ERedo () {
      if (mUndoRedo.Count > 0) {
         mShapesList.Add (mUndoRedo.Pop ());
         main.InvalidateVisual ();
      }
   }

   public void ERender (DrawingContext dc) {
      foreach (var shape in mShapesList) if (shape != null) Draw (dc, shape);
      if (mShapes != null) Draw (dc, mShapes);
   }

   public void EUndo () {
      mShapes = null;
      if (mShapesList.Count > 0 && !mIsSaved) {
         mUndoRedo.Push (mShapesList.Last ());
         mShapesList.Remove (mShapesList.Last ());
         main.InvalidateVisual ();
      }
      main.redo.IsEnabled = true;
   }
   #endregion

   #region Implementation -------------------------------------------
   //To draw the respective shapes
   private void Draw (DrawingContext dc, Shapes shape) {
      System.Windows.Point start, end;
      if (shape is Rectangle or Circle) {
         start.X = shape.pointList[0].X; start.Y = shape.pointList[0].Y;
         end.X = shape.pointList[^1].X; end.Y = shape.pointList[^1].Y;
         switch (shape) {
            case Rectangle: dc.DrawRectangle (null, mPen, new (start, end)); break;
            case Circle:
               double x = end.X - start.X;
               double y = end.Y - start.Y;
               double radius = Math.Sqrt ((x * x) + (y * y));
               dc.DrawEllipse (null, mPen, start, radius, radius);
               break;
         }
      } else
         for (int i = 0; i < shape.pointList.Count - 1; i++) {
            start.X = shape.pointList[i].X; start.Y = shape.pointList[i].Y;
            end.X = shape.pointList[i + 1].X; end.Y = shape.pointList[i + 1].Y;
            dc.DrawLine (mPen, start, end);
         }
   }

   //Shows messagebox when attempting to close the window without saving
   public void MainWindow_Closing (object sender, CancelEventArgs e) {
      if (!mIsSaved && mShapesList.Count > 0) {
         MessageBoxResult result = MessageBox.Show ("Do you want to save changes before closing?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
         if (result == MessageBoxResult.Cancel) e.Cancel = true;
         else if (result == MessageBoxResult.Yes) {
            if (mSavedFileName == null) SaveNew ();
            else SaveChanges (mSavedFileName);
         }
      }
   }

   //Saves the changes done on a saved file
   private void SaveChanges (string fileName) {
      using (BinaryWriter bw = new (File.Open (fileName, FileMode.Create))) {
         bw.Write (mShapesList.Count);
         foreach (var shape in mShapesList) shape.Save (bw);
      }
      mIsSaved = true;
   }

   //Saves a new file i.e.,saves the file for the first time
   private void SaveNew () {
      SaveFileDialog binSave = new ();
      binSave.FileName = "Untitled";
      binSave.DefaultExt = ".txt";
      binSave.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
      if (binSave.ShowDialog () == true) {
         mSavedFileName = binSave.FileName;
         SaveChanges (binSave.FileName);
      }
   }

   //Ensures only one toggle button is on at a time
   private void ToggleButton_Click (object sender, RoutedEventArgs e) {
      var clickedToggleButton = sender as ToggleButton;
      if (clickedToggleButton.IsChecked == true) {
         foreach (var control in main.Dock.Children)
            if (control is ToggleButton toggleButton)
               toggleButton.IsChecked = (toggleButton == clickedToggleButton);
      } else clickedToggleButton.IsChecked = true;
   }
   #endregion

   #region Private --------------------------------------------------
   Pen mPen = new (Brushes.White, 1);
   Shapes mShapes = null;
   List<Shapes> mShapesList = new ();
   Stack<Shapes> mUndoRedo = new ();
   ToggleButton mSelectedItem = null;
   bool mIsSaved = false;
   string mSavedFileName = null;
   #endregion
}
#endregion


