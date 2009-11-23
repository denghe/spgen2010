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
    public static class MySmoFiller
    {
        /// <summary>
        /// todo: filter fill
        /// </summary>
        public static void FillData(this My.Database mydb, Database db)
        {
            mydb.Tables = new List<My.Table>(
                from Table o in db.Tables
                where o.IsSystemObject == false
                select NewTable(mydb, o)
            );

            mydb.Views = new List<My.View>(
                from View o in db.Views
                where o.IsSystemObject == false
                select NewView(mydb, o)
            );

            mydb.UserDefinedFunctions = new List<My.UserDefinedFunction>(
                from UserDefinedFunction o in db.UserDefinedFunctions
                where o.IsSystemObject == false
                select NewUserDefinedFunction(mydb, o)
            );

            mydb.UserDefinedTableTypes = new List<My.UserDefinedTableType>(
                from UserDefinedTableType o in db.UserDefinedTableTypes
                select NewUserDefinedTableType(mydb, o)
            );

            mydb.StoredProcedures = new List<My.StoredProcedure>(
                from StoredProcedure o in db.StoredProcedures
                where o.IsSystemObject == false
                select NewStoredProcedure(mydb, o)
            );

            mydb.ExtendedProperties = NewExtendProperties(mydb, db.ExtendedProperties);
        }


        #region NewXxxxxxx Methods

        public static My.StoredProcedure NewStoredProcedure(My.Database mydb, StoredProcedure o)
        {
            return new My.StoredProcedure
            {
                Name = o.Name,
                Schema = o.Schema
            };
        }

        public static My.UserDefinedFunction NewUserDefinedFunction(My.Database mydb, UserDefinedFunction o)
        {
            var myf = new My.UserDefinedFunction();
            myf.ParentDatabase = mydb;
            myf.Columns = new List<My.Column>(
                from Column c in o.Columns
                select NewColumn(mydb, myf, c)
            );
            return myf;
        }

        public static My.UserDefinedTableType NewUserDefinedTableType(My.Database mydb, UserDefinedTableType o)
        {
            var myt = new My.UserDefinedTableType();
            myt.ParentDatabase = mydb;
            myt.Columns = new List<My.Column>(
                from Column c in o.Columns
                select NewColumn(mydb, myt, c)
            );
            return myt;
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
                Name = c.Name,
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

        public static My.ExtendedProperties NewExtendProperties(My.IExtendPropertiesBase parent, ExtendedPropertyCollection epc)
        {
            var eps = new My.ExtendedProperties { ParentExtendPropertiesBase = parent };
            foreach (ExtendedProperty ep in epc) eps.Add(ep.Name, ep.Value as string);
            // todo: 检查到如果当前 ep 为子对象的 ep 集（有可能子对象不支持多 ep 集合或不支持 ep）时，将 ep 部署到下级
            return eps;
        }

        #endregion
    }
}
