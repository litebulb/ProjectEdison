# Microsoft

# Safe Buildings Solution

## User Guide

### Contents
 
 - [1. Introduction to User Guide](#1-introduction-to-user-guide)
 - [2. Kubernetes Setup](#2-kubernetes-setup)
   - [2.1 Execute imageupdate6.sh ](#21-execute-imageupdate6.sh)
   - [2.2 Execute clusterconnect7.sh](#22-execute-clusterconnect7.sh)
   - [2.3 Execute set-kubernetes-config8.sh](#23-execute-set-kubernetes-config8.sh)
   - [2.4 Execute updateyaml9.sh](#24-execute-updateyaml9.sh)
   - [2.5 Execute ingress_custom10.sh](#25-execute-ingress-_custom10.sh)
 - [3. Manual configuration](#3-manual-configuration)
 - [4. Building Edison.Simulators.Sensors Project](#4-building-edison.-simulators.-sensors-project)
 - [5. Create a Firebase project](#5-create-a-firebase-project)
 - [6. Mobile Application Configuration](#6-mobile-application-configuration)

   
## 1. Introduction to User Guide

This Document explains about how to use the solution. In this we are building the docker images, pushing them to Azure Container Registry to configure the Edison Admin portal and monitoring the resources of the solution.  

## 2. Kubernetes Setup

### 2.1. Execute imageupdate6.sh

**Imagesupdate6.sh** script: To build the images and push them to ACR container repositories.

` sh imagesupdate6.sh `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/1.png)

Navigate to **Azure portal** -> **Resource Group** -> **Container Registry** > **Repositories** and check for the images which have been pushed.

**Note:** If you got any issues to push the images to ACR just rerun the deploy1.sh script and again run the imageupdate6.sh script.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/2.png)

### 2.2. Execute clusterconnect7.sh

**clusterconnect7.sh** script: Connect to the cluster.

` sh clusterconnect7.sh `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/3.png)

### 2.3. Execute set-kubernetes-config8.sh

**set-kubernetes-config8.sh** script: Creates config maps.

` sh set-kubernetes-config8.sh `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/4.png)

Check the **config maps** using the below command.

` kubectl get cm `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/5.png)

### 2.4. Execute updateyaml9.sh

**Updateyaml9.sh** script: Creates Pods and Services.

` sh updateyaml9.sh `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/6.png)

### 2.5. Execute ingress_custom10.sh 

**ingress_custom10.sh** script: Installs helm.

` sh ingress_custom10.sh `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/7.png)

1. Using **WINSCP** push your certificates to the virtual machine.

Switch user to **adminuser**.

Install **unzip command** using the below command.

` sudo apt install unzip `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/8.png)

2. Unzip the certificate using the below command.

` unzip Kubernetes_certs.zip `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/9.png)

3. Copy the file to another file.

` cat 2289f3206db82816.crt gd_bundle-g2-g1.crt > xxxxxxxx-xxx.com.chained.crt `

4. switch to **root** user and go to **/var/lib/waagent/custom-script/download/0**

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/10.png)

5. Create **secrets** using the below commands:

``` 
kubectl create secret tls <adminsecret name> --cert /home/adminuser/qloudable-npr.com.chained.crt --key /home/adminuser/qloudable-npr.key 

```

```
kubectl create secret tls <apisecret name> --cert /home/adminuser/qloudable-npr.com.chained.crt --key /home/adminuser/qloudable-npr.key

```
    	
```
kubectl get secrets 

```

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/11.png)

6. Update name of **hosts** and ****Secrets** in nginix config files.

a)

```
sed -i -e 's/edisonadminportal.eastus.cloudapp.azure.com/'<admin URL>'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-adminportal.yaml

```
Ex: 

```
sed -i -e 's/edisonadminportal.eastus.cloudapp.azure.com/'<basicadmin.xxxxx-xxx.com>'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-adminportal.yaml

```
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/12.png)

b)

