#include "Arduino.h"
#define CHECK(value) do { if (!(value)) return __LINE__; } while (0)

unsigned long fakeTime = 0;
int keys[32], sensors[32], outputs[32], soundingFrequency = 0;
FakeSerial Serial;
#include "../../Arduino/HootyFlute/HootyFlute.ino"

void step(unsigned long milliseconds) { fakeTime += milliseconds; loop(); }
void fingering(int mask) {
    for (int i = 0; i < 8; i++) keys[Config::keyPins[i]] = (mask & (1 << i)) ? LOW : HIGH;
    step(1); step(31);
}
bool silent() {
    if (soundingFrequency != 0) return false;
    for (int pin : Config::pumpPins) if (outputs[pin] != LOW) return false;
    return true;
}

int main() {
    for (int& key : keys) key = HIGH;
    for (int& sensor : sensors) sensor = 500;
    setup(); CHECK(silent());
    int decoded = 0;
    for (int mask = 0; mask < 256; mask++) {
        int note = decodeFingering(mask);
        if (note) { decoded++; CHECK(Config::fingerMasks[note - 1] == mask); }
    }
    CHECK(decoded == 8);
    fingering(255); CHECK(silent());
    Serial.send("E,1\n"); step(1);
    CHECK(outputs[10] == HIGH && soundingFrequency == 0);
    sensors[A0] = 440; step(1); step(39); CHECK(soundingFrequency == 0);
    step(1); CHECK(soundingFrequency == 262);
    step(20); CHECK(Serial.contains("HF1,1,440,500,500,1,1,1\n"));
    sensors[A0] = 475; step(1); step(45); CHECK(soundingFrequency == 262);
    sensors[A0] = 500; step(1); step(40); CHECK(soundingFrequency == 0);
    sensors[A0] = 440; step(1); step(40); CHECK(soundingFrequency == 262);
    Serial.send("E,2\n"); step(1); CHECK(silent());
    fingering(254); CHECK(outputs[11] == HIGH && soundingFrequency == 0);
    sensors[A1] = 440; step(1); step(40); CHECK(soundingFrequency == 294);
    step(501); CHECK(silent());
    Serial.send("E,2\n"); step(1); CHECK(soundingFrequency == 0);
    sensors[A1] = 500; step(1); step(40);
    // Heartbeats cannot bypass the continuous pump limit.
    for (int i = 0; i < 16; i++) { Serial.send("E,2\n"); step(100); }
    CHECK(silent());
    Serial.send("E,2\n"); step(100); CHECK(silent());
    fingering(0); fingering(254); Serial.send("E,2\n"); step(1);
    CHECK(outputs[11] == HIGH);
    sensors[A1] = 0; step(1); CHECK(silent());
    sensors[A1] = 500; fingering(0); fingering(160);
    Serial.send("E,8\n"); step(1); CHECK(outputs[10] == HIGH);
    // Existing contact on A0 cannot trigger a newly selected note.
    CHECK(soundingFrequency == 0);
    Serial.send("junkjunkjunkjunkE,8\n"); step(501); CHECK(silent());
    ContactGate gate;
    gate.update(40, 0xfffffff0UL); gate.update(40, 0x20UL);
    CHECK(gate.contact);
    return 0;
}
