using CADyeShapes;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace CADye;
public class DocManager {
   public DocManager (Editor editor) => mEditor = editor;
   public Editor mEditor;

   public void Clear () {
      //mEditor.Shape = null;
      mEditor.ShapesList.Clear ();
      //mUndoRedo.Clear ();
      mEditor.InvalidateVisual ();
   }

   public void New (RoutedEventArgs e) {
      if (!mIsSaved && mEditor.ShapesList.Count > 0) {
         MessageBoxResult result = MessageBox.Show ("Do you want to save changes before switching to a new file?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
         if (result == MessageBoxResult.Cancel) {
            e.Handled = true;
            return;
         } else if (result == MessageBoxResult.Yes) {
            if (mSavedFileName == null) SaveNew ();
            else SaveChanges (mSavedFileName);
         }
      }
      Clear ();
      mEditor.Window.Title = "Untitled - CADye";
   }

   public void Open (RoutedEventArgs e) {
      OpenFileDialog binOpen = new ();
      if (binOpen.ShowDialog () == true) {
         if (!mIsSaved && mEditor.ShapesList.Count > 0) {
            MessageBoxResult result = MessageBox.Show ("Do you want to save changes before opening?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel) {
               e.Handled = true;
               return;
            } else if (result == MessageBoxResult.Yes) {
               if (mSavedFileName == null) SaveNew ();
               else SaveChanges (mSavedFileName);
            }
         }
         mEditor.Shape = null;
         mEditor.ShapesList.Clear ();
         //mUndoRedo.Clear ();
         BinaryReader br = new (File.Open (binOpen.FileName, FileMode.Open));
         int shapeCount = br.ReadInt32 ();
         for (int i = 0; i < shapeCount; i++) {
            int num = br.ReadInt32 ();
            switch (num) {
               case 2: mEditor.Shape = new Line (); break;
               case 3: mEditor.Shape = new Rectangle (); break;
               case 4: mEditor.Shape = new Circle (); break;
               case 5: mEditor.Shape = new ConnectedLine (); break;
            }
            mEditor.Shape.Load (br);
            mEditor.ShapesList.Add (mEditor.Shape);
         }
         mEditor.InvalidateVisual ();
         mSavedFileName = binOpen.FileName;
         mIsSaved = true;
         mEditor.Window.Title = $"{Path.GetFileNameWithoutExtension (mSavedFileName)} - CADye";
      }
   }

   public void Save () {
      if (mSavedFileName == null) SaveNew ();
      else SaveChanges (mSavedFileName);
      mEditor.Window.Title = $"{Path.GetFileNameWithoutExtension (mSavedFileName)} - CADye";
   }

   //Saves the changes done on a saved file
   private void SaveChanges (string fileName) {
      using (BinaryWriter bw = new (File.Open (fileName, FileMode.Create))) {
         bw.Write (mEditor.ShapesList.Count);
         foreach (var shape in mEditor.ShapesList) shape.Save (bw);
      }
      mIsSaved = true;
   }

   //Saves a new file i.e.,saves the file for the first time
   private void SaveNew () {
      SaveFileDialog binSave = new () {
         FileName = "Untitled",
         DefaultExt = ".txt",
         Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
      };
      if (binSave.ShowDialog () == true) {
         mSavedFileName = binSave.FileName;
         SaveChanges (binSave.FileName);
      }
   }

   //Shows messagebox when attempting to close the window without saving
   public void MainWindow_Closing (object sender, CancelEventArgs e) {
      if (!mIsSaved && mEditor.ShapesList.Count > 0) {
         MessageBoxResult result = MessageBox.Show ("Do you want to save changes before closing?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
         if (result == MessageBoxResult.Cancel) e.Cancel = true;
         else if (result == MessageBoxResult.Yes) {
            if (mSavedFileName == null) SaveNew ();
            else SaveChanges (mSavedFileName);
         }
      }
   }
   bool mIsSaved = false;
   string mSavedFileName = null;
}