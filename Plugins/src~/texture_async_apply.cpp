#include <cstdint>
#include <unordered_map>

#include "IUnityInterface.h"
#include "IUnityRenderingExtensions.h"


std::unordered_map<unsigned int, void *> bufferMap;
unsigned int lastIndex;

static void event_handler(int eventId, UnityRenderingExtTextureUpdateParamsV2* data) {
    if (eventId == kUnityRenderingExtEventUpdateTextureBeginV2) {
        auto it = bufferMap.find(data->userData);
        if (it != bufferMap.end()) {
            data->texData = it->second;
        }
    }
}

extern "C" UNITY_INTERFACE_EXPORT void *TextureAsyncApply_event_handler() {
    return (void *) &event_handler;
}

extern "C" UNITY_INTERFACE_EXPORT unsigned int TextureAsyncApply_new(void *buffer) {
    bufferMap[++lastIndex] = buffer;
    return lastIndex;
}

extern "C" UNITY_INTERFACE_EXPORT void TextureAsyncApply_dispose(unsigned int index) {
    bufferMap.erase(index);
}
