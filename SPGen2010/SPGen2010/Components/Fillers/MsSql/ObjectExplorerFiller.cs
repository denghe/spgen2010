﻿using System;
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
    // todo: prefetch smo

    // todo: load data's progress bar (process notify)

    public partial class ObjectExplorerFiller : IObjectExplorerFiller
    {
        public Server Server { get; set; }

        public string GetInstanceName()
        {
            return Server.InstanceName;
        }

        public Oe.Server Fill(Oe.Server oeserver)
        {
            oeserver.Databases.Clear();
            oeserver.Databases.AddRange(
                from Database db in this.Server.Databases
                where db.IsSystemObject == false
                select new Oe.Database { Parent = oeserver, Text = db.Name });
            return oeserver;
        }

        public Oe.Database Fill(Oe.Database oedb, Action<double> progressNotify)
        {
            double progress = 0;
            var db = this.Server.Databases[oedb.Text];  // todo: check exists

            progressNotify(progress++);

            var sf = new Oe.Folder_Schemas { Parent = oedb, Text = "Schemas" };
            sf.Schemas.AddRange(
                from Schema o in db.Schemas
                where o.IsSystemObject == false || o.Name == "dbo"
                select new Oe.Schema { Parent = sf, Text = o.Name });
            oedb.Folders.Add(sf);

            progressNotify(progress++);

            var tf = new Oe.Folder_Tables { Parent = oedb, Text = "Tables" };
            tf.Tables.AddRange(
                from Table o in db.Tables
                where o.IsSystemObject == false
                select new Oe.Table { Parent = tf, Text = o.Name });
            oedb.Folders.Add(tf);

            progressNotify(progress++);

            var vf = new Oe.Folder_Views { Parent = oedb, Text = "Views" };
            vf.Views.AddRange(
                from View o in db.Views
                where o.IsSystemObject == false
                select new Oe.View { Parent = vf, Text = o.Name });
            oedb.Folders.Add(vf);

            progressNotify(progress++);

            var ff = new Oe.Folder_UserDefinedFunctions { Parent = oedb, Text = "UserDefinedFunctions" };
            ff.UserDefinedFunctions.AddRange(
                from UserDefinedFunction o in db.UserDefinedFunctions
                where o.IsSystemObject == false
                select o.FunctionType == UserDefinedFunctionType.Table ?
                    (Oe.UserDefinedFunctionBase)new Oe.UserDefinedFunction_Table { Parent = ff, Text = o.Name } :
                    (Oe.UserDefinedFunctionBase)new Oe.UserDefinedFunction_Scale { Parent = ff, Text = o.Name });
            oedb.Folders.Add(ff);

            progressNotify(progress++);

            var spf = new Oe.Folder_StoredProcedures { Parent = oedb, Text = "StoredProcedures" };
            spf.StoredProcedures.AddRange(
                from StoredProcedure o in db.StoredProcedures
                where o.IsSystemObject == false
                select new Oe.StoredProcedure { Parent = spf, Text = o.Name });
            oedb.Folders.Add(spf);

            progressNotify(progress++);

            var ttf = new Oe.Folder_UserDefinedTableTypes { Parent = oedb, Text = "UserDefinedTableTypes" };
            ttf.UserDefinedTableTypes.AddRange(
                from UserDefinedTableType o in db.UserDefinedTableTypes
                select new Oe.UserDefinedTableType { Parent = ttf, Text = o.Name });
            oedb.Folders.Add(ttf);

            progressNotify(progress++);

            return oedb;
        }

    }
}
