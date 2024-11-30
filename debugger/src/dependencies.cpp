#include "dependencies.hpp"

Dependencies::Dependencies() : mutex(), adj(), in() {
}
void Dependencies::add_edge(int from, int to) {
    if(adj.find(from) == adj.end()) {
        adj[from] = std::vector<int>();
    }
    if(adj.find(to) == adj.end()) {
        adj[to] = std::vector<int>();
    }
    if(in.find(from) == in.end()) {
        in[from] = 0;
    }
    if(in.find(to) == in.end()) {
        in[to] = 0;
    }


    adj[from].push_back(to);
    ++in[to];
}

void Dependencies::finish_task(int task) {
    std::lock_guard<std::mutex> lock(mutex);
    for (int next : adj[task]) {
        --in[next];
    }
    in.erase(task);
}

std::vector<int> Dependencies::get_next_tasks() {
    std::lock_guard<std::mutex> lock(mutex);
    std::vector<int> next_tasks;
    for (std::map<int, int>::const_iterator it = in.begin(); it != in.end(); ++it) {
        int task = it->first;
        int in_degree = it->second;
        if (in_degree == 0) {
            next_tasks.push_back(task);
        } else if (in_degree < 0) {
            throw std::runtime_error("Negative in-degree");
        }
    }
    return next_tasks;
}
