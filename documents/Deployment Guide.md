# Safe Buildings Solution

## Admin Guide

### Contents
 
 - [1.0 Introduction](#10-introduction)
 - [2.0 Accessing the Edison Admin Portal Application](#20-accessing-the-edison-admin-portal-application)
 - [3.0 Security](#30-security)
      - [3.1 Storage Security](#31-storage-security)
        - [3.1.1 Secure Transfer Required](#311-secure-transfer-required)
        - [3.1.2 Advanced Threat Protection](#312-advanced-threat-protection)
        - [3.1.3 Shared Access Signature](#313-shared-access-signature)
        - [3.1.4 Encryption](#314-encryption)
      - [3.2 Cosmos DB Security](#32-cosmos-db-security)
 - [4.0 Monitoring Component](#40-monitoring-component)
    - [4.1 Application Insights](#41-application-insights)
    - [4.2 OMS Log Analytics](#42-oms-log-analytics)
 - [5.0 Standard Solution Type](#50-standard-solution-type)
    - [5.1 Primary region Configuration](#51-primary-region-configuration)
    - [5.2 Kubernates HA](#52-kubernates-ha)
      - [5.2.1 Re-Deploy the Region-2 ARM Temple](#521-re-deploy-the-region-2-arm-temple)
      - [5.2.2 Add Messaging endpoint in Bot](#522-add-messaging-endpoint-in-bot)
      - [5.2.3 Enable Enhanced authentication option](#523-enable-enhanced-authentication-option)
      - [5.2.4 Login to the Edison Dr Virtual Machine](#524-login-to-the-edison-dr-virtual-machine)
      - [5.2.5 Execute deploy1.sh](#525-execute-deploy1.sh)
      - [5.2.6 Execute configupdate2.sh](#526-execute-configupdate2.sh)
      - [5.2.7 Execute commonupdate3.sh](#527-execute-commonupdate3.sh)
      - [5.2.8 Execute edsionwebenvupdate4.sh](#528-execute-edsionwebenvupdate4.sh)
      - [5.2.9 Execute updateappsettings5.sh](#529-execute-updateappsettings5.sh)
      - [5.2.10 Execute imageupdate6.sh](#5210-execute-imageupdate6.sh)
      - [5.2.11 Execute clusterconnect7.sh](#5211-execute-clusterconnect7.sh)
      - [5.2.12 Execute set-kubernetes-config8.sh](#5212-execute-set-kubernetes-config8.sh)
      - [5.2.13 Execute updateyaml9.sh](#5213-execute-updateyaml9.sh)
      - [5.2.14 Execute ingress_custom10.sh](#5214-execute-ingress_custom10.sh)
      - [5.2.15 Update nginx-config-adminportal.yaml and nginx-config-api.yaml](#5215-update-nginx-config-adminportal.yaml-and-nginx-config-api.yaml)
      - [5.2.16 Manual configuration](#5216-manual-configuration)
     - [5.3 IoThub Failover](#53-iothub-failover)
     - [5.4 Cosmos DB Failover](#54-cosmos-db-failover)
 - [6.0 Premium Solution Type](#60-premium-solution-type)
     - [6.1 Primary Region Configuration](#61-primary-region-configuration)
     - [6.2 Performing DR Strategies](#62-performing-dr-strategies)
     - [6.3 Dr Region Configuration](#63-dr-region-configuration)
     
     

## 1.0 Introduction

This document demonstrates how to perform administration activities for the Project Edison solution. In addition to the user document, this document explains the process for verifying data in the resources, updating SKUs, enabling security steps for the resources and performing DR activities for Standard and Premium solutions.

**Note**: This document is subjected to an assumption, that the solution is already deployed through ARM template, using the **Deployment Guide**.

## 2.0 Accessing the Edison Admin Portal Application
Refer to the **User Guide Document** for instructions on accessing the Edison admin portal application and the User Guide Document to onboard devices, generate activated responses, configuring responses primary and secondary range, download the history of the responses in a selected time span and receiving emergency messages, from Project Edison Solution.
The Project Edison Solution is enabling rapid response to nearby community events that might increase the danger level of persons in the nearby vicinity.

## 3.0 Security

### 3.1 Storage Security

#### 3.1.1 Secure Transfer Required

The **Secure transfer required** option enhances the security of your storage account by only allowing requests to the account from secure connections. For example, when you're calling REST APIs to access your storage account, the request must be made on HTTPS. "Secure transfer required" rejects requests that use HTTP. 

Secure transfer can be enabled by settings under configuration of the Storage account overview page.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a1.png)
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a2.png)
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a3.png)

#### 3.1.2 Advanced Threat Protection

Azure Storage Advanced Threat Protection detects anomalies in account activity and notifies you of any potential harmful attempts to access your account. This layer of protection allows you to address threats without the need of a security expert or manage security monitoring systems. 

On the storage account overview, click on advanced treat protection and click on **Enable Advanced Threat protection** to enable the feature.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a4.png)
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a5.png)
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a6.png)

#### 3.1.3 Shared Access Signature

A shared access signature token provides delegated access to resources in your storage account. With a SAS token, you can grant clients access to resources in your storage account, without sharing your account keys. This is the key point of using shared access signatures in your applications--a SAS token is a secure way to share your storage resources without compromising your account keys.
 
 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a7.png)
 
 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a8.png)

#### 3.1.4 Encryption

One way that the Azure storage platform protects your data is via Storage Service Encryption (SSE), which encrypts your data when writing it to storage, and decrypts your data when retrieving it. The encryption and decryption are automatic, transparent, and uses 256-bit AES encryption, one of the strongest block ciphers available. 

On the Settings blade for the storage account, click Encryption. Select the Use your own key option. You should already create a key in the key vault.

You can specify your key either as a URI, or by selecting the key from a key vault. 
 
 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a9.png)
 
 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a10.png)
 
*	Choose **Select from Key Vault** option.

*	Select the key vault containing the key you want to use. 

*	Select the key from the key vault. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a11.png)
 
Select specific key vault which is deployed through Edison Arm template.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a12.png)
 
Click on **Select** under the Encryption key section.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a13.png)
 
Choose existing key or click on **+Create** a new key, assign key name and click on Create.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a14.png)
 
Proceed to click on **Save**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a15.png)
 
If you want to check the key vault key go to the key vault resource and click on **keys**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a16.png)
 
### 3.2 Cosmos DB Security

As a PaaS service, Cosmos DB is very easy to use. Because all user data stored in Cosmos DB is encrypted at rest and in transport, you don't have to take any action. Another way to put this is that encryption at rest is "on" by default.

## 4.0 Monitoring Component

### 4.1 Application Insights

1. Go to Resource group and click on **application insights**. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a17.png)
 
**Performance**

2.	On **Overview page**, Summary details are displayed as shown in the following screenshot. 

3.	Click **Performance** on the left side of the page as shown below. 
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a18.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a19.png)
 
4.	Click on View in logs from **Dropdown list** select **Response time** in the following screenshot.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a20.png)
 
5.	After click on that **Response time**, it will open a new tab with some default queries & chart for the same.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a21.png)
 
6.	Back to performance then Click on **Request** from view in logs drop downlist, it will open a new tab with request and response operations.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a22.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a23.png) 

7.	Click **Chart** icon it should be display default queries & chart. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a24.png) 
 
8.	Back to the Application insights overview, Click on **Failures** on the left side of the page as shown below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a25.png) 
 
9.	Click on **View in Analytics** and select any of the analytics. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a26.png) 
 
10.	After click on Request Count, it will open a new tab with some default queries & chart
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a27.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a28.png) 
 
**Metric preview** 

11.	Then select **metric explorer** from the left menu. 

12.	Here you need to select the resource from the drop-down list, select the metric what you want to give and select the aggregation as per requirement. 

13.	Here we can see the graph after specifying our need. 
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a29.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a30.png) 
 
**Application Map** 

14.	Application Map helps you spot performance bottlenecks or failure hotspots across all components distributed application.

15.	After click on **application map** you can see the screen like below. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a31.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a32.png) 

16.	When you click on **edisonapi** it will open popup window in right side and click on **Investigate Performance** button.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a33.png) 
 
17.	To check the **logs**, click on the chart of which you want to see the logs then you will get the logs of each request as shown in below figure.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a34.png) 
 
18.	Similarly you can check all the services logs which are connected to the application map. 

**Live Metrics Stream**

Click on **Live Metric Stream** to view the incoming requests, outgoing requests, overall health and servers of applications.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a35.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a36.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a37.png) 

### 4.2 OMS Log Analytics 

1.	Open **Azure Portal -> Resource Group ->** Click the **OMS Workspace** in resource group to view OMS Overview Section.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a38.png) 

2.	Click **Azure Resources** on left side menu to view available Azure Resources. 

3.	Select your **Subscription** and **Resource Group** name from the dropdown list. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a39.png) 
 
4.	Click on **Logs** in the left side menu it will open log **search box**.

5.	Copy **Resource group name** and paste it in the **search box** and we write the **Kusto query** language and click **Run** button, it should show the Resource group Telemetry data.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a40.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a41.png)  
  
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a42.png) 

6.	Here you can check the **resource group** information is displayed below the page as shown in the following figure. 
  
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a43.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a44.png) 
 
7.	Copy **IoT Hub** name and paste it in the search box with **Kusto query language** and click **Run**.

8.	Here you can see the **IoT hub resource Telemetry information** is displayed below the page as show in the following figure

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a45.png)  

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a46.png) 

