# Smart Buildings Solution  

## Deployment Guide

### Table of Contents 


<!--ts-->
 - [1.0 Deployment Guide](#10-deployment-guide)
 - [2.0 What are Paired Regions](#20-what-are-paired-regions)
 - [3.0 Prerequisites for Deploying ARM Template](#30-prerequisites-for-deploying-arm-template)
    - [3.1 Integrating applications with Azure Active Directory](#31-integrating-applications-with-azure-active-directory)
        - [3.1.1 To register a new application in Azure AD using the Azure portal](#311-to-register-a-new-application-in-azure-ad-using-the-azure-portal)
        - [3.1.2 To get application ID and authentication key](#312-to-get-application-id-and-authentication-key)
        - [3.1.3 To Add Reply url](#313-to-add-reply-url)
        - [3.1.4 Add app Roles to Manifest File of Application](#313-add-app-roles-to-manifest-file-of-application)
        - [3.1.5 To get the User ID from Directory](#315-to-get-the-user-id-from-directory)
        - [3.1.6 To get Tenant ID](#316-to-get-tenant-id)
        - [3.1.7 To Assign owner permissions to the Application](#317-to-assign-owner-permissions-to-the-application)
        - [3.1.8 Add User to the Application](#318-add-user-to-the-application)
    - [3.2 Creating session ID](#32-creating-session-id)
    - [3.3 To register a new application in Microsoft Bot Framework](#33-to-register-a-new-application-in-microsoft-bot-framework)
    - [3.4 Azure B2C Tenant Creation and Configuration](#34-azure-b2c-tenant-creation-and-configuration)
        - [3.4.1 Create an Application in Azure B2C](#341-create-an-application-in-azure-b2c)
    - [3.5 Create an Azure Service Principal with Azure CLI](#35-create-an-azure-service-principal-with-azure-cli)
    - [3.6 Create a Twilio Account](#36-create-a-twilio-account)
- [4.0 ARM Template Input Parameters](#40-arm-template-input-parameters)
- [5.0 Getting Started](#50-getting-started)
    - [5.1 ARM Template Deployment Using Azure Portal](#51-arm-template-deployment-using-azure-portal)
        - [5.1.1 Inputs](#511-Inputs)
        - [5.1.2 Output](#512-Output)
    - [5.2 ARM Template Deployment Using Azure CLI](#52-arm-template-deployment-using-azure-cli)
        - [5.2.1 Create Resource Group using Azure CLI](#521-create-resource-group-using-azure-cli)
        - [5.2.2 Execute the Template Deployment](#522-execute-the-template-deployment)
- [6.0 Post Deployment](#60-post-deployment)
    - [6.1 Adding messaging endpoint in Bot](#61-adding-messaging-endpoint-in-bot)
    - [6.2 Enable Enhance authentication options](#62-enable-enhance-authentication-options)
    - [6.3 Login to Edison Virtual Machine](#63-login-to-edison-virtual-machine)
    - [6.4 Setting up the Environment](#64-setting-up-the-environment)
        - [6.4.1 Execute configupdate2.sh](#641-execute-configupdate2.sh)
        - [6.4.2 Execute commonupdate3.sh](#642-execute-commonupdate3.sh)
        - [6.4.3 Execute edsionwebenvupdate4.sh](#643-execute-edsionwebenvupdate4.sh)
        - [6.4.4 Execute updateappsettings5.sh](#644-execute-updateappsettings5.sh)
 <!--te--> 
    

## 1.0 Deployment Guide

This Document explains about how to deploy Project-Edison solution using ARM Template. In this Document explained about two ways of deploying solution.

*	Using Azure portal

*	Using Azure CLI

This document explains about input parameters, output parameters and points to be noted while deploying ARM Template.

## 2.0 What are Paired Regions

Azure operates in multiple geographies around the world. An Azure geography is a defined area of the world that contains at least one Azure Region. An Azure region is an area within a geography, containing one or more datacenters. 

Each Azure region is paired with another region within the same geography, together making a regional pair. The exception is Brazil South, which is paired with a region outside its geography. 

IoT Hub Manual Failover Support Geo-Paired Regions:

| **S.NO**           | **Geography**                  | **Paired Regions**                                                                                                                
| -------------              | ------------------         | --------------------  
| 1                  | North America          | East US 2           |  Central US 
| 2                  | North America          | Central US           |  East US 2    
| 3                  | North America          | West US 2           |  West Central US
| 4                  | North America          | West Central US           |  West US 2
| 5                  | Canada          | Canada Central           |  Canada East
| 6                  | Canada          | Canada East           |  Canada Central
| 7                  | Australia          | Australia East           |  Australia South East
| 8                  | Australia          | Australia South East           |  Australia East
| 9                  | India          | Central India           |  South India
| 10                 | India          | South India           |  Central India
| 11                 | Asia         | East Asia           |  South East Asia
| 12                 | Asia         | South East Asia           |  East Asia
| 13                 | Japan          | Japan West           |  Japan East
| 14                 | Japan          | Japan East           |  Japan West
| 15                 | Korea          | Korea Central           |  Korea South
| 16                 | Korea         | Korea South           |  Korea Central
| 17                 | UK          | UK South           |  UK West
| 18                 | UK          | UK West          |  UK South

## 3.0 Prerequisites for Deploying ARM Template

* Create an application in Azure Active Directory.

* Create a session ID

* Create an application in Bot Frame work portal.

* Create an application in Azure B2C.

* Create an application in Service principle using Azure CLI.

* Create Twilio Account

### 3.1 Integrating applications with Azure Active Directory

Any application that wants to use the capabilities of Azure AD must first be registered in an Azure AD tenant. This registration process involves giving Azure AD details about your application, such as the URL where it’s located, the URL to send replies after a user is authenticated, the URI that identifies the app, and so on.

#### 3.1.1 To register a new application in Azure AD using the Azure portal

1.	Sign in to the **Azure portal**.

2.	In the left-hand navigation pane, click the **Azure Active Directory(symbol)** service, click **App registrations**, and click **+ New application registration**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d1.png)

3.	When the **Create** page appears, enter your application's registration information:

*	**Name**: Enter the application name

*	**Application type**:

    o	Select "Web app / API" for client applications and resource/API applications that are installed on a secure server. This setting is used for OAuth confidential web clients and public user-agent-based clients. The same application can also expose both a client and resource/API.

*	**Sign-On URL**: For "Web app / API" applications, provide the base URL of your app. For example, **https://localhost** might be the URL for a web app running on your local machine. Users would use this URL to sign in to a web client application.

4.	When finished, click **Create**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d2.png)

#### 3.1.2 To get application ID and authentication key

1.	From **App registrations** in Azure Active Directory, **select** your **application**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d3.png)
 
2.	**Copy** the **Application ID** and **object ID**. The application ID value is referred as the **client ID**.

3.	Save the **Application ID** and **object ID** which is highlighted in the below figure, this will be used while deploying the **ARM template**.

 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d4.png)

4.	Click on the **Settings** page of the application. Click on **Keys** section on the **Settings** page.


 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d5.png)

5.	Add a description for your key and Select duration, click on **Save**. 

 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d6.png)

6.	The right-most column will contain the key value, after you save the configuration changes. **Make sure to copy the key** for use in your client application code, as it is not accessible once you leave this page.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d7.png)

#### 3.1.3 To Add Reply url

1.   Open the AD application and from the Overview menu, click on Redirect URIs as shown below

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d110.png)
 
2. Add the admin app URL **<https://adminapp.cloudapp.azure.com>** and click Save.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d111.png)
 
#### 3.1.4 Add appRoles to Manifest File of Application

1. Click on **Manifest** of the created application and click on **Edit**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d112.png)
 
2. Paste the below content in the appRoles with Admin and Comsumer user object id under the allowedMemberTypes as shown below.

**Note**: 

1.	Make sure that the user must have Global Administrator permissions to get the object ID of user to give access.

2.	We need to add one user object id for Admin role and another user object id for Consumer role.


#### 3.1.5 To get the User ID from Directory

Go to **Azure Active Directory -> Users -> All users** 

Enter the user id to which you want to add in roles.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d11.png)
 
Select user then you will redirect to selected **user profile** and copy the **Object ID**. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d12.png)
 
{
"allowedMemberTypes": [
	
"User"
	
],
"displayName": "Admin",

"id": **"<xxxxxx-f3ef-xxxxx-axx5-xxxxxxxx>"**,
	
"isEnabled": true,

"description": "Administrators can manage the tenant",

"value": "Admin"

},

{

"allowedMemberTypes": [

"User"

],

"displayName": "Consumer",

"id": **"<xxxx4xxxxx-xxxx-xxxx-xxxx-cxxxxxxa0daxxx>"**,

"isEnabled": true,

"description": "Regular users of the app",

"value": "Consumer"

}

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d13.png)

Add the below URLS as well in the AD Application, which will be used by Mobile application.
**1st Url** : Edison Admin Portal Application
**2nd Url** : It should be your tenant domain name i.e., 
                **“com.onmicrosoft.<tenant domain name>//redirect”**
**3rd Url** : It’s a OAuth URL, so no change. 
                **“https://login.microsoftonline.com/tfp/oauth2/nativeclient”**
**4th Url**: Prefix the URL with the AD application name. 
                **“http://<AD App name>.azurewebsites.net”**
**5th Url**: Add the AD Application ID prefixed with msal.  **“msal<AD application ID>://auth”**
**6th Url**: Its also OAuth URL, so keep it without changing  
                **“https://login.microsoftonline.com/common/oauth2/nativeclient”**
**7th Url**: Postfix the AD application name to the url.
                **“com.onmicrosoft.<AD Application name>://redirect/”**

Update here

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d113.png)

3.	Click on **Save**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d14.png)

#### 3.1.6 To get Tenant ID

1.	Select **Azure Active Directory**.

2.  Scroll down left-hand side menu and select **Properties** for your Azure AD tenant and **Copy** the **Directory ID** (Tenant ID).

3. **Make a note  of** the Copied **Directory ID** which will be used while deploying the **ARM template**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d15.png)
 
#### 3.1.7 To Assign owner permissions to the Application

1.	Click on the **Owners**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d114.png)
 
2.	Click on **+Add**. Assign the owner permission to the member by entering the name in search tab and click on **Select**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d115.png)
 
3.	Then view the added member under Owners.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d116.png)
 
#### 3.1.8 Add User to the Application

1. From the Overview menu, click on Managed application in local Directory 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d117.png)

2.	Click on **Users and Groups -> Add user**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d118.png)
 
3.	Select **User**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d20.png)
 
4.	Select role as **Admin**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d21.png)
 
5.	Click **Assign**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d22.png)
 
6.	Now the added user is listed under Users and Groups.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d23.png)
 
### 3.2 Creating Session ID

Session ID is used to generate a unique id for creating a new job in Automation Account.

1.	Use the below URL to generate GUID.
      **https://www.guidgenerator.com/**
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d24.png)

2.	Click **Generate some GUIDs!** , this will generate GUID in Results box. 

3.	**Make a Note of** the generated GUID GUID which will be used while deploying the **ARM template**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d25.png)

### 3.3 To register a new application in Microsoft Bot Framework

1. Browse with below link to sign in to the Microsoft Bot Framework Application Registration portal.

    https://apps.dev.microsoft.com/?referrer=https://portal.azure.com/#/appList

2.	Sign in with you Microsoft account credentials.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d26.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d27.png)
 
3.	This will be redirected to a bot framework page, Click on **Add an app** to register new app in application registration portal.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d28.png)
 
4.	Enter the name of the app and then click **Create application**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d29.png)
 
5.	To view the app in Azure Portal, Click on **View this ap in the Azure portal** else click on **Not now**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d30.png)
 
6.	**Save** the **Application Id**, this application id is referred as bot client id which will be used while  deploying ARM template.

7.	Click on **Generate New Password**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d31.png)
 
8.	**Copy** the generated **key** as this is the only time it will displayed.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d32.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d33.png)

### 3.4 Azure B2C Tenant Creation and Configuration

Creating Azure AD B2C tenant is a one-time activity, if you have a B2C Tenant already created by your admin then you should be added into that tenant as Global Administrator to register your app to get the B2C tenant id, application id and sign-in/sign-up policies.

Follow Below steps to create Azure AD B2C Tenant:

1.	Create a new B2C tenant in **Azure Active Directory B2C**. You'll be shown a page with the information on Azure Active Directory B2C. Click Create at the bottom to start configuring your new Azure Active Directory B2C tenant.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d34.png)

2.	Choose the **Organization name, Initial Domain name** and **Country of Region** for your Tenant.

3.	Note down your entire **Tenant name** which you created. It will be used later.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d35.png) 

4.	 Once the B2C Tenant is created, Click **Directory and Subscription filter** on the top right to see the newly created tenant.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d36.png)

5.	Switch to created tenant by clicking on it. Type **Azure** in search column and select **Azure AD B2C**.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d37.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d38.png)

