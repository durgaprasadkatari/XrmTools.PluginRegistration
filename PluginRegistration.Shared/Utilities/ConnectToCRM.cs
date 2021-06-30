using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Configuration;

namespace PluginRegistration.Shared.Utilities
{
   public static class ConnectToCrm
    {
        public static IOrganizationService GetCrmOrgnizationService(string connectionString)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            CrmServiceClient conn = new CrmServiceClient(connectionString);
            IOrganizationService _orgService;
            _orgService = conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
            return _orgService;
        }
    }
}
