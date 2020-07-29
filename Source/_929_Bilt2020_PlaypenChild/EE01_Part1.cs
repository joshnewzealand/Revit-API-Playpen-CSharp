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

namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE01_Part1_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
    public class EE01_Part1_ManualColorOverride : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; 

                Element myElement = null;
                if (uidoc.Selection.GetElementIds().Count == 0)
                {
                    string myString_RememberLast = uidoc.ActiveView.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION).AsString();
                    int n;
                    if (!int.TryParse(myString_RememberLast, out n)) return;
                    myElement = doc.GetElement(new ElementId(n));
                } else
                {
                    myElement = doc.GetElement(uidoc.Selection.GetElementIds().First());
                }
                if (myElement == null) return;


                ///                  TECHNIQUE 4 OF 19
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ COLOUR OVERWRITE ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Manual Color Override");

                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                    OverrideGraphicSettings ogsCheeck = doc.ActiveView.GetElementOverrides(myElement.Id);
                    FillPatternElement myFillPattern = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.Name.Contains("Solid fill"));

                    ogs.SetSurfaceBackgroundPatternId(myFillPattern.Id);
                    ogs.SetSurfaceBackgroundPatternColor(new Autodesk.Revit.DB.Color(255, 255, 0));

                    if (ogsCheeck.SurfaceBackgroundPatternId.IntegerValue != -1)
                    {
                        doc.ActiveView.SetElementOverrides(myElement.Id, new OverrideGraphicSettings());
                    }
                    else
                    {
                        doc.ActiveView.SetElementOverrides(myElement.Id, ogs);
                    }

                    uidoc.ActiveView.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION).Set(myElement.Id.IntegerValue.ToString());
                    tx.Commit();
                }
                ///↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑


                uidoc.Selection.SetElementIds(new List<ElementId>());
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1_ManualColorOverride" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE01_Part1_PlaceAFamily : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        //////////List<ElementId> _added_element_ids = new List<ElementId>();

        public Window1 myWindow1 { get; set; }
        //public string myString_NameOfFamiliy { get; set; }

        public FamilySymbol myFamilySymbol { get; set; }

        void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            try
            {
                // e.GetModifiedElementIds


                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                if (e.GetAddedElementIds().Count == 0 & e.GetModifiedElementIds().Count == 0) return;


                if (e.GetAddedElementIds().Count > 0)
                {
                    Element myElement = doc.GetElement(e.GetAddedElementIds().First());

                    if (myElement.GetType().Name != "FamilyInstance") return;
                }


                if (e.GetModifiedElementIds().Count > 0)
                {
                    Element myElement = doc.GetElement(e.GetModifiedElementIds().First());

                    if (myElement.GetType().Name != "FamilyInstance") return;
                }



                //ElementType myElementType = doc.GetElement(myElement.GetTypeId()) as ElementType;

                //if (myElementType == null) return;

                //  MessageBox.Show(myElement.GetType().Name);


                ////////_added_element_ids.AddRange(e.GetAddedElementIds());

                ////////if (e.GetAddedElementIds().Count == 0) return;


                uidoc.Application.Application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

                    //  MessageBox.Show("this works");

                    //MessageBox.Show(_added_element_ids[0].IntegerValue.ToString());

                    SetForegroundWindow(uidoc.Application.MainWindowHandle); //this is an excape event
                    keybd_event(0x1B, 0, 0, 0);
                    keybd_event(0x1B, 0, 2, 0);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("OnDocumentChanged" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        UIApplication uiapp;
        bool myBool_PassThrough = true;

        public void Execute(UIApplication uiappp)
        {
            uiapp = uiappp;

            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            uidoc.Application.Application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

            PromptForFamilyInstancePlacementOptions myPromptForFamilyInstancePlacementOptions = new PromptForFamilyInstancePlacementOptions();

            if (uidoc.ActiveView.SketchPlane.Name != "Level 1")
            {
                Level myLevel = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().First() as Level;
                using (Transaction y = new Transaction(doc, "SetDefaultPlane"))
                {
                    y.Start();
                    uidoc.ActiveView.SketchPlane = SketchPlane.Create(doc, myLevel.GetPlaneReference());
                    y.Commit();

                }
            }

            myPromptForFamilyInstancePlacementOptions.FaceBasedPlacementType = FaceBasedPlacementType.PlaceOnWorkPlane;

            try
            {
                uidoc.PromptForFamilyInstancePlacement(myFamilySymbol);
            }

            #region catch and finally
            catch (Exception ex)
            {
                if (ex.Message != "The user aborted the pick operation.")
                {
                    _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1_PlaceAFamily" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
                  
                } else
                {
                    uidoc.Application.Application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
                }
            }
            finally
            {
            }
            #endregion

            SetForegroundWindow(uidoc.Application.MainWindowHandle); //this is an excape event
        }

        public string GetName()
        {
            return "External Event Example";
        }
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE01_Part1 : IExternalEventHandler  //is thie one used at all?  isn't this drawing a face in 3D or 3d   this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                do
                {
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



                            //Reference pickedRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Face, "Please select a Face");
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

                        ////////////////////// stright lines
                        Line L2 = Line.CreateBound(myTransform.OfPoint(new XYZ(-.2, -.5, 0)), myTransform.OfPoint(new XYZ(.2, -.5, 0)));
                        Line L3 = Line.CreateBound(myTransform.OfPoint(new XYZ(.2, -.5, 0)), myTransform.OfPoint(new XYZ(0, .1, 0)));

                        // Create a ellipse
                        double param0 = 0.0;
                        double param1 = 2 * Math.PI;
                        double radiusEllipse = 1.4;
                        double radiusEllipse2 = 1.4 * 1.2;
                        Curve myCurve_Ellipse = Ellipse.CreateCurve(pickedRef.GlobalPoint, radiusEllipse, radiusEllipse2, xVec, yVec, param0, param1);
                 
                    
                    MyPreProcessor preproccessor = new MyPreProcessor();

                    using (Transaction y = new Transaction(doc, "Simily Face"))
                    {


                        y.Start();


                        FailureHandlingOptions options = y.GetFailureHandlingOptions();
                        options.SetFailuresPreprocessor(preproccessor);
                        y.SetFailureHandlingOptions(options);


                        //  SketchPlane sketch = SketchPlane.Create(doc, geomPlane);
                        SketchPlane sketch2 = SketchPlane.Create(doc, pickedRef);

                        try
                        {
                            doc.Create.NewModelCurve(geomArc2, sketch2);
                            doc.Create.NewModelCurve(myCurve_Ellipse, sketch2);
                            doc.Create.NewModelCurve(geomPlane2, sketch2);
                            doc.Create.NewModelCurve(geomPlane3, sketch2);
                            doc.Create.NewModelCurve(L2, sketch2);
                            doc.Create.NewModelCurve(L3, sketch2);
                        }

                        #region catch and finally
                        catch (Exception ex)
                        {
                            if (ex.Message != "Curve must be in the plane\r\nParameter name: pCurveCopy")
                            {
                                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1_PlaceAFamily" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
                            } else
                            {
                                //MessageBox.Show("hello");
                            }
                        }
                        finally
                        {
                        }
                        #endregion



                        // myModelCurve.SketchPlane =  (SketchPlane)myFace;

                        // sketch.
                        //myModelCurve.host






                        y.Commit();
                    }

                } while (true);

            }

            #region catch and finally
            catch (Exception ex)
            {
                if (ex.Message != "The user aborted the pick operation.")
                {
                    _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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

        ////public static Arc ArcsToCornerOfSurface(XYZ myXYZ_Origin, Plane geomPlane)
        ////{
        ////    // Create a geometry arc in Revit application
        ////    Transform myTransform = Transform.Identity;  //front
        ////                                                 //myTransform.Origin = myFace.GetEdgesAsCurveLoops()[0].ElementAt(0).GetEndPoint(0);
        ////    myTransform.Origin = myXYZ_Origin;
        ////    myTransform.BasisX = geomPlane.XVec;
        ////    myTransform.BasisY = geomPlane.YVec;
        ////    myTransform.BasisZ = geomPlane.Normal;

        ////    XYZ end0 = myTransform.OfPoint(new XYZ(-1, 0, 0));
        ////    XYZ end1 = myTransform.OfPoint(new XYZ(1, 0, 0));
        ////    XYZ pointOnCurve = myTransform.OfPoint(new XYZ(0, -1, 0));
        ////    return Arc.Create(end0, end1, pointOnCurve);
        ////}




    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE01_Part1_GridOutCirclesOnFace : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                do
                {
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

                    Transform myXYZ_FamilyTransform = Transform.Identity;

                    Face myFace = myElement.GetGeometryObjectFromReference(pickedRef) as Face;
                    if (myFace == null) continue;

                    XYZ myXYZ_Normal = myFace.ComputeNormal(UV.Zero);
                    XYZ myXYZ_CornerOne = myFace.Evaluate(myFace.GetBoundingBox().Min);
                    XYZ myXYZ_CornerTwo = myFace.Evaluate(myFace.GetBoundingBox().Max);

                    if (pickedRef.ConvertToStableRepresentation(doc).Contains("INSTANCE"))
                    {
                        myXYZ_FamilyTransform = (myElement as FamilyInstance).GetTotalTransform();

                        //computedFaceNormal = trans.OfVector(computedFaceNormal);
                        myXYZ_Normal = myXYZ_FamilyTransform.BasisZ;
                        if (myElement.Category.Name == "Doors") myXYZ_Normal = myXYZ_FamilyTransform.BasisY;
                        if (myElement.Category.Name == "Windows") myXYZ_Normal = -myXYZ_FamilyTransform.BasisY;
                        if (myElement.Category.Name == "Curtain Panels") myXYZ_Normal = -myXYZ_FamilyTransform.BasisY;

                        myXYZ_CornerOne = new XYZ(myFace.GetBoundingBox().Min.U, myFace.GetBoundingBox().Min.V, 0);
                        myXYZ_CornerTwo = new XYZ(myFace.GetBoundingBox().Max.U, myFace.GetBoundingBox().Max.V, 0);
                    }

                    // Create a geometry plane & sketch plane in Revit application
                    //XYZ origin = new XYZ(0, 0, 0);
                    //XYZ normal = new XYZ(1, 1, 0);
                    //Plane geomPlane = Plane.CreateByNormalAndOrigin(myXYZ_FamilyTransform.OfPoint(myFace.GetEdgesAsCurveLoops()[0].First().GetEndPoint(0)), myFace.Evaluate(myXYZ_FamilyTransform.OfPoint(myFace.GetEdgesAsCurveLoops()[0].uv.First().GetEndPoint(1)));
                    //Plane geomPlane = Plane.CreateByNormalAndOrigin(myXYZ_FamilyTransform.OfPoint(myFace.GetEdgesAsCurveLoops()[0].First().GetEndPoint(0)), myXYZ_FamilyTransform.OfPoint(myFace.GetEdgesAsCurveLoops()[0].First().GetEndPoint(1)));
                    //Plane geomPlane = Plane.CreateByNormalAndOrigin(myXYZ_FamilyTransform.OfPoint(myFace.ComputeNormal(t)), myXYZ_FamilyTransform.OfPoint(myFace.Evaluate(myFace.GetBoundingBox().Max)));
                    Plane geomPlane = Plane.CreateByNormalAndOrigin(myXYZ_Normal, myXYZ_FamilyTransform.OfPoint(myFace.Evaluate(UV.Zero)));

                    // Create a XYZ face corners using GetEdgesAsCurveLoops

                    // Create a geometry arc in Revit application
                    Transform myTransform = Transform.Identity;  //front
                    myTransform.Origin = geomPlane.Origin;
                    myTransform.BasisX = geomPlane.XVec;
                    myTransform.BasisY = geomPlane.YVec;
                    myTransform.BasisZ = geomPlane.Normal;


                    if (pickedRef.ConvertToStableRepresentation(doc).Contains("INSTANCE"))
                    {
                        myXYZ_CornerOne = myTransform.OfPoint(myXYZ_CornerOne);
                        myXYZ_CornerTwo = myTransform.OfPoint(myXYZ_CornerTwo);
                    }
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Draw Grid of Circles On Face");
                        SketchPlane sketch = SketchPlane.Create(doc, geomPlane);


                        XYZ myXYZ_Differernce = (myXYZ_CornerOne - myXYZ_CornerTwo) / 4;
                        if (myElement.Category.Name == "Doors") myXYZ_Differernce = -myXYZ_Differernce;
                        // if (myElement.Category.Name == "Furniture") myXYZ_Differernce = -myXYZ_Differernce;

                        // Create a geometry circle in Revit application
                        double startAngle = 0;
                        double endAngle = 2 * Math.PI;
                        double radius = myXYZ_Differernce.DistanceTo(myXYZ_Differernce * 2) / 4;

                        for (int int_Outer = 1; int_Outer <= 3; int_Outer++)
                        {
                            XYZ myXYZ_InversePassFirst = myTransform.Inverse.OfPoint(myXYZ_CornerOne - (myXYZ_Differernce * int_Outer));

                            for (int int_Inner = 1; int_Inner <= 3; int_Inner++)
                            {
                                XYZ myXYZ_InversePassSecond = myTransform.Inverse.OfPoint(myXYZ_CornerOne - (myXYZ_Differernce * int_Inner));

                                XYZ myCenter = new XYZ(myXYZ_InversePassFirst.X, myXYZ_InversePassSecond.Y, 0);

                                //   myCenter = myCenter + myXYZ_FamilyPostiion;

                                if (true)
                                {
                                    Arc geomPlane3 = Arc.Create(myTransform.OfPoint(myCenter), radius, startAngle, endAngle, myTransform.BasisX, myTransform.BasisY);
                                    doc.Create.NewModelCurve(geomPlane3, sketch);
                                }
                            }
                        }

                        tx.Commit();
                    }

                } while (true);
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1_GridOutCirclesOnFace" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
