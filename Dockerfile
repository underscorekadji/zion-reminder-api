FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Zion.Reminder.Api.csproj", "./"]
RUN dotnet restore "Zion.Reminder.Api.csproj"
COPY . .
RUN dotnet publish "Zion.Reminder.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .

# Create a non-root user for running the app
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Zion.Reminder.Api.dll"]
