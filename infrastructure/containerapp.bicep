param containerAppName string
param location string = resourceGroup().location

param enableIngress bool
param ingressTargetPort int = 80

param containerName string
param containerVersion string
param containerResources object = {
  cpu: json('0.25')
  memory: '0.5Gi'
}
param environmentVariables array

param enableDapr bool
param daprAppName string = containerName
param daprPort int = ingressTargetPort

param containerRegistryResourceName string
param containerRegistryResourceGroupName string

param containerAppEnvironmentResourceName string
param containerAppEnvironmentResourceGroupName string

param minimumReplicas int = 0
param maximumReplicas int = 6

param enableHttpTrafficBasedScaling bool

var httpScaling = enableHttpTrafficBasedScaling ? [
  {
    name: 'http-rule'
    http: {
      metadata: {
        concurrentRequests: '30'
      }
    }
  }
] : []

var ingress = enableIngress ? {
  external: true
  targetPort: ingressTargetPort
  transport: 'auto'
  allowInsecure: false
  traffic: [
    {
      weight: 100
      latestRevision: true
    }
  ]
} : {}

var dapr = enableDapr ? {
  enabled: enableDapr
  appPort: daprPort
  appId: daprAppName
} : {}

resource containerAppEnvironments 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: containerAppEnvironmentResourceName
  scope: resourceGroup(containerAppEnvironmentResourceGroupName)
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2022-02-01-preview' existing = {
  name: containerRegistryResourceName
  scope: resourceGroup(containerRegistryResourceGroupName)
}

resource apiContainerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: containerAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppEnvironments.id

    configuration: {
      activeRevisionsMode: 'Single'
      ingress: ingress
      dapr: dapr
      secrets: [
        {
          name: 'container-registry-password'
          value: containerRegistry.listCredentials().passwords[0].value
        }
      ]
      registries: [
        {
          server: containerRegistry.properties.loginServer
          username: containerRegistry.name
          passwordSecretRef: 'container-registry-password'
        }
      ]
    }
    template: {
      containers: [
        {
          image: '${containerRegistry.properties.loginServer}/${containerName}:${containerVersion}'
          name: containerName
          resources: containerResources
          env: environmentVariables
        }
      ]
      scale: {
        minReplicas: minimumReplicas
        maxReplicas: maximumReplicas
        rules: httpScaling
      }
    }
  }
}

output principalId string = apiContainerApp.identity.principalId
