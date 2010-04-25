using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using SPGen2010.Components.Modules.MySmo;
using SPGen2010.Components.Providers;
using SPGen2010.Components.Modules.ObjectExplorer;
using SPGen2010.Components.Generators;

namespace SPGen2010.Components.Configures
{
    /// <summary>
    /// Configure's interface
    /// </summary>
    public interface IConfigure
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
        /// execute config program
        /// </summary>
        void Execute(params NodeBase[] targets);
    }

}
