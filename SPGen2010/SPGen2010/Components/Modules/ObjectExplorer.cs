using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

using SPGen2010.Components.Utils;

namespace SPGen2010.Components.Modules.ObjectExplorer
{
    public abstract partial class NodeBase
    {
        public string Text { get; set; }
        public string Tips { get; set; }
        public NodeBase(string text = "", string tips = "")
        {
            this.Text = text; this.Tips = tips;
        }
    }

    public partial class Server : NodeBase
    {
        public Server(string text = "", string tips = "")
            : base(text, tips)
        {
            this.Databases = new Databases(this);
        }
        public Databases Databases { get; private set; }
    }

    public partial class Databases : List<Database>
    {
        public Databases(Server parent)
        {
            this.Parent = parent;
        }
        public Server Parent { get; set; }
        public T Add<T>(T item) where T : Database
        {
            item.Parent = this.Parent;
            base.Add(item);
            return item;
        }
        public T AddRange<T>(T items) where T : IEnumerable<Database>
        {
            foreach(var item in items)item.Parent = this.Parent;
            base.AddRange(items);
            return items;
        }
    }

    public partial class Database : NodeBase
    {
        public Database(Server parent, string text = "", string tips = "")
            : base(text, tips)
        {
            this.Folders = new Folders(this);
            this.Parent = parent;
            parent.Databases.Add(this);
        }
        public Server Parent { get; set; }
        public Folders Folders { get; private set; }
    }

    public partial class Folders : List<FolderBase>
    {
        public Folders(Database parent)
        {
            this.Parent = parent;
        }
        public Database Parent { get; set; }
        public T Add<T>(T item) where T : FolderBase
        {
            item.Parent = this.Parent;
            base.Add(item);
            return item;
        }
        public T AddRange<T>(T items) where T : IEnumerable<FolderBase>
        {
            foreach (var item in items) item.Parent = this.Parent;
            base.AddRange(items);
            return items;
        }
    }

    public abstract partial class FolderBase : NodeBase
    {
        public FolderBase(Database parent, string text = "", string tips = "")
            : base(text, tips)
        {
            this.Parent = parent;
            parent.Folders.Add(this);
        }
        public Database Parent { get; set; }
    }

    public partial class Folder_Tables : FolderBase
    {
        public Folder_Tables(Database parent, string text = "Tables", string tips = "")
            : base(parent, text, tips)
        {
            this.Tables = new Tables(this);
        }
        public Tables Tables { get; private set; }
    }
    public partial class Tables : List<Table>
    {
        public Tables(Folder_Tables parent)
        {
            this.Parent = parent;
        }
        public Folder_Tables Parent { get; set; }
        public T Add<T>(T item) where T : Table
        {
            item.Parent = this.Parent;
            base.Add(item);
            return item;
        }
        public T AddRange<T>(T items) where T : IEnumerable<Table>
        {
            foreach (var item in items) item.Parent = this.Parent;
            base.AddRange(items);
            return items;
        }
    }
    public partial class Folder_Views : FolderBase
    {
        public Folder_Views(Database parent, string text = "Views", string tips = "")
            : base(parent, text, tips)
        {
            this.Views = new Views(this);
        }
        public Views Views { get; private set; }
    }
    public partial class Views : List<View>
    {
        public Views(Folder_Views parent)
        {
            this.Parent = parent;
        }
        public Folder_Views Parent { get; set; }
        public T Add<T>(T item) where T : View
        {
            item.Parent = this.Parent;
            base.Add(item);
            return item;
        }
        public T AddRange<T>(T items) where T : IEnumerable<View>
        {
            foreach (var item in items) item.Parent = this.Parent;
            base.AddRange(items);
            return items;
        }
    }

    public partial class Folder_UserDefinedFunctions : FolderBase
    {
        public Folder_UserDefinedFunctions(Database parent, string text = "Functions", string tips = "")
            : base(parent, text, tips)
        {
            this.UserDefinedFunctions = new UserDefinedFunctions(this);
        }
        public UserDefinedFunctions UserDefinedFunctions { get; private set; }
    }
    public partial class UserDefinedFunctions : List<UserDefinedFunctionBase>
    {
        public UserDefinedFunctions(Folder_UserDefinedFunctions parent)
        {
            this.Parent = parent;
        }
        public Folder_UserDefinedFunctions Parent { get; set; }
        public T Add<T>(T item) where T : UserDefinedFunctionBase
        {
            item.Parent = this.Parent;
            base.Add(item);
            return item;
        }
        public T AddRange<T>(T items) where T : IEnumerable<UserDefinedFunctionBase>
        {
            foreach (var item in items) item.Parent = this.Parent;
            base.AddRange(items);
            return items;
        }
    }

