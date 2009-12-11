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
