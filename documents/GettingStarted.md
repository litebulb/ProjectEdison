# Smart Buildings Solution

## Getting Started Guide

### Table of Contents 


<!--ts-->
 - [1.0 Introduction](#10-introduction)
     - [1.1 The Internet of Things](#11-The-internet-of-things)
     - [1.2 Overview of Smart Buildings Solution](#12-overview-of-smart-buildings-solution)
     - [1.3 Core components](#13-core-components)
  - [2.0 IoT Solutions](#20-iot-solutions)
    - [2.1 Core Architecture](#21-core-architecture)
    - [2.2 Automated Solution](#22-automated-solution)
    - [2.3 Architectures](#23-architectures)
        - [2.3.1 Basic Architecture](#231-basic-architecture)
        - [2.3.2 Standard Architecture](#232-standard-architecture)
        - [2.3.3 Premium Architecture](#233-premium-architecture)
    - [2.4 Conventional Data Work Flow](#24-conventional-data-work-flow)
        - [2.4.1 Work flow with Simulator](#241-work-flow-with-simulator)
        - [2.4.2 Work flow with Devices](#242-work-flow-with-devices)
    - [2.5 Azure Components Functionality](#25-azure-components-functionality)
        - [2.5.1 IoT Hub](#251-iot-hub)
        - [2.5.2 Notification Hub](#252-notification-hub)
        - [2.5.3 Azure Active Directory](#253-azure-active-directory)
        - [2.5.4 Azure Automation](#254-azure-automation)
        - [2.5.5 Cosmos DB](#255-cosmos-db)
        - [2.5.6 OMS Log Analytics](#256-oms-log-analytics)
        - [2.5.7 Storage Account](#257-storage-account)
        - [2.5.8 Azure Kubernetes](#258-azure-kubernetes)
        - [2.5.9 Azure Container Registry](#259-azure-container-registry)
        - [2.5.10 Virtual Machine](#2510-virtual-machine)
        - [2.5.11 Service Bus](#2511-service-bus)
        - [2.5.12 Signal R](#2512-signal-r)
        - [2.5.13 Redis Cache](#2513-redis-cache)
        - [2.5.14 Azure Bot](#2514-azure-bot)
        - [2.5.15 Application Insights](#2515-application-insights)
- [3.0 Solution Types and Cost Mechanism](#30-solution-types-and-cost-mechanism)
   - [3.1 Solutions and Associated Costs](#31-solutions-and-associated-costs)
        - [3.1.1 Basic](#311-basic)
        - [3.1.2 Standard](#312-standard)
        - [3.1.3 Premium](#313-premium)
   - [3.2 Cost Comparison](#32-cost-comparison)
        - [3.2.1 In terms of features](#321-in-terms-of-features)
        - [3.2.2 Dollar Impact](#322-dollar-impact)
        - [3.2.3 Estimated Monthly Cost for each Solution](#323-estimated-monthly-cost-for-each-solution)
- [4.0 What are paired regions](#40-what-are-paired-regions)
- [5.0 Deployment Guide for the Solution](#50-deployment-guide-for-the-solution)
- [6.0 User Guide for the Solution](#60-user-guide-for-the-solution)
- [7.0 Admin Guide for the Solution](#70-admin-guide-for-the-solution)
 <!--te--> 
    
    
## 1.0 Introduction

The process for notifying authorities of an emergency is standardized and dates back to the 1960s.  The process for communicating into a crisis to the persons affected, however, is fragmented and not optimized.  Project Edison, a Safe Buildings solution, is a platform designed to manage this process and a way to speak into and monitor areas during a crisis event.

### 1.1 The Internet of Things

The Internet of Things (IoT) has created a buzz in the marketplace in the recent years. The IOT brings with it a concept of connecting any device to the internet and other connected devices to the network. 

IOT becomes a pivotal component which helps to have safer cities, homes and businesses; IOT enables both the private and public organizations to monitor facilities on a real-time basis. The IoT brings with it the secure connections of devices such as sensors, Gateway Devices to the smartphones to mention a few here. The combination of the connected devices would enable IoT solutions to “gather data, analyze the data and create an action” which enables to perform a task in near real time.

### 1.2 Overview of Smart Buildings Solution

Project Edison provides a template for a Microsoft Azure-based system for enabling rapid response to nearby community events that might increase the danger level of persons in the nearby vicinity. SmartBulbs, SmartButtons, and SmartSensors are used to enable rapid notification of nearby persons regarding impending or ongoing events of interest/concern.

### 1.3 Core components 

Project Edison solution is based on three core components.

*	**Mobile applications**

Admin mobile application is used for onboarding devices and associating metadata (latitude/longitude, building, floor, room, etc.) and ability to manage active crises.

Consumer Application (User Mobile application) allows administrators to send information to connected users in an area of crisis. Also, will allow consumers to provide additional information back to administrators about the crisis with future integration capability into popular social platforms.

*	**Devices**

Smart Bulb has an ability to change colors (Red/Yellow) to alert people of crisis event.

Sound Sensor will pick up specified threshold sound to trigger an event.

IoT Button is used by user for pushing a button to signal a crisis event.

*	**Kubernetes**

Azure Kubernetes Service will host the microservices for event processing, storing data, sending notifications to mobile applications (User and Admin) and managing devices (SmartBulb, Sound Sensor, IoT Button).

## 2.0 IoT Solutions

### 2.1 Core Architecture

Below Diagram explains the Core architecture for Smart Buildings solution.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/g1.png)
 
Core Architecture components:

*	1- Notification Hub

*	1- Cosmos DB

*	1- Storage Account

*	1- IoT Hub

*	1- Kubernetes

*	1- Azure Container Registry

*	1- Virtual Machine

*	1- Service Bus

*	1- Signal R

*	1- Redis Cache

### 2.2 Automated Solution

Automated IOT Solution is designed on the top of current core architecture. In addition, this solution also provides **Monitoring** and **High availability**.

This solution is deployed through ARM template. This is a single click deployment which reduces manual effort when compared with the existing solution.

In addition, this solution consists:

*	Application Insights to provide monitoring for Web Application. Application Insights store the logs of the Web API which will be helpful to trace the web API working.

*	Geo-replication to provide high availability for Cosmos DB. Geo-replication is used to set the recovery database as the primary database whenever primary database is failed.

*	This solution also provides Disaster Recovery activities. IoT Hub manual failover is helpful to make the IoT Hub available in another region, when IoT Hub of one region is failed.

*	Traffic Manager delivers high availability for critical web applications by monitoring the endpoints and providing automatic failover when an endpoint goes down.

### 2.3 Architectures

#### 2.3.1 Basic Architecture

Basic solution will have all core components, in addition this solution also consists monitoring components like Application Insights and OMS Log Analytics. 

*	Application Insights provide monitoring for Bot.

*	OMS Log Analytics provide monitoring for IoT hub, Cosmos DB, Kubernetes, Redis Cache, Service Bus.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/g2.png)
 
Basic Architecture comprises of following components:

*	1- Log analytics

*	1- Notification Hub

*	1- Cosmos DB

*	1- Storage Account

*	1- IoT Hub

*	1- Kubernetes

*	1- Azure Container Registry

*	1- Virtual Machine

*	1- Service Bus

*	1- Signal R

*	1- Redis Cache

#### 2.3.2 Standard Architecture

Standard Architecture diagram will have two regions.

1.	Primary Region (Deployment)

2.	Secondary Region (Re – Deployment)

We have IoT Hub manual failover, Cosmos DB geo replication, Service Bus Geo Recovery and redeployment components. The effect of these components will occur when primary Region goes down.

The main use of this solution is whenever disaster recovery occurs the redeployment components will deploy in another region, user need to manually add the obtained Admin IP address and API IP address after running the ingresses as an endpoint to the Traffic Manager.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/g3.png)

Standard Architecture comprises of following components:

*	1- Log analytics

*	1- Notification Hub

*	1- Cosmos DB

*	1- Storage Account

*	1- IoT Hub

*	1- Kubernetes

*	1- Azure Container Registry

*	1- Service Bus

*	1- Signal R

*	1- Redis Cache

*	1- Traffic Manager Profiles

**When there is a Region failover, user needs to redeploy ARM Template provided in GIT Repo. When redeployment Completed Successfully, below Azure resources will be deployed.**

*	1- Log analytics

*	1- Storage Account

*	1- Notification Hub

*	1- Signal R

*	1- Redis Cache

*	1- Kubernetes

*	1- Azure Container Registry

*	1- App Insights

*	1- Service Bus

**Note**:  Deployment process will take some time around 30mins to complete deployment Successfully.

#### 2.3.3 Premium Architecture:

Premium Architecture diagram also have two regions.

1.	Primary Region

2.	Secondary Region

All the components get deployed at once.

We have IoT Hub manual failover, Cosmos DB geo replication, Service Bus Geo Recovery.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/g4.png)
 
Premium Architecture comprises of following components:

*	2- Log analytics

*	2- Notification Hub

*	2- Signal R

*	2- Redis Cache

*	2- Storage Account

*	2- Kubernetes

*	2- Azure Container Registry

*	2- Traffic Manager Profiles

*	1- Cosmos DB

*	1- Automation Account

*	1- IoT Hub

*	1-Virtual Machine

*	1- Service Bus

### 2.4 Conventional Data Work Flow 

#### 2.4.1 Work flow with Simulator 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/g5.png)
 
#### 2.4.2 Work flow with Devices

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/g6.png)

### 2.5 Azure Components Functionality

Microsoft Azure is a cloud computing service created by Microsoft for building, testing, deploying, and managing applications and services through a global network of Microsoft-managed data centers. It provides software as a service (SaaS), platform as a service (PaaS) and infrastructure as a service (IaaS) and supports many different programming languages, tools and frameworks, including both Microsoft-specific and third-party software and systems.

Microsoft lists over 600 Azure services, of which some are as below:

*	Compute

*	Storage services

*	Data management

*	Management

*	Machine learning

*	IoT

#### 2.5.1 IoT Hub

**Introduction**:

Azure IoT Hub is a fully managed service that enables reliable and secure bi-directional communications between millions of IoT devices and an application back end. 

Azure IoT Hub offers reliable device-to-cloud and cloud-to-device hyper-scale messaging, enables secure communications using per-device security credentials and access control, and includes device libraries for the most popular languages and platforms. Before you can communicate with IoT Hub from a gateway you must create an IoT Hub instance in your Azure subscription and then provision your device in your IoT hub. Some samples in this repository require that you have a usable IoT Hub instance.

The Azure IoT Hub offers several services for connecting IoT devices with Azure services, processing incoming messages or sending messages to the devices. From a device perspective, the functionalities of the Azure IoT Hub enable simple and safe connection of IoT devices with Azure services by facilitating bidirectional communication between the devices and the Azure IoT Hub.

**Implementation**:

In this solution the IoT Hub receives the events from the SoundSensor, SmartBulb and IoTbutton devices or from the simulators, which will be pushed to Azure service bus and get populated in the Edison Admin Portal Application.


#### 2.5.2 Notification Hub

**Introduction**:

Azure Notification Hubs provides a highly scalable, cross-platform push notification infrastructure that enables you to either broadcast push notifications to millions of users at once, or tailor notifications to individual users. You can use Notification Hubs with any connected mobile application—whether it’s built on Azure Virtual Machines, Cloud Services, Web Sites, or Mobile Services.

Azure Notification Hubs are push notification software engines designed to alert users about new content for a given site, service or app. Azure Notification Hubs are part of Microsoft Azure’s public cloud service offerings.

**Implementation**:

The Notification Hub pushes the Alerts (Activated Responses) and messages to the User Mobile application.

#### 2.5.3 Azure Active Directory

**Introduction**:

Microsoft Azure Active Directory (Azure AD) is a cloud service that provides administrators with the ability to manage end user identities and access privileges. The service gives administrators the freedom to choose which information will stay in the cloud, who can manage or use the information, what services or applications can access the information and which end users can have access.

**Implementation**:

Azure Active directory is used to authenticate users to login to Admin Portal. Azure active Directory enables secure authentications to Admin Portal.

#### 2.5.4 Azure Automation

**Introduction**:

Azure Automation enables the users to automate the tasks, which are manual and repetitive in nature by using Runbooks. 

Runbooks in Azure Automation are based on Windows PowerShell or Windows PowerShell Workflow. We can code and implement the logic, which we want to automate, using PowerShell.

**Implementation**:

In this Solution Azure run books are used to create Database and collections in Document DB, it is also used to update reply URLs in Active Directory Application.

#### 2.5.5 Cosmos DB  

**Introduction**:

Azure Cosmos DB is a Microsoft cloud database that supports multiple ways of storing and processing data. As such, it is classified as a multi-model database. In multi-model databases, various database engines are natively supported and accessible via common APIs.

**Implementation**:

In this Solution, Cosmos DB have Templates, Messages and Groups Collections. The Messages collections will get updated with the telemetry data of the Device.

#### 2.5.6 OMS Log Analytics

**Introduction**:

The Microsoft Operations Management Suite (OMS), previously known as Azure Operational Insights, is a software as a service platform that allows an administrator to manage on-premises and cloud IT assets from one console.

Microsoft OMS handles log analytics, IT automation, backup and recovery, and security and compliance tasks.

Log analytics will collect and store your data from various log sources and allow you to query over them using a custom query language.

**Implementation**:

Log analytics to provide monitoring for Cosmos DB, IoT Hub, Kubernetes, Redis Cache, Service Bus.

Log analytics store the logs, which will be helpful to trace the working of these resources. OMS log analytics provides in detailed insights using solutions

#### 2.5.7 Storage Account

**Introduction**:

An Azure storage account contains all your Azure Storage data objects: blobs, files, queues, tables, and disks. Data in your Azure storage account is durable and highly available, secure, massively scalable, and accessible from anywhere in the world over HTTP or HTTPS.

**Implementation**:

The older events (actions and telemetry) will be stored in Blob Storage/Table storage for auditing purposes.

### 2.5.8 Azure Kubernetes

**Introduction**:

Kubernetes provides a container-centric management environment. It orchestrates computing, networking, and storage infrastructure on behalf of user workloads. Its groups containers that make up an application into logical units for easy management and discovery. 

**Implementation**:

Azure Kubernetes Service will host the microservices for event processing, storing, sending notifications to mobile applications and managing devices

#### 2.5.9 Azure Container Registry

**Introduction**:

Azure Container Registry is a private registry for hosting container images. Using the Azure Container Registry, you can store Docker-formatted images for all types of container deployments. Azure Container Registry integrates well with orchestrators hosted in Azure Container Service, including Docker Swarm, DC/OS, and Kubernetes. Users can benefit from using familiar tooling capable of working with the open source Docker Registry v2.

**Implementation**:

Control image names for all container deployments. Use Kubernetes commands to push images into a repository or pull an image from a repository. In addition to container images, Azure Container Registry stores images used to deploy applications to Kubernetes.

#### 2.5.10 Virtual Machine

**Introduction**:

Virtual Machines gives you the flexibility of virtualization for a wide range of computing solutions with support for Linux, Windows Server, SQL Server, Oracle, IBM, SAP and more. All current generation Virtual Machines include load balancing and auto-scaling at no cost.

Each virtual machine provides its own virtual hardware, including CPUs, memory, hard drives, network interfaces and other devices. The virtual hardware is then mapped to the real hardware on the physical machine which saves costs by reducing the need for physical hardware systems along with the associated maintenance costs that go with it, plus reduces power and cooling demand.

**Implementation**:

Execute scripts which updates the configurations, environments of Microservices buy building the docker images and pushing them to Azure Container Registry also installs helm, ingress controllers making it responsible to access Admin Portal.

#### 2.5.11 Service Bus

**Introduction**:

Microsoft Azure Service Bus is a fully managed enterprise integration message broker. Service Bus is most commonly used to decouple applications and services from each other and is a reliable and secure platform for asynchronous data and state transfer. Data is transferred between different applications and services using messages. A message is in binary format, which can contain JSON, XML, or just text.

**Implementation**:

The Events message which got triggered from the Devices and sent to IoT Hub, Service Bus is pulling data from IoTHub and sends to the services deployed in the Kubernetes.

#### 2.5.12 Signal R

**Introduction**:

Azure SignalR Service simplifies the process of adding real-time web functionality to applications over HTTP. This real-time functionality allows the service to push content updates to connected clients, such as a single page web or mobile application. As a result, clients are updated without the need to poll the server or submit new HTTP requests for updates.

**Implementation**:

The notification and alerts which from sent from Edison Admin Portal is begin sent to User Mobile application and to the devices.

#### 2.5.13 Redis Cache

**Introduction**:

Azure Cache for Redis is based on the popular software Redis. It is typically used as a cache to improve the performance and scalability of systems that rely heavily on backend data-stores. With Azure Cache for Redis, this fast storage is located in-memory with Azure Cache for Redis instead of being loaded from disk by a database.

Azure Cache for Redis can also be used as an in-memory data structure store, distributed non-relational database, and message broker. 

**Implementation**:

Azure Service Bus messages which are pulled from IoT Hub are being sent to Redis Cache, which will also get populated in the Cosmos DB’s collections.

#### 2.5.14 Azure Bot:

**Introduction**:

Azure Bot Service provides tools to build, test, deploy, and manage intelligent bots all in one place. Your own Bot hosted where you want, registered with the Azure Bot Service. Build, connect, and manage Bots to interact with your users wherever they are - from your app or website to Cortana, Skype, Messenger and many other services.

**Implementation**:

Azure Bot is used for communication between User Mobile Application and Edison Admin Portal Application, for sending messages, activities and notifications.

#### 2.5.15 Application Insights

**Introduction**:

Application Insights is an extensible Application Performance Management (APM) service for web developers on multiple platforms. Use it to monitor live web application. It will automatically detect performance anomalies. It includes powerful analytics tools to help diagnose issues and to understand what users do with web application.

Application Insights monitor below:

•	Request rates, response times, and failure rates

•	Dependency rates, response times, and failure rates

•	Exceptions 

•	Page views and load performance

•	AJAX calls

•	User and session counts

•	Performance counters

•	Host diagnostics from Docker or Azure

•	Diagnostic trace logs

•	Custom events and metrics

**Implementation**:

We are implementing Application Insights monitoring for Azure Service Bus, Services in the Kubernetes Container, Notification hub and Azure bot.

## 3.0 Solution Types and Cost Mechanism

### 3.1 Solutions and Associated Costs

The Automated solutions provided by us covers in below Section. Will have the following Cost associated. The solutions are created considering users requirements & have Cost effective measures. User have control on what Type of Azure resources need to be deploy with respect to SKU And Cost. These options will let user to choose whether user wants to deploy Azure resources with minimal SKU and Production ready SKU. The Cost Models per solutions are explained in future sections:

#### 3.1.1 Basic

The Basic solution requires minimum Azure components with minimal available SKU’s. This Solution provides (Core + Monitoring) features such as application Insights & OMS Log Analytics. The details on components used in this solution is listed in Section: 

 The Estimated Monthly Azure cost is: **$603.36**

*Note: Refer below table for the optional component list & Features*

**Pricing Model for Basic**:

Prices are calculated by considering Location as **East US** and Pricing Model as **“PAYG”**.


| **Resource Name**                   | **Size**                    | **Azure Cost/month**                                                                                   
| -------------                     | -------------                  | --------------------                                                                    
| **Application Insights**       | 5 GB ingested, 0 Multi-step Web Tests         | $0.00  
| **Notification Hub**   | Free Includes 1 million pushes for 500 active devices.      | $0.00  
| **IoT Hub**        | S1, Unlimited devices, 1 Unit-$25.00/per month 400,000 messages/day                    | $25.00    
| **Storage Account**        | Block Blob Storage, General Purpose V2, LRS Redundancy, Hot Access Tier, 10 GB Capacity, 100,000 Write operations, 100,000 List and Create Container Operations, 100,000 Read operations, 0 Other operations. 10 GB Data Retrieval, 0 GB Data Write       | $1.25   
| **Azure Cosmos DB**       | Standard, throughput 400 RU/s (Request Units per second) 4x100 Rus(Throughput)- $23.36 10 GB storage – $2.50         | $25.86  
| **Log Analytics**   | 1 VMs monitored, 5 GB average log size, 0 additional days of data retention   | $0.00  
| **Azure Automation Account**   | Capability: Process Automation 500 minutes of process automation and 744 hours of watchers are free each month.    | $0.00
| **Azure Container Registry**   | Basic Tier, 1 units x 30 days, 5 GB Bandwidth     | $5.00
| **Kubernetes**   | 3 * [B2S (2 vCPU(s), 4 GB RAM) nodes x 730 Hours; managed OS disks x P4 ($14.40)]      | $124.46
| **Service Bus**   | Standard tier: 10, 1,000 brokered connection(s), 0 Hybrid Connect listener(s) + 0 overage per GB, 0 relay hour(s), 10 relay message(s)      | $9.82
| **Virtual Machine**   | standard_A2 (2 vCPU(s), 3.5 GB RAM) x 730 Hours; 2 Linux – Ubuntu managed OS disks – S4      | $90.72
| **Windows VM**   | 1 D4s v3 (4 vCPU(s), 16 GB RAM) x 730 Hours; Windows – (OS Only); Pay as you go; 0 managed OS disks – S4, 100 transaction units      | $305.19
| **Signal R**   | Free Tier, Includes 1 unit with 20 concurrent connections per unit and 20,000 messages per unit/day      | $0.00
| **Redis Cache**   | C0: Basic tier, 1 instance(s), 730 Hours      | $16.06
| **Bot**   | Free Tier  Standard Channels- Unlimited messages included.  Premium Channels- 10,000 messages included.      | $0.00
|        |         |    **Total Cost/Month**     | **$603.19** 


#### 3.1.2 Standard

This Solution provides (Core + Monitoring +Hardening) features such as application Insights, OMS Log Analytics, High Availability & Disaster recovery. The details on components used in this solution is listed in Section: 

The Estimated Monthly Azure cost is: **$1228.61**

*Note: Refer below table for the optional component list & Features*

**Pricing Model for Standard Solution**:

Prices are calculated by Location as **East US** and Pricing Model as **“PAYG”**.


| **Resource Name**               | **Size**              | **Azure Cost/month**                                                                        
| -------------              | -------------                  | --------------------                                                                                                  
| **Application Insights**       | 2 * Basic, 1GB * $2.30 First 5GB free per month          | $4.60  
| **Notification Hub**   | 2 * (Free Includes 1 million pushes for 500 active devices.)     | $0.00  
| **IoT Hub**        | S1, Unlimited devices, 1 Unit-$25.00/per month 400,000 messages/day                    | $25.00    
| **Storage Account**        | Block Blob Storage, General Purpose V2, LRS Redundancy, Hot Access Tier, 10 GB Capacity, 100,000 Write operations, 100,000 List and Create Container Operations, 100,000 Read operations, 0 Other operations. 10 GB Data Retrieval, 0 GB Data Write       | $2.50   
| **Azure Cosmos DB**       | Standard, throughput 400 RU/s (Request Units per second) 4x100 Rus(Throughput)- $23.36 10 GB storage – $2.50         | $25.86  
| **Log Analytics**   | 1 VMs monitored, 5 GB average log size, 0 additional days of data retention   | $0.00  
| **Azure Automation Account**   | Capability: Process Automation 500 minutes of process automation and 744 hours of watchers are free each month.    | $0.00
| **Azure Container Registry**   | Standard Tier, 1 units x 30 days, 5 GB Bandwidth     | $20.45
| **Kubernetes**   | 2 * [3B2S (2 vCPU(s), 4 GB RAM) nodes x 730 Hours; managed OS disks x P4 ($14.40)]      | $248.92
| **Service Bus**   | 2 * (Standard tier: 10, 1,000 brokered connection(s), 0 Hybrid Connect listener(s) + 0 overage per GB, 0 relay hour(s), 10 relay message(s))      | $19.64
| **Virtual Machine**   | standard_A2 (2 vCPU(s), 3.5 GB RAM) x 730 Hours; 2 Linux – Ubuntu managed OS disks – S4      | $90.72
| **Windows VM**   | 2 * (1 D4s v3 (4 vCPU(s), 16 GB RAM) x 730 Hours; Windows – (OS Only); Pay as you go; 0 managed OS disks – S4, 100 transaction units)      | $610.38
| **Signal R**   | 2 * (Free Tier, Includes 1 unit with 20 concurrent connections per unit and 20,000 messages per unit/day)      | $97.94
| **Redis Cache**   | 2 * (C0: Basic tier, 1 instance(s), 730 Hours)      | $80.30
| **Bot**   | Free Tier  Standard Channels- Unlimited messages included.  Premium Channels- 10,000 messages included.      | $0.50
| **Traffic Manager**   | 2 * DNS Query $0.54 + Azure Endpoint $0.36       | $1.80
|        |         |    **Total Cost/Month**     | **$1228.61** 

**Note: When we redeploy the solution, there will not be any extra cost, since primary region is already paid.** 

#### 3.1.3 Premium 

This solution also provides (Core + Monitoring +Hardening), the difference between Standard & Premium solution is under Premium - Both the regions can be deployed at same time, however this is not possible under standard solution. The details on components used in this solution is listed in Section: 

 The Estimated Monthly Azure cost is: **$1228.61**

**Pricing Model for Premium Solution:**

Prices are calculated by Considering Location as **East US** and Pricing Model as **“PAYG”**.


| **Resource Name**               | **Size**                | **Azure Cost/month**                                                                                         
| -------------                  | -------------                 | --------------------                                                                                                      
| **Application Insights**       | 2 * Basic, 1GB * $2.30 First 5GB free per month          | $4.60  
| **Notification Hub**   | 2 * (Free Includes 1 million pushes for 500 active devices.)     | $0.00  
| **IoT Hub**        | S1, Unlimited devices, 1 Unit-$25.00/per month 400,000 messages/day                    | $25.00    
| **Storage Account**        | Block Blob Storage, General Purpose V2, LRS Redundancy, Hot Access Tier, 10 GB Capacity, 100,000 Write operations, 100,000 List and Create Container Operations, 100,000 Read operations, 0 Other operations. 10 GB Data Retrieval, 0 GB Data Write       | $2.50   
| **Azure Cosmos DB**       | Standard, throughput 400 RU/s (Request Units per second) 4x100 Rus(Throughput)- $23.36 10 GB storage – $2.50         | $25.86  
| **Log Analytics**   | 1 VMs monitored, 5 GB average log size, 0 additional days of data retention   | $0.00  
| **Azure Automation Account**   | Capability: Process Automation 500 minutes of process automation and 744 hours of watchers are free each month.    | $0.00
| **Azure Container Registry**   | Standard Tier, 1 units x 30 days, 5 GB Bandwidth     | $20.45
| **Kubernetes**   | 2 * [3B2S (2 vCPU(s), 4 GB RAM) nodes x 730 Hours; managed OS disks x P4 ($14.40)]      | $248.92
| **Service Bus**   | 2 * (Standard tier: 10, 1,000 brokered connection(s), 0 Hybrid Connect listener(s) + 0 overage per GB, 0 relay hour(s), 10 relay message(s))      | $19.64
| **Virtual Machine**   | standard_A2 (2 vCPU(s), 3.5 GB RAM) x 730 Hours; 2 Linux – Ubuntu managed OS disks – S4      | $90.72
| **Windows VM**   | 2 * (1 D4s v3 (4 vCPU(s), 16 GB RAM) x 730 Hours; Windows – (OS Only); Pay as you go; 0 managed OS disks – S4, 100 transaction units)      | $610.38
| **Signal R**   | 2 * (Free Tier, Includes 1 unit with 20 concurrent connections per unit and 20,000 messages per unit/day)      | $97.94
| **Redis Cache**   | 2 * (C0: Basic tier, 1 instance(s), 730 Hours)      | $80.30
| **Bot**   | Free Tier  Standard Channels- Unlimited messages included.  Premium Channels- 10,000 messages included.      | $0.50
| **Traffic Manager**   | 2 * DNS Query $0.54 + Azure Endpoint $0.36       | $1.80
|        |         |    **Total Cost/Month**     | **$1228.61** 

### 3.2 Cost Comparison 

In this section we will be comparing the cost for all the solution provided in terms of Features &dollar $ Impact:

#### 3.2.1 In terms of features

The below table explain the distinctive features available across solution types.


| **Resource Name**           | **Parameter**         | **Basic**                  | **Standard**            | **Premium**                                                                                                                 
| -------------               | -------------         | --------------------       | ------------            | ----------    
| Traffic Manager             | DNS Queries           |                            | 1 million/month         | 1 million/month
|                             | Endpoint       |                            | Azure EndPoint 1 per month             | Azure EndPoint 1 per month
| IoT Hub                     | SKU                   | S1                         | S1                      | S1
|                             | Devices               | Unlimited devices          | Unlimited Devices       | Unlimited Devices
|                             | Messages              | 400,000 messages/day           | 4,00,000 msgs/day       | 4,00,000 msgs/day
| Automation Account            | Capability        | Process Automation 500 minutes of process automation and 744 hours of watchers are free each month         | Process Automation 500 minutes of process automation and 744 hours of watchers are free each month            | Process Automation 500 minutes of process automation and 744 hours of watchers are free each month
| Storage Account                    | Storage Account Type         | Standard_LRS      | Standard_GRS    | Standard_GRS
| Azure Cosmos DB             | SKU                   | Standard                   | Standard               |Standard 
|                              | Database                   | 1                   | 1               |1
|                             | Storage               | 10 GB                 | 10 GB             | 10 GB 
|                             | Purchase model        | 4 * 100 RU/sec             | 4 * 100 RU/sec         |4 * 100 RU/sec
| Log Analytics               | Logs ingested        | 5 GB of data is included for free. An average Azure VM ingests 1 GB to 3 GB            | 5 GB of data is included for free. An average Azure VM ingests 1 GB to 3 GB              | 5 GB of data is included for free. An average Azure VM ingests 1 GB to 3 GB
| Application Insights        | Logs collected        | 6 GB, 5 GB of data is included for free.            | 6 GB, 5 GB of data is included for free.              | 6 GB, 5 GB of data is included for free.
| Kubernetes            | Cluster Management              | 3 x B2S (2 vCPU(s), 4 GB RAM) nodes            | 3 x B2S (2 vCPU(s), 4 GB RAM) nodes          | 3 x B2S (2 vCPU(s), 4 GB RAM) nodes
| Azure Container Registry            | SKU              | Basic            | Standard          | Standard
| Service Bus            | SKU              | Standard            | Standard          | Standard
| Signal R            | SKU              | Free            | Standard          | Standard
| Virtual Machine            | Size              | 1x standard_A2 (2 vCPU(s), 3.5 GB RAM) ;2 Linux – Ubuntu managed OS disks             | 1x standard_A2 (2 vCPU(s), 3.5 GB RAM) ;2 Linux – Ubuntu managed OS disks           | 1x standard_A2 (2 vCPU(s), 3.5 GB RAM) ;2 Linux – Ubuntu managed OS disks 
| Notification Hub            | SKU              | Free            | Free          | Free


#### 3.2.2 Dollar Impact: 

The below Table explains the $ impact for the solutions by resources.

| **Resource Name**           | **Basic**                  | **Standard**                 | **Premium**                                                                                                                
| -------------              | ------------------         | --------------------                       | ------------ 
| **Application Insights**                  | $0.00          | $4.60          | $4.60
| **Notification Hub**                  | $0.00          | $0.00          | $0.00
| **IoT Hub**                      | $25.00           | $25.00	          | $25.00
| **Storage Account**           | $1.25	         | $2.50	          | $2.50
| **Azure Cosmos DB**              | $25.86	         | $25.86	          | $25.86
| **Log Analytics**              | $0.00	         | $0.00	          | $0.00
| **Automation Account**   | $0.00	         | $0.00	          | $0.00
| **Traffic Manager**              | $0.00	         | $1.80           | $1.80
| **Notification Hub**              | $0.00	         | $0.00           | $0.00
| **Kubernetes**              | $124.46	         | $248.92           | $248.92
| **Azure Container Registry**              | $5.00	         | $20.45           | $20.45
| **Service Bus**              | $9.82	         | $9.82           | $9.82
| **Virtual Machine**              | $90.72	         | $90.72           | $90.72
| **Signal R**              | $0.00	         | $97.94           | $97.94
| **Redis Cache**              | $16.06	         | $80.3           | $80.3
| **Bot**              | $0.00	         | $0.50           | $0.50

#### 3.2.3 Estimated Monthly Cost for each Solution:

| **Resource Name**           | **Basic**                    | **Standard**                 | **Premium**                                                                                                                
| -------------              | ------------------------       | --------------------      | ------------ 
| **Estimated monthly cost**            | **$ 603.36**            | **$ 1228.61**  	             | **$ 1228.61**

## 4.0 What are paired regions

Azure operates in multiple geographies around the world. An Azure geography is a defined area of the world that contains at least one Azure Region. An Azure region is an area within a geography, containing one or more datacenters. 

Each Azure region is paired with another region within the same geography, together making a regional pair. The exception is Brazil South, which is paired with a region outside its geography. 

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

## 5.0 Deployment Guide for the Solution

To Deploy Basic, Standard or Premium Solution please refer [Deployment Guide Documentation](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Deplyment%20Guide.md). 

## 6.0 User Guide for the Solution

For Device Configuration, Running Simulator and verifying Edison Admin Portal please refer [User Guide Documentation](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/User%20Guide.md). 

## 7.0 Admin Guide for the Solution

To configure and validate the Standard and Premium Solution please refer [Admin Guide](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Admin%20Guide.md). 
