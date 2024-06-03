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
      mWidget = new LineBuilder (this);
      mDoc = new (this);
      mPanWidget = new (this, OnPan);

      Loaded += delegate {
         var bound = new Bound (new Point (0, 0), new Point (100, 100));
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
         System.Windows.Point
         cornerA = mInvProjXfm.Transform (new System.Windows.Point (30, 30)),
         cornerB = mInvProjXfm.Transform (new System.Windows.Point (ActualWidth - 30, ActualHeight - 30));
         var b = new Bound (new (cornerA.X, cornerA.Y), new (cornerB.X, cornerB.Y));
         b = b.Inflated (new Point (ptDraw.X, ptDraw.Y), zoomFactor);
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

   public Matrix InvProjXfm { get => mInvProjXfm; set => mInvProjXfm = value; }

   public ToggleButton SelectedButton { get => mTogglebutton; set => mTogglebutton = value; }

   public DrawingSheet Dwg { get => mDwg; set => mDwg = value; }
   #endregion

   #region Overrides ------------------------------------------------
   protected override void OnRender (DrawingContext dc) {
      base.OnRender (dc);
      if (mWidget is null) return;
      var dwgCmd = DrawingCommands.GetInstance;
      (dwgCmd.Dc, dwgCmd.Xfm) = (dc, mXfm);
      // Draw feedback, if any
      mWidget!.Draw (dwgCmd);
      dwgCmd.Brush = Brushes.Black;
      // Draw the drawing (or drawing entities)
      mDwg!.Draw (dwgCmd);
   }
   #endregion

   #region Methods --------------------------------------------------
   public void EscKey (KeyEventArgs e) {
      if (e.Key == Key.Escape) mWidget.StopDrawing ();
      InvalidateVisual ();
   }

   public void Redo () {
      if (mUndoRedo.Count > 0) {
         mDwg.Plines.Add (mUndoRedo.Pop ());
         InvalidateVisual ();
      }
   }

   public void Undo () {
      if (!mDoc.IsSaved && mDwg.Plines.Count > 0) {
         mUndoRedo.Push (mDwg.Plines.Last ());
         mDwg.Plines.Remove (mDwg.Plines.Last ());
         InvalidateVisual ();
      }
   }

   public void SwitchWidget () {
      mWidget?.Detach ();
      if (Window != null) {
         mWidget = mTogglebutton.Name switch {
            "rect" => new RectBuilder (this),
            "conLine" => new ConLineBuilder (this),
            _ => new LineBuilder (this)
         };
         mWidget.Attach ();
      }
   }
   #endregion

   #region Implementation -------------------------------------------
   void OnPan (System.Windows.Vector panDisp) {
      Matrix m = Matrix.Identity; m.Translate (panDisp.X, panDisp.Y);
      mProjXfm.Append (m);
      mInvProjXfm = mProjXfm; mInvProjXfm.Invert ();
      mXfm = mProjXfm;
      InvalidateVisual ();
   }
   #endregion

   #region Private --------------------------------------------------
   ToggleButton mTogglebutton = new ();
   Widget mWidget;
   Stack<Pline> mUndoRedo = new ();
   DrawingSheet mDwg = new ();
   DocManager mDoc;
   readonly PanWidget mPanWidget;
   Matrix mProjXfm = Matrix.Identity, mInvProjXfm = Matrix.Identity, mXfm;
   #endregion
}
#endregion