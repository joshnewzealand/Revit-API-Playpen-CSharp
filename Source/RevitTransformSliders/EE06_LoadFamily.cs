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
using System.Windows.Media.Media3D;
using Numerics = System.Numerics;
using std = System.Math;
using _952_PRLoogleClassLibrary;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace RevitTransformSliders
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE06_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {


        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("EE06_Template");

                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE06_Template" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        public string GetName()
        {
            return "EE06_Template";
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE06_PlaceFamily : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public Window1 myWindow1 { get; set; }
        public Xceed.Wpf.Toolkit.IntegerUpDown myIntUPDown { get; set; }

        public string myString_Family_Platform { get; set; }
        public string myString_Type_Platform { get; set; }

        public string myString_Family_Chair { get; set; }
        public string myString_Type_Chair { get; set; }

        public bool myBool_AlsoPlaceAChair { get; set; }


        private FamilySymbol myMethod_CheckExistanceOfFamily(Document doc, string myString_Family, string myString_Type)
        {
            FamilySymbol myFamilySymbol = null;

            if (true)  //candidate for methodisation 202006061505
            {
                List<Element> myListFamily = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(Family))).Where(x => x.Name == myString_Family).ToList();
                if (myListFamily.Count != 1)
                {
                    MessageBox.Show(myString_Family + ".rfa family has not been loaded into model");
                    return null;
                }

                Family myFamily = (Family)myListFamily.First();

                foreach (ElementId myID in myFamily.GetFamilySymbolIds())
                {
                    myFamilySymbol = doc.GetElement(myID) as FamilySymbol;

                    if (myFamilySymbol.Name == myString_Type) break;
                }

                if (myFamilySymbol == null)
                {
                    MessageBox.Show(myString_Type + " 'type' of family " + myString_Family + " was not found.");
                    return null;
                }

                if (!myFamilySymbol.IsActive) myFamilySymbol.Activate();

                return myFamilySymbol;
            }
        }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("EE06_PlaceFamily");

                    FamilySymbol myFamilySymbol_Platform = myMethod_CheckExistanceOfFamily(doc, myString_Family_Platform, myString_Type_Platform);
                    if (myFamilySymbol_Platform == null) return;

                    FamilySymbol myFamilySymbol_Chair = null;
                    if (myBool_AlsoPlaceAChair)
                    {
                        myFamilySymbol_Chair = myMethod_CheckExistanceOfFamily(doc, myString_Family_Chair, myString_Type_Chair);
                        if (myFamilySymbol_Chair == null) return;
                    }
                    Level myLevel = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().First() as Level;
                    Reference pickedRef = null;
                    XYZ myXYZ = null;
                    try
                    {

                        SketchPlane sp = SketchPlane.Create(doc, myLevel.GetPlaneReference());
                        doc.ActiveView.SketchPlane = sp;

                        // doc.ActiveView.ShowActiveWorkPlane();

                        //clearing existing command
                        SetForegroundWindow(uidoc.Application.MainWindowHandle);

                        myXYZ = uiapp.ActiveUIDocument.Selection.PickPoint();
                        pickedRef = sp.GetPlaneReference();
                        //pickedRef = uiapp.ActiveUIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.PointOnElement, "Please select a Face");
                    }

                    #region catch and finally
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                    }
                    #endregion

                    if (pickedRef == null) return;

                    FamilyInstance myFamilyInstance_New = doc.Create.NewFamilyInstance(XYZ.Zero, myFamilySymbol_Platform, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                    doc.Regenerate();

                    IList<ElementId> placePointIds_1338 = AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_New);
                    ReferencePoint myReferencePoint_Centre = doc.GetElement(placePointIds_1338.First()) as ReferencePoint;

                    //UV point_in_3d_UV = pickedRef.UVPoint;
                    UV point_in_3d_UV = new UV(myXYZ.X, myXYZ.Y);
                    PointOnPlane myPointOnPlane = uidoc.Application.Application.Create.NewPointOnPlane(doc.ActiveView.SketchPlane.GetPlaneReference(), point_in_3d_UV, UV.BasisU, 0.0);

                    myReferencePoint_Centre.SetPointElementReference(myPointOnPlane);

                    doc.Regenerate();
                    myReferencePoint_Centre.get_Parameter(BuiltInParameter.POINT_ELEMENT_DRIVEN).Set(0);

                    ElementTransformUtils.MoveElement(doc, myReferencePoint_Centre.Id, myReferencePoint_Centre.GetCoordinateSystem().OfPoint(new XYZ(0, 0, 0.1)) - myReferencePoint_Centre.Position);

                    if (myFamilySymbol_Chair != null)
                    {
                        GeometryElement myGeomeryElement = myFamilyInstance_New.get_Geometry(new Options() { ComputeReferences = true });
                        GeometryInstance myGeometryInstance = myGeomeryElement.First() as GeometryInstance;
                        GeometryElement myGeomeryElementSymbol = myGeometryInstance.GetSymbolGeometry();
                        GeometryObject myGeometryObject = myGeomeryElementSymbol.Where(x => (x as Solid) != null).First();
                        PlanarFace myPlanarFace = ((Solid)myGeometryObject).Faces.get_Item(0) as PlanarFace;

                        doc.Create.NewFamilyInstance(myPlanarFace, myReferencePoint_Centre.Position, myReferencePoint_Centre.GetCoordinateSystem().OfVector(new XYZ(1, 0, 0)), myFamilySymbol_Chair);
                    }

                    tx.Commit();

                    uidoc.Selection.SetElementIds(new List<ElementId>() { myReferencePoint_Centre.Id });

                    myWindow1.mySelectMethod(myIntUPDown);
                    myIntUPDown.Value = myFamilyInstance_New.Id.IntegerValue;
                }

                myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisZ, false);
                myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisX, false);
                myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisY, true);
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE06_PlaceFamily" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        public string GetName()
        {
            return "EE06_PlaceFamily";
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE06_LoadFamily : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("EE06_LoadFamily");

                    string stringAppDataPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string stringAppDataCompanyProductYearPath = stringAppDataPath + "\\Default Company Name\\CSharp PLAYPEN II Google 'Josh API Revit'\\2019";
                    string stringAGM_FileName = "\\PRL-GM Adaptive Carrier Youtube.rfa";
                    string stringChair_FileName = "\\PRL-GM Chair with always vertical OFF.rfa";

                    doc.LoadFamily(stringAppDataCompanyProductYearPath + stringAGM_FileName);
                    doc.LoadFamily(stringAppDataCompanyProductYearPath + stringChair_FileName);
                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE06_LoadFamily" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        public string GetName()
        {
            return "EE05_Template";
        }
    }

}
