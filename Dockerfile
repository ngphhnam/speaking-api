FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SpeakingPractice.Api.csproj", "."]
RUN dotnet restore "SpeakingPractice.Api.csproj"
COPY . .
RUN dotnet publish "SpeakingPractice.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "SpeakingPractice.Api.dll"]

