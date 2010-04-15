using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SPGen2010.Components.Modules.MySmo;
using System.IO;

namespace SPGen2010.Components.Generators.Extensions.MsSql
{
    /// <summary>
    /// generate extensions for mysmo
    /// </summary>
    public static partial class Extensions
    {
        #region EscapeTo...

        /// <summary>
        /// 取转义后的数据库对象名（标识符）
        /// </summary>
        public static string EscapeToSqlName(this string s)
        {
            return s.Replace("]", @"]]");
        }

        /// <summary>
        /// 取转义后的 TSQL 过程，函数的参数名称字串（过滤非法字符）
        /// </summary>
        public static string EscapeToParmName(this string s)
        {
            return s.Replace(' ', '_')
                .Replace(',', '_')
                .Replace('.', '_')
                .Replace(';', '_')
                .Replace(':', '_')
                .Replace('~', '_')
                .Replace('(', '_')
                .Replace(')', '_')
                .Replace('#', '_')
                .Replace('\\', '_')
                .Replace('/', '_')
                .Replace('=', '_')
                .Replace('>', '_')
                .Replace('<', '_')
                .Replace('+', '_')
                .Replace('-', '_')
                .Replace('*', '_')
                .Replace('%', '_')
                .Replace('&', '_')
                .Replace('|', '_')
                .Replace('^', '_')
                .Replace('\'', '_')
                .Replace('"', '_')
                .Replace('[', '_')
                .Replace(']', '_')
                .Replace('!', '_')
                .Replace('@', '_')
                .Replace('$', '_');
        }

        #endregion

        #region Get SP's ParmDeclareStr

        /// <summary>
        /// 根据字段数据类型取过程/函数参数声明字串
        /// </summary>
        public static string GetParmDeclareStr(this Column c)
        {
            switch (c.DataType.SqlDataType)
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
                    return c.DataType.Name.ToUpper();

                case SqlDataType.Decimal:
                    return c.DataType.Name.ToUpper() + " (" + c.DataType.NumericPrecision.ToString() + "," + c.DataType.NumericScale.ToString() + ")";

                default:
                    return c.DataType.Name.ToUpper() + "(" + (c.DataType.MaximumLength == -1 ? "MAX" : c.DataType.MaximumLength.ToString()) + ")";
            }
        }

        #endregion

        #region CheckIs

        #region CheckIsStringType

        /// <summary>
        /// 判断一个数据类型是否为 “字串类”
        /// </summary>
        public static bool CheckIsStringType(this DataType dt)
        {
            SqlDataType sdt = dt.SqlDataType;
            if (sdt == SqlDataType.UserDefinedDataType)
                throw new Exception("not Implementation");
            return (sdt == SqlDataType.Char
                    || sdt == SqlDataType.Text
                    || sdt == SqlDataType.VarChar
                    || sdt == SqlDataType.NChar
                    || sdt == SqlDataType.NText
                    || sdt == SqlDataType.NVarChar
                    || sdt == SqlDataType.NVarCharMax
                    || sdt == SqlDataType.VarCharMax
                    || sdt == SqlDataType.Xml);

            // todo: sql08 hierachyid
        }

        #endregion

        #endregion
    }
}
