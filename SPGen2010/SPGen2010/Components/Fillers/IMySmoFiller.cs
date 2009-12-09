using System;
using System.Collections.Generic;
using System.ComponentModel;
using SPGen2010.Components.Modules.MySmo;
namespace SPGen2010.Components.Fillers
{
    public interface IMySmoFiller
    {
        bool Fill<T>(ref List<T> items) where T : IMySmoObject;

        bool Fill<T>(ref T t, string name) where T : INameBase;
        bool Fill<T>(ref T t, string name, string schema) where T : ISchemaBase;

        bool Fill<T>(ref ExtendedProperties extendproerties) where T : IExtendPropertiesBase;
    }
}
