# MiningService
## Starts and monitors mining jobs using DynamoDb storage

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

## Prerequisites

* Automation of deployment is done using AWS CLI, make sure it is installed locally 
* Deployment scripts are available for Linux/OS X (build.sh) as well as powershell script for windows environments (build.ps1)
* Obtain Access Key ID and Secret Access Key from your AWS account


## Solution Projects

The following projects are included in the solution:

| Project | Description |
| ------ | ------ |
| MiningService.Core | Holds all interfaces and business logic |
| MiningService.Infrastructure | Holds infrastructure implementations such as DynamoDb |
| MiningService.PostMine | Web api POST endpoint to start a mining job |
| MiningService.GetMine | Web api GET endpoint to retrieve status of a given job |
| MiningService.Infrastructure.Tests | Unit test project for testing the infrastructure layer |

## Build

1. Edit the deploy.ps1 script and enter your AWS Access Key ID and Secret Access Key 
2. To run the build and deployment on windows machines execute the following in a powershell window:

```sh
deploy.ps1
```

## MiningService.PostMine

The PostMine endpoint starts a new mining operation

```sh
POST /mine
```



## MiningService.GetMine

The GetMine endpoint retrieves the status of a mining job

```sh
GET /mine?jobId=cafcb792-132e-4091-a980-410af950077b
```
