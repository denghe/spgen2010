using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGen2010.Codes.MySmo
{
    public interface IExtendedInformation
    {
        string Description { get; set; }
        string Caption { get; set; }
        string Summary { get; set; }
    }


    partial class Database : IExtendedInformation
    {
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Summary { get; set; }
    }

    partial class Table : IExtendedInformation
    {
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Summary { get; set; }
    }

    partial class View : IExtendedInformation
    {
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Summary { get; set; }
    }

    partial class UserDefinedTableType : IExtendedInformation
    {
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Summary { get; set; }
    }

    partial class UserDefinedFunction : IExtendedInformation
    {
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Summary { get; set; }
    }

    partial class StoredProcedure : IExtendedInformation
    {
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Summary { get; set; }

        // todo: result information
    }

    partial class Column : IExtendedInformation
    {
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Summary { get; set; }
    }

    partial class Parameter : IExtendedInformation
    {
        public string Description { get; set; }
        public string Caption { get; set; }
        public string Summary { get; set; }
    }

    partial class DataType
    {
    }

    partial class ExtendedProperties
    {
    }

}
