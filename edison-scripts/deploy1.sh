#!/bin/bash
#Comment - Installs the required packages for building images
#Author - Vivek

AZ_REPO=$(lsb_release -cs)
LOG="/tmp/install.log.`date +%d%m%Y_%T`"
GIT_URL="https://github.com/litebulb/ProjectEdison.git"
GIT_PATH=`pwd`

#Installing Azure CLI
sudo apt-get update
sleep 10
sudo apt-get install apt-transport-https lsb-release ca-certificates curl software-properties-common gnupg2 pass jq -y
sleep 60
echo "deb [arch=amd64] https://packages.microsoft.com/repos/azure-cli/ $AZ_REPO main" | sudo tee /etc/apt/sources.list.d/azure-cli.list
sudo apt-key --keyring /etc/apt/trusted.gpg.d/Microsoft.gpg adv --keyserver packages.microsoft.com --recv-keys BC528686B50D79E339D3721CEB3E94ADBE1229CF
sudo apt-get update
sleep 10
sudo apt-get install azure-cli
sleep 10
az -v
if [ $? -eq 0 ]
then
        echo "------------------------------------" >> $LOG
        echo "Azure CLI installation is successful" >> $LOG
else
        echo "------------------------------------" >> $LOG
        echo "Azure CLI installation failed" >> $LOG
fi

#Installing Docker CE & Compose

curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
sudo apt-key fingerprint 0EBFCD88
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"
sudo apt-get update
sleep 10
sudo apt-get install docker-ce docker-compose -y
sleep 10
docker -v
if [ $? -eq 0 ]
then
        echo "------------------------------------" >> $LOG
        echo "Docker CE installation is successful" >> $LOG
else
        echo "------------------------------------" >> $LOG
        echo "Docker CE installation failed" >> $LOG
fi

#Initialize Docker Swarn

sudo docker swarm init

#Installing kubectl

curl -s https://packages.cloud.google.com/apt/doc/apt-key.gpg | sudo apt-key add -
echo "deb https://apt.kubernetes.io/ kubernetes-xenial main" | sudo tee -a /etc/apt/sources.list.d/kubernetes.list
sudo apt-get update
sleep 20s
sudo apt-get install -y kubectl
kubectl --help
if [ $? -eq 0 ]
then
echo "------------------------------------" >> $LOG
echo "kubectl installation is successful" >> $LOG
else
echo "------------------------------------" >> $LOG
echo "kubectl installation failed" >> $LOG
fi

#Cloning your GIT Repository

git clone $GIT_URL
if [ -d $GIT_PATH ]
then
        echo "------------------------------------" >> $LOG
        echo "The $GIT_PATH exist & clone successful" >> $LOG
        cd $GIT_PATH/ProjectEdison

else
        echo "------------------------------------" >> $LOG
        echo "The $GIT_PATH doen't exist & clone failed" >> $LOG
fi