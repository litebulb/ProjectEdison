#!/bin/bash
#Comment - Installs the required packages for building images
#Author - Vivek


#------------------------------------
ADCLIENTID=`head -1 input.txt | awk -F "\"" '{print $2}'`
TENANTID=`head -2 input.txt | awk -F "\"" '{print $2}'| tail -1`
B2CCLIENTID=`head -3 input.txt | awk -F "\"" '{print $2}'| tail -1`
DOMAIN=`head -4 input.txt | awk -F "\"" '{print $2}'| tail -1`
SIGNUPSIGNINPOLICYID=`head -5 input.txt | awk -F "\"" '{print $2}'| tail -1`
COSMOSDBEP=`head -6 input.txt | awk -F "\"" '{print $2}'| tail -1`
NOTIFICATIONHUBPATH=`head -7 input.txt | awk -F "\"" '{print $2}'| tail -1`
TWILIOACCID=`head -8 input.txt | awk -F "\"" '{print $2}'| tail -1`
MSAPPID=`head -9 input.txt | awk -F "\"" '{print $2}'| tail -1`
sudo apt install jq -y
#------------------------------------
GITPATH=`pwd`
GIT_DIRPATH="$GITPATH/ProjectEdison"
GIT_DIRCONFIGPATH="$GIT_DIRPATH/Edison.Web/Kubernetes/qa/Config"
CONFIGAPI="config-edisonapi.json"
CONFIGADMINPORTAL="config-edisonadminportal.json"
CONFIGCHATSERVICE="config-edisonchatservice.json"
CONFIGDEVICEPROVISING="config-edisondeviceprovisioning.json"
CONFIGEVENTPROCESSINGSERVICE="config-edisoneventprocessorservice.json"
CONFIGIOTHUBCONTROLLERSERVICE="config-edisoniothubcontrollerservice.json"
CONFIGMESSAGEDISPATCHERSERVICE="config-edisonmessagedispatcherservice.json"
CONFIGRESPONSESERVICE="config-edisonresponseservice.json"
CONFIGSIGNALRSERVICE="config-edisonsignalrservice.json"
CONFIGWORKFLOWS="config-edisonworkflows.json"
CONFIGDEVICESYNCHRONIZATIONSERVICE="config-edisondevicesynchronizationservice.json"
LOG="/tmp/config.log.`date +%d%m%Y_%T`"
#------------------------------------
export ADCLIENTID
export TENANTID
export B2CCLIENTID
export DOMAIN
export SIGNUPSIGNINPOLICYID
export COSMOSDBEP
export NOTIFICATIONHUBPATH
export TWILIOACCID
export MSAPPID
#------------------------------------
ls $GIT_DIRPATH
if [ $? -eq 0 ]
then
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH exists" >> $LOG
        cd $GIT_DIRCONFIGPATH
        echo "------------------------------------" >> $LOG
        echo "Updating $CONFIGAPI with the values..." >> $LOG
        mv $GIT_DIRCONFIGPATH/$CONFIGAPI $GIT_DIRCONFIGPATH/$CONFIGAPI.bak
        cat $GIT_DIRCONFIGPATH/$CONFIGAPI.bak | jq '.CosmosDb.Endpoint=env.COSMOSDBEP' | jq '.AzureAd.ClientId=env.ADCLIENTID' | jq '.AzureAd.TenantId=env.TENANTID' | jq '.AzureAdB2CWeb.ClientId=env.B2CCLIENTID' | jq '.AzureAdB2CWeb.Domain=env.DOMAIN' | jq '.AzureAdB2CWeb.SignUpSignInPolicyId=env.SIGNUPSIGNINPOLICYID' | jq '.NotificationHub.PathName=env.NOTIFICATIONHUBPATH' | jq '.Twilio.AccountSID=env.TWILIOACCID' > $GIT_DIRCONFIGPATH/$CONFIGAPI
        echo "------------------------------------" >> $LOG
        echo "Updating $CONFIGADMINPORTAL with the values..." >> $LOG
        echo "------------------------------------" >> $LOG
        echo "Updating $CONFIGCHATSERVICE with the values..." >> $LOG
        echo "------------------------------------" >> $LOG
        mv $GIT_DIRCONFIGPATH/$CONFIGCHATSERVICE $GIT_DIRCONFIGPATH/$CONFIGCHATSERVICE.bak
        cat $GIT_DIRCONFIGPATH/$CONFIGCHATSERVICE.bak | jq '.Bot.MicrosoftAppId=env.MSAPPID' | jq '.AzureAd.ClientId=env.ADCLIENTID' | jq '.AzureAd.TenantId=env.TENANTID' | jq '.AzureAdB2CWeb.ClientId=env.B2CCLIENTID' | jq '.AzureAdB2CWeb.Domain=env.DOMAIN' | jq '.AzureAdB2CWeb.SignUpSignInPolicyId=env.SIGNUPSIGNINPOLICYID' | jq '.CosmosDb.Endpoint=env.COSMOSDBEP' | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.TenantId=env.TENANTID' > $GIT_DIRCONFIGPATH/$CONFIGCHATSERVICE
        echo "------------------------------------" >> $LOG
        echo "Updating $CONFIGDEVICEPROVISING with the values..." >> $LOG
        echo "------------------------------------" >> $LOG
        mv $GIT_DIRCONFIGPATH/$CONFIGDEVICEPROVISING $GIT_DIRCONFIGPATH/$CONFIGDEVICEPROVISING.bak
        cat $GIT_DIRCONFIGPATH/$CONFIGDEVICEPROVISING.bak | jq '.AzureAd.ClientId=env.ADCLIENTID' | jq '.AzureAd.TenantId=env.TENANTID' | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.TenantId=env.TENANTID' > $GIT_DIRCONFIGPATH/$CONFIGDEVICEPROVISING
        echo "Updating $CONFIGEVENTPROCESSINGSERVICE with the values..." >> $LOG
        echo "------------------------------------" >> $LOG
        mv $GIT_DIRCONFIGPATH/$CONFIGEVENTPROCESSINGSERVICE $GIT_DIRCONFIGPATH/$CONFIGEVENTPROCESSINGSERVICE.bak
        cat $GIT_DIRCONFIGPATH/$CONFIGEVENTPROCESSINGSERVICE.bak | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.TenantId=env.TENANTID' > $GIT_DIRCONFIGPATH/$CONFIGEVENTPROCESSINGSERVICE
        echo "------------------------------------" >> $LOG
        echo "Updating $CONFIGIOTHUBCONTROLLERSERVICE with the values..." >> $LOG
        echo "------------------------------------" >> $LOG
        echo "Updating $CONFIGMESSAGEDISPATCHERSERVICE with the values..." >> $LOG
        echo "------------------------------------" >> $LOG
        echo "Updating $CONFIGRESPONSESERVICE with the values..." >> $LOG
        echo "------------------------------------" >> $LOG
        mv $GIT_DIRCONFIGPATH/$CONFIGRESPONSESERVICE $GIT_DIRCONFIGPATH/$CONFIGRESPONSESERVICE.bak
        cat $GIT_DIRCONFIGPATH/$CONFIGRESPONSESERVICE.bak | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.TenantId=env.TENANTID' > $GIT_DIRCONFIGPATH/$CONFIGRESPONSESERVICE
        echo "Updating $CONFIGSIGNALRSERVICE with the values..." >> $LOG
        echo "------------------------------------" >> $LOG
        mv $GIT_DIRCONFIGPATH/$CONFIGSIGNALRSERVICE $GIT_DIRCONFIGPATH/$CONFIGSIGNALRSERVICE.bak
        cat $GIT_DIRCONFIGPATH/$CONFIGSIGNALRSERVICE.bak | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.TenantId=env.TENANTID' > $GIT_DIRCONFIGPATH/$CONFIGSIGNALRSERVICE
        echo "Updating $CONFIGWORKFLOWS with the values..." >> $LOG
        echo "------------------------------------" >> $LOG
        mv $GIT_DIRCONFIGPATH/$CONFIGWORKFLOWS $GIT_DIRCONFIGPATH/$CONFIGWORKFLOWS.bak
        cat $GIT_DIRCONFIGPATH/$CONFIGWORKFLOWS.bak | jq '.CosmosDb.Endpoint=env.COSMOSDBEP' > $GIT_DIRCONFIGPATH/$CONFIGWORKFLOWS
        echo "Updating $CONFIGDEVICESYNCHRONIZATIONSERVICE with the values..." >> $LOG
        echo "------------------------------------" >> $LOG
        mv $GIT_DIRCONFIGPATH/$CONFIGDEVICESYNCHRONIZATIONSERVICE $GIT_DIRCONFIGPATH/$CONFIGDEVICESYNCHRONIZATIONSERVICE.bak
        cat $GIT_DIRCONFIGPATH/$CONFIGDEVICESYNCHRONIZATIONSERVICE.bak | jq '.RestService.AzureAd.ClientId=env.ADCLIENTID' | jq '.RestService.AzureAd.TenantId=env.TENANTID' > $GIT_DIRCONFIGPATH/$CONFIGDEVICESYNCHRONIZATIONSERVICE
else
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH doesn't exists" >> $LOG
fi
