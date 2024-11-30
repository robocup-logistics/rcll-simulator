cmake_minimum_required(VERSION 3.30)
project(sim-debugger)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY "${CMAKE_SOURCE_DIR}/bin")
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "${CMAKE_SOURCE_DIR}/bin")
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_SOURCE_DIR}/bin")

find_package(Protobuf REQUIRED)
find_package(PkgConfig REQUIRED)
set(PROTO_FILES
     ../Simulator/protobuf/rcll-protobuf-msgs/AgentTask.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/AttentionMessage.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/BeaconSignal.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/ExplorationInfo.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/GameInfo.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/GameState.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/MachineCommands.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/MachineDescription.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/MachineInfo.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/MachineInstructions.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/MachineReport.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/NavigationChallenge.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/OrderInfo.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/Pose2D.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/ProductColor.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/RingInfo.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/RobotCommands.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/RobotInfo.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/SimTimeSync.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/Team.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/Time.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/VersionInfo.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/WorkpieceInfo.proto
     ../Simulator/protobuf/rcll-protobuf-msgs/Zone.proto)
protobuf_generate_cpp(PROTO_SRCS PROTO_HDRS ${PROTO_FILES})
add_library(rcll-protobuf-msgs SHARED ${PROTO_SRCS} ${PROTO_HDRS})
# Include directories for the generated protobuf headers
target_include_directories(rcll-protobuf-msgs
    PUBLIC
    ${CMAKE_BINARY_DIR}
)

add_executable(sim-debugger src/main.cpp src/dependencies.cpp src/gamereport.cpp)
target_link_libraries(sim-debugger ${Protobuf_LIBRARIES})
target_include_directories(sim-debugger PUBLIC ${Protobuf_INCLUDE_DIRS})
target_link_libraries(
    sim-debugger
    rcll-protobuf-msgs
    protobuf::libprotobuf )
    pkg_check_modules(protobuf_comm protobuf_comm)
target_link_libraries(sim-debugger ${protobuf_comm_LDFLAGS})
target_compile_options(sim-debugger PUBLIC ${protobuf_comm_CFLAGS})
