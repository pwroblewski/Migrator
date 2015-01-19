﻿using Microsoft.Win32;
using Migrator.Helpers;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Text;

namespace Migrator.Services.ZESTAWIENIE
{
    public static class Zestawienie_File
    {
        public static string[] OpenFileDialog()
        {
            OpenFileDialog accessDialog = new OpenFileDialog() { DefaultExt = "txt", Filter = "Database files (*.txt)|*.txt|All Files (*.*)|*.*", AddExtension = true, Multiselect = true };

            if (accessDialog.ShowDialog() == true)
            {
                string safeFileName = accessDialog.SafeFileName;

                return accessDialog.FileNames;
            }
            else
                return null;
        }

        public static object[] LoadData(string[] paths)
        {
            object[] obj = new object[2];
            List<string> list_jim = new List<string>();
            List<Zestawienie> zestawienia = new List<Zestawienie>();
            List<ZestawienieKlas> zestawieniaKlas = new List<ZestawienieKlas>();

            for (int i = 0; i < paths.Length; i++)
            {
                using (StreamReader sr = new StreamReader(paths[i], Encoding.GetEncoding(1250)))
                {
                    string fileName = Path.GetFileName(paths[i]);
                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 17)
                        {
                            string jim = line.Substring(0, 18).Trim();
                            if (!list_jim.Contains(line.Substring(0, 18).Trim()))
                            {
                                list_jim.Add(jim);
                                string zaklad = fileName.Substring(0, 4);
                                string sklad = fileName.Substring(5, 4);
                                // TO DO obsługa czytania z pliku uzytkownika
                                string uzytkownik = string.Empty;

                                if (fileName[3].Equals('K'))
                                {
                                    // Materiał z EWPB 319/320
                                    uzytkownik = line.Split('\t')[line.Split('\t').Length - 1].Trim();
                                }

                                zestawienia.Add(new Zestawienie(jim, zaklad, sklad, uzytkownik));
                                zestawieniaKlas.Add(new ZestawienieKlas() { Jim = jim });
                            }
                        }
                    }
                }
            }

            obj[0] = zestawienia;
            obj[1] = zestawieniaKlas;

            return obj;
        }
    }
}
