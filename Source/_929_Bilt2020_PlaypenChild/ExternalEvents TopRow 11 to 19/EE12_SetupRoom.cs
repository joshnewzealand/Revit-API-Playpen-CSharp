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
using Autodesk.Revit.DB.ExtensibleStorage;

namespace _929_Bilt2020_PlaypenChild
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE12_SetupRoom : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
                    tx.Start("SetupRoom");

                    List<ElementId> myFEC_DataStorage = new FilteredElementCollector(doc).OfClass(typeof(DataStorage)).WhereElementIsNotElementType().Where(x => x.Name == "Room Setup Entities").Select(x => x.Id).ToList();

                    if (myFEC_DataStorage.Count != 0)
                    {
                        doc.Delete(myFEC_DataStorage);
                        List<ElementId> myFEC_RoomSetupEntities = new FilteredElementCollector(doc).WhereElementIsNotElementType().Where(x => x.LookupParameter("Comments") != null).Where(x => x.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString() == "Room Setup Entities").Select(x => x.Id).ToList();
                        doc.Delete(myFEC_RoomSetupEntities);

                        myWindow1.myWindow4.myListViewEE.ItemsSource = null;

                    }
                    else
                    {
                        Element myLevel = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().First();
                        List<ElementId> myFEC_Walls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().Where(x => x.Name == "Exterior Glazing").Select(x => x.Id).ToList();

                        if (myFEC_Walls.Count == 0)
                        {
                            MessageBox.Show("Wall type Exterior  Glazing needs to exist.\n\r\n\rIt is contained in the default Construction template.");

                        }
                        else
                        {
                            ///                      TECHNIQUE 12 OF 19 (EE12_SetupRoom.cs)
                            ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ ARRANGE FURNITURE IN A ROOM WITH GLASS WALLS ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                            ///
                            /// Interfaces and ENUM's:
                            ///     IFamilyLoadOptions
                            /// 
                            /// Demonstrates classes:
                            ///     DataStorage
                            ///     FamilySymbol
                            /// 
                            /// 
                            /// Key methods:
                            ///     DataStorage.Create(
                            ///     doc.LoadFamily(
                            ///     doc.GetElement(myFamily.GetFamilySymbolIds().First()) as FamilySymbol;
                            ///     
                            /// 
                            ///
                            ///
							///	https://github.com/joshnewzealand/Revit-API-Playpen-CSharp


                            // Build a wall profile for the wall creation
                            XYZ first = new XYZ(80, 0, 0);
                            XYZ second = new XYZ(100, 0, 0);
                            XYZ third = new XYZ(100, -20, 0);
                            XYZ fourth = new XYZ(80, -20, 0);
                            IList<Curve> profile = new List<Curve>();

                            profile.Add(Line.CreateBound(first, second));
                            profile.Add(Line.CreateBound(second, third));
                            profile.Add(Line.CreateBound(third, fourth));
                            profile.Add(Line.CreateBound(fourth, first));

                            profile = profile.Reverse().ToList();

                            foreach (Curve c in profile)
                            {
                                Wall myWall = Wall.Create(doc, c, myFEC_Walls.First(), myLevel.Id, 10, 0, false, false);
                                myWall.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Room Setup Entities");

                                myWall.Pinned = true;
                                // foreach (ElementId myEleID in myWall.CurtainGrid.GetPanelIds()) doc.GetElement(myEleID).Pinned = true;

                                uidoc.RefreshActiveView();
                            }

                            DataStorage myDatastorage = DataStorage.Create(doc);
                            myDatastorage.Name = "Room Setup Entities";
                            //DatabaseMethods.writeDebug(myDatastorage.Id.IntegerValue.ToString(), true);

                            if (true)
                            {
                                XYZ myXYZ_Location = new XYZ(85.77, -3.08, 0); double myDouble_Rotation = 1.57 - (Math.PI / 2);
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 0); //Furniture Chair Executive
                                PlaceAndRotateFamily(uidoc, myFamilySymbol, myXYZ_Location, myDouble_Rotation, myLevel);
                            }

                            if (true)
                            {
                                XYZ myXYZ_Location = new XYZ(99.67, -4.58, 0); double myDouble_Rotation = 3.14 + (Math.PI / 2);
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 1); //Furniture Chair Viper
                                PlaceAndRotateFamily(uidoc, myFamilySymbol, myXYZ_Location, myDouble_Rotation, myLevel);
                            }

                            if (true)
                            {
                                XYZ myXYZ_Location = new XYZ(80.33, -10.83, 0); double myDouble_Rotation = 0 + (Math.PI / 2);
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 2); //Furniture Couch Viper
                                PlaceAndRotateFamily(uidoc, myFamilySymbol, myXYZ_Location, myDouble_Rotation, myLevel);
                            }

                            if (true)
                            {
                                XYZ myXYZ_Location = new XYZ(85.69, -1.45, 0); double myDouble_Rotation = 1.57 - (Math.PI / 2);
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 3); //Furniture Desk
                                PlaceAndRotateFamily(uidoc, myFamilySymbol, myXYZ_Location, myDouble_Rotation, myLevel);
                            }

                            if (true)
                            {
                                XYZ myXYZ_Location = new XYZ(95.06, -14.72, 0); double myDouble_Rotation = 1.57 - (Math.PI / 2);
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 4); //Furniture Table Dining Round w Chairs
                                PlaceAndRotateFamily(uidoc, myFamilySymbol, myXYZ_Location, myDouble_Rotation, myLevel);
                            }

                            if (true)
                            {
                                XYZ myXYZ_Location = new XYZ(98.67, -7.88, 0); double myDouble_Rotation = 1.57 - (Math.PI / 2);
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 5); //Furniture Table Night Stand
                                PlaceAndRotateFamily(uidoc, myFamilySymbol, myXYZ_Location, myDouble_Rotation, myLevel);
                            }

                            if (true)
                            {
                                XYZ myXYZ_Location = new XYZ(90.84, -9.55, 4.99); double myDouble_Rotation = 1.57 - (Math.PI / 2);
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 6); //Generic Adaptive Nerf Gun
                                PlaceAndRotateFamily(uidoc, myFamilySymbol, myXYZ_Location, myDouble_Rotation, myLevel);
                            }

                            if (true)
                            {
                                XYZ myXYZ_Location = new XYZ(82.95, -10.81, 0); double myDouble_Rotation = 0 + (Math.PI / 2);
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 7); //Generic Model Man Sitting Eating
                                PlaceAndRotateFamily(uidoc, myFamilySymbol, myXYZ_Location, myDouble_Rotation, myLevel);
                            }

                            if (true)
                            {
                                XYZ myXYZ_Location = new XYZ(85.37, -7.71, 0); double myDouble_Rotation = 2.36 - (Math.PI);
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 8); //Generic Model Man Women Construction Worker
                                PlaceAndRotateFamily(uidoc, myFamilySymbol, myXYZ_Location, myDouble_Rotation, myLevel);
                            }

                            if (true)
                            {
                                XYZ myXYZ_Location = new XYZ(92.97, -1.55, 0); double myDouble_Rotation = 2.36 - (Math.PI);
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 9); //Generic Model Tipping Hat Man
                                PlaceAndRotateFamily(uidoc, myFamilySymbol, myXYZ_Location, myDouble_Rotation, myLevel);
                            }
                        }
                    }

                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE12_ExtensibleStorage_SetupRoom" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void PlaceAndRotateFamily(UIDocument uidoc, FamilySymbol myFamilySymbol, XYZ myXYZ_Location, double myDouble_Rotation, Element myLevel)
        {
            Document doc = uidoc.Document; 

            myFamilySymbol.Activate();
            FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(myXYZ_Location, myFamilySymbol, myLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            Line myLine = Line.CreateUnbound(myXYZ_Location, XYZ.BasisZ);

            ElementTransformUtils.RotateElement(doc, myFamilyInstance.Id, myLine, myDouble_Rotation);
            myFamilyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Room Setup Entities");
            uidoc.RefreshActiveView();
        }


        private FamilySymbol myFamilyReturn_FindInModel(Document doc, int myIntOf_ListStaticFamilyNames)
        {
            Window0506_LoadAndPlaceFamilies.ListView_Class myListViewClass = myWindow1.myListClass[myIntOf_ListStaticFamilyNames];

            List<Element> myListElement = new FilteredElementCollector(doc).OfClass(typeof(Family)).Where(x => x.Name == myListViewClass.String_Name).ToList();

            FamilySymbol myFamilySymbol = null;
            if (myListElement.Count() == 0)
            {
                myFamilySymbol = myFamilyReturn_LoadExternally(doc, myListViewClass);
            }
            else
            {
                myFamilySymbol = doc.GetElement(((Family)myListElement.First()).GetFamilySymbolIds().First()) as FamilySymbol;
            }

            return myFamilySymbol;
        }


        private FamilySymbol myFamilyReturn_LoadExternally(Document doc, Window0506_LoadAndPlaceFamilies.ListView_Class myListViewClass)
        {

            string myString_TempPath = "";

            if (myWindow1.myThisApplication.messageConst.Split('|')[0] == "Button_01_Invoke01")
            {
                myString_TempPath = myWindow1.myThisApplication.messageConst.Split('|')[1] + myListViewClass.String_FileName;
            }
            if (myWindow1.myThisApplication.messageConst.Split('|')[0] == "Button_01_Invoke01Development")
            {
                myString_TempPath = myWindow1.myThisApplication.messageConst.Split('|')[1] + @"\_929_Bilt2020_PlaypenChild" + myListViewClass.String_FileName;
            }

            doc.LoadFamily(myString_TempPath, new FamilyOptionOverWrite(), out Family myFamily);

            FamilySymbol myFamilySymbol = doc.GetElement(myFamily.GetFamilySymbolIds().First()) as FamilySymbol;


            return myFamilySymbol;
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

        public string GetName()
        {
            return "External Event Example";
        }
    }
}
