using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BasicScrawls {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      #region constructor -------------------------------------------
      public MainWindow () {
         mEvents = new EventHandler (this);
         InitializeComponent ();
         Closing += mEvents.MainWindow_Closing;
      }
      EventHandler mEvents;
      #endregion

      #region EventHandlers -----------------------------------------
      //Opens an existing file
      private void OnBinOpen_Click (object sender, RoutedEventArgs e) => mEvents.EBinOpen (e);

      //Saves the file
      private void OnBinSave_Click (object sender, RoutedEventArgs e) => mEvents.EBinSave ();

      //Clears the window
      private void OnClear_Click (object sender, RoutedEventArgs e) => mEvents.EClear ();

      //Click event for connected line icon
      private void OnConLine_Click (object sender, RoutedEventArgs e) => mEvents.EConLine ();

      //Click event for all toggle buttons on the dock panel
      private void DockPanel_Checked (object sender, RoutedEventArgs e) => mEvents.EDockCheck (e);

      //Creates a new file
      private void OnNew_Click (object sender, RoutedEventArgs e) => mEvents.ENew (e);

      //Performs redo action
      private void OnRedo_Click (object sender, RoutedEventArgs e) => mEvents.ERedo ();

      //Performs undo action
      private void OnUndo_Click (object sender, RoutedEventArgs e) => mEvents.EUndo ();
      #endregion

      #region MouseEvents -------------------------------------------
      protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e) => mEvents.EMouseDown (e);

      protected override void OnMouseLeftButtonUp (MouseButtonEventArgs e) => mEvents.EMouseUp (e);

      protected override void OnMouseMove (MouseEventArgs e) => mEvents.EMouseMove (e);

      protected override void OnRender (DrawingContext dc) {
         base.OnRender (dc);
         mEvents.ERender (dc);
      }
      #endregion
   }
}