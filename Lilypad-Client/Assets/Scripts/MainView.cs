using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

public class MainView : View<MainViewModel>
{
    public override void OnActivation(MainViewModel viewModel, CompositeDisposable disposable)
    {
        Label simpleLabel = Root.Q<Label>("Debug_Label");
        Button ConnectButton = Root.Q<Button>("Connect_Button");
        Button LoginButton = Root.Q<Button>("Login_Button");
        Button CreateButton = Root.Q<Button>("Create_Button");
        Button PingServer = Root.Q<Button>("Ping_Button");
        Button closeTab = Root.Q<Button>("Tab_Button");
        TextField IP = Root.Q<TextField>("IP_Textfield");
        TextField Port = Root.Q<TextField>("Port_Textfield");
        TextField UserName = Root.Q<TextField>("User_Textfield");
        VisualElement image = Root.Q<VisualElement>("Image_Texture");
        VisualElement container = Root.Q<VisualElement>("Container");

        container.visible = false;

        closeTab.RegisterCallback<ClickEvent>(evt => container.visible = false);
        disposable.Add(
            Disposable.Create(() => closeTab.UnregisterCallback<ClickEvent>(evt => container.visible = false)));
        

        viewModel.QRCode.BindTo(x =>
        {
            if (x != null)
            {
                image.style.backgroundImage = x;
                image.style.width = 300;
                image.style.height = 300;
                container.visible = true;
            }
        }).AddTo(disposable);
        //Bind Debug Text to Document
        viewModel.DebugText.BindTo(x => simpleLabel.text = x).AddTo(disposable);

        ConnectButton.BindToClick(viewModel.ConnectCommand).AddTo(disposable);
        LoginButton.BindToClick(viewModel.LoginCommand).AddTo(disposable);
        CreateButton.BindToClick(viewModel.CreateCommand).AddTo(disposable);
        PingServer.BindToClick(viewModel.PingCommand).AddTo(disposable);

        IP.BindToTextLabel(viewModel.ConnectAddress).AddTo(disposable);
        Port.BindToTextLabel(viewModel.ConnectPort, (newVal, oldVal) =>
        {
            if (int.TryParse(newVal, out int res))
            {
                return res;
            }
            return oldVal;
        }).AddTo(disposable);
        UserName.BindToTextLabel(viewModel.UserName).AddTo(disposable);
    }
}
