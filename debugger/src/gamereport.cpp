#include "gamereport.hpp"
#include "dependencies.hpp"
#include <vector>

void gamereport::set_dict(std::map<std::string, int> &dict) {
    dict["M-BS"] = -1;
    dict["M-CS1"] = -1;
    dict["M-CS2"] = -1;
    dict["M-RS1"] = -1;
    dict["M-RS2"] = -1;
    dict["M-DS"] = -1;
    dict["M-SS"] = -1;
    dict["C-BS"] = -1;
    dict["C-CS1"] = -1;
    dict["C-CS2"] = -1;
    dict["C-RS1"] = -1;
    dict["C-RS2"] = -1;
    dict["C-DS"] = -1;
    dict["C-SS"] = -1;
}

void gamereport::get_tasks(nlohmann::json &json_data, std::vector<AgentTask> &agent_tasks) {
    int id = 0;

    nlohmann::json::array_t tasks = json_data["agent_task_history"];


    for(nlohmann::json::object_t task : tasks) {
        if(!task["processed"]) {
            continue;
        }

        AgentTask agent_task = AgentTask();
        agent_task.start_time = task["start_time"];
        agent_task.end_time = task["end_time"];
        agent_task.robot_id = task["robot_id"];

        if(task["team_color"] == "MAGENTA") {
            agent_task.team = llsf_msgs::Team::MAGENTA;
        } else {
            agent_task.team = llsf_msgs::Team::CYAN;
        }

        if(task["task_type"].is_null()) {
            std::cerr << "Task type is null going to ignore this task" << std::endl;
            continue;
        }

        std::string type = task["task_type"];
        nlohmann::json::object_t task_parameter = task["task_parameters"];

        std::shared_ptr<llsf_msgs::AgentTask> msg = std::make_shared<llsf_msgs::AgentTask>();
        if(type == "MOVE") {
            llsf_msgs::Move *move = new llsf_msgs::Move();
            if(!task_parameter["waypoint"].is_null()) {
                std::string waypoint = task_parameter["waypoint"];
                move->set_waypoint(waypoint);
            }
            if(!task_parameter["machine_point"].is_null()) {
                std::string machine_point = task_parameter["machine_point"];
                move->set_machine_point(machine_point);
            }
            msg->set_allocated_move(move);
        } else if (type == "DELIVER") {
            llsf_msgs::Deliver *deliver = new llsf_msgs::Deliver();
            if(!task_parameter["machine_id"].is_null()) {
                std::string machine_id = task_parameter["machine_id"];
                deliver->set_machine_id(machine_id);
            }
            if(!task_parameter["machine_point"].is_null()) {
                std::string machine_point = task_parameter["machine_point"];
                deliver->set_machine_point(machine_point);
            }
            msg->set_allocated_deliver(deliver);
        } else if (type == "BUFFER") {
            llsf_msgs::BufferStation *buffer = new llsf_msgs::BufferStation();
            std::string machine_id = task_parameter["machine_id"];
            int shelf_number = task_parameter["shelf_number"];
            buffer->set_machine_id(machine_id);
            buffer->set_shelf_number(shelf_number);
            msg->set_allocated_buffer(buffer);
        } else if (type == "RETRIEVE") {
            std::string machine_id = task_parameter["machine_id"];
            std::string machine_point = task_parameter["machine_point"];
            llsf_msgs::Retrieve *retrieve = new llsf_msgs::Retrieve();
            retrieve->set_machine_id(machine_id);
            retrieve->set_machine_point(machine_point);
            msg->set_allocated_retrieve(retrieve);
        } else if (type == "EXPLORE") {
            std::string waypoint = task_parameter["waypoint"];
            llsf_msgs::ExploreWaypoint *explore = new llsf_msgs::ExploreWaypoint();
            explore->set_waypoint(waypoint);
            msg->set_allocated_explore_machine(explore);
        } else {
            std::cerr << "Unkown type for a AgentTask going to ignore it: " << type;
            continue;
        }
        agent_task.message = msg;

        agent_task.id = id++;
        agent_task.message->set_task_id(agent_task.id);
        agent_task.message->set_robot_id(agent_task.robot_id);
        agent_task.message->set_team_color(agent_task.team);
        agent_tasks.push_back(agent_task);
    }
}

