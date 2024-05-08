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
      mPanWidget = new (this, OnPan);

      Loaded += delegate {
         var bound = new Bound (new Point (-10, -10), new Point (1000, 1000));
         mProjXfm = Util.ComputeZoomExtentsProjXfm (ActualWidth, ActualHeight, bound);
         mInvProjXfm = mProjXfm; mInvProjXfm.Invert ();
         mXfm = mProjXfm;
      };

      MouseRightButtonDown += delegate { // Carry out zoom extents, on right mouse down!
         mProjXfm = Util.ComputeZoomExtentsProjXfm (ActualWidth, ActualHeight, mDwg.Bound);
         mInvProjXfm = mProjXfm; mInvProjXfm.Invert ();
         mXfm = mProjXfm;
         InvalidateVisual ();
      };
      
      MouseWheel += (sender, e) => {
         double zoomFactor = 1.05;
         if (e.Delta > 0) zoomFactor = 1 / zoomFactor;
         var ptDraw = mInvProjXfm.Transform (e.GetPosition (this)); // mouse point in drawing space
         // Actual visible drawing area
         Point cornerA, cornerB;
         System.Windows.Point xfmA,xfmB;
         xfmA= mInvProjXfm.Transform (new System.Windows.Point (20, 20));
         xfmB = mInvProjXfm.Transform (new System.Windows.Point (ActualWidth, ActualHeight));
         (cornerA.X,cornerA.Y,cornerB.X,cornerB.Y)=(xfmA.X,xfmA.Y,xfmB.X,xfmB.Y);
         var b = new Bound (cornerA, cornerB);
         b = b.Inflated (new Point(ptDraw.X,ptDraw.Y), zoomFactor);
         mProjXfm = Util.ComputeZoomExtentsProjXfm (ActualWidth, ActualHeight, b);
         mInvProjXfm = mProjXfm; mInvProjXfm.Invert ();
         mXfm = mProjXfm;
         InvalidateVisual ();
      };
   }
   #endregion

   #region Properties -----------------------------------------------
   public MainWindow? Window { get; set; }

   public DocManager DocMgr => mDoc;

   public Matrix InvProjXfm=> mInvProjXfm;

   public ToggleButton SelectedButton {
      get => mTogglebutton;
      set => mTogglebutton = value;
   }

   public DrawingSheet Dwg {
      get => mDwg;
      set => mDwg = value;
   }

   public Shape Shape {
      get => mShape!;
      set => mShape = value;
   }
   #endregion

   #region Overrides ------------------------------------------------
   protected override void OnRender (DrawingContext dc) {
      base.OnRender (dc);
      foreach (var shape in mDwg.Shapes) {
         shape.Draw (new Draw (mPen, dc,mXfm));
      }
      if (mShape != null && mShape.Points.Count > 0) mShape.Draw (new Draw (mFeedback, dc,mXfm));
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
            "conLine" => new ConLineWidget (this),
            _ => new LineWidget (this)
         };
         mWidget.Attach ();
      }
   }
   #endregion

   void OnPan (System.Windows.Vector panDisp) {
      Matrix m = Matrix.Identity; m.Translate (panDisp.X, panDisp.Y);
      mProjXfm.Append (m);
      mInvProjXfm = mProjXfm; mInvProjXfm.Invert ();
      mXfm = mProjXfm;
      InvalidateVisual ();
   }

   #region Private --------------------------------------------------
   ToggleButton mTogglebutton = new ();
   Shape? mShape;
   Widget mWidget;
   Stack<Shape> mUndoRedo = new ();
   Pen mFeedback = new (Brushes.Red, 1);
   Pen mPen = new (Brushes.Black, 1);
   DrawingSheet mDwg = new ();
   DocManager mDoc;
   PanWidget mPanWidget;
   Matrix mProjXfm = Matrix.Identity, mInvProjXfm = Matrix.Identity,mXfm;
   #endregion
}
#endregion