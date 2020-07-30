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
                    myStringCollectup = myStringCollectup + myMethod_BindParameters(path + "\\PlayPenSharedParametersType.txt", true) + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                    myStringCollectup = myStringCollectup + myMethod_BindParameters(path + "\\PlayPenSharedParametersInstance.txt", false);

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

            uidoc.Application.Application.SharedParametersFilename = path;

            DefinitionFile myDefinitionFile = uidoc.Application.Application.OpenSharedParameterFile();

            if (myStringSharedParameterFileName != "")
            {
                uidoc.Application.Application.SharedParametersFilename = myStringSharedParameterFileName;
            }

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
            foreach (Definition myDefinition in group.Definitions)
            {
                myStringCollectup = myStringCollectup + " • " + myDefinition.Name;
                Binding binding = IsTypeParameter ? uidoc.Application.Application.Create.NewTypeBinding(catSet) as Binding : uidoc.Application.Application.Create.NewInstanceBinding(catSet) as Binding;

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("EE07_Part1_Binding");
                    doc.ParameterBindings.Insert(myDefinition, binding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                    tx.Commit();
                }
            }

            myStringCollectup = (IsTypeParameter ? "Type" : "Instance") + " parameters added to all categories:" + Environment.NewLine + myStringCollectup;
            return(myStringCollectup);
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
                if (myWindow2.myLabelElementID.Content.ToString() == "null")
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

                Element myElement = doc.GetElement(new ElementId(int.Parse(myWindow2.myLabelElementID.Content.ToString())));

                if (myElement == null)
                {
                    myWindow2.myLabelElementID.Content = "null";
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
                                MessageBox.Show("'" + ((Window2.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theParameterName + "' parameter does not exist.");
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

                                uidoc.RefreshActiveView();
                            }

                            //Element myElement_DataStorage = doc.GetElement(new ElementId(138191)) as Element;
                            
                            DataStorage myDatastorage = DataStorage.Create(doc);
                            myDatastorage.Name = "Room Setup Entities";
                            //DatabaseMethods.writeDebug(myDatastorage.Id.IntegerValue.ToString(), true);

                            if (true)
                            {
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 0); //Furniture Chair Executive
                                myFamilySymbol.Activate();
                                FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(new XYZ(90, -4, 0), myFamilySymbol, myLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                //FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(myLevel)
                                myFamilyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Room Setup Entities");
                                uidoc.RefreshActiveView();
                            }

                            if (true)
                            {
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 1); //Furniture Couch Viper
                                myFamilySymbol.Activate();
                                FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(new XYZ(90, -8, 0), myFamilySymbol, myLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                myFamilyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Room Setup Entities");
                                uidoc.RefreshActiveView();
                            }

                            if (true)
                            {
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 2); //Furniture Desk
                                myFamilySymbol.Activate();
                                FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(new XYZ(90, -12, 0), myFamilySymbol, myLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                myFamilyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Room Setup Entities");
                                uidoc.RefreshActiveView();
                            }

                            if (true)
                            {
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 3); //Furniture Table Dining Round w Chairs
                                myFamilySymbol.Activate();
                                FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(new XYZ(90, -16, 0), myFamilySymbol, myLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                myFamilyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Room Setup Entities");
                                uidoc.RefreshActiveView();
                            }

                            if (true)
                            {
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 4); //Furniture Table Night Stand
                                myFamilySymbol.Activate();
                                FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(new XYZ(90, -20, 0), myFamilySymbol, myLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                myFamilyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Room Setup Entities");
                                uidoc.RefreshActiveView();
                            }

                            if (true)
                            {
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 5); //Generic Adaptive Nerf Gun
                                myFamilySymbol.Activate();
                                FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(new XYZ(90, -24, 5), myFamilySymbol, myLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                myFamilyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Room Setup Entities");
                                uidoc.RefreshActiveView();
                            }

                            if (true)
                            {
                                FamilySymbol myFamilySymbol = myFamilyReturn_FindInModel(doc, 6); //Generic Model Tipping Hat Man
                                myFamilySymbol.Activate();
                                FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(new XYZ(90, -28, 0), myFamilySymbol, myLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                myFamilyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Room Setup Entities");
                                uidoc.RefreshActiveView();
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

