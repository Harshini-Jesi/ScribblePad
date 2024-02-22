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
         InitializeComponent ();
      }

      Pen marker;
      PointCollection scrawlPoints;

      protected override void OnRender (DrawingContext dc) {
         base.OnRender (dc);
         for (int i = 0; i < scrawlPoints.Count - 1; i++) {
            dc.DrawLine (marker, scrawlPoints[i], scrawlPoints[i + 1]);
         }
      }

      protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e) {
         scrawlPoints.Clear ();
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
         }
      }

      private void TxtSave_Click (object sender, RoutedEventArgs e) {
         SaveFileDialog txt_save = new ();
         if (txt_save.ShowDialog () == true)
            File.WriteAllText (txt_save.FileName, scrawlPoints.ToString ());
      }

      private void BinSave_Click (object sender, RoutedEventArgs e) {
         SaveFileDialog bin_save = new ();
         if (bin_save.ShowDialog () == true) {
            BinaryWriter bw = new (File.Open (bin_save.FileName, FileMode.Create));
            foreach (Point pt in scrawlPoints) {
               bw.Write (pt.X);
               bw.Write (pt.Y);
            }
         }
      }

      private void TxtOpen_Click (object sender, RoutedEventArgs e) {
         scrawlPoints.Clear ();
         OpenFileDialog txt_open = new ();
         if (txt_open.ShowDialog () == true) {
            scrawlPoints = PointCollection.Parse (File.ReadAllText (txt_open.FileName));
            InvalidateVisual ();
         }
      }

      private void BinOpen_Click (object sender, RoutedEventArgs e) {
         OpenFileDialog bin_open = new ();
         if (bin_open.ShowDialog () == true) {
            scrawlPoints.Clear ();
            BinaryReader br = new (File.Open (bin_open.FileName, FileMode.Open));
            while (br.BaseStream.Position != br.BaseStream.Length) {
               double x = br.ReadDouble ();
               double y = br.ReadDouble ();
               scrawlPoints.Add (new Point (x, y));
            }
            InvalidateVisual ();
         }
      }
   }
}