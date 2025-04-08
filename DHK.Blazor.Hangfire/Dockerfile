#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["SprintTek.Blazor.Hangfire/SprintTek.Blazor.Hangfire.csproj", "SprintTek.Blazor.Hangfire/"]
COPY ["SprintTek.Blazor.Module/SprintTek.Blazor.Module.csproj", "SprintTek.Blazor.Module/"]
COPY ["SprintTek.Module/SprintTek.Module.csproj", "SprintTek.Module/"]
RUN dotnet restore "SprintTek.Blazor.Hangfire/SprintTek.Blazor.Hangfire.csproj"
COPY . .
WORKDIR "/src/SprintTek.Blazor.Hangfire"
RUN dotnet build "SprintTek.Blazor.Hangfire.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SprintTek.Blazor.Hangfire.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SprintTek.Blazor.Hangfire.dll"]