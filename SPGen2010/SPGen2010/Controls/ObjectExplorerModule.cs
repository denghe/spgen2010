using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace SPGen2010.ObjectExplorer
{
    [ContentProperty("Databases")]
    public partial class Server
    {
        public IEnumerable<Database> Databases { get; private set; }
    }
    [ContentProperty("Folders")]
    public partial class Database
    {
        public IEnumerable<Folder> Folders { get; private set; }
    }
    [ContentProperty("Entities")]
    public partial class Folder
    {
        public IEnumerable<Entity> Entities { get; private set; }
    }
    public partial class Entity
    {
        
    }
}
