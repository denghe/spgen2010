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
    public interface IName
    {
        string Name { get; set; }
    }
    public interface INameSchema
    {
        string Name { get; set; }
        string Schema { get; set; }
    }
    public interface IDescription
    {
        string Description { get; set; }
    }
    public interface IExtendPropertiesBase
    {
        ExtendedProperties ExtendedProperties { get; set; }
    }
    public interface IOwner
    {
        string Owner { get; set; }
    }
    public interface ICreateTime
    {
        DateTime CreateTime { get; set; }
    }

    #endregion

    public partial class Server : IMySmoObject, IName
    {
        public List<Database> Databases { get; set; }
        public string Name { get; set; }
    }

    public partial class Database : IMySmoObject, IParentServer, IName, IExtendPropertiesBase, IOwner, ICreateTime
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
        public string Owner { get; set; }
        public DateTime CreateTime { get; set; }
        public CompatibilityLevel CompatibilityLevel { get; set; }
    }

    public partial class Table : IMySmoObject, IParentDatabase, ITableBase, INameSchema, IExtendPropertiesBase, IDescription, IOwner, ICreateTime
    {
        public Database ParentDatabase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public List<ForeignKey> ForeignKeys { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public partial class View : IMySmoObject, IParentDatabase, ITableBase, INameSchema, IExtendPropertiesBase, IDescription, IOwner, ICreateTime
    {
        public Database ParentDatabase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public partial class UserDefinedTableType : IMySmoObject, IParentDatabase, ITableBase, INameSchema, IExtendPropertiesBase, IDescription, IOwner, ICreateTime
    {
        public Database ParentDatabase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public partial class UserDefinedFunction : IMySmoObject, IParentDatabase, ITableBase, IParameterBase, INameSchema, IExtendPropertiesBase, IDescription, IOwner, ICreateTime
    {
        public Database ParentDatabase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public UserDefinedFunctionType FunctionType { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public partial class StoredProcedure : IMySmoObject, IParentDatabase, IParameterBase, INameSchema, IExtendPropertiesBase, IDescription, IOwner, ICreateTime
    {
        public Database ParentDatabase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public partial class Column : IMySmoObject, IParentDatabase, IParentTableBase, IName, IExtendPropertiesBase, IDescription
    {
        public Database ParentDatabase { get; set; }
        public ITableBase ParentTableBase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public DataType DataType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

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

    public partial class ForeignKey : IMySmoObject, IParentDatabase, IName, IExtendPropertiesBase, ICreateTime
    {
        public Database ParentDatabase { get; set; }
        public List<ForeignKeyColumn> Columns { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public bool IsChecked { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSystemNamed { get; set; }
        public string Name { get; set; }
        public bool NotForReplication { get; set; }
        public Table ParentTable { get; set; }
        public string ReferencedKey { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedTableSchema { get; set; }
        public string ScriptReferencedTable { get; set; }
        public string ScriptReferencedTableSchema { get; set; }

        public ForeignKeyAction UpdateAction { get; set; }
        public ForeignKeyAction DeleteAction { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public partial class ForeignKeyColumn : IMySmoObject, IParentDatabase, IName
    {
        public Database ParentDatabase { get; set; }
        public string Name { get; set; }
        public ForeignKey ParentForeignKey { get; set; }
        public string ReferencedColumn { get; set; }
    }

    public partial class Parameter : IMySmoObject, IParentDatabase, IParentParameterBase, IName, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public IParameterBase ParentParameterBase { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
        public DataType DataType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public bool IsReadOnly { get; set; }
        /// <summary>
        /// for stored procedure only
        /// </summary>
        public bool IsOutputParameter { get; set; }
    }

    public partial class DataType : IMySmoObject, IName
    {
        public SqlDataType SqlDataType { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// for user defined table type
        /// </summary>
        public string Schema { get; set; }
        public int MaximumLength { get; set; }
        public int NumericPrecision { get; set; }
        public int NumericScale { get; set; }
    }

    public partial class Schema : IMySmoObject, IName, IParentDatabase, IOwner, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public ExtendedProperties ExtendedProperties { get; set; }
    }

    public partial class ExtendedProperties : Dictionary<string, string>, IMySmoObject
    {
        public IExtendPropertiesBase ParentExtendPropertiesBase { get; set; }
    }

    public enum ForeignKeyAction
    {
        NoAction = 0,
        Cascade = 1,
        SetNull = 2,
        SetDefault = 3,
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

    public enum CompatibilityLevel {
        Version60 = 60,
        Version65 = 65,
        Version70 = 70,
        Version80 = 80,
        Version90 = 90,
        Version100 = 100,
    }
}
