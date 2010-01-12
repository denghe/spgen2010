using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPGen2010.Components.Generators;
using System.IO;
using System.Windows;
using System.Diagnostics;
using SPGen2010.Components.Windows;
using Microsoft.VisualC.StlClr;

namespace SPGen2010.Components.Helpers.IO
{
    public static class OutputHelper
    {
        /// <summary>
        /// 输出生成结果
        /// </summary>
        public static void Output(GenResult result)
        {
            if (result == null)
            {
                // do nothing
            }
            else if (result.GenResultType == GenResultTypes.Message)
            {
                if (result.Message == null) return;
                new WOutputText(result.Message).ShowDialog();
            }
            else if (result.GenResultType == GenResultTypes.CodeSegment)
            {
                new WOutputText(result.CodeSegment).ShowDialog();
            }
            else if (result.GenResultType == GenResultTypes.CodeSegments)
            {
                new WOutputTexts(result.CodeSegments).ShowDialog();
            }
            else if (result.GenResultType == GenResultTypes.File)
            {
                CleanOutput();

                Output(result.File.first, result.File.second);

                PopupOutput();
            }
            else if (result.GenResultType == GenResultTypes.Files)
            {
                CleanOutput();

                foreach (GenericPair<string, byte[]> file in result.Files)
                {
                    Output(file.first, file.second);
                }

                PopupOutput();
            }
        }

        public static string OutputPath = Path.Combine(new FileInfo(App.ResourceAssembly.Location).Directory.FullName, "Output");
        
        /// <summary>
        /// 清空输出结果
        /// </summary>
        public static void CleanOutput()
        {
            try
            {
                Directory.Delete(OutputPath, true);
            }
            catch { }
            try
            {
                Directory.CreateDirectory(OutputPath);
            }
            catch { }
        }

        /// <summary>
        /// 输出生成结果到文件
        /// </summary>
        public static void Output(string fn, byte[] fc)
        {
            var fp = Path.Combine(OutputPath, fn);
            using (var fs = new FileStream(fp, FileMode.Create, FileAccess.Write))
            {
                fs.Write(fc, 0, fc.Length);
            }
        }

        /// <summary>
        /// 弹出代码输出目录
        /// </summary>
        public static void PopupOutput()
        {
            Process.Start("Explorer.exe", OutputPath);
        }
    }
}
