using Microsoft.Win32;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Migrator.Services
{
    public class FileWykazIlosciowyService : IFileWykazIlosciowyService
    {
        private List<WykazIlosciowy> _listWykazIlosciowy = new List<WykazIlosciowy>();

        public string OpenFileDialog()
        {
            OpenFileDialog accessDialog = new OpenFileDialog() { DefaultExt = "prn", Filter = "Text files (*.prn)|*.prn|All Files (*.*)|*.*", AddExtension = true };

            if (accessDialog.ShowDialog() == true)
            {
                string safeFileName = accessDialog.SafeFileName;

                return accessDialog.FileName;
            }
            else
                return string.Empty;
        }

        public List<WykazIlosciowy> GetAll(string path)
        {
            Clean();

            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding(1250)))
            {
                string line = null;
                string prevLine = null;

                while ((line = sr.ReadLine()) != null)
                {
                    if (!line.Equals("") && line[0].Equals('|') && !line[1].Equals('=') && !line[1].Equals('-') && !line[2].Equals('L'))
                    {
                        if (!line.Substring(0, 12).Equals("|     |NUMER") && !line.Substring(0, 12).Equals("|     |INDEK"))
                        {
                            if (line.Substring(0, 7).Equals("|     |"))
                            {
                                line = line.Remove(0, 7);
                                string[] subLines = prevLine.Split('|');
                                string[] subLines2 = line.Split('|');
                                string temp = String.Format("{0:0.00}", Convert.ToDouble(subLines[6].Trim().Replace('.', ' ')));
                                string temp2 = String.Empty;
                                if (subLines2.Length == 9)
                                {
                                    temp2 = String.Format("{0:0.00}", Convert.ToDouble(subLines2[5].Trim().Replace('.', ' ')));
                                }
                                else
                                {
                                    temp2 = String.Format("{0:0.00}", Convert.ToDouble(subLines2[4].Trim().Replace('.', ' ')));
                                }

                                _listWykazIlosciowy.Add(new WykazIlosciowy()
                                {
                                    NrInwentarzowy = subLines[2].Trim(),
                                    WartoscPoczatkowa = temp,
                                    IndeksMaterialowy = subLines2[0].Trim(),
                                    Umorzenie = temp2

                                });
                            }
                            else
                            {
                                prevLine = line;
                            }
                        }
                    }
                }

                return _listWykazIlosciowy;
            }
        }

        public void Clean()
        {
            _listWykazIlosciowy.Clear();
        }
    }
}
