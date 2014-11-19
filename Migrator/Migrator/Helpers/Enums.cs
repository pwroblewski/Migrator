using System;
using System.Collections.Generic;
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
        Magmat305,
        Ewpb319_320,
        EWpb351
    }

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