using Microsoft.Xrm.Sdk;
using PluginRegistration.Shared.Classes;
using PluginRegistration.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginRegistration.TaskRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var exePath = Directory.GetCurrentDirectory();
                string projectRoot = new FileInfo(exePath).DirectoryName;
                IOrganizationService service = ConnectToCrm.GetCrmOrgnizationService(args[0]);
                RegisterPlugins registerPlugins = new RegisterPlugins();
                var registrationxmlPath = Path.Combine(projectRoot, "PluginRegistration.xml");
                var projectDll = Path.GetFileName(Path.GetDirectoryName(exePath)) + ".dll";
                var projectbinfolder = Path.Combine(projectRoot, "bin", "debug", projectDll);
                registerPlugins.RegisterPluginsFromXml(registrationxmlPath, projectbinfolder, service);
            }
            else
            {
                Console.WriteLine("Please provide the connection string");
                Console.ReadLine();
            }
        }
    }
}
