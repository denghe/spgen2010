using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

using SPGen2010.Components.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SPGen2010.Components.Modules.ObjectExplorer
{
    public abstract partial class NodeBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string pn)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pn));
        }
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyPropertyChange("Text");
            }
        }
        private string _tips;
        public string Tips
        {
            get { return _tips; }
            set
            {
                _tips = value;
                NotifyPropertyChange("Tips");
            }
        }
        public object Tag { get; set; }
    }
    public partial class Server : NodeBase
    {
        public Server() { this.Databases = new Databases { Parent = this }; }
        public Databases Databases { get; private set; }
    }
    public partial class Databases : ObservableCollection<Database>
    {
        public Server Parent { get; set; }
        public IEnumerable<T> AddRange<T>(IEnumerable<T> items) where T : Database
        {
            foreach (var item in items) this.Add(item);
            return items;
        }
    }
    public partial class Database : NodeBase
    {
        public Database() { this.Folders = new Folders { Parent = this }; }
        public Server Parent { get; set; }
        private Folders _folders;
        public Folders Folders
        {
            get { return _folders; }
            private set
            {
                _folders = value;
                NotifyPropertyChange("Folders");
            }
        }
    }
    public partial class Folders : ObservableCollection<FolderBase>
    {
        public Database Parent { get; set; }
        public Folders AddRange<T>(IEnumerable<T> items) where T : FolderBase
        {
            foreach (var item in items) this.Add(item);
            return this;
        }
    }
    public abstract partial class FolderBase : NodeBase
    {
        public Database Parent { get; set; }
    }
    public partial class Folder_Tables : FolderBase
    {
        public Folder_Tables() { this.Tables = new Tables { Parent = this }; }
        public Tables Tables { get; private set; }
    }
    public partial class Tables : ObservableCollection<Table>
    {
        public Folder_Tables Parent { get; set; }
        public Tables AddRange<T>(IEnumerable<T> items) where T : Table
        {
            foreach (var item in items) this.Add(item);
            return this;
        }
    }
    public partial class Folder_Views : FolderBase
    {
        public Folder_Views() { this.Views = new Views { Parent = this }; }
        public Views Views { get; private set; }
    }
    public partial class Views : ObservableCollection<View>
    {
        public Folder_Views Parent { get; set; }
        public Views AddRange<T>(IEnumerable<T> items) where T : View
        {
            foreach (var item in items) this.Add(item);
            return this;
        }
    }
    public partial class Folder_UserDefinedFunctions : FolderBase
    {
        public Folder_UserDefinedFunctions() { this.UserDefinedFunctions = new UserDefinedFunctions { Parent = this }; }
        public UserDefinedFunctions UserDefinedFunctions { get; private set; }
    }
    public partial class UserDefinedFunctions : ObservableCollection<UserDefinedFunctionBase>
    {
        public Folder_UserDefinedFunctions Parent { get; set; }
        public UserDefinedFunctions AddRange<T>(IEnumerable<T> items) where T : UserDefinedFunctionBase
        {
            foreach (var item in items) this.Add(item);
            return this;
        }
    }
    public partial class Folder_UserDefinedTableTypes : FolderBase
    {
        public Folder_UserDefinedTableTypes() { this.UserDefinedTableTypes = new UserDefinedTableTypes { Parent = this }; }
        public UserDefinedTableTypes UserDefinedTableTypes { get; private set; }
    }
    public partial class UserDefinedTableTypes : ObservableCollection<UserDefinedTableType>
    {
        public Folder_UserDefinedTableTypes Parent { get; set; }
        public UserDefinedTableTypes AddRange<T>(IEnumerable<T> items) where T : UserDefinedTableType
        {
            foreach (var item in items) this.Add(item);
            return this;
        }
    }
    public partial class Folder_StoredProcedures : FolderBase
    {
        public Folder_StoredProcedures() { this.StoredProcedures = new StoredProcedures { Parent = this }; }
        public StoredProcedures StoredProcedures { get; private set; }
    }
    public partial class StoredProcedures : ObservableCollection<StoredProcedure>
    {
        public Folder_StoredProcedures Parent { get; set; }
        public StoredProcedures AddRange<T>(IEnumerable<T> items) where T : StoredProcedure
        {
            foreach (var item in items) this.Add(item);
            return this;
        }
    }
    public partial class Folder_Schemas : FolderBase
    {
        public Folder_Schemas() { this.Schemas = new Schemas { Parent = this }; }
        public Schemas Schemas { get; private set; }
    }
    public partial class Schemas : ObservableCollection<Schema>
    {
        public Folder_Schemas Parent { get; set; }
        public Schemas AddRange<T>(IEnumerable<T> items) where T : Schema
        {
            foreach (var item in items) this.Add(item);
            return this;
        }
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
