@description('Function app name.')
@minLength(1)
param name string

@description('Function app kind.')
@allowed([
  'functionapp'
  'linux'
  'container'
])
param kind string = 'functionapp'

@description('Function app location.')
param location string

@description('Service plan name.')
@minLength(1)
param servicePlanName string

@description('app settings')
param secretSettings object

@description('Name of the \'data\' key vault.')
param dataKeyVaultName string

@description('Name of the resource group where the \'data\' key vault is located.')
param dataKeyVaultResourceGroup string

@description('User assigned managed identities. Single or list of user assigned managed identities. Format: /subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ManagedIdentity/userAssignedIdentities/{identityName}')
param userManagedIdentities object = {}

var deployUserManagedIdentity = userManagedIdentities != null && userManagedIdentities != {}

@description('Log Analytics Workspace Id.')
param logAnalyticsWorkspaceId string

resource functionApp 'Microsoft.Web/sites@2018-02-01' = {
  name: name
  location: location
  kind: kind
  properties: {
    serverFarmId: resourceId('Microsoft.Web/serverfarms', servicePlanName)
    clientAffinityEnabled: false
    httpsOnly: true
    siteConfig: {
      use32BitWorkerProcess : false
      appSettings: secretSettings
      ftpsState: 'Disabled'
    }
  }
  identity: {
    type: deployUserManagedIdentity ? 'SystemAssigned, UserAssigned' : 'SystemAssigned'
    userAssignedIdentities: deployUserManagedIdentity ? userManagedIdentities : null
  }
}

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'functionApp-diagnostics'
  scope: functionApp
  properties: {
    workspaceId:  logAnalyticsWorkspaceId
    logs: [
      {
        category: 'FunctionAppLogs'
        enabled: true
        retentionPolicy: {
          days: 0
          enabled: false
        }
      }
    ]
  }
}

resource snScmBasicAuth 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2022-09-01' = {
  parent: functionApp
  name: 'scm'
  properties: {
    allow: false
  }
}

resource snFtpBasicAuth 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2022-09-01' = {
  parent: functionApp
  name: 'ftp'
  properties: {
    allow: false
  }
}

module secretsTemplate 'keyVaultSecrets.bicep' = {
  name: 'secretsTemplate-AzureUserReader'
  scope: resourceGroup(dataKeyVaultResourceGroup)
  params: {
    keyVaultName: dataKeyVaultName
    keyVaultParameters: [
      {
        name: 'azureUserReaderUrl'
        value: 'https://${functionApp.properties.defaultHostName}'
      }
      {
        name: 'azureUserReaderFunctionName'
        value: '${name}-AzureUserReader'
      }
    ]
  }
}

module secureSecretsTemplate 'keyVaultSecretsSecure.bicep' = {
  name: 'secureSecretsTemplate-AzureUserReader'
  scope: resourceGroup(dataKeyVaultResourceGroup)
  params: {
    keyVaultName: dataKeyVaultName
    keyVaultSecrets: {
      secrets: [
        {
          name: 'azureUserReaderKey'
          value: listkeys('${functionApp.id}/host/default', '2018-11-01').functionKeys.default
        }
      ]
    }
  }
}



resource functionAppSlotConfig 'Microsoft.Web/sites/config@2021-03-01' = {
  name: 'slotConfigNames'
  parent: functionApp
  properties: {
    appSettingNames: [
      'AzureFunctionsJobHost__extensions__durableTask__hubName'
      'AzureWebJobs.StarterFunction.Disabled'
      'AzureWebJobs.OrchestratorFunction.Disabled'
      'AzureWebJobs.UserCreatorSubOrchestratorFunction.Disabled'
      'AzureWebJobs.UserReaderSubOrchestratorFunction.Disabled'
      'AzureWebJobs.AzureUserCreatorFunction.Disabled'
      'AzureWebJobs.AzureUserReaderFunction.Disabled'
      'AzureWebJobs.PersonnelNumberReaderFunction.Disabled'
      'AzureWebJobs.UploadUsersFunction.Disabled'
      'AzureWebJobsStorage'
      'AzureFunctionsWebHost__hostid'
    ]
  }
}

output msi string = functionApp.identity.principalId
