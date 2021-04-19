using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PluginRegistrationUsingXml.Classes
{
    public class GenerateRegistrationFile
    {
        public void GeneratePluginOrWorkflowTypes(bool isWorkflowActivity, List<Entity> pluginTypesColl, ref XmlNode solution, ref XmlDocument xmlDoc, IOrganizationService service)
        {
            RetrievePluginTypes retrievePluginTypes = new RetrievePluginTypes();
            XmlNode pluginTypes;
            if (!isWorkflowActivity)
            {
                pluginTypes = xmlDoc.CreateElement("PluginTypes");
                solution.AppendChild(pluginTypes);
            }
            else
            {
                pluginTypes = xmlDoc.CreateElement("WorkflowTypes");
                solution.AppendChild(pluginTypes);
            }
            foreach (Entity pluginType in pluginTypesColl)
            {
                XmlNode pluginTypeNode = null;
                if (!isWorkflowActivity)
                {
                    pluginTypeNode = xmlDoc.CreateElement("Plugin");
                }
                else
                {
                    pluginTypeNode = xmlDoc.CreateElement("WorkflowType");
                    XmlAttribute workflowactivitygroupname = xmlDoc.CreateAttribute("WorkflowActivityGroupName");
                    workflowactivitygroupname.Value = pluginType["workflowactivitygroupname"].ToString();
                    pluginTypeNode.Attributes.Append(workflowactivitygroupname);
                }

                pluginTypes.AppendChild(pluginTypeNode);

                if (pluginType.Attributes.Contains("description"))
                {
                    XmlAttribute description = xmlDoc.CreateAttribute("Description");
                    description.Value = pluginType["description"].ToString();
                    pluginTypeNode.Attributes.Append(description);
                }

                XmlAttribute friendlyname = xmlDoc.CreateAttribute("friendlyname");
                friendlyname.Value = pluginType["friendlyname"].ToString();
                pluginTypeNode.Attributes.Append(friendlyname);
                XmlAttribute name = xmlDoc.CreateAttribute("Name");
                name.Value = pluginType["name"].ToString();
                pluginTypeNode.Attributes.Append(name);
                XmlAttribute plugintypeId = xmlDoc.CreateAttribute("Id");
                plugintypeId.Value = pluginType.Id.ToString();
                pluginTypeNode.Attributes.Append(plugintypeId);
                XmlAttribute typeName = xmlDoc.CreateAttribute("TypeName");
                typeName.Value = pluginType["typename"].ToString();
                pluginTypeNode.Attributes.Append(typeName);

                if (!isWorkflowActivity)
                {
                    //Steps
                    XmlNode steps = xmlDoc.CreateElement("Steps");
                    pluginTypeNode.AppendChild(steps);
                    GeneratePluginSdkMessageProcessingStep(retrievePluginTypes, pluginType.Id, ref steps, ref xmlDoc, service);
                }
            }
        }

        private void GeneratePluginSdkMessageProcessingStep(RetrievePluginTypes retrievePluginTypes, Guid pluginTypeId, ref XmlNode steps, ref XmlDocument xmlDoc, IOrganizationService service)
        {
            EntityCollection stepsColl = retrievePluginTypes.GetSdkProcessingStep(pluginTypeId, service);
            foreach (Entity sdkstep in stepsColl.Entities)
            {
                try
                {
                    XmlNode step = xmlDoc.CreateElement("Step");
                    steps.AppendChild(step);
                    XmlAttribute stepname = xmlDoc.CreateAttribute("Name");
                    stepname.Value = sdkstep["name"].ToString();
                    step.Attributes.Append(stepname);
                    XmlAttribute stepDescription = xmlDoc.CreateAttribute("Description");
                    if (sdkstep.Attributes.Contains("description") && sdkstep["description"] != null)
                    {
                        stepDescription.Value = sdkstep["description"].ToString();
                    }
                    else
                    {
                        stepDescription.Value = sdkstep["name"].ToString();
                    }
                    step.Attributes.Append(stepDescription);
                    XmlAttribute stepId = xmlDoc.CreateAttribute("Id");
                    stepId.Value = sdkstep.Id.ToString();
                    step.Attributes.Append(stepId);
                    if (sdkstep.Attributes.Contains("filteringattributes"))
                    {
                        XmlAttribute filteringAttributes = xmlDoc.CreateAttribute("FilteringAttributes");
                        filteringAttributes.Value = sdkstep["filteringattributes"].ToString();
                        step.Attributes.Append(filteringAttributes);
                    }
                    XmlAttribute sdkstepMode = xmlDoc.CreateAttribute("Mode");
                    sdkstepMode.Value = sdkstep["mode"].ToString();
                    step.Attributes.Append(sdkstepMode);
                    EntityReference sdkmessageRef = (EntityReference)sdkstep["sdkmessageid"];
                    Entity sdkmessageEntity = service.Retrieve(sdkmessageRef.LogicalName, sdkmessageRef.Id, new ColumnSet("name"));
                    XmlAttribute sdkmessage = xmlDoc.CreateAttribute("MessageName");
                    sdkmessage.Value = sdkmessageEntity["name"].ToString();
                    step.Attributes.Append(sdkmessage);
                    XmlAttribute mode = xmlDoc.CreateAttribute("Mode");
                    mode.Value = ((OptionSetValue)sdkstep["mode"]).Value.ToString();
                    step.Attributes.Append(mode);
                    XmlAttribute rank = xmlDoc.CreateAttribute("Rank");
                    rank.Value = sdkstep["rank"].ToString();
                    step.Attributes.Append(rank);
                    XmlAttribute stage = xmlDoc.CreateAttribute("Stage");
                    stage.Value = ((OptionSetValue)sdkstep["stage"]).Value.ToString();
                    step.Attributes.Append(stage);
                    XmlAttribute supporteddeployment = xmlDoc.CreateAttribute("SupportedDeployment");
                    supporteddeployment.Value = ((OptionSetValue)sdkstep["supporteddeployment"]).Value.ToString();
                    step.Attributes.Append(supporteddeployment);
                    //sdkmessage filter
                    XmlAttribute primaryentity = xmlDoc.CreateAttribute("PrimaryEntityName");
                    if (sdkstep.Attributes.Contains("sdkmessagefilterid") && sdkstep["sdkmessagefilterid"] != null)
                    {
                        EntityReference sdkmessageFilterRef = (EntityReference)sdkstep["sdkmessagefilterid"];
                        Entity sdkmessageFilterEntity = service.Retrieve(sdkmessageFilterRef.LogicalName, sdkmessageFilterRef.Id, new ColumnSet("primaryobjecttypecode", "sdkmessageid"));
                        primaryentity.Value = sdkmessageFilterEntity["primaryobjecttypecode"].ToString();
                    }
                    else
                    {
                        primaryentity.Value = "none";
                    }
                    step.Attributes.Append(primaryentity);

                    //Images
                    XmlNode Images = xmlDoc.CreateElement("Images");
                    step.AppendChild(Images);
                    GeneratePluginSdkMessageImage(retrievePluginTypes, sdkstep.Id, ref Images, ref xmlDoc, service);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void GeneratePluginSdkMessageImage(RetrievePluginTypes retrievePluginTypes, Guid pluginstepId, ref XmlNode images, ref XmlDocument xmlDoc, IOrganizationService service)
        {
            EntityCollection imageColl = retrievePluginTypes.GetSdkProcessingStepImage(pluginstepId, service);
            foreach (Entity sdkstepImage in imageColl.Entities)
            {
                XmlNode image = xmlDoc.CreateElement("Image");
                images.AppendChild(image);

                XmlAttribute attributes = xmlDoc.CreateAttribute("Attributes");
                attributes.Value = sdkstepImage.Attributes.Contains("attributes") ? sdkstepImage["attributes"].ToString() : "All";
                image.Attributes.Append(attributes);

                XmlAttribute entityAlias = xmlDoc.CreateAttribute("EntityAlias");
                entityAlias.Value = sdkstepImage["entityalias"].ToString();
                image.Attributes.Append(entityAlias);

                XmlAttribute name = xmlDoc.CreateAttribute("Name");
                name.Value = sdkstepImage["name"].ToString();
                image.Attributes.Append(name);

                XmlAttribute id = xmlDoc.CreateAttribute("Id");
                id.Value = sdkstepImage.Id.ToString();
                image.Attributes.Append(id);

                XmlAttribute messagePropertyName = xmlDoc.CreateAttribute("MessagePropertyName");
                messagePropertyName.Value = sdkstepImage["messagepropertyname"].ToString();
                image.Attributes.Append(messagePropertyName);

                XmlAttribute imageType = xmlDoc.CreateAttribute("ImageType");
                imageType.Value = ((OptionSetValue)sdkstepImage["imagetype"]).Value.ToString();
                image.Attributes.Append(imageType);
            }
        }
    }
}
