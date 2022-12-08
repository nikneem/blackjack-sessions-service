param defaultResourceName string
param location string
param storageAccountTables array
param containerVersion string
param integrationResourceGroupName string
param containerAppEnvironmentResourceName string
param azureAppConfigurationName string

param containerRegistryResourceGroupName string
param containerRegistryResourceName string

param containerPort int
param containerName string

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: azureAppConfigurationName
  scope: resourceGroup(integrationResourceGroupName)
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: uniqueString(defaultResourceName)
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}
resource storageAccountTableService 'Microsoft.Storage/storageAccounts/tableServices@2021-09-01' = {
  name: 'default'
  parent: storageAccount
}
resource storageAccountTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2021-09-01' = [for table in storageAccountTables: {
  name: table
  parent: storageAccountTableService
}]

var environmentVariables = [
  {
    name: 'Azure__AppConfiguration'
    value: appConfiguration.properties.endpoint
  }
  {
    name: 'AzureWebJobsStorage'
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
  }
  {
    name: 'Azure__StorageAccount'
    value: storageAccount.name
  }
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: 'dotnet-isolated'
  }
]

module containerAppModuleModule 'containerapp.bicep' = {
  name: 'ContainerAppModuleModule'
  params: {
    containerAppName: '${defaultResourceName}-aca'
    location: location
    containerAppEnvironmentResourceGroupName: integrationResourceGroupName
    containerAppEnvironmentResourceName: containerAppEnvironmentResourceName
    containerName: containerName
    containerRegistryResourceName: containerRegistryResourceName
    containerRegistryResourceGroupName: containerRegistryResourceGroupName
    containerVersion: containerVersion
    enableDapr: true
    daprPort: containerPort
    enableHttpTrafficBasedScaling: true
    enableIngress: true
    environmentVariables: environmentVariables
  }
}

module roleAssignmentsModule 'role-assignments.bicep' = {
  name: 'roleAssignmentsModule'
  params: {
    containerAppPrincipalId: containerAppModuleModule.outputs.principalId
    integrationResourceGroupName: integrationResourceGroupName
  }
}
