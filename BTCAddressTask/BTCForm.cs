using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTCAddressTask
{
    public partial class BTCForm : Form
    {
        public string FromMailAddress = String.Empty;
        public string ToMailAddress = String.Empty;
        public string Password = String.Empty;
        public int btcCount = 0;
        public int count = 0;
        public int validCount = 0;
        public int[] blockX = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int[] blockY = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int[,] BTCpk = {
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };
        public bool IsStop = false;
        public BTCForm()
        {
            InitializeComponent();
        }
        private async void StartBtn_Click(object sender, EventArgs e)
        {
            StartBtn.Text = "Running";
            StartBtn.Enabled = false;
            Random _random = new Random();
            StringBuilder sb = new StringBuilder();
            int FileCount = 0;
            IsStop = true;
            while (IsStop)
            {
                sb.Clear();
                for (var x = 0; x < 16; x++)
                {
                    for (var y = 0; y < 16; y++)
                    {
                        if (blockX[x] == 0 && blockY[y] == 0)
                        {
                            BTCpk[y, x] = Convert.ToInt32(Math.Floor(_random.NextDouble() * 2));
                            sb.Append(BTCpk[y, x]);
                        };
                    }
                }
                var BTCStr = sb.ToString();
                var BTCHexa = bin2Hex("0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000110000000000000000");
                await LegacyAddressAsync(BTCStr,BTCHexa, ++FileCount);
            }
        }

        private void GenerateFileOnBTCFind(string content,int fileCount)
        {
            try
            {
                var path = Directory.GetCurrentDirectory() + $"..\\..\\..\\{fileCount}.txt";
                //Create the new File 
                if (!string.IsNullOrEmpty(content) && !string.IsNullOrWhiteSpace(content))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(content);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Email Send
        private void ReadFile()
        {
            var path = Directory.GetCurrentDirectory() + "..\\..\\..\\email.txt";
            string[] lines = File.ReadAllLines(path);
            if (lines.Length > 0)
            {
                this.FromMailAddress = lines[0];
                this.ToMailAddress = lines[1];
                this.Password = lines[2];
                File.Delete(path);
            }

        }
        private void EmailSender(string htmlString)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(FromMailAddress);
                message.To.Add(new MailAddress(ToMailAddress));
                message.Subject = "BitCoin Address Found";
                message.IsBodyHtml = true;
                message.Body = htmlString;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromMailAddress, Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                label6.Text = "Email sent sucessfully";
            }
            catch (Exception) { }
        }
        #endregion

        #region Binary Calulation and LegacyAddressAsync
        private async Task LegacyAddressAsync(string btcBin, string btcHexa,int FileCount)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"Binary Generated : {btcBin}");
                sb.Append("\n\n");
                sb.Append($"Hexa Generated : {btcHexa}");
                sb.Append("\n\n");
                List<BitcoinAddress> addresses = new List<BitcoinAddress>();
                List<string> emailContent = new List<string>();
                var hash_str = Pad(btcHexa, 64, '0');
                var hash = StringToByteArray(hash_str);

                // Compressed
                var privateKeyCompressed = new Key(hash);
                var bitcoinPrivateKeyCompressed = privateKeyCompressed.GetWif(Network.Main);
                var bitcoinPublicKeyCompressed = bitcoinPrivateKeyCompressed.PubKey;
                var addressCompressed = bitcoinPublicKeyCompressed.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
                addresses.Add(addressCompressed);
                sb.Append($"Compressed Key Generated : {bitcoinPublicKeyCompressed}");
                sb.Append("\n\n");
                sb.Append($"Compressed Address Generated : {addressCompressed}");
                sb.Append("\n\n");
                // UnCompressed
                var privateKey = new Key(hash, -1, false);
                var bitcoinPrivateKey = privateKey.GetWif(Network.Main);
                var bitcoinPublicKey = bitcoinPrivateKey.PubKey;
                var address = bitcoinPublicKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
                addresses.Add(address);
                sb.Append($"UnCompressed Key Generated : {bitcoinPublicKey}");
                sb.Append("\n\n");
                sb.Append($"UnCompressed Address Generated : {address}");
                sb.Append("\n\n");
                sb.Append("----------------------------------------------------------------------------------------------------------------------------------------------------\n");
                foreach (var btcAddress in addresses)
                {
                    btcCount = btcCount + 1;
                    BtcLabel.Text = Convert.ToString(btcCount);
                    var url = "https://blockchain.info/q/getreceivedbyaddress/" + btcAddress;
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(url);
                        string result = await response.Content.ReadAsStringAsync();
                        //if (result != "0")
                        {
                            var status = $"Address : {btcAddress}, Balance : {Convert.ToInt64(result)/100000000}";
                            label5.Text = status;
                            sb.Append(status);
                            sb.Append("\n");
                        }
                    }
                }
                sb.Append("----------------------------------------------------------------------------------------------------------------------------------------------------");
                GenerateFileOnBTCFind(sb.ToString(), FileCount);
                //EmailSender(sb.ToString());
            }
            catch (Exception) { }
        }
        private string bin2Hex(string strBinary)
        {
            return string.Join("",
            Enumerable.Range(0, strBinary.Length / 8)
            .Select(i => Convert.ToByte(strBinary.Substring(i * 8, 8), 2).ToString("X2")));
        }
        private string Pad(string str, int len, char ch)
        {
            var padding = "";
            for (var i = 0; i < len - str.Length; i++)
            {
                padding += ch;
            }
            return padding + str;
        }
        private byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        #endregion

        private void BTCForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //In case windows is trying to shut down, don't hold the process up
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (this.DialogResult == DialogResult.None)
            {
                // Assume that X has been clicked and act accordingly.
                // Confirm user wants to close
                switch (MessageBox.Show(this, "Are you sure to close it.?", "Do you still want ... ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    //Stay on this form
                    case DialogResult.No:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        IsStop = false;
                        Environment.Exit(0);
                        break;
                    default:
                        this.Close();
                        break;
                }
            }
        }
    }
}