9.	Here you can see the **IoT hub resource information** is displayed below the page as show in the following figure.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a47.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a48.png) 

10.	Copy **Cosmos DB resource group** name and paste it in the search box with Kusto query language and click **Run**. 

11.	Here you can see the **Cosmos DB resource Telemetry information** is displayed below the page as show in the following figure.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a49.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a50.png) 

## 5.0 Standard Solution Type

### 5.1 Primary region Configuration 

Refer the Section 6.4 in Deployment guide and refer section 2 in User Guide for configuring the primary region configuration.

After the primary region configuration, you will get the events and devices in primary adminportal like below.

**Note**: We need to add the **Admin** and **API URL** in the domain registry along with the external IP address for both, before accessing the Edison Admin Portal.   

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a51.png)
 
### 5.2 Kubernates HA

#### 5.2.1 Re-Deploy the Region-2 ARM Temple

We are using redeploy.json template to route the traffic to another Kubernetes cluster.

**To configure the secondary cluster, Deploy standard solution and deploy redeploy.json in the same resource group.**

1.	Go to Project Edison git hub master branch redeploy.json click on the raw.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a52.png)

2.	click on CTL+A and copy the code.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a53.png)

3.	Go to Azure portal click on **Create New +** search for template deployment and click on it.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a54.png) 

