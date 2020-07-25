using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE04_Part1_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Appropriate descriptor");

                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE04_Part2_2DShapes : IExternalEventHandler 
    {
        public Window1 myWindow1 { get; set; }

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



                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Draw some 2D shapes.");

                    ////////////   uidoc.ActiveView.curr

                    XYZ myXYZ_Corner1 = uiview.GetZoomCorners()[0];
                    XYZ myXYZ_Corner2 = uiview.GetZoomCorners()[1];

                    XYZ myXYZ_Centre = uiview.GetZoomCorners()[0] + ((uiview.GetZoomCorners()[1] - uiview.GetZoomCorners()[0])/2);


                    // Create a geometry line
                    XYZ startPoint = new XYZ(0, 0, 0);
                    XYZ endPoint = new XYZ(10, 10, 0);

                    ////////// Create a geometry arc
                    ////////XYZ end0 = new XYZ(1, 0, 0);
                    ////////XYZ end1 = new XYZ(10, 10, 10);
                    ////////XYZ pointOnCurve = new XYZ(10, 0, 0);
                    ////////Arc geomArc = Arc.Create(end0, end1, pointOnCurve);

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
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE04_Part2_2DShapes" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE04_Part1 : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Rotate Man");

                    FamilyInstance myFamilyInstance = doc.GetElement(new ElementId(536235)) as FamilyInstance;

                    XYZ myXYZInstance = ((LocationPoint)myFamilyInstance.Location).Point;

                    Line myLineBasisY = Line.CreateUnbound(myXYZInstance, myFamilyInstance.GetTransform().BasisZ); //  tf_01072_4 = Transform.CreateRotation(myTransform_FakeBasis2.BasisZ, d1 / 10 * myInt_DivideInteger);

                    ElementTransformUtils.RotateElement(doc, myFamilyInstance.Id, myLineBasisY, Math.PI / 4);

                    tx.Commit();
                }


                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Rotate Wall");

                    Wall myWall = doc.GetElement(new ElementId(528428)) as Wall;
                    FamilyInstance myFamilyInstance = doc.GetElement(new ElementId(536235)) as FamilyInstance;

                    Curve myCurve = ((LocationCurve)myWall.Location).Curve;

                    //.WhereElementIsNotElementType().Where(x => x.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString() == "03 - Floor").Where(x => ((LocationPoint)x.Location).Point.X > ((LocationCurve)myWall.Location).Curve.GetEndPoint(0).X).Where(x => ((LocationPoint)x.Location).Point.Y < ((LocationCurve)myWall.Location).Curve.GetEndPoint(0).Y).ToList();
                    XYZ myXYZ = (myCurve.GetEndPoint(1) + myCurve.GetEndPoint(0)) / 2;
                    XYZ myXYZInstance =  ((LocationPoint)myFamilyInstance.Location).Point;

                    Line myLineBasisZ = Line.CreateUnbound(myXYZ, XYZ.BasisZ);
                    Line myLineBasisY = Line.CreateUnbound(myXYZInstance, myFamilyInstance.GetTransform().BasisZ); //  tf_01072_4 = Transform.CreateRotation(myTransform_FakeBasis2.BasisZ, d1 / 10 * myInt_DivideInteger);

                    ElementTransformUtils.RotateElement(doc, myWall.Id, myLineBasisZ, Math.PI / 20);
                    //ElementTransformUtils.RotateElement(doc, myFamilyInstance.Id, myLineBasisY, Math.PI / 4);

                    if(false)
                    {
                        WallType myWallType = doc.GetElement(myWall.GetTypeId()) as WallType;

                        CompoundStructure myCompoundStructure = myWallType.GetCompoundStructure();

                        CompoundStructureLayer compoundStructureLayer = myCompoundStructure.GetLayers()[0];

                        compoundStructureLayer.Width = compoundStructureLayer.Width + 0.05;

                        myCompoundStructure.SetLayerWidth(0, compoundStructureLayer.Width);
                        //myCompoundStructure.SetLayer(compoundStructureLayer.LayerId, compoundStructureLayer);
                        myWallType.SetCompoundStructure(myCompoundStructure);
                    }

                    tx.Commit();
                }

            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE04_Part1" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
