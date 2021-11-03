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
| MiningService.WebApi | web api exposing a GET and POST endpoints. Examples below. |
| MiningService.Infrastructure.Tests | Unit test project for testing the infrastructure layer |

## WebApi


