using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Resources.Models;
using Azure.ResourceManager.Storage.Models;

class Program
{
    static async Task Main(string[] args)
    {
        // Set variables
        string subscriptionId = "<YOUR_SUBSCRIPTION_ID>";
        string resourceGroupName = "myBlobResourceGroup";
        string location = "eastus";
        string storageAccountName = "myuniquestorageacct123";

        // Authenticate
        var armClient = new ArmClient(new DefaultAzureCredential());
        var subscription = armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{subscriptionId}"));

        // Create resource group
        var rgData = new ResourceGroupData(location);
        var resourceGroup = await subscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, resourceGroupName, rgData);

        // Define storage account parameters
        var storageData = new StorageAccountCreateOrUpdateContent(
            new StorageSku(StorageSkuName.StandardLRS),
            StorageKind.StorageV2,
            location)
        {
            AccessTier = StorageAccountAccessTier.Hot
        };

        // Create the storage account
        var storageAccount = await resourceGroup.Value.GetStorageAccounts()
            .CreateOrUpdateAsync(WaitUntil.Completed, storageAccountName, storageData);

        Console.WriteLine($"Storage account '{storageAccountName}' created successfully in resource group '{resourceGroupName}'.");
    }
}
