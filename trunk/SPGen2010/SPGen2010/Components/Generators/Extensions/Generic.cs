using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SPGen2010.Components.Modules.MySmo;
using System.IO;

namespace SPGen2010.Components.Generators.Extensions.Generic
{
    /// <summary>
    /// generate extensions for mysmo
    /// </summary>
    public static partial class Extensions {
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

        #region CheckIs

        #region CheckIsTree

        /// <summary>
        /// 查一个表是否为 树表（符合外键指向自己的条件）
        /// </summary>
        public static bool CheckIsTree(this Table t)
        {
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

        #endregion

        #region CheckIsForeignKey

        /// <summary>
        /// 检查一个字段是否为所属表的外键字段之一
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckIsForeignKey(this Column c)
        {
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


        #endregion


        #endregion
    }
}
