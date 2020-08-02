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
    public class EE18_UnderStandingTransforms : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            string dllModuleName = "RevitTransformSliders";
            string myString_TempPath = "";
            try
            {
                if (myWindow1.myThisApplication.messageConst.Split('|')[0] == "Button_01_Invoke01")
                {
                    myString_TempPath = myWindow1.myThisApplication.messageConst.Split('|')[1];

                    //string path = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("TARGETDIR").ToString(); ;

                    System.Reflection.Assembly objAssembly01 = System.Reflection.Assembly.Load(System.IO.File.ReadAllBytes(myString_TempPath + "\\" + dllModuleName + ".dll"));
                    string strCommandName = "ThisApplication";

                    IEnumerable<Type> myIEnumerableType = GetTypesSafely(objAssembly01);
                    foreach (Type objType in myIEnumerableType)
                    {
                        if (objType.IsClass)
                        {
                            if (objType.Name.ToLower() == strCommandName.ToLower())
                            {
                                object ibaseObject = Activator.CreateInstance(objType);
                                object[] arguments = new object[] { myWindow1.commandData, "Button_01_Invoke01|" + myString_TempPath, new ElementSet() };

                                object result = null;

                                result = objType.InvokeMember("OpenWindowForm", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, ibaseObject, arguments);

                                break;
                            }
                        }
                    }
                }


                if (myWindow1.myThisApplication.messageConst.Split('|')[0] == "Button_01_Invoke01Development")
                {

                    myString_TempPath = myWindow1.myThisApplication.messageConst.Split('|')[1];

                    System.Reflection.Assembly objAssembly01 = System.Reflection.Assembly.Load(System.IO.File.ReadAllBytes(myString_TempPath + "\\" + dllModuleName + "\\AddIn\\" + dllModuleName + ".dll"));

                    string strCommandName = "ThisApplication";

                    IEnumerable<Type> myIEnumerableType = GetTypesSafely(objAssembly01);
                    foreach (Type objType in myIEnumerableType)
                    {
                        if (objType.IsClass)
                        {
                            if (objType.Name.ToLower() == strCommandName.ToLower())
                            {
                                object ibaseObject = Activator.CreateInstance(objType);
                                object[] arguments = new object[] { myWindow1.commandData, "Button_01_Invoke01Development|" + myString_TempPath, new ElementSet() };
                                object result = null;

                                result = objType.InvokeMember("OpenWindowForm", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, ibaseObject, arguments);

                                break;
                            }
                        }
                    }
                }

            }

            #region catch and finally
            catch (Exception ex)
            {
                string pathHeader = "Please check this file (and directory) exist: " + Environment.NewLine;
                // string path = myWindow1.myThisApplication.messageConst.Split('|')[1] + @"\_929_Bilt2020_PlaypenChild";
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug(pathHeader + myString_TempPath, true);
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

        private static IEnumerable<Type> GetTypesSafely(System.Reflection.Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
        }
    }


}
