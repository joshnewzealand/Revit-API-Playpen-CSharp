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


namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE05_LoadAllFamilies : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }

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

                foreach (Window0506_LoadAndPlaceFamilies.ListView_Class myListView_Class in myWindow1.myListClass)
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

                string myStringStart = myInt.ToString() + " families have been loaded: " + Environment.NewLine + Environment.NewLine;

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

}
