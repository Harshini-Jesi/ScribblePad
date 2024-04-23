using CADye.lib;
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
   public Editor () => mWidget = new LineWidget (this);
   #endregion

   #region Properties -----------------------------------------------
   public MainWindow Window { get; set; }

   public ToggleButton SelectedButton {
      get => mTogglebutton;
      set => mTogglebutton = value;
   }

   public List<Shapes> ShapesList {
      get => mShapesList;
      set => mShapesList = value;
   }

   public Shapes Shape {
      get => mShape;
      set => mShape = value;
   }
   #endregion

   #region Overrides ------------------------------------------------
   protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e) => mWidget.OnMouseDown (e);

   protected override void OnMouseMove (MouseEventArgs e) => mWidget.OnMouseMove (e);

   protected override void OnRender (DrawingContext dc) {
      base.OnRender (dc);
      foreach (var shape in mShapesList) {
         mDwg.Draw (dc, shape as dynamic);
      }
      if (mShape != null && mShape.pointList.Count > 0) mDwg.Draw (dc, mShape as dynamic);
   }
   #endregion

   #region Methods --------------------------------------------------
   public void Redo () {
      if (mUndoRedo.Count > 0) {
         mShapesList.Add (mUndoRedo.Pop ());
         InvalidateVisual ();
      }
   }

   public void Undo () {
      mShape = null;
      if (mShapesList.Count > 0) {
         mUndoRedo.Push (mShapesList.Last ());
         mShapesList.Remove (mShapesList.Last ());
         InvalidateVisual ();
      }
   }

   public void SwitchWidget () {
      if (mWidget == null) return;
      mWidget = mTogglebutton.Name switch {
         "rect" => new RectWidget (this),
         "circle" => new CircleWidget (this),
         _ => new LineWidget (this)
      };
   }
   #endregion

   #region Private --------------------------------------------------
   ToggleButton mTogglebutton = new ();
   List<Shapes> mShapesList = new ();
   Shapes mShape;
   Widget mWidget;
   Drawing mDwg = new ();
   Stack<Shapes> mUndoRedo = new ();
   #endregion
}
#endregion