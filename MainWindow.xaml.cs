using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CADye {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      public MainWindow () {
         InitializeComponent ();
         mCanvas.Window = this;
         mDoc = new (mCanvas);
         Closing += mDoc.MainWindow_Closing;
      }

      public TextBlock Prompt => mPrompt;

      private void OnNew_Click (object sender, RoutedEventArgs e) => mDoc.New (e);

      private void OnOpen_Click (object sender, RoutedEventArgs e) => mDoc.Open (e);

      private void OnSave_Click (object sender, RoutedEventArgs e) => mDoc.Save ();

      private void OnUndo_Click (object sender, RoutedEventArgs e) => mCanvas.Undo ();

      private void OnRedo_Click (object sender, RoutedEventArgs e) => mCanvas.Redo ();

      //private void OnConLine_Click (object sender, RoutedEventArgs e) { }

      private void OnClear_Click (object sender, RoutedEventArgs e) => mDoc.Clear ();

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
         if (clickedToggleButton.IsChecked == true) {
            foreach (var control in Stack.Children)
               if (control is ToggleButton toggleButton)
                  toggleButton.IsChecked = (toggleButton == clickedToggleButton);
         } else clickedToggleButton.IsChecked = true;
      }

      DocManager mDoc;
   }
}