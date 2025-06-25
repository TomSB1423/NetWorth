@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource funcstoragef2721 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: take('funcstoragef2721${uniqueString(resourceGroup().id)}', 24)
  kind: 'StorageV2'
  location: location
  sku: {
    name: 'Standard_GRS'
  }
  properties: {
    accessTier: 'Hot'
    allowSharedKeyAccess: false
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
  tags: {
    'aspire-resource-name': 'funcstoragef2721'
  }
}

resource blobs 'Microsoft.Storage/storageAccounts/blobServices@2024-01-01' = {
  name: 'default'
  parent: funcstoragef2721
}

output blobEndpoint string = funcstoragef2721.properties.primaryEndpoints.blob

output queueEndpoint string = funcstoragef2721.properties.primaryEndpoints.queue

output tableEndpoint string = funcstoragef2721.properties.primaryEndpoints.table

output name string = funcstoragef2721.name