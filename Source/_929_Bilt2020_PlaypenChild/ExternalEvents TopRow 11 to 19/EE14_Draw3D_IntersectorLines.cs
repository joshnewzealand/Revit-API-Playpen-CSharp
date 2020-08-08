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


namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE14_Draw3D_IntersectorLines_Delete : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Remove Intersector Ray Lines");

                    doc.Delete(myWindow1.myListElementID_SketchPlanesToDelete);

                    myWindow1.myListElementID_SketchPlanesToDelete.Clear();

                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE14_Draw3D_IntersectorLines_Delete" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE14_Draw3D_IntersectorLines : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }

        public double GetRandomNumber()
        {
            Random random = new Random();
            return random.NextDouble() * ((Math.PI * 2) - 0) + 0;
        }

        public class MyPreProcessor : IFailuresPreprocessor
        {
            FailureProcessingResult IFailuresPreprocessor.PreprocessFailures(FailuresAccessor failuresAccessor)
            {
                String transactionName = failuresAccessor.GetTransactionName();

                IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();

                if (fmas.Count == 0) return FailureProcessingResult.Continue;

                foreach (FailureMessageAccessor fma in fmas)
                {
                    FailureSeverity fseverity = fma.GetSeverity();

                    if (fseverity == FailureSeverity.Warning) failuresAccessor.DeleteWarning(fma);
                }

                return FailureProcessingResult.Continue;
            }
        }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                if (doc.ActiveView.GetType() != typeof(View3D))
                {
                    MessageBox.Show("ActiveView is not typeof View3D.");
                    return;
                }

                FamilyInstance myFamilyInstance_NerfGun = null;
                if (uidoc.Selection.GetElementIds().Count == 0)
                {
                    string myString_RememberLast = doc.ProjectInformation.get_Parameter(BuiltInParameter.PROJECT_NUMBER).AsString();
                    int n;
                    if (int.TryParse(myString_RememberLast, out n)) myFamilyInstance_NerfGun = doc.GetElement(new ElementId(n)) as FamilyInstance;
                }
                else
                {
                    myFamilyInstance_NerfGun = doc.GetElement(uidoc.Selection.GetElementIds().First()) as FamilyInstance;
                }


                if (myFamilyInstance_NerfGun == null)
                {
                    MessageBox.Show("Please perform step 5 of 19 first." + Environment.NewLine + Environment.NewLine + "(Placing Nerf Gun)");
                    return;
                }

                ///            TECHNIQUE 14 OF 19 (EE14_Draw3D_IntersectorLines.cs)
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ DRAW INTERSECTOR LINES FROM NERF GUN ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                ///
                /// Interfaces and ENUM's:
                ///     BuiltInParameter.POINT_ELEMENT_DRIVEN
                ///     IFailuresPreprocessor
                ///     
                /// 
                /// Demonstrates classes:
                ///     ReferencePoint
                ///     AdaptiveComponentInstanceUtils
                ///     Transform
                ///     TransactionGroup
                ///     Random*
                ///     ElementMulticategoryFilter
                ///     ReferenceIntersector
                /// 
                /// 
                /// Key methods:
                ///     AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(
                ///     myReferencePoint.GetCoordinateSystem(
                ///     refIntersector.FindNearest(
                ///     myReferenceWithContext.GetReference(
                ///     uidoc.RefreshActiveView();
                /// 
                ///
                ///
                /// * class is actually part of the .NET framework (not Revit API)
                ///
                ///
				///	https://github.com/joshnewzealand/Revit-API-Playpen-CSharp

                uidoc.Selection.SetElementIds(new List<ElementId>());

                ReferencePoint myReferencePoint = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_NerfGun).First()) as ReferencePoint;

                Transform myTransform_FromNurfGun = myReferencePoint.GetCoordinateSystem();


                using (TransactionGroup transGroup = new TransactionGroup(doc))
                {
                    transGroup.Start("Transaction Group");

                    if (!myReferencePoint.get_Parameter(BuiltInParameter.POINT_ELEMENT_DRIVEN).IsReadOnly)
                    {
                        using (Transaction tx = new Transaction(doc))
                        {
                            tx.Start("Unhost");

                            myReferencePoint.get_Parameter(BuiltInParameter.POINT_ELEMENT_DRIVEN).Set(0);

                            tx.Commit();
                            uidoc.RefreshActiveView();
                        }
                    }

                    MyPreProcessor preproccessor = new MyPreProcessor();

                    for (int i = 1; i <= 100; i++)
                    {

                        if (i > 100)
                        {
                            MessageBox.Show("Stopped at 'i > 100' because more will cause Revit to 'freeze up'.");
                            break;
                        }

                        Random rnd = new Random();
                        OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                        ogs.SetProjectionLineColor(new Autodesk.Revit.DB.Color((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)));

                        using (Transaction tx = new Transaction(doc))
                        {
                            FailureHandlingOptions options = tx.GetFailureHandlingOptions();
                            options.SetFailuresPreprocessor(preproccessor);
                            tx.SetFailureHandlingOptions(options);

                            tx.Start("Splatter Gun");

                            Line myLine_BasisX = Line.CreateUnbound(myTransform_FromNurfGun.Origin, myTransform_FromNurfGun.BasisX);
                            myReferencePoint.Location.Rotate(myLine_BasisX, GetRandomNumber());

                            Line myLine_BasisZ = Line.CreateUnbound(myTransform_FromNurfGun.Origin, myTransform_FromNurfGun.BasisZ);
                            myReferencePoint.Location.Rotate(myLine_BasisZ, GetRandomNumber());

                            myTransform_FromNurfGun = myReferencePoint.GetCoordinateSystem();

                            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
                            builtInCats.Add(BuiltInCategory.OST_Roofs);
                            builtInCats.Add(BuiltInCategory.OST_Ceilings);
                            builtInCats.Add(BuiltInCategory.OST_Floors);
                            builtInCats.Add(BuiltInCategory.OST_Walls);
                            builtInCats.Add(BuiltInCategory.OST_Doors);
                            builtInCats.Add(BuiltInCategory.OST_Windows);
                            builtInCats.Add(BuiltInCategory.OST_CurtainWallPanels);
                            builtInCats.Add(BuiltInCategory.OST_CurtainWallMullions);

                            ElementMulticategoryFilter intersectFilter = new ElementMulticategoryFilter(builtInCats);
                            ReferenceIntersector refIntersector = new ReferenceIntersector(intersectFilter, FindReferenceTarget.Face, doc.ActiveView as View3D);

                            ReferenceWithContext myReferenceWithContext = refIntersector.FindNearest(myTransform_FromNurfGun.Origin, myTransform_FromNurfGun.BasisZ);

                            if (myReferenceWithContext != null)
                            {
                                Reference myReferenceHosting_Normal = myReferenceWithContext.GetReference();

                                Element myElement_ContainingFace = doc.GetElement(myReferenceHosting_Normal.ElementId);
                                Face myFace = myElement_ContainingFace.GetGeometryObjectFromReference(myReferenceHosting_Normal) as Face;
                                if (myFace.GetType() != typeof(PlanarFace)) return;// continue;

                                Plane plane = Plane.CreateByNormalAndOrigin(myTransform_FromNurfGun.BasisX, myTransform_FromNurfGun.Origin);
                                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

                                if (myTransform_FromNurfGun.Origin.DistanceTo(myReferenceHosting_Normal.GlobalPoint) > 0.0026)//minimum lenth check
                                {

                                    Line line = Line.CreateBound(myTransform_FromNurfGun.Origin, myReferenceHosting_Normal.GlobalPoint);

                                    ModelLine myModelLine = doc.Create.NewModelCurve(line, sketchPlane) as ModelLine;

                                    doc.ActiveView.SetElementOverrides(myModelLine.Id, ogs);

                                    myWindow1.myListElementID_SketchPlanesToDelete.Add(sketchPlane.Id);

                                    Transform myXYZ_FamilyTransform = Transform.Identity;

                                    if (myReferenceHosting_Normal.ConvertToStableRepresentation(doc).Contains("INSTANCE"))
                                    {
                                        myXYZ_FamilyTransform = (myElement_ContainingFace as FamilyInstance).GetTotalTransform();
                                    }

                                    PlanarFace myPlanarFace = myFace as PlanarFace;

                                    Transform myTransform = Transform.Identity;
                                    myTransform.Origin = myReferenceHosting_Normal.GlobalPoint;
                                    myTransform.BasisX = myXYZ_FamilyTransform.OfVector(myPlanarFace.XVector);
                                    myTransform.BasisY = myXYZ_FamilyTransform.OfVector(myPlanarFace.YVector);
                                    myTransform.BasisZ = myXYZ_FamilyTransform.OfVector(myPlanarFace.FaceNormal);

                                    SketchPlane mySketchPlane = SketchPlane.Create(doc, myReferenceHosting_Normal);

                                    // Create a geometry circle in Revit application
                                    XYZ xVec = myTransform.OfVector(XYZ.BasisX);
                                    XYZ yVec = myTransform.OfVector(XYZ.BasisY);
                                    double startAngle2 = 0;
                                    double endAngle2 = 2 * Math.PI;
                                    double radius2 = 1.23;
                                    Arc geomPlane3 = Arc.Create(myTransform.OfPoint(new XYZ(0, 0, 0)), radius2, startAngle2, endAngle2, xVec, yVec);

                                    ModelArc arc = doc.Create.NewModelCurve(geomPlane3, mySketchPlane) as ModelArc;
                                    //doc.Delete(sketch2.Id);

                                    doc.ActiveView.SetElementOverrides(arc.Id, ogs);
                                }
                            }
                            tx.Commit();
                            uidoc.RefreshActiveView();
                        }
                    }
                    transGroup.Assimilate();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE05_Part1" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
