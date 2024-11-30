#include "MachineInfo.pb.h"
#include "dependencies.hpp"
#include "gamereport.hpp"

#include "Team.pb.h"
#include "AgentTask.pb.h"
#include "MachineInstructions.pb.h"
#include "MachineDescription.pb.h"
#include "ProductColor.pb.h"
#include "GameState.pb.h"

#include <memory>
#include <protobuf_comm/message_register.h>
#include <protobuf_comm/peer.h>

#include <fstream>
#include <iostream>
#include <string>
#include <nlohmann/json.hpp>
#include <vector>


Dependencies dep = Dependencies();
std::vector<MachineInstruction> instructions;
std::vector<AgentTask> agent_tasks;
volatile bool game_started = false;
std::map<std::string, int> machine_task;

std::vector<int> finished_tasks = std::vector<int>();
std::mutex finished_mutex = std::mutex();

void handle_peer_msg(boost::asio::ip::udp::endpoint &, uint16_t, uint16_t,
                     std::shared_ptr<google::protobuf::Message> msg_ptr) {
    if(!msg_ptr) {
        std::cerr << "Received message is empty" << std::endl;
        return;
    }

    std::lock_guard<std::mutex> lock(finished_mutex);

    const google::protobuf::Descriptor *descriptor = msg_ptr->GetDescriptor();
    if(descriptor->name() == "GameState") {
        const llsf_msgs::GameState *gamestate_msg = dynamic_cast<llsf_msgs::GameState *>(msg_ptr.get());
        if(gamestate_msg->phase() == llsf_msgs::GameState_Phase::GameState_Phase_PRODUCTION &&
            gamestate_msg->state() == llsf_msgs::GameState_State::GameState_State_RUNNING) {
            game_started = true;
        } else {
            game_started = false;
        }
    } else if (descriptor->name() == "AgentTask") {
        const llsf_msgs::AgentTask *agent_task_msg = dynamic_cast<llsf_msgs::AgentTask *>(msg_ptr.get());
        if (agent_task_msg->successful() && std::find(finished_tasks.begin(), finished_tasks.end(), agent_task_msg->task_id()) == finished_tasks.end()){
            std::cout << "Finished " << gamereport::agent_task_to_text(agent_tasks[agent_task_msg->task_id()].message) << std::endl;
            dep.finish_task(agent_task_msg->task_id());
            finished_tasks.push_back(agent_task_msg->task_id());
        } else if ((agent_task_msg->error_code() != 0 || agent_task_msg->cancel_task()) && std::find(finished_tasks.begin(), finished_tasks.end(), agent_task_msg->task_id()) == finished_tasks.end()) {
            std::cout << "Failed " << gamereport::agent_task_to_text(agent_tasks[agent_task_msg->task_id()].message) << std::endl;
            dep.finish_task(agent_task_msg->task_id());
            finished_tasks.push_back(agent_task_msg->task_id());
        }
    } else if (descriptor->name() == "MachineInfo") {
        const llsf_msgs::MachineInfo *machine_info_msg = dynamic_cast<llsf_msgs::MachineInfo *>(msg_ptr.get());

        for(llsf_msgs::Machine machine : machine_info_msg->machines()) {
            if (machine_task[machine.name()] != -1 && machine_task[machine.name()] < 10000 && machine.state() == "IDLE") {
                dep.finish_task(machine_task[machine.name()]);
                std::cout << "Finished " << gamereport::instruction_to_text(instructions[machine_task[machine.name()] - 1000].message) << std::endl;
                machine_task[machine.name()] = -1;
            }
            if(machine_task[machine.name()] >= 10000 && machine.state() != "IDLE") {
                machine_task[machine.name()] -= 10000;
            }
        }
    }
}

