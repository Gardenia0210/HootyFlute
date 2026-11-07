#include "InstrumentLogic.h"

int baseline[3] = {0, 0, 0};
int pressure[3] = {0, 0, 0};
ContactGate gates[3];
uint8_t expectedNote = 0;
uint8_t rawMask = 0;
uint8_t stableMask = 0;
uint8_t previousNote = 0;
uint8_t playingNote = 0;
unsigned long keyChangedAt = 0;
unsigned long lastHostAt = 0;
unsigned long pumpStartedAt = 0;
unsigned long lastTelemetryAt = 0;
bool hostSeen = false;
bool pumpLocked = false;
char command[12];
uint8_t commandLength = 0;
bool commandOverflow = false;

void stopOutputs() {
    for (uint8_t i = 0; i < 3; ++i) digitalWrite(Config::pumpPins[i], LOW);
    noTone(Config::buzzerPin);
    playingNote = 0;
}

void readCommands(unsigned long now) {
    uint8_t budget = 64;
    while (Serial.available() && budget--) {
        char c = Serial.read();
        if (c == '\r') continue;
        if (c == '\n') {
            if (!commandOverflow && commandLength == 3 && command[0] == 'E' &&
                command[1] == ',' && command[2] >= '0' && command[2] <= '8') {
                expectedNote = command[2] - '0';
                lastHostAt = now;
                hostSeen = true;
            }
            commandLength = 0;
            commandOverflow = false;
        } else if (commandLength < sizeof(command) - 1) {
            command[commandLength++] = c;
        } else commandOverflow = true;
    }
}

void setup() {
    for (uint8_t i = 0; i < 8; ++i) pinMode(Config::keyPins[i], INPUT_PULLUP);
    for (uint8_t i = 0; i < 3; ++i) pinMode(Config::pumpPins[i], OUTPUT);
    pinMode(Config::buzzerPin, OUTPUT);
    stopOutputs();
    Serial.begin(115200);
    // Start unloaded. Pumps remain off throughout calibration.
    long sums[3] = {0, 0, 0};
    for (uint8_t sample = 0; sample < 64; ++sample) {
        for (uint8_t i = 0; i < 3; ++i) sums[i] += analogRead(Config::pressurePins[i]);
        delay(5);
    }
    for (uint8_t i = 0; i < 3; ++i) baseline[i] = sums[i] / 64;
}

void loop() {
    unsigned long now = millis();
    readCommands(now);
    bool online = hostSeen && now - lastHostAt <= Config::hostTimeoutMs;
    uint8_t mask = 0;
    for (uint8_t i = 0; i < 8; ++i)
        if (digitalRead(Config::keyPins[i]) == LOW) mask |= 1 << i;
    if (mask != rawMask) { rawMask = mask; keyChangedAt = now; }
    if (now - keyChangedAt >= Config::debounceMs) stableMask = rawMask;
    uint8_t note = decodeFingering(stableMask);
    if (note == 0) pumpLocked = false;
    if (note != previousNote) {
        stopOutputs();
        previousNote = note;
    }

    for (uint8_t i = 0; i < 3; ++i) {
        pressure[i] = analogRead(Config::pressurePins[i]);
        int delta = Config::pressurePolarity[i] * (pressure[i] - baseline[i]);
        gates[i].update(delta, now);
    }
    bool eligible = online && note > 0 && note == expectedNote && !pumpLocked;
    uint8_t pump = note > 0 ? Config::notePump[note - 1] : 0;
    bool sensorValid = pressure[pump] > 1 && pressure[pump] < 1022 &&
        baseline[pump] > 1 && baseline[pump] < 1022;
    eligible = eligible && sensorValid;
    // A contact held before the correct fingering must be released first.
    for (uint8_t i = 0; i < 3; ++i)
        if (gates[i].contact && (!eligible || i != pump)) gates[i].consumed = true;
    static bool pumping = false;
    if (eligible && !pumping) pumpStartedAt = now;
    if (eligible && now - pumpStartedAt >= Config::maxPumpMs) {
        pumpLocked = true;
        eligible = false;
    }
    pumping = eligible;
    for (uint8_t i = 0; i < 3; ++i)
        digitalWrite(Config::pumpPins[i], eligible && i == pump ? HIGH : LOW);

    if (!eligible || !gates[pump].contact || playingNote != note) {
        noTone(Config::buzzerPin);
        playingNote = 0;
    }
    if (eligible && gates[pump].contact && !gates[pump].consumed) {
        gates[pump].consumed = true;
        playingNote = note;
        tone(Config::buzzerPin, Config::frequency[note - 1]);
    }
    if (now - lastTelemetryAt >= Config::telemetryMs) {
        lastTelemetryAt = now;
        Serial.print("HF1,"); Serial.print(note);
        for (uint8_t i = 0; i < 3; ++i) { Serial.print(','); Serial.print(pressure[i]); }
        Serial.print(','); Serial.print(note > 0 && gates[pump].contact ? 1 : 0);
        Serial.print(','); Serial.print(playingNote > 0 ? 1 : 0);
        Serial.println(",1");
    }
}