6.	You can see the created tenant overview like below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d39.png)
 
7.	Click on **sign-up or sign-in policies**. Then click on **Add** to add policy.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d40.png)

8.	Provide the **name** and enter the details as shown below.

9.	Make a note of the policy name that you are creating now, this will be used later.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d41.png)

10.	Select all the **Sign-up attributes** as show below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d42.png)
 
11.	Select all the **Application claims** as shown below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d43.png) 

12.	After filling all the required details, click on **Create**.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d44.png)
 
13.	Once the deployment is complete, the below screen will appear with sign-up details.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d45.png)

#### 3.4.1 Create an Application in Azure B2C 

1.	Click on the **Applications** tab and click **Add** to create a new application
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d46.png)

2.	Provide a name for the **application**.

3.	Under the **Web APP/Web API** tab, click **No**.

4.	Click **Yes** under the **Native client** to include the native client URL as shown below.

5.	Before clicking on Create, note down the website name and Redirect URI.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d47.png)

6.	Select the application you created and note down the **Application ID**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d48.png)

### 3.5 Create an Azure Service Principal with Azure CLI

1.	Open Power shell with **Run as administrator**.

Login to your azure account using below command.

 **Command: az login**
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d49.png)

2. Set your subscription with below command

**Command: az account set --subscription <subscription ID>**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d50.png)
 
