using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGen2010.Components.Modules;

namespace SPGen2010.Components.Fillers
{
    public static class ConnLogFiller
    {
        public static bool Fill(this DS.ConnLogDataTable cl)
        {
            // todo: 从配置文件读取数据填充到 cl
            return true;
        }
    }
}
