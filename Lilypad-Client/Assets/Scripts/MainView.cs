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


        //Bind Debug Text to Document
        viewModel.DebugText.BindTo(x => simpleLabel.text = x).AddTo(disposable);

        ConnectButton.BindToClick(viewModel.ConnectCommand).AddTo(disposable);
    }
}
