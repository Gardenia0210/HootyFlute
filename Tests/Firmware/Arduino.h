#pragma once
typedef unsigned char uint8_t;
typedef signed char int8_t;
typedef unsigned short uint16_t;

const int LOW = 0, HIGH = 1, OUTPUT = 1, INPUT_PULLUP = 2;
const int A0 = 14, A1 = 15, A2 = 16;
extern unsigned long fakeTime;
extern int keys[32], sensors[32], outputs[32], soundingFrequency;
inline void pinMode(int, int) {}
inline int digitalRead(int pin) { return keys[pin]; }
inline int analogRead(int pin) { return sensors[pin]; }
inline void digitalWrite(int pin, int value) { outputs[pin] = value; }
inline void noTone(int) { soundingFrequency = 0; }
inline void tone(int, int frequency) { soundingFrequency = frequency; }
inline unsigned long millis() { return fakeTime; }
inline void delay(unsigned long value) { fakeTime += value; }

struct FakeSerial {
    char input[256];
    char output[32768];
    int inputRead = 0, inputWrite = 0, outputLength = 0;
    void begin(int) {}
    int available() { return inputWrite - inputRead; }
    char read() { return input[(inputRead++) % 256]; }
    void send(const char* text) { while (*text) input[(inputWrite++) % 256] = *text++; }
    void print(const char* value) { while (*value) print(*value++); }
    void print(char value) { if (outputLength < 32767) output[outputLength++] = value; }
    void print(int value) {
        if (value < 0) { print('-'); value = -value; }
        if (value >= 10) print(value / 10);
        print(static_cast<char>('0' + value % 10));
    }
    void println(const char* value) { print(value); print('\n'); }
    bool contains(const char* value) {
        for (int i = 0; i < outputLength; i++) {
            int j = 0;
            while (value[j] && i + j < outputLength && output[i + j] == value[j]) j++;
            if (!value[j]) return true;
        }
        return false;
    }
};
extern FakeSerial Serial;