```
sed -i -e 's/tls-secret-adminportal/'<adminsecret name>'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-adminportal.yaml

```
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/13.png)

c)

```
sed -i -e 's/edisonapi.eastus.cloudapp.azure.com/'<api URL>'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-api.yaml

```
Ex:

```
sed -i -e 's/edisonapi.eastus.cloudapp.azure.com/'<basicapi.xxxxx-xxx.com>'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-api.yaml

```

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/14.png)

d)

```
sed -i -e 's/tls-secret-api/'<apisecret>'/g' ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom/nginx-config-api.yaml

```
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/d.png)

7. Assign a **static-IP** to an Ingress on through the Nginx controller.

a) **On Admin:**

```
az network public-ip create -g <Cluster Resource Group Name>-n <name of admin ip> --dns-name <admin dns name> --allocation-method static

```
**Ex:** 

```
az network public-ip create -g MC_MO_basic_2304_akswih6_eastus2 -n adminbotip --dns-name dnsbotadmin --allocation-method static

```

**Copy** the **admin static IP** and **save** it.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/15.png)

b) **On API:**

```
az network public-ip create -g <Cluster Resource Group Name>-n <name of api ip> --dns-name <api dns name> --allocation-method static

```
**Ex:**

```
az network public-ip create -g MC_MO_basic_2304_akswih6_eastus2 -n apibotip --dns-name dnsbotapi --allocation-method static

```
**Copy** the **api static IP** and **save** it.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/16.png)

8. Install the NGINX Ingress controller into the system namespace using the existing static IP address and as AKS is not RBAC enabled, the command needs to set RBAC related values to false. 

**On Admin:**

```
helm install --name nginx-ingress-admin stable/nginx-ingress --namespace kube-system --set rbac.create=false --set rbac.createRole=false --set rbac.createClusterRole=false --set controller.ingressClass=nginx-admin --set controller.service.loadBalancerIP=”<admin Static IP address>”

```

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/17.png)

**On API:**

```
helm install --name nginx-ingress-api stable/nginx-ingress --namespace kube-system --set rbac.create=false --set rbac.createRole=false --set rbac.createClusterRole=false --set controller.ingressClass=nginx-api --set controller.service.loadBalancerIP="<api Static IP address>”

```
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/18.png)

9. Create config file.

cd ProjectEdison/Edison.Web/Kubernetes/qa/Deployment/Ingress_Custom

Create **admin config** file by running the below command

` kubectl create -f ./nginx-config-adminportal.yaml `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/19.png)

Create **API config** file by running the below command

` kubectl create -f ./nginx-config-api.yaml `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/20.png)

To get **ingress** give the below command

` kubectl get ing `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/21.png)

Check **status of services** using the namespace kube-system

` kubectl get svc -n kube-system `

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/22.png)

## 3.Manual configuration

1. Copy the **API URL** from Hosts.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/23.png)

2. Update in the **messaging endpoint** of **Bot Channel** Registration and click on **Save**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/24.png)

3. Update the **reply URL** of the **azure active directory** application with admin URL from Hosts and Click on **Save**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/25.png)

## 4. Building Edison.Simulators.Sensors Project

Browse to the **Admin Portal** using the **Admin URL** also can be taken from step 1 of section 3.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/26.png)

Initially there are no events triggered.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/27.png)

1. For **configuring raspberry pi** and devices follow below documentation. 

[Device Configuration Documentation](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/deviceconfiguration.docx)

2. Follow the below document to **configure the simulator**.

[Simulator Documentation](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/simulator.docx)

3. Once any **events** gets triggered from devices or simulator it will get reflected in the **Edison admin portal** as shown below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/28.jpg)

4. To **view the devices** which got created or onboarded click on the **wi-fi** icon as shown below. Here you can also see the location, type, current response of the devices.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/29.png)

5. To create or generate an alert, click on **Activate Response**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/30.png)

6. Select the **activity**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/31.png)

7. **Click and hold** on **Activate** icon to get triggered.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/32.png)

