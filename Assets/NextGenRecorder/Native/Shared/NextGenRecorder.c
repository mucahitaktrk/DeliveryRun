#include "UnityInterface.h"

extern int ngr_RegisterRenderingPlugin(void* lPtr, void* ulPtr)
{
#if UNITY_VERSION >= 540
    UnityRegisterRenderingPluginV5(lPtr, ulPtr);
    return 1;
#else
    return 0;
#endif
}
