# intl-wallet

[![Build Status](https://ci-next.docker.com/public/buildStatus/icon?job=compose/master)](https://ci-next.docker.com/public/job/compose/job/master/)

API Documentation: https://drive.google.com/file/d/1WWue6IdmY7x7hJ2yEpgx8R7TjfI7QC4s/view?usp=sharing

Technologies used in this project include:
1. Visual Studio IDE.
2. ASP.NET Core API and Class Library Projects.
3. Entity Framework Core: an object-relational mapper which enables developers to work with data using objects of domain specific classes without focusing on the underlying database where this data is stored.
4. ASP.NET Core Identity: used for managing api/app user accounts.
5. ASP.NET Core Authentication JwtBearer: ASP.NET Core middleware that enables an application to receive an OpenID Connect bearer token.
6. MS SQLServer: a relational database management system.
8. NUnit: a test framework for defining and running unit tests.
9. Swashbuckle Swagger, SwaggerGen and SwaggerUI: builds a rich, customizable experience for describing the web API functionality.

Docker Compose
==============

Docker Compose is a tool for running multi-container applications on Docker
defined using the [Compose file format](https://compose-spec.io).
A Compose file is used to define how the one or more containers that make up
your application are configured.
Once you have a Compose file, you can create and start your application with a
single command: `docker-compose up`.

Quick Start
-----------

Using Docker Compose is basically a three-step process:
1. Define your app's environment with a `Dockerfile` so it can be
   reproduced anywhere.
2. Define the services that make up your app in `docker-compose.yml` so
   they can be run together in an isolated environment.
3. Lastly, run `docker-compose up` and Compose will start and run your entire
   app.
