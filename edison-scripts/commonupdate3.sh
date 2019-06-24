#!/bin/bash
#Comment - Installs the required packages for building images
#Author - Vivek

#------------------------------------
CosmosDbSRT=`head -10 input.txt | awk -F "\"" '{print $2}'| tail -1`
AzureServiceBusCONN=`head -11 input.txt | awk -F "\"" '{print $2}'| tail -1`
ServiceBusRabbitMQUSR=`head -12 input.txt | awk -F "\"" '{print $2}'| tail -1`
ServiceBusRabbitMQPWD=`head -13 input.txt | awk -F "\"" '{print $2}'| tail -1`
AzureAdSRT=`head -14 input.txt | awk -F "\"" '{print $2}'| tail -1`
SignalCONN=`head -15 input.txt | awk -F "\"" '{print $2}'| tail -1`
IoTHubControllerSRT=`head -16 input.txt | awk -F "\"" '{print $2}'| tail -1`
NotificationHubSRT=`head -17 input.txt | awk -F "\"" '{print $2}'| tail -1`
ApplicationInsightsKey=`head -18 input.txt | awk -F "\"" '{print $2}'| tail -1`
BotAppPassword=`head -19 input.txt | awk -F "\"" '{print $2}'| tail -1`
BOTSECRETTOKEN=`head -26 input.txt | awk -F "\"" '{print $2}'| tail -1`
TuserName=`head -21 input.txt | awk -F "\"" '{print $2}'| tail -1`
Tpassword=`head -22 input.txt | awk -F "\"" '{print $2}'| tail -1`
TauthToken=`head -23 input.txt | awk -F "\"" '{print $2}'| tail -1`

#------------------------------------
export CosmosDbSRT
export AzureServiceBusCONN
export ServiceBusRabbitMQUSR
export ServiceBusRabbitMQPWD
export AzureAdSRT
export SignalCONN
export IoTHubControllerSRT
export NotificationHubSRT
export ApplicationInsightsKey
export BotAppPassword
export BOTSECRETTOKEN
export TuserName
export Tpassword
export TauthToken
#------------------------------------
GITPATH=`pwd`
GIT_DIRPATH="$GITPATH/ProjectEdison"
GIT_DIRCONFIGPATH="$GIT_DIRPATH/Edison.Web/Kubernetes/qa/Config/Secrets"
LOG="/tmp/common.log.`date +%d%m%Y_%T`"
#------------------------------------
echo $AzureServiceBusCONN
ls $GIT_DIRPATH
if [ $? -eq 0 ]
then
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH exists" >> $LOG
        cd $GIT_DIRCONFIGPATH
        cat common.json | jq '.CosmosDb.AuthKey=env.CosmosDbSRT' | jq '.AzureServiceBus.ConnectionString=env.AzureServiceBusCONN' | jq '.ServiceBusRabbitMQ.Username=env.ServiceBusRabbitMQUSR' | jq '.ServiceBusRabbitMQ.Password=env.ServiceBusRabbitMQPWD' | jq '.RestService.AzureAd.ClientSecret=env.AzureAdSRT' | jq '.SignalR.ConnectionString=env.SignalCONN' | jq '.IoTHubController.IoTHubConnectionString=env.IoTHubControllerSRT' | jq '.NotificationHub.ConnectionString=env.NotificationHubSRT' | jq '.ApplicationInsights.InstrumentationKey=env.ApplicationInsightsKey' | jq '.Bot.MicrosoftAppPassword=env.BotAppPassword' | jq '.Bot.RestService.SecretToken=env.BOTSECRETTOKEN' | jq '.Twilio.UserName=env.TuserName' | jq '.Twilio.Password=env.Tpassword' | jq '.Twilio.AuthToken=env.TauthToken' > $GIT_DIRCONFIGPATH/common.secrets
else
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH doesn't exists" >> $LOG
fi
