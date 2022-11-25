targetScope = 'subscription'

param systemName string
param integrationSystemName string
param environmentName string
param locationAbbreviation string
param location string = deployment().location

param containerRegistryResourceGroupName string
param containerRegistryResourceName string

param containerImageName string
param containerImageVersion string
param containerExposedPort int
param daprApplicationId string
param daprApplicationPort int

param storageTables array

var resourceGroupName = '${systemName}-${environmentName}-${locationAbbreviation}'
var defaultResourceName = resourceGroupName

var integrationResourceGroupName = '${integrationSystemName}-${environmentName}-${locationAbbreviation}'
var azureAppConfigurationName = '${integrationResourceGroupName}-cfg'
var azureContainerAppEnvironmentName = '${integrationResourceGroupName}-env'

resource targetResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

module resourcesModule 'resources.bicep' = {
  name: 'ResourcesModule'
  scope: targetResourceGroup
  params: {
    azureAppConfigurationName: azureAppConfigurationName
    containerAppEnvironmentResourceName: azureContainerAppEnvironmentName
    containerVersion: containerImageVersion
    defaultResourceName: defaultResourceName
    integrationResourceGroupName: integrationResourceGroupName
    location: location
    storageAccountTables: storageTables
    containerName: containerImageName
    containerPort: containerExposedPort
    containerRegistryResourceGroupName: containerRegistryResourceGroupName
    containerRegistryResourceName: containerRegistryResourceName
  }
}
