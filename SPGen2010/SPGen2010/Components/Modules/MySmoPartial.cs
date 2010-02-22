using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGen2010.Components.Helpers.MsSql;

namespace SPGen2010.Components.Modules.MySmo
{
    partial class Server
    {
    }

    partial class Database
    {
    }

    partial class Table
    {
        /// <summary>
        /// 返回树表的 主外键 字典（如果没有返回 0 长度字典）
        /// </summary>
        public Dictionary<Column, Column> GetTreePKFKColumns()
        {
            var ccs = new Dictionary<Column, Column>();
            foreach (var fk in this.ForeignKeys)
            {
                if (fk.ReferencedTable != this.Name || fk.ReferencedTableSchema != this.Schema) continue;
                int equaled = 0;
                foreach (var fkc in fk.Columns)		// 判断是否一个外键约束所有字段都是在当前表
                {
                    if (fkc.ParentForeignKey.ParentTable == this) equaled++;
                }
                if (equaled == fk.Columns.Count)					// 当前表为树表
                {
                    for (int i = 0; i < fk.Columns.Count; i++)
                    {
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

        public List<Column> GetPKColumns()
        {
            return (from Column c in this.Columns where c.InPrimaryKey select c).ToList();
        }
        public List<Column> GetNonPKColumns()
        {
            return (from Column c in this.Columns where !c.InPrimaryKey select c).ToList();
        }

    }

    partial class View
    {
    }

    partial class UserDefinedTableType
    {
    }

    partial class UserDefinedFunction
    {
    }

    partial class StoredProcedure
    {
    }

    partial class Column
    {
    }

    partial class Parameter
    {
    }

    partial class DataType
    {
        public override string ToString()
        {
            switch (this.SqlDataType)
            {
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

    partial class ExtendedProperties
    {
    }

}
