FROM mcr.microsoft.com/dotnet/sdk:8.0
#RUN apt-get update && apt-get -y install xterm libncurses5-dev locales locales-all
RUN apt-get update && \
    apt-get install -y --no-install-recommends  protobuf-compiler && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/*
COPY /MPS/ /simulator/MPS
COPY /protobuf/ /simulator/protobuf
COPY /RobotEssentials/ /simulator/RobotEssentials
#COPY /TerminalGui/ /simulator/TerminalGui/
COPY /Utility/ /simulator/Utility
COPY /Configurations.cs /simulator/
COPY /MainClass.cs /simulator/
COPY /Simulator.csproj /simulator/
COPY /cfg/ /simulator/cfg/
COPY /WebGui/ /simulator/WebGui/
WORKDIR /simulator
RUN dotnet restore
RUN dotnet build
RUN mkdir logs
