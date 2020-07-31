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
    public class EE05_Part1_LoadAllFamilies : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                // string myString_FamilyName = "Generic Adaptive Nerf Gun";
                //string myString_FamilyFileName = @"\Generic Adaptive Nerf Gun.rfa";  //Families

                string myStringMessageBox = "";

                int myInt = 0;

                foreach (Window3.ListView_Class myListView_Class in myWindow1.myWindow3.myListClass)
                {
                    List<Element> myListFamily = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(Family))).Where(x => x.Name == myListView_Class.String_Name).ToList();

                    if (myListFamily.Count == 0)
                    {
                        string myString_TempPath = "";

                        if (myWindow1.myThisApplication.messageConst.Split('|')[0] == "Button_01_Invoke01")
                        {
                            myString_TempPath = myWindow1.myThisApplication.messageConst.Split('|')[1] + myListView_Class.String_FileName;
                        }
                        if (myWindow1.myThisApplication.messageConst.Split('|')[0] == "Button_01_Invoke01Development")
                        {
                            myString_TempPath = myWindow1.myThisApplication.messageConst.Split('|')[1] + @"\_929_Bilt2020_PlaypenChild" + myListView_Class.String_FileName;
                        }
                        using (Transaction tx = new Transaction(doc))
                        {
                            tx.Start("Load a " + myListView_Class.String_Name);
                            doc.LoadFamily(myString_TempPath, new FamilyOptionOverWrite(), out Family myFamily);
                            tx.Commit();
                        }

                        myStringMessageBox = myStringMessageBox + Environment.NewLine + myListView_Class.String_Name;
                        myInt++;
                    }

                }

                string myStringStart = myInt.ToString() + " families have been loaded: " +  Environment.NewLine + Environment.NewLine;

                MessageBox.Show(myStringStart + myStringMessageBox + Environment.NewLine + Environment.NewLine + "This only happens once per project.");

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE05_Part1_PlaceNerfGun" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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

        public class FamilyOptionOverWrite : IFamilyLoadOptions
        {
            public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = true;
                return true;
            }
            public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
            {
                source = FamilySource.Family;
                overwriteParameterValues = true;
                return true;
            }
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
