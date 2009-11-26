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
            this.Tables = new List<Table>();
        }
        public List<Table> Tables { get; private set; }
    }
    [ContentProperty("Views")]
    public partial class Folder_Views : FolderBase
    {
        public Folder_Views(Database parent)
            : base(parent, "Views")
        {
            this.Views = new List<View>();
        }
        public List<View> Views { get; private set; }
    }
    [ContentProperty("UserDefinedFunctions")]
    public partial class Folder_UserDefinedFunctions : FolderBase
    {
        public Folder_UserDefinedFunctions(Database parent)
            : base(parent, "Functions")
        {
            this.UserDefinedFunctions = new List<UserDefinedFunctionBase>();
        }
        public List<UserDefinedFunctionBase> UserDefinedFunctions { get; private set; }
    }
    [ContentProperty("UserDefinedTableTypes")]
    public partial class Folder_UserDefinedTableTypes : FolderBase
    {
        public Folder_UserDefinedTableTypes(Database parent)
            : base(parent, "TableTypes")
        {
            this.UserDefinedTableTypes = new List<UserDefinedTableType>();
        }
        public List<UserDefinedTableType> UserDefinedTableTypes { get; private set; }
    }
    [ContentProperty("StoredProcedures")]
    public partial class Folder_StoredProcedures : FolderBase
    {
        public Folder_StoredProcedures(Database parent)
            : base(parent, "StoredProcedures")
        {
            this.StoredProcedures = new List<StoredProcedure>();
        }
        public List<StoredProcedure> StoredProcedures { get; private set; }
    }
    [ContentProperty("Schemas")]
    public partial class Folder_Schemas : FolderBase
    {
        public Folder_Schemas(Database parent)
            : base(parent, "Schemas")
        {
            this.Schemas = new List<Schema>();
        }
        public List<Schema> Schemas { get; private set; }
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
        public StoredProcedure(Folder_UserDefinedTableTypes parent, string caption)
            : base(caption, StoredProcedure.DefaultIcon)
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
