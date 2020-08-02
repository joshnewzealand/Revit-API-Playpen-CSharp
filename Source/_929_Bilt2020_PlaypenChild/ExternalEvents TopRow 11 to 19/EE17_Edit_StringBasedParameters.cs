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
    public class EE17_Edit_StringBasedParameters : IExternalEventHandler
    {
        public MainWindow myWindow1 { get; set; }
        public Window1617_AddEditParameters myWindow2 { get; set; }

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
                        if (((Window1617_AddEditParameters.aBuiltInParameter_and_Name)myWindow2.myListBoxInstanceParameters.SelectedItem).theIsBuiltInParameter)
                        {
                            myParameter = myElement.get_Parameter(((Window1617_AddEditParameters.aBuiltInParameter_and_Name)myWindow2.myListBoxInstanceParameters.SelectedItem).theBIP);
                        }
                        else
                        {
                            if (myElement.LookupParameter(((Window1617_AddEditParameters.aBuiltInParameter_and_Name)myWindow2.myListBoxInstanceParameters.SelectedItem).theParameterName) == null)
                            {
                                MessageBox.Show("'" + ((Window1617_AddEditParameters.aBuiltInParameter_and_Name)myWindow2.myListBoxInstanceParameters.SelectedItem).theParameterName + "' parameter does not exist." + Environment.NewLine + Environment.NewLine + "Please click the LEFT 'Add Shared Parameters' button");
                                myWindow2.myTextBoxPrevious.Text = "";
                                myWindow2.myTextBoxNew.Text = "";
                                myWindow2.myTextBoxNew.IsEnabled = false;
                                myWindow2.myBool_StayDown = true;
                                myWindow2.myListBoxInstanceParameters.SelectedIndex = -1;
                                myWindow2.myListBoxTypeParameters.SelectedIndex = -1;
                                return;
                            }

                            myParameter = myElement.GetParameters(((Window1617_AddEditParameters.aBuiltInParameter_and_Name)myWindow2.myListBoxInstanceParameters.SelectedItem).theParameterName)[0];
                        }
                    }

                    ///i don't know what the last thing we werwe doing when this was finished up last night was it
                    ///one thing we can do is methodise the common  lines as per above and below
                    ///the problem is the adding of the parameters doesn't always work because it is not within a transaction

                    if (myWindow2.myListBoxTypeParameters.SelectedIndex != -1)
                    {
                        Element myElementType = doc.GetElement(myElement.GetTypeId());

                        if (((Window1617_AddEditParameters.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theIsBuiltInParameter)
                        {
                            myParameter = myElementType.get_Parameter(((Window1617_AddEditParameters.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theBIP);  //myListBoxTypeParameters
                        }
                        else
                        {
                            if (myElementType.LookupParameter(((Window1617_AddEditParameters.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theParameterName) == null)
                            {
                                MessageBox.Show("'" + ((Window1617_AddEditParameters.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theParameterName + "' type parameter does not exist." + Environment.NewLine + Environment.NewLine + "Please click the RIGHT 'Add Shared Parameters' button");
                                myWindow2.myTextBoxPrevious.Text = "";
                                myWindow2.myTextBoxNew.Text = "";
                                myWindow2.myTextBoxNew.IsEnabled = false;
                                myWindow2.myBool_StayDown = true;
                                myWindow2.myListBoxInstanceParameters.SelectedIndex = -1;
                                myWindow2.myListBoxTypeParameters.SelectedIndex = -1;
                                return;
                            }

                            myParameter = myElementType.GetParameters(((Window1617_AddEditParameters.aBuiltInParameter_and_Name)myWindow2.myListBoxTypeParameters.SelectedItem).theParameterName)[0];
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
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE17_Edit_StringBasedParameters" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
