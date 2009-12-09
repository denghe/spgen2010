using System;
using System.ComponentModel;
using SPGen2010.Components.Modules.MySmo;
namespace SPGen2010.Components.Fillers
{
    public interface IMySmoFiller
    {
        Server Fill(Server server);
        Database Fill(Database db);
        Schema Fill(Schema s);
        Table Fill(Table t);
        View Fill(View v);
        UserDefinedFunction Fill(UserDefinedFunction f);
        UserDefinedTableType Fill(UserDefinedTableType tt);
        StoredProcedure Fill(StoredProcedure sp);
    }
}
