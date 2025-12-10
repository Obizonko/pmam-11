#include <iostream>
#include <vector>
#include <string>
#include <memory>
#include <iomanip>

// --- Abstract Base Class ---
class Device {
protected:
    std::string id;
    bool isActive;

public:
    Device(std::string name) : id(name), isActive(false) {}
    virtual ~Device() {}

    std::string getId() const { return id; }
    bool getStatus() const { return isActive; }

    void toggle() {
        isActive = !isActive;
        std::cout << "[LOG] " << id << " is now " << (isActive ? "ON" : "OFF") << std::endl;
    }

    // Pure virtual function
    virtual void performAction() = 0;

    virtual void getInfo() const {
        std::cout << "- Device: " << id << " | Status: " << (isActive ? "ON" : "OFF");
    }
};

// --- Smart Light ---
class SmartLight : public Device {
    int brightness;
    std::string color;

public:
    SmartLight(std::string name) : Device(name), brightness(50), color("White") {}

    void setBrightness(int level) {
        if (level < 0) level = 0;
        if (level > 100) level = 100;
        brightness = level;
    }

    void setColor(std::string c) { color = c; }

    void performAction() override {
        if (!isActive) {
            std::cout << id << " is OFF. Cannot perform action." << std::endl;
            return;
        }
        std::cout << id << " is glowing " << color << " at " << brightness << "% brightness." << std::endl;
    }

    void getInfo() const override {
        Device::getInfo();
        std::cout << " | Color: " << color << " | Brightness: " << brightness << "%" << std::endl;
    }
};

// --- Thermostat ---
class Thermostat : public Device {
    double temperature;
    double targetTemp;

public:
    Thermostat(std::string name) : Device(name), temperature(20.0), targetTemp(22.0) {}

    void setTarget(double temp) { targetTemp = temp; }

    void performAction() override {
        if (!isActive) return;

        if (temperature < targetTemp) {
            temperature += 0.5;
            std::cout << id << " Heating... Current: " << temperature << "C (Target: " << targetTemp << "C)" << std::endl;
        }
        else if (temperature > targetTemp) {
            temperature -= 0.5;
            std::cout << id << " Cooling... Current: " << temperature << "C (Target: " << targetTemp << "C)" << std::endl;
        }
        else {
            std::cout << id << " Idle. Target temperature reached." << std::endl;
        }
    }

    void getInfo() const override {
        Device::getInfo();
        std::cout << " | Temp: " << temperature << "C" << std::endl;
    }
};

// --- Security Camera ---
class SecurityCamera : public Device {
    bool isRecording;

public:
    SecurityCamera(std::string name) : Device(name), isRecording(false) {}

    void performAction() override {
        if (isActive) {
            isRecording = true;
            std::cout << id << " is scanning sector... [REC]" << std::endl;
        }
        else {
            isRecording = false;
        }
    }

    void getInfo() const override {
        Device::getInfo();
        std::cout << " | Recording: " << (isRecording ? "YES" : "NO") << std::endl;
    }
};

// --- Controller Hub ---
class HomeHub {
    std::vector<std::unique_ptr<Device>> devices;

public:
    void addDevice(std::unique_ptr<Device> dev) {
        devices.push_back(std::move(dev));
    }

    void masterSwitch(bool turnOn) {
        std::cout << "\n--- MASTER SWITCH " << (turnOn ? "ON" : "OFF") << " ---" << std::endl;
        for (const auto& dev : devices) {
            if (dev->getStatus() != turnOn) {
                dev->toggle();
            }
        }
    }

    void runRoutine() {
        std::cout << "\n--- RUNNING ROUTINE ---" << std::endl;
        for (const auto& dev : devices) {
            dev->performAction();
        }
    }

    void showStatus() {
        std::cout << "\n--- SYSTEM STATUS ---" << std::endl;
        for (const auto& dev : devices) {
            dev->getInfo();
        }
        std::cout << "---------------------" << std::endl;
    }

    // Access specific device for demo purposes (unsafe cast for simplicity)
    Device* getDevice(int index) {
        if (index >= 0 && index < devices.size()) {
            return devices[index].get();
        }
        return nullptr;
    }
};

int main() {
    HomeHub myHome;

    // Adding devices
    myHome.addDevice(std::make_unique<SmartLight>("Living Room Light"));
    myHome.addDevice(std::make_unique<Thermostat>("Main Thermostat"));
    myHome.addDevice(std::make_unique<SecurityCamera>("Front Door Cam"));

    // Demo Scenario
    myHome.showStatus();

    // Turn everything ON
    myHome.masterSwitch(true);

    // Configure specific devices
    SmartLight* light = dynamic_cast<SmartLight*>(myHome.getDevice(0));
    if (light) {
        light->setColor("Warm Yellow");
        light->setBrightness(85);
    }

    Thermostat* therm = dynamic_cast<Thermostat*>(myHome.getDevice(1));
    if (therm) {
        therm->setTarget(25.0);
    }

    // Simulate functionality
    myHome.runRoutine();
    myHome.runRoutine(); // Run again to see temp change

    myHome.showStatus();

    // Turn OFF
    myHome.masterSwitch(false);

    return 0;
}