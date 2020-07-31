using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _929_Bilt2020_PlaypenChild
{
    /// <summary>
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        public ExternalCommandData commandData { get; set; }
        public Window1 myWindow1 { get; set; }

        public List<Window3.ListView_Class> myListClass { get; set; } = new List<Window3.ListView_Class>();
        

        public Window4(ExternalCommandData cD)
        {
            commandData = cD;

            int eL = -1;

            try
            {
                InitializeComponent();
                this.Top = Properties.Settings.Default.Win4Top;
                this.Left = Properties.Settings.Default.Win4Top;

                foreach (string myStrrr in Families_ThatMustBeLoaded.ListStringMustHaveFamilies) myListClass.Add(new Window3.ListView_Class() { String_Name = myStrrr, String_FileName = "//Families//" + myStrrr + ".rfa" });
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("Window4, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            int eL = -1;

            try
            {
                Properties.Settings.Default.Win4Top = this.Top;
                Properties.Settings.Default.Win4Left = this.Left;
                Properties.Settings.Default.Save();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("Window_Closing, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButtonPlaceRoom_Click(object sender, RoutedEventArgs e)
        {

            int eL = -1;

            try
            {
                myWindow1.myExternalEvent_EE07_SetupRoom.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonPlaceRoom_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButtonSave_Click(object sender, RoutedEventArgs e)
        {

            int eL = -1;

            try
            {
                if(myListView.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select an item from the list view.");
                    return;
                }

                myWindow1.myEE03_Part1_NewOrSave.myBool_New = false;
                myWindow1.myExternalEvent_EE03_Part1_NewOrSave.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonSave_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion

        }


        private void myButtonNew_Click(object sender, RoutedEventArgs e)
        {

            int eL = -1;

            try
            {
                myWindow1.myEE03_Part1_NewOrSave.myBool_New = true;
                myWindow1.myExternalEvent_EE03_Part1_NewOrSave.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonNew_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int eL = -1;

            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                List<ElementId> myFEC_DataStorage = new FilteredElementCollector(doc).OfClass(typeof(DataStorage)).WhereElementIsNotElementType().Where(x => x.Name == "Room Setup Entities").Select(x => x.Id).ToList();

                if (myFEC_DataStorage.Count == 0) return;

                DataStorage myDatastorage = doc.GetElement(myFEC_DataStorage.First()) as DataStorage;

                Schema schema_FurnLocations_Index = Schema.Lookup(new Guid(Schema_FurnLocations.myConstantStringSchema_FurnLocations_Index));
                if (schema_FurnLocations_Index == null) schema_FurnLocations_Index = Schema_FurnLocations.createSchema_FurnLocations_Index();

                Entity ent_Parent = myDatastorage.GetEntity(schema_FurnLocations_Index);

                if (!ent_Parent.IsValid()) return;

                IDictionary<string, Entity> dict_Parent = ent_Parent.Get<IDictionary<string, Entity>>("FurnLocations_Index", DisplayUnitType.DUT_MILLIMETERS);

                myListView.ItemsSource = dict_Parent;
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonSave_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }


        private void ListViewItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int eL = -1;

            try
            {
                myWindow1.myExternalEvent_EE03_Part2_ShowArrangement.Raise();

                //MessageBox.Show(myKeyValuePair.Key);
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("ListViewItem_PreviewMouseDoubleClick, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion

        }

        private void myButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            int eL = -1;

            try
            {
                if (myListView.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select an item from the list view.");
                    return;
                }

                myWindow1.myExternalEvent_EE03_Part2_DeleteOne.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonDelete_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int eL = -1;

            try
            {
                myIntegerUpDown.Value = myListView.SelectedIndex + 1;
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myListView_SelectionChanged, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            int eL = -1;

            try
            {
                if (myListView.SelectedIndex == -1)
                {
                    myListView.SelectedIndex = myListView.Items.Count - 1;
                } else
                {
                    if (myListView.SelectedIndex == 0)
                    {
                        myIntegerUpDown.Value = myListView.Items.Count;
                        myListView.SelectedIndex = myListView.Items.Count - 1;
                    }
                    else
                    {
                        myIntegerUpDown.Value = myIntegerUpDown.Value - 1;
                        myListView.SelectedIndex = myIntegerUpDown.Value.Value - 1;
                    }
                }
                myWindow1.myExternalEvent_EE03_Part2_ShowArrangement.Raise();

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonPrevious_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButtonNext_Click(object sender, RoutedEventArgs e)
        {
            int eL = -1;

            try
            {
                if (myListView.SelectedIndex == -1)
                {
                    myListView.SelectedIndex = 0;
                } else
                {
                    if (myListView.SelectedIndex == myListView.Items.Count - 1)
                    {
                        myIntegerUpDown.Value = 1;
                        myListView.SelectedIndex = 0;
                    }
                    else
                    {
                        myIntegerUpDown.Value = myIntegerUpDown.Value + 1;
                        myListView.SelectedIndex = myIntegerUpDown.Value.Value - 1;
                    }
                }
                myWindow1.myExternalEvent_EE03_Part2_ShowArrangement.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonNext_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
    }
}
