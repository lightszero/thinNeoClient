
using NEO.AllianceOfThinWallet;
using NEO.AllianceOfThinWallet.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace testthing
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        bool bExit = false;
        async void InitHeight()
        {
            while (bExit == false)
            {
                int height = await thinWallet.getHeight();
                this.labelHeight.Content = height;
                await Task.Delay(1000);
            }


        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitHeight();


        }
        async void CheckAddress(string address)
        {
            var allasset = await thinWallet.getAssetKind();//获取资产种类
            //var str = allasset.ToString();

            var info = await thinWallet.checkAddressInfo(address);
            this.listAddressInfo.Items.Clear();
            if (info.Count == 0)
            {
                this.listAddressInfo.Items.Add("地址无信息");
                return;
            }

            this.listAddressInfo.Items.Add("address:" + info["_id"].AsString());
            this.listAddressInfo.Items.Add("财产");



            foreach (var b in info["balances"].asDict())
            {
                if (allasset.ContainsKey(b.Key))
                {
                    this.listAddressInfo.Items.Add("资产:" + allasset[b.Key].AsString() + " 资产数量:" + b.Value);
                }
                else
                {
                    this.listAddressInfo.Items.Add("资产id:" + b.Key + " 资产数量:" + b.Value);
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckAddress(this.txtAddress.Text);


        }

        private void txtAddress_Copy1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            CheckAddress(this.txtAddress2.Text);
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            CheckAddress(this.txtAddress3.Text);

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var json = await thinWallet.createRawData_Transfer(this.txtSender.Text, new string[] { this.txtRecv.Text }, new float[] { float.Parse(this.txtAmount.Text) },
                this.txtAsset.Text);

            this.txtRaw.Text = json["transaction"].AsString();

        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {//广播交易
            var json = await thinWallet.boardCast_Transfer(publicKey_NoComp, this.txtBroadRaw.Text, this.txtRaw.Text);
            if(json["result"].AsBool()==true)
            {
                MessageBox.Show("转账成功");
            }
            else
            {
                MessageBox.Show("转账失败");
            }
        }
        byte[] privateKey;
        byte[] publicKey;
        string publicKey_NoComp;
        string address;
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {//解析wif

            var txt = this.txtwif.Text;
            var bytes_PrivateKey = Helper.GetPrivateKeyFromWIF(txt);
            var bytes_PublicKey = Helper.GetPublicKeyFromPrivateKey(bytes_PrivateKey);
            publicKey_NoComp = Helper.Bytes2HexString( Helper.GetPublicKeyFromPrivateKey_NoComp(bytes_PrivateKey));
            var bytes_PublicKeyHash = Helper.GetPublicKeyHash(bytes_PublicKey);
            this.address = Helper.GetAddressFromPublicKey(bytes_PublicKey);
            this.privateKey = bytes_PrivateKey;
            this.publicKey = bytes_PublicKey;

            this.listhash.Items.Clear();
            this.listhash.Items.Add("private key:" + Helper.Bytes2HexString(bytes_PrivateKey));
            this.listhash.Items.Add("public key:" + Helper.Bytes2HexString(bytes_PublicKey));
            this.listhash.Items.Add("publickeyhash:" + Helper.Bytes2HexString(bytes_PublicKeyHash));
            this.listhash.Items.Add("publickey_nocomp:" +(publicKey_NoComp));
            this.listhash.Items.Add("address:" + address);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var data = Helper.HexString2Bytes(this.txtRaw.Text);

            var signdata = Helper.Sign(data, this.privateKey);
            this.txtBroadRaw.Text = Helper.Bytes2HexString(signdata);
        }
    }
}
