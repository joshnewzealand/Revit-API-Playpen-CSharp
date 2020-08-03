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
    public class EE09_Draw3D_ModelLines : IExternalEventHandler  //is thie one used at all?  isn't this drawing a face in 3D or 3d   this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                do
                {
                    ///                             TECHNIQUE 09 OF 19
                    ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ DRAWING 3D MODEL LINES (A SIMILY FACE) ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                    ///
                    /// Interfaces and ENUM's:
                    ///     FailureHandlingOptions
                    ///     
                    /// 
                    /// Demonstrates classes:
                    ///     Arc
                    ///     Line
                    ///     Curve
                    ///     Ellipse
                    /// 
                    /// 
                    /// Key methods:
                    ///     Arc.Create(end02, end12, pointOnCurve2);
                    ///     Arc.Create(myTransform.OfPoint(new XYZ(-.6, 0.5, 0)), radius, startAngle, endAngle, xVec, yVec);
                    ///     Ellipse.CreateCurve(pickedRef.GlobalPoint, radiusEllipse, radiusEllipse2, xVec, yVec, param0, param1);
                    ///     doc.Create.NewModelCurve(
                    /// 
                    ///
                    ///
                    /// * class is actually part of the .NET framework (not Revit API)


                    Reference pickedRef = null;
                    try
                    {
                        pickedRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Face, "Please select a Face");
                    }

                    #region catch and finally
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                    }
                    #endregion

                    if (pickedRef == null) break;

                    Element myElement = doc.GetElement(pickedRef.ElementId);
                    Face myFace = myElement.GetGeometryObjectFromReference(pickedRef) as Face;

                    if (myFace == null) return;

                    Transform myXYZ_FamilyTransform = Transform.Identity;

                    if (pickedRef.ConvertToStableRepresentation(doc).Contains("INSTANCE"))
                    {
                        myXYZ_FamilyTransform = (myElement as FamilyInstance).GetTotalTransform();
                    }

                    Transform myTransform = Transform.Identity;

                    if (myFace.GetType() != typeof(PlanarFace)) continue;

                    PlanarFace myPlanarFace = myFace as PlanarFace;

                    myTransform.Origin = pickedRef.GlobalPoint;
                    myTransform.BasisX = myXYZ_FamilyTransform.OfVector(myPlanarFace.XVector);
                    myTransform.BasisY = myXYZ_FamilyTransform.OfVector(myPlanarFace.YVector);
                    myTransform.BasisZ = myXYZ_FamilyTransform.OfVector(myPlanarFace.FaceNormal);

                    // Create a geometry line
                    XYZ startPoint = new XYZ(0, 0, 0);
                    XYZ endPoint = new XYZ(10, 10, 0);

                    // Create a arc
                    XYZ origin = myTransform.OfPoint(new XYZ(0, 0, 0));
                    XYZ normal = myTransform.OfPoint(new XYZ(1, 1, 0));
                    XYZ end02 = myTransform.OfPoint((new XYZ(-.7, -.9, 0)));
                    XYZ end12 = myTransform.OfPoint((new XYZ(.7, -.9, 0)));
                    XYZ pointOnCurve2 = myTransform.OfPoint((new XYZ(0, -1.25, 0)));
                    Arc geomArc2 = Arc.Create(end02, end12, pointOnCurve2);

                    // Create a geometry circle in Revit application
                    XYZ xVec = myTransform.OfVector(XYZ.BasisX);
                    XYZ yVec = myTransform.OfVector(XYZ.BasisY);
                    double startAngle = 0;
                    double endAngle = 2 * Math.PI;
                    double radius = .3;
                    Arc geomPlane2 = Arc.Create(myTransform.OfPoint(new XYZ(-.6, 0.5, 0)), radius, startAngle, endAngle, xVec, yVec);
                    Arc geomPlane3 = Arc.Create(myTransform.OfPoint(new XYZ(.6, 0.5, 0)), radius, startAngle, endAngle, xVec, yVec);

                    ////////////////////// straight lines
                    Line L2 = Line.CreateBound(myTransform.OfPoint(new XYZ(-.2, -.5, 0)), myTransform.OfPoint(new XYZ(.2, -.5, 0)));
                    Line L3 = Line.CreateBound(myTransform.OfPoint(new XYZ(.2, -.5, 0)), myTransform.OfPoint(new XYZ(0, .1, 0)));

                    // Create a ellipse
                    double param0 = 0.0;
                    double param1 = 2 * Math.PI;
                    double radiusEllipse = 1.4;
                    double radiusEllipse2 = 1.4 * 1.2;
                    Curve myCurve_Ellipse = Ellipse.CreateCurve(pickedRef.GlobalPoint, radiusEllipse, radiusEllipse2, xVec, yVec, param0, param1);

                    using (Transaction y = new Transaction(doc, "Simily Face"))
                    {
                        y.Start();

                        FailureHandlingOptions options = y.GetFailureHandlingOptions();
                        options.SetFailuresPreprocessor(new MyPreProcessor());
                        y.SetFailureHandlingOptions(options);

                        SketchPlane sketch = SketchPlane.Create(doc, pickedRef);

                        try
                        {
                            doc.Create.NewModelCurve(geomArc2, sketch);
                            doc.Create.NewModelCurve(myCurve_Ellipse, sketch);
                            doc.Create.NewModelCurve(geomPlane2, sketch);
                            doc.Create.NewModelCurve(geomPlane3, sketch);
                            doc.Create.NewModelCurve(L2, sketch);
                            doc.Create.NewModelCurve(L3, sketch);
                        }

                        #region catch and finally
                        catch (Exception ex)
                        {
                            if (ex.Message != "Curve must be in the plane\r\nParameter name: pCurveCopy")
                            {
                                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
                            }
                        }
                        finally
                        {
                        }
                        #endregion

                        y.Commit();
                    }

                } while (true);

            }

            #region catch and finally
            catch (Exception ex)
            {
                if (ex.Message != "The user aborted the pick operation.")
                {
                    _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE09_Draw3D_ModelLines" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
                }
                else
                {
                }
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
    }
}
