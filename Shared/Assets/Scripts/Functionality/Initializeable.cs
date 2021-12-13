using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Initializeable
{
    bool CanInitialize();
    void TryInitialize(bool requireIntiailization = false);
    void Initialize();
    void DeInitialize();
}
