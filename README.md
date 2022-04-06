# General
Simulation of the Environment for the RoboCup Refbox and Robots.
Specialized for the GRIPS Team currently but should be adaptable for other teams too.

# Setup of simulation
To start the environment you need to first set your xhost to be able to connect. this can be done with 
```
 xhost + 
```
next you need to build the containers for the refbox and the simulation. this can be done with the script
```
setup.sh.
```
if the containers are build you only need to rebuild if you update to a new version of the simulator if you want to update some configurations in the refbox or simulator
if this is setup you can start the simulated environment with the 
```
start_environment.sh 
```
script.

# Troubleshooting 

## Simulation terminal is missing some borders and looks incorrect
This is a problem that I'm still working on to solve. I hope in future updates the terminal will look correctly.
## Terminal keeps spawning
if the spawned terminal from the simulation is restarting even when the compose has been stopped you need to kill the docker container manually. you can check your runnning containers with 
```
docker ps
```
and with 
```
docker kill [container name>]
```
you can stop the spawn of the xterminal and the simulation generally.

