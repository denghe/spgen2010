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
    public interface IConfigures
    {
        Dictionary<ConfigureProperties, object> Properties { get; }

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
    /// Configure's proerties
    /// </summary>
    public enum ConfigureProperties
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

}
