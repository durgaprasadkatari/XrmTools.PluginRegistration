using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PluginRegistration.Shared.Classes;
using PluginRegistration.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using XrmToolBox.Extensibility;

namespace PluginRegistrationUsingXml
{
    public partial class PluginRegistrationControl : PluginControlBase
    {
        private Settings mySettings;
        public string fileName;

        public delegate void FunctionDelegate(EntityCollection pluginAssemblies, RetrievePluginTypes retrievePluginTypes);

        public PluginRegistrationControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }


        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                if (mySettings.LastUsedOrganizationWebappUrl != null)
                {
                    ExecuteMethod(LoadPlugins);
                }

                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        private void LoadPlugins()
        {
            RetrievePluginTypes retrievePluginTypes = new RetrievePluginTypes();
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading the plugins",
                Work = (worker, args) =>
                {
                    SetLoading(true);
                    EntityCollection pluginAssemblies = retrievePluginTypes.GetPluginAssemblies(Service);
                    args.Result = pluginAssemblies;


                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var result = args.Result as EntityCollection;
                        PopulateData(result, retrievePluginTypes);
                        if (result.Entities.Count == 0)
                        {
                            MessageBox.Show("No plugins available to show");
                        }
                        SetLoading(false);
                    }
                }
            });
        }

        private void PopulateData(EntityCollection pluginAssemblies, RetrievePluginTypes retrievePluginTypes)
        {
            treeView1.Nodes.Clear();
            foreach (Entity assembly in pluginAssemblies.Entities)
            {
                TreeNode treeNode = new TreeNode(assembly["name"].ToString());
                treeView1.Nodes.Add(treeNode);
                EntityCollection pluginTypesColl = retrievePluginTypes.GetPluginTypes(assembly.Id, Service);
                foreach (Entity pluginTypeEntity in pluginTypesColl.Entities)
                {
                    TreeNode pluginTypeNode = new TreeNode(pluginTypeEntity["name"].ToString());
                    treeNode.Nodes.Add(pluginTypeNode);
                    EntityCollection pluginStepsColl = retrievePluginTypes.GetSdkProcessingStep(pluginTypeEntity.Id, Service);
                    foreach (Entity pluginStep in pluginStepsColl.Entities)
                    {
                        TreeNode pluginstepNode = new TreeNode(pluginStep["name"].ToString());
                        pluginTypeNode.Nodes.Add(pluginstepNode);
                        EntityCollection stepImageColl = retrievePluginTypes.GetSdkProcessingStepImage(pluginStep.Id, Service);
                        foreach (Entity stepImage in stepImageColl.Entities)
                        {
                            string imageAttributes = "(All)";
                            if (stepImage.Attributes.Contains("attributes"))
                            {
                                imageAttributes = "(" + stepImage["attributes"] + ")";
                            }
                            TreeNode pluginstepImageNode = new TreeNode(stepImage["name"].ToString() + " " + imageAttributes);
                            pluginstepNode.Nodes.Add(pluginstepImageNode);
                        }
                    }
                }
            }
        }

        private void SetLoading(bool displayLoader)
        {
            if (displayLoader)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                });
            }
        }

        private void btnGenerateFile_Click(object sender, EventArgs e)
        {
            ExecuteMethod(GenerateRegistrationFile);
        }

        private void GenerateRegistrationFile()
        {
            XmlDocument xmlDoc = new XmlDocument();
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Generating the registration xml file",
                Work = (worker, args) =>
                {
                    args.Result = "";
                    RetrievePluginTypes retrievePluginTypes = new RetrievePluginTypes();
                    EntityCollection pluginAssemblies = retrievePluginTypes.GetPluginAssemblies(Service);
                    XmlNode rootNode = xmlDoc.CreateElement("Register");
                    xmlDoc.AppendChild(rootNode);
                    XmlNode solutions = xmlDoc.CreateElement("Solutions");
                    rootNode.AppendChild(solutions);
                    foreach (Entity pluginAssembly in pluginAssemblies.Entities)
                    {
                        XmlNode solution = xmlDoc.CreateElement("Solution");
                        solutions.AppendChild(solution);
                        XmlAttribute assembly = xmlDoc.CreateAttribute("Assembly");
                        assembly.Value = pluginAssembly["name"].ToString() + ".dll";
                        solution.Attributes.Append(assembly);
                        XmlAttribute Id = xmlDoc.CreateAttribute("Id");
                        Id.Value = pluginAssembly.Id.ToString();
                        solution.Attributes.Append(Id);
                        XmlAttribute IsolationMode = xmlDoc.CreateAttribute("IsolationMode");
                        IsolationMode.Value = ((OptionSetValue)pluginAssembly["isolationmode"]).Value.ToString();
                        solution.Attributes.Append(IsolationMode);
                        XmlAttribute SourceType = xmlDoc.CreateAttribute("SourceType");
                        SourceType.Value = ((OptionSetValue)pluginAssembly["sourcetype"]).Value.ToString();
                        solution.Attributes.Append(SourceType);

                        //Plugin-Types
                        EntityCollection pluginTypesorWorkflowColl = retrievePluginTypes.GetPluginTypes(pluginAssembly.Id, Service);
                        List<Entity> pluginTypesColl = pluginTypesorWorkflowColl.Entities.Where(x => Convert.ToBoolean(x.Attributes["isworkflowactivity"]) == false).ToList();
                        List<Entity> workflowTypesColl = pluginTypesorWorkflowColl.Entities.Where(x => Convert.ToBoolean(x.Attributes["isworkflowactivity"]) == true).ToList();
                        GenerateRegistrationFile generateRegistrationFile = new GenerateRegistrationFile();
                        generateRegistrationFile.GeneratePluginOrWorkflowTypes(false, pluginTypesColl, ref solution, ref xmlDoc, Service);
                        generateRegistrationFile.GeneratePluginOrWorkflowTypes(true, workflowTypesColl, ref solution, ref xmlDoc, Service);
                        args.Result = "success";
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        using (var fbd = new FolderBrowserDialog())
                        {
                            DialogResult result = fbd.ShowDialog();
                            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                            {
                                xmlDoc.Save(fbd.SelectedPath + "\\RegisterFile.xml");
                                MessageBox.Show("File generated successfully!");
                            }
                        }
                    }

                }
            });




        }

        private void btnbrowseRegistrationFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog
            {
                Filter = "Xml Files(.xml) | *.xml",
                Title = "Browse for file"
            };

            if (browseFile.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            try
            {
                txtRegistrationFile.Text = browseFile.FileName;
                fileName = txtRegistrationFile.Text;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnBrowsePluginsDLL_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog
            {
                Filter = "dll Files(.dll) | *.dll",
                Title = "Browse for file"
            };

            if (browseFile.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            try
            {
                txtBrowsePluginsdll.Text = browseFile.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnRegisterPlugins_Click(object sender, EventArgs e)
        {
            ExecuteMethod(RegisterPlugins);
        }

        private void RegisterPlugins()
        {
            if (txtRegistrationFile.Text == "Browse Registration File")
            {
                MessageBox.Show("Please browse registration xml file.","Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtBrowsePluginsdll.Text == "Browser Plugin Assembly DLL")
            {
                MessageBox.Show("Please browse plugins dll file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RegisterPlugins registerPlugins = new RegisterPlugins();
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Registering plugins/workflows",
                Work = (worker, args) =>
                {
                    registerPlugins.RegisterPluginsFromXml(txtRegistrationFile.Text, txtBrowsePluginsdll.Text, Service);
                    args.Result = "success";
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Plugins or workflows are registered successfully!!");
                        ExecuteMethod(LoadPlugins);
                    }
                }
            });
        }


        private void tsbLoadPlugins_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadPlugins);
        }

        private void tsbGenerateXmlFile_Click(object sender, EventArgs e)
        {
            ExecuteMethod(GenerateRegistrationFile);
        }
    }
}