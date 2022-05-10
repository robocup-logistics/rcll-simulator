FROM mcr.microsoft.com/dotnet/sdk:6.0
RUN apt-get update && apt-get -y install xterm libncurses5-dev locales locales-all
COPY Simulator/MPS/ /simulator/MPS
COPY Simulator/protobuff/ /simulator/protobuff
COPY Simulator/RobotEssentials/ /simulator/RobotEssentials
COPY Simulator/TerminalGui/ /simulator/TerminalGui/
COPY Simulator/Utility/ /simulator/Utility
COPY Simulator/Configurations.cs /simulator/
COPY Simulator/MainClass.cs /simulator/
COPY Simulator/Simulator.csproj /simulator/
COPY Simulator/cfg/ /simulator/cfg/
WORKDIR /simulator
