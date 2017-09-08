using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;


namespace BarcodePOC
{
	public partial class MainPage : ContentPage
	{
        public string BarCodeResult
        {
            get;
            set;
        }

		public MainPage()
		{
			InitializeComponent();
		}

        async void OnScanRequest(object sender, EventArgs e)
        {
            var scanPage = new ZXingScannerPage();

            scanPage.OnScanResult += (result) => {
                // Stop scanning
                scanPage.IsScanning = false;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                    BarCodeResult = result.Text;
                    PostBarCode(BarCodeResult);
                    DisplayAlert("Scanned Barcode", result.Text, "OK");
                });
            };
            

            // Navigate to our scanner page
            try
            {
                await Navigation.PushAsync(scanPage);
            }
            catch (Exception ex){
                await this.DisplayAlert(
                    "Error",
                    "Exception: "+ex.Message,
                    "Ok!");
            }

        }

        async void TestButton(object sender, EventArgs e)
        {
            await this.DisplayAlert(
                    "OnScanRequest",
                    "Barcode scan requested!",
                    "Ok!");
        }

        public void PostBarCode(string barCode)
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://poccodingtaskwebapi20170831020758.azurewebsites.net/");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            var bcModel = new BarcodeModel();

            bcModel.Barcode = barCode;

            string contents = "{\"Barcode\":"+barCode+"}";

            var stringContent = new StringContent(contents, Encoding.UTF8, "application/json");
			var response = client.PostAsync("api/BarCode/CreateNewBarcode/", stringContent).Result;

			if (response.IsSuccessStatusCode)
			{
				DisplayAlert("Barcode Added","BarcodeAdded","Ok!");
				
			}
			else
			{
                DisplayAlert("Barcode Error", "Error Code" +
				response.StatusCode + " : Message - " + response.ReasonPhrase, "Error!");
			}
        }
    }

    public class BarcodeModel
    {
        public int BarcodeID
        {
            get;
            set;
        }

		public string Barcode
		{
			get;
			set;
		}

        public DateTime CreatedAt
        {
            get;
            set;
        }
        public int CreatedBy
        {
            get;
            set;
        }

    }
}
