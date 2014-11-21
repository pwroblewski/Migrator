using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Helpers
{
    public enum Modul
    {
        SRTR,
        MAGMAT_EWPB,
        ZESTAWIENIE
    }

    public enum MagmatEWPB
    {
        Magmat_305,
        EWPB_319_320,
        EWPB_351
    }

    //public sealed class MagmatEWPB
    //{
    //    private readonly string name;

    //    public static readonly MagmatEWPB Magmat305 = new MagmatEWPB("MAGMAT - 305");
    //    public static readonly MagmatEWPB Ewpb319_320 = new MagmatEWPB("EWPB - 319/320");
    //    public static readonly MagmatEWPB EWpb351 = new MagmatEWPB("EWPB - 351");

    //    private MagmatEWPB(string name)
    //    {
    //        this.name = name;
    //    }

    //    public override string ToString()
    //    {
    //        return name;
    //    }
    //}

    public enum SRTRPages
    {
        SrtrLoadKartoteka,
        SrtrUserConversion,
        SrtrGroupGus,
        SrtrWykaz,
        SrtrJim,
        SrtrPlikWynikowy
    }

    public enum MagmatEWPBPages
    {
        MagChooseType,
        MagFillData,
        MagFillDictionary,
        MagJimData,
        MagSigmat
    }

    public enum ZestawieniePages
    {
        ZestawienieLoadFiles,
        ZestawienieJimData,
        ZestawieniePlikWynikowy
    }

    public enum Dictionaries
    {
        Amortyzacja,
        GrupaGUS,
        JednstkaMiary,
        Jednostki,
        Uzytkownicy,
        Magazyny
    }
}