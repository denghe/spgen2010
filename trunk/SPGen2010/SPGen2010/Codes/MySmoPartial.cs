using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGen2010.Codes.MySmo
{
    public interface IDescription
    {
        string Description { get; set; }
    }

    partial class Database : IDescription
    {
        public string Description { get; set; }
    }

    partial class Table : IDescription
    {
        public string Description { get; set; }
    }

    partial class View : IDescription
    {
        public string Description { get; set; }
    }

    partial class UserDefinedTableType : IDescription
    {
        public string Description { get; set; }
    }

    partial class UserDefinedFunction : IDescription
    {
        public string Description { get; set; }
    }

    partial class StoredProcedure : IDescription
    {
        public string Description { get; set; }
    }

    partial class Column : IDescription
    {
        public string Description { get; set; }
    }

    partial class Parameter : IDescription
    {
        public string Description { get; set; }
    }

    partial class DataType
    {
    }

    partial class ExtendedProperty
    {
    }

}
