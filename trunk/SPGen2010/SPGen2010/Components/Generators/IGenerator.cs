using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using SPGen2010.Components.Modules.MySmo;
using SPGen2010.Components.Providers;
using SPGen2010.Components.Modules.ObjectExplorer;

namespace SPGen2010.Components.Generators
{
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
        public GenResult(GenResultTypes rt) { _resultType = rt; }

        protected GenResultTypes _resultType;
        public GenResultTypes GenResultType { get { return _resultType; } }

        protected string _message;
        /// <summary>
        /// get or set output message
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        protected KeyValuePair<string, string> _codeSegment;

        /// <summary>
        /// get or set (title, code)
        /// </summary>
        public KeyValuePair<string, string> CodeSegment
        {
            get { return _codeSegment; }
            set { _codeSegment = value; }
        }
        protected List<KeyValuePair<string, string>> _codeSegments;
        /// <summary>
        /// get or set (title, code)[]
        /// </summary>
        public List<KeyValuePair<string, string>> CodeSegments
        {
            get { return _codeSegments; }
            set { _codeSegments = value; }
        }
        protected KeyValuePair<string, byte[]> _file;
        /// <summary>
        /// get or set (filename, data)
        /// </summary>
        public KeyValuePair<string, byte[]> File
        {
            get { return _file; }
            set { _file = value; }
        }
        protected List<KeyValuePair<string, byte[]>> _files;

        /// <summary>
        /// get or set (filename, data)[]
        /// </summary>
        public List<KeyValuePair<string, byte[]>> Files
        {
            get { return _files; }
            set { _files = value; }
        }
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
        /// KeyValuePair＜string name, string content＞[] 
        /// </summary>
        CodeSegments,
        /// <summary>
        /// string filename, byte[] content
        /// </summary>
        File,
        /// <summary>
        /// KeyValuePair＜string filename, byte[] content＞[] 
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

}
