using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Windows;
using SPGen2010.Components.Generators;

namespace SPGen2010.Components.Configures
{
    public static partial class ConfigureLoader
    {
        /// <summary>
        /// load & init configures
		/// </summary>
        public static void InitComponents(ref List<IConfigure> gens)
		{
            // load generators from current assembly
			InitComponents(Assembly.GetExecutingAssembly(), ref gens);

			// load Components\*.cs
			string[] files = null;
			try
			{
				files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Components"), "*.cs", SearchOption.AllDirectories);
			}
			catch { }
			if (files == null || files.Length == 0) return;

			var options = new CompilerParameters();

			var path_this = Assembly.GetExecutingAssembly().Location;
			var path_smo = typeof(Microsoft.SqlServer.Management.Smo.Server).Assembly.Location;
			var path_smo_sfc = typeof(Microsoft.SqlServer.Management.Sdk.Sfc.DataProvider).Assembly.Location;
			var path_smo_connectinfo = typeof(Microsoft.SqlServer.Management.Common.ServerConnection).Assembly.Location;
			var path_smo_sqlenum = typeof(Microsoft.SqlServer.Management.Smo.UserDefinedFunctionType).Assembly.Location;

            //var path_40 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Reference Assemblies\Microsoft\Framework\v4.0");
            //var path_40_core = Path.Combine(path_40, "System.Core.dll");
            //var path_40_dsext = Path.Combine(path_40, "System.Data.DataSetExtensions.dll");
            //var path_40_xmllinq = Path.Combine(path_40, "System.Xml.Linq.dll");

			options.ReferencedAssemblies.AddRange(new string[]{
				"System.dll"
				, "System.Data.dll"
                , "System.Data.DataSetExtensions"
                , "System.Xaml.dll"
				, "System.Xml.dll"
                , "System.Xml.Linq.dll"
                , "Microsoft.CSharp.dll"
                , "Microsoft.VisualBasic.dll"
                , "PresentationCore.dll"
                , "PresentationFramework.dll"
                , "WindowsBase.dll"
                , path_this
                , path_smo
                , path_smo_sfc
                , path_smo_connectinfo
                , path_smo_sqlenum
			});

			options.CompilerOptions = "/target:library";
			options.GenerateInMemory = true;

			var provider = new CSharpCodeProvider();

			// alert errors
			try
			{
				var result = provider.CompileAssemblyFromFile(options, files);
				if (result.Errors != null && result.Errors.Count > 0)
				{
                    //using (FOutputText f = new FOutputText())
                    //{
                    //    f.Text = "Load components (*.cs) ccurred some error:";
                    //    f.Width = 780;
                    //    f.Height = 550;
                    //    foreach (CompilerError ce in result.Errors)
                    //    {
                    //        f.WriteLine("FileName    = {0}", ce.FileName);
                    //        f.WriteLine("Line        = {0}", ce.Line);
                    //        f.WriteLine("ErrorNumber = {0}", ce.ErrorNumber);
                    //        f.WriteLine("ErrorText   = {0}", ce.ErrorText);
                    //        f.WriteLine("Column      = {0}", ce.Column);
                    //        f.WriteLine("IsWarning   = {0}", ce.IsWarning);
                    //        f.WriteLine(2);
                    //    }
                    //    f.ShowDialog();
                    //    App.Exit();
                    //}
				}
				InitComponents(result.CompiledAssembly, ref gens);

                gens.Sort(new Comparison<IConfigure>((a, b) => { return string.Compare(string.Concat(a.Properties[GenProperties.Group], a.Properties[GenProperties.Caption]), string.Concat(b.Properties[GenProperties.Group], b.Properties[GenProperties.Caption])); }));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

        /// <summary>
        /// load generators from assembly
        /// </summary>
        public static void InitComponents(Assembly a, ref List<IConfigure> gens)
		{
            string interfacename = typeof(IConfigure).FullName;
			Type[] types = a.GetTypes();
			foreach (Type t in types)
			{
				List<Type> interfaces = new List<Type>(t.GetInterfaces());
				if (interfaces.Exists(delegate(Type type) { return type.FullName == interfacename; }))
				{
                    IConfigure igc = (IConfigure)a.CreateInstance(t.FullName);
                    if (igc.Properties.ContainsKey(GenProperties.IsEnabled) && (bool)igc.Properties[GenProperties.IsEnabled] == false) continue;
					gens.Add(igc);
				}
			}
		}


    }
}
