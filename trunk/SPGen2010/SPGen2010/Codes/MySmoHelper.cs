using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// SMO
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;

using My = SPGen2010.Codes.MySmo;
using SmoUtils = SPGen2010.Codes.Helpers.Utils;

namespace SPGen2010.Codes
{
    public static class MySmoHelper
    {
        public static void FillData(this My.Database mydb, Database db)
        {
            mydb.Tables.AddRange(
                from Table t in db.Tables
                where t.IsSystemObject == false
                select NewTable(mydb, t)
            );

            mydb.Views.AddRange(
                from View v in db.Views
                where v.IsSystemObject == false
                select NewView(mydb, v)
            );

            // ... 
        }

        public static My.Table NewTable(My.Database mydb, Table t)
        {
            var myt = new My.Table();
            myt.ParentDatabase = mydb;
            myt.Columns = new List<My.Column>(
                from Column c in t.Columns
                select NewColumn(mydb, myt, c)
            );
            return myt;
        }

        public static My.View NewView(My.Database mydb, View v)
        {
            var myv = new My.View();
            myv.ParentDatabase = mydb;
            myv.Columns = new List<My.Column>(
                from Column c in v.Columns
                select NewColumn(mydb, myv, c)
            );
            return myv;
        }

        public static My.Column NewColumn(My.Database mydb, My.ITableBase myt, Column c)
        {
            return new My.Column
            {
                ParentDatabase = mydb,
                ParentTableBase = myt,
                DataType = NewDataType(c.DataType),

                Computed = c.Computed,
                ComputedText = c.ComputedText,
                Default = c.Default,
                Identity = c.Identity,
                IdentityIncrement = c.IdentityIncrement,
                IdentitySeed = c.IdentitySeed,
                InPrimaryKey = c.InPrimaryKey,
                IsForeignKey = c.IsForeignKey,
                Nullable = c.Nullable,
                RowGuidCol = c.RowGuidCol
            };
        }

        public static My.DataType NewDataType(DataType dt)
        {
            return new My.DataType
            {
                Name = dt.Name,
                MaximumLength = dt.MaximumLength,
                NumericPrecision = dt.NumericPrecision,
                NumericScale = dt.NumericScale
            };
        }

        public static List<My.ExtendedProperty> NewExtendProperties(My.IExtendPropertiesBase parent, ExtendedPropertyCollection epc)
        {
            return new List<My.ExtendedProperty>(
                from ExtendedProperty ep in epc
                select new My.ExtendedProperty { Name = ep.Name, Value = ep.Value, ParentExtendPropertiesBase = parent }
            );
        }
    }

}
