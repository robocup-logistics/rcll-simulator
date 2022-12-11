FROM mcr.microsoft.com/dotnet/sdk:6.0
RUN apt-get update && apt-get -y install xterm libncurses5-dev locales locales-all vim
COPY /MPS/ /simulator/MPS
COPY /protobuff/ /simulator/protobuff
COPY /RobotEssentials/ /simulator/RobotEssentials
COPY /TerminalGui/ /simulator/TerminalGui/
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
