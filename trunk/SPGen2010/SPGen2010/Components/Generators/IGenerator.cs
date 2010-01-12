using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using SPGen2010.Components.Modules.MySmo;
using SPGen2010.Components.Providers;
using SPGen2010.Components.Modules.ObjectExplorer;
using Microsoft.VisualC.StlClr;

namespace SPGen2010.Components.Generators
{
    /*
     result sample:
     single text:
     
            var gr = new GenResult(GenResultTypes.CodeSegment);
            gr.CodeSegment.first = this.Properties[GenProperties.Tips].ToString();
            gr.CodeSegment.second = sb.ToString();

     multi texts:
     
            var gr = new GenResult(GenResultTypes.CodeSegments);
            gr.CodeSegments.Add("xxxx", sb1);
            gr.CodeSegments.Add("eeee", sb2);
            return gr;

     files output:

            var gr = new GenResult(GenResultTypes.Files);
			gr.Files.Add("xxxx.aspx", sb1);
            gr.Files.Add("eeee.cs", sb2);

     */



    /// <summary>
    /// Generator's interface
    /// </summary>
    public interface IGenerator
    {
        Dictionary<GenProperties, object> Properties { get; }

        /// <summary>
        /// refresh to target sql element type
        /// </summary>
        SqlElementTypes TargetSqlElementType { get; }

        /// <summary>
        /// check sql element before gen
        /// </summary>
        bool Validate(params NodeBase[] targets);

        /// <summary>
        /// generate
        /// </summary>
        GenResult Generate(params NodeBase[] targets);
    }

    /// <summary>
    /// Generator's proerties
    /// </summary>
    public enum GenProperties
    {
        /// <summary>
        /// string : unique key
        /// </summary>
        Name,
        /// <summary>
        /// string : title text
        /// </summary>
        Caption,
        /// <summary>
        /// string : group category
        /// </summary>
        Group,
        /// <summary>
        /// string : tool tip text
        /// </summary>
        Tips,
        /// <summary>
        /// bool (default: True)
        /// </summary>
        IsEnabled
    }

    /// <summary>
    /// Generator's return type struct.
    /// </summary>
    public class GenResult
    {
        public GenResult(GenResultTypes rt)
        {
            this.GenResultType = rt;
            switch (rt)
            {
                case GenResultTypes.CodeSegment:
                    this.CodeSegment = new GenericPair<string, string>();
                    break;
                case GenResultTypes.CodeSegments:
                    this.CodeSegments = new List<GenericPair<string, string>>();
                    break;
                case GenResultTypes.File:
                    this.File = new GenericPair<string, byte[]>();
                    break;
                case GenResultTypes.Files:
                    this.Files = new List<GenericPair<string, byte[]>>();
                    break;
                case GenResultTypes.Message: break;
                default: break;
            }
        }

        public GenResultTypes GenResultType { get; private set; }

        /// <summary>
        /// get or set output message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// get or set (title, code)
        /// </summary>
        public GenericPair<string, string> CodeSegment { get; private set; }
        /// <summary>
        /// get or set (title, code)[]
        /// </summary>
        public List<GenericPair<string, string>> CodeSegments { get; private set; }
        /// <summary>
        /// get or set (filename, data)
        /// </summary>
        public GenericPair<string, byte[]> File { get; private set; }
        /// <summary>
        /// get or set (filename, data)[]
        /// </summary>
        public List<GenericPair<string, byte[]>> Files { get; private set; }
    }

    /// <summary>
    /// Generator's return result types
    /// </summary>
    public enum GenResultTypes
    {
        /// <summary>
        /// string name, string content
        /// </summary>
        CodeSegment,
        /// <summary>
        /// GenericPair＜string name, string content＞[] 
        /// </summary>
        CodeSegments,
        /// <summary>
        /// string filename, byte[] content
        /// </summary>
        File,
        /// <summary>
        /// GenericPair＜string filename, byte[] content＞[] 
        /// </summary>
        Files,
        /// <summary>
        /// string output messages
        /// </summary>
        Message
    }

    /// <summary>
    /// sql element types
    /// </summary>
    [Flags]
    public enum SqlElementTypes : int
    {
        Database = 1,
        Databases = 2,

        Table = 4,
        Tables = 8,

        View = 16,
        Views = 32,

        StoredProcedure = 64,
        StoredProcedures = 128,

        UserDefinedFunction_Scale = 256,
        UserDefinedFunction_Table = 512,
        UserDefinedFunctions = 1024,

        UserDefinedTableType = 2048,
        UserDefinedTableTypes = 4096,

        Column = 8192,

        ExtendedProperty = 16384,

        Schema = 32768,
        Schemas = 65536,
    }

    public static class GenResultExtensions
    {
        public static GenericPair<string, string> Add(this List<GenericPair<string, string>> texts, string title, object text)
        {
            var item = new GenericPair<string, string>(title, text.ToString());
            texts.Add(item);
            return item;
        }
        public static GenericPair<string, byte[]> Add(this List<GenericPair<string, byte[]>> files, string filename, object content)
        {
            var item = new GenericPair<string, byte[]>{ first = filename};
            if (content is byte[])
            {
                item.second = (byte[])content;
            }
            else item.second = Encoding.UTF8.GetBytes(content.ToString());
            files.Add(item);
            return item;
        }
    }
}
