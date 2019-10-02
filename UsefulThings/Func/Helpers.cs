using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Microsoft;

namespace UsefulThings
{
    public class Helpers
    {
        public static StringCollection GetStringCollection(string list)
        {
            StringCollection coll = new StringCollection();

            if (list != null)
            {
                string[] s = list.Split(new Char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string str in s)
                {

                    char[] charsToTrim = { '\n', '\r' };
                    string str_trim = str.Trim(charsToTrim);
                    if (str_trim != null && str_trim != "" && str_trim != String.Empty)
                    {
                        coll.Add(str_trim);
                    }
                }
            }

            return coll;
        }

        public static StringCollection GetStringCollectionFromFile()
        {
            StringCollection coll = new StringCollection();
            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = "All files (*.*)|*.*";
                Nullable<bool> result = dialog.ShowDialog();

                if (result == true)
                {
                    string filename = dialog.FileName;
                    string line;
                    StreamReader file = new System.IO.StreamReader(@filename);
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line == String.Empty || line == "" || line == null)
                        {
                            continue;
                        }
                        coll.Add(line);
                    }
                    file.Close();
                }  
            }
            catch (Exception ex)
            {
                L.LW(ex);
            }
            return coll;
        }
    }
}
