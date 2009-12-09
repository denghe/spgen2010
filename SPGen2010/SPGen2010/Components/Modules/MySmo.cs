using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGen2010.Components.Modules.MySmo
{
    #region interfaces

    public interface IMySmoObject
    {
    }
    public interface IParentServer
    {
        Server ParentServer { get; set; }
    }
    public interface IParentDatabase
    {
        Database ParentDatabase { get; set; }
    }
    public interface IParentTableBase
    {
        ITableBase ParentTableBase { get; set; }
    }
    public interface IParentParameterBase
    {
        IParameterBase ParentParameterBase { get; set; }
    }
    public interface ITableBase
    {
        List<Column> Columns { get; set; }
    }
    public interface IParameterBase
    {
        List<Parameter> Parameters { get; set; }
    }
    public interface INameBase
    {
        string Name { get; set; }
    }
    public interface ISchemaBase
    {
        string Name { get; set; }
        Schema Schema { get; set; }
    }
    public interface IExtendPropertiesBase
    {
        ExtendedProperties ExtendedProperties { get; set; }
    }

    #endregion

    public partial class Server : IMySmoObject, INameBase
    {
        public List<Database> Databases { get; set; }
        public string Name { get; set; }
    }

    public partial class Database : IMySmoObject, IParentServer, INameBase, IExtendPropertiesBase
    {
        public Server ParentServer { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Table> Tables { get; set; }
        public List<View> Views { get; set; }
        public List<StoredProcedure> StoredProcedures { get; set; }
        public List<UserDefinedFunction> UserDefinedFunctions { get; set; }
        public List<UserDefinedTableType> UserDefinedTableTypes { get; set; }
        public List<Schema> Schemas { get; set; }
        public string Name { get; set; }
    }

    public partial class Table : IMySmoObject, IParentDatabase, ITableBase, ISchemaBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public Schema Schema { get; set; }
    }

    public partial class View : IMySmoObject, IParentDatabase, ITableBase, ISchemaBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public Schema Schema { get; set; }
    }

    public partial class UserDefinedTableType : IMySmoObject, IParentDatabase, ITableBase, ISchemaBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public Schema Schema { get; set; }
    }

    public partial class UserDefinedFunction : IMySmoObject, IParentDatabase, ITableBase, IParameterBase, ISchemaBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string Name { get; set; }
        public Schema Schema { get; set; }
        public UserDefinedFunctionType UserDefinedFunctionType { get; set; }

    }

    public partial class StoredProcedure : IMySmoObject, IParentDatabase, IParameterBase, ISchemaBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string Name { get; set; }
        public Schema Schema { get; set; }
    }

    public partial class Column : IMySmoObject, IParentDatabase, IParentTableBase, INameBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public ITableBase ParentTableBase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public DataType DataType { get; set; }
        public string Name { get; set; }

        public bool Computed { get; set; }
        public string ComputedText { get; set; }
        public string Default { get; set; }
        public bool Identity { get; set; }
        public long IdentityIncrement { get; set; }
        public long IdentitySeed { get; set; }
        public bool InPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool Nullable { get; set; }
        public bool RowGuidCol { get; set; }
    }

    public partial class Parameter : IMySmoObject, IParentDatabase, IParentParameterBase, INameBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public IParameterBase ParentParameterBase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public DataType DataType { get; set; }
        public string Name { get; set; }
    }

    public partial class DataType : IMySmoObject, INameBase
    {
        public SqlDataType SqlDataType { get; set; }
        public string Name { get; set; }
        public int MaximumLength { get; set; }
        public int NumericPrecision { get; set; }
        public int NumericScale { get; set; }
    }

    public partial class Schema : IMySmoObject, INameBase, IParentDatabase
    {
        public Database ParentDatabase { get; set; }
        public string Name { get; set; }
    }

    public partial class ExtendedProperties : Dictionary<string, string>, IMySmoObject
    {
        public IExtendPropertiesBase ParentExtendPropertiesBase { get; set; }
    }

    public enum UserDefinedFunctionType
    {
        Unknown = 0,
        Scalar = 1,
        Table = 2,
        Inline = 3,
    }

    public enum SqlDataType
    {
        None = 0,
        BigInt = 1,
        Binary = 2,
        Bit = 3,
        Char = 4,
        DateTime = 6,
        Decimal = 7,
        Float = 8,
        Image = 9,
        Int = 10,
        Money = 11,
        NChar = 12,
        NText = 13,
        NVarChar = 14,
        NVarCharMax = 15,
        Real = 16,
        SmallDateTime = 17,
        SmallInt = 18,
        SmallMoney = 19,
        Text = 20,
        Timestamp = 21,
        TinyInt = 22,
        UniqueIdentifier = 23,
        UserDefinedDataType = 24,
        UserDefinedType = 25,
        VarBinary = 28,
        VarBinaryMax = 29,
        VarChar = 30,
        VarCharMax = 31,
        Variant = 32,
        Xml = 33,
        SysName = 34,
        Numeric = 35,
        Date = 36,
        Time = 37,
        DateTimeOffset = 38,
        DateTime2 = 39,
        UserDefinedTableType = 40,
        HierarchyId = 41,
        Geometry = 42,
        Geography = 43,
    }
}
