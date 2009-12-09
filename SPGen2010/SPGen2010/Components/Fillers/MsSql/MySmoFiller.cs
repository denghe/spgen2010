using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SPGen2010.Components.Modules;
using MySmo = SPGen2010.Components.Modules.MySmo;
using SmoUtils = SPGen2010.Components.Utils.MsSql.Utils;

// SMO
using Microsoft.SqlServer.Management.Common;
using Smo = Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer;

namespace SPGen2010.Components.Fillers.MsSql
{
    public partial class MySmoFiller : IMySmoFiller
    {
        public MySmoFiller(Smo.Server smo_server)
        {

        }


        #region IMySmoFiller Members

        public bool Fill<T>(ref List<T> items) where T : MySmo.IMySmoObject
        {
            throw new NotImplementedException();
        }

        public bool Fill<T>(ref T t, string name) where T : MySmo.INameBase
        {
            throw new NotImplementedException();
        }

        public bool Fill<T>(ref T t, string name, string schema) where T : MySmo.ISchemaBase
        {
            throw new NotImplementedException();
        }

        public bool Fill<T>(ref MySmo.ExtendedProperties extendproerties) where T : MySmo.IExtendPropertiesBase
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
