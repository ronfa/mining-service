# QuickStart
* There needs to be at least one git commit for GitVersion to be able to generate a gitversion
* Run ./build.ps1 to initiate build & deploy process
* Run ./build.ps1 --target ParameterUpload -> this will upload parameters to parameter store.
* Run ./utils/SecureConfiguration.ps1 script to protect secure config.

# Authorization & Authentication

Json Web Tokens (JWT)

References : 

* https://jwt.io/introduction/
* https://www.pingidentity.com/en/company/blog/posts/2019/jwt-security-nobody-talks-about.html



Bloxx Api Key

We are using API Gateway Authorizer mechanism. Authorization Lambda Connects to Bloxx Authentication System and validates API Keys. It can also support Oauth but this support is not yet provided.

Api gateway authorizer lambda documentation : https://docs.aws.amazon.com/apigateway/latest/developerguide/apigateway-use-lambda-authorizer.html

https://docs.aws.amazon.com/apigateway/latest/developerguide/configure-api-gateway-lambda-authorization-with-console.html

Scopes (**this section needs to be removed once we have swagger specification**) : 

* Consents.WebApi.Register
  * Grants access to POST /userconsent/register endpoint 
* Consents.WebApi.Full Access
  * Grants access to all endpoonts
* Consents.WebApi.Update
  * Grants access to PUT /userconsent/update endpoint 

Configuration is done under /src/ConsentManagementService.WebApi/config/authorizer/appsettings.json file.

Postman collection at /postman/Consent_WebApi_WithAuthCall_2_July_2020_postman_collection.json provides calls to Bloxx Authentication Management system to create scopes, api keys and also provides calls to the POST /userconsent/register endpoint.

#Deploy.cake file usage

This file is used for deployment pipeline and currently contains below tasks :

* Default
  * Deployment Pipeline task which is also dependent on below tasks
* Db-Migration
  * Does Ef core Db migration to setup consent schema and seed data by calling ConsentManagementService.Orm project
* EFMigration-Base-Table-Creation
  * Executes a mySql query against consent database for creating the base __EFMigrationHistory table if table does not exists.

# Remaining tasks for August 2020
* Final decision on JWT structure -> Are we going to encrypt the payload?
* Final decision on using JWT tokens??? should we go for B stored tokens?
* Decision page for Tokens :) 
* Update of Postman collection - may be small documentation on the structure of postman.
* Improve API KEY validation replacement function setup which is available only for dev env.
  * may be move to its own class?
* FIX UNIT TESTS!!! :)
* Create documentation to explain authentication & Authorization
	* Topics
		* Api gateway authorizers
			* Api Key
			* JWT
			* why JWT is the default authorizer?
		* How to override default API GW AUthorizer for lambdas? (like for register)
		* How to pass data from authorizer lambda to actual lambda
		* How to create a new authorizer if need be (if we decide to do token management in DB for instance)
		* How to extract/pickup authorizer data in target lambda function (like in update/man)
		* Elaboration on ApiKey Validation Replacement API GW and lambda function which are only relevant for dev environment/playpen
		* JWT Configuration setup
			* refering to https://www.pingidentity.com/en/company/blog/posts/2019/jwt-security-nobody-talks-about.html -> why are we going with a plain implementation as opposed to using asynmetrical keys & encrpyted payloads
				* Simple answer: everything happens within the api & payload is already hashed.
		