4.	click on Create

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a55.png)
 
5.	click on Build your own template in the editor

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a56.png)
 
6.	delete the following code and Paste the copied redeploy.json code 
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a57.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a58.png)

7.	click on Save. then select same subscription ,same resource group, create another clientid and client secret and enter in the Aks service principal clientid Dr, client secret Dr, clone repository url then tick the checkbox and click on **purchage**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a59.png) 
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a60.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a61.png)

8.	It will take few minutes to complete the deployment then you can see the few resources are deployed with Dr prefix in other region.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a62.png) 
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a63.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a64.png)

#### 5.2.2 Add Messaging endpoint in Bot

Go to resource group -> click on **Bot Channel Registration -> Settings ->** add the api url as the messaging endpoint under configuration.

API secondary URL-  https://<secondary url>/chat/messages
 
Save the API secondary URL , will be used to update the values in environment file.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a65.png)
 
#### 5.2.3 Enable Enhanced authentication option

Navigate to **Resource Group > click on Bot Channel Registration > Channels > Click on Edit in Azure portal**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a66.png)

1.	Scroll up the Page, Click on **Show** to copy and note the secret keys.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a67.png)
 
2.	Scroll down and **Enable** the Enhanced authentication options.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a68.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a69.png)

3.	Click on **Add a trusted origin** and enter the seconday **API URL** saved from step **5.1.** Click on **Done**.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a70.png)

