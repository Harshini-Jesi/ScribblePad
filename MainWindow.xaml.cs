using CADye.Lib;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CADye {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      public MainWindow () {
         InitializeComponent ();
         mCanvas.Window = this;
         Closing += mCanvas.DocMgr.MainWindow_Closing;
      }

      public StackPanel InputBar => mInputBar;

      public TextBlock Prompt => mPrompt;

      private void OnEscKey (object sender, KeyEventArgs e) => mCanvas.EscKey (e);

      private void OnNew_Click (object sender, RoutedEventArgs e) => mCanvas.DocMgr.New ();

      private void OnOpen_Click (object sender, RoutedEventArgs e) {
         mCanvas.DocMgr.Open (out DrawingSheet dwg);
         if (dwg.Shapes.Count > 0) {
            mCanvas.Dwg.Shapes.Clear ();
            for (int i = 0; i < dwg.Shapes.Count; i++) mCanvas.Dwg.Shapes.Add (dwg.Shapes[i]);
         }
         mCanvas.InvalidateVisual ();
      }

      private void OnSave_Click (object sender, RoutedEventArgs e) => mCanvas.DocMgr.Save ();

      private void OnUndo_Click (object sender, RoutedEventArgs e) => mCanvas.Undo ();

      private void OnRedo_Click (object sender, RoutedEventArgs e) => mCanvas.Redo ();

      private void OnClear_Click (object sender, RoutedEventArgs e) => mCanvas.DocMgr.Clear ();

      private void StackPanel_Checked (object sender, RoutedEventArgs e) {
         if (mCanvas != null) {
            mCanvas.SelectedButton = (ToggleButton)e.Source;
            mCanvas.SelectedButton.Click += ToggleButton_Click;
            mCanvas.SwitchWidget ();
            e.Handled = true;
         }
      }

      private void ToggleButton_Click (object sender, RoutedEventArgs e) {
         var clickedToggleButton = sender as ToggleButton;
         if (clickedToggleButton != null) {
            if (clickedToggleButton.IsChecked == true) {
               foreach (var control in Stack.Children)
                  if (control is ToggleButton toggleButton)
                     toggleButton.IsChecked = (toggleButton == clickedToggleButton);
            } else clickedToggleButton.IsChecked = true;
         }
      }
   }
}