8. Once activated will be shown in Responses.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/33.png)

9. Set **location** on map.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/34.png)

10. Click on **Done**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/35.png)

11. To get the History of **All the Activities** which are occurred in the selected interval of date, go to **History** blade as shown below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/36.png)

12.	select the **Start date** and **End Date** from the Calendar on right side of the field and click on “Download” button.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/37.png)

13.	Once the download is completed it will generate an excel sheet consists of all the events occurred in the selected date interval.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/38.png)

14. To update the Activity Activation message and Deactivation message, primary and secondary radius distance, go to settings blade and select the Activity which you want to modify.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/39.png)

15.	Modify the Activation message or Deactivation message as per requirement and increase/decrease the Primary and Secondary radius of the Activity as shown below. Once done click on **SAVE CHANGES** button.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/40.png)

## 5. Create a Firebase project

We need to create an application in the Firebase from which will get an API Key and google-services.json file, these are used in the User Mobile application configuration.

1. Click on the below link to follow the steps to create a project in Firebase.

[Firebase Documentation](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/firebase.docx)

2. In the Firebase console, select the **settings** icon for your project. Then select Project Settings.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/41.png)

3. If you haven't downloaded the **google-services.json** file before, you can do so on this page.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/42.png)

4. Switch to the **Cloud Messaging** tab at the top.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/43.png)

5. **Copy** and **save** the **Legacy Server key** for later use. You use this value to configure your notification hub.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/45.png)

## 6. Mobile Application Configuration

1. Follow the below link to configure and build the User mobile application.

[Mobile Application Configuration](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/mobile_application.docx)

2. Navigate to Portal -> **Notification Hub** -> Select **Google** from Settings and **paste** the **Legacy Server Key** copied from step 6 of section 5 in API Key. Click on **Save**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/44.png)


3. We need to create **Emulator** of Android Version 8.1 in visual studio. Open Visual Studio and select the **Tools** -> **Android** -> **Android Device Manager**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/46.png)

4. Select Yes from the pop-up window and then we will get the below screen.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/47.png)

5. Click on **+ New** to create a new Emulator from right hand side menu.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/48.png)

6. The below screen will appear, provide unique name to Emulator and modify the ram to 2GB and click on **create** button.

**Note:** The Android os version should be 8.1-API 27

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/49.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/50.png)

7. The Emulator will take few seconds to create, close the **Android Device Manager window**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/51.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/52.png)

8. Re-open the Mobile application source code in Visual studio, the created Emulator will get displayed as shown below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/53.png)

9. Click on the created **Emulator** to run the User Mobile application. 

**Note:** Set the default project to **Edison.Mobile.User.Client.Droid** and select the **Android User** from top menu.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/54.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/55.png)

10. Click on **Sign up** now for First time user in your Mobile.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/56.png)

11.	Enter the details to create a **Login** and click on **Send Verification code**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/57.png)

12. Once you mail is verified, click on **verify** code after entering the code for verification.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/Solution%20Documentation/Images/58.png)

13.	**Sign in** with the created username and password.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/59.png)

14. Click on **report activity** to send message in case of any activity.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/60.png)

15. Enter your message.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/61.png)

16. The report will be activated in Edison Admin portal application.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/62.png)

17. Similarly, the vice-versa communication can be done from Edison Admin Portal to User Mobile application.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/63.png)

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/64.png)

18.	The test notification message can be sent to User Mobile application using Notification Hub’s test send button as shown below. 

**Note:** select Platforms to **Android** and click on **send** button.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/65.png)

19.	Once the notification successfully sent will get the success notification message in portal.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/66.png)

20.	We can see the notification alert in the user mobile application as below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/67.png)

21.	Similarly, when we create any **Activated Response** in the Edison Admin Portal then we will get alert popup for it in User Mobile application, as shown below.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/68.png)

22. Similarly, we can validate the User Mobile application functionality on IOS Platform.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/69.png)
