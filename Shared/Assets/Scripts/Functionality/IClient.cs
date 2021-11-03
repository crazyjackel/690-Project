using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClient
{
    void NewProviderAvailable(IProvider newProvider);
    void ProviderRemoved(IProvider removeProvider);
    void NewProviderFullyInstalled(IProvider newProvider);
}
