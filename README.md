RecordSQL.Backend
==============

Description
----------

Backend for object-relational framework RecordSQL (Record v2)

Instruction
----------
### Start .NET Core application on Ubuntu
* [Base manual](https://medium.com/@JohGeoCoder/deploying-a-net-core-2-0-web-application-to-a-production-environment-on-ubuntu-16-04-using-nginx-683b7e831e6)
* [Installation of .NET Core 2.2 on Ubuntu 18.04](https://dotnet.microsoft.com/download/linux-package-manager/ubuntu18-04/sdk-current)
* [Configure Nginx](https://docs.microsoft.com/ru-ru/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.2#https-configuration)
* [.Net commands](https://docs.microsoft.com/ru-ru/dotnet/core/tools/dotnet-publish?tabs=netcore21)

```sudo apt-get install git npm nginx dotnet-sdk-2.2```

### Clone
```git clone githubrepo recordsql-backend```
```git reset --hard```
```git checkout -b development origin/development```
		
### Publish
```sudo dotnet publish RecordSPA.csproj -c Release -o /var/www/recordsql/backend```
	
### Environment variables
* **Permanent**
	```sudo nano /etc/environment```
		ASPNETCORE_ENVIRONMENT=Development
* **Temporary**
	* Set ```export ASPNETCORE_ENVIRONMENT='Development'```	
	* Show ```echo $ASPNETCORE_ENVIRONMENT```

### Start
```dotnet run```

### Errors
[fix throw er;](https://stackoverflow.com/questions/49975596/events-js183-throw-er-unhandled-error-event)
**for npm run (ENOSPC)** you need grow **inotify file watch limit**
[fix cache overflow you need run](https://stackoverflow.com/questions/40566348/maximum-call-stack-size-exceeded-on-npm-install)
```npm cache clean --force```
```rm package-lock.json```
```rm -r node_modules```