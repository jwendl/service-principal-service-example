# Service Principal Example

This is an example of how to use Microsoft Graph and the On Behalf of Flow in Azure AD to create a service principal.

## Application Registration for Server Application

Steps are leveraging steps from: [Azure AD Docs](https://github.com/Azure-Samples/active-directory-dotnet-native-aspnetcore-v2/tree/master/2.%20Web%20API%20now%20calls%20Microsoft%20Graph#register-the-service-app-todolistservice).

1. Navigate to Application Registration page
1. Click on Add New Application
    1. Provide a name for the registration (ServicePrincipalService-Server)
    1. For the Supported Account Types choose Accounts in this organization directory only
1. On the overview tab we will need the application id and tenant id
1. Under Certificates and Secrets, create a secret - we will need this somehow too. Currently our plan is to store it inside Azure Key Vault for the applications to use so it will not be exposed anywhere
1. Select API Permissions
    1. Click Add Permission
        1. Select Microsoft Graph under Microsoft APIs
        1. Go under Delegated permissions
            1. Select Application.ReadWrite.All
    1. Select "Grant admin consent for…" This is required for the on-behalf-of flow because a UI isn't available for users to consent with
1. Select Expose and API
    1. Name the app (something like api://wba-rx-virtual-consult-server)
    1. Enter in a scope name like access_as_user
    1. Select Admin and Users
    1. Enter display name and description (anything works like "Access the server as a user"
    1. Select AddScope
1. Click on Manifest
    1. Edit the "accessTokenAcceptedVersion" and set that value to "2"

## Application Registration for Client Application

Steps are leveraging steps from: [Azure AD Docs](https://github.com/Azure-Samples/active-directory-dotnet-native-aspnetcore-v2/tree/master/2.%20Web%20API%20now%20calls%20Microsoft%20Graph#register-the-client-app-todolistclient).

1. Navigate to App Registrations page
1. Add new Application
    1. Provide a name for the registration (ServicePrincipalService-Client)
    1. For the Supported Account Types choose Accounts in this organization directory only
1. On the overview tab, we will need the application id and tenant id
1. Select Authentication tab
    1. Click on Add a platform
    1. Add the platform that you are using (in our case just add native client application since we are using device code in this repo)
1. Select API Permissions tab
    1. Click add permission
    1. Click My APIs
    1. Select the server app (wba-rx-virtual-consult-server)
    1. In delegated permissions select access_as_user
    1. Select add permission button

Finally, go back to the server application

1. Open up Manifest
1. Change "knownClientApplications": [] to be

``` json
"knownClientApplications": [
         "<client application guid>"
 ],
```
