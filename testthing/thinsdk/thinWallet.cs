using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NEO.AllianceOfThinWallet
{
    public static class thinWallet
    {
        static string api = "https://api.otcgo.cn/testnet";
        static public async Task<int> getHeight()
        {
            WebClient wc = new WebClient();
            var str = await wc.DownloadStringTaskAsync(api + "/height");
            var json = MyJson.Parse(str);
            return json.GetDictItem("height").AsInt();
        }

        static public async Task<MyJson.JsonNode_Object> checkAddressInfo(string address)
        {
            WebClient wc = new WebClient();
            var str = await wc.DownloadStringTaskAsync(api + "/address/" + address);
            var json = MyJson.Parse(str);
            return json as MyJson.JsonNode_Object;
        }

        //差一个获取资产种类列表的API

        static public async Task<MyJson.JsonNode_Object> getAssetKind()
        {
            MyJson.JsonNode_Object obj = new MyJson.JsonNode_Object();
            obj.SetDictValue("c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b", "小蚁股");
            obj.SetDictValue("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7", "小蚁币");
            return obj;
        }

        static public async Task<MyJson.JsonNode_Object> createRawData_Transfer(string addressSource, string[] addressDests, float[] amounts, string assetId)
        {
            WebClient wc = new WebClient();
            var values = new NameValueCollection();
            values["source"] = addressSource;
            values["assetId"] = assetId;
            string dest = addressDests[0];
            string amount = amounts[0].ToString("#0.0000");
            for (var i = 1; i < addressDests.Length; i++)
            {
                dest += "," + addressDests[i];
                amount += "," + amounts[i].ToString("#0.0000");
            }
            values["dests"] = dest;
            values["amounts"] = amount;
            byte[] bts = null;

            bts = await wc.UploadValuesTaskAsync(api + "/transfer", values);

            var str = System.Text.Encoding.UTF8.GetString(bts);
            return MyJson.Parse(str) as MyJson.JsonNode_Object;
        }
        static public async Task<MyJson.JsonNode_Object> boardCast_Transfer(string pubkey_nocomp,string signature,string transaction)
        {
            WebClient wc = new WebClient();
            var values = new NameValueCollection();
            values["publicKey"] = pubkey_nocomp;
            values["signature"] = signature;
            values["transaction"] = transaction;
            byte[] bts = null;
            bts = await wc.UploadValuesTaskAsync(api + "/broadcast", values);

            var str = System.Text.Encoding.UTF8.GetString(bts);
            return MyJson.Parse(str) as MyJson.JsonNode_Object;
        }
    }

}
