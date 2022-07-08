using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;
using System.Diagnostics;

namespace KeyPaste
{
    [Serializable]
    public class Settings
    {
        public static string DefaultSettingsPath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "KeyPaste",
                    "Settings.xml"
                    );
            }
        }

        public static Settings Current { get; set; }

        #region Settings
        // Add settings
        #endregion

        public static Settings Load(string path = null)
        {
            if (path == null)
                path = DefaultSettingsPath;

            XmlSerializer xs = new XmlSerializer(typeof(Settings));
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    return (Settings)xs.Deserialize(fs);
                }
            }
            catch (IOException)
            {
                return new Settings();
            }
        }

        public void Save(string path = null)
        {
            if (path == null)
                path = DefaultSettingsPath;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Settings));
                using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write))
                {
                    xs.Serialize(fs, this);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }
    }
}
