using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace PluginRegistrationUsingXml
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Plugin Registration Using RegXml"),
        ExportMetadata("Description", "This tool will unregister all the plugins and register again using the xml file"),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "AAABAAEAICAQAAAAAADoAgAAFgAAACgAAAAgAAAAQAAAAAEABAAAAAAAgAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAACAAAAAgIAAgAAAAIAAgACAgAAAgICAAMDAwAAAAP8AAP8AAAD//wD/AAAA/wD/AP//AAD///8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAABAQAAAAAAAAAAAAAAAAAAAQARAAAAAAAAAAAAAAAAAAEQAREAAAAAAAAAAAAAAAABEAAREQAAAAAAAAAAAAAAAREAARERAAAAAAAAAAAAAAERAAARERAAAAAAAAAAAAABEQAAAREREAAAAAAAAAAAAREAAAARERAAAAAAAAAAAAEREAAAAREQAAAAAAAAAAABERAAAAAREAAAAAAAAAAAAREQAAAAARAAAAAAAAAAAAEREAABEAAQAAAAAAAAAAABEQABEREAAAAAAAAAAAAAAQABEREREQAAAAAAAAAAAAABEREREREAAAAAAAAAAAABEREREREAAAAAAAAAAAAAAREREREAAAAAAAAAAAAAAAEREREAAAAAAAAAAAAAAAABEREQAAAAAAAAAAAAAAAAAREQAAAAAAAAAAAAAAAAAAEQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD////////////////////////////v////8////9j////cf///3h///88H///Pgf//z8H//8fh///H8f//x/n//8ed///OD///+AP//+AD///AD///wD///8D////D////z////9///////////////////////////////////w=="),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class PluginRegistration : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new PluginRegistrationControl();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public PluginRegistration()
        {
            // If you have external assemblies that you need to load, uncomment the following to 
            // hook into the event that will fire when an Assembly fails to resolve
            // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.MyPlugin 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}