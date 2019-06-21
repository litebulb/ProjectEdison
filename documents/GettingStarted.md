# Smart Buildings Solution

## Getting Started Guide

### Table of Contents 


<!--ts-->
 - [1.0 Introduction](#10-introduction)
     - [1.1 The Internet of Things](#11-The-internet-of-things)
     - [1.2 Overview of Smart Buildings Solution](#12-overview-of-smart-buildings-solution)
     - [1.3 Core components](#13-core-components)
  - [2.0 IoT Core Solution Architecture](#20-iot-core-solution-architecture)
    - [2.1 Core Architecture](#21-core-architecture)
    - [2.2 Automated Deployment Configurations](#22-automated-deployment-configurations)
    - [2.3 Deployment Configurations Tiers](#23-deployment-configurations-tiers)
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

The process for notifying authorities of an emergency is standardized and dates to the 1960s.  The process for communicating into a crisis to the persons affected, however, is fragmented and not optimized.  Project Edison, a Safe Buildings solution, is a platform designed to manage this process and a way to speak into and monitor areas during a crisis event.

This solution accelerator provides the complete application code as well as various deployment configurations to accelerate the adoption and customization of the solution as required. 


### 1.1 The Internet of Things

The Internet of Things (IoT) has created a buzz in the marketplace in recent years. The IoT brings with it a concept of connecting any compatible device to the internet and all connected devices to a network. 

IoT becomes a pivotal component which helps to have safer cities, homes and businesses by enabling both the private and public organizations to monitor facilities on a real-time basis. The IoT brings with it the secure connections of devices such as sensors, Gateway Devices to the smartphones to mention a few here. The combination of the connected devices would enable IoT solutions to “gather data, analyze the data and create an action” which enables to perform a task in near real time.


### 1.2 Overview of Smart Buildings Solution

The process for notifying authorities of an emergency is standardized and dates to the 1960s.  The process for communicating into a crisis to the persons affected, however, is fragmented and not optimized.  Project Edison, a Safe Buildings solution, is a platform designed to manage this process and a way to speak into and monitor areas during a crisis event.

The platform allows a way to receive events from a centralized communication hub or from an Edge based gateway to alert persons in crisis.  For example, let’s consider this simple use case of a "Smart LED lightbulb” in school classrooms.  If an emergency has occurred at the school, triggered by sound sensors, AI/ML, or by central administration hub alerting, the system can light the bulb red.  This will give students more time to perform their safety drill to shelter in place.  If the emergency has happened near the school; The system lights the bulb yellow.  This is an indicator in the room that something has happened, the persons being notified are not in immediate danger and that help is on the way.  Parents, using a consumer app, connected to the School’s central administration hub can receive timely and secure communications from the authorities about the emergency. This can be expanded to multiple use cases including safe communities, safe cities, Safe work environments.

### 1.3 Core components 

Project Edison solution is based on the following core components.

*	**Mobile applications**

Admin mobile application is used for onboarding devices and associating metadata (latitude/longitude, building, floor, room, etc.) and ability to manage active crises.

Consumer Application (User Mobile application) allows administrators to send information to connected users in an area of crisis. Also, will allow consumers to provide additional information back to administrators about the crisis with future integration capability into popular social platforms.

*	**Admin Portal (Web Application)**

The solution will include an administrator portal which will serve as the main interaction point for the administrator to set rules, define & manage crisis events, and communicate with people in areas of crisis.

*	**Devices**

A **Smart Bulb** has the ability to change colors (Red/Yellow) to alert people during a crisis event.

**Sound Sensor** will pick up specified threshold sound to trigger an event.

**IoT Button** is used by user for pushing a button to signal a crisis event.

*	**Kubernetes**

Azure Kubernetes Service will host the microservices for event processing, storing data, sending notifications to mobile applications (User and Admin) and managing devices (SmartBulb, Sound Sensor, IoT Button).

*	**ACR (Azure Container Registry)**

Azure Container Registry is a private registry for hosting container images. Using the Azure Container Registry, you can store Docker-formatted images for all types of container deployments.

*	**IoT Hub**

IoT Hub receives the events from the Sound Sensor, Smart Bulb and IoT button or from the simulators, which will be pushed to Azure Service Bus and get populated on the Edison Admin Portal Application.

* **Cosmos DB**

Azure Cosmos DB is a NoSQL database service on Azure, that supports multiple ways of storing and processing data, used to store telemetry data collected from the devices and user activity.

*	**Azure Notification Hub**

Azure Notification Hubs provides a highly scalable, cross-platform push notification infrastructure that enables you to either broadcast push notifications to millions of users at once, or tailor notifications to individual users.


## 2.0 IoT Core Solution Architecture

### 2.1 Core Architecture

Below Diagram explains the Core logical and techinical architecture for Smart Buildings solution.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/g0.png)

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

### 2.2 Automated Deployment Configurations

As part of this solution accelerator, multiple deployment configurations are automated to enable accelerated time to market. From demos to pilots and production, these deployment configurations can be used to accelerate time to market and significantly reduce development and management costs. 

This solution is deployed through an ARM template. Infrastructure as a Code automation using ARM templates has been used to achieve 80-90% of end to end automation and at places, where automation is not feasible, detailed steps are provided to guide the user.

In addition, this solution consists:

*	Application Insights to monitor the Web Application and store the logs of the Web API, which will be helpful to trace the Web API working.

*	Geo-replication to provide high availability for Cosmos DB and used to set the recovery database as the primary database, in cases of failure of the primary database.

*	This solution also provides Disaster Recovery activities. IoT Hub manual failover is helpful to make the IoT Hub available in another region, when IoT Hub of one region is failed.

*	Traffic Manager delivers high availability for critical web applications by monitoring the endpoints and providing automatic failover when an endpoint goes down.


### 2.3 Deployment Configurations Tiers

#### 2.3.1 Basic Architecture

Basic solution will have all core components.In addition this solution also consists of monitoring components like Application Insights and OMS Log Analytics. 

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

*	2- Virtual Machine (Linux and Windows)

*	1- Service Bus

*	1- Signal R

*	1- Redis Cache

#### 2.3.2 Standard Architecture

Standard Architecture provides disaster recovery with secondary region, where the components that store state, (Cosmos DB, Blob Storage) are deployed in HA mode in the secondary region. Rest of the components can be deployed in the secondary region using provided automated templates in case of a disaster. The Recovery Time Objective (RTO) for this solution is higher than Basic solution in case of a region failover. This can be used for pre-production/pilot deployments or in production for use cases where few hours of RTO are acceptable.

1.	Primary Region (Deployment)

2.	Secondary Region (Re-deployment on instance of a disaster in the primary region.)

We have IoT Hub manual failover, Cosmos DB geo replication, Kubernetes HA  and redeployment components. The effect of these components will occur when primary Region goes down.

Once the secondary region is deployed after a disaster using the automated ARM templates, deployment admin would need to manually add the  Admin IP address and API IP address after running the ingresses as an endpoint to the Traffic Manager.

 
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

*	2- Traffic Manager Profiles

*   2- Virtual Machines (Linux and Windows) 

**When there is a Region failover, user needs to redeploy ARM Template provided in GIT Repo.**

When redeployment Completed Successfully, below Azure resources will be deployed. 

*	1- Log analytics

*	1- Storage Account

*	1- Notification Hub

*	1- Signal R

*	1- Redis Cache

*	1- Kubernetes

*	1- Azure Container Registry

*	1- App Insights

*	1- Service Bus

**Note**:  As explained above, the re-deployment of ARM Template would take around 30 minutes. After which, we’ll need to make the manual configuration for secondary region resources to Kubernetes and both Mobile applications

#### 2.3.3 Premium Architecture:

Premium Architecture provides a solution with disaster recovery, which includes full level deployment of solution as illustrated in the diagram, on the secondary region.

1.	Primary Region

2.	Secondary Region

This deployment includes synchronized database. However, only the primary region is actively handling network requests from the users. The secondary region becomes active only when the primary region experiences a service disruption. In that case, all new network requests are routed to the secondary region. Azure Traffic Manager can manage this failover automatically.

This topology provides a very low RTO. The secondary failover region must be ready to go immediately after failure of the primary region.

**IoT Hub manual failover feature:** 

Manual failover is a feature of the IoT Hub service that allows customers to failover their hub's operations from a primary region to the corresponding Azure geo-paired region,

Manual failover can be done in the event of a regional disaster or an extended service outage.

**Traffic Manager:** 

1.	Traffic Manager routes incoming requests to the primary region. If the application running that region becomes unavailable, Traffic Manager fails over to the secondary region.

2.	Traffic Manager automatically fails over, when the primary region becomes unavailable. When Traffic Manager fails over, there is a period when clients cannot reach the application.

**Cosmos DB:** 

Azure Cosmos DB is a globally distributed, low-latency, high throughput NoSQL database service. 

Azure Cosmos DB provides global distribution, which means you can scale and distribute it across different Azure regions. Global replication of your Azure Cosmos DB enables you to have your data replicated over as many as datacenters as you require, providing the control and access for your replicated data seamlessly.

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

*	2-Virtual Machine (1 Linux, 1 Windows)

*	2- Service Bus

### 2.4 Conventional Data Work Flow 

#### 2.4.1 Work flow with Simulator 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/g5.png)
 
#### 2.4.2 Work flow with Devices

![alt text](https://github.com/sysgain/Iot-ProjectEdison/blob/master/documents/Images/g6.png)

### 2.5 Azure Components Functionality

Microsoft Azure is a cloud computing service offering by Microsoft, for building, testing, deploying, and managing applications and services through a global network of Microsoft managed data centers. It provides software as a service (SaaS), platform as a service (PaaS) and infrastructure as a service (IaaS) and supports multiple programming languages, tools and frameworks, including both Microsoft-specific and third-party software and systems.

Microsoft lists over 600 Azure services, of which some are,

*	Compute

*	Storage services

*	Data management

*	Management

*	Machine learning

*	IoT

#### 2.5.1 IoT Hub

**Introduction**:

Azure IoT Hub is a fully managed service that enables reliable and secure bi-directional communications between millions of IoT devices and an application back end. 

Azure IoT Hub offers reliable device-to-cloud and cloud-to-device hyper-scale messaging, enables secure communications using per-device security credentials, access control and includes device libraries for the most popular languages and platforms. Before you can communicate with IoT Hub from a gateway you must create an IoT Hub instance in your Azure subscription and then provision your device in your IoT hub. Some samples in this repository require that you have a usable IoT Hub instance.

The Azure IoT Hub offers several services for connecting IoT devices with Azure services, processing incoming messages or sending messages to the devices. From a device perspective, the functionalities of the Azure IoT Hub enable simple and safe connection of IoT devices with Azure services by facilitating bidirectional communication between the devices and the Azure IoT Hub.


**Implementation**:

In this solution the IoT Hub receives the events from Sound Sensors, Smart Bulbs, IoT Buttons or from the simulators, which will be pushed to Azure service bus and get populated on the Edison Admin Portal Application.


#### 2.5.2 Notification Hub

**Introduction**:

Azure Notification Hubs provides a highly scalable, cross-platform push notification infrastructure that enables you to either broadcast push notifications to millions of users at once, or tailor notifications to individual users. You can use Notification Hubs with any connected mobile application, whether it’s built on Azure Virtual Machines, Cloud Services, Web Sites, or Mobile Services.

Azure Notification Hub is a push notification engine, designed to alert subscribers about new content for a given site, service or an application. Azure Notification Hubs are part of Microsoft Azure’s public cloud service offerings.


**Implementation**:

The Notification Hub pushes the Alerts (Activated Responses) and messages to the User Mobile application.

#### 2.5.3 Azure Active Directory

**Introduction**:

Microsoft Azure Active Directory (Azure AD) is a cloud service that provides administrators with the ability to manage end user identities and access privileges. The service gives administrators the freedom to choose which information stays in the cloud, who can manage or use the information, what services or applications can access the information and which end users can have access.

**Implementation**:

Azure Active directory is used to authenticate users to login to Admin Portal. Azure active Directory enables secure authentications to the Admin Portal.

#### 2.5.4 Azure Automation

**Introduction**:

Azure Automation enables the users to automate the tasks, which are manual and repetitive in nature by using Runbooks. 

Runbooks in Azure Automation are based on Windows PowerShell or Windows PowerShell Workflow. We can code and implement the logic, requires automation, using PowerShell.

**Implementation**:

In this Solution Azure run books are used to create Database and collections in Document DB, it is also used to update reply URLs in Active Directory Application.

#### 2.5.5 Cosmos DB  

**Introduction**:

Azure Cosmos DB is a NoSQL database service, from Azure, that supports multiple ways of storing and processing data. As such, it is classified as a multi-model database. In multi-model databases, various database engines are natively supported and accessible via common APIs.

**Implementation**:

In this Solution, Cosmos DB have Templates, Messages and Groups Collections. The Messages collections will get updated with the telemetry data of the Device.

#### 2.5.6 OMS Log Analytics

**Introduction**:

Azure Operations Management Suite (OMS), previously known as Azure Operational Insights, is a software as a service platform that allows an administrator to manage on-premise and cloud IT assets from a single console.

Azure OMS handles log analytics, IT automation, backup, recovery, security and compliance tasks.

Log analytics will collect and store your data from various log sources and allow you to query over them using a custom query language.

**Implementation**:

Log analytics provides monitoring for Cosmos DB, IoT Hub, Kubernetes, Redis Cache, Service Bus. Log analytics store the logs, which will be helpful to trace the working of these resources and provides detailed insights using solutions.

#### 2.5.7 Storage Account

**Introduction**:

Azure Storage Account contains all your data objects, blobs, files, queues, tables, and disks. Data in your Storage Account is durable, highly available, secure, massively scalable, and accessible from anywhere in the world over HTTP or HTTPS.

**Implementation**:

The older events (actions and telemetry) will be stored in Blob Storage/Table storage for auditing purposes.

### 2.5.8 Azure Kubernetes

**Introduction**:

Kubernetes provides a container-centric management environment. It orchestrates computing, networking and storage infrastructure on behalf of user workloads. It can group containers that make up an application, into logical units for easy management and discovery.  

**Implementation**:

Azure Kubernetes Service will host the microservices for event processing, storing, sending notifications to mobile applications and devices.

#### 2.5.9 Azure Container Registry

**Introduction**:

Azure Container Registry is a private registry for hosting container images. Using the Azure Container Registry, you can store Docker-formatted images for all types of container deployments. Azure Container Registry integrates well with orchestrators, hosted in Azure Container Service, including Docker Swarm, DC/OS, and Kubernetes. Users can benefit from using familiar tool capable of working with the open source Docker Registry v2.

**Implementation**:

Azure Container Registry controls image names for all container deployments, use Kubernetes commands to push or pull an image from a repository. In addition to container images, Azure Container Registry stores images used to deploy applications to Kubernetes.

#### 2.5.10 Virtual Machine

**Introduction**:

Virtual Machines gives you the flexibility of Virtualization on a wide range of computing solutions with support for Linux, Windows Server, SQL Server, Oracle, IBM, SAP and more. 

Each virtual machine has its own virtual hardware, including CPUs, memory, hard drives, network interfaces and other devices. The virtual hardware is then mapped to the real hardware on the physical machine which saves costs by reducing the need for physical hardware systems along with the associated maintenance costs that go with it, plus reduces power and cooling demand.

**Implementation**:

Execute scripts which updates the configurations, environments of Microservices by building the docker images and pushing them to Azure Container Registry, installs helm, ingress controllers making it responsible to access Admin Portal.

#### 2.5.11 Service Bus

**Introduction**:

Microsoft Azure Service Bus is a fully managed enterprise integration message broker. Service Bus is most commonly used to decouple applications and services from each other and is a reliable and secure platform for asynchronous data and state transfer. Data is transferred between services using messages. A message is in binary format, which can contain JSON, XML, or just text.

**Implementation**:

The Event messages which got triggered from the Devices are sent to IoT Hub and Service Bus pulls the data from IoTHub and sends to the services deployed in the Kubernetes.

#### 2.5.12 Signal R

**Introduction**:

Azure SignalR Service simplifies the process of adding real-time web functionality to applications over HTTP. This real-time functionality allows the service to push content updates to connected clients, such as a single page web or mobile application. As a result, clients are updated without the need to poll the server or submit new HTTP requests for updates.

**Implementation**:

The notification and alerts from Edison Admin Portal are pushed to User Mobile application and to the devices.

#### 2.5.13 Redis Cache

**Introduction**:

Azure Cache for Redis is an in-memory data structure store. It is typically used as a cache to improve the performance and scalability of systems that rely heavily on backend data-stores. It can also be used as distributed non-relational database, and message broker. 

**Implementation**:

Azure Service Bus messages which are pulled from IoT Hub are being sent to Redis Cache, which will also get populated in the Cosmos DB’s collections.

#### 2.5.14 Azure Bot:

**Introduction**:

Azure Bot Service provides tools to build, test, deploy, and manage intelligent bots all in one place. Your Bot can be hosted wherever you want, registered with the Azure Bot Service. Build, connect, and manage Bots to interact with your users wherever they are - from your app or website to Cortana, Skype, Messenger and many other services.

**Implementation**:

Azure Bot is used for communication between User Mobile Application and Edison Admin Portal Application, for sending messages, activities and notifications.

#### 2.5.15 Application Insights

**Introduction**:

Application Insights is an extensible Application Performance Management (APM) service for web developers on multiple platforms. Use it to monitor live web application. It will automatically detect performance anomalies. It includes powerful analytics tools to help diagnose issues and to understand what users do with web application.

Application Insights monitor below:

*	Request rates, response times, and failure rates

*	Dependency rates, response times, and failure rates

*	Exceptions 

*	Page views and load performance

*	AJAX calls

*	User and session counts

*	Performance counters

*	Host diagnostics from Docker or Azure

*	Diagnostic trace logs

*	Custom events and metrics

**Implementation**:

We are implementing Application Insights monitoring for Azure Service Bus, Services in the Kubernetes Container, Notification hub and Azure bot.

## 3.0 Solution Types and Cost Mechanism

### 3.1 Solutions and Associated Costs

The Automated solutions provided by us covers in below Section. Will have the following Cost associated. The solutions are created considering users requirements & have Cost effective measures. User have control on what Type of Azure resources need to be deploy with respect to SKU And Cost. These options will let user to choose whether user wants to deploy Azure resources with minimal SKU and Production ready SKU. The Cost Models per solutions are explained in next sections.

#### 3.1.1 Basic

The Basic solution requires minimum Azure components with minimal available SKU’s. This Solution provides (Core + Monitoring) features such as Application Insights & OMS Log Analytics. The details on components used in this solution is listed in Section: 

 The Estimated Monthly Azure cost is: **$282.95**

**Note:** *Refer below table for the optional component list & Features*

**Pricing Model for Basic**:

Prices are calculated by considering Location as **East US** and Pricing Model as **“PAYG”**.


| **Resource Name**                   | **Size**                    | **Azure Cost/month**                                                                                   
| -------------                     | -------------                  | --------------------                                                                    
| **Application Insights**       | Basic,1 GB*$2.30. First 5GB free per month         | $2.30  
| **Notification Hub**   | Free Includes 1 million pushes for 500 active devices.      | $0.00  
| **IoT Hub**        | S1, Unlimited devices, 1 Unit-$25.00/per month 400,000 messages/day                    | $25.00    
| **Storage Account**        | Block Blob Storage, General Purpose V2, LRS Redundancy, Hot Access Tier, 10 GB Capacity, 100,000 Write operations, 100,000 List and Create Container Operations, 100,000 Read operations, 0 Other operations. 10 GB Data Retrieval, 0 GB Data Write       | $1.25   
| **Azure Cosmos DB**       | Standard, throughput 400 RU/s (Request Units per second) 4x100 Rus(Throughput)- $23.36 10 GB storage – $2.50         | $25.86  
| **Log Analytics**   | 1 VMs monitored, 5 GB average log size, 0 additional days of data retention   | $0.00  
| **Azure Automation Account**   | Capability: Process Automation 500 minutes of process automation and 744 hours of watchers are free each month.    | $0.00
| **Azure Container Registry**   | Basic Tier, 1 units x 30 days, 5 GB Bandwidth     | $5.00
| **Kubernetes**   | 3 * [B2S (2 vCPU(s), 4 GB RAM) nodes x 730 Hours; managed OS disks x P4 ($14.40)]      | $106.94
| **Service Bus**   | Standard tier: 10, 1,000 brokered connection(s), 0 Hybrid Connect listener(s) + 0 overage per GB, 0 relay hour(s), 10 relay message(s)      | $9.82
| **Virtual Machine**   | standard_A2 (2 vCPU(s), 3.5 GB RAM) x 730 Hours; 2 Linux – Ubuntu managed OS disks – S4      | $90.72
| **Signal R**   | Free Tier, Includes 1 unit with 20 concurrent connections per unit and 20,000 messages per unit/day      | $0.00
| **Redis Cache**   | C0: Basic tier, 1 instance(s), 730 Hours      | $16.06
| **Bot**   | Free Tier  Standard Channels- Unlimited messages included.  Premium Channels- 10,000 messages included.      | $0.00
|        |         |    **Total Cost/Month**     | **$282.95** 


#### 3.1.2 Standard

This Solution provides (Core + Monitoring +Hardening) features such as application Insights, OMS Log Analytics, High Availability & Disaster recovery. The details on components used in this solution is listed in Section: 

The Estimated Monthly Azure cost is: **$367.86**, Adding the optional resources, the cost estimated is: **$369.66**

**Note**: *Refer below table for the optional component list & Features*

**Pricing Model for Standard Solution**:

Prices are calculated by Location as **East US** and Pricing Model as **“PAYG”**.


| **Resource Name**               | **Size**              | **Azure Cost/month**                                                                        
| -------------              | -------------                  | --------------------                                                                                                  
| **Application Insights**       | Basic, 1GB * $2.30 First 5GB free per month          | $2.30  
| **Notification Hub**   | Free Includes 1 million pushes for 500 active devices.     | $0.00  
| **IoT Hub**        | S1, Unlimited devices, 1 Unit-$25.00/per month 400,000 messages/day                    | $25.00    
| **Storage Account**        | Block Blob Storage, General Purpose V2, LRS Redundancy, Hot Access Tier, 10 GB Capacity, 100,000 Write operations, 100,000 List and Create Container Operations, 100,000 Read operations, 0 Other operations. 10 GB Data Retrieval, 0 GB Data Write       | $1.25   
| **Azure Cosmos DB**       | Standard, throughput 400 RU/s (Request Units per second) 4x100 Rus(Throughput)- $23.36 10 GB storage – $2.50         | $25.86  
| **Log Analytics**   | 1 VMs monitored, 5 GB average log size, 0 additional days of data retention   | $0.00  
| **Azure Automation Account**   | Capability: Process Automation 500 minutes of process automation and 744 hours of watchers are free each month.    | $0.00
| **Azure Container Registry**   | Standard Tier, 1 units x 30 days, 0 GB Bandwidth , 5GB extra storage     | $20.45
| **Kubernetes**   | 3B2S (2 vCPU(s), 4 GB RAM) nodes x 730 Hours; 3 managed OS disks with 30 GB * $4.80-P4     | $106.94
| **Service Bus**   | Standard tier: 10, 1,000 brokered connection(s), 0 Hybrid Connect listener(s) + 0 overage per GB, 0 relay hour(s), 10 relay message(s)      | $9.82
| **Virtual Machine**   | standard_A2 (2 vCPU(s), 3.5 GB RAM) x 730 Hours; 2 Linux – Ubuntu managed OS disks – S4      | $90.72
| **Signal R**   |Standard Tier, Includes 1,000 concurrent connections per unit and 10,00,000 messages per unit/day. Maximum of 100 units can be configured      | $48.97
| **Redis Cache**   | C0: Standard tier, 1 instance(s), 730 Hoursx $40.15     | $40.15
| **Bot**   | Standard Tier  Standard Channels- Unlimited messages included.  Premium Channels- 10,000 messages x $0.50 included.      | $0.50
| **Traffic Manager**   | 2 * DNS Query $0.54 + Azure Endpoint $0.36       | $1.80
|        |         |    **Estimated monthly cost (With Traffic Manager)**     | **$369.66** 
|        |         |    **Estimated monthly cost (Without Traffic Manager)**     | **$367.86** 

**Note: When we redeploy the solution, there will not be any extra cost, since primary region is already paid.** 

#### 3.1.3 Premium 

This solution also provides (Core + Monitoring +Hardening), the difference between Standard & Premium solution is under Premium - Both the regions can be deployed at same time, however this is not possible under standard solution. The details on components used in this solution is listed in Section: 

 The Estimated Monthly Azure cost is: **$ 600.59** and including the traffic manager, the cost estimated cost would be **$602.39**

**Pricing Model for Premium Solution:**

Prices are calculated by Considering Location as **East US** and Pricing Model as **“PAYG”**.


| **Resource Name**               | **Size**                | **Azure Cost/month**                                                                                         
| -------------                  | -------------                 | --------------------                                                                                                      
| **Application Insights**       | 2 * Basic, 1GB * $2.30 First 5GB free per month          | $4.60  
| **Notification Hub**   | 2 * (Free Includes 1 million pushes for 500 active devices.)     | $0.00  
| **IoT Hub**        | S1, Unlimited devices, 1 Unit-$25.00/per month 400,000 messages/day                    | $25.00    
| **Storage Account**        | Block Blob Storage, General Purpose V2, LRS Redundancy, Hot Access Tier, 10 GB Capacity, 100,000 Write operations, 100,000 List and Create Container Operations, 100,000 Read operations, 0 Other operations. 10 GB Data Retrieval, 0 GB Data Write       | $1.25   
| **Azure Cosmos DB**       | Standard, throughput 400 RU/s (Request Units per second) 4x100 Rus(Throughput)- $23.36 10 GB storage – $2.50         | $25.86  
| **Log Analytics**   | 1 VMs monitored, 5 GB average log size, 0 additional days of data retention   | $0.00  
| **Azure Automation Account**   | Capability: Process Automation 500 minutes of process automation and 744 hours of watchers are free each month.    | $0.00
| **Azure Container Registry**   | 2 * [Standard Tier, 1 units x 30 days, 0 GB Bandwidth, 5 GB Extra Storage]    | $40.90
| **Kubernetes**   | 2 * [3B2S (2 vCPU(s), 4 GB RAM) nodes x 730 Hours; 3managed OS disks with 30 GB x $4.80 – P4]      | $213.88
| **Service Bus**   | 2 * (Standard tier: 10, 1,000 brokered connection(s), 0 Hybrid Connect listener(s) + 0 overage per GB, 0 relay hour(s), 10 relay message(s))      | $19.64
| **Virtual Machine**   | standard_A2 (2 vCPU(s), 3.5 GB RAM) x 730 Hours; 2 Linux – Ubuntu managed OS disks – S4      | $90.72
| **Signal R**   | 2* [Standard Tier, Includes 1,000 concurrent connections per unit and 10,00,000 messages per unit/day. Maximum of 100 units can be configured- $48.97]      | $97.94
| **Redis Cache**   | 2 * [ C0: Standard tier, 1 instance(s), 730 Hours X $40.15]      | $80.30
| **Bot**   | Standard Tier  Standard Channels- Unlimited messages included.  Premium Channels- 10,000 messages included.      | $0.50
| **Traffic Manager**   | 2 * DNS Query $0.54 + Azure Endpoint $0.36       | $1.80
|        |         |    **Estimated monthly cost (With Traffic Manager)**     | **$602.39‬** 
|        |         |    **Estimated monthly cost (Without Traffic Manager)**     | **$600.59** 

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
| **Application Insights**                  | $2.30          | $2.30          | $4.60
| **Notification Hub**                  | $0.00          | $0.00          | $0.00
| **IoT Hub**                      | $25.00           | $25.00	          | $25.00
| **Storage Account**           | $1.25	         | $1.25	          | $1.25
| **Azure Cosmos DB**              | $25.86	         | $25.86	          | $25.86
| **Log Analytics**              | $0.00	         | $0.00	          | $0.00
| **Automation Account**   | $0.00	         | $0.00	          | $0.00
| **Traffic Manager**              | NA	         | $1.80           | $1.80
| **Notification Hub**              | $0.00	         | $0.00           | $0.00
| **Kubernetes**              | $106.94	         | $106.94           | $213.88
| **Azure Container Registry**              | $5.00	         | $20.45           | $40.90
| **Service Bus**              | $9.82	         | $9.82           | $19.64
| **Virtual Machine**              | $90.72	         | $90.72           | $90.72
| **Signal R**              | $0.00	         | $48.97           | $97.94
| **Redis Cache**              | $16.06	         | $40.15           | $80.3
| **Bot**              | $0.00	         | $0.50           | $0.50

#### 3.2.3 Estimated Monthly Cost for each Solution:

| **Resource Name**           | **Basic**                    | **Standard**                 | **Premium**                                                                                                                
| -------------              | ------------------------       | --------------------      | ------------ 
| **Estimated monthly cost (With Traffic Manager)**            | **NA**            | **$369.66**  	             | **$602.39**
| **Estimated monthly cost (Without Traffic Manager)**            | **$282.95**            | **$367.86**  	             | **$600.59**

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
