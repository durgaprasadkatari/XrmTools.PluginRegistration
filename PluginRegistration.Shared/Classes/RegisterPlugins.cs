using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PluginRegistration.Shared.Classes
{
    public class RegisterPlugins
    {
        public void RegisterPluginsFromXml(string registrationXmlPath, string pluginsDllFilePath, IOrganizationService service)
        {
            RetrievePluginTypes retrievePluginTypes = new RetrievePluginTypes();
            string strPluginDllName = Path.GetFileName(pluginsDllFilePath);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(registrationXmlPath);
            XmlNodeList xnList = xmlDoc.DocumentElement.SelectNodes("/Register/Solutions/Solution[@Assembly='" + strPluginDllName + "']");

            foreach (XmlElement node in xnList)
            {
                var id = node.GetAttribute("Id");
                UnRegisterPlugins unregisterPlugins = new UnRegisterPlugins();
                Console.WriteLine("Unregistering the plugin assembly: " + strPluginDllName);
                unregisterPlugins.UnRegisterPluginTypes(new Guid(id), service);
                var sourceType = Convert.ToInt32(node.GetAttribute("SourceType").ToString());
                var isolationMode = Convert.ToInt32(node.GetAttribute("IsolationMode").ToString());
                Entity pluginAssembly = new Entity("pluginassembly", new Guid(id));
                pluginAssembly["isolationmode"] = new OptionSetValue(isolationMode);
                pluginAssembly["sourcetype"] = new OptionSetValue(sourceType);
                pluginAssembly["content"] = Convert.ToBase64String(File.ReadAllBytes(pluginsDllFilePath));
                UpsertRequest upsertpluginAssemblyRequest = new UpsertRequest();
                upsertpluginAssemblyRequest.Target = pluginAssembly;
                UpsertResponse upsertPluginAssemblyResponse = (UpsertResponse)service.Execute(upsertpluginAssemblyRequest);
                XmlNodeList workflowTypeList = node.SelectNodes("WorkflowTypes/WorkflowType");
                foreach (XmlElement workflowType in workflowTypeList)
                {
                    Guid workflowTypeId = RegisterWorkflowTypes(new EntityReference(pluginAssembly.LogicalName, upsertPluginAssemblyResponse.Target.Id), workflowType, service);
                    workflowType.Attributes["Id"].Value = workflowTypeId.ToString();
                }

                XmlNodeList pluginTypeList = node.SelectNodes("PluginTypes/Plugin");
                foreach (XmlElement pluginType in pluginTypeList)
                {
                    Console.WriteLine("Registering the plugin type: " + pluginType.GetAttribute("TypeName"));
                    Entity pluginTypeEntity = new Entity("plugintype", new Guid(pluginType.GetAttribute("Id")));
                    pluginTypeEntity["typename"] = pluginType.GetAttribute("TypeName");
                    pluginTypeEntity["friendlyname"] = pluginType.GetAttribute("friendlyname");
                    pluginTypeEntity["name"] = pluginType.GetAttribute("Name");
                    pluginTypeEntity["pluginassemblyid"] = new EntityReference(pluginAssembly.LogicalName, pluginAssembly.Id);
                    UpsertRequest upsertPluginTypeRequest = new UpsertRequest();
                    upsertPluginTypeRequest.Target = pluginTypeEntity;
                    UpsertResponse upsertPluginTypeResponse = (UpsertResponse)service.Execute(upsertPluginTypeRequest);
                    if (upsertPluginTypeResponse.RecordCreated)
                    {
                        pluginType.Attributes["Id"].Value = upsertPluginTypeResponse.Target.Id.ToString();
                    }

                    XmlNodeList sdksteps = pluginType.SelectNodes("Steps/Step");
                    foreach (XmlElement sdkmessageStepNode in sdksteps)
                    {
                        Console.WriteLine("Registering the sdk step: " + sdkmessageStepNode.GetAttribute("Name"));
                        Entity sdkmessageProcessingStep = new Entity("sdkmessageprocessingstep", new Guid(sdkmessageStepNode.GetAttribute("Id")));
                        sdkmessageProcessingStep["name"] = sdkmessageStepNode.GetAttribute("Name");
                        sdkmessageProcessingStep["description"] = sdkmessageStepNode.GetAttribute("Description");
                        Guid messageId = retrievePluginTypes.GetSdkMessageId(sdkmessageStepNode.GetAttribute("MessageName"), service);
                        sdkmessageProcessingStep["sdkmessageid"] = new EntityReference("sdkmessage", messageId);
                        sdkmessageProcessingStep["plugintypeid"] = new EntityReference("plugintype", upsertPluginTypeResponse.Target.Id);
                        sdkmessageProcessingStep["mode"] = new OptionSetValue(Convert.ToInt32(sdkmessageStepNode.GetAttribute("Mode"))); //0=sync,1=async
                        sdkmessageProcessingStep["rank"] = Convert.ToInt32(sdkmessageStepNode.GetAttribute("Rank"));
                        sdkmessageProcessingStep["stage"] = new OptionSetValue(Convert.ToInt32(sdkmessageStepNode.GetAttribute("Stage"))); //10-preValidation, 20-preOperation, 40-PostOperation
                        sdkmessageProcessingStep["supporteddeployment"] = new OptionSetValue(Convert.ToInt32(sdkmessageStepNode.GetAttribute("SupportedDeployment")));
                        Guid messageFitlerId = retrievePluginTypes.GetSdkMessageFilterId(sdkmessageStepNode.GetAttribute("PrimaryEntityName"), messageId, service);
                        sdkmessageProcessingStep["sdkmessagefilterid"] = new EntityReference("sdkmessagefilter", messageFitlerId);
                        UpsertRequest upsertPluginStepsRequest = new UpsertRequest();
                        upsertPluginStepsRequest.Target = sdkmessageProcessingStep;
                        UpsertResponse upsertPluginStepResponse = (UpsertResponse)service.Execute(upsertPluginStepsRequest);
                        if (upsertPluginStepResponse.RecordCreated)
                        {
                            sdkmessageStepNode.Attributes["Id"].Value = upsertPluginStepResponse.Target.Id.ToString();
                        }

                        string messageName = sdkmessageStepNode.GetAttribute("MessageName");
                        XmlNodeList sdkstepImages = sdkmessageStepNode.SelectNodes("Images/Image");
                        foreach (XmlElement sdkstepImageNode in sdkstepImages)
                        {
                            Guid createdImageId = CreateSdkImage(upsertPluginStepResponse.Target.Id, sdkstepImageNode,messageName, service);
                            if (createdImageId != Guid.Empty)
                            {
                                sdkstepImageNode.Attributes["Id"].Value = createdImageId.ToString();
                            }
                        }
                    }
                }
            }

            xmlDoc.Save(registrationXmlPath);
        }

        private Guid CreateSdkImage(Guid stepId, XmlElement sdkstepImageNode, string messageName, IOrganizationService service)
        {
            Guid createdImageId = Guid.Empty;
            Entity image = new Entity("sdkmessageprocessingstepimage", new Guid(sdkstepImageNode.GetAttribute("Id")));
            switch (messageName)
            {
                case "Create":
                    image["messagepropertyname"] = "Id";
                    break;

                case "SetState":
                case "SetStateDynamicEntity":
                    image["messagepropertyname"] = "EntityMoniker";
                    break;

                case "Send":
                case "DeliverIncoming":
                case "DeliverPromote":
                    image["messagepropertyname"] = "EmailId";
                    break;

                default:
                    image["messagepropertyname"] = "Target";
                    break;
            }

            Console.WriteLine("Registering the sdk image: " + sdkstepImageNode.GetAttribute("Name"));
            image["imagetype"] = new OptionSetValue(Convert.ToInt32(sdkstepImageNode.GetAttribute("ImageType")));
            image["entityalias"] = sdkstepImageNode.GetAttribute("EntityAlias");
            image["name"] = sdkstepImageNode.GetAttribute("Name");
            image["attributes"] = sdkstepImageNode.GetAttribute("Attributes");
            image["sdkmessageprocessingstepid"] = new EntityReference("sdkmessageprocessingstep", stepId);
            UpsertRequest upsertsdkImage = new UpsertRequest();
            upsertsdkImage.Target = image;
            UpsertResponse upsertSdkImageReponse = (UpsertResponse)service.Execute(upsertsdkImage);
            if (upsertSdkImageReponse.RecordCreated)
            {
                createdImageId = upsertSdkImageReponse.Target.Id;
            }

            return createdImageId;
        }

        private Guid RegisterWorkflowTypes(EntityReference pluginAssembly, XmlElement workflowTypeNode, IOrganizationService service)
        {
            Guid workflowTypeId = Guid.Empty;
            Entity pluginTypeEntity = new Entity("plugintype", new Guid(workflowTypeNode.GetAttribute("Id")));
            pluginTypeEntity["typename"] = workflowTypeNode.GetAttribute("TypeName");
            pluginTypeEntity["friendlyname"] = workflowTypeNode.GetAttribute("friendlyname");
            pluginTypeEntity["name"] = workflowTypeNode.GetAttribute("Name");
            pluginTypeEntity["pluginassemblyid"] = pluginAssembly;
            pluginTypeEntity["workflowactivitygroupname"] = workflowTypeNode.GetAttribute("WorkflowActivityGroupName");
            pluginTypeEntity["isworkflowactivity"] = true;
            UpsertRequest upsertPluginTypeRequest = new UpsertRequest();
            upsertPluginTypeRequest.Target = pluginTypeEntity;
            UpsertResponse upsertPluginTypeResponse = (UpsertResponse)service.Execute(upsertPluginTypeRequest);
            if (upsertPluginTypeResponse.RecordCreated)
            {
                workflowTypeId = upsertPluginTypeResponse.Target.Id;
            }

            return workflowTypeId;
        }
    }
}
