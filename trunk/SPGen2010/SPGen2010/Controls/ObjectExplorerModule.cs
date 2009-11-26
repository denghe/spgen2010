using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SPGen2010.ObjectExplorer
{
    public partial class NodeBase
    {
        public string Caption { get; private set; }
        public BitmapImage Icon { get; private set; }
        public NodeBase(string caption, BitmapImage icon)
        {
            Caption = caption; Icon = icon;
        }
    }

    [ContentProperty("Databases")]
    public partial class Server : NodeBase
    {
        public Server(string caption)
            : base(caption, Server.DefaultIcon)
        {
        }
        public static BitmapImage DefaultIcon
        {
            get
            {
                return ImageSourceHelper.NewImageSource("sql_server.png");
            }
        }
        public IEnumerable<Database> Databases { get; private set; }
    }

    [ContentProperty("Folders")]
    public partial class Database : NodeBase
    {
        public Database(Server parent, string caption)
            : base(caption, Database.DefaultIcon)
        {
            Parent = parent;
        }
        public Server Parent { get; private set; }
        public static BitmapImage DefaultIcon
        {
            get
            {
                return ImageSourceHelper.NewImageSource("sql_database.png");
            }
        }
        public IEnumerable<Folder> Folders { get; private set; }
    }

    #region Folder

    public partial class Folder : NodeBase
    {
        public Folder(Database parent, string caption)
            : base(caption, Folder.DefaultIcon)
        {
            Parent = parent;
        }
        public Database Parent { get; private set; }
        public static BitmapImage DefaultIcon
        {
            get
            {
                return ImageSourceHelper.NewImageSource("sql_folder.png");
            }
        }
    }

    [ContentProperty("Tables")]
    public partial class Folder_Tables : Folder
    {
        public Folder_Tables(Database parent)
            : base(parent, "Tables")
        {
        }
        public IEnumerable<Table> Tables { get; private set; }
    }
    [ContentProperty("Views")]
    public partial class Folder_Views : Folder
    {
        public Folder_Views(Database parent)
            : base(parent, "Views")
        {
        }
        public IEnumerable<View> Views { get; private set; }
    }
    [ContentProperty("UserDefinedFunctions")]
    public partial class Folder_UserDefinedFunctions : Folder
    {
        public Folder_UserDefinedFunctions(Database parent)
            : base(parent, "Functions")
        {
        }
        public IEnumerable<UserDefinedFunction> UserDefinedFunctions { get; private set; }
    }
    [ContentProperty("UserDefinedTableTypes")]
    public partial class Folder_UserDefinedTableTypes : Folder
    {
        public Folder_UserDefinedTableTypes(Database parent)
            : base(parent, "TableTypes")
        {
        }
        public IEnumerable<UserDefinedTableType> UserDefinedTableTypes { get; private set; }
    }

    #endregion

    //[ContentProperty("Columns")]
    public partial class Table : NodeBase
    {
        public Table(Folder_Tables parent, string caption)
            : base(caption, Table.DefaultIcon)
        {
            Parent = parent;
        }
        public Folder_Tables Parent = null;

        public static BitmapImage DefaultIcon
        {
            get
            {
                return ImageSourceHelper.NewImageSource("sql_table.png");
            }
        }
    }

    //[ContentProperty("Columns")]
    public partial class View : NodeBase
    {
        public View(Folder_Views parent, string caption)
            : base(caption, View.DefaultIcon)
        {
            Parent = parent;
        }
        public Folder_Views Parent = null;

        public static BitmapImage DefaultIcon
        {
            get
            {
                return ImageSourceHelper.NewImageSource("sql_view.png");
            }
        }
    }

    public partial class UserDefinedFunction : NodeBase
    {
        public UserDefinedFunction(Folder_UserDefinedFunctions parent, string caption, BitmapImage icon)
            : base(caption, icon)
        {
            Parent = parent;
        }
        public Folder_UserDefinedFunctions Parent = null;
    }
    //[ContentProperty("Parameters")]
    public partial class UserDefinedFunction_Scale : UserDefinedFunction
    {
        public UserDefinedFunction_Scale(Folder_UserDefinedFunctions parent, string caption)
            : base(parent, caption, UserDefinedFunction_Scale.DefaultIcon)
        {
            Parent = parent;
        }
        public static BitmapImage DefaultIcon
        {
            get
            {
                return ImageSourceHelper.NewImageSource("sql_function_scale.png");
            }
        }
    }
    //[ContentProperty("Folders")]
    public partial class UserDefinedFunction_Table : UserDefinedFunction
    {
        public UserDefinedFunction_Table(Folder_UserDefinedFunctions parent, string caption)
            : base(parent, caption, UserDefinedFunction_Table.DefaultIcon)
        {
            Parent = parent;
        }
        public static BitmapImage DefaultIcon
        {
            get
            {
                return ImageSourceHelper.NewImageSource("sql_function_table.png");
            }
        }
    }

    //[ContentProperty("Columns")]
    public partial class UserDefinedTableType : NodeBase
    {
        public UserDefinedTableType(Folder_UserDefinedTableTypes parent, string caption)
            : base(caption, View.DefaultIcon)
        {
            Parent = parent;
        }
        public Folder_UserDefinedTableTypes Parent = null;

        public static BitmapImage DefaultIcon
        {
            get
            {
                return ImageSourceHelper.NewImageSource("sql_tabletype.png");
            }
        }
    }
}
