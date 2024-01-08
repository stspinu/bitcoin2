# Bitcoin Price Display


## Description

Welcome to the Bitcoin Price Display.

This project is a simple web app that displays the price of Bitcoin in parity with USD - updated on a 10 seconds interval.

It also displays the average price of BTC-USD over the last 10 minutes. 

By default, the exchange used in this project is Kraken, but other [CCXT](https://github.com/ccxt-net/ccxt.net) supported exchanges (Bybit, Binance etc) can be utilized if required. 

The project uses Terraform to deploy the resources to an Azure subscription. 

As a bonus, a simple web API is included, which responds with status code 200 only to GET requests.

Both the Bitcoin app and the web API are deployed in Azure Kubernetes which uses ingress to do routing between the two services and perform the TLS termination. 

## Requirements
- [Azure subscription](https://learn.microsoft.com/en-us/azure/azure-portal/get-subscription-tenant-id)
- [Docker](https://docs.docker.com/engine/install/)
- [Helm](https://helm.sh/docs/intro/install/)
- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
- [GIT](https://git-scm.com/)


## Installation

Clone the project so that you have a copy of the working files on your computer.

```bash
git clone https://github.com/stspinu/bitcoin2.git
cd bitcoin2
git checkout master
```
Adjust the Terraform code with your desired names/values for the resources, region and other such aspects. Then deploy the Terraform template. 

Login to your Azure tenant and select the desired subscription where you want to create the resources.


```bash
az login --tenant 16b3c013-d300-468d-ac64-7eda0820b6d3
az account set --subscription 18d7383b-303e-489e-86cc-cd3fa3c2eb6a
```
Create the resources via Terraform deployment.

```bash
cd terraform
tf init
tf plan
tf apply --auto-approve
```

Get access to your cluster and then grant it permissions over the ACR (Azure Container Registry)

```bash
az aks get-credentials --resource-group tutorial15 --name dev-demo15
az aks update -g tutorial15 -n dev-demo15 --attach-acr stef15acr15
```
The above method is to preferred, so that you allow access in an AKS managed manner. If that doesn’t work, you can use an alternative where you get the object ID of your application and assign the AcrPull role directly on the resource group of you cluster. You might get a resource not found for the ACR initially - if that happens, simply wait for a few minutes and try again. 
![image](https://github.com/stspinu/bitcoin2/assets/46924453/c7e23efb-87be-456c-a3d7-d5f73775c6a4)


```bash
az role assignment create --role "AcrPull" --assignee 28f4ac4f-ffdc-40a1-83f0-d774f7b068e5 --scope /subscriptions/18d7383b-303e-489e-86cc-cd3fa3c2eb6a/resourceGroups/tutorial15-dev-demo15
```
Ensure docker is running on your system and then build the images locally and test them

```bash
cd bitcoin2/app0
d build -t bitcoin2:v1 -f .\Dockerfile .
docker run -p 5001:5006 --name container-bitcoin2 -d bitcoin2:v1
```
![image](https://github.com/stspinu/bitcoin2/assets/46924453/4b9de191-755d-4699-bd49-c99a41eadbfc)


```bash
cd bitcoin2/nginxapi-get
d build -t nginxapi-get:v1 -f .\Dockerfile .
docker run -p 5002:80 --name container-nginxapi-get -d nginxapi-get:v1
```
![image](https://github.com/stspinu/bitcoin2/assets/46924453/7322fc98-c98c-4d3e-bce3-8608e64921c1)

Push the images to ACR

```bash
docker tag bitcoin2:v1 stef15acr15.azurecr.io/bitcoin2:v15
docker tag nginxapi-get:v1 stef15acr15.azurecr.io/nginxapi-get:v15

az acr login --name stef15acr15

docker push stef15acr15.azurecr.io/bitcoin2:v15
docker push stef15acr15.azurecr.io/nginxapi-get:v15
```
Adjust the yaml code with your desired names/values
- ISSUER modify the issuer both in staging and production with your e-mail address
- DEPLOYMENT modify the app name and the ACR
- SERVICE modify the service name and the apps used
- INGRESS modify the names accordingly and start with staging to obtain your certificate
	

Deploy your services to AKS

```bash
cd bitcoin2
kubectl apply -f ./k8s/
```
If you investigate the chain of Certificate -> CertificateRequest -> Order -> Challenge using the __kubectl describe__ command, you will see that Let's Encrypt is trying to verify your ownership for the provided domain, so you need to create a host in your DNS zone. 

![image](https://github.com/stspinu/bitcoin2/assets/46924453/882fa0ea-6d74-4302-b372-8a218cf97a18)


Therefore, you need to take the IP of your ingress and add it into your DNS zone (if you have one). In this example, we own a .site domain from GoDaddy and configured the registrar to point towards a specific public Azure DNS zone.

![image](https://github.com/stspinu/bitcoin2/assets/46924453/33e43e9f-75a0-4a4f-97a5-47e67a7519a2)


Modify the 4-ingress.yaml file to use the production issuer and redeploy the ingress file


```bash
cd bitcoin2
kubectl apply -f .\k8s\4-ingress.yaml
```

![image](https://github.com/stspinu/bitcoin2/assets/46924453/df6a807f-26b3-4557-b3f0-0eb5b6d3351b)


In a few minutes, you will see that the temporary ingresses start to disappear as they finish their jobs and your certificates become ready (True):

![image](https://github.com/stspinu/bitcoin2/assets/46924453/4350169d-44e3-41bf-8d84-d7e233d81612)


Test the two websites and ensure they work as expected. 

![image](https://github.com/stspinu/bitcoin2/assets/46924453/02497b73-8777-40fe-823f-7169eb5f286e)

## Usage
The website can be used as a reference of the Bitcoin price. It does not come with any guarantee and should not be considered as an advice to buy or sell Bitcoin. 

## Disclaimer
The information provided on this website serves as a reference for the current Bitcoin price and is for informational purposes only. It does not come with any guarantee and should not be considered as advice to buy or sell Bitcoin.

