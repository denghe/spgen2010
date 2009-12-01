using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

using SPGen2010.Components.Modules;
using SmoUtils = SPGen2010.Components.Utils.MsSql.Utils;
using Oe = SPGen2010.Components.Modules.ObjectExplorer;

// SMO
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;

namespace SPGen2010.Components.Fillers.MsSql
{
    public partial class ObjectExplorerFiller : IObjectExplorerFiller
    {
        public Server Server { get; set; }

        public string GetInstanceName()
        {
            return Server.InstanceName;
        }

        public Oe.Server Fill(Oe.Server oeserver, bool is_fill_db_name_only = true)
        {
            var server = this.Server;
            oeserver.Databases = new Oe.Databases { Parent = oeserver };
            foreach (Database db in server.Databases)
            {
                var oedb = new Oe.Database { Parent = oeserver, Text = db.Name };
                if (!is_fill_db_name_only) this.Fill(oedb);
                oeserver.Databases.Add(oedb);
            }
            return oeserver;
        }

        public Oe.Database Fill(Oe.Database oedb)
        {
            var db = this.Server.Databases[oedb.Text];
            oedb.Folders = new Oe.Folders { Parent = oedb };

            var sf = new Oe.Folder_Schemas { Parent = oedb, Text = "Schemas" };
            sf.Schemas = new Oe.Schemas { Parent = sf };
            var schemas = from Schema o in db.Schemas
                          where o.IsSystemObject == false || o.Name == "dbo"
                          select new Oe.Schema { Parent = sf, Text = o.Name };
            foreach (var schema in schemas) sf.Schemas.Add(schema);
            oedb.Folders.Add(sf);

            var tf = new Oe.Folder_Tables { Parent = oedb, Text = "Tables" };
            tf.Tables = new Oe.Tables { Parent = tf };
            var tables = from Table o in db.Tables
                         where o.IsSystemObject == false
                         select new Oe.Table { Parent = tf, Text = o.Name };
            foreach (var table in tables) tf.Tables.Add(table);
            oedb.Folders.Add(tf);

            var vf = new Oe.Folder_Views { Parent = oedb, Text = "Views" };
            vf.Views = new Oe.Views { Parent = vf };
            var views = from View o in db.Views
                        where o.IsSystemObject == false
                        select new Oe.View { Parent = vf, Text = o.Name };
            foreach (var view in views) vf.Views.Add(view);
            oedb.Folders.Add(vf);

            var ff = new Oe.Folder_UserDefinedFunctions { Parent = oedb, Text = "UserDefinedFunctions" };
            ff.UserDefinedFunctions = new Oe.UserDefinedFunctions { Parent = ff };
            var fs = from UserDefinedFunction o in db.UserDefinedFunctions
                      where o.IsSystemObject == false
                      select o.FunctionType == UserDefinedFunctionType.Table ?
                        (Oe.UserDefinedFunctionBase)new Oe.UserDefinedFunction_Table { Parent = ff, Text = o.Name } :
                        (Oe.UserDefinedFunctionBase)new Oe.UserDefinedFunction_Scale { Parent = ff, Text = o.Name };
            foreach(var f in fs) ff.UserDefinedFunctions.Add(f);
            oedb.Folders.Add(ff);

            var spf = new Oe.Folder_StoredProcedures { Parent = oedb, Text = "StoredProcedures" };
            spf.StoredProcedures = new Oe.StoredProcedures { Parent = spf };
            var sps = from StoredProcedure o in db.StoredProcedures
                      where o.IsSystemObject == false
                      select new Oe.StoredProcedure { Parent = spf, Text = o.Name };
            foreach (var sp in sps) spf.StoredProcedures.Add(sp);
            oedb.Folders.Add(spf);

            var ttf = new Oe.Folder_UserDefinedTableTypes { Parent = oedb, Text = "UserDefinedTableTypes" };
            ttf.UserDefinedTableTypes = new Oe.UserDefinedTableTypes { Parent = ttf };
            var tts = from UserDefinedTableType o in db.UserDefinedTableTypes
                      select new Oe.UserDefinedTableType { Parent = ttf, Text = o.Name };
            foreach (var tt in tts) ttf.UserDefinedTableTypes.Add(tt);
            oedb.Folders.Add(ttf);

            return oedb;
        }

    }
}
