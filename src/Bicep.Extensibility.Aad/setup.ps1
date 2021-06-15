az login

$subscriptionId = & az account show --query "id"

$appName = "bicep_poc_aad"
$creds = & az ad sp create-for-rbac `
  --name $appName `
  --sdk-auth `
  --role contributor `
  --scopes /subscriptions/$subscriptionId

$appId = ($creds | ConvertFrom-Json).clientId

$sp = & az ad sp show --id $appId
$spId = ($sp | ConvertFrom-Json).objectId

$msGraphSp = & az ad sp show --id 00000003-0000-0000-c000-000000000000
$msGraphSpId = ($msGraphSp | ConvertFrom-Json).objectId

Start-Sleep -Seconds 5 # replication delay?

& az rest -m POST -u https://graph.microsoft.com/v1.0/servicePrincipals/$spId/appRoleAssignments --headers "Content-Type=application/json" -b "{\""principalId\"": \""$spId\"", \""resourceId\"": \""$msGraphSpId\"", \""appRoleId\"": \""1bfefb4e-e0b5-418b-a88f-73c46d2cc8e9\""}"

$creds > "$env:LOCALAPPDATA\BICEP_POC_AAD_CREDS"