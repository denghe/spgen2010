using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static partial class DbSet_Extensions
{
    public static void Dump(this DbSet ds)
    {
        if (ds.Tables.Count > 0)
        {
            Console.Write("\r\nData Dump:");
            foreach (var dt in ds.Tables) Dump(dt);
        }
        if (ds.Messages.Count > 0)
        {
            Console.Write("\r\nPrint Messages:");
            foreach (var m in ds.Messages) Console.Write("\r\n" + m);
        }
        if (ds.Errors.Count > 0)
        {
            Console.Write("\r\nRaise Errors:");
            foreach (var e in ds.Errors) Console.Write("\r\n" + e.ToString());
        }
        Console.Write("\r\nRecords Affected:" + ds.RecordsAffected);
        Console.Write("\r\nReturn:" + ds.ReturnValue);
    }
    public static void Dump(this DbTable dt)
    {
        var count = dt.Columns.Count;
        Console.WriteLine("\r\nTable:" + dt.Name.ToNameString());
        Console.Write(dt.Columns[0].Name.ToNameString());
        for (var i = 1; i < count; i++)
            Console.Write("\t" + dt.Columns[i].Name.ToNameString());
        foreach (var dr in dt.Rows)
        {
            Console.Write("\r\n" + dr[0].ToValueString());
            for (var i = 1; i < count; i++)
                Console.Write("\t" + dr[i].ToValueString());
        }
    }
    public static string ToNameString(this string s)
    {
        if (string.IsNullOrEmpty(s)) return "[NoName]";
        return s;
    }
    public static string ToValueString(this object o)
    {
        return o == DBNull.Value ? "[Null]" : o.ToString();
    }
}