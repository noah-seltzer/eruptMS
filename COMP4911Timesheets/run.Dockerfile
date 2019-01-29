FROM microsoft/dotnet:2.2-sdk

# install the .net core debugger
RUN apt-get update
RUN apt-get -y --no-install-recommends install unzip
RUN apt-get -y --no-install-recommends install procps
RUN rm -rf /var/lib/apt/lists/*

RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

VOLUME /app
WORKDIR /app

CMD dotnet watch run --urls http://0.0.0.0:80