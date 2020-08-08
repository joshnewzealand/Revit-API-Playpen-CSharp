using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB.Events;
using System.Runtime.InteropServices;
using _952_PRLoogleClassLibrary;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace _929_Bilt2020_PlaypenChild
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE10_Draw2D_DetailLines : IExternalEventHandler
    {
        public MainWindow myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);


                if (doc.ActiveView.ViewType != ViewType.AreaPlan & doc.ActiveView.ViewType != ViewType.FloorPlan & doc.ActiveView.ViewType != ViewType.CeilingPlan & doc.ActiveView.ViewType != ViewType.DraftingView & doc.ActiveView.ViewType != ViewType.Detail & doc.ActiveView.ViewType != ViewType.DrawingSheet & doc.ActiveView.ViewType != ViewType.EngineeringPlan & doc.ActiveView.ViewType != ViewType.Legend)
                {
                    TaskDialog.Show("Plugin stopped", "Active view is not 2D and will not receive Detail Lines");
                    return;
                }


                UIView uiview = null;


                IList<UIView> uiviews = uidoc.GetOpenUIViews();

                foreach (UIView uv in uiviews)
                {
                    if (uv.ViewId.Equals(doc.ActiveView.Id))
                    {
                        uiview = uv;
                        break;
                    }
                }

                ///                TECHNIQUE 10 OF 19 (EE10_Draw2D_DetailLines.cs)
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ DRAWING 2D DETAIL LINES (A SIMILY FACE) ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                ///
                /// Interfaces and ENUM's:
                ///     
                /// 
                /// Demonstrates classes:
                ///     UIDocument
                /// 
                /// 
                /// Key methods:
                ///     uiview.GetZoomCorners(
                ///     doc.Create.NewDetailCurve(
                /// 
                ///
                ///
                /// * class is actually part of the .NET framework (not Revit API)
				///	https://github.com/joshnewzealand/Revit-API-Playpen-CSharp



                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Drawing Detail Lines");

                    XYZ myXYZ_Corner1 = uiview.GetZoomCorners()[0];
                    XYZ myXYZ_Corner2 = uiview.GetZoomCorners()[1];

                    XYZ myXYZ_Centre = uiview.GetZoomCorners()[0] + ((uiview.GetZoomCorners()[1] - uiview.GetZoomCorners()[0]) / 2);


                    // Create a geometry line
                    XYZ startPoint = new XYZ(0, 0, 0);
                    XYZ endPoint = new XYZ(10, 10, 0);

                    // Create a ellipse
                    XYZ origin = new XYZ(0, 0, 0);
                    XYZ normal = new XYZ(1, 1, 0);
                    XYZ end02 = myXYZ_Centre + (new XYZ(-.7, -.9, 0));
                    XYZ end12 = myXYZ_Centre + (new XYZ(.7, -.9, 0));
                    XYZ pointOnCurve2 = myXYZ_Centre + (new XYZ(0, -1.25, 0));
                    Arc geomArc2 = Arc.Create(end02, end12, pointOnCurve2);

                    // Create a geometry circle in Revit application
                    XYZ xVec = XYZ.BasisX;
                    XYZ yVec = XYZ.BasisY;
                    double startAngle = 0;
                    double endAngle = 2 * Math.PI;
                    double radius = .3;
                    Arc geomPlane2 = Arc.Create(myXYZ_Centre + new XYZ(-.6, 0.5, 0), radius, startAngle, endAngle, xVec, yVec);
                    Arc geomPlane3 = Arc.Create(myXYZ_Centre + new XYZ(.6, 0.5, 0), radius, startAngle, endAngle, xVec, yVec);

                    // stright lines
                    Line L2 = Line.CreateBound(myXYZ_Centre + new XYZ(-.2, -.5, 0), myXYZ_Centre + new XYZ(.2, -.5, 0));
                    Line L3 = Line.CreateBound(myXYZ_Centre + new XYZ(.2, -.5, 0), myXYZ_Centre + new XYZ(0, .1, 0));

                    double param0 = 0.0;
                    double param1 = 2 * Math.PI;
                    double radiusEllipse = 1.4;
                    double radiusEllipse2 = 1.4 * 1.2;
                    Curve myCurve_Ellipse = Ellipse.CreateCurve(myXYZ_Centre, radiusEllipse, radiusEllipse2, xVec, yVec, param0, param1);

                    doc.Create.NewDetailCurve(doc.ActiveView, geomArc2);
                    doc.Create.NewDetailCurve(doc.ActiveView, myCurve_Ellipse);
                    doc.Create.NewDetailCurve(doc.ActiveView, geomPlane2);
                    doc.Create.NewDetailCurve(doc.ActiveView, geomPlane3);
                    doc.Create.NewDetailCurve(doc.ActiveView, L2);
                    doc.Create.NewDetailCurve(doc.ActiveView, L3);

                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE10_Draw2D_DetailLines" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        public string GetName()
        {
            return "External Event Example";
        }
    }
}
