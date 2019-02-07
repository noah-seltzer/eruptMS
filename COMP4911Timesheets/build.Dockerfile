FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "COMP4911Timesheets.dll"]

# FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 443

# FROM microsoft/dotnet:2.2-sdk AS build
# WORKDIR /src
# COPY ["COMP4911Timesheets.csproj", "COMP4911Timesheets/"]
# RUN dotnet restore "COMP4911Timesheets/COMP4911Timesheets.csproj"
# COPY . .
# WORKDIR "/src/COMP4911Timesheets"
# RUN dotnet build "COMP4911Timesheets/COMP4911Timesheets.csproj" -c Release -o /app

# FROM build AS publish
# RUN dotnet publish "COMP4911Timesheets.csproj" -c Release -o /app

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app .
# ENTRYPOINT ["dotnet", "COMP4911Timesheets.dll"]


# #This block of commands 
# FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 443

# FROM microsoft/dotnet:2.2-sdk AS build
# WORKDIR /src
# COPY ["COMP4911Timesheets.csproj", "COMP4911Timesheets/"]
# RUN dotnet restore "COMP4911Timesheets/COMP4911Timesheets.csproj"
# COPY . .

# RUN dotnet build "COMP4911Timesheets.csproj" -c Release -o /app

# FROM build AS publish
# RUN dotnet publish "COMP4911Timesheets.csproj" -c Release -o /app

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app .
# RUN ls
# # RUN dotnet COMP4911Timesheets.dll
# ENTRYPOINT ["dotnet", "COMP4911Timesheets.dll"]
# #RUN dotnet restore && dotnet COMP4911Timesheets.dll

# # FROM microsoft/dotnet:2.2-sdk

# # VOLUME /app

# # # restore as a separate layer to speed up builds
# # WORKDIR /src
# # COPY src/COMP4911Timesheets.csproj .
# # RUN dotnet restore

# # COPY src/Project/ .
# # CMD dotnet publish -c Release -o /app/out/