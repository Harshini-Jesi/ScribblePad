using BasicScrawls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BasicScrawls {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      public MainWindow () {
         InitializeComponent ();
         Closing += MainWindow_Closing;
         mToolTip.Content = "To stop or start a new one, click on the connected line tool again.";
      }

      Pen mPen = new (Brushes.White, 1);
      Shapes mShapes = null;
      ConnectedLine mConnectedLine = null;
      List<Shapes> mShapesList = new ();
      Stack<Shapes> mUndoRedo = new ();
      ToggleButton mSelectedItem = null;
      ToolTip mToolTip = new ();


      protected override void OnRender (DrawingContext dc) {
         base.OnRender (dc);
         foreach (var shape in mShapesList) {
            switch (shape) {
               case Scrawls scrawl:
                  for (int i = 0; i < scrawl.PointList.Count - 1; i++) dc.DrawLine (mPen, scrawl.PointList[i], scrawl.PointList[i + 1]);
                  break;
               case Line line:
                  dc.DrawLine (mPen, line.PointList[0], line.PointList[^1]);
                  break;
               case Rect rect:
                  Point pt1 = rect.PointList[0];
                  Point pt2 = rect.PointList[1];
                  dc.DrawRectangle (null, mPen, new (pt1, pt2));
                  break;
               case Circle circle:
                  Point start = circle.PointList[0];
                  Point end = circle.PointList[^1];
                  double x = end.X - start.X;
                  double y = end.Y - start.Y;
                  double radius = Math.Sqrt ((x * x) + (y * y));
                  dc.DrawEllipse (null, mPen, start, radius, radius);
                  break;
               case ConnectedLine conLine:
                  for (int i = 0; i < conLine.PointList.Count - 1; i++) dc.DrawLine (mPen, conLine.PointList[i], conLine.PointList[i + 1]);
                  break;
            }
         }
      }

      protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e) {
         if (e.LeftButton == MouseButtonState.Pressed) {
            Point pt = e.GetPosition (this);
            switch (mSelectedItem.Name) {
               case "scrawl":
                  mShapes = new Scrawls ();
                  mShapes.PointList.Add (pt);
                  mShapesList.Add (mShapes);
                  break;
               case "line":
                  mShapes = new Line ();
                  mShapes.PointList.Add (pt);
                  mShapes.PointList.Add (pt);
                  mShapesList.Add (mShapes);
                  break;
               case "rect":
                  mShapes = new Rect ();
                  mShapes.PointList.Add (pt);
                  mShapes.PointList.Add (pt);
                  mShapesList.Add (mShapes);
                  break;
               case "circle":
                  mShapes = new Circle ();
                  mShapes.PointList.Add (pt);
                  mShapes.PointList.Add (pt);
                  mShapesList.Add (mShapes);
                  break;
               case "conLine":
                  mShapes = mConnectedLine;
                  mShapes.PointList.Add (pt);
                  mShapes.PointList.Add (pt);
                  mToolTip.IsOpen = true;
                  break;
            }
            mUndoRedo.Clear ();
            IsSaved = false;
         }
      }

      protected override void OnMouseMove (MouseEventArgs e) {
         if (e.LeftButton == MouseButtonState.Pressed) {
            Point pt = e.GetPosition (this);
            if (mShapes is Scrawls) mShapes.PointList.Add (pt);
            else mShapes.PointList[^1] = pt;
            mShapesList[^1] = mShapes;
            InvalidateVisual ();
         }
      }

      protected override void OnMouseLeftButtonUp (MouseButtonEventArgs e) {
         if (e.LeftButton == MouseButtonState.Released) {
            Point pt = e.GetPosition (this);
            if (mShapes is Scrawls) mShapes.PointList.Add (pt);
            else mShapes.PointList[^1] = pt;
            mShapesList[^1] = mShapes;
            InvalidateVisual ();
         }
      }

      private bool IsSaved = false;
      private string mSavedFileName = null;

      private void BinSave_Click (object sender, RoutedEventArgs e) {
         if (mSavedFileName == null) NewSave ();
         else SaveChanges (mSavedFileName);
         Title = $"{System.IO.Path.GetFileNameWithoutExtension (mSavedFileName)} - Scribble Pad";
      }

      private void NewSave () {
         SaveFileDialog binSave = new ();
         binSave.FileName = "Untitled";
         binSave.DefaultExt = ".txt";
         binSave.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
         if (binSave.ShowDialog () == true) {
            mSavedFileName = binSave.FileName;
            SaveChanges (binSave.FileName);
         }
      }

      private void SaveChanges (string fileName) {
         using (BinaryWriter bw = new (File.Open (fileName, FileMode.Create))) {
            bw.Write (mShapesList.Count);
            foreach (var shape in mShapesList) {
               switch (shape) {
                  case Scrawls: bw.Write (1); break;
                  case Line: bw.Write (2); break;
                  case Rect: bw.Write (3); break;
                  case Circle: bw.Write (4); break;
                  case ConnectedLine: bw.Write (5); break;
               }
               bw.Write (shape.PointList.Count);
               foreach (Point pt in shape.PointList) {
                  bw.Write (pt.X);
                  bw.Write (pt.Y);
               }
            }
         }
         IsSaved = true;
      }

      private void MainWindow_Closing (object sender, CancelEventArgs e) {
         if (!IsSaved && mShapesList.Count > 0) {
            MessageBoxResult result = MessageBox.Show ("Do you want to save changes before closing?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel) e.Cancel = true;
            else if (result == MessageBoxResult.Yes) {
               if (mSavedFileName == null) NewSave ();
               else SaveChanges (mSavedFileName);
            }
         }
      }

      private void BinOpen_Click (object sender, RoutedEventArgs e) {
         OpenFileDialog binOpen = new ();
         if (binOpen.ShowDialog () == true) {
            if (!IsSaved && mShapesList.Count > 0) {
               MessageBoxResult result = MessageBox.Show ("Do you want to save changes before opening?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
               if (result == MessageBoxResult.Cancel) {
                  e.Handled = true;
                  return;
               } else if (result == MessageBoxResult.Yes) {
                  if (mSavedFileName == null) NewSave ();
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
                  case 3: mShapes = new Rect (); break;
                  case 4: mShapes = new Circle (); break;
                  case 5: mShapes = new ConnectedLine (); break;
               }
               int pointsCount = br.ReadInt32 ();
               for (int j = 0; j < pointsCount; j++) {
                  double x = br.ReadDouble ();
                  double y = br.ReadDouble ();
                  mShapes.PointList.Add (new Point (x, y));
               }
               mShapesList.Add (mShapes);
            }
            InvalidateVisual ();
            mSavedFileName = binOpen.FileName;
            IsSaved = true;
            Title = $"{System.IO.Path.GetFileNameWithoutExtension (mSavedFileName)} - Scribble Pad";
         }
      }

      private void Undo_Click (object sender, RoutedEventArgs e) {
         if (mShapesList.Count > 0) {
            mUndoRedo.Push (mShapesList.Last ());
            mShapesList.RemoveAt (mShapesList.Count - 1);
            InvalidateVisual ();
         }

      }

      private void Redo_Click (object sender, RoutedEventArgs e) {
         if (mUndoRedo.Count > 0) {
            mShapesList.Add (mUndoRedo.Pop ());
            InvalidateVisual ();
         }
      }

      private void Clear_Click (object sender, RoutedEventArgs e) {
         mShapesList.Clear ();
         mUndoRedo.Clear ();
         InvalidateVisual ();
      }

      private void DockPanel_Checked (object sender, RoutedEventArgs e) {
         FrameworkElement fwe = e.Source as FrameworkElement;
         mSelectedItem = fwe as ToggleButton;
         mSelectedItem.Click += ToggleButton_Click;
         e.Handled = true;
      }

      private void ToggleButton_Click (object sender, RoutedEventArgs e) {
         var clickedToggleButton = sender as ToggleButton;
         if (clickedToggleButton.IsChecked == true) {
            foreach (var control in Dock.Children) {
               if (control is ToggleButton toggleButton) {
                  toggleButton.IsChecked = (toggleButton == clickedToggleButton);
               }
            }
         } else clickedToggleButton.IsChecked = true;
      }

      private void ConLine_Click (object sender, RoutedEventArgs e) {
         mShapesList.Add (mConnectedLine);
         mToolTip.IsOpen = false;
         mConnectedLine = new ();
      }

      private void New_Click (object sender, RoutedEventArgs e) {
         if (!IsSaved && mShapesList.Count > 0) {
            MessageBoxResult result = MessageBox.Show ("Do you want to save changes before switching to a new file?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel) {
               e.Handled = true;
               return;
            } else if (result == MessageBoxResult.Yes) {
               if (mSavedFileName == null) NewSave ();
               else SaveChanges (mSavedFileName);
            }
         }
         Clear_Click (sender, e);
         Title = "Untitled - Scribble Pad";
      }
   }
}