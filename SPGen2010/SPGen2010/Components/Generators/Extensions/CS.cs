using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SPGen2010.Components.Modules.MySmo;
using System.IO;

namespace SPGen2010.Components.Generators.Extensions.CS {
    /// <summary>
    /// generate extensions for mysmo
    /// </summary>
    public static partial class Extensions {
        #region ToSummary

        /// <summary>
        /// 返回为 代码段的 summary 部分而格式化的备注输出格式。每一行的前面带三个斜杠
        /// todo: 内容转义
        /// </summary>
        public static string ToSummary(this string o) {
            return ToSummary(o, string.Empty, 2);
        }

        /// <summary>
        /// 返回为 代码段的 summary 部分而格式化的备注输出格式。每一行的前面带三个斜杠
        /// todo: 内容转义
        /// </summary>
        public static string ToSummary(this string o, int numTabs) {
            return ToSummary(o, string.Empty, numTabs);
        }

        /// <summary>
        /// 返回为 代码段的 summary 部分而格式化的备注输出格式。每一行的前面带三个斜杠。内容最后面附加一些字串，前面可空 numTabs 个 Tab 符
        /// todo: 内容转义
        /// </summary>
        public static string ToSummary(this string o, string attach, int numTabs) {
            var str = o + attach;
            var tabs = new string('\t', numTabs);
            if(string.IsNullOrEmpty(str)) {
                return @"
" + tabs + @"/// <summary>
" + tabs + @"/// 
" + tabs + @"/// </summary>";
            }
            var sb = new StringBuilder();
            sb.Append(@"
" + tabs + @"/// <summary>");
            using(var tr = new StringReader(str)) {
                while(true) {
                    var s = tr.ReadLine();
                    if(s == null) break;
                    if(s.Contains("--")) {
                        if(s.StartsWith("-- ============================")) continue;
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
        public static string GetNullableTypeName(this DataType o) {
            switch(o.SqlDataType) {
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


        public static string GetTypeName(this DataType o) {

            switch(o.SqlDataType) {
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


        public static string GetExpressionTypeName(this DataType o) {

            switch(o.SqlDataType) {
                case SqlDataType.Bit:
                    return "Boolean";
                case SqlDataType.TinyInt:
                    return "Byte";
                case SqlDataType.SmallInt:
                    return "Int16";
                case SqlDataType.Int:
                    return "Int32";
                case SqlDataType.BigInt:
                    return "Int64";
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return "Decimal";

                case SqlDataType.Float:
                    return "Double";

                case SqlDataType.Real:
                    return "Float";
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
                    return "String";
                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Timestamp:
                    return "Bytes";
                case SqlDataType.UniqueIdentifier:
                    return "Guid";

                case SqlDataType.UserDefinedDataType:
                    throw new Exception("not implement");
                //    return GetDataType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedTableType:
                    throw new Exception("not implement");

                case SqlDataType.UserDefinedType:

                default:
                    return "Object";
            }
        }

        #endregion

        #region GetSqlDbType

        /// <summary>
        /// for command parameter's type
        /// </summary>
        public static string GetSqlDbType(this SqlDataType t, bool isSimpleMode = false) {
            string result = "";
            switch(t) {
                case SqlDataType.BigInt:
                    result = "SqlDbType.BigInt";
                    break;
                case SqlDataType.Decimal:
                    result = "SqlDbType.Decimal";
                    break;
                case SqlDataType.Int:
                    result = "SqlDbType.Int";
                    break;
                case SqlDataType.Numeric:
                    result = "SqlDbType.Decimal";
                    break;
                case SqlDataType.SmallInt:
                    result = "SqlDbType.SmallInt";
                    break;
                case SqlDataType.Money:
                    result = "SqlDbType.Money";
                    break;
                case SqlDataType.TinyInt:
                    result = "SqlDbType.TinyInt";
                    break;
                case SqlDataType.SmallMoney:
                    result = "SqlDbType.SmallMoney";
                    break;
                case SqlDataType.Bit:
                    result = "SqlDbType.Bit";
                    break;
                case SqlDataType.Float:
                    result = "SqlDbType.Float";
                    break;
                case SqlDataType.Real:
                    result = "SqlDbType.Real";
                    break;
                case SqlDataType.DateTime:
                    result = "SqlDbType.DateTime";
                    break;
                case SqlDataType.SmallDateTime:
                    result = "SqlDbType.SmallDateTime";
                    break;
                case SqlDataType.Char:
                    result = "SqlDbType.Char";
                    break;
                case SqlDataType.Text:
                    result = "SqlDbType.Text";
                    break;
                case SqlDataType.VarChar:
                case SqlDataType.VarCharMax:
                    result = "SqlDbType.VarChar";
                    break;
                case SqlDataType.NChar:
                    result = "SqlDbType.NChar";
                    break;
                case SqlDataType.NText:
                    result = "SqlDbType.NText";
                    break;
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                    result = "SqlDbType.NVarChar";
                    break;
                case SqlDataType.Binary:
                    result = "SqlDbType.Binary";
                    break;
                case SqlDataType.Image:
                    result = "SqlDbType.Image";
                    break;
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                    result = "SqlDbType.VarBinary";
                    break;
                case SqlDataType.UniqueIdentifier:
                    result = "SqlDbType.UniqueIdentifier";
                    break;

                case SqlDataType.UserDefinedDataType:
                    throw new Exception("not implement");
                //result = GetSqlDbType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedType:
                    result = "SqlDbType.Udt";
                    break;

                case SqlDataType.UserDefinedTableType:
                    result = "SqlDbType.Structured";
                    break;

                case SqlDataType.DateTime2:
                    result = "SqlDbType.DateTime2";
                    break;
                case SqlDataType.DateTimeOffset:
                    result = "SqlDbType.DateTimeOffset";
                    break;
                case SqlDataType.Date:
                    result = "SqlDbType.Date";
                    break;
                case SqlDataType.Time:
                    result = "SqlDbType.Time";
                    break;

                case SqlDataType.Xml:
                    result = "SqlDbType.Xml";
                    break;

                case SqlDataType.Timestamp:
                    result = "SqlDbType.Timestamp";
                    break;

                default:
                    result = "SqlDbType.Variant";
                    break;
            }
            return isSimpleMode ? result : ("System.Data." + result);
        }

        #endregion

        #region GetEscapeName


        /// <summary>
        /// 取转义后的 Name 字串（去空格，过滤类，类成员名中的非法字符，处理和 C# 数据类型同名的字段名：前面加 _）
        /// </summary>
        public static string GetEscapeName(this IName o) {
            return o.Name.Escape();
        }
        /// <summary>
        /// 取转义后的 Name 字串（去空格，过滤类，类成员名中的非法字符，处理和 C# 数据类型同名的字段名：前面加 _）
        /// </summary>
        public static string GetEscapeName(this INameSchema o) {
            return o.Name.Escape();
        }

        #endregion

        #region Escape

        /// <summary>
        /// 取转义后的名称（类名，属性名，参数名等）字串（去空格，过滤类，类成员名中的非法字符，处理和 C# 数据类型同名的字段名：前面加 _）
        /// </summary>
        public static string Escape(this string s) {
            s = s.Trim();
            if(s.CheckIsKeywords()) return "_" + s;
            if(s[0] >= '0' && s[0] <= '9') s = "_" + s;
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

        #region CheckIs

        #region CheckIsKeywords

        /// <summary>
        /// 判断一个字串是否为 C# 的关键字 (支持到 .net 4) (没有去理会上下文系列，如 linq)
        /// http://msdn.microsoft.com/en-us/library/x53a06bb(VS.100).aspx
        /// </summary>
        public static bool CheckIsKeywords(this string s) {
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

        #region CheckIsBinaryType

        /// <summary>
        /// 判断一个数据类型是否为 “Binary 类”
        /// </summary>
        public static bool CheckIsBinaryType(this DataType dt) {
            SqlDataType sdt = dt.SqlDataType;
            if(sdt == SqlDataType.UserDefinedDataType)
                throw new Exception("not Implementation");
            return (sdt == SqlDataType.Image
                    || sdt == SqlDataType.Binary
                    || sdt == SqlDataType.Timestamp
                    || sdt == SqlDataType.VarBinaryMax
                    || sdt == SqlDataType.VarBinary);
        }

        #endregion

        #region CheckIsValueType

        /// <summary>
        /// 判断并返回字段数据类型是否为“值类型”
        /// </summary>
        public static bool CheckIsValueType(this DataType dt) {
            switch(dt.SqlDataType) {
                case SqlDataType.UserDefinedDataType:
                    throw new Exception("not Implementation");
                case SqlDataType.BigInt:
                case SqlDataType.Decimal:
                case SqlDataType.Int:
                case SqlDataType.Numeric:
                case SqlDataType.SmallInt:
                case SqlDataType.Money:
                case SqlDataType.TinyInt:
                case SqlDataType.SmallMoney:
                case SqlDataType.Bit:
                case SqlDataType.Float:
                case SqlDataType.Real:
                case SqlDataType.Date:
                case SqlDataType.Time:
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.UniqueIdentifier:
                    return true;

                default:
                    return false;
            }
        }

        #endregion

        #region CheckIsStringType

        /// <summary>
        /// 判断一个数据类型是否为 “字串类”
        /// </summary>
        public static bool CheckIsStringType(this DataType dt) {
            SqlDataType sdt = dt.SqlDataType;
            if(sdt == SqlDataType.UserDefinedDataType)
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

        #region CheckIsDateTimeType

        /// <summary>
        /// 判断一个数据类型是否为 “日期类”
        /// </summary>
        public static bool CheckIsDateTimeType(Database db, DataType dt) {
            SqlDataType sdt = dt.SqlDataType;
            if(sdt == SqlDataType.UserDefinedDataType)
                throw new Exception("not Implementation");
            return (sdt == SqlDataType.DateTimeOffset
                    || sdt == SqlDataType.DateTime2
                    || sdt == SqlDataType.DateTime
                    || sdt == SqlDataType.Date
                    || sdt == SqlDataType.Time);
        }

        #endregion

        #endregion

        #region CheckIsGuidType

        /// <summary>
        /// 判断一个数据类型是否为 “Guid 类”
        /// </summary>
        public static bool CheckIsGuidType(this DataType dt) {
            SqlDataType sdt = dt.SqlDataType;
            if(sdt == SqlDataType.UserDefinedDataType)
                throw new Exception("not Implementation");
            return (sdt == SqlDataType.UniqueIdentifier);
        }

        #endregion

        #region CheckIsBooleanType

        /// <summary>
        /// 判断一个数据类型是否为 “Boolean 类”
        /// </summary>
        public static bool CheckIsBooleanType(this DataType dt) {
            SqlDataType sdt = dt.SqlDataType;
            if(sdt == SqlDataType.UserDefinedDataType)
                throw new Exception("not Implementation");
            return (sdt == SqlDataType.Bit);
        }

        #endregion

        #region CheckIsNumericType

        /// <summary>
        /// 判断一个数据类型是否为 “数字类”
        /// </summary>
        public static bool CheckIsNumericType(this DataType dt) {
            SqlDataType sdt = dt.SqlDataType;
            if(sdt == SqlDataType.UserDefinedDataType)
                throw new Exception("not Implementation");
            return (sdt == SqlDataType.BigInt
                || sdt == SqlDataType.Decimal
                || sdt == SqlDataType.Float
                || sdt == SqlDataType.Int
                || sdt == SqlDataType.Money
                || sdt == SqlDataType.Numeric
                || sdt == SqlDataType.Real
                || sdt == SqlDataType.SmallInt
                || sdt == SqlDataType.SmallMoney
                || sdt == SqlDataType.TinyInt);
        }

        #endregion

        #region CheckIsTree

        /// <summary>
        /// 查一个表是否为 树表（符合外键指向自己的条件）
        /// </summary>
        public static bool CheckIsTree(this Table t) {
            if(t == null) return false;
            var pks = t.GetPrimaryKeyColumns();
            if(pks == null || pks.Count == 0)		//没有主键？
            {
                return false;
            }

            if(t.ForeignKeys.Count == 0) {
                return false;
            }

            foreach(ForeignKey fk in t.ForeignKeys) {
                if(fk.ReferencedTable != t.Name || fk.ReferencedTableSchema != t.Schema) continue;
                int equaled = 0;
                foreach(ForeignKeyColumn fkc in fk.Columns)		//判断是否一个外键约束所有字段都是在当前表
                {
                    if(fkc.ParentForeignKey.ParentTable == t) equaled++;
                }
                if(equaled == fk.Columns.Count)					//当前表为树表
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region CheckIsForeignKey

        /// <summary>
        /// 检查一个字段是否为所属表的外键字段之一
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckIsForeignKey(this Column c) {
            Table t = (Table)c.ParentTableBase;
            foreach(ForeignKey fk in t.ForeignKeys) {
                foreach(ForeignKeyColumn fkc in fk.Columns) {
                    Column o = t.Columns.Find(a => a.Name == fkc.Name);
                    if(c == o) return true;
                }
            }
            return false;
        }


        #endregion

        #endregion

        #region FillSpace

        /// <summary>
        /// 在字串后面 数个 空格, 令总长度达到 len. 如果超长则在后面添加 1 个空格
        /// </summary>
        public static string FillSpace(this string s, int len) {
            var L = Encoding.ASCII.GetByteCount(s);
            if(L < len) return s + new string(' ', len - L);
            return s + " ";
        }

        #endregion

        #region GetMaxLength

        /// <summary>
        /// 返回字串集合中最长的字串长度 (byte length)
        /// </summary>
        public static int GetMaxLength(this IEnumerable<string> ss) {
            if(ss == null) return 0;
            if(ss.Count() > 0) return ss.Max(s => Encoding.ASCII.GetByteCount(s));
            return 0;
        }

        #endregion

        #region GetByteCount

        /// <summary>
        /// 返回字串的 ASCII 字节长
        /// </summary>
        public static int GetByteCount(this string s) {
            return Encoding.ASCII.GetByteCount(s);
        }

        #endregion
        
        #region GetDataReaderMethod
        /// <summary>
        /// 根据数据类型返回在 DataReader 的字段读取中所用的方法名
        /// </summary>
        public static string GetDataReaderMethod(this DataType dt) {
            switch(dt.SqlDataType) {
                case SqlDataType.Bit:
                    return "GetBoolean";
                case SqlDataType.TinyInt:
                    return "GetByte";
                case SqlDataType.SmallInt:
                    return "GetInt16";
                case SqlDataType.Int:
                    return "GetInt32";
                case SqlDataType.BigInt:
                    return "GetInt64";
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return "GetDecimal";
                case SqlDataType.Float:
                    return "GetDouble";
                case SqlDataType.Real:
                    return "GetFloat";
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Date:
                case SqlDataType.Time:
                    return "GetDateTime";
                case SqlDataType.Char:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.Xml:
                    return "GetString";

                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Timestamp:
                    return "GetSqlBinary";				// GetSqlBinary 方法还须在后加 .Value

                case SqlDataType.UniqueIdentifier:
                    return "GetGuid";

                case SqlDataType.UserDefinedDataType:
                case SqlDataType.UserDefinedType:
                case SqlDataType.HierarchyId:
                case SqlDataType.Geography:
                case SqlDataType.Geometry:
                    throw new Exception("not Implementation");

                default:
                    return "GetValue";

            }
        }

        #endregion

        #region GetDataReaderMethod
        /// <summary>
        /// 根据数据类型返回 byte[] 转为　原值　处理中所用的方法名
        /// </summary>
        public static string GetToTypeMethod(this DataType dt) {
            switch(dt.SqlDataType) {
                case SqlDataType.Bit:
                    return "ToBoolean";
                case SqlDataType.TinyInt:
                    return "ToByte";
                case SqlDataType.SmallInt:
                    return "ToInt16";
                case SqlDataType.Int:
                    return "ToInt32";
                case SqlDataType.BigInt:
                    return "ToInt64";
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return "ToDecimal";
                case SqlDataType.Float:
                    return "ToDouble";
                case SqlDataType.Real:
                    return "ToSingle";
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Date:
                case SqlDataType.Time:
                    return "ToDateTime";
                case SqlDataType.Char:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.Xml:
                    return "ToString";

                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Timestamp:
                    return "ToBytes";

                case SqlDataType.UniqueIdentifier:
                    return "ToGuid";

                case SqlDataType.UserDefinedDataType:
                case SqlDataType.UserDefinedType:
                case SqlDataType.HierarchyId:
                case SqlDataType.Geography:
                case SqlDataType.Geometry:
                    throw new Exception("not Implementation");

                default:
                    return "GetValue";

            }
        }

        #endregion
    }
}
