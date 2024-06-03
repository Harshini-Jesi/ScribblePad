using CADye.Lib;
using Microsoft.Win32;
using System.ComponentModel;
using System;
using System.IO;
using System.Windows;

namespace CADye;
public class DocManager {
   public DocManager (Editor editor) => mEditor = editor;

   public bool IsSaved => mIsSaved;

   public void Clear () {
      if (mEditor.Dwg.Plines.Count > 0) {
         MessageBoxResult result = MessageBox.Show ("Are you sure you want to clear all?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);
         if (result == MessageBoxResult.OK) mEditor.Dwg.Plines.Clear ();
      }
      mEditor.InvalidateVisual ();
   }

   public void New () {
      if (!mIsSaved && mEditor.Dwg.Plines.Count > 0) {
         MessageBoxResult result = MessageBox.Show ("Do you want to save changes before switching to a new file?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
         if (result == MessageBoxResult.Cancel) return;
         else if (result == MessageBoxResult.Yes) {
            if (mSavedFileName == null) SaveNew ();
            else SaveChanges (mSavedFileName);
         }
      }
      mEditor.Dwg = new ();
      if (mEditor.Window != null) mEditor.Window.Title = "Untitled - CADye";
      mEditor.InvalidateVisual ();
   }

   public void Open () {
      OpenFileDialog binOpen = new ();
      if (binOpen.ShowDialog () == true) {
         if (!mIsSaved && mEditor.Dwg.Plines.Count > 0) {
            MessageBoxResult result = MessageBox.Show ("Do you want to save changes before opening?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel) return;
            else if (result == MessageBoxResult.Yes) {
               if (mSavedFileName == null) SaveNew ();
               else SaveChanges (mSavedFileName);
            } else mEditor.Dwg.Plines.Clear ();
         }
         BinaryReader br = new (File.Open (binOpen.FileName, FileMode.Open));
         int pLineCount = br.ReadInt32 ();
         for (int i = 0; i < pLineCount; i++) {
            var pLine = new Pline ();
            pLine.Load (br);
            mEditor.Dwg.AddPline (pLine);
         }
         mSavedFileName = binOpen.FileName;
         mIsSaved = true;
         if (mEditor.Window != null) mEditor.Window.Title = $"{Path.GetFileNameWithoutExtension (mSavedFileName)} - CADye";
         mEditor.InvalidateVisual ();
      }
   }

   public void Save () {
      if (mSavedFileName == null) SaveNew ();
      else SaveChanges (mSavedFileName);
      if (mEditor.Window != null) mEditor.Window.Title = $"{Path.GetFileNameWithoutExtension (mSavedFileName)} - CADye";
   }

   //Saves the changes done on a saved file
   private void SaveChanges (string fileName) {
      using (BinaryWriter bw = new (File.Open (fileName, FileMode.Create))) {
         bw.Write (mEditor.Dwg.Plines.Count);
         foreach (var shape in mEditor.Dwg.Plines) shape.Save (bw);
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
   public void MainWindow_Closing (object? sender, CancelEventArgs e) {
      if (!mIsSaved && mEditor.Dwg.Plines.Count > 0) {
         MessageBoxResult result = MessageBox.Show ("Do you want to save changes before closing?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
         if (result == MessageBoxResult.Cancel) e.Cancel = true;
         else if (result == MessageBoxResult.Yes) {
            if (mSavedFileName == null) SaveNew ();
            else SaveChanges (mSavedFileName);
         }
      }
   }

   Editor mEditor;
   bool mIsSaved = false;
   string? mSavedFileName;
}