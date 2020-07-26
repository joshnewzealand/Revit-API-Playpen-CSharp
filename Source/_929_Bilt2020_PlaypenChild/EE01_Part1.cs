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

        public Window1 myWindow1 { get; set; }

        List<ElementId> _added_element_ids = new List<ElementId>();

        void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            try
            {
                _added_element_ids.AddRange(e.GetAddedElementIds());

                if (e.GetAddedElementIds().Count == 0) return;

                UIDocument uidoc = uiapp.ActiveUIDocument;
                uidoc.Application.Application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

                //MessageBox.Show(_added_element_ids[0].IntegerValue.ToString());

                SetForegroundWindow(uidoc.Application.MainWindowHandle);
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
            try
            {
                uiapp = uiappp;

                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                string myString_AdaptiveCarrier = "Women Tipping Hat Man";

                IEnumerable<Element> myIEnumerableElement = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(Family))).Where(x => x.Name == myString_AdaptiveCarrier);

                if (myIEnumerableElement.Count() == 0)
                {
                    MessageBox.Show(myString_AdaptiveCarrier + Environment.NewLine + Environment.NewLine + "Is not present in model");
                    return;
                }

                FamilySymbol myFamilySymbol_Carrier = doc.GetElement(((Family)myIEnumerableElement.First()).GetFamilySymbolIds().First()) as FamilySymbol;

                _added_element_ids.Clear();

                uidoc.Application.Application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

                uidoc.PromptForFamilyInstancePlacement(myFamilySymbol_Carrier);

            }

            #region catch and finally
            catch (Exception ex)
            {
                if (ex.Message != "The user aborted the pick operation.")
                {
                    _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1_PlaceAFamily" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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

                    using (Transaction y = new Transaction(doc, "Simily Face"))
                    {
                        y.Start();
                        Element myElement = doc.GetElement(pickedRef.ElementId);
                        Face myFace = myElement.GetGeometryObjectFromReference(pickedRef) as Face;
                        //if we are not going to convert back to normal face we will need to put an oftype(PlanarFace) trip in here
                        if (myFace == null) return;
                        ////////// get handle to application from document
                        ////////Autodesk.Revit.ApplicationServices.Application application = doc.Application;



                        XYZ myXYZ_Normal = myFace.ComputeNormal(UV.Zero);

                        if (pickedRef.ConvertToStableRepresentation(doc)
                          .Contains("INSTANCE"))
                        {
                            Transform myXYZ_FamilyTransform = (myElement as FamilyInstance).GetTotalTransform();

                            //computedFaceNormal = trans.OfVector(computedFaceNormal);
                            myXYZ_Normal = myXYZ_FamilyTransform.BasisZ;

                            myXYZ_Normal = myXYZ_FamilyTransform.BasisZ;
                            if (myElement.Category.Name == "Doors") myXYZ_Normal = -myXYZ_FamilyTransform.BasisY;
                            if (myElement.Category.Name == "Windows") myXYZ_Normal = -myXYZ_FamilyTransform.BasisY;
                            if (myElement.Category.Name == "Curtain Panels") myXYZ_Normal = -myXYZ_FamilyTransform.BasisY;
                            //faceNormal = trans.OfVector(faceNormal);

                        }

                        Plane geomPlane = Plane.CreateByNormalAndOrigin(myXYZ_Normal, pickedRef.GlobalPoint);

                        //////if (true)
                        //////{
                        //////    // Create a geometry plane & sketch plane in Revit application
                        //////    XYZ origin = new XYZ(0, 0, 0);
                        //////    XYZ normal = new XYZ(1, 1, 0);

                        //////    XYZ startPoint = (geomPlane.YVec * -1) + pickedRef.GlobalPoint;
                        //////    XYZ endPoint = (geomPlane.YVec * 1) + pickedRef.GlobalPoint;
                        //////    Line geomLine = Line.CreateBound(startPoint, endPoint);

                        //////    // Create a geometry circle in Revit application
                        //////    double startAngle = 0;
                        //////    double endAngle = 2 * Math.PI;
                        //////    double radius = 1.23;
                        //////    Arc geomPlane2 = Arc.Create(geomPlane, radius, startAngle, endAngle);

                        //////    PointOnFace myPointOnFace = uidoc.Application.Application.Create.NewPointOnFace(pickedRef, pickedRef.UVPoint + new UV(10, 10));


                        //////    SketchPlane sketch = SketchPlane.Create(doc, geomPlane);

                        //////    ModelLine line = doc.Create.NewModelCurve(geomLine, sketch) as ModelLine;
                        //////    ModelArc arc1 = doc.Create.NewModelCurve(geomLine, sketch) as ModelArc;
                        //////    ModelArc arc2 = doc.Create.NewModelCurve(geomPlane2, sketch) as ModelArc;
                        //////    ModelArc arc3 = doc.Create.NewModelCurve(ArcsToCornerOfSurface(pickedRef.GlobalPoint, geomPlane), sketch) as ModelArc;
                        //////}


                        if(true)
                        {
                            Transform myTransform = Transform.Identity;  //front
                                                                         //myTransform.Origin = myFace.GetEdgesAsCurveLoops()[0].ElementAt(0).GetEndPoint(0);
                            myTransform.Origin = pickedRef.GlobalPoint;
                            myTransform.BasisX = geomPlane.XVec;
                            myTransform.BasisY = geomPlane.YVec;
                            myTransform.BasisZ = geomPlane.Normal;


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

                            SketchPlane sketch = SketchPlane.Create(doc, geomPlane);

                            doc.Create.NewModelCurve(geomArc2, sketch);
                            doc.Create.NewModelCurve(myCurve_Ellipse, sketch);
                            doc.Create.NewModelCurve(geomPlane2, sketch);
                            doc.Create.NewModelCurve(geomPlane3, sketch);
                            doc.Create.NewModelCurve(L2, sketch);
                            doc.Create.NewModelCurve(L3, sketch);
                        }






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

        public static Arc ArcsToCornerOfSurface(XYZ myXYZ_Origin, Plane geomPlane)
        {
            // Create a geometry arc in Revit application
            Transform myTransform = Transform.Identity;  //front
                                                         //myTransform.Origin = myFace.GetEdgesAsCurveLoops()[0].ElementAt(0).GetEndPoint(0);
            myTransform.Origin = myXYZ_Origin;
            myTransform.BasisX = geomPlane.XVec;
            myTransform.BasisY = geomPlane.YVec;
            myTransform.BasisZ = geomPlane.Normal;

            XYZ end0 = myTransform.OfPoint(new XYZ(-1, 0, 0));
            XYZ end1 = myTransform.OfPoint(new XYZ(1, 0, 0));
            XYZ pointOnCurve = myTransform.OfPoint(new XYZ(0, -1, 0));
            return Arc.Create(end0, end1, pointOnCurve);
        }
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
