REM Create a resource group for the DNS Zone and records
az group create --name DnsResourceGroup --location westeurope

REM Create a DNS zone called babosbird.com in the resource group DnsResourceGroup 
az network dns zone create --name babosbird.com --resource-group DnsResourceGroup --tags environment=K8s type=DNS

REM Create an A record for the public ip exposed by the public load balancer created for the TodoApi service in Kubernetes
az network dns record-set a add-record --resource-group DnsResourceGroup --zone-name babosbird.com --record-set-name www --ipv4-address 13.93.46.58
az network dns record-set a add-record --resource-group DnsResourceGroup --zone-name babosbird.com --record-set-name * --ipv4-address 13.93.46.58

REM View records
az network dns record-set list --resource-group DnsResourceGroup --zone-name babosbird.com

REM View records records at the top of the zone.
az network dns record-set list --resource-group DnsResourceGroup --zone-name babosbird.com --query "[?name=='@' && ends_with(type, 'NS')]|[0]"

REM Retrieve the name servers for your zone. These name servers should be configured with 
REM the domain name registrar where you purchased the domain name.
az network dns zone list --resource-group DnsResourceGroup --query "[?name=='babosbird.com']|[0].nameServers" --output json

REM Verify name resolution
nslookup -type=SOA babosbird.com
nslookup -type=A www.babosbird.com
nslookup -type=A babosbird.com