#pragma once
#include <Arduino.h>

namespace Config {
const uint8_t keyPins[8] = {2, 3, 4, 5, 6, 7, 8, 9};
const uint8_t pumpPins[3] = {10, 11, 12};
const uint8_t pressurePins[3] = {A0, A1, A2};
const uint8_t buzzerPin = 13;
// Exact original fingering tables, including high C on keys 6 and 8.
const uint8_t fingerMasks[8] = {255, 254, 252, 248, 240, 224, 192, 160};
const uint8_t notePump[8] = {0, 1, 2, 0, 1, 2, 0, 0};
const uint16_t frequency[8] = {262, 294, 330, 349, 392, 440, 494, 523};
// The supplied analog module example reports decreasing ADC values under load.
// Calibrate these engineering defaults against the actual unloaded hardware.
const int8_t pressurePolarity[3] = {-1, -1, -1};
const int contactOn = 40;
const int contactOff = 20;
const unsigned long debounceMs = 30;
const unsigned long contactDebounceMs = 40;
const unsigned long hostTimeoutMs = 500;
const unsigned long maxPumpMs = 1500;
const unsigned long telemetryMs = 20;
}
