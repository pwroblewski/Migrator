using Microsoft.Win32;
using Migrator.Helpers;
using Migrator.Model;
using Migrator.Model.State;
using Migrator.Services.SRTR;
using Migrator.Services.ZESTAWIENIE;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Windows;

namespace Migrator.Services
{
    public class ZestawienieService : IZestawienieService
    {
        private ZestawienieState stan = new ZestawienieState();

        #region Properties
        public ZestawienieState ZestawienieState
        {
            get { return stan; }
            set { stan = value; }
        }
        public string ViewName
        {
            get { return stan.ViewName; }
            set { stan.ViewName = value; }
        }
        public List<Zestawienie> Zestawienia
        {
            get { return stan.Zestawienia; }
            set { stan.Zestawienia = value; }
        }
        public List<ZestawienieKlas> ZestawieniaKlas
        {
            get { return stan.ZestawieniaKlas; }
            set { stan.ZestawieniaKlas = value; }
        }
        #endregion

        #region Zestawienie
        public string[] OpenFileDialog()
        {
            return Zestawienie_File.OpenFileDialog();
        }
        public void LoadData(string[] paths)
        {
            var ret = Zestawienie_File.LoadData(paths);
            Zestawienia = (List<Zestawienie>)ret[0];
            ZestawieniaKlas = (List<ZestawienieKlas>)ret[1];
        }
        #endregion
        #region Jim
        public string SaveJimFile()
        {
            return SRTR_Jim.SaveFileDialog(ZestawieniaKlas);
        }
        public string OpenJimFile()
        {
            return SRTR_Jim.OpenFileDialog();
        }
        public void AddJimData(string path)
        {
            var zestawienieKlas = SRTR_Jim.AddJimData(path, ZestawieniaKlas, Zestawienia);
            ZestawieniaKlas = zestawienieKlas;
        }
        #endregion

        public void ZapiszPliki()
        {
            if (Zestawienia != null && Zestawienia.Count > 0 && ZestawieniaKlas != null && ZestawieniaKlas.Count > 0)
            {
                SaveFileDialog saveFile = new SaveFileDialog() { FileName = "MATERIAL_", DefaultExt = ".text", Filter = "Dokumenty tekstowe (.txt)|*.txt" };

                if (saveFile.ShowDialog() == true)
                {
                    try
                    {
                        using (Stream writeStream = saveFile.OpenFile())
                        {
                            using (StreamWriter writer = new StreamWriter(writeStream))
                            {
                                foreach (Zestawienie zestawienie in Zestawienia)
                                {
                                    foreach (ZestawienieKlas zestawienieKlas in ZestawieniaKlas)
                                    {
                                        if (zestawienie.Jim.Equals(zestawienieKlas.Jim.Trim()) && zestawienieKlas.KlasaZaop != null)
                                        {
                                            writer.Write("{0}\t", zestawienieKlas.Jim.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.KlasaZaop.Trim());
                                            writer.Write("{0}\t", zestawienie.Zaklad.Trim());
                                            writer.Write("{0}\t", zestawienie.Sklad.Trim());
                                            writer.Write("{0}\t", zestawienie.Dzial.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.Nazwa.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.Jm.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.StaryNrInd.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.KlasyfikatorHier.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.GrupaKlasaKwo.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.WagaBrutto.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.WagaNetto.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.JednWagi.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.Objetosc.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.JednObj.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.Gestor.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.Norma.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.WyroznikProdNiebezp.Trim());
                                            writer.Write("{0}\t", zestawienie.SymbolKat.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.KodCpv.Trim());
                                            writer.Write("{0}\t", zestawienie.Wskaznik.Trim());
                                            writer.Write("{0}\t", zestawienie.ZapasBezp.Trim());
                                            writer.Write("{0}\t", zestawienie.TypWyceny.Trim());
                                            writer.Write("{0}\t", zestawienie.Rodzaj.Trim());
                                            writer.Write("{0}\t", zestawienie.MaterialKonto.Trim());
                                            writer.Write("{0}\t", zestawienie.StarCena.Trim());
                                            writer.Write("{0}\t", zestawienie.JednCena.Trim());
                                            writer.Write("{0}\t", zestawienie.CenaSrednia.Trim());
                                            writer.Write("{0}\t", zestawienie.CenaStand.Trim());
                                            writer.Write("{0}\t", zestawienie.ZakladDost.Trim());
                                            writer.Write("{0}\t", zestawienie.MaterialProf.Trim());
                                            writer.Write("{0}\t", zestawienieKlas.WyroznikCpv.Trim());
                                            writer.Write(writer.NewLine);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {

                    }
                }

                SaveFileDialog saveFile2 = new SaveFileDialog() { FileName = "MATERIAL_KLAS_", DefaultExt = ".text", Filter = "Dokumenty tekstowe (.txt)|*.txt" };

                if (saveFile2.ShowDialog() == true)
                {
                    try
                    {
                        using (Stream writeStream = saveFile2.OpenFile())
                        {
                            using (StreamWriter writer = new StreamWriter(writeStream))
                            {
                                foreach (Zestawienie zestawienie in Zestawienia)
                                {
                                    foreach (ZestawienieKlas zestawienieKlas in ZestawieniaKlas)
                                    {
                                        if (zestawienie.Jim.Equals(zestawienieKlas.Jim.Trim()))
                                        {
                                            writer.WriteLine("{0}\t022\t{1}", zestawienieKlas.Jim, zestawienieKlas.KlasyfikacjaPartii);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("Błąd - {0}", ex), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {

                    }
                }
            }
            else
            {
                MessageBox.Show("Brak danych!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Clean()
        {
            Zestawienia = null;
            ZestawieniaKlas = null;
        }
    }
}
