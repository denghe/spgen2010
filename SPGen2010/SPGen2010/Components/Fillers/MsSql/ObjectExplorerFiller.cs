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
            foreach (Database db in server.Databases)
            {
                oeserver.Databases.Add(new Oe.Database(oeserver, db.Name, "").Fill(db));
            }

            return oeserver;
        }

        public static Oe.Database Fill(this Oe.Database oedb, Database db)
        {
            var sf = oedb.Folders.Add(new Oe.Folder_Schemas(oedb));
            sf.Schemas.AddRange(
                from Schema o in db.Schemas
                where o.IsSystemObject == false
                select new Oe.Schema(sf, o.Name)
            );

            var tf = oedb.Folders.Add(new Oe.Folder_Tables(oedb));
            tf.Tables.AddRange(
                from Table o in db.Tables
                where o.IsSystemObject == false
                select new Oe.Table(tf, o.Schema + "." + o.Name)
            );

            var vf = oedb.Folders.Add(new Oe.Folder_Views(oedb));
            vf.Views.AddRange(
                from Table o in db.Tables
                where o.IsSystemObject == false
                select new Oe.View(vf, o.Schema + "." + o.Name)
            );

            var ff = oedb.Folders.Add(new Oe.Folder_UserDefinedFunctions(oedb));
            ff.UserDefinedFunctions.AddRange(
                from UserDefinedFunction o in db.UserDefinedFunctions
                where o.IsSystemObject == false
                select o.FunctionType == UserDefinedFunctionType.Table ?
                    (Oe.UserDefinedFunctionBase)new Oe.UserDefinedFunction_Table(ff, o.Schema + "." + o.Name) :
                    (Oe.UserDefinedFunctionBase)new Oe.UserDefinedFunction_Scale(ff, o.Schema + "." + o.Name)
            );

            var spf = oedb.Folders.Add(new Oe.Folder_StoredProcedures(oedb));
            spf.StoredProcedures.AddRange(
                from Table o in db.Tables
                where o.IsSystemObject == false
                select new Oe.StoredProcedure(spf, o.Schema + "." + o.Name)
            );

            var ttf = oedb.Folders.Add(new Oe.Folder_UserDefinedTableTypes(oedb));
            ttf.UserDefinedTableTypes.AddRange(
                from Table o in db.Tables
                where o.IsSystemObject == false
                select new Oe.UserDefinedTableType(ttf, o.Schema + "." + o.Name)
            );

            return oedb;
        }

    }
}
