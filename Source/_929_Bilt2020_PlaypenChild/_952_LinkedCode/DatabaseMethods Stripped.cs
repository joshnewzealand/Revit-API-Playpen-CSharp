using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
//using System.Data.SqlClient;   // System.Data.dll
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;


namespace _952_PRLoogleClassLibrary
{

    /*
    public class Person
    {
        [Category("Data")]
        [DisplayName("Given name")]
        public string FirstName { get; set; }

        [DisplayName("Family name")]
        public string LastName { get; set; }

        public int Age { get; set; }

        public double Height { get; set; }

        public Mass Weight { get; set; }

        public Genders Gender { get; set; }

        public Color HairColor { get; set; }

        [Description("Check the box if the person owns a bicycle.")]
        public bool OwnsBicycle { get; set; }
    }
    */


    public class DatabaseMethods
    {



           




        public static void writeDebug(string x, bool AndShow)
        {

            string path = (System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\` aa PRLGoogle Backups");
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

            //string subdirectory_reversedatetothesecond = (path + ("\\" + (DateTime.Now.ToString("yyyyMMddHHmmss"))));
            //if (!System.IO.Directory.Exists(subdirectory_reversedatetothesecond)) System.IO.Directory.CreateDirectory(subdirectory_reversedatetothesecond);

            string FILE_NAME = (path + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");

            System.IO.File.Create(FILE_NAME).Dispose();

            System.IO.StreamWriter objWriter = new System.IO.StreamWriter(FILE_NAME, true);
            objWriter.WriteLine(x);
            objWriter.Close();

            if(AndShow)System.Diagnostics.Process.Start(FILE_NAME);
        }


        /*  public static List<string> GetAllQueriesFromDataBase(OleDbConnection connRLPrivateFlexible, string theTableName)
          {
             // DataTable tables = connRLPrivateFlexible.GetSchema("Tables");

              DataRow recSpecificTable = connRLPrivateFlexible.GetSchema("Tables").AsEnumerable()
                  .Where(r => r.Field<string>("TABLE_NAME")
                  .StartsWith(theTableName)).FirstOrDefault();

              var queries = new List<string>();
              using (connRLPrivateFlexible)
              {
                  var dt = connRLPrivateFlexible.GetSchema("BASE TABLE");
                  queries = dt.AsEnumerable().Select(dr => dr.Field<string>(theTableName)).ToList();
              }

              return queries;
          }*/

  }
}
