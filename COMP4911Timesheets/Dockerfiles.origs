FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["COMP4911Timesheets/COMP4911Timesheets.csproj", "COMP4911Timesheets/"]
RUN dotnet restore "COMP4911Timesheets/COMP4911Timesheets.csproj"
COPY . .
WORKDIR "/src/COMP4911Timesheets"
RUN dotnet build "COMP4911Timesheets.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "COMP4911Timesheets.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "COMP4911Timesheets.dll"]