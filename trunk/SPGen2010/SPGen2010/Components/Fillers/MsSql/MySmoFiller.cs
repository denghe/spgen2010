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
        private Smo.Server _smo_server;
        public MySmoFiller(Smo.Server smo_server)
        {
            _smo_server = smo_server;
        }

        #region IMySmoFiller Members

        public bool Fill<T>(ref List<T> items) where T : MySmo.IMySmoObject
        {
            throw new NotImplementedException();
        }

        public bool Fill<T>(ref T item, string name) where T : MySmo.INameBase
        {
            throw new NotImplementedException();
        }

        public bool Fill<T>(ref T item, string name, string schema) where T : MySmo.INameSchemaBase
        {
            throw new NotImplementedException();
        }

        public bool Fill<T>(ref MySmo.ExtendedProperties extendproerties,T item) where T : MySmo.IExtendPropertiesBase
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