int main(int argc, char *argv[]) {
    int id = 0;
    const std::string default_path = "../latest";

    std::string path = (argc > 1) ? argv[1] : default_path;

    std::cout << "Using path: " << path << std::endl;

    std::ifstream file(path);
    if(!file.is_open()) {
        std::cerr << "Failed to open the file: " << path << std::endl;
        return 0;
    }

    nlohmann::json json_data;
    file >> json_data;

    std::string report_name = json_data["report_name"];
    std::cout << "make sure to load the report in the refbox with the name: " << report_name << std::endl;

    agent_tasks = std::vector<AgentTask>();
    gamereport::get_tasks(json_data, agent_tasks);


    instructions = std::vector<MachineInstruction>();
    gamereport::get_instructions(json_data, instructions);

    for (AgentTask agent_task : agent_tasks) {
        for (AgentTask cmp_task : agent_tasks) {
            if (agent_task.id == cmp_task.id) {
                continue;
            }

            if (cmp_task.end_time < agent_task.start_time) {
                dep.add_edge(cmp_task.id, agent_task.id);
            }
        }

        for (MachineInstruction instruction : instructions) {
            if (instruction.end_time < agent_task.start_time) {
                dep.add_edge(instruction.id, agent_task.id);
            }
        }
    }

    for (MachineInstruction instruction : instructions) {
        for (AgentTask agent_task : agent_tasks) {
            if (agent_task.end_time < instruction.start_time) {
                dep.add_edge(agent_task.id, instruction.id);
            }
        }

        for (MachineInstruction cmp_instruction : instructions) {
            if(cmp_instruction.id == instruction.id) {
                continue;
            }

            if (cmp_instruction.end_time < instruction.start_time) {
                dep.add_edge(instruction.id, instruction.id);
            }
        }
    }

    machine_task = std::map<std::string, int>();
    gamereport::set_dict(machine_task);

    std::vector<std::string> proto_dirs = { "../../Simulator/protobuf/rcll-protobuf-msgs" };
    std::shared_ptr<protobuf_comm::MessageRegister> message_register_ =
        std::make_shared<protobuf_comm::MessageRegister>(proto_dirs);

    //TODO TEAM SELECT
    std::string peer_address_ = "127.0.0.1";
    unsigned short recv_port_magenta_ = 4442;
    unsigned short recv_port_cyan_ = 4441;
    unsigned short recv_port_public_ = 4444;
    unsigned short send_port_public_ = 4445;
    unsigned short send_port_cyan_ = 4446;
    unsigned short send_port_magenta_ = 4447;
    std::shared_ptr<protobuf_comm::ProtobufBroadcastPeer> refbox_private_peer_ =
        std::make_shared<protobuf_comm::ProtobufBroadcastPeer>(peer_address_, send_port_magenta_, recv_port_magenta_, message_register_.get(), "randomkey", "aes-256-cbc");

    refbox_private_peer_->signal_received().connect(boost::bind(
        &handle_peer_msg, boost::placeholders::_1, boost::placeholders::_2,
        boost::placeholders::_3, boost::placeholders::_4));

    std::shared_ptr<protobuf_comm::ProtobufBroadcastPeer> refbox_public_peer_ =
        std::make_shared<protobuf_comm::ProtobufBroadcastPeer>(peer_address_, send_port_public_, recv_port_public_, message_register_.get());

    refbox_public_peer_->signal_received().connect(boost::bind(
        &handle_peer_msg, boost::placeholders::_1, boost::placeholders::_2,
        boost::placeholders::_3, boost::placeholders::_4));
    std::map<int, std::shared_ptr<protobuf_comm::ProtobufBroadcastPeer>> robot_peers = std::map<int, std::shared_ptr<protobuf_comm::ProtobufBroadcastPeer>>();

    for(int i = 1; i < 4; i++) {
        for(int team = 0; team < 2; team++) {
            int team_offset = team == 0 ? 0 : 3;
            std::shared_ptr<protobuf_comm::ProtobufBroadcastPeer> robot_peer_ =
                std::make_shared<protobuf_comm::ProtobufBroadcastPeer>(peer_address_, 2125 + i + team_offset, 2115 + i + team_offset, message_register_.get());//, "randomkey", "aes-256-cbc");
            robot_peers[team * 10 + i] = robot_peer_;
        }
    }

    for(std::pair<int, std::shared_ptr<protobuf_comm::ProtobufBroadcastPeer>> peer : robot_peers) {
        peer.second->signal_received().connect(boost::bind(
            &handle_peer_msg, boost::placeholders::_1, boost::placeholders::_2,
            boost::placeholders::_3, boost::placeholders::_4));
    }

    std::vector<int> send = std::vector<int>();

    while(true) {
        if(!game_started) {
            std::this_thread::sleep_for(std::chrono::milliseconds(100));
            continue;
        }
        if(send.size() == instructions.size() + agent_tasks.size()) {
            std::cout << "All tasks send!" << std::endl;
            std::this_thread::sleep_for(std::chrono::milliseconds(1000));
            break;
        }
        for(int task_id : dep.get_next_tasks()) {
            if(std::find(send.begin(), send.end(),  task_id) == send.end()) {
                if(task_id >= 1000) {
                    std::vector<MachineInstruction>::iterator instruction = std::find_if(instructions.begin(), instructions.end(), [task_id](const MachineInstruction& instruction) {
                        return instruction.id == task_id;
                    });

                    if(instruction == instructions.end()) {
                        std::cerr << "No task with ID: " << task_id << " Found!" << std::endl;
                        continue;
                    }
                    machine_task[instruction->message->machine()] = task_id + 10000;
                    std::cout << "Seding " << gamereport::instruction_to_text(instruction->message) << std::endl;
                    std::string wait;
                    std::cin >> wait;
                    refbox_private_peer_->send(instruction->message);
                    send.push_back(task_id);
                } else {
                    std::vector<AgentTask>::iterator agent_task = std::find_if(agent_tasks.begin(), agent_tasks.end(), [task_id](const AgentTask& agent_task) {
                        return agent_task.id == task_id;
                    });

                    if(agent_task == agent_tasks.end()) {
                        std::cerr << "No task with ID: " << task_id << " Found!" << std::endl;
                        continue;
                    }

                    std::cout << "Sending " <<
                        (agent_task->team == llsf_msgs::Team::CYAN ? "CYAN" : "MAGENTA")
                        << " Robot " << agent_task->robot_id << " a " << gamereport::agent_task_to_text(agent_task->message) << std::endl;
                    std::string wait;
                    std::cin >> wait;
                    robot_peers[agent_task->robot_id]->send(agent_task->message);
                    send.push_back(task_id);
                }
            }
        }
        std::this_thread::sleep_for(std::chrono::milliseconds(100));
    }
}
