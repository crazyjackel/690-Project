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
        TextField IP = Root.Q<TextField>("IP_Textfield");
        TextField Port = Root.Q<TextField>("Port_Textfield");
        TextField UserName = Root.Q<TextField>("User_Textfield");

        //Bind Debug Text to Document
        viewModel.DebugText.BindTo(x => simpleLabel.text = x).AddTo(disposable);

        ConnectButton.BindToClick(viewModel.ConnectCommand).AddTo(disposable);
        LoginButton.BindToClick(viewModel.LoginCommand).AddTo(disposable);
        CreateButton.BindToClick(viewModel.CreateCommand).AddTo(disposable);

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
