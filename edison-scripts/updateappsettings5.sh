#!/bin/bash
#Comment - Updates the appsettings file for Edisonweb services code
#------------------------------------
COSMOSDBENDPOINT=`head -6 input.txt | awk -F "\"" '{print $2}'| tail -1`
COSMOSDBKEY=`head -10 input.txt | awk -F "\"" '{print $2}'| tail -1`
ADCLIENTID=`head -1 input.txt | awk -F "\"" '{print $2}'`
ADSECRET=`head -14 input.txt | awk -F "\"" '{print $2}'| tail -1`
ADDOMAIN=`head -25 input.txt | awk -F "\"" '{print $2}'| tail -1`
ADTENANTID=`head -2 input.txt | awk -F "\"" '{print $2}'| tail -1`
B2CCLIENTID=`head -3 input.txt | awk -F "\"" '{print $2}'| tail -1`
B2CDOMAIN=`head -4 input.txt | awk -F "\"" '{print $2}'| tail -1`
B2CSIGNINPOLICY=`head -5 input.txt | awk -F "\"" '{print $2}'| tail -1`
NOTIFICATIONHUBNAME=`head -7 input.txt | awk -F "\"" '{print $2}'| tail -1`
NOTIFICATIONCONNSTRING=`head -17 input.txt | awk -F "\"" '{print $2}'| tail -1`
SIGNALLRCONNSTRING=`head -15 input.txt | awk -F "\"" '{print $2}'| tail -1`
RABBITMQUSER=`head -12 input.txt | awk -F "\"" '{print $2}'| tail -1`
RABBITMQPWD=`head -13 input.txt | awk -F "\"" '{print $2}'| tail -1`
SERVICEBUSCONNSTRING=`head -11 input.txt | awk -F "\"" '{print $2}'| tail -1`
TWILIOUSER=`head -21 input.txt | awk -F "\"" '{print $2}'| tail -1`
TWILIOPWD=`head -22 input.txt | awk -F "\"" '{print $2}'| tail -1`
TWILIOACCID=`head -8 input.txt | awk -F "\"" '{print $2}'| tail -1`
TWILIOAUTHTOKEN=`head -23 input.txt | awk -F "\"" '{print $2}'| tail -1`
MSAPPID=`head -9 input.txt | awk -F "\"" '{print $2}'| tail -1`
MSAPPPWD=`head -19 input.txt | awk -F "\"" '{print $2}'| tail -1`
IOTHUBCONN=`head -16 input.txt | awk -F "\"" '{print $2}'| tail -1`
BOTSECRETTOKEN=`head -26 input.txt | awk -F "\"" '{print $2}'| tail -1`
#------------------------------------
GITPATH=`pwd`
GIT_DIRPATH="$GITPATH/ProjectEdison"
GIT_DIRCONFIGPATH="$GIT_DIRPATH/Edison.Web"
APPSET="appsettings.json"
APPSETDEV="appsettings.Development.json"
LOG="/tmp/appsettings.log.`date +%d%m%Y_%T`"
#------------------------------------
export COSMOSDBENDPOINT
export ADCLIENTID
export ADSECRET
export ADDOMAIN
export ADTENANTID
export B2CCLIENTID
export B2CDOMAIN
export B2CSIGNINPOLICY
export NOTIFICATIONHUBNAME
export TWILIOACCID
export SIGNALLRCONNSTRING
export COSMOSDBKEY 
export RABBITMQUSER 
export RABBITMQPWD 
export SERVICEBUSCONNSTRING 
export NOTIFICATIONCONNSTRING 
export TWILIOUSER 
export TWILIOPWD 
export TWILIOAUTHTOKEN
export MSAPPID
export MSAPPPWD
export IOTHUBCONN
export BOTSECRETTOKEN
#------------------------------------
apt install jq -y

