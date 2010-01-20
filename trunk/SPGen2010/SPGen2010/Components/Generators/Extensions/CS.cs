using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SPGen2010.Components.Modules.MySmo;

namespace SPGen2010.Components.Generators.Extensions.CS
{
    /// <summary>
    /// generate extensions for mysmo
    /// </summary>
    public static partial class Extension
    {
        public static string GetEscapeName(this DataType o)
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

                //case SqlDataType.UserDefinedDataType:
                //    return GetDataType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedType:

                default:
                    return "object";
            }
        }

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
    }
}
