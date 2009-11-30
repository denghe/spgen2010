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
    }
    public partial class Server : NodeBase
    {
        public Databases Databases { get; set; }
    }
    public partial class Databases : List<Database>
    {
        public Server Parent { get; set; }
    }
    public partial class Database : NodeBase
    {
        public Server Parent { get; set; }
        public Folders Folders { get; set; }
    }
    public partial class Folders : List<FolderBase>
    {
        public Database Parent { get; set; }
    }
    public abstract partial class FolderBase : NodeBase
    {
        public Database Parent { get; set; }
    }
    public partial class Folder_Tables : FolderBase
    {
        public Tables Tables { get; set; }
    }
    public partial class Tables : List<Table>
    {
        public Folder_Tables Parent { get; set; }
    }
    public partial class Folder_Views : FolderBase
    {
        public Views Views { get; set; }
    }
    public partial class Views : List<View>
    {
        public Folder_Views Parent { get; set; }
    }
    public partial class Folder_UserDefinedFunctions : FolderBase
    {
        public UserDefinedFunctions UserDefinedFunctions { get; set; }
    }
    public partial class UserDefinedFunctions : List<UserDefinedFunctionBase>
    {
        public Folder_UserDefinedFunctions Parent { get; set; }
    }
    public partial class Folder_UserDefinedTableTypes : FolderBase
    {
        public UserDefinedTableTypes UserDefinedTableTypes { get; set; }
    }
    public partial class UserDefinedTableTypes : List<UserDefinedTableType>
    {
        public Folder_UserDefinedTableTypes Parent { get; set; }
    }
    public partial class Folder_StoredProcedures : FolderBase
    {
        public StoredProcedures StoredProcedures { get; set; }
    }
    public partial class StoredProcedures : List<StoredProcedure>
    {
        public Folder_StoredProcedures Parent { get; set; }
    }
    public partial class Folder_Schemas : FolderBase
    {
        public Schemas Schemas { get; set; }
    }
    public partial class Schemas : List<Schema>
    {
        public Folder_Schemas Parent { get; set; }
    }
    public partial class Table : NodeBase
    {
        public Folder_Tables Parent { get; set; }
    }
    public partial class View : NodeBase
    {
        public Folder_Views Parent { get; set; }
    }
    public partial class UserDefinedFunctionBase : NodeBase
    {
        public Folder_UserDefinedFunctions Parent { get; set; }
    }
    public partial class UserDefinedFunction_Scale : UserDefinedFunctionBase
    {
    }
    public partial class UserDefinedFunction_Table : UserDefinedFunctionBase
    {
    }
    public partial class UserDefinedTableType : NodeBase
    {
        public Folder_UserDefinedTableTypes Parent { get; set; }
    }
    public partial class StoredProcedure : NodeBase
    {
        public Folder_StoredProcedures Parent { get; set; }
    }
    public partial class Schema : NodeBase
    {
        public Folder_Schemas Parent { get; set; }
    }
}