    public partial class Folder_UserDefinedTableTypes : FolderBase
    {
        public Folder_UserDefinedTableTypes(Database parent, string text = "Table Types", string tips = "")
            : base(parent, text, tips)
        {
            this.UserDefinedTableTypes = new UserDefinedTableTypes(this);
        }
        public UserDefinedTableTypes UserDefinedTableTypes { get; private set; }
    }
    public partial class UserDefinedTableTypes : List<UserDefinedTableType>
    {
        public UserDefinedTableTypes(Folder_UserDefinedTableTypes parent)
        {
            this.Parent = parent;
        }
        public Folder_UserDefinedTableTypes Parent { get; set; }
        public T Add<T>(T item) where T : UserDefinedTableType
        {
            item.Parent = this.Parent;
            base.Add(item);
            return item;
        }
        public T AddRange<T>(T items) where T : IEnumerable<UserDefinedTableType>
        {
            foreach (var item in items) item.Parent = this.Parent;
            base.AddRange(items);
            return items;
        }
    }

    public partial class Folder_StoredProcedures : FolderBase
    {
        public Folder_StoredProcedures(Database parent, string text = "Stored Procedures", string tips = "")
            : base(parent, text, tips)
        {
            this.StoredProcedures = new StoredProcedures(this);
        }
        public StoredProcedures StoredProcedures { get; private set; }
    }
    public partial class StoredProcedures : List<StoredProcedure>
    {
        public StoredProcedures(Folder_StoredProcedures parent)
        {
            this.Parent = parent;
        }
        public Folder_StoredProcedures Parent { get; set; }
        public T Add<T>(T item) where T : StoredProcedure
        {
            item.Parent = this.Parent;
            base.Add(item);
            return item;
        }
        public T AddRange<T>(T items) where T : IEnumerable<StoredProcedure>
        {
            foreach (var item in items) item.Parent = this.Parent;
            base.AddRange(items);
            return items;
        }
    }
    public partial class Folder_Schemas : FolderBase
    {
        public Folder_Schemas(Database parent, string text = "Schemas", string tips = "")
            : base(parent, text, tips)
        {
            this.Schemas = new Schemas(this);
        }
        public Schemas Schemas { get; private set; }
    }
    public partial class Schemas : List<Schema>
    {
        public Schemas(Folder_Schemas parent)
        {
            this.Parent = parent;
        }
        public Folder_Schemas Parent { get; set; }
        public T Add<T>(T item) where T : Schema
        {
            item.Parent = this.Parent;
            base.Add(item);
            return item;
        }
        public T AddRange<T>(T items) where T : IEnumerable<Schema>
        {
            foreach (var item in items) item.Parent = this.Parent;
            base.AddRange(items);
            return items;
        }
    }

    public partial class Table : NodeBase
    {
        public Table(Folder_Tables parent, string text = "", string tips = "")
            : base(text, tips)
        {
            this.Parent = parent;
            parent.Tables.Add(this);
        }
        public Folder_Tables Parent { get; set; }
    }

    public partial class View : NodeBase
    {
        public View(Folder_Views parent, string text = "", string tips = "")
            : base(text, tips)
        {
            this.Parent = parent;
            parent.Views.Add(this);
        }
        public Folder_Views Parent { get; set; }
    }

    public partial class UserDefinedFunctionBase : NodeBase
    {
        public UserDefinedFunctionBase(Folder_UserDefinedFunctions parent, string text = "", string tips = "")
            : base(text, tips)
        {
            this.Parent = parent;
            parent.UserDefinedFunctions.Add(this);
        }
        public Folder_UserDefinedFunctions Parent { get; set; }
    }
    public partial class UserDefinedFunction_Scale : UserDefinedFunctionBase
    {
        public UserDefinedFunction_Scale(Folder_UserDefinedFunctions parent, string text = "")
            : base(parent, text)
        {
        }
    }
    public partial class UserDefinedFunction_Table : UserDefinedFunctionBase
    {
        public UserDefinedFunction_Table(Folder_UserDefinedFunctions parent, string text = "")
            : base(parent, text)
        {
        }
    }

    public partial class UserDefinedTableType : NodeBase
    {
        public UserDefinedTableType(Folder_UserDefinedTableTypes parent, string text = "", string tips = "")
            : base(text, tips)
        {
            this.Parent = parent;
            parent.UserDefinedTableTypes.Add(this);
        }
        public Folder_UserDefinedTableTypes Parent { get; set; }
    }

    public partial class StoredProcedure : NodeBase
    {
        public StoredProcedure(Folder_StoredProcedures parent, string text = "", string tips = "")
            : base(text, tips)
        {
            this.Parent = parent;
            parent.StoredProcedures.Add(this);
        }
        public Folder_StoredProcedures Parent { get; set; }
    }

    public partial class Schema : NodeBase
    {
        public Schema(Folder_Schemas parent, string text = "", string tips = "")
            : base(text, tips)
        {
            this.Parent = parent;
            parent.Schemas.Add(this);
        }
        public Folder_Schemas Parent { get; set; }
    }
}
