using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using _952_PRLoogleClassLibrary;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace _929_Bilt2020_PlaypenChild
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        public ExternalCommandData commandData { get; set; }

        public class ListView_Class
        { 
            public string String_Name { get; set; }
            public string String_FileName { get; set; }
        }

        public List<ListView_Class> myListClass { get; set; } = new List<ListView_Class>();

        public Window1 myWindow1 { get; set; }

        public Window3(ExternalCommandData cD)
        {
            commandData = cD;

            int eL = -1;

            try
            {
                InitializeComponent();
                this.Top = Properties.Settings.Default.Top;
                this.Left = Properties.Settings.Default.Left;
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("Window3, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButton_PlaceThemAll_Click(object sender, RoutedEventArgs e)
        {
            int eL = -1;

            try
            {
                /*

                FamilySymbol myFamilySymbol = doc.GetElement(myFamily.GetFamilySymbolIds().First()) as FamilySymbol;

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Place a " + myListView_Class.String_Name);
                    myFamilySymbol.Activate();
                    FamilyInstance myFamilyInstance = doc.Create.NewFamilyInstance(new XYZ(70, -30, 12), myFamilySymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    doc.ProjectInformation.get_Parameter(BuiltInParameter.PROJECT_NUMBER).Set(myFamilyInstance.Id.IntegerValue.ToString());
                    tx.Commit();
                }

    */

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_PlaceThemAll_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
                //myWindow1.myExternalEvent_EE01_Part1_PlaceAFamily.Raise();

                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                Window3.ListView_Class myListView_Class = myWindow1.myWindow3.myListView.SelectedItem as Window3.ListView_Class;
                // string myString_AdaptiveCarrier = "Women Tipping Hat Man";

                IEnumerable<Element> myIEnumerableElement = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(Family))).Where(x => x.Name == myListView_Class.String_Name);

                if (myIEnumerableElement.Count() == 0)
                {
                    MessageBox.Show(myListView_Class.String_Name + Environment.NewLine + Environment.NewLine + "Is not present in model" + Environment.NewLine + "...please click the 'Load all families' button below");
                    return;
                }
                FamilySymbol myFamilySymbol_Carrier = doc.GetElement(((Family)myIEnumerableElement.First()).GetFamilySymbolIds().First()) as FamilySymbol;


                myWindow1.myEE01_Part1_PlaceAFamily.myFamilySymbol = myFamilySymbol_Carrier;
                myWindow1.myExternalEvent_EE01_Part1_PlaceAFamily.Raise();

             //   uidoc.PromptForFamilyInstancePlacement(myFamilySymbol_Carrier);
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

        ////[DllImport("user32.dll")]
        ////public static extern bool SetForegroundWindow(IntPtr hWnd);

        ////[DllImport("user32.dll", SetLastError = true)]
        ////static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);



        ////void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        ////{
        ////    try
        ////    {
        ////        _added_element_ids.AddRange(e.GetAddedElementIds());

        ////        if (e.GetAddedElementIds().Count == 0) return;

        ////        UIDocument uidoc = commandData.Application.ActiveUIDocument;
        ////        uidoc.Application.Application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

        ////        //MessageBox.Show(_added_element_ids[0].IntegerValue.ToString());

        ////        SetForegroundWindow(uidoc.Application.MainWindowHandle);
        ////        keybd_event(0x1B, 0, 0, 0);
        ////        keybd_event(0x1B, 0, 2, 0);
        ////        keybd_event(0x1B, 0, 0, 0);
        ////        keybd_event(0x1B, 0, 2, 0);
        ////    }

        ////    #region catch and finally
        ////    catch (Exception ex)
        ////    {
        ////        _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("OnDocumentChanged" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
        ////    }
        ////    finally
        ////    {
        ////    }
        ////    #endregion
        ////}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int eL = -1;

            try
            {
                // @"\Generic Adaptive Nerf Gun.rfa";  //Families

                List<string> myListString = new List<string>();
                myListString.Add("Furniture Chair Executive");
                myListString.Add("Furniture Chair Viper");
                myListString.Add("Furniture Couch Viper");
                myListString.Add("Furniture Desk");
                myListString.Add("Furniture Table Dining Round w Chairs");
                myListString.Add("Furniture Table Night Stand");
                myListString.Add("Generic Adaptive Nerf Gun");
                myListString.Add("Generic Model Tipping Hat Man");


                foreach (string myStrrr in myListString) myListClass.Add(new ListView_Class() { String_Name = myStrrr, String_FileName = "//Families//" + myStrrr + ".rfa"});

                myListView.ItemsSource = myListClass;

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("Window_Loaded, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
                Properties.Settings.Default.Win3Top = this.Top;
                Properties.Settings.Default.Win3Left = this.Left;
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

        private void myButton_LoadAllFamilies_Click(object sender, RoutedEventArgs e)
        {
            int eL = -1;

            try
            {
                myWindow1.myExternalEvent_EE05_Part1_LoadAllFamilies.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_LoadAllFamilies_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
    }
}
