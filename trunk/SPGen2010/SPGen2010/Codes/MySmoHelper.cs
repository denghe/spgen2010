using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// SMO
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;

using My = SPGen2010.Codes.MySmo;
using SmoUtils = SPGen2010.Codes.Helpers.Utils;

namespace SPGen2010.Codes
{
    public static class MySmoHelper
    {
        public static void FillData(this My.Database mydb, Database db)
        {
            mydb.Tables.AddRange(
                from Table t in db.Tables
                where t.IsSystemObject == false
                select NewTable(mydb, t)
            );

            mydb.Views.AddRange(
                from View v in db.Views
                where v.IsSystemObject == false
                select NewView(mydb, v)
            );

            // ... 
        }

        public static My.Table NewTable(My.Database mydb, Table t)
        {
            var myt = new My.Table();
            myt.ParentDatabase = mydb;
            myt.Columns = new List<My.Column>();
            myt.Columns.AddRange(
                from Column c in t.Columns
                select NewColumn(mydb, myt, c)
            );
            return myt;
        }

        public static My.View NewView(My.Database mydb, View v)
        {
            var myv = new My.View();
            myv.ParentDatabase = mydb;
            myv.Columns = new List<My.Column>();
            myv.Columns.AddRange(
                from Column c in v.Columns
                select NewColumn(mydb, myv, c)
            );
            return myv;
        }

        public static My.Column NewColumn(My.Database mydb, My.ITableBase myt, Column c)
        {
            var myc = new My.Column();
            myc.ParentDatabase = mydb;
            myc.ParentTableBase = myt;
            myc.DataType = NewDataType(c.DataType);

            // result.xxxx = c.xxxx;
            // result.xxxx = c.xxxx;
            // result.xxxx = c.xxxx;

            return myc;
        }

        public static My.DataType NewDataType(DataType dt)
        {
            var mydt = new My.DataType();

            // mydt.xxx = dt.xxx;
            // mydt.xxx = dt.xxx;
            // mydt.xxx = dt.xxx;

            return mydt;
        }
    }

}
