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
                    if (!int.TryParse(myString_RememberLast, out n))
                    {
                        MessageBox.Show("Please select just one geometric element (e.g. a Wall).");
                        return;
                    }
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
            SetForegroundWindow(uidoc.Application.MainWindowHandle); //this is an excape event

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
                 
                    MyPreProcessor preproccessor = new MyPreProcessor();

                    using (Transaction y = new Transaction(doc, "Simily Face"))
                    {
                        y.Start();

                        FailureHandlingOptions options = y.GetFailureHandlingOptions();
                        options.SetFailuresPreprocessor(preproccessor);
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

        public bool myBool_DoLoop { get; set; } = true;
        public bool myBool_JustClear { get; set; } = false;

        public List<ElementId> myListElementID  { get; set; } = new List<ElementId>();

        UIApplication uiapp;

        public Reference myReference { get; set; } = null;

        public void Execute(UIApplication uiappp)
        {
            uiapp = uiappp;

            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                if (!myBool_JustClear)
                {

                    if (myBool_DoLoop)
                    {
                        myListElementID.Clear();
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
                            myReference = pickedRef;

                            myWindow1.myIntegerUpDown_Columns.IsEnabled = true;
                            myWindow1.myIntegerUpDown_Rows.IsEnabled = true;
                            myWindow1.myCheckBox_OneTwoOne.IsEnabled = true;


                            CreatingCircles_ReturnsBreak(myReference);

                        } while (true);
                    }
                    else
                    {
                        CreatingCircles_ReturnsBreak(myReference);
                    }
                }
                else
                {
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Delete the previous circles.");
                        doc.Delete(myListElementID);
                        myListElementID.Clear();
                        tx.Commit();
                    }

                    myReference = null; 

                    myWindow1.myIntegerUpDown_Columns.IsEnabled = false;
                    myWindow1.myIntegerUpDown_Rows.IsEnabled = false;
                    myWindow1.myCheckBox_OneTwoOne.IsEnabled = false;

                }

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



        private void CreatingCircles_ReturnsBreak(Reference pickedRef)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Delete the previous circles.");
                doc.Delete(myListElementID);
                myListElementID.Clear();
                tx.Commit();
            }

            Element myElement = doc.GetElement(pickedRef.ElementId);

            Face myFace = myElement.GetGeometryObjectFromReference(pickedRef) as Face;
            if (myFace == null) return;

            Transform myXYZ_FamilyTransform = Transform.Identity;

            if (pickedRef.ConvertToStableRepresentation(doc).Contains("INSTANCE"))
            {
                myXYZ_FamilyTransform = (myElement as FamilyInstance).GetTotalTransform();
            }

            Transform myTransform = Transform.Identity;

            if (myFace.GetType() != typeof(PlanarFace)) return;

            PlanarFace myPlanarFace = myFace as PlanarFace;

            UV myUV_Min = myFace.GetBoundingBox().Min;
            UV myUV_Max = myFace.GetBoundingBox().Max;

            XYZ myXYZ_CornerOne = myFace.Evaluate(myUV_Min);
            XYZ myXYZ_CornerTwo = myFace.Evaluate(myUV_Max);

            XYZ myXYZ_CornerOne_Transformed = myXYZ_FamilyTransform.OfPoint(myXYZ_CornerOne);
            XYZ myXYZ_CornerTwo_Transformed = myXYZ_FamilyTransform.OfPoint(myXYZ_CornerTwo);

            myTransform.Origin = myXYZ_CornerOne_Transformed;
            myTransform.BasisX = myXYZ_FamilyTransform.OfVector(myPlanarFace.XVector);
            myTransform.BasisY = myXYZ_FamilyTransform.OfVector(myPlanarFace.YVector);
            myTransform.BasisZ = myXYZ_FamilyTransform.OfVector(myPlanarFace.FaceNormal);

            XYZ myXYZ_Centre = XYZ.Zero;

            XYZ myXYZ_Min = new XYZ(myUV_Min.U, myUV_Min.V, 0);
            XYZ myXYZ_Max = new XYZ(myUV_Max.U, myUV_Max.V, 0);


            bool myBool_OneTwoOneLayout =  myWindow1.myCheckBox_OneTwoOne.IsChecked.Value;

            int myInt_Columns = myWindow1.myIntegerUpDown_Columns.Value.Value;
            int myInt_Rows = myWindow1.myIntegerUpDown_Rows.Value.Value;

            int myIntDivideColumn = myBool_OneTwoOneLayout ? myInt_Columns * 2 : myInt_Columns + 1; 
            int myIntDivideRow = myBool_OneTwoOneLayout ? myInt_Rows * 2 : myInt_Rows + 1;

            XYZ myXYZ_DifferernceColumn = (myXYZ_Max - myXYZ_Min) / myIntDivideColumn;
            double myDouble_WidthColumn = myXYZ_DifferernceColumn.X;

            XYZ myXYZ_DifferernceRow = (myXYZ_Max - myXYZ_Min) / myIntDivideRow;
            double myDouble_WidthRow = myXYZ_DifferernceRow.Y;

            double myDouble_SmallestWins = myDouble_WidthColumn < myDouble_WidthRow ? myDouble_WidthColumn : myDouble_WidthRow;

            double startAngle = 0;
            double endAngle = 2 * Math.PI;
            double radius = myDouble_SmallestWins / (myBool_OneTwoOneLayout ? 1 : 2 );

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Draw Grid of Circles On Face");
                SketchPlane mySketchPlane = SketchPlane.Create(doc, pickedRef);

                for (int int_Outer = 1; int_Outer <= myWindow1.myIntegerUpDown_Columns.Value.Value * (myBool_OneTwoOneLayout ? 2 : 1); int_Outer++)
                {
                    if (int_Outer % 2 != 0 | !myBool_OneTwoOneLayout) //here
                    {
                        for (int int_Inner = 1; int_Inner <= myWindow1.myIntegerUpDown_Rows.Value.Value * (myBool_OneTwoOneLayout ? 2 : 1); int_Inner++)
                        {
                            XYZ myXYZ_GridPoint = new XYZ(myXYZ_DifferernceColumn.X * int_Outer, myXYZ_DifferernceRow.Y * int_Inner, 0);

                            if (int_Inner % 2 != 0 | !myBool_OneTwoOneLayout) //here
                            {
                                Arc geomPlane3 = Arc.Create(myTransform.OfPoint(myXYZ_GridPoint), radius, startAngle, endAngle, myTransform.BasisX, myTransform.BasisY);
                                myListElementID.Add(doc.Create.NewModelCurve(geomPlane3, mySketchPlane).Id);
                            }
                        }
                    }
                }

                tx.Commit();
            }
            return;
        }

        public string GetName()
        {
            return "External Event Example";
        }
    }
}
