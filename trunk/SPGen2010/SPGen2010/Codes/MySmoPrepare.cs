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
            // foreach columns
        }

        public static void Prepare(this View o)
        {
            o.PrepareExtendedInformation();
            // foreach columns
        }

        public static void Prepare(this UserDefinedFunction o)
        {
            o.PrepareExtendedInformation();
            // foreach columns
            // foreach parameters
        }

        public static void Prepare(this UserDefinedTableType o)
        {
            o.PrepareExtendedInformation();
            // foreach columns
        }

        public static void Prepare(this StoredProcedure o)
        {
            o.PrepareExtendedInformation();
            // foreach parameters
        }

        // todo
        public static void Prepare(this Column o)
        {
            // 从 Table 的 ExtendProperties 中取出 Columns 的除了 MS_Description 的配置项并分别写入 Columns 的 ExtendProperties
            if (o.ParentTableBase is Table) { }
            // 从 Parent 的 ExtendProperties 中取出 Columns 的配置项并分别写入 Columns 的 ExtendProperties
            else { }
        }

        // todo
        public static void Prepare(this Parameter o)
        {
            // 从 Parent 的 ExtendProperties 中取出 Parameters 的配置项并分别写入 Parameters 的 ExtendProperties
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
