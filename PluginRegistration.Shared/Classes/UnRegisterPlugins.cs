using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginRegistration.Shared.Classes
{
    public class UnRegisterPlugins
    {
        public void UnRegisterPluginTypes(Guid pluginAssemblyId, IOrganizationService service)
        {
            RetrievePluginTypes retrievePluginTypes = new RetrievePluginTypes();
            EntityCollection pluginTypesColl = retrievePluginTypes.GetPluginTypes(pluginAssemblyId, service);
            foreach (Entity pluginTypeEntity in pluginTypesColl.Entities)
            {
                EntityCollection pluginStepsColl = retrievePluginTypes.GetSdkProcessingStep(pluginTypeEntity.Id, service);
                foreach (Entity pluginStep in pluginStepsColl.Entities)
                {
                    EntityCollection stepImageColl = retrievePluginTypes.GetSdkProcessingStepImage(pluginStep.Id, service);
                    foreach (Entity stepImage in stepImageColl.Entities)
                    {
                        service.Delete(stepImage.LogicalName, stepImage.Id);
                    }
                    service.Delete(pluginStep.LogicalName, pluginStep.Id);
                }
                service.Delete(pluginTypeEntity.LogicalName, pluginTypeEntity.Id);
            }
        }
    }
}
