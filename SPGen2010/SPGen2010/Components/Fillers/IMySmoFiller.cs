using System;
using System.Collections.Generic;
using System.ComponentModel;
using SPGen2010.Components.Modules.MySmo;
namespace SPGen2010.Components.Fillers
{
    public interface IMySmoFiller
    {
        bool Fill<T>(ref List<T> items) where T : IMySmoObject;

        bool Fill<T>(ref T item, string name) where T : INameBase;
        bool Fill<T>(ref T item, string name, string schema) where T : INameSchemaBase;

        bool Fill<T>(ref ExtendedProperties extendproerties, T item) where T : IExtendPropertiesBase;
    }
}
