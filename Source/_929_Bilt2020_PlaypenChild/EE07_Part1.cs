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
using _952_PRLoogleClassLibrary;

using Transform = Autodesk.Revit.DB.Transform;
using Binding = Autodesk.Revit.DB.Binding;

namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE07_Part1_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE06_Part1_Template" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE07_Part1_Binding : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }
        public Window2 myWindow2 { get; set; }
        public bool myBool_AddToProject { get; set; } = true;

        public void Execute(UIApplication uiapp)
        {
            try
            {
                myWindow2.myTextBoxPrevious.Text = "";
                myWindow2.myTextBoxNew.Text = "";
                myWindow2.myListBoxInstanceParameters.SelectedIndex = -1;
                myWindow2.myListBoxTypeParameters.SelectedIndex = -1;

                string FILE_NAME = System.Environment.GetEnvironmentVariable("ProgramData") + "\\Pedersen Read Limited"; // cSharpPlaypen joshnewzealand

                if (true) //grouping for clarity will alwasy be true
                {
                    if (!System.IO.Directory.Exists(FILE_NAME)) System.IO.Directory.CreateDirectory(FILE_NAME);
                    FILE_NAME = FILE_NAME + "\\cSharpPlaypen joshnewzealand"; // 
                    if (!System.IO.Directory.Exists(FILE_NAME)) System.IO.Directory.CreateDirectory(FILE_NAME);
                    FILE_NAME = (FILE_NAME + "\\Location Of Shared Parameters File.txt");
                }

                string path = "";

                if (true) //read line
                {
                    if (!System.IO.File.Exists(FILE_NAME))
                    {
                        path = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("TARGETDIR").ToString();
                    }
                    else
                    {
                        System.IO.StreamReader objReader = new System.IO.StreamReader(FILE_NAME);
                        path = objReader.ReadLine();
                    }

                    string myStringCollectup = "";
                    if (!myBool_AddToProject)  myStringCollectup = myStringCollectup + myMethod_BindParameters(path + "\\PlayPenSharedParametersType.txt", true);
                    if (myBool_AddToProject) myStringCollectup = myStringCollectup + myMethod_BindParameters(path + "\\PlayPenSharedParametersInstance.txt", false);

                    MessageBox.Show(myStringCollectup);
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE07_Part1_Binding" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        public string GetName()
        {
            return "EE07_Part1_Binding";
        }

        public string myMethod_BindParameters(string path, bool IsTypeParameter)
        {
            UIDocument uidoc = myWindow1.commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            string myStringSharedParameterFileName = "";

            if (uidoc.Application.Application.SharedParametersFilename != null)
            {
                myStringSharedParameterFileName = uidoc.Application.Application.SharedParametersFilename; //Q:\Revit Revit Revit\Template 2018\PRL_Parameters.txt
            }


            int eL = -1;

            try
            {

                uidoc.Application.Application.SharedParametersFilename = path;

                DefinitionFile myDefinitionFile = uidoc.Application.Application.OpenSharedParameterFile();


                if (myDefinitionFile == null)
                {
                    DatabaseMethods.writeDebug(path + Environment.NewLine + Environment.NewLine + "File does not exist OR cannot be opened.", true);
                    return "";
                }

                CategorySet catSet = uidoc.Application.Application.Create.NewCategorySet();
                foreach (Category myCatttt in doc.Settings.Categories)
                {
                    if (myCatttt.AllowsBoundParameters) catSet.Insert(myCatttt);
                }

                string myStringCollectup = "";
                DefinitionGroup group = myDefinitionFile.Groups.get_Item("Default");


                if (!IsTypeParameter)
                {
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Shared parameters to Project");

                        foreach (Definition myDefinition in group.Definitions)
                        {
                            myStringCollectup = myStringCollectup + " • " + myDefinition.Name + Environment.NewLine;
                            Binding binding = IsTypeParameter ? uidoc.Application.Application.Create.NewTypeBinding(catSet) as Binding : uidoc.Application.Application.Create.NewInstanceBinding(catSet) as Binding;

                            doc.ParameterBindings.Insert(myDefinition, binding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                        }
                        tx.Commit();
                    }
                    myStringCollectup = (IsTypeParameter ? "Type" : "Instance") + " parameters added to Project (all categories):" + Environment.NewLine + myStringCollectup;
                }



                if (IsTypeParameter)
                {
                    if (doc.GetElement(new ElementId(myWindow2.myIntegerUpDown.Value.Value)) != null)  //i would rather this come from the spinner so we can select other entities
                    {
                        Element myElement = doc.GetElement(new ElementId(myWindow2.myIntegerUpDown.Value.Value)) as Element;
                        ElementType myElementType = doc.GetElement(new ElementId(myWindow2.myIntegerUpDown.Value.Value)) as ElementType;

                        if (myElement.GetType() == typeof(FamilyInstance))
                        {
                            FamilyInstance myFamilyInstance = doc.GetElement(uidoc.Selection.GetElementIds().First()) as FamilyInstance;
                            Family myFamily = ((FamilySymbol)doc.GetElement(myFamilyInstance.GetTypeId())).Family;


                            if (myFamily.IsEditable)
                            {
                                Document famDoc = null;
                                famDoc = doc.EditFamily(myFamily);

                                bool myBool_GoForthAndAddParameters = false;

                                foreach (Definition myDefinition in group.Definitions)
                                {

                                    myStringCollectup = myStringCollectup + " • " + myDefinition.Name + Environment.NewLine;
                                    if (famDoc.FamilyManager.Parameters.Cast<FamilyParameter>().Where(x => x.Definition.Name == myDefinition.Name).Count() == 0)
                                    {
                                        myBool_GoForthAndAddParameters = true;
                                    }
                                }

                                if (myBool_GoForthAndAddParameters)
                                {

                                    using (Transaction tx = new Transaction(famDoc))
                                    {
                                        tx.Start("Shared parameters to Family");

                                        foreach (ExternalDefinition eD in group.Definitions)
                                        {

                                            if (famDoc.FamilyManager.Parameters.Cast<FamilyParameter>().Where(x => x.Definition.Name == eD.Name).Count() == 0)
                                            {
                                                famDoc.FamilyManager.AddParameter(eD, BuiltInParameterGroup.PG_IDENTITY_DATA, false);
                                            }
                                        }

                                        tx.Commit();
                                    }
                                    myFamily = famDoc.LoadFamily(doc, new FamilyOption());
                                }
                                famDoc.Close(false);
                            }
                            myStringCollectup = (IsTypeParameter ? "Type" : "Instance") + " parameters added to Family:" + Environment.NewLine + myStringCollectup;
                        } else
                        {
                            using (Transaction tx = new Transaction(doc))
                            {
                                tx.Start("Shared parameters to Project");

                                CategorySet catSet2 = uidoc.Application.Application.Create.NewCategorySet();
                                catSet2.Insert(myElement.Category);

                                foreach (Definition myDefinition in group.Definitions)
                                {
                                    myStringCollectup = myStringCollectup + " • " + myDefinition.Name + Environment.NewLine;
                                    Binding binding = IsTypeParameter ? uidoc.Application.Application.Create.NewTypeBinding(catSet2) as Binding : uidoc.Application.Application.Create.NewInstanceBinding(catSet2) as Binding;

                                    doc.ParameterBindings.Insert(myDefinition, binding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                                }
                                tx.Commit();
                            }
                            myStringCollectup = (IsTypeParameter ? "Type" : "Instance") + " parameters added to Category: " + myElement.Category.Name + Environment.NewLine + myStringCollectup;
                        }
                        
                    } else
                    {
                        myStringCollectup = "Please use 'Acquire Selected' button.";
                    }
                    
                }

                if (myStringSharedParameterFileName != "")
                {
                    uidoc.Application.Application.SharedParametersFilename = myStringSharedParameterFileName;
                }

                
                return (myStringCollectup);
            }

            #region catch and finally
            catch (Exception ex)
            {
                if (myStringSharedParameterFileName != "")
                {
                    uidoc.Application.Application.SharedParametersFilename = myStringSharedParameterFileName;
                }

                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myMethod_BindParameters, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);

                return "";
            }
            finally
            {
            }
            #endregion


        }
    }

    class FamilyOption : IFamilyLoadOptions
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

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE07_Part1 : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }
        public Window2 myWindow2 { get; set; }
       
        public void Execute(UIApplication uiapp)
        {
            try
            {
                if (myWindow2.myIntegerUpDown.Value.Value == -1)
                {
                    myWindow2.myTextBoxPrevious.Text = "";
                    myWindow2.myTextBoxNew.Text = "";
                    myWindow2.myListBoxInstanceParameters.SelectedIndex = -1;
                    myWindow2.myListBoxTypeParameters.SelectedIndex = -1;
                    MessageBox.Show("Please select and 'Acquire' an entity.");
                    return;
                }

                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                Element myElement = doc.GetElement(new ElementId(myWindow2.myIntegerUpDown.Value.Value));

                if (myElement == null)
                {
                    myWindow2.myIntegerUpDown.Value = -1;
                    MessageBox.Show("Entity was removed.");
                    return;
                }

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Edit Parameters");

                    Parameter myParameter = null;

                    if (myWindow2.myListBoxInstanceParameters.SelectedIndex != -1)
                    {
                        if (((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxInstanceParameters.SelectedItem).theIsBuiltInParameter)
                        {
                            myParameter = myElement.get_Parameter(((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxInstanceParameters.SelectedItem).theBIP);
                        } else
                        {
                            if (myElement.LookupParameter(((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxInstanceParameters.SelectedItem).theParameterName) == null)
                            {
                                MessageBox.Show("'" + ((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxInstanceParameters.SelectedItem).theParameterName + "' parameter does not exist." + Environment.NewLine + Environment.NewLine + "Please click 'Add Parameters'");
                                myWindow2.myTextBoxPrevious.Text = "";
                                myWindow2.myTextBoxNew.Text = "";
                                myWindow2.myTextBoxNew.IsEnabled = false;
                                myWindow2.myBool_StayDown = true;
                                myWindow2.myListBoxInstanceParameters.SelectedIndex = -1;
                                myWindow2.myListBoxTypeParameters.SelectedIndex = -1;
                                return;
                            }

                            myParameter = myElement.GetParameters(((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxInstanceParameters.SelectedItem).theParameterName)[0];
                        }
                    }

                    ///i don't know what the last thing we werwe doing when this was finished up last night was it
                    ///one thing we can do is methodise the common  lines as per above and below
                    ///the problem is the adding of the parameters doesn't always work because it is not within a transaction

                    if (myWindow2.myListBoxTypeParameters.SelectedIndex != -1)
                    {
                        Element myElementType = doc.GetElement(myElement.GetTypeId());

                        if (((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theIsBuiltInParameter)
                        {
                            myParameter = myElementType.get_Parameter(((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theBIP);  //myListBoxTypeParameters
                        }
                        else
                        {
                            if (myElementType.LookupParameter(((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theParameterName) == null)
                            {
                                MessageBox.Show("'" + ((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theParameterName + "' type parameter does not exist.");
                                myWindow2.myTextBoxPrevious.Text = "";
                                myWindow2.myTextBoxNew.Text = "";
                                myWindow2.myTextBoxNew.IsEnabled = false;
                                myWindow2.myBool_StayDown = true;
                                myWindow2.myListBoxInstanceParameters.SelectedIndex = -1;
                                myWindow2.myListBoxTypeParameters.SelectedIndex = -1;
                                return;
                            }

                            myParameter = myElementType.GetParameters(((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theParameterName)[0];
                        }
                    }

                    if (myParameter != null)
                    {
                        myParameter.Set(myWindow2.myTextBoxNew.Text.ToString());
                        myWindow2.myTextBoxPrevious.Text = myWindow2.myTextBoxNew.Text;
                    }

                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE07_Part1" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE07_SetupRoom : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
                    tx.Start("SetupRoom");

                    List<ElementId> myFEC_DataStorage = new FilteredElementCollector(doc).OfClass(typeof(DataStorage)).WhereElementIsNotElementType().Where(x => x.Name == "Room Setup Entities").Select(x => x.Id).ToList();

                    if (myFEC_DataStorage.Count != 0)
                    {
                        doc.Delete(myFEC_DataStorage);
                        List<ElementId> myFEC_RoomSetupEntities = new FilteredElementCollector(doc).WhereElementIsNotElementType().Where(x => x.LookupParameter("Comments") != null).Where(x => x.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString() == "Room Setup Entities").Select(x => x.Id).ToList();
                        doc.Delete(myFEC_RoomSetupEntities);

                        myWindow1.myWindow4.myListView.ItemsSource = null;

                    } else
                    {
                        Element myLevel = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().First();
                        List<ElementId> myFEC_Walls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().Where(x => x.Name == "Exterior Glazing").Select(x => x.Id).ToList();

                        if (myFEC_Walls.Count == 0)
                        {
                            MessageBox.Show("Wall type Exterior  Glazing needs to exist.\n\r\n\rIt is contained in the default Construction template.");
                          
                        } else
                        {
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

                            //Element myElement_DataStorage = doc.GetElement(new ElementId(138191)) as Element;
                            
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
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE07_SetupRoom" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void PlaceAndRotateFamily(UIDocument uidoc, FamilySymbol myFamilySymbol, XYZ myXYZ_Location, double myDouble_Rotation, Element myLevel)
        {
            Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

            myFamilySymbol.Activate();
            FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(myXYZ_Location, myFamilySymbol, myLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            Line myLine = Line.CreateUnbound(myXYZ_Location, XYZ.BasisZ);

            ElementTransformUtils.RotateElement(doc, myFamilyInstance.Id, myLine, myDouble_Rotation);
            //FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(myLevel)
            myFamilyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Room Setup Entities");
            uidoc.RefreshActiveView();
        }


        private FamilySymbol myFamilyReturn_FindInModel(Document doc, int myIntOf_ListStaticFamilyNames)
        {
            Window3.ListView_Class myListViewClass = myWindow1.myWindow4.myListClass[myIntOf_ListStaticFamilyNames];

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


        private FamilySymbol myFamilyReturn_LoadExternally(Document doc, Window3.ListView_Class myListViewClass)
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

           // doc.LoadFamilySymbol

            doc.LoadFamily(myString_TempPath,  new FamilyOptionOverWrite(), out Family myFamily);
         //   doc.Regenerate();
            FamilySymbol myFamilySymbol = doc.GetElement(myFamily.GetFamilySymbolIds().First()) as FamilySymbol;
          //  List<Element> myListElement = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == myListViewClass.String_Name).ToList();

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

