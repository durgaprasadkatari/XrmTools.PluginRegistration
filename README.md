
[![Build Status](https://durgaprasadkatari.visualstudio.com/XrmTools_PluginRegistrationUsingXml/_apis/build/status/Pack-XrmTools_PluginRegistrationUsingXml-.NET%20Desktop-CI?branchName=main)](https://durgaprasadkatari.visualstudio.com/XrmTools_PluginRegistrationUsingXml/_build/latest?definitionId=13&branchName=main)

![Build Status](https://durgaprasadkatari.vsrm.visualstudio.com/_apis/public/Release/badge/9147abdb-cbed-4c2f-8da6-d1bccca8a9e0/1/1)

# XrmTools.PluginRegistration

1. Generates the plugin registration file
2. Registers the new plugins and updates the existing plugins.
3. Loads the custom plugins in the tree view.

![alt text](https://github.com/durgaprasadkatari/XrmTools.PluginRegistration/blob/main/XrmToolBox.PluginRegistrationUsingXml/Images/ReadMe.png?raw=true)

# Plugin Registration Task Runner
To register plugins from you local visual studion development IDE or from the azure devops build pipeline.
1. Install the this package from the nuget
`Install-Package PluginRegistration.TaskRunner -Version 1.1.2`
2. After installing the above package successfully it will add the below files to your plugins project.

```
📦Project.Xrm.Plugins
 ┣ 📂PluginRegistrationTaskRunner
 ┃ ┗ 📜deploy-plugins.bat
 ┣ 📜RegisterFile.xml
 ```

3. Make sure to update the connection string in the deploy-plugins.bat file.
4. Generate and replace the "RegisterFile.xml" using xrm toolbox "Plugin registration using xml" tool and save the output xml into the plugins project with the same name.
5. If you are registering your plugins for the first time then feel free to update the RegisterFile.xml file with the relevant plugin steps and update the GUIDs to the empty guid(00000000-0000-0000-0000-000000000000)
6. Plugin registration task runner will automatically generate the new guid and update the registration xml file with newly generated id.
7. Now run the deploy-plugins.bat file from the command prompt or run directly from your visual studio. 
Note: To run the bat file directly from the visual studio install this extension to your visual studio https://marketplace.visualstudio.com/items?itemName=MadsKristensen.OpenCommandLine)
8. That's it! Have fun. I will cover how to setup the azure devops pipeline in my upcoming blog post.

