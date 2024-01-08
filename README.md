# Bitcoin Price Display


```bash

```

## Description

Welcome to the Bitcoin Price Display.

This project is a simple web app that displays the price of Bitcoin in parity with USD - updated on a 10 seconds interval.

It also displays the average price of BTC-USD over the last 10 minutes. 

By default, the exchange used in this project is Kraken, but other CCXT supported exchanges (Bybit, Binance etc) can be utilized if required. 

The project uses Terraform to deploy the resource to an Azure subscription. 

As a bonus, a simple web API is included, which responds with status code 200 only to GET requests.

Both the Bitcoin app and the web API are deployed in Azure Kubernetes which uses ingress to do routing between the two service and perform the TLS termination as well. 

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
The above method is to preferred, so that you allow access in an AKS managed manner. If that doesn’t work, you can use an alternative where you get the object ID of your application and assign the AcrPull role directly on the resource group of you cluster. This should only be used if problems like the one in the screenshot below persist.

```bash
az role assignment create --role "AcrPull" --assignee 28f4ac4f-ffdc-40a1-83f0-d774f7b068e5 --scope /subscriptions/18d7383b-303e-489e-86cc-cd3fa3c2eb6a/resourceGroups/tutorial15-dev-demo15
```


```bash

```


```bash

```


```bash

```


```bash

```


```bash

```
