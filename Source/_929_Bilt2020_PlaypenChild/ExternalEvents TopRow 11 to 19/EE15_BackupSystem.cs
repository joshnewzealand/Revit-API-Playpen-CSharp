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


namespace _929_Bilt2020_PlaypenChild
{


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE15_BackupSystem : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }
        public FamilyInstance myFamilyInstance;

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; 

                string myString_FamilyBackups_FamilyName = "";
                Family myFamily = null;
                string myString_FamilyName = "";


                ///                          TECHNIQUE 15 OF 19
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ ONE CLICK FAMILY BACKUP SYSTEM ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                ///
                /// Interfaces and ENUM's:
                ///     IFamilyLoadOptions
                /// 
                /// 
                /// Demonstrates classes:
                ///     Directory*
                ///     StreamWriter*
                ///     StreamReader*
                /// 
                /// 
                /// Key methods:
                ///     Directory.Exists(
                ///     Directory.CreateDirectory(
                ///     Path.GetInvalidFileNameChars(
                ///     myStreamWriter_FirstZero.Write(
                ///     myStreamWriter_FirstZero.Close(
                ///     File.SetAttributes(
                ///     myStreamReader.ReadLine(
                ///     doc.EditFamily(
                ///     famDoc.FamilyManager.AddParameter
                ///     famDoc.LoadFamily(
                ///     famDoc.SaveAs(
                ///     famDoc.Close(
                ///
                ///
                /// * class is actually part of the .NET framework (not Revit API)

                if (true) //candidate for methodisation 202004251537
                {
                    if (uidoc.Selection.GetElementIds().Count != 1)
                    {
                        MessageBox.Show("Please select just one element instance.");
                        return;
                    }

                    if (doc.GetElement(uidoc.Selection.GetElementIds().First()).GetType() != typeof(FamilyInstance))
                    {
                        MessageBox.Show("Please select an element of type - 'FamilyInstance'.");
                        return;
                    }

                    myFamilyInstance = doc.GetElement(uidoc.Selection.GetElementIds().First()) as FamilyInstance;

                    myFamily = ((FamilySymbol)doc.GetElement(myFamilyInstance.GetTypeId())).Family;

                    if (!myFamily.IsEditable)
                    {
                        MessageBox.Show("Family is not editable.");
                        return;
                    }

                    string myString_FamilyBackups = System.IO.Path.GetDirectoryName(doc.PathName) + "\\Family Backups";

                    if (!System.IO.Directory.Exists(myString_FamilyBackups))
                    {
                        System.IO.Directory.CreateDirectory(myString_FamilyBackups);
                    }

                    myString_FamilyName = myFamily.Name;
                    string invalid = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());

                    foreach (char c in invalid)
                    {
                        myString_FamilyName = myString_FamilyName.Replace(c.ToString(), "");
                    }

                    myString_FamilyBackups_FamilyName = myString_FamilyBackups + "\\" + myString_FamilyName;

                    if (!System.IO.Directory.Exists(myString_FamilyBackups_FamilyName))
                    {
                        System.IO.Directory.CreateDirectory(myString_FamilyBackups_FamilyName);
                    }
                }

                string myString_FamilyBackups_FamilyName_CurrentRevision = "";
                int myInt_StreamReader = 0;

                if (true) //candidate for methodisation 202004251614
                {
                    myString_FamilyBackups_FamilyName_CurrentRevision = myString_FamilyBackups_FamilyName + "\\Current Revision - do not edit manually.txt";

                    if (!System.IO.File.Exists(myString_FamilyBackups_FamilyName_CurrentRevision))
                    {
                        System.IO.StreamWriter myStreamWriter_FirstZero = new System.IO.StreamWriter(myString_FamilyBackups_FamilyName_CurrentRevision);
                        myStreamWriter_FirstZero.Write(((int)0).ToString());
                        myStreamWriter_FirstZero.Close();
                    }
                    System.IO.File.SetAttributes(myString_FamilyBackups_FamilyName_CurrentRevision, System.IO.File.GetAttributes(myString_FamilyBackups_FamilyName_CurrentRevision) & ~System.IO.FileAttributes.ReadOnly);

                    System.IO.StreamReader myStreamReader = new System.IO.StreamReader(myString_FamilyBackups_FamilyName_CurrentRevision);
                    string myString_StreamReader = myStreamReader.ReadLine();
                    myStreamReader.Close();

                    myInt_StreamReader = int.Parse(myString_StreamReader); myInt_StreamReader++;
                }


                System.IO.StreamWriter myStreamWriter = new System.IO.StreamWriter(myString_FamilyBackups_FamilyName_CurrentRevision);
                myStreamWriter.Write((myInt_StreamReader).ToString());
                myStreamWriter.Close();

                System.IO.StreamReader myStreamReader_CheckPass = new System.IO.StreamReader(myString_FamilyBackups_FamilyName_CurrentRevision);

                string myString_TheRevision = myStreamReader_CheckPass.ReadLine();

                myStreamReader_CheckPass.Close();
                System.IO.File.SetAttributes(myString_FamilyBackups_FamilyName_CurrentRevision, System.IO.FileAttributes.ReadOnly);

                string myString_Directory03 = myString_TheRevision.PadLeft(4, '0');

                string myString_FamilyBackups_FamilyName_TheRevision = myString_FamilyBackups_FamilyName + "\\" + myString_Directory03;

                if (!System.IO.Directory.Exists(myString_FamilyBackups_FamilyName_TheRevision))
                {
                    System.IO.Directory.CreateDirectory(myString_FamilyBackups_FamilyName_TheRevision);
                }

                System.IO.File.Create(myString_FamilyBackups_FamilyName_TheRevision + "\\Restore instructions.txt").Dispose();

                System.IO.StreamWriter objWriter = new System.IO.StreamWriter(myString_FamilyBackups_FamilyName_TheRevision + "\\Restore instructions.txt", true);
                objWriter.WriteLine("To Restore: " + Environment.NewLine + "1. Open this *.rfa file in Revit." + Environment.NewLine + "2. Check it is the revision you wish to restore." + Environment.NewLine + "3. Load into Project and Close." + Environment.NewLine + Environment.NewLine + "Note: This system only increments the revision - it never 'rolls back' and it never deletes." + Environment.NewLine + Environment.NewLine + "4. THEREFORE -> Click SAVE 'FOREVER' BACKUP again, after step 3, to make sure it becomes the new revision.");
                objWriter.Close();


                Document famDoc = null;
                famDoc = doc.EditFamily(myFamily);

                if (true)
                {
                    if (famDoc.FamilyManager.Parameters.Cast<FamilyParameter>().Where(x => x.Definition.Name == "Family Revision").Count() == 0)
                    {
                        using (Transaction tx = new Transaction(famDoc))
                        {
                            tx.Start("Add Family Revision parameters");

                            famDoc.FamilyManager.AddParameter("Family Revision", BuiltInParameterGroup.INVALID, ParameterType.Integer, false); // true = instance parameter
                            myFamily = famDoc.LoadFamily(doc, new FamilyOption());

                            tx.Commit();
                        }
                    }

                    FamilySymbol myFamilySymbol = doc.GetElement(myFamily.GetFamilySymbolIds().First()) as FamilySymbol;

                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Save revision");
                        foreach (ElementId myElementID in myFamily.GetFamilySymbolIds())
                        {
                            myFamilySymbol.GetParameters("Family Revision")[0].Set(myInt_StreamReader); //we're gonna replace myFamilyType after after checking we can't remove it altogether
                        }
                        tx.Commit();
                    }
                }
                famDoc.SaveAs(myString_FamilyBackups_FamilyName_TheRevision + "\\" + myString_FamilyName + ".rfa", new SaveAsOptions() { OverwriteExistingFile = true });
                famDoc.Close(false);
                MessageBox.Show("Revsion " + myInt_StreamReader.ToString() + " saved.");
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE15_BackupSystem" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
}
