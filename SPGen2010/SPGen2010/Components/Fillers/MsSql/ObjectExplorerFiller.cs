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
    public static partial class ObjectExplorerFiller
    {
        public static Oe.Server Fill(this Oe.Server oeserver, Server server)
        {
            oeserver.Databases = new Oe.Databases { Parent = oeserver };
            foreach (Database db in server.Databases)
            {
                oeserver.Databases.Add(new Oe.Database { Parent = oeserver, Text = db.Name }.Fill(db));
            }
            return oeserver;
        }

        public static Oe.Server FillDatabaseNameOnly(this Oe.Server oeserver, Server server)
        {
            oeserver.Databases = new Oe.Databases { Parent = oeserver };
            foreach (Database db in server.Databases)
            {
                oeserver.Databases.Add(new Oe.Database { Parent = oeserver, Text = db.Name });
            }
            return oeserver;
        }

        public static Oe.Database Fill(this Oe.Database oedb, Database db)
        {
            oedb.Folders = new Oe.Folders { Parent = oedb };

            var sf = new Oe.Folder_Schemas { Parent = oedb, Text = "Schemas" };
            sf.Schemas = new Oe.Schemas { Parent = sf };
            sf.Schemas.AddRange(
                from Schema o in db.Schemas
                where o.IsSystemObject == false || o.Name == "dbo"
                select new Oe.Schema { Parent = sf, Text = o.Name }
            );
            oedb.Folders.Add(sf);

            var tf = new Oe.Folder_Tables { Parent = oedb, Text = "Tables" };
            tf.Tables = new Oe.Tables { Parent = tf };
            tf.Tables.AddRange(
                from Table o in db.Tables
                where o.IsSystemObject == false
                select new Oe.Table { Parent = tf, Text = o.Name }
            );
            oedb.Folders.Add(tf);

            var vf = new Oe.Folder_Views { Parent = oedb, Text = "Views" };
            vf.Views = new Oe.Views { Parent = vf };
            vf.Views.AddRange(
                from View o in db.Views
                where o.IsSystemObject == false
                select new Oe.View { Parent = vf, Text = o.Name }
            );
            oedb.Folders.Add(vf);

            var ff = new Oe.Folder_UserDefinedFunctions { Parent = oedb, Text = "UserDefinedFunctions" };
            ff.UserDefinedFunctions = new Oe.UserDefinedFunctions { Parent = ff };
            ff.UserDefinedFunctions.AddRange(
                from UserDefinedFunction o in db.UserDefinedFunctions
                where o.IsSystemObject == false
                select o.FunctionType == UserDefinedFunctionType.Table ?
                    (Oe.UserDefinedFunctionBase)new Oe.UserDefinedFunction_Table { Parent = ff, Text = o.Name } :
                    (Oe.UserDefinedFunctionBase)new Oe.UserDefinedFunction_Scale { Parent = ff, Text = o.Name }
            );
            oedb.Folders.Add(ff);

            var spf = new Oe.Folder_StoredProcedures { Parent = oedb, Text = "StoredProcedures" };
            spf.StoredProcedures = new Oe.StoredProcedures { Parent = spf };
            spf.StoredProcedures.AddRange(
                from StoredProcedure o in db.StoredProcedures
                where o.IsSystemObject == false
                select new Oe.StoredProcedure { Parent = spf, Text = o.Name }
            );
            oedb.Folders.Add(spf);

            var ttf = new Oe.Folder_UserDefinedTableTypes { Parent = oedb, Text = "UserDefinedTableTypes" };
            ttf.UserDefinedTableTypes = new Oe.UserDefinedTableTypes { Parent = ttf };
            ttf.UserDefinedTableTypes.AddRange(
                from UserDefinedTableType o in db.UserDefinedTableTypes
                select new Oe.UserDefinedTableType { Parent = ttf, Text = o.Name }
            );
            oedb.Folders.Add(ttf);

            return oedb;
        }

    }
}