#### 5.2.4 Login to the Edison Dr Virtual Machine.

1.	Navigate to deployed Dr Virtual Machine and click on it.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a71.png)
 
2.	Copy the public IP Address

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a72.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a73.png)
 
3.	Enter the credentials to log in to VM.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a74.png)

4.	Switch to root user using below command.

***sudo -i***

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a75.png)
 
5.	Change the directory to view the downloaded scripts.

***Cd /var/lib/waagent/custom-script/download/0***

***ls***

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a76.png)
 
6.	Open the **input.txt** file and Update the values as mentioned in the Deployment Guide. For below the values, Use secondary region resources only.

ACRvalues,signallrvalue, Notificationhubpath, NotificationHubSRT, AzureServiceBusConn, ApplicationInsightKey, Clustername

7.	But update the **BASEURL_VALUE** with API secondary DNS name Uri and update **ADMINURL** with secondary DNS name.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a77.png)

#### 5.2.5 Execute deploy1.sh

Go to below path in the Virtual Machine

***cd  /var/lib/waagent/custom-script/download/0***

deploy1.sh script: To install the required packages Use the below command to execute the script 

***sh deploy1.sh**

#### 5.2.6 Execute configupdate2.sh

Go to below path in the Virtual Machine

***cd  /var/lib/waagent/custom-script/download/0***


**Configupdate2.sh** script: To update all config files. Use the below command to execute the script.

***sh configupdate2.sh***

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a78.png)
 
#### 5.2.7 Execute commonupdate3.sh 

**Commonupdae3.sh** script: To update the values in the common.secrets file. Use the below command to execute the script.

***sh commonupdate3.sh***

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a79.png)
 
#### 5.2.8 Execute edsionwebenvupdate4.sh

**edsionwebenvupdate4.sh** script: To update the values in the environment files. Use the below command to execute the script.

***sh edsionwebenvupdate4.sh***

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a80.png)
 
#### 5.2.9 Execute updateappsettings5.sh 

**updateappsettings5.sh** script: To update the values in the appsettings.json file of all the microservices. Use the below command to execute the script.

***sh updateappsettings5.sh***

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a81.png)
 
#### 5.2.10 Execute imageupdate6.sh

**Imagesupdate6.sh** script: To build the images and push them to ACR container repositories.
 
Navigate to **Azure portal > Container Registry > Repositories** and check for the images which have been pushed.

**Note:** If you got any issues to push the images to ACR  just rerun the deploy1.sh script and again run the imageupdate6.sh script.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a82.png)

#### 5.2.11 Execute clusterconnect7.sh

**clusterconnect7.sh** script: Connect to the Dr cluster

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a83.png)
 
#### 5.2.12 Execute set-kubernetes-config8.sh

**set-kubernetes-config8.sh** script: Creates config maps.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a84.png)
 
Check the config maps using the below command

**kubectl get cm**
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a85.png)

#### 5.2.13 Execute updateyaml9.sh

**Updateyaml9.sh** script: Creates Pods and Services
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a86.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a87.png)
 
#### 5.2.14 Execute ingress_custom10.sh

**ingress_custom10.sh** script:  Installs helm

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a88.png) 

1.	Using **WINSCP** push your certificates to the virtual machine.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a89.png)
 
Switch user to **adminuser**

Install unzip command using the below command

***sudo apt install unzip***

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a90.png)
 
2.	Unzip the certificate using the below command

**unzip Kubernetes_certs.zip**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a91.png)
 
3.	Copy the file to another file.

**cat 2289f3206db82816.crt gd_bundle-g2-g1.crt > xxxxxxxx-xxx.com.chained.crt**

4.	switch to **root** user and go to **/var/lib/waagent/custom-script/download/0**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a92.png)

2.	Create secrets using the below commands: 

**Command: kubectl create secret tls <adminsecret2 name> --cert /home/adminuser/<xxxxxxxx>.com.<xxxxx.crt> --key /home/adminuser/<xxxxxx.key>** 
 
**Command: kubectl create secret tls <apisecret2 name> --cert /home/adminuser/<xxxxxxxx>.com.<xxxxx.crt> --key /home/adminuser/<xxxxxxx.key>** 

