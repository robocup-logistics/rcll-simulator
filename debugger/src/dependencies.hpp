#pragma once

#include "MachineInstructions.pb.h"
#include "AgentTask.pb.h"

#include <vector>

#include <nlohmann/json.hpp>
#include <vector>
#include <map>
#include <mutex>
#include <memory>


class Dependencies {
public:
    Dependencies();
    void add_edge(int from, int to);
    void finish_task(int task);
    std::vector<int> get_next_tasks();

    std::string to_string() const {
        std::ostringstream oss;
        for (const auto& [from, to_list] : adj) {
            oss << from << " -> [";
            for (size_t i = 0; i < to_list.size(); ++i) {
                oss << to_list[i];
                if (i < to_list.size() - 1) oss << ", ";
            }
            oss << "]\n";
        }
        return oss.str();
    }
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
