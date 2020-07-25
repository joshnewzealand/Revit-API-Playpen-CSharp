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
    public class EE05_Part1_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE05_Part1_Template" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE05_Part1 : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

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

                // We already know the transaction name.

                foreach (FailureMessageAccessor fma in fmas)
                {
                    FailureSeverity fseverity = fma.GetSeverity();

                    // ResolveFailure mimics clicking 
                    // 'Remove Link' button             .
                    if (fseverity == FailureSeverity.Warning) failuresAccessor.DeleteWarning(fma);

                    //failuresAccessor.ResolveFailure(fma);
                    // DeleteWarning mimics clicking 'Ok' button.
                    //failuresAccessor.DeleteWarning( fma );         
                }

                //return FailureProcessingResult
                //  .ProceedWithCommit;
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

                //using (Transaction tx = new Transaction(doc))
                //{
                //    tx.Start("Splatter Gun");

                    //here we are frendo
                    FamilyInstance myFamilyInstance = doc.GetElement(new ElementId(536937)) as FamilyInstance;

                    //myFamilyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Hello world");

                    ReferencePoint myReferencePoint = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance).First()) as ReferencePoint;

                    Transform myTransform_DirectlyPerform = myReferencePoint.GetCoordinateSystem();

                    ReferenceIntersector refIntersector = new ReferenceIntersector(doc.ActiveView as View3D);

                using (TransactionGroup transGroup = new TransactionGroup(doc))
                {
                    transGroup.Start("Transaction Group");

                    MyPreProcessor preproccessor = new MyPreProcessor();

                    for (int i = 1; i <= 100; i++)
                    {

                        if (i > 100)
                        {
                            MessageBox.Show("Stopped at 'i > 100' because more will cause Revit to 'freeze up'.");
                            break;
                        }

                        using (Transaction tx = new Transaction(doc))
                        {
                            FailureHandlingOptions options = tx.GetFailureHandlingOptions();
                            options.SetFailuresPreprocessor(preproccessor);
                            tx.SetFailureHandlingOptions(options);

                            tx.Start("Splatter Gun");

                            Line myLine_BasisX = Line.CreateUnbound(myTransform_DirectlyPerform.Origin, myTransform_DirectlyPerform.BasisX);
                            myReferencePoint.Location.Rotate(myLine_BasisX, GetRandomNumber());
                            myTransform_DirectlyPerform = myReferencePoint.GetCoordinateSystem();

                            Line myLine_BasisY = Line.CreateUnbound(myTransform_DirectlyPerform.Origin, myTransform_DirectlyPerform.BasisY);
                            myReferencePoint.Location.Rotate(myLine_BasisY, GetRandomNumber());
                            myTransform_DirectlyPerform = myReferencePoint.GetCoordinateSystem();

                            //Line myLine_BasisZ = Line.CreateUnbound(myTransform_DirectlyPerform.Origin, myTransform_DirectlyPerform.BasisZ);
                            //myReferencePoint.Location.Rotate(myLine_BasisZ, GetRandomNumber());
                            //myTransform_DirectlyPerform = myReferencePoint.GetCoordinateSystem();

                            XYZ myXYZ_DirectlyOffset = myTransform_DirectlyPerform.OfVector(new XYZ(0, 0, 3));
                            XYZ myXYZ_HeightScan_100Below_Transform = myTransform_DirectlyPerform.Origin + myXYZ_DirectlyOffset;

                            Reference myReferenceHosting_Normal = refIntersector.FindNearest(myXYZ_HeightScan_100Below_Transform, myTransform_DirectlyPerform.BasisZ).GetReference();

                            //if (myReferenceHosting_Normal.ElementId != null)
                            //{
                                Element myElement = doc.GetElement(myReferenceHosting_Normal.ElementId);

                            if (myElement.GetGeometryObjectFromReference(myReferenceHosting_Normal).GetType() != typeof(PlanarFace)) continue;

                            Face myFace = myElement.GetGeometryObjectFromReference(myReferenceHosting_Normal) as Face;

                            // Create a geometry plane in Revit application
                            XYZ origin = new XYZ(0, 0, 0);
                            XYZ normal = new XYZ(1, 1, 0);
                            Plane geomPlane = Plane.CreateByNormalAndOrigin(myFace.ComputeNormal(myReferenceHosting_Normal.UVPoint), myReferenceHosting_Normal.GlobalPoint);

                            // Create a geometry circle in Revit application
                            double startAngle = 0;
                            double endAngle = 2 * Math.PI;
                            double radius = 1.23;
                            Arc geomPlane2 = Arc.Create(geomPlane, radius, startAngle, endAngle);

                            SketchPlane sketch = SketchPlane.Create(doc, geomPlane); ;

                            ModelArc arc = doc.Create.NewModelCurve(geomPlane2, sketch) as ModelArc;

                            //XYZ norm = v.CrossProduct(w).Normalize();
                            Plane plane = Plane.CreateByNormalAndOrigin(myTransform_DirectlyPerform.BasisX, myTransform_DirectlyPerform.Origin); // 2017
                            SketchPlane sketchPlane = SketchPlane.Create(doc, plane); // 2014
                            Line line = Line.CreateBound(myTransform_DirectlyPerform.Origin, myReferenceHosting_Normal.GlobalPoint); // 2014

                            //doc.Create.NewModelCurve(line, sketchPlane);

                            ModelLine myModelLine = doc.Create.NewModelCurve(line, sketchPlane) as ModelLine;

                            //ModelLine myModelLine = CreateModelLine(doc, myTransform_DirectlyPerform.Origin, myReferenceHosting_Normal.GlobalPoint);

                            //doc.Regenerate();
                            //}

                            tx.Commit();

                            uidoc.RefreshActiveView();

                            //MessageBox.Show("hello world");
                            //fight joshua, it is creating the circle offset, 
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
