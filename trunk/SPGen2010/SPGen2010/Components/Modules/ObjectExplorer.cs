using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

using SPGen2010.Components.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SPGen2010.Components.Modules.ObjectExplorer
{
    public abstract partial class NodeBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (this.PropertyChanged != null)
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Text"));
            }
        }
        private string _tips;
        public string Tips
        {
            get { return _tips; }
            set { _tips = value;
            if (this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Tips"));
            }
        }
        public object Tag { get; set; }
    }
    public partial class Server : NodeBase
    {
        public Databases Databases { get; set; }
    }
    public partial class Databases : ObservableCollection<Database>
    {
        public Server Parent { get; set; }
    }
    public partial class Database : NodeBase, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        public Server Parent { get; set; }
        private Folders _folders;
        public Folders Folders
        {
            get { return _folders; }
            set
            {
                _folders = value;
                if (this.PropertyChanged != null)
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Folders"));
            }
        }
    }
    public partial class Folders : ObservableCollection<FolderBase>
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
    public partial class Tables : ObservableCollection<Table>
    {
        public Folder_Tables Parent { get; set; }
    }
    public partial class Folder_Views : FolderBase
    {
        public Views Views { get; set; }
    }
    public partial class Views : ObservableCollection<View>
    {
        public Folder_Views Parent { get; set; }
    }
    public partial class Folder_UserDefinedFunctions : FolderBase
    {
        public UserDefinedFunctions UserDefinedFunctions { get; set; }
    }
    public partial class UserDefinedFunctions : ObservableCollection<UserDefinedFunctionBase>
    {
        public Folder_UserDefinedFunctions Parent { get; set; }
    }
    public partial class Folder_UserDefinedTableTypes : FolderBase
    {
        public UserDefinedTableTypes UserDefinedTableTypes { get; set; }
    }
    public partial class UserDefinedTableTypes : ObservableCollection<UserDefinedTableType>
    {
        public Folder_UserDefinedTableTypes Parent { get; set; }
    }
    public partial class Folder_StoredProcedures : FolderBase
    {
        public StoredProcedures StoredProcedures { get; set; }
    }
    public partial class StoredProcedures : ObservableCollection<StoredProcedure>
    {
        public Folder_StoredProcedures Parent { get; set; }
    }
    public partial class Folder_Schemas : FolderBase
    {
        public Schemas Schemas { get; set; }
    }
    public partial class Schemas : ObservableCollection<Schema>
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
