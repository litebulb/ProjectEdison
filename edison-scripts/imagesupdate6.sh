#!/bin/bash
#Comment - Builds the images and pushes Edison images to ACR
#------------------------------------
ACR_SERVERNAME=`head -27 input.txt | awk -F "\"" '{print $2}'| tail -1`
ACR_USERNAME=`head -28 input.txt | awk -F "\"" '{print $2}'| tail -1`
ACR_PASSWD=`head -29 input.txt | awk -F "\"" '{print $2}'| tail -1`
TAG=`head -30 input.txt | awk -F "\"" '{print $2}'| tail -1`
#------------------------------------
GITPATH=`pwd`
GIT_DIRPATH="$GITPATH/ProjectEdison"
LOG="/tmp/imagesupdate.log.`date +%d%m%Y_%T`"

#------------------------------------
ls $GIT_DIRPATH
if [ $? -eq 0 ]
then
        echo "------------------------------------" >> $LOG
        echo "The $GIT_DIRPATH exists" >> $LOG
        cd $GIT_DIRPATH
        sudo docker-compose build
        sleep 30
        IMAGE=`docker images --format "{{.Repository}}" --filter=reference='edison*' | wc -l`
        if [ $IMAGE -eq 11 ]
        then
                echo "------------------------------------" >> $LOG
                echo "All the 11 Edison Images are successfully built" >> $LOG
                echo "$ACR_PASSWD" | docker login $ACR_SERVERNAME -u $ACR_USERNAME --password-stdin >> $LOG
                IMAGE_NAMES=`docker images --format "{{.Repository}}" --filter=reference='edison*'`
                echo "------------------------------------" >> $LOG
                echo "Edison images names: " >> $LOG
                echo $IMAGE_NAMES >> $LOG
                for i in $IMAGE_NAMES
                do
                        echo "------------------------------------" >> $LOG
                        echo "$i" >> $LOG
                        sudo docker tag $i $ACR_SERVERNAME/$i:$TAG
                        sudo docker push $ACR_SERVERNAME/$i:$TAG
                        echo "The Image $i is successfully pushed" >> $LOG
                done
        else
                echo "Edison Images didn't built" >>  $LOG
        fi
else
        echo "------------------------------------" >> $LOG
        echo "The $GIT_PATH doen't exist & clone failed" >> $LOG
fi