ls $GIT_DIRPATH
if [ $? -eq 0 ]
then
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH exists" >> $LOG
        cd $GIT_DIRCONFIGPATH
        echo "------------------------------------" >> $LOG
        echo "Updating $APPSET with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Api/$APPSET $GIT_DIRCONFIGPATH/Edison.Api/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Api/$APPSET.bak | jq '.ServiceBusRabbitMQ.Username=env.RABBITMQUSER' | jq '.ServiceBusRabbitMQ.Password=env.RABBITMQPWD' | jq '.ServiceBusAzure.ConnectionString=env.SERVICEBUSCONNSTRING' | jq '.CosmosDb.AuthKey=env.COSMOSDBKEY' | jq '.CosmosDb.Endpoint=env.COSMOSDBENDPOINT' | jq '.SignalR.ConnectionString=env.SIGNALLRCONNSTRING' | jq '.Twilio.UserName=env.TWILIOUSER' | jq '.Twilio.Password=env.TWILIOPWD' | jq '.AzureAd.ClientId=env.ADCLIENTID' | jq '.AzureAd.Domain=env.ADDOMAIN' | jq '.AzureAd.TenantId=env.ADTENANTID' | jq '.AzureAdB2CWeb.ClientId=env.B2CCLIENTID' | jq '.AzureAdB2CWeb.Domain=env.B2CDOMAIN' | jq '.AzureAdB2CWeb.SignUpSignInPolicyId=env.B2CSIGNINPOLICY' | jq '.NotificationHub.PathName=env.NOTIFICATIONHUBNAME' |jq '.NotificationHub.ConnectionString=env.NOTIFICATIONCONNSTRING' | jq '.Twilio.AuthToken=env.TWILIOAUTHTOKEN' | jq '.Twilio.AccountSID=env.TWILIOACCID' > $GIT_DIRCONFIGPATH/Edison.Api/$APPSET
        echo "Updating $APPSETDEV with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Api/$APPSETDEV $GIT_DIRCONFIGPATH/Edison.Api/$APPSETDEV.bak
        cat $GIT_DIRCONFIGPATH/Edison.Api/$APPSETDEV.bak | jq '.SignalR.ConnectionString=env.SIGNALLRCONNSTRING' > $GIT_DIRCONFIGPATH/Edison.Api/$APPSETDEV
        echo "------------------------------------" >> $LOG 
        echo "Updating ChatService appsettings with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Microservices.ChatService/$APPSET $GIT_DIRCONFIGPATH/Edison.Microservices.ChatService/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Microservices.ChatService/$APPSET.bak | jq '.ServiceBusRabbitMQ.Username=env.RABBITMQUSER' | jq '.ServiceBusRabbitMQ.Password=env.RABBITMQPWD' | jq '.ServiceBusAzure.ConnectionString=env.SERVICEBUSCONNSTRING' | jq '.Bot.MicrosoftAppId=env.MSAPPID' | jq '.Bot.MicrosoftAppPassword=env.MSAPPPWD' | jq '.Bot.RestService.SecretToken=env.BOTSECRETTOKEN' | jq '.CosmosDb.AuthKey=env.COSMOSDBKEY' | jq '.CosmosDb.Endpoint=env.COSMOSDBENDPOINT' | jq '.AzureAdB2CWeb.ClientId=env.B2CCLIENTID' | jq '.AzureAdB2CWeb.Domain=env.B2CDOMAIN' | jq '.AzureAdB2CWeb.SignUpSignInPolicyId=env.B2CSIGNINPOLICY' | jq '.AzureAd.ClientId=env.ADCLIENTID' | jq '.AzureAd.TenantId=env.ADTENANTID' | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID'| jq '.RestService.AzureAd.ClientSecret=env.ADSECRET' | jq '.RestService.AzureAd.TenantId=env.ADTENANTID' > $GIT_DIRCONFIGPATH/Edison.Microservices.ChatService/$APPSET  
        echo "------------------------------------" >> $LOG 
        echo "Updating DeviceProvisioning appsettings with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Microservices.DeviceProvisioning/$APPSET $GIT_DIRCONFIGPATH/Edison.Microservices.DeviceProvisioning/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Microservices.DeviceProvisioning/$APPSET.bak | jq '.AzureAd.ClientId=env.ADCLIENTID' | jq '.AzureAd.TenantId=env.ADTENANTID' | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.ClientSecret=env.ADSECRET' | jq '.RestService.AzureAd.TenantId=env.ADTENANTID' > $GIT_DIRCONFIGPATH/Edison.Microservices.DeviceProvisioning/$APPSET
        echo "------------------------------------" >> $LOG 
        echo "Updating DeviceSynchronizationService appsettings with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Microservices.DeviceSynchronizationService/$APPSET $GIT_DIRCONFIGPATH/Edison.Microservices.DeviceSynchronizationService/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Microservices.DeviceSynchronizationService/$APPSET.bak | jq '.ServiceBusRabbitMQ.Username=env.RABBITMQUSER' | jq '.ServiceBusRabbitMQ.Password=env.RABBITMQPWD' | jq '.RestService.AzureAd.TenantId=env.ADTENANTID' | jq '.ServiceBusAzure.ConnectionString=env.SERVICEBUSCONNSTRING' | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.ClientSecret=env.ADSECRET' > $GIT_DIRCONFIGPATH/Edison.Microservices.DeviceSynchronizationService/$APPSET
        echo "------------------------------------" >> $LOG  
        echo "Updating EventProcessorService appsettings with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Microservices.EventProcessorService/$APPSET $GIT_DIRCONFIGPATH/Edison.Microservices.EventProcessorService/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Microservices.EventProcessorService/$APPSET.bak | jq '.ServiceBusRabbitMQ.Username=env.RABBITMQUSER' | jq '.ServiceBusRabbitMQ.Password=env.RABBITMQPWD' | jq '.RestService.AzureAd.TenantId=env.ADTENANTID' | jq '.ServiceBusAzure.ConnectionString=env.SERVICEBUSCONNSTRING' | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.ClientSecret=env.ADSECRET' > $GIT_DIRCONFIGPATH/Edison.Microservices.EventProcessorService/$APPSET  
        echo "------------------------------------" >> $LOG  
        echo "Updating IoTHubControllerService appsettings with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Microservices.IoTHubControllerService/$APPSET $GIT_DIRCONFIGPATH/Edison.Microservices.IoTHubControllerService/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Microservices.IoTHubControllerService/$APPSET.bak | jq '.ServiceBusRabbitMQ.Username=env.RABBITMQUSER' | jq '.ServiceBusRabbitMQ.Password=env.RABBITMQPWD' | jq '.ServiceBusAzure.ConnectionString=env.SERVICEBUSCONNSTRING' | jq '.IoTHubController.IoTHubConnectionString=env.IOTHUBCONN' > $GIT_DIRCONFIGPATH/Edison.Microservices.IoTHubControllerService/$APPSET
        echo "------------------------------------" >> $LOG  
        echo "Updating MessageDispatcherService appsettings with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Microservices.MessageDispatcherService/$APPSET $GIT_DIRCONFIGPATH/Edison.Microservices.MessageDispatcherService/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Microservices.MessageDispatcherService/$APPSET.bak | jq '.ServiceBusRabbitMQ.Username=env.RABBITMQUSER' | jq '.ServiceBusRabbitMQ.Password=env.RABBITMQPWD' | jq '.ServiceBusAzure.ConnectionString=env.SERVICEBUSCONNSTRING' > $GIT_DIRCONFIGPATH/Edison.Microservices.MessageDispatcherService/$APPSET
        echo "------------------------------------" >> $LOG  
        echo "Updating NotificationHubService appsettings with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Microservices.NotificationHubService/$APPSET $GIT_DIRCONFIGPATH/Edison.Microservices.NotificationHubService/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Microservices.NotificationHubService/$APPSET.bak | jq '.ServiceBusRabbitMQ.Username=env.RABBITMQUSER' | jq '.ServiceBusRabbitMQ.Password=env.RABBITMQPWD' | jq '.ServiceBusAzure.ConnectionString=env.SERVICEBUSCONNSTRING' | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.ClientSecret=env.ADSECRET' | jq '.RestService.AzureAd.TenantId=env.ADTENANTID' > $GIT_DIRCONFIGPATH/Edison.Microservices.NotificationHubService/$APPSET
        echo "------------------------------------" >> $LOG  
        echo "Updating ResponseService appsettings with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Microservices.ResponseService/$APPSET $GIT_DIRCONFIGPATH/Edison.Microservices.ResponseService/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Microservices.ResponseService/$APPSET.bak | jq '.ServiceBusRabbitMQ.Username=env.RABBITMQUSER' | jq '.ServiceBusRabbitMQ.Password=env.RABBITMQPWD' | jq '.ServiceBusAzure.ConnectionString=env.SERVICEBUSCONNSTRING' | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.ClientSecret=env.ADSECRET' | jq '.RestService.AzureAd.TenantId=env.ADTENANTID' > $GIT_DIRCONFIGPATH/Edison.Microservices.ResponseService/$APPSET
        echo "------------------------------------" >> $LOG  
        echo "Updating SignalRService appsettings with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Microservices.SignalRService/$APPSET $GIT_DIRCONFIGPATH/Edison.Microservices.SignalRService/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Microservices.SignalRService/$APPSET.bak | jq '.ServiceBusRabbitMQ.Username=env.RABBITMQUSER' | jq '.ServiceBusRabbitMQ.Password=env.RABBITMQPWD' | jq '.ServiceBusAzure.ConnectionString=env.SERVICEBUSCONNSTRING' | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.ClientSecret=env.ADSECRET' | jq '.RestService.AzureAd.TenantId=env.ADTENANTID' > $GIT_DIRCONFIGPATH/Edison.Microservices.SignalRService/$APPSET
        echo "------------------------------------" >> $LOG  
        echo "Updating Workflows appsettings with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/Edison.Workflows/$APPSET $GIT_DIRCONFIGPATH/Edison.Workflows/$APPSET.bak
        cat $GIT_DIRCONFIGPATH/Edison.Workflows/$APPSET.bak | jq '.ServiceBusRabbitMQ.Username=env.RABBITMQUSER' | jq '.ServiceBusRabbitMQ.Password=env.RABBITMQPWD' | jq '.ServiceBusAzure.ConnectionString=env.SERVICEBUSCONNSTRING' | jq '.CosmosDb.AuthKey=env.COSMOSDBKEY' | jq '.CosmosDb.Endpoint=env.COSMOSDBENDPOINT' > $GIT_DIRCONFIGPATH/Edison.Workflows/$APPSET
else
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH doesn't exists" >> $LOG
fi
