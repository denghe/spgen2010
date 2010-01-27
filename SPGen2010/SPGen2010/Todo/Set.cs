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

    }
    public partial class DbColumn
    {
        private DbColumn();
        public DbColumn(DbTable parent, string name, Type type) { this.Table = parent; parent.Columns.Add(this); this.Name = name; this.Type = type; }
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
        public DbRow(DbTable parent, object[] data) { this.Table = parent; this.DataArray = data; parent.Rows.Add(this); }
        public DbTable Table { get; private set; }
        public object[] DataArray { get; set; }
        public object this[int idx] { get { return DataArray[idx]; } set { DataArray[idx] = value; } }
        public object this[DbColumn col] { get { return DataArray[col.GetIndex()]; } set { DataArray[col.GetIndex()] = value; } }
        public object this[string name]
        {
            get { return DataArray[Table.Columns.Find(o => o.Name == name).GetIndex()]; }
            set { DataArray[Table.Columns.Find(o => o.Name == name).GetIndex()] = value; }
        }
        public void SetValues(params object[] data) { DataArray = data; }
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
