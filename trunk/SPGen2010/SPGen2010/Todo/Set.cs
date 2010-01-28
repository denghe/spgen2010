using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SPGen2010.Todo
{
    public partial class DbSet
    {
        public DbSet()
        {
            this.Tables = new List<DbTable>();
        }
        public List<DbTable> Tables { get; private set; }
        public DbTable this[int index] { get { return Tables[index]; } }
        public DbTable this[string name] { get { return Tables.Find(o => o.Name == name); } }
        public DbTable this[string name, string schema] { get { return Tables.Find(o => o.Name == name && o.Schema == schema); } }
    }
    public partial class DbTable
    {
        public DbTable()
        {
            this.Rows = new List<DbRow>();
            this.Columns = new List<DbColumn>();
        }
        public DbSet Set { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public List<DbRow> Rows { get; private set; }
        public List<DbColumn> Columns { get; private set; }
        public DbRow this[int rowIdx] { get { return this.Rows[rowIdx]; } }
        public int GetIndex() { if (Set == null) return 0; return Set.Tables.IndexOf(this); }
        public DbRow NewRow(params object[] data) { return new DbRow(this, data); }
        public DbColumn NewColumn() { return new DbColumn(this); }
        public DbColumn NewColumn(string name, Type type) { return new DbColumn(this, name, type); }
        public DbColumn NewColumn(string name) { return new DbColumn(this, name); }
    }
    public partial class DbColumn
    {
        private DbColumn() { }
        public DbColumn(DbTable parent, string name, Type type)
        {
            this.Table = parent; parent.Columns.Add(this); this.Name = name; this.Type = type;
            if (parent.Rows.Count > 0) foreach (var row in parent.Rows) row.Increase();
        }
        public DbColumn(DbTable parent, string name) : this(parent, name, typeof(string)) { }
        public DbColumn(DbTable parent) : this(parent, null, typeof(string)) { }
        public DbTable Table { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public int GetIndex() { return this.Table.Columns.IndexOf(this); }
    }
    public partial class DbRow
    {
        private DbRow() { }
        public DbRow(DbTable parent, params object[] data)
        {
            if (parent.Columns.Count == 0 && (data == null || data.Length > 0))
                throw new Exception("Beyond the limited number of fields");
            else if (parent.Columns.Count > 0 && (data == null || data.Length != parent.Columns.Count))
                throw new Exception("Insufficient data or Beyond the limited number of fields");
            this.Table = parent;
            this.DataArray = data;
            parent.Rows.Add(this);
        }
        public DbTable Table { get; private set; }
        private object[] _dataArray;
        public object[] DataArray { get { return this._dataArray; } set { this._dataArray = value; } }
        public object this[int idx] { get { return this._dataArray[idx]; } set { this._dataArray[idx] = value; } }
        public object this[DbColumn col] { get { return this._dataArray[col.GetIndex()]; } set { this._dataArray[col.GetIndex()] = value; } }
        public object this[string name]
        {
            get { return this._dataArray[this.Table.Columns.Find(o => o.Name == name).GetIndex()]; }
            set { this._dataArray[this.Table.Columns.Find(o => o.Name == name).GetIndex()] = value; }
        }
        public void SetValues(params object[] data) { this._dataArray = data; }
        internal void Increase()
        {
            if (this._dataArray == null) this._dataArray = new object[] { null };
            else Array.Resize<object>(ref this._dataArray, this._dataArray.Length + 1);
        }
    }




    public class xxx
    {
        public xxx()
        {
            {
                var ds = new DataSet();
                var dt = ds.Tables[0];
                var dr = dt.NewRow();
            }

            {
                var ds = new DbSet();
                var dr = ds[0].NewRow();
            }
        }
    }
}
