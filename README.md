# MiningService
## Starts and monitors mining jobs using DynamoDb storage

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

## Prerequisites

* Automation of deployment is done using AWS CLI, make sure one is installed locally (Cake is used, and will be installed for you)
* Deployment scripts are available for Linux/OS X (build.sh) as well as powershell script for windows environments (build.ps1)

## Solution Projects

The following projects are included in the solution:

| Project | Description |
| ------ | ------ |
| MiningService.Core | Holds all interfaces and business logic |
| MiningService.Infrastructure | Holds infrastructure implementations such as DynamoDb |
| MiningService.PostMine | Web api POST endpoint to start a mining job |
| MiningService.GetMine | Web api GET endpoint to retrieve status of a given job |
| MiningService.Infrastructure.Tests | Unit test project for testing the infrastructure layer |

## MiningService.PostMine

## MiningService.GetMine