3.	Create without a default role assignment.

**Command: az ad sp create-for-rbac -n <appname> --skip-assignment**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d51.png)
 
4.	To retrieve the object ID of the created application.

**Command: az ad sp show --id <created app id> --query "objectId"**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d52.png)

### 3.6 Create a Twilio Account 

Browse to the below link and create a Twilio Account 
    https://www.twilio.com
	
After the Account is created, navigate to settings (icon) and note down the Auth Token.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d53.png)

## 4.0 ARM Template Input Parameters

In the parameters section of the template, specify the values as inputs when deploying the ARM Template. These parameter values enable you to customize the deployment by providing values that are tailored for your environment.

|   **Parameter name**    |    **Description**                               |    **Allowed Values**    |     **Default Values**   |      
|-------------------------|--------------------------------------------------|------------------------- |--------------------------- |   
| **solution type**       | The type of Solution                 |                 | Project-Edison                  |
| **DeploymentType**      | choose your deploymenttype to deploy     | Basic or Standard or Premium |              |
| **geo-paired-region**       | Select the geopaired region as deployment region, for more information about the geo-paired-region navigate to section 4 in the Getting Started Document(https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/GettingStarted.md)  | EastUS2, CentralUS, WestUS2, WestCentralUS, CanadaCentral, CanadaEast, AustraliaEast, AustraliaSout,hEast, CentralIndia, SouthIndia, EastAsia, SouthEastAsia, JapanWest, JapanEast, KoreaCentral, KoreaSouth, UKSouth, UKWest             |            |
| **signalRlocation**            | Choose location for signalR to deploy         | AustraliaEast, CanadaEast, CentralUS, EastUS2, EastUS, JapanEast, NorthEurope, SouthCentralUS, SoutheastAsia, UKSouth, WestEurope, WestUS2, WestUS           | EastUS2         |   
| **signalRlocationDr**          | If deploymenttype is choosen as premium, enter location for signalRlocationDr for disaster recovery.              | AustraliaEast, CanadaEast, CentralUS, EastUS2, EastUS, JapanEast,NorthEurope, SouthCentralUS, SoutheastAsia, UKSouth, WestEurope, WestUS2, WestUS              | CentralUS      |                            
| **acrDeploymentLocation** | Choose location for Azure container Registry to deploy.       | JapanEast, JapanWest, AustraliaSouthEast, AustraliaEast, CanadaCentral, CanadaEast, CentralIndia, SouthIndia, CentralUS, EastUS2, SouthEastAsia, EastAsia, WestCentralUS, WestUS2       |               |                                                                                     
|**omsWorkspaceRegion**    | Choose location for OMS Log Analytics to deploy. For more information, see https://docs.microsoft.com/en-us/azure/log-analytics/log-analytics-overview     | Australiasoutheast,  Canadacentral, Centralindia, Eastus, Japaneast, Southeastasia, Uksouth, westeurope           | southeastasia     |  
**omsWorkspaceRegionDr**    | If deploymenttype is choosen as premium, enter location for omsWorkspaceRegionDr for disaster recovery.    | Australiasoutheast,  Canadacentral, Centralindia, Eastus, Japaneast, Southeastasia, Uksouth, westeurope           | southeastasia     |      
| **appInsightsLocation** | Location for the instance of App Insights     | Southcentralus, westeurope, eastus, NorthEurope      | eastus       |                                      
|   **appInsightsLocationDr**        | If deployment type is choosen as premium, enter location for application insights for disaster recovery. In case of Standard deployment enter the value in redeploy template deployment.   | Southcentralus, Westeurope, Eastus, NorthEurope      |   southcentralus   |
|   **Tenant Id**   | TenantId of the created Azure active directory application. For instructions, see  (https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-howto-tenant) in the Microsoft documentation  |          |            |                      
| **botAdClientId** | Application ID deployed for Bot, used for Bot deployment. Use https://apps.dev.microsoft.com to deploy application |        |            |                              
|   **adObject Id**       | Object Id of the deployed Application in Azure Active Directry. Get using Get-AzureRmADUser or Get-AzureRmADServicePrincipal cmdlets     |           |             |
|**adClient Secret** |  Client Secret of the created Azure active directory application.For instructions, see (https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal#get-application-id-and-authentication-key)  in the Microsoft documentation.                   |         |                  |
| **azureAccount Name**    | Azure Portal Login UserName                                   |                |                        |
|**azurePassword**| Azure Portal Login Password        |         |               |                                    
| **adminName**        | Name of Admin Portal used as hosts while executing the Ingress_Custom Script and will be updated in reply URL of Active Directory application     |       |        |
|  **sessionId** | Provide the guid prefix for the runbook job to be created. Generate using https://www.guidgenerator.com/online-guid-generator.aspx .Refer section 3.2  |      |          |                                  
| **vmUsername**| Username to login into Virtual Machine     |      |        |                                 
|   **vmPassword**    |  Password to login into Virtual Machine |    |       |
|  **aksServicePrincipalClientId** |  Client ID of the service principal. Refer section 3.5 to get ad Service principal client id     |      |         |
| **aksServicePrincipalClientSecret**      | ClientSecret of the service principal. Refer section 3.5. to get ad Service principal client secret      |        |         |
|   **aksServicePrincipalClientIdDr**        | If deploymenttype is choosen as premium, enter Client ID of the newly created service principal else leave it blank.In case of Standard deployment enter the value in redeploy template deployment.       |       |     |
|  **aksServicePrincipalClientSecretDr**           |  If deploymenttype is choosen as premium, enter Client secret of the newly created service principal else leave it blank.In case of Standard deployment enter the value in redeploy template deployment.     |      |          |
|  **signalrCapacity**      | The number of SignalR Unit.      | 1, 2, 5, 10, 20, 50, 100       | 1         |
|  **dockerVM**      |  Choose Yes/No to add Docker VM as part of Solution deployment                         | Yes, No             |           |
|  **githuburl**        |  GitHub url used to clone the repository to build the images in docker VM                      |             |      |
|  **azureAdPreviewModuleUri**     |  Provide the url to install azure active directory preview module in azure automation from github    |        | https://github.com/sysgain/Iot-ProjectEdison/raw/dev/code/AzureADPreview.zip     |
|  **cosmosdbModuleUri**  |  Provide the url to install azure active directory preview module in azure automation from the github         |     | https://github.com/sysgain/Iot-ProjectEdison/raw/dev/code/cosmos.zip       |
|  **siteName**     | The name of Bot Direct line channel site         |                | edisonwebapp     |
|  **trafficManager**     | If the solution type is Basic choose No option for trafficmanager parameter .If the solution type is Standard or Premium choose Yes/No base don your requirement.         |                |       |


## 5.0 Getting Started

Azure Resource Manager allows you to provision your applications using a declarative template. In a single template, you can deploy multiple services along with their dependencies. The template consists of JSON and expressions that you can use to construct values for your deployment. You use the same template to repeatedly deploy your application during every stage of the application lifecycle.

Resource Manager provides a consistent management layer to perform tasks through Azure PowerShell, Azure CLI, Azure portal, REST API, and client SDKs.

Resource manager provides the following feature:

*	Deploy, manage, and monitor all the resources for your solution as a group, rather than handling these resources individually.

*	Repeatedly deploy your solution through the development lifecycle and your resources are deployed in a consistent state.

*	Manage your infrastructure through declarative templates rather than scripts.

*	Define the dependencies between resources so they're deployed in the correct order.

*	Apply access control to all services in your resource group because Role-Based Access Control (RBAC) is natively integrated into the management platform.

*	Apply tags to resources to logically organize all the resources in your subscription.

### 5.1 ARM Template Deployment Using Azure Portal

1.	Click the below **Git hub** repo URL.

    https://github.com/sysgain/Iot-ProjectEdison/tree/master 

2. Select **main-template.json** from **Master** branch as shown in the following figure.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d119.png)
 