**Command: kubectl get secrets** 
  
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a93.png)

#### 5.2.15 Update nginx-config-adminportal.yaml and nginx-config-api.yaml

Update the names of **hosts** and **Secrets** in **nginix config** files.

a.	**sed -i -e 's/edisonadminportal.eastus.cloudapp.azure.com/'<Dr admin URL>'/g'** **ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-adminportal.yaml**

**Ex:**

**sed -i -e 's/edisonadminportal.eastus.cloudapp.azure.com/'<edisonadmin2.xxxxx-xxx.com>'/g'** **ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-adminportal.yaml**
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a94.png)

b.	**sed -i -e 's/tls-secret-adminportal/'<adminsecret2 name>'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-adminportal.yaml**
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a95.png)

c.	**sed -i -e 's/edisonapi.eastus.cloudapp.azure.com/'<Dr api URL>'/g'** **ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-api.yaml**

**Ex:** 

sed -i -e 's/edisonapi.eastus.cloudapp.azure.com/'<edisonapi2.xxxxx-xxx.com>'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-api.yaml
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a96.png)

d.	**sed -i -e 's/tls-secret-api/'<apisecret2>'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-api.yaml**
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a97.png)
 
7.	Assign a static-IP to an Ingress on through the Nginx controller

a.	**On Admin:**

**az network public-ip create -g <Cluster Resource Group Name>-n <name of admin ip2> --dns-name <admin dns name2> --allocation-method static**

**Ex:**

az network public-ip create -g **MC_MO_basic_2304_akswih6_eastus2** -n **adminbotip2** --dns-name **dnsbotadmin2** --allocation-method static

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a98.png)
 
Copy the admin static IP and save it.

b.	**On API:**

**az network public-ip create -g <Cluster Resource Group Name>-n <name of api ip2> --dns-name <api dns name2> --allocation-method static**

**Ex:**

**az network public-ip create -g MC_MO_basic_2304_akswih6_eastus2 -n apibotip2 --dns-name dnsbotapi2 --allocation-method static**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a99.png)
 
Copy the api static IP and save it.

8.	Install the NGINX Ingress controller into the system namespace using the existing static IP address and as AKS is not RBAC enabled, the command needs to set RBAC related values to false. 

**On Admin:**

**helm install --name nginx-ingress-admin stable/nginx-ingress --namespace kube-system --set rbac.create=false --set rbac.createRole=false --set rbac.createClusterRole=false --set controller.ingressClass=nginx-admin --set controller.service.loadBalancerIP=”<admin Static IP2 address>”**
  
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a100.png)

**On API:**

**helm install --name nginx-ingress-api stable/nginx-ingress --namespace kube-system --set rbac.create=false --set rbac.createRole=false --set rbac.createClusterRole=false --set controller.ingressClass=nginx-api --set controller.service.loadBalancerIP="<apiStatic IP2 address>”**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a101.png)

9.	For creating config file navigate to Ingress_Custom folder.

**cd ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom**

Create admin config file by running the below command

**kubectl create -f ./nginx-config-adminportal.yaml**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a102.png)
 
Create API config file by running the below command

**kubectl create -f ./nginx-config-api.yaml**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a103.png)
 
To get ingress give the below command

**kubectl get ing**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a104.png)
 
Check status of services using the namespace kube-system

**kubectl get svc -n kube-system**
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a105.png)

#### 5.2.16 Manual configuration

1. Copy the API URL from Hosts.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a106.png)
 
3.	Update in the messaging endpoint with Dr api url of Bot Channel Registration and click on Save.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a107.png)
 
4.	Update the reply URL of the azure active directory application with Dr admin URL from Hosts and Click on Save.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a108.png)

### 5.3 IoThub Failover

1.	Go to the IoThub in the deployed resource group.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a109.png)

2.	In IoThub overview go down click on IoT devices and check if there are devices or not.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a110.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a111.png)

3.	In IoThub overview click on manual failover option then click on **Initiate failover**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a112.png)
 
4.	It will ask for the confirm manual failover in that enter iothub name and click on **Ok**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a113.png)
 
