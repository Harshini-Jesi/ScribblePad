using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
         marker = new (Brushes.White, 1);
         scrawlPoints = new ();
         scrawlPointsList = new ();
         undoRedo = new ();
         InitializeComponent ();
      }

      Pen marker;
      PointCollection scrawlPoints;
      List<PointCollection> scrawlPointsList;
      Stack<PointCollection> undoRedo;

      protected override void OnRender (DrawingContext dc) {
         base.OnRender (dc);
         foreach (PointCollection scrawl in scrawlPointsList) {
            for (int i = 0; i < scrawl.Count - 1; i++)
               dc.DrawLine (marker, scrawl[i], scrawl[i + 1]);
         }
         for (int i = 0; i < scrawlPoints.Count - 1; i++)
            dc.DrawLine (marker, scrawlPoints[i], scrawlPoints[i + 1]);
      }

      protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e) {
         if (e.LeftButton == MouseButtonState.Pressed) {
            Point pt = e.GetPosition (this);
            scrawlPoints.Add (pt);
            InvalidateVisual ();
         }
      }

      protected override void OnMouseMove (MouseEventArgs e) {
         if (e.LeftButton == MouseButtonState.Pressed) {
            Point pt = e.GetPosition (this);
            scrawlPoints.Add (pt);
            InvalidateVisual ();
         }
      }

      protected override void OnMouseLeftButtonUp (MouseButtonEventArgs e) {
         if (e.LeftButton == MouseButtonState.Released) {
            Point pt = e.GetPosition (this);
            scrawlPoints.Add (pt);
            scrawlPointsList.Add (scrawlPoints);
            scrawlPoints = new PointCollection ();
         }
      }

      private void TxtSave_Click (object sender, RoutedEventArgs e) {
         SaveFileDialog txtSave = new ();
         if (txtSave.ShowDialog () == true) {
            using StreamWriter sw = new (txtSave.FileName);
            foreach (PointCollection pc in scrawlPointsList) {
               foreach (Point pt in pc) sw.WriteLine (pt.ToString ());
               sw.WriteLine ("pc_end");
            }
         }
      }

      private void BinSave_Click (object sender, RoutedEventArgs e) {
         SaveFileDialog binSave = new ();
         if (binSave.ShowDialog () == true) {
            BinaryWriter bw = new (File.Open (binSave.FileName, FileMode.Create));
            bw.Write (scrawlPointsList.Count);
            foreach (PointCollection pc in scrawlPointsList) {
               bw.Write (pc.Count);
               foreach (Point pt in pc) {
                  bw.Write (pt.X);
                  bw.Write (pt.Y);
               }
            }
         }
      }

      private void TxtOpen_Click (object sender, RoutedEventArgs e) {
         scrawlPointsList.Clear ();
         undoRedo.Clear ();
         OpenFileDialog txtOpen = new ();
         if (txtOpen.ShowDialog () == true) {
            using (StreamReader sr = new (txtOpen.FileName)) {
               string scrawl;
               while ((scrawl = sr.ReadLine ()) != null) {
                  if (scrawl == "pc_end") {
                     scrawlPointsList.Add (new PointCollection (scrawlPoints));
                     scrawlPoints = new PointCollection ();
                  } else {
                     string[] point = scrawl.Split (',');
                     double x = double.Parse (point[0]);
                     double y = double.Parse (point[1]);
                     scrawlPoints.Add (new Point (x, y));
                  }
               }
            }
            InvalidateVisual ();
         }
      }

      private void BinOpen_Click (object sender, RoutedEventArgs e) {
         scrawlPointsList.Clear ();
         undoRedo.Clear ();
         OpenFileDialog binOpen = new ();
         if (binOpen.ShowDialog () == true) {
            BinaryReader br = new (File.Open (binOpen.FileName, FileMode.Open));
            int scrawlCount = br.ReadInt32 ();
            for (int i = 0; i < scrawlCount; i++) {
               int pointsCount = br.ReadInt32 ();
               PointCollection scrawls = new ();
               for (int j = 0; j < pointsCount; j++) {
                  double x = br.ReadDouble ();
                  double y = br.ReadDouble ();
                  scrawls.Add (new Point (x, y));
               }
               scrawlPointsList.Add (scrawls);
            }
            InvalidateVisual ();
         }
      }

      private void Undo_Click (object sender, RoutedEventArgs e) {
         if (scrawlPointsList.Count > 0) {
            undoRedo.Push (scrawlPointsList.Last ());
            scrawlPointsList.Remove (scrawlPointsList.Last ());
            InvalidateVisual ();
         }
      }

      private void Redo_Click (object sender, RoutedEventArgs e) {
         if (undoRedo.Count > 0) {
            scrawlPointsList.Add (undoRedo.Pop ());
            InvalidateVisual ();
         }
      }

      private void Clear_Click (object sender, RoutedEventArgs e) {
         scrawlPointsList.Clear ();
         InvalidateVisual ();
      }
   }
}