3.	Select **Raw** from the top right corner.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d55.png)
 
4.	**Copy** the raw template and **paste** in your Azure portal for template deployment.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d56.png)
  
**To deploy a template for Azure Resource Manager, follow the below steps.**

5.	Go to **Azure portal**.

6.	Navigate to **Create a resource (+)**, search for **Template deployment**.

7.	Click **Create** button and click **Build your own Template in the editor** as shown in the following figure.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d57.png)
 
8.	The **Edit template** page is displayed as shown in the following figure. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d58.png)
 
9.	**Replace / paste** the template and click Save button.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d59.png)
  
10.	The **Custom deployment** page is displayed as shown in the following figure.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d60.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d61.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d62.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d63.png)

#### 5.1.1 Inputs

These parameter values enable you to customize the deployment by providing values. There parameters allow to choose the solution type, region and AD Application details.  

**Parameters for Basic Solution**: 

For **Basic Solution** deployment we need to provide the values for below parameters. 

Select the geo-paired region for template deployment and choose the values of **OMS Workspace Region, App Insights Location** or keep the default values as it is. It is not necessary to choose **High availability region of App Insights Location Dr**.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d64.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d65.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d66.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d67.png)

**Parameters for Standard Solution:** 

For deploying the **standard solution** we need to provide the below parameters. 

