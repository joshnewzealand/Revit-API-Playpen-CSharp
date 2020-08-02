
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
using _952_PRLoogleClassLibrary;

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
    public class EE04_UnderStandingTransforms : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            string dllModuleName = "RevitTransformSliders";
            string myString_TempPath = "";
            try
            {
                if (myWindow1.myThisApplication.messageConst.Split('|')[0] == "Button_01_Invoke01")
                {
                    myString_TempPath = myWindow1.myThisApplication.messageConst.Split('|')[1];

                    //string path = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("TARGETDIR").ToString(); ;

                    System.Reflection.Assembly objAssembly01 = System.Reflection.Assembly.Load(System.IO.File.ReadAllBytes(myString_TempPath + "\\" + dllModuleName + ".dll"));
                    string strCommandName = "ThisApplication";

                    IEnumerable<Type> myIEnumerableType = GetTypesSafely(objAssembly01);
                    foreach (Type objType in myIEnumerableType)
                    {
                        if (objType.IsClass)
                        {
                            if (objType.Name.ToLower() == strCommandName.ToLower())
                            {
                                object ibaseObject = Activator.CreateInstance(objType);
                                object[] arguments = new object[] { myWindow1.commandData, "Button_01_Invoke01|" + myString_TempPath, new ElementSet() };

                                object result = null;

                                result = objType.InvokeMember("OpenWindowForm", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, ibaseObject, arguments);

                                break;
                            }
                        }
                    }
                }


                if (myWindow1.myThisApplication.messageConst.Split('|')[0] == "Button_01_Invoke01Development")
                {

                    myString_TempPath = myWindow1.myThisApplication.messageConst.Split('|')[1];

                    System.Reflection.Assembly objAssembly01 = System.Reflection.Assembly.Load(System.IO.File.ReadAllBytes(myString_TempPath + "\\" + dllModuleName + "\\AddIn\\" + dllModuleName + ".dll"));

                    string strCommandName = "ThisApplication";

                    IEnumerable<Type> myIEnumerableType = GetTypesSafely(objAssembly01);
                    foreach (Type objType in myIEnumerableType)
                    {
                        if (objType.IsClass)
                        {
                            if (objType.Name.ToLower() == strCommandName.ToLower())
                            {
                                object ibaseObject = Activator.CreateInstance(objType);
                                object[] arguments = new object[] { myWindow1.commandData, "Button_01_Invoke01Development|" + myString_TempPath, new ElementSet() };
                                object result = null;

                                result = objType.InvokeMember("OpenWindowForm", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, ibaseObject, arguments);

                                break;
                            }
                        }
                    }
                }

            }

            #region catch and finally
            catch (Exception ex)
            {
                string pathHeader = "Please check this file (and directory) exist: " + Environment.NewLine;
                // string path = myWindow1.myThisApplication.messageConst.Split('|')[1] + @"\_929_Bilt2020_PlaypenChild";
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug(pathHeader + myString_TempPath, true);
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

        private static IEnumerable<Type> GetTypesSafely(System.Reflection.Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
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

                    XYZ myXYZ_Centre = uiview.GetZoomCorners()[0] + ((uiview.GetZoomCorners()[1] - uiview.GetZoomCorners()[0]) / 2);


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

        public Wall myWall;

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                if (uidoc.Selection.GetElementIds().Count != 1)
                {
                    if (myWall == null) MessageBox.Show("Please select ONE Wall.");
                    return;
                }

                Element myElementWall = doc.GetElement(uidoc.Selection.GetElementIds().First());


                if (myElementWall.GetType() == typeof(FamilyInstance))
                {
                    FamilyInstance myFamilyInstance = myElementWall as FamilyInstance;

                    myElementWall = myFamilyInstance.Host;

                    if (myElementWall == null)
                    {
                        MessageBox.Show("Family instance must be hosted to a wall.");
                        return;
                    }
                }


                if (myElementWall.Category.Name != "Walls")
                {
                    if (myWall == null) MessageBox.Show("Selected entity must be a Wall.");
                    return;
                }

                myWall = myElementWall as Wall;


               List<Element> myListOfStuffOnWall = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_GenericModel).Where(x => (((FamilyInstance)x).Host != null)).Where(x => ((FamilyInstance)x).Host.Id == myWall.Id).ToList();

                if(myListOfStuffOnWall.Count() > 0)
                {
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Rotate People");

                        foreach(FamilyInstance myFI in myListOfStuffOnWall)
                        {
                            FamilyInstance myFamilyInstance = myFI as FamilyInstance;

                            XYZ myXYZInstance = ((LocationPoint)myFamilyInstance.Location).Point;

                            Line myLineBasisY = Line.CreateUnbound(myXYZInstance, myFamilyInstance.GetTransform().BasisZ); //  tf_01072_4 = Transform.CreateRotation(myTransform_FakeBasis2.BasisZ, d1 / 10 * myInt_DivideInteger);

                            ElementTransformUtils.RotateElement(doc, myFamilyInstance.Id, myLineBasisY, Math.PI / 4);
                        }

                        tx.Commit();
                    }
                }

                if(myWindow1.myCheckBoxWithWall.IsChecked.Value)
                {

                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Rotate Wall");

                        Curve myCurve = ((LocationCurve)myWall.Location).Curve;

                        XYZ myXYZ = (myCurve.GetEndPoint(1) + myCurve.GetEndPoint(0)) / 2;

                        Line myLineBasisZ = Line.CreateUnbound(myXYZ, XYZ.BasisZ);

                        ElementTransformUtils.RotateElement(doc, myWall.Id, myLineBasisZ, Math.PI / 20);

                        tx.Commit();
                    }
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


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE04_Part1_MoveElementOnWall : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public Wall myWall;

        public List<ElementId> myListElementID { get; set; } = new List<ElementId>();

        public void Execute(UIApplication uiapp)
        {
            try
            {

                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                if (uidoc.Selection.GetElementIds().Count != 1)
                {
                    if (myWall == null) MessageBox.Show("Please select ONE Wall.");
                    return;
                }

                Element myElementWall = doc.GetElement(uidoc.Selection.GetElementIds().First());

                if(myElementWall.GetType() == typeof(FamilyInstance))
                {
                    FamilyInstance myFamilyInstance = myElementWall as FamilyInstance;

                    myElementWall = myFamilyInstance.Host;

                    if (myElementWall == null)
                    {
                        MessageBox.Show("Family instance must be hosted to a wall.");
                        return;
                    }
                }


                if (myElementWall.Category.Name != "Walls")
                {
                    MessageBox.Show("Selected entity must be a Wall.");
                    return;
                }

                myWall = myElementWall as Wall;

                List<Element> myListOfStuffOnWall = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_GenericModel).Where(x => (((FamilyInstance)x).Host != null)).Where(x => ((FamilyInstance)x).Host.Id == myWall.Id).ToList();


                if (myListOfStuffOnWall.Count() > 0)
                {
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Rotate People");

                        foreach (FamilyInstance myFI in myListOfStuffOnWall)
                        {
                            FamilyInstance myFamilyInstance = myFI as FamilyInstance;

                            Reference pickedRef = myFI.HostFace;

                            ////doc.Delete(myListElementID);
                            ////myListElementID.Clear();

                            Face myFace = myElementWall.GetGeometryObjectFromReference(pickedRef) as Face;
                            if (myFace == null) return;


                            Transform myXYZ_FamilyTransform = Transform.Identity;

                            if (pickedRef.ConvertToStableRepresentation(doc).Contains("INSTANCE"))
                            {
                                myXYZ_FamilyTransform = (myElementWall as FamilyInstance).GetTotalTransform();
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

                            XYZ myXYZ_UV_CornerOne = (((myXYZ_Max - myXYZ_Min) * 0.1));//; + myXYZ_Min;
                            XYZ myXYZ_UV_CornerTwo = (((myXYZ_Max - myXYZ_Min) * 0.9));// + myXYZ_Min;
                            
                            XYZ myXYZ_UV_Corner01 = new XYZ(myXYZ_UV_CornerOne.X, myXYZ_UV_CornerOne.Y, 0);
                            XYZ myXYZ_UV_Corner02 = new XYZ(myXYZ_UV_CornerTwo.X, myXYZ_UV_CornerOne.Y, 0);
                            XYZ myXYZ_UV_Corner03 = new XYZ(myXYZ_UV_CornerTwo.X, myXYZ_UV_CornerTwo.Y, 0);
                            XYZ myXYZ_UV_Corner04 = new XYZ(myXYZ_UV_CornerOne.X, myXYZ_UV_CornerTwo.Y, 0);
                          
                            Line L1 = Line.CreateBound(myTransform.OfPoint(myXYZ_UV_Corner01), myTransform.OfPoint(myXYZ_UV_Corner02));
                            Line L2 = Line.CreateBound(myTransform.OfPoint(myXYZ_UV_Corner02), myTransform.OfPoint(myXYZ_UV_Corner03));
                            Line L3 = Line.CreateBound(myTransform.OfPoint(myXYZ_UV_Corner03), myTransform.OfPoint(myXYZ_UV_Corner04));
                            Line L4 = Line.CreateBound(myTransform.OfPoint(myXYZ_UV_Corner04), myTransform.OfPoint(myXYZ_UV_Corner01));

                            CurveLoop myCurveLoop = new CurveLoop();
                            myCurveLoop.Append(L1);
                            myCurveLoop.Append(L2);
                            myCurveLoop.Append(L3);
                            myCurveLoop.Append(L4);

                            double myDouble_ExactLength = myCurveLoop.GetExactLength();
                            double myDouble_ExactLength_Twenty = myDouble_ExactLength / 40;

                            XYZ myXYZ_Result = myTransform.OfPoint((myXYZ_Max - myXYZ_Min) / 2);

                            int myIntCurrentSpinnerValue = myWindow1.myIntegerUpDown_OneToTwentyCount.Value.Value;

                            int myInt_Positioning = (40 / myListOfStuffOnWall.Count()) * (myListOfStuffOnWall.IndexOf(myFI) + 1);

                            myIntCurrentSpinnerValue = (40 - myInt_Positioning) + myIntCurrentSpinnerValue;

                            if (myIntCurrentSpinnerValue > 40) myIntCurrentSpinnerValue = myIntCurrentSpinnerValue - 40;

                            double myDouble_FutureForeach = myDouble_ExactLength_Twenty * myIntCurrentSpinnerValue;

                            double myDouble_ThisFarAlong;

                            switch (myDouble_FutureForeach)
                            {
                                case double n when n < L1.Length:

                                    myDouble_ThisFarAlong = myDouble_FutureForeach;
                                    myXYZ_Result = L1.GetEndPoint(0) + (L1.GetEndPoint(1) - L1.GetEndPoint(0)).Normalize().Multiply(myDouble_ThisFarAlong);

                                    break;
                                case double n when n >= L1.Length & n < L1.Length + L2.Length:

                                    myDouble_ThisFarAlong = myDouble_FutureForeach - L1.Length;
                                    myXYZ_Result = L2.GetEndPoint(0) + (L2.GetEndPoint(1) - L2.GetEndPoint(0)).Normalize().Multiply(myDouble_ThisFarAlong);

                                    break;
                                case double n when n >= L1.Length + L2.Length & n < L1.Length + L2.Length + L3.Length:

                                    myDouble_ThisFarAlong = myDouble_FutureForeach - L1.Length - L2.Length;
                                    myXYZ_Result = L3.GetEndPoint(0) + (L3.GetEndPoint(1) - L3.GetEndPoint(0)).Normalize().Multiply(myDouble_ThisFarAlong);

                                    break;
                                case double n when n >= L1.Length + L2.Length + L3.Length:

                                    myDouble_ThisFarAlong = myDouble_FutureForeach - L1.Length - L2.Length - L3.Length;
                                    myXYZ_Result = L4.GetEndPoint(0) + (L4.GetEndPoint(1) - L4.GetEndPoint(0)).Normalize().Multiply(myDouble_ThisFarAlong);

                                    break;
                            }

                            XYZ myXYZ_MoveThisMuch = myXYZ_Result - ((LocationPoint)myFamilyInstance.Location).Point;
                            ElementTransformUtils.MoveElement(doc, myFamilyInstance.Id, myXYZ_MoveThisMuch);


                            //SketchPlane mySketchPlane = SketchPlane.Create(doc, pickedRef);
                            //myListElementID.Add(doc.Create.NewModelCurve(L1, mySketchPlane).Id);
                            //myListElementID.Add(doc.Create.NewModelCurve(L2, mySketchPlane).Id);
                            //myListElementID.Add(doc.Create.NewModelCurve(L3, mySketchPlane).Id);
                            //myListElementID.Add(doc.Create.NewModelCurve(L4, mySketchPlane).Id);


                          
                        }
                        tx.Commit();
                    }
                    if (myWindow1.myIntegerUpDown_OneToTwentyCount.Value.Value == 40) myWindow1.myIntegerUpDown_OneToTwentyCount.Value = 0;
                    myWindow1.myIntegerUpDown_OneToTwentyCount.Value++;
                }
            }
            #region catch and finally
            catch (Exception ex)
            {
                DatabaseMethods.writeDebug("EE04_Part1" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
