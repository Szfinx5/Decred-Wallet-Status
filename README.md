<div align="center">
    <img src="decred_logo.png" alt="logo" width="400"/>
</div>
<h1 align="center">Decred Hot Wallet Status Checker</h1>


## Why Decred?
I have started my journey into the Cryptocurrency world at 2016 when I bought my firts stake of Decred (DRC). After doing even more research, I have realised the technological and financial possibilities of the project, learning the theory behind blockchain, proof-of-stake and proof-of-work mining and also the passive income it can create.

## What is the Decred-Wallet Status Checker?
To learn more about how consensus and decision making works in the cryptocurrency world, I started hosting my own proof-of-stake mining pool since the end of 2016. I am still running 3 voting wallets on geographically diverse Linux virtual servers. As those wallets are the ones which confirm my stake on the Decred network and verifying new blocks (therefore earning passive income), it is very important to keep them synced to the network.

## What is this application doing?
The Decred-Wallet Status Checker SSH into each virtual servers, issue a command to check the current block height then save those details in a MySQL database. Also, fetching the latest block height from the Decred block explorer API and compare this to the results from the hot wallets.

## Plans for the future?
This application is only the backbend solution, it fetches the data from the virtual servers and the block explorer, compare the data and store them. Currently, the front end is a Grafana dashboard, but I am planning to write a simple front end to display the information in an easily understandable way.

## Tech Stack       

For this portfolio page, I have used the following tech:
- C#
- MySQL
- Grafana