Select the geo-paired region for your template deployment and choose the values of **OMS Workspace Region, App Insights Location** and **High availability region of App Insights Location Dr** or keep the default values as it is.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d68.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d69.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d70.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d71.png)
 
**Parameters for Premium solution:**

For **Premium solution** deployment, select the geo-paired region of template deployment and choose the values of **OMS Workspace Region, App Insights Location** and **High availability region of App Insights Location Dr** or keep the default values as it is.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d72.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d73.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d74.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d75.png)
 
 
11.	Once all the parameters are entered, check in the **terms and conditions** check box and click **Purchase**.

12.	After the successful deployment of the ARM template, the following **resources** are created in a **Resource Group**.

*	1 Application Insights

*	1 Automation Account

*	1 Azure Cosmos DB Account

*	1 Bot Channels Registration

*	1 Container Registry

*	1 IoT Hub

*	1 Kubernetes service

*	1 Redis Cache

*	1 Run Book

*	1 Service Bus Namespace

*	1 SignalR

*	1 Linux VM

*	1 log analytics

*	1 Notification Hub

*	1 Storage Account

*   1 Windows VM

The above resources are deployed for Basic Solution.

Expect IoT Hub, Cosmos DB, OMS Log Analytics, Automation Account, Run Book and Storage Account the rest of the resources are created additionally along with Traffic manager as disaster recovery for Standard and Premium Solution deployment.