5.	It will take few minutes to complete the failover operation.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a114.png)

6.	After completion of IoThub failover you can see the notification like below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a115.png)
 
7.	Now if you observed that the primary location changed to secondary location and the secondary changed to primary location.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a116.png) 

### 5.4 Cosmos DB Failover

By doing the cosmos DB failover the read region becomes the write region and the write region becomes the read region.

1.	Go to the Cosmos DB resource in the deployed resource group.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a117.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a118.png)
 
2.	In cosmos DB over view click on Replicate data globally option. Now you can see the write regions and read regions.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a119.png)

3.	click on manual failover.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a120.png)
 
4.	then it will show the pop up like below select the read region and tick the check box click on **OK**.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a121.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a122.png)

5.	It will take few minutes to complete the manual failover operation.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a123.png)
 
6.	After completion of manual failover you can get the notification with green mark like below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a124.png)
 
7.	If you observed after manual failover the write region becomes the read region and the read region becomes as write region.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a125.png)

Refer the **4 section** of the user guide to run the simulator.

**Note:** We need to add the **Admin** and **API** URL in the domain registry along with the external IP address for both, before accessing the Edison Admin Portal.   

To view the devices which got created or onboarded, Access the Edison Admin Portal and click on the wi-fi icon as shown below. Here you can also see the location, type, current response of the devices.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a126.png)

initially there are no events triggered. Once any event gets triggered from devices or simulator it will get reflected in the Edison admin portal as shown below.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a127.png)

## 6.0 Premium Solution Type

To perform HA when the primary region fails, we need to use secondary region (DR) components from the same resource group where all DR components were deployed as part of the Premium solution

### 6.1 Primary Region Configuration

 Refer the Section 6.4 in Deployment guide and refer section 2 in User Guide for configure the primary region. 
 
**Note:** We need to add the **Admin** and **API** URL in the domain registry along with the external IP address for both, before accessing the Edison Admin Portal.   

Access the Admin Portal with **Admin Url**. Initially there are no events triggered.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a128.png)
 
Follow the section 4 in user guide to send the simulator data.

Once the simulator data is sent the devices are created in admin portal.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a129.png)

Once any event gets triggered from devices or simulator it will get reflected in the Edison admin portal as shown below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a130.png) 

By follow the section 4 in userguide you can acticvate the responces.Once activated will be shown in Responses like below.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a131.png)

### 6.2 Performing DR Strategies 

Perform the IoT Hub, Cosmos DB failovers as discussed in 5.3 and 5.4 section in above.

### 6.3 Dr Region Configuration

1.	Create a new direcotry "ProjectEdisonDr" for building the images in Dr region

2.	Copy the scripts-download.sh from /var/lib/waagent/custom-script/download/0/ to /var/lib/waagent/custom-script/download/0/ProjectEdisonDr/

3.	 Change directory to ProjectEdisonDr

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a132.png)

4.	To change the path for the downloading scripts, execute the below command.

**sed -i -e 's:'/var/lib/waagent/custom-script/download/0':'/var/lib/waagent/custom-script/download/0/ProjectEdisonDr':g' scripts-download.sh**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a133.png)

5.	Execute the scripts-download.sh file and clone the git hub repo in the current directory.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a134.png)
 
6.	Update the input.txt file inside the ProjectEdisonDr directory with Dr Region resource values.

**Note:** Keep the same **BASEURL_VALUE** and  **ADMINURL** for Dr Region.

7.	Refer the Section 6.4 in Deployment guide and refer section 2 in User Guide for configuring the Dr region configuration.

**Note:** We need to add the **Admin** and **API** URL in the domain registry along with the external IP address for both, before accessing the Edison Admin Portal.

As we are using same **Admin** and **API** Uris for **Primary and Dr Premium solutions,** Remove the Primary external IP address and update the Dr External IP address for Admin and API URL in the domain registry to access Dr region Edison Admin Portal.

Once we open Edison Portal Application, we will get the same "Activation Responses" and Devices which we created in Primary region.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a135.png) 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a136.png)
 
Refer the User Guide section4 to run simulator for getting events.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a137.png)
