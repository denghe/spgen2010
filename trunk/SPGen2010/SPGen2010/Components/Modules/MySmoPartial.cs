using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGen2010.Components.Helpers.MsSql;

namespace SPGen2010.Components.Modules.MySmo {
    partial class Server {
    }

    partial class Database {
        /// <summary>
        /// 返回用户定义架构列表（如果没有返回 0 长度列表）
        /// </summary>
        public List<Schema> GetUserSchemas() {
            return this.Schemas.Where(s => !(
                s.Name == "db_accessadmin"
                || s.Name == "db_backupoperator"
                || s.Name == "db_datareader"
                || s.Name == "db_datawriter"
                || s.Name == "db_ddladmin"
                || s.Name == "db_denydatareader"
                || s.Name == "db_denydatawriter"
                || s.Name == "db_owner"
                || s.Name == "db_securityadmin"
                || s.Name == "dbo"
                || s.Name == "guest"
                || s.Name == "INFORMATION_SCHEMA"
                || s.Name == "sys")).ToList();
        }
    }

    partial class Table {

        /// <summary>
        /// 返回树表的 主外键 字典（如果没有返回 0 长度字典）
        /// </summary>
        public Dictionary<Column, Column> GetTreePKFKColumns() {
            var ccs = new Dictionary<Column, Column>();
            foreach (var fk in this.ForeignKeys) {
                if (fk.ReferencedTable != this.Name || fk.ReferencedTableSchema != this.Schema) continue;
                int equaled = 0;
                foreach (var fkc in fk.Columns)		// 判断是否一个外键约束所有字段都是在当前表
                {
                    if (fkc.ParentForeignKey.ParentTable == this) equaled++;
                }
                if (equaled == fk.Columns.Count)					// 当前表为树表
                {
                    for (int i = 0; i < fk.Columns.Count; i++) {
                        var fkc = fk.Columns[i];
                        var f = this.Columns.Find(o => o.Name == fkc.Name);
                        var p = this.Columns.Find(o => o.Name == fkc.ReferencedColumn);
                        ccs.Add(p, f);
                    }
                    return ccs;
                }
            }
            return ccs;
        }

        ///// <summary>
        ///// 返回当前表的所有　父表　外键关系
        ///// </summary>
        //public List<ForeignKey> GetParentForeignKeys() {
        //    return this.ParentDatabase.Tables.Where(o => o != this).SelectMany(o => o.ForeignKeys)
        //        .Where(o => o.ReferencedTable == this.Name && o.ReferencedTableSchema == this.Schema).ToList();
        //    //return this.ForeignKeys.Where(o =>
        //    //    o.ParentTable == this && o.ReferencedTable == this.Name && o.ReferencedTableSchema == this.Schema
        //    //    || o.ParentTable != this
        //    //).ToList();
        //    //var results = new List<ForeignKey>();
        //    //var ts = this.ParentDatabase.Tables.Where(o => o.ForeignKeys.Count > 0);
        //    //foreach (var t in ts) {
        //    //    var fks = t.ForeignKeys.Where(o => o.ReferencedTable == this.Name && o.ReferencedTableSchema == this.Schema);
        //    //    results.AddRange(fks);
        //    //}
        //    //return results;
        //}

        /// <summary>
        /// 返回当前表的主键集合（如果没有返回 0 长度列表）
        /// </summary>
        public List<Column> GetPrimaryKeyColumns() {
            return (from Column c in this.Columns where c.InPrimaryKey select c).ToList();
        }

        /// <summary>
        /// 返回当前表的非主键集合（如果没有返回 0 长度列表）
        /// </summary>
        public List<Column> GetNonPrimaryKeyColumns() {
            return (from Column c in this.Columns where !c.InPrimaryKey select c).ToList();
        }

        /// <summary>
        /// 返回当前表的可比较字段集合（如果没有返回 0 长度列表）
        /// </summary>
        public List<Column> GetCompareableColumns() {
            return (from Column c in this.Columns
                    where !(c.DataType.SqlDataType == SqlDataType.Variant
                    || c.DataType.SqlDataType == SqlDataType.HierarchyId)
                    select c).ToList();
        }

        /// <summary>
        /// 返回当前表里的自增字段（如果没有返回空）
        /// </summary>
        public Column GetIdentityColumn() {
            return this.Columns.FirstOrDefault(o => o.Identity);
        }

        /// <summary>
        /// 返回当前表中的可写字段集合 （排除计算列，自增列，Timestamp 列）（如果没有返回 0 长度列表）
        /// </summary>
        public List<Column> GetWriteableColumns() {
            return this.Columns.Where(c => !(c.Computed || c.Identity || c.DataType.SqlDataType == SqlDataType.Timestamp)).ToList();  // || c.RowGuidCol
        }