13.	Once the solution is deployed successfully, navigate to the resource group to view the list of resources that are created as shown below.
   
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d76.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d77.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d78.png)

#### 5.1.2 Output

1.	Go to **Resource Group -> Click Deployments> Click on Microsoft.Template**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d79.png)

2.	Click on Outputs to view the output values.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d80.png)


### 5.2 ARM Template Deployment Using Azure CLI 

Azure CLI is used to deploy your resources to Azure. The Resource Manager template you deploy, can either be a local file on your machine, or an external file that is in a repository like GitHub.   

Azure Cloud Shell is an interactive, browser-accessible shell for managing Azure resources. Cloud Shell enables access to a browser-based command-line experience built with Azure management tasks in mind.  

Deployment can proceed within the Azure Portal via Windows PowerShell.  

1.	Clone the Master branch and save it locally, refer **section 5.1** of this document for Git hub URL

2.	Open **Windows PowerShell** and run the following command

     **az login**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d81.png)

3.	It will redirect to the login page, provide Azure Portal credentials. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d82.png)
 
4.	Enter user name then click **Next**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d83.png)
 
5.	Enter Password and click **Sign in**.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d84.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d85.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d86.png)
 
 
6.	Download the **main-template.parameters.json** in your local system from the below  URL, paste it in  an editor , update the values and save it.

