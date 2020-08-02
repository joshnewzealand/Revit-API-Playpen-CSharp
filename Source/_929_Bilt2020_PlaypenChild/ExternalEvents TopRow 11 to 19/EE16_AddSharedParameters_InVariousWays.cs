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
using Binding = Autodesk.Revit.DB.Binding;

namespace _929_Bilt2020_PlaypenChild
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE16_AddSharedParameters_InVariousWays : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }
        public Window1617_AddEditParameters myWindow2 { get; set; }
        public bool myBool_AddToProject { get; set; } = true;

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = myWindow1.commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

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
                    if (!myBool_AddToProject) myStringCollectup = myStringCollectup + myMethod_BindParameters(path + "\\PlayPenSharedParametersType.txt", true);
                    if (myBool_AddToProject) myStringCollectup = myStringCollectup + myMethod_BindParameters(path + "\\PlayPenSharedParametersInstance.txt", false);

                    MessageBox.Show(myStringCollectup);

                    if(uidoc.Selection.GetElementIds().Count > 0)
                    {
                        if (uidoc.Selection.GetElementIds().First().IntegerValue != myWindow2.myIntegerUpDown.Value.Value)
                        {
                            MessageBox.Show("Note: The 'Selected' element did not match the 'Acquired' element.");
                        }
                    }
                   
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
                       // ElementType myElementType = doc.GetElement(new ElementId(myWindow2.myIntegerUpDown.Value.Value)) as ElementType;

                        if (myElement.GetType() == typeof(FamilyInstance))
                        {
                            FamilyInstance myFamilyInstance = myElement as FamilyInstance;
                            
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
                        }
                        else
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

                    }
                    else
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

                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE17_AddSharedParameters_InVariousWays, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);

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
}
