#pragma once
#include "InstrumentConfig.h"

inline uint8_t decodeFingering(uint8_t mask) {
    for (uint8_t i = 0; i < 8; ++i)
        if (mask == Config::fingerMasks[i]) return i + 1;
    return 0;
}

// Contact is hysteretic and must be released before another note can sound.
class ContactGate {
public:
    bool contact = false;
    bool consumed = false;
    bool pending = false;
    unsigned long changedAt = 0;

    void update(int delta, unsigned long now) {
        bool next = contact ? delta > Config::contactOff : delta >= Config::contactOn;
        if (next != pending) { pending = next; changedAt = now; }
        if (pending != contact && now - changedAt >= Config::contactDebounceMs) {
            contact = pending;
            if (!contact) consumed = false;
        }
    }
};
