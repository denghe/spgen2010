using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPGen2010.Codes.MySmo
{
    #region interfaces

    public interface IMySmoObject
    {
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
        string Schema { get; set; }
    }
    public interface IExtendPropertiesBase
    {
        List<ExtendedProperty> ExtendedProperties { get; set; }
    }

    #endregion



    public class Database : IMySmoObject, INameBase, IExtendPropertiesBase
    {
        public List<ExtendedProperty> ExtendedProperties { get; set; }
        public List<Table> Tables { get; set; }
        public List<View> Views { get; set; }
        public List<StoredProcedure> StoredProcedures { get; set; }
        public List<UserDefinedFunction> Functions { get; set; }
        public List<UserDefinedTableType> UserDefinedTableTypes { get; set; }
        public string Name { get; set; }
    }

    public class Table : IMySmoObject, IParentDatabase, ITableBase, INameBase, ISchemaBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public List<ExtendedProperty> ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
    }

    public class View : IMySmoObject, IParentDatabase, ITableBase, INameBase, ISchemaBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public List<ExtendedProperty> ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
    }

    public class UserDefinedTableType : IMySmoObject, IParentDatabase, ITableBase, INameBase, ISchemaBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public List<ExtendedProperty> ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
    }

    public class UserDefinedFunction : IMySmoObject, IParentDatabase, ITableBase, IParameterBase, INameBase, ISchemaBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public List<ExtendedProperty> ExtendedProperties { get; set; }
        public List<Column> Columns { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
    }

    public class StoredProcedure : IMySmoObject, IParentDatabase, IParameterBase, INameBase, ISchemaBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public List<ExtendedProperty> ExtendedProperties { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
    }

    public class Column : IMySmoObject, IParentDatabase, IParentTableBase, INameBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public ITableBase ParentTableBase { get; set; }
        public List<ExtendedProperty> ExtendedProperties { get; set; }
        public DataType DataType { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }

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

    public class Parameter : IMySmoObject, IParentDatabase, IParentParameterBase, INameBase, IExtendPropertiesBase
    {
        public Database ParentDatabase { get; set; }
        public IParameterBase ParentParameterBase { get; set; }
        public List<ExtendedProperty> ExtendedProperties { get; set; }
        public DataType DataType { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
    }

    public class DataType : IMySmoObject, INameBase
    {
        public string Name { get; set; }
    }

    public class ExtendedProperty : IMySmoObject, INameBase
    {
        public string Name { get; set; }
    }
}
