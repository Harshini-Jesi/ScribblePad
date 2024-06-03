using CADye.Lib;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace CADye;
#region class DrawableBase ------------------------------------------------------------------------
public abstract class DrawableBase {
   public abstract void Draw (DrawingCommands drawingCommands);
}
#endregion

#region class DrawingSheet ------------------------------------------------------------------------
public class DrawingSheet : DrawableBase {
   #region Properties -----------------------------------------------
   public Bound Bound { get; private set; }

   public List<Pline> Plines { get => mPlines; set => mPlines = value; }
   #endregion

   #region Methods --------------------------------------------------
   public void AddPline (Pline pline) {
      mPlines.Add (pline);
      Bound = new Bound (mPlines.Select (pline => pline.Bound));
   }
   #endregion

   #region DrawableBase implementation ------------------------------
   public override void Draw (DrawingCommands drawingCommands) {
      foreach (var pline in mPlines)
         drawingCommands.DrawLines (pline.GetPoints ());
   }
   #endregion

   #region Private --------------------------------------------------
   List<Pline> mPlines = new ();
   #endregion
}
#endregion

#region class DrawingCommands ---------------------------------------------------------------------
public class DrawingCommands {
   #region Constructors ---------------------------------------------
   private DrawingCommands () { }

   public static DrawingCommands GetInstance { get { mDwgCmd ??= new DrawingCommands (); return mDwgCmd; } }
   #endregion

   #region Properties -----------------------------------------------
   public DrawingContext Dc { set => mDc = value; }
   public Matrix Xfm { get => mXfm; set => mXfm = value; }
   public Brush Brush { get => mBrush; set => mBrush = value; }
   #endregion

   #region Methods --------------------------------------------------
   public void DrawLines (IEnumerable<Point> dwgPts) {
      var itr = dwgPts.GetEnumerator ();
      if (!itr.MoveNext ()) return;
      var prevPt = itr.Current;
      while (itr.MoveNext ()) {
         DrawLine (prevPt, itr.Current);
         prevPt = itr.Current;
      }
   }

   public void DrawLine (Point startPt, Point endPt) {
      var pen = new Pen (mBrush, 1);
      mDc!.DrawLine (pen, mXfm.Transform (new System.Windows.Point (startPt.X, startPt.Y)), mXfm.Transform (new System.Windows.Point (endPt.X, endPt.Y)));
   }
   #endregion

   #region Private --------------------------------------------------
   DrawingContext? mDc;
   Matrix mXfm;
   Brush mBrush = Brushes.Black;
   static DrawingCommands? mDwgCmd = null;
   #endregion
}
#endregion