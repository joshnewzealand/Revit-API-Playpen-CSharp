/*
 * Created by SharpDevelop.
 * User: Joshua
 * Date: 22/02/2020
 * Time: 3:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("48EA8AE3-58FF-4CD2-8D6E-4E6CE41A2CDF")]
	public partial class ThisApplication
	{
		private void Module_Startup(object sender, EventArgs e)
		{

		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}

        public string messageConst { get; set; }
        public MainWindow myWindow1 { get; set; }
        //this is is the method that invoked from outside
        public Result Start(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            messageConst = message;

            try
            {
                myWindow1 = new MainWindow(commandData, this);

                myWindow1.Show();
            }

            #region catch and finally
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;
        }

        public Result DrawWallTypes(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            messageConst = message;

            try
            {
                myWindow1 = new MainWindow(commandData, this);

                myWindow1.myExternalEvent_EE02_OneOfEachWall.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;
        }

        public Result SetDefault(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            messageConst = message;

            try
            {
                myWindow1 = new MainWindow(commandData, this);

                myWindow1.myExternalEvent_EE03_SetDefaultType.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;
        }

        public Result ManualOverrideColor(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            messageConst = message;

            try
            {
                myWindow1 = new MainWindow(commandData, this);

                myWindow1.myExternalEvent_EE04_ManualColorOverride.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;
        }

        public Result ExtensibleStorage(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            messageConst = message;

            myWindow1 = new MainWindow(commandData, this);

            myWindow1.myWindow4 = new Window1213_ExtensibleStorage(commandData);

            try
            {
                myWindow1.myWindow4.myWindow1 = myWindow1;
                myWindow1.myWindow4.Topmost = true;
                //myWindow1.myWindow4.Owner = myWindow1;
                myWindow1.myWindow4.Show();
            }


            #region catch and finally
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;
        }

        public Result SingleClickFamilyBackup(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            messageConst = message;

            try
            {
                myWindow1 = new MainWindow(commandData, this);

                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                if (doc.PathName == "")
                {
                    MessageBox.Show("Please save project file.");
                    return Result.Succeeded;
                }

                myWindow1.myExternalEvent_EE15_BackupSystem.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;
        }

        public Result AddEditParameters(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            messageConst = message;

            myWindow1 = new MainWindow(commandData, this);

            Window1617_AddEditParameters myWindowWindow1617 = new Window1617_AddEditParameters(commandData);
            try
            {
                myWindowWindow1617.myWindow1 = myWindow1;
                myWindowWindow1617.Topmost = true;
                //myWindowWindow1617.Owner = this;
                myWindowWindow1617.Show();
            }

            #region catch and finally
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;
        }

        public Result UnderstandingTransforms(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            messageConst = message;

            try
            {
                myWindow1 = new MainWindow(commandData, this);

                myWindow1.myExternalEvent_EE18_UnderStandingTransforms.Raise();

            }

            #region catch and finally
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;
        }


        #region Revit Macros generated code
        private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		#endregion
	}
}