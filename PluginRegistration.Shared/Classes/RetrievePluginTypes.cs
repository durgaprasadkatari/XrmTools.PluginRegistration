using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginRegistration.Shared.Classes
{
    public class RetrievePluginTypes
    {
        public EntityCollection GetPluginAssemblies(IOrganizationService service)
        {
            QueryExpression pluginAssemblyQueryExpression = new QueryExpression("pluginassembly");
            pluginAssemblyQueryExpression.ColumnSet = new ColumnSet(true);
            pluginAssemblyQueryExpression.Criteria = new FilterExpression
            {
                Conditions =
            {
              new ConditionExpression
              {
                AttributeName = "ismanaged",
                Operator = ConditionOperator.Equal,
                Values = { false }
               },
              }
            };

            //RETRIEVE ASSEMBLY
            EntityCollection pluginAssemblies = service.RetrieveMultiple(pluginAssemblyQueryExpression);
            return pluginAssemblies;
        }

        public EntityCollection GetPluginTypes(Guid assemblyId, IOrganizationService service)
        {
            QueryExpression pluginTypeQueryExpression = new QueryExpression("plugintype");
            pluginTypeQueryExpression.ColumnSet = new ColumnSet(true);
            pluginTypeQueryExpression.Criteria = new FilterExpression
            {
                Conditions =
                                    {
                                        new ConditionExpression
                                        {
                                            AttributeName = "pluginassemblyid",
                                            Operator = ConditionOperator.Equal,
                                            Values = {assemblyId}
                                        }

                                    }
            };

            //RETRIEVE PLUGIN TYPES IN ASSEMBLY
            return service.RetrieveMultiple(pluginTypeQueryExpression);
        }

        public EntityCollection GetSdkProcessingStep(Guid pluginTypeId, IOrganizationService service)
        {
            QueryExpression sdkStepQueryExpression = new QueryExpression("sdkmessageprocessingstep");
            sdkStepQueryExpression.ColumnSet = new ColumnSet(true);
            sdkStepQueryExpression.Criteria = new FilterExpression
            {
                Conditions =
                                    {
                                        new ConditionExpression
                                        {
                                            AttributeName = "plugintypeid",
                                            Operator = ConditionOperator.Equal,
                                            Values = { pluginTypeId }
                                        }

                                    }
            };

            //RETRIEVE PLUGIN Steps IN Plugin-Type
            return service.RetrieveMultiple(sdkStepQueryExpression);
        }

        public EntityCollection GetSdkProcessingStepImage(Guid sdkstepId, IOrganizationService service)
        {
            QueryExpression sdkImageQueryExpression = new QueryExpression("sdkmessageprocessingstepimage");
            sdkImageQueryExpression.ColumnSet = new ColumnSet(true);
            sdkImageQueryExpression.Criteria = new FilterExpression
            {
                Conditions =
                                    {
                                        new ConditionExpression
                                        {
                                            AttributeName = "sdkmessageprocessingstepid",
                                            Operator = ConditionOperator.Equal,
                                            Values = { sdkstepId }
                                        }

                                    }
            };

            //RETRIEVE PLUGIN Steps IN Plugin-Type
            return service.RetrieveMultiple(sdkImageQueryExpression);
        }

        public Guid GetSdkMessageId(string SdkMessageName, IOrganizationService service)
        {
            try
            {
                //GET SDK MESSAGE QUERY
                QueryExpression sdkMessageQueryExpression = new QueryExpression("sdkmessage");
                sdkMessageQueryExpression.ColumnSet = new ColumnSet("sdkmessageid");
                sdkMessageQueryExpression.Criteria = new FilterExpression
                {
                    Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "name",
                                        Operator = ConditionOperator.Equal,
                                        Values = {SdkMessageName}
                                    },
                                }
                };

                //RETRIEVE SDK MESSAGE
                EntityCollection sdkMessages = service.RetrieveMultiple(sdkMessageQueryExpression);
                if (sdkMessages.Entities.Count != 0)
                {
                    return sdkMessages.Entities.First().Id;
                }
                throw new Exception(String.Format("SDK MessageName {0} was not found.", SdkMessageName));
            }
            catch (InvalidPluginExecutionException invalidPluginExecutionException)
            {
                throw invalidPluginExecutionException;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public Guid GetSdkMessageFilterId(string entityLogicalName, Guid sdkmessageId, IOrganizationService service)
        {
            try
            {
                //GET SDK MESSAGE FILTER QUERY
                QueryExpression sdkMessageFilterQueryExpression = new QueryExpression("sdkmessagefilter");
                sdkMessageFilterQueryExpression.ColumnSet = new ColumnSet("sdkmessagefilterid");
                sdkMessageFilterQueryExpression.Criteria = new FilterExpression
                {
                    Conditions =
                                {
                                    new ConditionExpression
                                    {
                                        AttributeName = "primaryobjecttypecode",
                                        Operator = ConditionOperator.Equal,
                                        Values = { entityLogicalName }
                                    },
                                    new ConditionExpression
                                    {
                                        AttributeName = "sdkmessageid",
                                        Operator = ConditionOperator.Equal,
                                        Values = { sdkmessageId }
                                    },
                                }
                };

                //RETRIEVE SDK MESSAGE FILTER
                EntityCollection sdkMessageFilters = service.RetrieveMultiple(sdkMessageFilterQueryExpression);

                if (sdkMessageFilters.Entities.Count != 0)
                {
                    return sdkMessageFilters.Entities.First().Id;
                }
                throw new Exception(String.Format("SDK Message Filter for {0} was not found.", entityLogicalName));
            }
            catch (InvalidPluginExecutionException invalidPluginExecutionException)
            {
                throw invalidPluginExecutionException;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