void gamereport::get_instructions(nlohmann::json &json_data, std::vector<MachineInstruction> &instructions) {
    int id = 1000;
    nlohmann::json::array_t machine_history = json_data["machine_history"];

    std::map<std::string, int> machine_state = std::map<std::string, int>();
    gamereport::set_dict(machine_state);

    for(nlohmann::json::object_t machine : machine_history) {
        std::string name = machine["name"];
        std::string state = machine["state"];
        if(state == "PREPARED") {
            MachineInstruction instruct;
            std::shared_ptr<llsf_msgs::PrepareMachine> prepare = std::make_shared<llsf_msgs::PrepareMachine>();
            prepare->set_machine(name);
            if(name.find("CS") != std::string::npos) {
                llsf_msgs::PrepareInstructionCS *cs = new llsf_msgs::PrepareInstructionCS();
                std::string operation_mode = machine["operation_mode"];
                if(operation_mode == "RETRIEVE_CAP") {
                    cs->set_operation(llsf_msgs::CSOp::RETRIEVE_CAP);
                } else if (operation_mode == "MOUNT_CAP") {
                    cs->set_operation(llsf_msgs::CSOp::MOUNT_CAP);
                } else {
                    std::cerr << "Unknown operation for a CS: " << operation_mode << std::endl;
                    continue;
                }
                prepare->set_allocated_instruction_cs(cs);
            } else if (name.find("RS") != std::string::npos) {
                llsf_msgs::PrepareInstructionRS *rs = new llsf_msgs::PrepareInstructionRS();
                std::string ring_color = machine["current_ring_color"];
                if (ring_color == "RING_BLUE") {
                    rs->set_ring_color(llsf_msgs::RingColor::RING_BLUE);
                } else if (ring_color == "RING_GREEN") {
                    rs->set_ring_color(llsf_msgs::RingColor::RING_GREEN);
                } else if (ring_color == "RING_ORANGE") {
                    rs->set_ring_color(llsf_msgs::RingColor::RING_ORANGE);
                } else if (ring_color == "RING_YELLOW") {
                    rs->set_ring_color(llsf_msgs::RingColor::RING_YELLOW);
                } else {
                    std::cerr << "Unknown Ring color for the RS: " << ring_color << std::endl;
                    continue;
                }
                prepare->set_allocated_instruction_rs(rs);
            } else if (name.find("BS") != std::string::npos) {
                llsf_msgs::PrepareInstructionBS *bs = new llsf_msgs::PrepareInstructionBS();
                std::string current_side = machine["current_side"];
                std::string current_base_color = machine["current_base_color"];
                if (current_side == "INPUT") {
                    bs->set_side(llsf_msgs::MachineSide::INPUT);
                } else if (current_side == "OUTPUT") {
                    bs->set_side(llsf_msgs::MachineSide::OUTPUT);
                } else {
                    std::cerr << "Unknown side for the BS: " << current_side << std::endl;
                    continue;
                }

                if(current_base_color == "BASE_RED") {
                    bs->set_color(llsf_msgs::BaseColor::BASE_RED);
                } else if (current_base_color == "BASE_BLACK") {
                    bs->set_color(llsf_msgs::BaseColor::BASE_BLACK);
                } else if (current_base_color == "BASE_SILVER") {
                    bs->set_color(llsf_msgs::BaseColor::BASE_SILVER);
                } else {
                    std::cerr << "Unknown base color for the BS: " << current_base_color << std::endl;
                    continue;
                }
                prepare->set_allocated_instruction_bs(bs);
            } else if (name.find("DS") != std::string::npos) {
                llsf_msgs::PrepareInstructionDS *ds = new llsf_msgs::PrepareInstructionDS();
                int order_id = machine["order_id"];
                ds->set_order_id(order_id);
                prepare->set_allocated_instruction_ds(ds);
            } else if (name.find("SS") != std::string::npos) {
                llsf_msgs::PrepareInstructionSS *ss = new llsf_msgs::PrepareInstructionSS();
                std::string wp_description = machine["current_wp_description"];
                int shelf = machine["shelf"];
                int slot = machine["slot"];
                ss->set_wp_description(wp_description);
                ss->set_shelf(shelf);
                ss->set_slot(slot);
                std::string operation = machine["current_operation"];
                if (operation == "STORE") {
                    ss->set_operation(llsf_msgs::SSOp::STORE);
                } else if (operation == "RETRIEVE") {
                    ss->set_operation(llsf_msgs::SSOp::RETRIEVE);
                } else if (operation == "CHANGE_INFO") {
                    ss->set_operation(llsf_msgs::SSOp::CHANGE_INFO);
                } else {
                    std::cerr << "Unknown operation for the SS: " << operation << std::endl;
                    continue;
                }
                prepare->set_allocated_instruction_ss(ss);
            } else {
                std::cerr << "Unknown machine type: " << name << std::endl;
                continue;
            }

            instruct.message = prepare;
            instruct.start_time = machine["game_time"];
            if(name.find("M-") != std::string::npos) {
                prepare->set_team_color(llsf_msgs::Team::MAGENTA);
                instruct.team = llsf_msgs::Team::MAGENTA;
            } else {
                prepare->set_team_color(llsf_msgs::Team::CYAN);
                instruct.team = llsf_msgs::Team::CYAN;
            }
            instruct.id = id++;
            instructions.push_back(instruct);
            machine_state[name] = instructions.size() - 1;
        } else if ((state == "READY-AT-OUTPUT" || state == "IDLE" || state == "WAIT-IDLE") && machine_state[name] != -1) {
            int index = machine_state[name];
            instructions[index].end_time = machine["game_time"];
            machine_state[name] = -1;
        }
    }
}

