# 安装

`dotnet new --install DotNetCore.ApiTemplate.CSharp::1.0.2`

# 卸载

`dotnet new -u DotNetCore.ApiTemplate.CSharp`
# 帮助

`dotnet new dncat --help`

```shell
DotNetCoreApiTemplate (C#)
作者: 欧俊
选项:
  -ha|--hangfire  是否使用hangfire
                  bool - Required

  -r|--rabbitmq   是否使用RabbitMQ,使用 RabbitMQ.EventBus.AspNetCore包
                  bool - Required

  -re|--redis     是否使用DistributedRedisCache
                  bool - Required

  -p|--pg         是否使用Postgresql
                  bool - Optional
                  默认: true

  -e|--es         是否使用ElasticSearch
                  bool - Required
```

# 例

## 使用pg,rabbitmq,redis
`dotnet new dncat -n Core.New.Service -ha false -r true -re true -e false -p true`
## 使用pg,rabbitmq,redis,hangfire
`dotnet new dncat -n Core.New.Service -ha true -r true -re true -e false -p true`
## 使用pg,rabbitmq,redis,es
`dotnet new dncat -n Core.New.Service -ha false -r true -re true -e true -p true`
## 使用rabbitmq,es
`dotnet new dncat -n Core.New.Service -ha false -r true -re false -e true -p false`