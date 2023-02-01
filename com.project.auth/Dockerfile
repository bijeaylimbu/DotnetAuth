FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["com.project.auth/com.project.auth.csproj", "com.project.auth/"]
RUN dotnet restore "com.project.auth/com.project.auth.csproj"
COPY . .
WORKDIR "/src/com.project.auth"
RUN dotnet build "com.project.auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "com.project.auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "com.project.auth.dll"]