std::string gamereport::base_color_to_text(const llsf_msgs::BaseColor base_color) {
    if (base_color == llsf_msgs::BaseColor::BASE_RED) {
        return "BASE_RED";
    } else if (base_color == llsf_msgs::BaseColor::BASE_SILVER) {
        return "BASE_SILVER";
    } else if (base_color == llsf_msgs::BaseColor::BASE_BLACK) {
        return "BASE_BLACK";
    }  else if (base_color == llsf_msgs::BaseColor::BASE_CLEAR) {
        return "BASE_CLEAR";
    }
    return "BASE_UNKNOWN";
}

std::string gamereport::ring_color_to_text(const llsf_msgs::RingColor ring_color) {
    if (ring_color == llsf_msgs::RingColor::RING_BLUE) {
        return "RING_BLUE";
    } else if (ring_color == llsf_msgs::RingColor::RING_GREEN) {
        return "RING_GREEN";
    } else if (ring_color == llsf_msgs::RingColor::RING_ORANGE) {
        return "RING_ORANGE";
    } else if (ring_color == llsf_msgs::RingColor::RING_YELLOW) {
        return "RING_YELLOW";
    }
    return "RING_UNKNOWN";
}

std::string gamereport::instruction_to_text(std::shared_ptr<llsf_msgs::PrepareMachine> instruction) {
    if(instruction->has_instruction_bs()) {
        const llsf_msgs::PrepareInstructionBS bs = instruction->instruction_bs();
        return "Dispense " + base_color_to_text(bs.color()) + " on " + instruction->machine() + " at " +(bs.side() == llsf_msgs::MachineSide::INPUT ? "INPUT" : "OUPUT");
    } else if (instruction->has_instruction_cs()) {
        const llsf_msgs::PrepareInstructionCS cs = instruction->instruction_cs();
        if(cs.operation() == llsf_msgs::CSOp::RETRIEVE_CAP) {
            return "Retrieving Cap on " + instruction->machine();
        } else {
            return "Munting Cap on " + instruction->machine();
        }
    } else if (instruction->has_instruction_rs()) {
        const llsf_msgs::PrepareInstructionRS rs = instruction->instruction_rs();
        return "Mounting " + ring_color_to_text(rs.ring_color()) + " on " + instruction->machine();
    } else if (instruction->has_instruction_ds()) {
        const llsf_msgs::PrepareInstructionDS ds = instruction->instruction_ds();
        return "Delivering order " + std::to_string(ds.order_id()) + " on " + instruction->machine();
    } else if (instruction->has_instruction_ss()) {
        //TODO
        // const llsf_msgs::PrepareInstructionDS ds = instruction->instruction_ds();
        // return "Delivering order " + std::to_string(ds.order_id()) + " on " + instruction->machine();
    }
    return "NO COMMAND SET";
}

std::string gamereport::agent_task_to_text(std::shared_ptr<llsf_msgs::AgentTask> agent_task) {
    if(agent_task->has_buffer()) {
        llsf_msgs::BufferStation buffer_msg = agent_task->buffer();
        return "Buffer Task at " + buffer_msg.machine_id() + " and shelf number " + std::to_string(buffer_msg.shelf_number());
    } else if (agent_task->has_retrieve()){
        llsf_msgs::Retrieve retrieve_msg = agent_task->retrieve();
        return "Retrieve Task at " + retrieve_msg.machine_id() + " on side " + retrieve_msg.machine_point();
    } else if (agent_task->has_deliver()){
        llsf_msgs::Deliver deliver_msg = agent_task->deliver();
        return "Deliver Task at " + deliver_msg.machine_id() + " on side " + deliver_msg.machine_point();
    } else if (agent_task->has_move()){
        llsf_msgs::Move move_msg = agent_task->move();
        return "Move Task to " + move_msg.waypoint() + (move_msg.has_machine_point() ? " on side " + move_msg.machine_point() : "");
    } else if (agent_task->has_explore_machine()){
        llsf_msgs::ExploreWaypoint explore_msg = agent_task->explore_machine();
        return "Explore Task to " + explore_msg.waypoint();
    }
    return "NO TASK DESCRIPTION";
}
