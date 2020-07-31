
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
                        tx.Start("Rotate Man");

                        FamilyInstance myFamilyInstance = myListOfStuffOnWall.First() as FamilyInstance;

                        XYZ myXYZInstance = ((LocationPoint)myFamilyInstance.Location).Point;

                        Line myLineBasisY = Line.CreateUnbound(myXYZInstance, myFamilyInstance.GetTransform().BasisZ); //  tf_01072_4 = Transform.CreateRotation(myTransform_FakeBasis2.BasisZ, d1 / 10 * myInt_DivideInteger);

                        ElementTransformUtils.RotateElement(doc, myFamilyInstance.Id, myLineBasisY, Math.PI / 4);

                        tx.Commit();
                    }
                }


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
