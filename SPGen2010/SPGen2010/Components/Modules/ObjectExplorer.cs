using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using SPGen2010.Components.Controls;

namespace SPGen2010.Components.Modules.ObjectExplorer
{
    public abstract partial class NodeBase
    {
        public string Caption { get; set; }
        public Image Icon { get; set; }
        public NodeBase(string caption, Image icon)
        {
            this.Caption = caption; this.Icon = icon;
        }
        public static Image NewImage(string fn)
        {
            return new Image { Source = ImageSourceHelper.NewImageSource(fn) };
        }
    }

    [ContentProperty("Databases")]
    public partial class Server : NodeBase
    {
        public Server(string caption)
            : base(caption, Server.DefaultIcon)
        {
            this.Databases = new Databases(this);
        }
        public static Image DefaultIcon
        {
            get
            {
                return NewImage("sql_server.png");
            }
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
    }

    [ContentProperty("Folders")]
    public partial class Database : NodeBase
    {
        public Database(Server parent, string caption)
            : base(caption, Database.DefaultIcon)
        {
            this.Folders = new Folders(this);
            this.Parent = parent;
            parent.Databases.Add(this);
        }
        public Server Parent { get; set; }
        public static Image DefaultIcon
        {
            get
            {
                return NewImage("sql_database.png");
            }
        }
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
    }

    public abstract partial class FolderBase : NodeBase
    {
        public FolderBase(Database parent, string caption)
            : base(caption, FolderBase.DefaultIcon)
        {
            this.Parent = parent;
            parent.Folders.Add(this);
        }
        public Database Parent { get; set; }
        public static Image DefaultIcon
        {
            get
            {
                return NewImage("sql_folder.png");
            }
        }
    }

    [ContentProperty("Tables")]
    public partial class Folder_Tables : FolderBase
    {
        public Folder_Tables(Database parent)
            : base(parent, "Tables")
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
    }
    [ContentProperty("Views")]
    public partial class Folder_Views : FolderBase
    {
        public Folder_Views(Database parent)
            : base(parent, "Views")
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
    }

    [ContentProperty("UserDefinedFunctions")]
    public partial class Folder_UserDefinedFunctions : FolderBase
    {
        public Folder_UserDefinedFunctions(Database parent)
            : base(parent, "Functions")
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
    }

    [ContentProperty("UserDefinedTableTypes")]
    public partial class Folder_UserDefinedTableTypes : FolderBase
    {
        public Folder_UserDefinedTableTypes(Database parent)
            : base(parent, "TableTypes")
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
    }

    [ContentProperty("StoredProcedures")]
    public partial class Folder_StoredProcedures : FolderBase
    {
        public Folder_StoredProcedures(Database parent)
            : base(parent, "StoredProcedures")
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
    }
    [ContentProperty("Schemas")]
    public partial class Folder_Schemas : FolderBase
    {
        public Folder_Schemas(Database parent)
            : base(parent, "Schemas")
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
    }

    //[ContentProperty("Columns")]
    public partial class Table : NodeBase
    {
        public Table(Folder_Tables parent, string caption)
            : base(caption, Table.DefaultIcon)
        {
            this.Parent = parent;
            parent.Tables.Add(this);
        }
        public Folder_Tables Parent { get; set; }

        public static Image DefaultIcon
        {
            get
            {
                return NewImage("sql_table.png");
            }
        }
    }

    //[ContentProperty("Columns")]
    public partial class View : NodeBase
    {
        public View(Folder_Views parent, string caption)
            : base(caption, View.DefaultIcon)
        {
            this.Parent = parent;
            parent.Views.Add(this);
        }
        public Folder_Views Parent { get; set; }

        public static Image DefaultIcon
        {
            get
            {
                return NewImage("sql_view.png");
            }
        }
    }

    public partial class UserDefinedFunctionBase : NodeBase
    {
        public UserDefinedFunctionBase(Folder_UserDefinedFunctions parent, string caption, Image icon)
            : base(caption, icon)
        {
            this.Parent = parent;
            parent.UserDefinedFunctions.Add(this);
        }
        public Folder_UserDefinedFunctions Parent { get; set; }
    }
    //[ContentProperty("Parameters")]
    public partial class UserDefinedFunction_Scale : UserDefinedFunctionBase
    {
        public UserDefinedFunction_Scale(Folder_UserDefinedFunctions parent, string caption)
            : base(parent, caption, UserDefinedFunction_Scale.DefaultIcon)
        {
        }
        public static Image DefaultIcon
        {
            get
            {
                return NewImage("sql_function_scale.png");
            }
        }
    }
    //[ContentProperty("Folders")]
    public partial class UserDefinedFunction_Table : UserDefinedFunctionBase
    {
        public UserDefinedFunction_Table(Folder_UserDefinedFunctions parent, string caption)
            : base(parent, caption, UserDefinedFunction_Table.DefaultIcon)
        {
        }
        public static Image DefaultIcon
        {
            get
            {
                return NewImage("sql_function_table.png");
            }
        }
    }

    //[ContentProperty("Columns")]
    public partial class UserDefinedTableType : NodeBase
    {
        public UserDefinedTableType(Folder_UserDefinedTableTypes parent, string caption)
            : base(caption, UserDefinedTableType.DefaultIcon)
        {
            this.Parent = parent;
            parent.UserDefinedTableTypes.Add(this);
        }
        public Folder_UserDefinedTableTypes Parent { get; set; }

        public static Image DefaultIcon
        {
            get
            {
                return NewImage("sql_tabletype.png");
            }
        }
    }

    //[ContentProperty("Columns")]
    public partial class StoredProcedure : NodeBase
    {
        public StoredProcedure(Folder_StoredProcedures parent, string caption)
            : base(caption, StoredProcedure.DefaultIcon)
        {
            this.Parent = parent;
            parent.StoredProcedures.Add(this);
        }
        public Folder_StoredProcedures Parent { get; set; }

        public static Image DefaultIcon
        {
            get
            {
                return NewImage("sql_tabletype.png");
            }
        }
    }

    //[ContentProperty("Folders")]
    public partial class Schema : NodeBase
    {
        public Schema(Folder_Schemas parent, string caption)
            : base(caption, Schema.DefaultIcon)
        {
            this.Parent = parent;
            parent.Schemas.Add(this);
        }
        public Folder_Schemas Parent { get; set; }

        public static Image DefaultIcon
        {
            get
            {
                return NewImage("sql_schema.png");
            }
        }
    }
}
