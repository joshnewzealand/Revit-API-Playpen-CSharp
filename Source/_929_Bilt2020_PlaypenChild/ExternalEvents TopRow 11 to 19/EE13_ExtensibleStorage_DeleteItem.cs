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
    public class EE13_ExtensibleStorage_DeleteItem : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                List<ElementId> myFEC_DataStorage = new FilteredElementCollector(doc).OfClass(typeof(DataStorage)).WhereElementIsNotElementType().Where(x => x.Name == "Room Setup Entities").Select(x => x.Id).ToList();

                if (myFEC_DataStorage.Count == 0)
                {
                    MessageBox.Show("Data storage entity is not present.");
                    return;
                }

                DataStorage myDatastorage = doc.GetElement(myFEC_DataStorage.First()) as DataStorage;

                Schema schema_FurnLocations_Index = Schema.Lookup(new Guid(Schema_FurnLocations.myConstantStringSchema_FurnLocations_Index));
                if (schema_FurnLocations_Index == null) schema_FurnLocations_Index = Schema_FurnLocations.createSchema_FurnLocations_Index();

                Entity ent_Parent = myDatastorage.GetEntity(schema_FurnLocations_Index);
                IDictionary<string, Entity> dict_Parent = ent_Parent.Get<IDictionary<string, Entity>>("FurnLocations_Index", DisplayUnitType.DUT_MILLIMETERS);

                KeyValuePair<string, Entity> myKeyValuePair = (KeyValuePair<string, Entity>)myWindow1.myWindow4.myListViewEE.SelectedItem;
                dict_Parent.Remove(myKeyValuePair.Key);
                ent_Parent.Set<IDictionary<string, Entity>>("FurnLocations_Index", dict_Parent, DisplayUnitType.DUT_MILLIMETERS);

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Delete Arrangement");

                    myDatastorage.SetEntity(ent_Parent);

                    tx.Commit();
                }

                myWindow1.myWindow4.myListViewEE.ItemsSource = dict_Parent;
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE13_ExtensibleStorage_Delete" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
