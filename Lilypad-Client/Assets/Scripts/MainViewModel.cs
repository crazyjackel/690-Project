using Enjin.SDK.Core;
using MLAPI;
using MLAPI.Transports.UNET;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MainViewModel : ViewModel<MainViewModel>
{
    [SerializeField]
    public int MaxLength = 100000;


    private ReactiveCommand _connectCommand;
    public ReactiveCommand ConnectCommand => _connectCommand;

    private ReactiveCommand _loginCommand;
    public ReactiveCommand LoginCommand => _loginCommand;

    private ReactiveCommand _createCommand;
    public ReactiveCommand CreateCommand => _createCommand;

    private ReactiveCommand _pingCommand;
    public ReactiveCommand PingCommand => _pingCommand;

    private ReactiveCommand _mintCommand;
    public ReactiveCommand MintCommand => _mintCommand;

    private ReactiveProperty<string> _connectAddress = new ReactiveProperty<string>("127.0.0.1");
    public ReactiveProperty<string> ConnectAddress => _connectAddress;


    private ReactiveProperty<int> _connectPort = new ReactiveProperty<int>(7777);
    public ReactiveProperty<int> ConnectPort => _connectPort;

    private ReactiveProperty<string> _userName = new ReactiveProperty<string>();
    public ReactiveProperty<string> UserName => _userName;


    private ReactiveProperty<Texture2D> _qrCode = new ReactiveProperty<Texture2D>();
    public ReactiveProperty<Texture2D> QRCode => _qrCode;

    private ReactiveProperty<string> _debugText = new ReactiveProperty<string>("");
    public ReactiveProperty<string> DebugText => _debugText;

    private NetworkManager _network => _networkProp.Value;
    private ReactiveProperty<NetworkManager> _networkProp = new ReactiveProperty<NetworkManager>();
    private ClientNetworkPortal _clientPortal => _clientProp.Value;
    private ReactiveProperty<ClientNetworkPortal> _clientProp = new ReactiveProperty<ClientNetworkPortal>();
    private UNetTransport _transport => _transportProp.Value;
    private ReactiveProperty<UNetTransport> _transportProp = new ReactiveProperty<UNetTransport>();

    private ClientEnjinManager _enjin => _enjinProp.Value;
    private ReactiveProperty<ClientEnjinManager> _enjinProp = new ReactiveProperty<ClientEnjinManager>();
    private ReactiveProperty<bool> _isConnected = new ReactiveProperty<bool>(false);
    private ReactiveProperty<bool> _isLogin = new ReactiveProperty<bool>(false);

    public override void OnInitialization()
    {
        _enjinProp.Where(x => x != null).Subscribe(cem =>
        {
            cem.QRCode.BindTo(_qrCode);

            cem.isLogin.BindTo(_isLogin);

            cem.isConnected.Subscribe(ic =>
            {
                _isConnected.Value = ic;
                if (ic) cem.UpdateAccessToken();
            });
        });

        


        _connectCommand = new ReactiveCommand(_networkProp.CombineLatest(_transportProp, _clientProp, (x, y, z) => x && y && z));
        _connectCommand.Subscribe(_ =>
        {
            _transport.ConnectPort = _connectPort.Value;
            _transport.ConnectAddress = _connectAddress.Value;
            _clientPortal.Connect(_network);
        });

        _loginCommand = new ReactiveCommand(
            _enjinProp
            .CombineLatest(_userName, _isConnected, (enj, us, tk) => (enj != null) && (us != null) && (us != "") && tk));
        _loginCommand.Subscribe(_ =>
        {
            _enjin.Login(_userName.Value);
        });

        _pingCommand = new ReactiveCommand(_isConnected);
        _pingCommand.Subscribe(_ =>
        {
            _enjin.PingServer();
        });

        _mintCommand = new ReactiveCommand(_isLogin);
        _mintCommand.Subscribe(_ =>
        {
            _enjin.MintItem();
        });

        _createCommand = new ReactiveCommand(
            _enjinProp
            .CombineLatest(_userName, _isConnected, (enj, us, tk) => (enj != null) && (us != null) && (us != "") && tk));
        _createCommand.Subscribe(_ =>
        {
            _enjin.CreateUser(_userName.Value);
        });
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Application.logMessageReceived += Application_logMessageReceived;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Application.logMessageReceived -= Application_logMessageReceived;
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        _debugText.Value += $"<color={GetColorFromLogType(type)}>[{Enum.GetName(typeof(LogType), type)}] {condition}</color>\n";
        if (_debugText.Value.Length > MaxLength)
        {
            _debugText.Value = _debugText.Value.Substring(_debugText.Value.IndexOf('\n'));
        }
    }

    public string GetColorFromLogType(LogType type)
    {
        switch (type)
        {
            case LogType.Exception:
            case LogType.Error:
                return "#FF0000";
            case LogType.Warning:
                return "#FFFF00";
            case LogType.Log:
            case LogType.Assert:
            default:
                return "#FFFFFF";
        }
    }

    public override void NewProviderAvailable(IProvider newProvider)
    {
        base.NewProviderAvailable(newProvider);

        DepInjector.MapProvider<NetworkManagerProvider, NetworkManager>(newProvider, _networkProp);
        DepInjector.MapProvider<TransportToProvider, UNetTransport>(newProvider, _transportProp);
        DepInjector.MapProvider(newProvider, _clientProp);
        DepInjector.MapProvider(newProvider, _enjinProp);
    }
}
