using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SPGen2010.Controls.ObjectExplorerModule
{
    public partial class NodeBase
    {
        public string Caption { get; private set; }
        public BitmapImage Icon { get; private set; }
        public NodeBase(string caption, BitmapImage icon)
        {
            this.Caption = caption; this.Icon = icon;
        }
        public static BitmapImage NewImageSource(string fn)
        {
            return ImageSourceHelper.NewImageSource(fn);
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
        public static BitmapImage DefaultIcon
        {
            get
            {
                return NewImageSource("sql_server.png");
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
            this.Parent = parent;
            this.Folders = new Folders(this);
        }
        public Server Parent { get; set; }
        public static BitmapImage DefaultIcon
        {
            get
            {
                return NewImageSource("sql_database.png");
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


    public partial class FolderBase : NodeBase
    {
        public FolderBase(Database parent, string caption)
            : base(caption, FolderBase.DefaultIcon)
        {
            this.Parent = parent;
        }
        public Database Parent { get; set; }
        public static BitmapImage DefaultIcon
        {
            get
            {
                return NewImageSource("sql_folder.png");
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
        }
        public Folder_Tables Parent { get; set; }

        public static BitmapImage DefaultIcon
        {
            get
            {
                return NewImageSource("sql_table.png");
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
        }
        public Folder_Views Parent { get; set; }

        public static BitmapImage DefaultIcon
        {
            get
            {
                return NewImageSource("sql_view.png");
            }
        }
    }

    public partial class UserDefinedFunctionBase : NodeBase
    {
        public UserDefinedFunctionBase(Folder_UserDefinedFunctions parent, string caption, BitmapImage icon)
            : base(caption, icon)
        {
            this.Parent = parent;
        }
        public Folder_UserDefinedFunctions Parent { get; set; }
    }
    //[ContentProperty("Parameters")]
    public partial class UserDefinedFunction_Scale : UserDefinedFunctionBase
    {
        public UserDefinedFunction_Scale(Folder_UserDefinedFunctions parent, string caption)
            : base(parent, caption, UserDefinedFunction_Scale.DefaultIcon)
        {
            this.Parent = parent;
        }
        public static BitmapImage DefaultIcon
        {
            get
            {
                return NewImageSource("sql_function_scale.png");
            }
        }
    }
    //[ContentProperty("Folders")]
    public partial class UserDefinedFunction_Table : UserDefinedFunctionBase
    {
        public UserDefinedFunction_Table(Folder_UserDefinedFunctions parent, string caption)
            : base(parent, caption, UserDefinedFunction_Table.DefaultIcon)
        {
            this.Parent = parent;
        }
        public static BitmapImage DefaultIcon
        {
            get
            {
                return NewImageSource("sql_function_table.png");
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
        }
        public Folder_UserDefinedTableTypes Parent { get; set; }

        public static BitmapImage DefaultIcon
        {
            get
            {
                return NewImageSource("sql_tabletype.png");
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
        }
        public Folder_StoredProcedures Parent { get; set; }

        public static BitmapImage DefaultIcon
        {
            get
            {
                return NewImageSource("sql_tabletype.png");
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
        }
        public Folder_Schemas Parent { get; set; }

        public static BitmapImage DefaultIcon
        {
            get
            {
                return NewImageSource("sql_schema.png");
            }
        }
    }
}