        /// <summary>
        /// 返回当前表中的必写字段集合（如果没有返回 0 长度列表）
        /// </summary>
        public List<Column> GetMustWriteColumns() {
            return this.Columns.Where(c => !(c.Identity || c.Computed || c.Nullable || c.DefaultConstraint != null)).ToList();  //    || c.RowGuidCol  
        }

        /// <summary>
        /// 返回当前表中的可排序字段集合 （排除二进制，图片，文本等类型列）（如果没有返回 0 长度列表）
        /// </summary>
        public List<Column> GetSortableColumns() {
            return this.Columns.Where(c => !(
                c.DataType.SqlDataType == SqlDataType.Image
                || c.DataType.SqlDataType == SqlDataType.Binary
                || c.DataType.SqlDataType == SqlDataType.Timestamp
                || c.DataType.SqlDataType == SqlDataType.VarBinaryMax
                || c.DataType.SqlDataType == SqlDataType.VarBinary)).ToList();
        }

        /// <summary>
        /// 判断当前表是否为 树表（符合外键指向自己的条件）
        /// </summary>
        public bool CheckIsTree()
        {
            var t = this;
            if (t == null) return false;
            var pks = t.GetPrimaryKeyColumns();
            if (pks == null || pks.Count == 0)		//没有主键？
            {
                return false;
            }

            if (t.ForeignKeys.Count == 0)
            {
                return false;
            }

            foreach (ForeignKey fk in t.ForeignKeys)
            {
                if (fk.ReferencedTable != t.Name || fk.ReferencedTableSchema != t.Schema) continue;
                int equaled = 0;
                foreach (ForeignKeyColumn fkc in fk.Columns)		//判断是否一个外键约束所有字段都是在当前表
                {
                    if (fkc.ParentForeignKey.ParentTable == t) equaled++;
                }
                if (equaled == fk.Columns.Count)					//当前表为树表
                {
                    return true;
                }
            }
            return false;
        }
    }

    partial class View {
    }

    partial class UserDefinedTableType {
    }

    partial class UserDefinedFunction {
    }

    partial class StoredProcedure {
    }

    partial class Column {
        public int GetOrdinal() { if (this.ParentTableBase == null) return 0; return this.ParentTableBase.Columns.IndexOf(this); }

        /// <summary>
        /// 检查一个字段是否为所属表的外键字段之一
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool CheckIsForeignKey()
        {
            var c = this;
            Table t = (Table)c.ParentTableBase;
            foreach (ForeignKey fk in t.ForeignKeys)
            {
                foreach (ForeignKeyColumn fkc in fk.Columns)
                {
                    Column o = t.Columns.Find(a => a.Name == fkc.Name);
                    if (c == o) return true;
                }
            }
            return false;
        }
    }

    partial class Parameter {
    }

    partial class DataType {
        public override string ToString() {
            switch (this.SqlDataType) {
                case SqlDataType.Int:
                case SqlDataType.BigInt:
                case SqlDataType.Numeric:
                case SqlDataType.SmallInt:
                case SqlDataType.Money:
                case SqlDataType.TinyInt:
                case SqlDataType.SmallMoney:
                case SqlDataType.Bit:
                case SqlDataType.Float:
                case SqlDataType.Real:
                case SqlDataType.Text:
                case SqlDataType.NText:
                case SqlDataType.Image:
                case SqlDataType.Date:
                case SqlDataType.Time:
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Timestamp:
                case SqlDataType.UniqueIdentifier:
                case SqlDataType.UserDefinedTableType:
                case SqlDataType.UserDefinedDataType:
                case SqlDataType.UserDefinedType:
                case SqlDataType.Geography:
                case SqlDataType.Geometry:
                case SqlDataType.HierarchyId:
                case SqlDataType.Xml:
                case SqlDataType.Variant:
                case SqlDataType.SysName:
                    return this.Name.ToUpper();

                case SqlDataType.Decimal:
                    return this.Name.ToUpper() + " (" + this.NumericPrecision.ToString() + "," + this.NumericScale.ToString() + ")";

                default:
                    return this.Name.ToUpper() + "(" + (this.MaximumLength == -1 ? "MAX" : this.MaximumLength.ToString()) + ")";
            }
        }
    }

    partial class ExtendedProperties {
    }


    public static partial class Extensions
    {
        public static Table Find(this IEnumerable<Table> tables, string name, string schema)
        {
            return tables.FirstOrDefault(o => o.Name == name && o.Schema == schema);
        }

        public static Column Find(this IEnumerable<Column> columns, string name)
        {
            return columns.FirstOrDefault(o => o.Name == name);
        }

    }
}
