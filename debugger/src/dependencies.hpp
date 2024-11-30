#pragma once

#include "MachineInstructions.pb.h"
#include "AgentTask.pb.h"

#include <vector>

#include <nlohmann/json.hpp>
#include <vector>

class Dependencies {
public:
    Dependencies();
    void add_edge(int from, int to);
    void finish_task(int task);
    std::vector<int> get_next_tasks();
private:
    std::map<int, std::vector<int>> adj;
    std::map<int, int> in;
    std::mutex mutex;
};

struct MachineInstruction {
    std::shared_ptr<llsf_msgs::PrepareMachine> message;
    llsf_msgs::Team team;
    float start_time;
    float end_time;
    int id;
};

struct AgentTask {
    std::shared_ptr<llsf_msgs::AgentTask> message;
    int robot_id;
    llsf_msgs::Team team;
    float start_time;
    float end_time;
    int id;
};
