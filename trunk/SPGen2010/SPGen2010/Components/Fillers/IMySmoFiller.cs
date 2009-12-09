using System;
using System.Collections.Generic;
using System.ComponentModel;
using SPGen2010.Components.Modules.MySmo;
namespace SPGen2010.Components.Fillers
{
    public interface IMySmoFiller
    {
        bool Fill(ref Server server);

        bool Fill(ref List<Database> databases);
        bool Fill(ref List<Schema> schemas);
        bool Fill(ref List<Table> tables);
        bool Fill(ref List<View> views);
        bool Fill(ref List<UserDefinedFunction> fs);
        bool Fill(ref List<UserDefinedTableType> tts);
        bool Fill(ref List<StoredProcedure> sps);

        bool Fill(ref Database database, string name);
        bool Fill(ref Schema schema, string name);
        bool Fill(ref Table table, string name, string schema);
        bool Fill(ref View view, string name, string schema);
        bool Fill(ref UserDefinedFunction f, string name, string schema);
        bool Fill(ref UserDefinedTableType tt, string name, string schema);
        bool Fill(ref StoredProcedure sp, string name, string schema);
    }
}
