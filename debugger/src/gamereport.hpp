#pragma once

#include <map>
#include <string>
#include "dependencies.hpp"

namespace gamereport {
    void set_dict(std::map<std::string, int> &dict);
    void get_tasks(nlohmann::json &json_data, std::vector<AgentTask> &agent_tasks);
    void get_instructions(nlohmann::json &json_data, std::vector<MachineInstruction> &instructions);
    std::string base_color_to_text(const llsf_msgs::BaseColor base_color);
    std::string ring_color_to_text(const llsf_msgs::RingColor ring_color);
    std::string instruction_to_text(std::shared_ptr<llsf_msgs::PrepareMachine> instruction);
    std::string agent_task_to_text(std::shared_ptr<llsf_msgs::AgentTask> agent_task);
}
