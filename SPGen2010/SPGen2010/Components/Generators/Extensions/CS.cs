using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SPGen2010.Components.Modules.MySmo;
using System.IO;

namespace SPGen2010.Components.Generators.Extensions.CS
{
    /// <summary>
    /// generate extensions for mysmo
    /// </summary>
    public static partial class Extensions
    {
        #region ToSummary

        /// <summary>
        /// 返回为 代码段的 summary 部分而格式化的备注输出格式。每一行的前面带三个斜杠
        /// todo: 内容转义
        /// </summary>
        public static string ToSummary(this string o)
        {
            return ToSummary(o, string.Empty, 2);
        }

        /// <summary>
        /// 返回为 代码段的 summary 部分而格式化的备注输出格式。每一行的前面带三个斜杠
        /// todo: 内容转义
        /// </summary>
        public static string ToSummary(this string o, int numTabs)
        {
            return ToSummary(o, string.Empty, numTabs);
        }

        /// <summary>
        /// 返回为 代码段的 summary 部分而格式化的备注输出格式。每一行的前面带三个斜杠。内容最后面附加一些字串，前面可空 numTabs 个 Tab 符
        /// todo: 内容转义
        /// </summary>
        public static string ToSummary(this string o, string attach, int numTabs)
        {
            var str = o + attach;
            var tabs = new string('\t', numTabs);
            if (string.IsNullOrEmpty(str))
            {
                return @"
" + tabs + @"/// <summary>
" + tabs + @"/// 
" + tabs + @"/// </summary>";
            }
            var sb = new StringBuilder();
            sb.Append(@"
" + tabs + @"/// <summary>");
            using (var tr = new StringReader(str))
            {
                while (true)
                {
                    var s = tr.ReadLine();
                    if (s == null) break;
                    if (s.Contains("--"))
                    {
                        if (s.StartsWith("-- ============================")) continue;
                    }
                    sb.Append(@"
" + tabs + @"/// " + s);
                }
            }
            sb.Append(@"
" + tabs + @"/// </summary>");
            return sb.ToString();
        }

        #endregion

        #region GetTypeName

        /// <summary>
        /// 返回一个字段数据类型所对应的 C# 对象类型（可空类型）
        /// </summary>
        public static string GetNullableTypeName(this DataType o)
        {
            switch (o.SqlDataType)
            {
                case SqlDataType.Bit:
                    return "bool?";
                case SqlDataType.TinyInt:
                    return "byte?";
                case SqlDataType.SmallInt:
                    return "short?";
                case SqlDataType.Int:
                    return "int?";
                case SqlDataType.BigInt:
                    return "Int64?";
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return "decimal?";
                case SqlDataType.Float:
                    return "double?";
                case SqlDataType.Real:
                    return "float?";
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Date:
                case SqlDataType.Time:
                    return "System.DateTime?";
                case SqlDataType.Char:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.Xml:
                    return "string";
                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Timestamp:
                    return "byte[]";
                case SqlDataType.UniqueIdentifier:
                    return "System.Guid?";

                //case SqlDataType.UserDefinedDataType:
                //    return GetNullableDataType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedType:

                // todo: hierachyid, geography, ...

                default:
                    return "object";
            }
        }


        public static string GetTypeName(this DataType o)
        {

            switch (o.SqlDataType)
            {
                case SqlDataType.Bit:
                    return "bool";
                case SqlDataType.TinyInt:
                    return "byte";
                case SqlDataType.SmallInt:
                    return "short";
                case SqlDataType.Int:
                    return "int";
                case SqlDataType.BigInt:
                    return "Int64";
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return "decimal";

                case SqlDataType.Float:
                    return "double";

                case SqlDataType.Real:
                    return "float";
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Time:
                case SqlDataType.Date:
                    return "DateTime";
                case SqlDataType.Char:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.Xml:
                    return "string";
                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Timestamp:
                    return "byte[]";
                case SqlDataType.UniqueIdentifier:
                    return "Guid";

                case SqlDataType.UserDefinedDataType:
                    throw new Exception("not implement");
                //    return GetDataType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedTableType:
                    throw new Exception("not implement");

                case SqlDataType.UserDefinedType:

                default:
                    return "object";
            }
        }

        #endregion

        #region GetSqlDbType

        /// <summary>
        /// for command parameter's type
        /// </summary>
        public static string GetSqlDbType(this SqlDataType t)
        {
            switch (t)
            {
                case SqlDataType.BigInt:
                    return "System.Data.SqlDbType.BigInt";
                case SqlDataType.Decimal:
                    return "System.Data.SqlDbType.Decimal";
                case SqlDataType.Int:
                    return "System.Data.SqlDbType.Int";
                case SqlDataType.Numeric:
                    return "System.Data.SqlDbType.Decimal";
                case SqlDataType.SmallInt:
                    return "System.Data.SqlDbType.SmallInt";
                case SqlDataType.Money:
                    return "System.Data.SqlDbType.Money";
                case SqlDataType.TinyInt:
                    return "System.Data.SqlDbType.TinyInt";
                case SqlDataType.SmallMoney:
                    return "System.Data.SqlDbType.SmallMoney";
                case SqlDataType.Bit:
                    return "System.Data.SqlDbType.Bit";
                case SqlDataType.Float:
                    return "System.Data.SqlDbType.Float";
                case SqlDataType.Real:
                    return "System.Data.SqlDbType.Real";
                case SqlDataType.DateTime:
                    return "System.Data.SqlDbType.DateTime";
                case SqlDataType.SmallDateTime:
                    return "System.Data.SqlDbType.SmallDateTime";
                case SqlDataType.Char:
                    return "System.Data.SqlDbType.Char";
                case SqlDataType.Text:
                    return "System.Data.SqlDbType.Text";
                case SqlDataType.VarChar:
                case SqlDataType.VarCharMax:
                    return "System.Data.SqlDbType.VarChar";
                case SqlDataType.NChar:
                    return "System.Data.SqlDbType.NChar";
                case SqlDataType.NText:
                    return "System.Data.SqlDbType.NText";
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                    return "System.Data.SqlDbType.NVarChar";
                case SqlDataType.Binary:
                    return "System.Data.SqlDbType.Binary";
                case SqlDataType.Image:
                    return "System.Data.SqlDbType.Image";
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                    return "System.Data.SqlDbType.VarBinary";
                case SqlDataType.UniqueIdentifier:
                    return "System.Data.SqlDbType.UniqueIdentifier";

                case SqlDataType.UserDefinedDataType:
                    throw new Exception("not implement");
                //return GetSqlDbType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedType:
                    return "System.Data.SqlDbType.Udt";

                case SqlDataType.UserDefinedTableType:
                    return "System.Data.SqlDbType.Structured";

                case SqlDataType.DateTime2:
                    return "System.Data.SqlDbType.DateTime2";
                case SqlDataType.DateTimeOffset:
                    return "System.Data.SqlDbType.DateTimeOffset";
                case SqlDataType.Date:
                    return "System.Data.SqlDbType.Date";
                case SqlDataType.Time:
                    return "System.Data.SqlDbType.Time";

                case SqlDataType.Xml:
                    return "System.Data.SqlDbType.Xml";

                case SqlDataType.Timestamp:
                    return "System.Data.SqlDbType.Timestamp";

                default:
                    return "System.Data.SqlDbType.Variant";
            }
        }

        #endregion

        #region GetEscapeName


        /// <summary>
        /// 取转义后的 Name 字串（去空格，过滤类，类成员名中的非法字符，处理和 C# 数据类型同名的字段名：前面加 _）
        /// </summary>
        public static string GetEscapeName(this IName o)
        {
            return o.Name.Escape();
        }
        /// <summary>
        /// 取转义后的 Name 字串（去空格，过滤类，类成员名中的非法字符，处理和 C# 数据类型同名的字段名：前面加 _）
        /// </summary>
        public static string GetEscapeName(this INameSchema o)
        {
            return o.Name.Escape();
        }

        #endregion

        #region Escape

        /// <summary>
        /// 取转义后的名称（类名，属性名，参数名等）字串（去空格，过滤类，类成员名中的非法字符，处理和 C# 数据类型同名的字段名：前面加 _）
        /// </summary>
        public static string Escape(this string s)
        {
            s = s.Trim();
            if (s.CheckIsKeywords()) return "_" + s;
            if (s[0] >= '0' && s[0] <= '9') s = "_" + s;
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

        #region CheckIsKeywords

        /// <summary>
        /// 判断一个字串是否为 C# 的关键字 (支持到 .net 4) (没有去理会上下文系列，如 linq)
        /// http://msdn.microsoft.com/en-us/library/x53a06bb(VS.100).aspx
        /// </summary>
        public static bool CheckIsKeywords(this string s)
        {
            s = s.ToLower();
            return s == "abstract" ||
                s == "event" ||
                s == "new" ||
                s == "struct" ||
                s == "as" ||
                s == "explicit" ||
                s == "null" ||
                s == "switch" ||
                s == "base" ||
                s == "extern" ||
                s == "object" ||
                s == "this" ||

                s == "boolean" ||
                s == "false" ||
                s == "operator" ||
                s == "throw" ||

                s == "break" ||
                s == "finally" ||
                s == "out" ||
                s == "true" ||

                s == "byte" ||
                s == "fixed" ||
                s == "override" ||
                s == "try" ||

                s == "case" ||
                s == "float" ||
                s == "params" ||
                s == "typeof" ||

                s == "catch" ||
                s == "for" ||
                s == "private" ||
                s == "uint" ||

                s == "char" ||
                s == "foreach" ||
                s == "protected" ||
                s == "ulong" ||

                s == "checked" ||
                s == "goto" ||
                s == "public" ||
                s == "unchecked" ||

                s == "class" ||
                s == "if" ||
                s == "readonly" ||
                s == "unsafe" ||

                s == "const" ||
                s == "implicit" ||
                s == "ref" ||
                s == "ushort" ||

                s == "continue" ||
                s == "in" ||
                s == "return" ||
                s == "using" ||

                s == "decimal" ||
                s == "int32" ||
                s == "sbyte" ||
                s == "virtual" ||

                s == "default" ||
                s == "interface" ||
                s == "sealed" ||
                s == "volatile" ||

                s == "delegate" ||
                s == "public" ||
                s == "int16" ||
                s == "void" ||

                s == "do" ||
                s == "is" ||
                s == "sizeof" ||
                s == "while" ||

                s == "double" ||
                s == "lock" ||
                s == "stackalloc" ||


                s == "else" ||
                s == "int64" ||
                s == "static" ||


                s == "enum" ||
                s == "namespace" ||
                s == "string";
        }

        #endregion

        #region FillSpace

        /// <summary>
        /// 在字串后面 数个 空格, 令总长度达到 len. 如果超长则在后面添加 1 个空格
        /// </summary>
        public static string FillSpace(this string s, int len)
        {
            var L = Encoding.ASCII.GetByteCount(s);
            if (L < len) return s + new string(' ', len - L);
            return s + " ";
        }

        #endregion

        #region GetMaxLength

        /// <summary>
        /// 返回字串集合中最长的字串长度 (byte length)
        /// </summary>
        public static int GetMaxLength(this IEnumerable<string> ss)
        {
            if (ss == null) return 0;
            if (ss.Count() > 0) return ss.Max(s => Encoding.ASCII.GetByteCount(s));
            return 0;
        }

        #endregion

        #region GetByteCount

        /// <summary>
        /// 返回字串的 ASCII 字节长
        /// </summary>
        public static int GetByteCount(this string s)
        {
            return Encoding.ASCII.GetByteCount(s);
        }

        #endregion
    }
}
