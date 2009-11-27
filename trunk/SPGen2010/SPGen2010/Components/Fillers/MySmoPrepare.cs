using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGen2010.Codes.MySmo
{
    public static class MySmoPrepare
    {
        public static void Prepare(this Database db)
        {
            foreach (var o in db.Tables) o.Prepare();
            foreach (var o in db.Views) o.Prepare();
            foreach (var o in db.UserDefinedFunctions) o.Prepare();
            foreach (var o in db.UserDefinedTableTypes) o.Prepare();
            foreach (var o in db.StoredProcedures) o.Prepare();
        }


        #region Methods

        public static void Prepare(this Table o)
        {
            o.PrepareExtendedInformation();
            foreach (var c in o.Columns) c.PrepareExtendedInformation();
        }

        public static void Prepare(this View o)
        {
            o.PrepareExtendedInformation();
            foreach (var c in o.Columns) c.PrepareExtendedInformation();
        }

        public static void Prepare(this UserDefinedFunction o)
        {
            o.PrepareExtendedInformation();
            foreach (var c in o.Columns) c.PrepareExtendedInformation();
            foreach (var p in o.Parameters) p.PrepareExtendedInformation();
        }

        public static void Prepare(this UserDefinedTableType o)
        {
            o.PrepareExtendedInformation();
            foreach (var c in o.Columns) c.PrepareExtendedInformation();
        }

        public static void Prepare(this StoredProcedure o)
        {
            o.PrepareExtendedInformation();
            foreach (var p in o.Parameters) p.PrepareExtendedInformation();

            // todo result 描述
        }

        public static void PrepareExtendedInformation(this IExtendedInformation o)
        {
            o.Caption = o.GetDescription("Caption");
            o.Summary = o.GetDescription("Summary");
            o.Description = o.GetDescription("MS_Description");
        }

        public static string GetDescription(this IExtendedInformation iei, string key)
        {
            var ep = (IExtendPropertiesBase)iei;
            if (!ep.ExtendedProperties.ContainsKey(key)) return "";
            return ep.ExtendedProperties[key] ?? "";
        }

        #endregion
    }
}