https://github.com/sysgain/Iot-ProjectEdison/blob/master/main-template.parameters.json

7.	Update the following parameters in **main-template.parameters.json file.**

*	Solution Type

*	deploymenttype

*	geo-paired-region

*	signalRlocation

*	signalRlocationDr

*	acrDeploymentLocation

*	omsWorkspaceRegion 

*	appInsightsLocation 

*	appInsightsLocationDr

*	Tenant Id 

*	botAdClientId

*	adObject Id 

*	adClient Secret 

*	azureAccount Name 

*	azurePassword 

*	adminName

*	sessionId 

*	vmUsername

*	vmPassword

*	aksServicePrincipalClientId

*	aksServicePrincipalClientSecret

*	aksServicePrincipalClientIdDr

*	aksServicePrincipalClientSecretDr

*	signalrCapacity

*	dockerVM

*	githuburl

*	azureAdPreviewModuleUri

*	cosmosdbModuleUri

*	siteName

#### 5.2.1 Create Resource Group using Azure CLI

Use the **az group create** command to create a **Resource Group** in your region.

**Description**: To create a resource group, use **az group create** command, 

It uses the name parameter to specify the name for resource group (-n) and location parameter to specify the location (-l).  

**Syntax:   az group create -n <resource group name> -l <location>** 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d87.png)

