# Public IP address
$IP="137.117.34.22"

# Name to associate with public IP address
$DNSNAME="edisoningress"

# Get the resource-id of the public ip
$PUBLICIPID=$(az network public-ip list --query "[?ipAddress!=null]|[?contains(ipAddress, '$IP')].[id]" --output tsv)

# Update public ip address with dns name
az network public-ip update --ids $PUBLICIPID --dns-name $DNSNAME