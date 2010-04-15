using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SPGen2010.Components.Modules.MySmo;

namespace SPGen2010.Components.Generators.Extensions.MySmo
{
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
