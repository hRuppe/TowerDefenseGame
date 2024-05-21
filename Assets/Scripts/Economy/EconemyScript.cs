using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class EconomySetupTest : MonoBehaviour
{
    public async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Replace with currency
}
