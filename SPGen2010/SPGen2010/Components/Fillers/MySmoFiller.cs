using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

// SMO
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;

using SPGen2010.Components.Modules;
using My = SPGen2010.Components.Modules.MySmo;
using SmoUtils = SPGen2010.Components.Utils;

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

            // 如果有找到别的项的命名＝当前项名 + "#@!Part_" + 数字　合并，纳入删除表
            var mark_part = "#@!Part_";

            var delList = new List<string>();
            foreach (var o in eps)
            {
                if (o.Key.Contains(mark_part)) continue;      // 分页项就不处理了　直接跳过
                var s = o.Value;
                // 根据　页码　部分　从小到大 排序取当前对象
                var parts = from ep in eps 
                            where ep.Key.Contains(mark_part)
                            orderby int.Parse(ep.Key.Substring(ep.Key.LastIndexOf(mark_part) + 8))
                            select ep;
                foreach (var part in parts)
                {
                    s += part.Value;
                    delList.Add(part.Key);
                }
                if (s != o.Value) eps[o.Key] = s;
            }

            foreach (var key in delList) eps.Remove(key);       // 删除已合并键
            delList.Clear();

            // 检查到如果当前 ep 为子项配置集（有可能子对象不支持多 ep 集合或不支持 ep）时，将 ep 应用到子项

            foreach (var o in eps)
            {
                if (o.Key == "ColumnSettings")
                {
                    var t = (My.ITableBase)parent;
                    var dt = new DS.ColumnExtendedInformationsDataTable();
                    dt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(o.Value)));
                    foreach (var column in t.Columns)
                    {
                        var row = dt.FindByName(column.Name);
                        if (row != null)
                        {
                            foreach(System.Data.DataColumn dc in dt.Columns)
                            {
                                if (dc.Unique) continue;
                                var ceps = column.ExtendedProperties;
                                if (ceps.ContainsKey(dc.ColumnName)) continue;
                                ceps.Add(dc.ColumnName, (string)row[dc]);
                            }
                        }
                    }
                    delList.Add(o.Key);
                }
                else if (o.Key == "ParameterSettings")
                {
                    var t = (My.IParameterBase)parent;
                    var dt = new DS.ParameterExtendedInformationsDataTable();
                    dt.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(o.Value)));
                    foreach (var parameter in t.Parameters)
                    {
                        var row = dt.FindByName(parameter.Name);
                        if (row != null)
                        {
                            foreach (System.Data.DataColumn dc in dt.Columns)
                            {
                                if (dc.Unique) continue;
                                parameter.ExtendedProperties.Add(dc.ColumnName, (string)row[dc]);
                            }
                        }
                    }
                    delList.Add(o.Key);
                }
                // else if   ResultSettings for SP
            }

            foreach (var key in delList) eps.Remove(key);       // 删除已颁布到子元素的并键
            delList.Clear();

            return eps;
        }

        #endregion
    }
}
