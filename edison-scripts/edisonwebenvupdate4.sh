#!/bin/bash
#Comment - Updates the environment files for Edison Admin code
#Author - Vivek
#------------------------------------
TENANT_VALUE=`head -2 input.txt | awk -F "\"" '{print $2}'| tail -1`
CLIENTID_VALUE=`head -1 input.txt | awk -F "\"" '{print $2}'`
BASEURL_VALUE=`head -24 input.txt | awk -F "\"" '{print $2}'| tail -1`
LOCALHOST_VALUE="localhost:26000"
#------------------------------------
GIT_URL="https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/stage/scripts"
GITPATH=`pwd`
GIT_DIRPATH="$GITPATH/ProjectEdison"
GIT_DIRCONFIGPATH="$GIT_DIRPATH/Edison.Web/Edison.AdminPortal/ClientApp/src/environments"
ENV="/environment.ts.bak"
ENVLOCAL="/environment.local.ts.bak"
ENVDEBUG="/environment.debug.ts.bak"
ENVPROD="/environment.prod.ts.bak"
ENVQA="/environment.qa.ts.bak"
LOG="/tmp/edisonwebenv.log.`date +%d%m%Y_%T`"
#------------------------------------
ls $GIT_DIRPATH
if [ $? -eq 0 ]
then
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH exists" >> $LOG
        cd $GIT_DIRCONFIGPATH
        mv $GIT_DIRCONFIGPATH/* /tmp/
        echo "------------------------------------" >> $LOG
        echo "Updating the $ENV file..." >> $LOG
        wget -O $GIT_DIRCONFIGPATH$ENV $GIT_URL$ENV
        sed -i -e s/TENANTVALUE/${TENANT_VALUE}/ -e s/CLIENTIDVALUE/${CLIENTID_VALUE}/ -e s/BASEURLVALUE/${BASEURL_VALUE}/ $GIT_DIRCONFIGPATH$ENV
        mv $GIT_DIRCONFIGPATH$ENV $GIT_DIRCONFIGPATH/environment.ts
        echo "------------------------------------" >> $LOG
        echo "Updating the $ENVLOCAL file..." >> $LOG
        wget -O $GIT_DIRCONFIGPATH$ENVLOCAL $GIT_URL$ENVLOCAL
        sed -i -e s/TENANTVALUE/${TENANT_VALUE}/ -e s/CLIENTIDVALUE/${CLIENTID_VALUE}/ -e s/BASEURLVALUE/${BASEURL_VALUE}/ $GIT_DIRCONFIGPATH$ENVLOCAL
        mv $GIT_DIRCONFIGPATH$ENVLOCAL $GIT_DIRCONFIGPATH/environment.local.ts
        echo "------------------------------------" >> $LOG
        echo "Updating the $ENVDEBUG file..." >> $LOG
        wget -O $GIT_DIRCONFIGPATH$ENVDEBUG $GIT_URL$ENVDEBUG
        sed -i -e s/TENANTVALUE/${TENANT_VALUE}/ -e s/CLIENTIDVALUE/${CLIENTID_VALUE}/ -e s/BASEURLVALUE/${LOCALHOST_VALUE}/ $GIT_DIRCONFIGPATH$ENVDEBUG
        mv $GIT_DIRCONFIGPATH$ENVDEBUG $GIT_DIRCONFIGPATH/environment.debug.ts
        echo "------------------------------------" >> $LOG
        echo "Updating the $ENVPROD file..." >> $LOG
        wget -O $GIT_DIRCONFIGPATH$ENVPROD $GIT_URL$ENVPROD
        sed -i -e s/TENANTVALUE/${TENANT_VALUE}/ -e s/CLIENTIDVALUE/${CLIENTID_VALUE}/ -e s/BASEURLVALUE/${BASEURL_VALUE}/  $GIT_DIRCONFIGPATH$ENVPROD
        mv $GIT_DIRCONFIGPATH$ENVPROD $GIT_DIRCONFIGPATH/environment.prod.ts
        echo "------------------------------------" >> $LOG
        echo "Updating the $ENVQA file..." >> $LOG
        wget -O $GIT_DIRCONFIGPATH$ENVQA $GIT_URL$ENVQA
        sed -i -e s/TENANTVALUE/${TENANT_VALUE}/ -e s/CLIENTIDVALUE/${CLIENTID_VALUE}/ -e s/BASEURLVALUE/${BASEURL_VALUE}/ $GIT_DIRCONFIGPATH$ENVQA
        mv $GIT_DIRCONFIGPATH$ENVQA $GIT_DIRCONFIGPATH/environment.qa.ts
fi