#### 5.2.2 Execute the Template Deployment  

Use the **az group deployment create** command to deploy the ARM template  

**Description:** To deploy the ARM Template, you require two files:  

**main-template.json** – contains the resource & its dependency resources to be provisioned from the ARM template  

**main-template.parameters.json** –contains the input values that are required to provision respective SKU & Others details, for more details on the input parameter values navigate to Section 3.2 (6th point) of this document.  

**az group deployment create --template-file './<main-template.json filename>' --parameters '@./<main-template.parameters.json filename>' -g < provide resource group name> -n deploy >> <provide the outputs filename>**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d88.png)

After successful deployment you can see the deployment outputs as follows. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d89.png)

## 6.0 Post Deployment

### 6.1 Adding messaging endpoint in Bot

Go to resource group -> click on **Bot Channel Registration -> Settings -> add the api url** as the messaging endpoint under configuration.

apiURL-  https://<api url>/chat/messages

Save the API URL , will be used to update the values in environment file.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d90.png)
 
### 6.2 Enable Enhance authentication options.

1.	Navigate to **Resource Group >** click on **Bot Channel Registration > Channels>** Click on **Edit** in Azure portal.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d91.png)
 
2.	Scroll up the Page, Click on **Show** to copy and note the **secret keys**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d92.png)
 
3.	Scroll down and **Enable** the Enhanced authentication options.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d93.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d94.png)

4.	Click on **Add a trusted origin** and enter the **API URL** saved from step **5.1.** Click on **Done**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d95.png)
 
### 6.3 Login to Edison Virtual Machine

1.	Navigate to deployed Virtual Machine and click on it.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d96.png)
 
2.	Copy the **public IP Address**
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d97.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d98.png)

3.	Enter the credentials to log in to VM.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d99.png)
 
4.	Switch to root user using below command.

    **sudo -i**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d100.png)

5.	change the directory to below path and list the files with the following command

    **cd /var/lib/waagent/custom-script/download/0**
    **ls**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d120.png)

6.	Execute deploy1.sh script by passing the git uri as an argument

    **./deploy1.sh https://github.com/litebulb/ProjectEdison.git**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d121.png)

7.	One pop up will be opened, click on **yes** to Restart services during package upgrades without asking.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d122.png)


8.	Check the docker version using **docker -v** command.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d101.png)
 
9.	Change the directory to view the downloaded scripts.

    **Command: cd /var/lib/waagent/custom-script/download/0**

    **Command: ls**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d102.png)
 
10.	Open the **input.txt** file and Update the values to the corresponding key from the Azure Portal and Follow 4.1.2 section to see few values of Keys.

11. Below is the link table containing the keys and references of the value to be taken from.

    **https://github.com/sysgain/Iot-ProjectEdison/raw/master/code/input_values.docx**
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d103.png)

### 6.4 Setting up the Environment

#### 6.4.1 Execute configupdate2.sh

**Configupdate2.sh** script: To update all config files. Use the below command to execute the script.

**sh configupdate2.sh**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d104.png)
 
#### 6.4.2 Execute commonupdate3.sh 

**Commonupdae3.sh** script: To update the values in the common.secrets file. Use the below command to execute the script.

**sh commonupdate3.sh**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d105.png)
 
#### 6.4.3 Execute edsionwebenvupdate4.sh

**edsionwebenvupdate4.sh script**: To update the values in the environment files. Use the below command to execute the script.

**sh edsionwebenvupdate4.sh**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d106.png)
 
#### 6.4.4 Execute updateappsettings5.sh 

**updateappsettings5.sh** script: To update the values in the appsettings.json file of all the microservices. Use the below command to execute the script.

**sh updateappsettings5.sh**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/d107.png)
