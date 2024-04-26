using CADye.Lib;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CADye;

#region class Editor ------------------------------------------------------------------------------
public class Editor : Canvas {
   #region Constructor ----------------------------------------------
   public Editor () {
      mWidget = new LineWidget (this);
      mDoc = new (this);
   }
   #endregion

   #region Properties -----------------------------------------------
   public MainWindow Window { get; set; }

   public DocManager DocMgr => mDoc;

   public ToggleButton SelectedButton {
      get => mTogglebutton;
      set => mTogglebutton = value;
   }

   public DrawingSheet Dwg {
      get => mDwg;
      set => mDwg = value;
   }

   public Shape Shape {
      get => mShape;
      set => mShape = value;
   }
   #endregion

   #region Overrides ------------------------------------------------
   protected override void OnRender (DrawingContext dc) {
      base.OnRender (dc);
      foreach (var shape in mDwg.Shapes) {
         shape.Draw (new Draw (mPen, dc));
      }
      if (mShape != null && mShape.Points.Count > 0) mShape.Draw (new Draw (mFeedback, dc));
   }
   #endregion

   #region Methods --------------------------------------------------
   public void EscKey (KeyEventArgs e) {
      mWidget.StopDrawing (e);
      InvalidateVisual ();
   }

   public void Redo () {
      if (mUndoRedo.Count > 0) {
         mDwg.Shapes.Add (mUndoRedo.Pop ());
         InvalidateVisual ();
      }
   }

   public void Undo () {
      if (!mDoc.IsSaved && mDwg.Shapes.Count > 0) {
         mUndoRedo.Push (mDwg.Shapes.Last ());
         mDwg.Shapes.Remove (mDwg.Shapes.Last ());
         InvalidateVisual ();
      }
   }

   public void SwitchWidget () {
      mWidget?.Detach ();
      if (Window != null) {
         mWidget = mTogglebutton.Name switch {
            "rect" => new RectWidget (this),
            "circle" => new CircleWidget (this),
            "conLine" => new ConLineWidget (this),
            _ => new LineWidget (this)
         };
         mWidget.Attach ();
      }
   }
   #endregion

   #region Private --------------------------------------------------
   ToggleButton mTogglebutton = new ();
   Shape mShape;
   Widget mWidget;
   Stack<Shape> mUndoRedo = new ();
   Pen mFeedback = new (Brushes.Red, 1);
   Pen mPen = new (Brushes.Black, 1);
   DrawingSheet mDwg = new ();
   DocManager mDoc;
   #endregion
}
#endregion