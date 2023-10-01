#include <iostream>
#include <vector>
#include <string>
#include <thread>
#include <atomic>
#include <chrono>
#include <sys/sysinfo.h>
#include <openssl/md5.h>

// you should choose which charset to use
const std::string charsetMax = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+-*/?!@#$%^&*()_+=-{}[]|:;<>?~";
const std::string charset = "abcdefghijklmnopqrstuvwxyz0123456789";
const std::string charsetMin = "0123456789";

const std::int8_t MAXLen = 8; // Maximum length
std::string target_hash = "YOUR_TARGET_HASH_HERE";
std::atomic<int> hash_count(0);
bool done = false;

std::string hashInput(const std::string &input) {
    unsigned char result[MD5_DIGEST_LENGTH];
    MD5((unsigned char*)input.c_str(), input.size(), result);
    char charArray[2*MD5_DIGEST_LENGTH+1];
    for (int i = 0; i < MD5_DIGEST_LENGTH; ++i) {
        sprintf(&(charArray[i*2]), "%02x", result[i]);
    }
    return std::string(charArray).substr(10, 20);
}

void generateStringsIterative(int maxLen, char start, char end) {
    for (int length = 1; length <= maxLen; length++) {
        std::string current(length, start);
        while (!done && current.length() == length) {
            if (hashInput(current) == target_hash) {
                std::cout << "Match found for " << current << " with hash " << target_hash << std::endl;
                done = true;
                return;
            }
            hash_count++;

            for (int idx = length - 1; idx >= 0; idx--) {
                if (current[idx] < end) {
                    current[idx]++;
                    break;
                } else if (idx > 0) {
                    current[idx] = start;
                } else {
                    current.clear();
                }
            }
        }
    }
}

void workerFunction(int id, int totalThreads) {
    int len = charset.length();
    int charsPerThread = len / totalThreads;
    char startChar = charset[id * charsPerThread];
    char endChar = (id == totalThreads - 1) ? charset[len - 1] : charset[(id + 1) * charsPerThread - 1];

    generateStringsIterative(MAXLen, startChar, endChar);
}

void showStatus() {
    auto start_time = std::chrono::steady_clock::now();

    while (!done) {
        std::this_thread::sleep_for(std::chrono::seconds(5));

        struct sysinfo info;
        sysinfo(&info);
        double memory_usage = (info.totalram - info.freeram) * 100.0 / info.totalram;

        auto elapsed_time = std::chrono::duration_cast<std::chrono::seconds>(std::chrono::steady_clock::now() - start_time).count();
        int current_hash_count = hash_count.load();
        double hash_per_second = (double)current_hash_count / elapsed_time;

        std::cout << "Memory Usage: " << memory_usage << "%, "
                  << "Total Hashes: " << current_hash_count << ", "
                  << "Hashes/second: " << hash_per_second << std::endl;
    }
}

int main() {
    std::thread statusThread(showStatus);

    int numThreads = std::thread::hardware_concurrency();
    std::vector<std::thread> workers;

    for (int i = 0; i < numThreads; ++i) {
        workers.push_back(std::thread(workerFunction, i, numThreads));
    }

    for (auto &th : workers) {
        th.join();
    }
    statusThread.join();

    return 0;
}
