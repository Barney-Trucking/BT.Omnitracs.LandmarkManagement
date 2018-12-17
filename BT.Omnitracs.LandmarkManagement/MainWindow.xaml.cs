using System;
using System.Collections.Generic;
using System.Linq;
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
using BT.Omnitracs.LandmarkManagement.OLOXWebSvcs;
using System.ServiceModel.Security;
using System.ServiceModel;

namespace BT.Omnitracs.LandmarkManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string companyLoginId = "barneytru1";
        private string userName = "BTAPP";
        private string password = "Barney123$";

        private CommonRepository rep = new CommonRepository();

        public MainWindow()
        {
            InitializeComponent();
            //ProcessNewLandmarks();
            GetLandmarkInfo();
            //CreateLandmarks();
            //DeleteLandmark();
            //DeleteBtAppLocations();
        }

        public void CreateLandmarks()
        {
            var geofences = rep.GetTmwGeofences();

            var client = getOloxClient();
            int num = 0;

            foreach (var item in geofences)
            {
                var landmark = new Landmark();
                string alias;
                //landmark.name = "SCOTTSDALE CITY WATER TREATMENT PLANT";
                if (item.AtHomeLoc == "Y")
                {
                    //if (item.Name.Length > 25)
                    //{
                    //    landmark.name = item.Name.Remove(25);
                    //}
                    //else
                    //{
                    //    landmark.name = item.Name;
                    //}

                    landmark.typeID = "dropYard";
                }
                else
                {
                    //landmark.name = item.Name;
                    //if (item.Name.Length > 22)
                    //{
                    //    landmark.name = item.Name.Remove(22);
                    //}
                    //StringBuilder nameSB = new StringBuilder(landmark.name);
                    //nameSB.Append("@BT");
                    //landmark.name = nameSB.ToString();
                    landmark.typeID = "customer";
                }
                landmark.name = item.Id;
                StringBuilder nameSB = new StringBuilder(landmark.name);
                nameSB.Append("@BT");
                landmark.name = nameSB.ToString();
                landmark.address = new Address();
                landmark.address.street = item.Street;
                landmark.address.city = item.City;
                landmark.address.state = item.State;
                landmark.address.country = "US";
                if (item.Zip.Length >= 5)
                {
                    landmark.address.postalCode = item.Zip;
                }
                else
                {
                    landmark.address.postalCode = "84654";
                }           
                landmark.useInProxRef = "1";
                landmark.circCentroidLat = item.Lat;
                landmark.circCentroidLong = item.Lon;
                landmark.circRadius = item.Radius;
                landmark.divisionID = "";
                client.createLandmark("", "C", landmark);
                alias = item.Name;
                if (item.Name.Length > 23)
                {
                    alias = item.Name.Remove(23);
                }
                StringBuilder aliasSB = new StringBuilder(alias);
                aliasSB.Append(num);
                alias = aliasSB.ToString();
                client.addLandmarkAlias("", landmark.name, "", alias);
                if (num == 99)
                {
                    num = 0;
                }
                else
                {
                    num++;
                };
            }

        }

        public void DeleteBtAppLocations()
        {
            var client = getOloxClient();
            var landmarks = GetLandmarkInfo();
            foreach (var item in landmarks)
            {
                if (item.createUser == "BT APP")
                {
                    client.deleteLandmark("", item.name, "");
                }
            }
        }

        public void DeleteLandmark()
        {
            var client = getOloxClient();

            client.deleteLandmark("", "tmp", "");
        }

        public void ProcessNewLandmarks()
        {
            var companies = rep.GetTmwGeofences();
            List<Models.TmwGeofence> fencesToAdd = new List<Models.TmwGeofence>();
            var lMarks = GetListOfLandmarks()
                .Where(ln => ln.name.EndsWith("@BT"))
                .ToList();
            foreach (var cmp in companies)
            {
                var lmark = lMarks
                    .Where(lm => lm.name.Contains(cmp.Id))
                    .FirstOrDefault();
                if (lmark == null)
                {
                    fencesToAdd.Add(cmp);
                }
            }
            int seqNum = 0;
            foreach (var fence in fencesToAdd)
            {
                CreateLandMark(fence, seqNum);
                if (seqNum == 99)
                {
                    seqNum = 0;
                }
                else
                {
                    seqNum++;
                }
            }
        }

        public bool CreateLandMark(Models.TmwGeofence tmwGeofence, int seqNum)
        {
            bool result = false;
            var client = getOloxClient();
            var landmark = new Landmark();
            string alias;
            if (tmwGeofence.AtHomeLoc == "Y")
            {

                landmark.typeID = "dropYard";
            }
            else
            {
                landmark.typeID = "customer";
            }
            landmark.name = tmwGeofence.Id;
            StringBuilder nameSB = new StringBuilder(landmark.name);
            nameSB.Append("@BT");
            landmark.name = nameSB.ToString();
            landmark.address = new Address();
            landmark.address.street = tmwGeofence.Street;
            landmark.address.city = tmwGeofence.City;
            landmark.address.state = tmwGeofence.State;
            landmark.address.country = "US";
            if (tmwGeofence.Zip.Length >= 5)
            {
                landmark.address.postalCode = tmwGeofence.Zip;
            }
            else
            {
                landmark.address.postalCode = "84654";
            }
            landmark.useInProxRef = "1";
            landmark.circCentroidLat = tmwGeofence.Lat;
            landmark.circCentroidLong = tmwGeofence.Lon;
            if (tmwGeofence.Radius == 0)
            {
                landmark.circRadius = .5;
            }
            else
            {
                landmark.circRadius = tmwGeofence.Radius;
            }            
            landmark.divisionID = "";
            client.createLandmark("", "C", landmark);
            alias = tmwGeofence.Name;
            if (tmwGeofence.Name.Length > 23)
            {
                alias = tmwGeofence.Name.Remove(23);
            }
            StringBuilder aliasSB = new StringBuilder(alias);
            aliasSB.Append(seqNum);
            alias = aliasSB.ToString();
            client.addLandmarkAlias("", landmark.name, "", alias);
            return result = true;
        }

        public List<Landmark> GetLandmarkInfo()
        {
            var lNames = GetListOfLandmarks()
                .Where(ln => ln.name.EndsWith("@BT"))
                .ToList();
            var client = getOloxClient();
            List<Landmark> landmarks = new List<Landmark>();

            foreach (var item in lNames)
            {
                var lDetails = client.getLandmarkDetails(companyLoginId, item.name, "");
                landmarks.Add(lDetails);
            }
            return landmarks;
        }

        private LandmarkNames[] GetListOfLandmarks()
        {
            var client = getOloxClient();

            var result = client.getLandmarkNamesList("", "", 10000);

            return result;
        }

        private OLOXWebSvcsClient getOloxClient()
        {
            string endpointUri = @"https://services.omnitracs.com/oloxWebWS/services/OLOXWebSvcs/wsdl/OLOXWebSvcs.wsdl";
            endpointUri = @"https://services.omnitracs.com/oloxWebWS/services/OLOXWebSvcs";

            var endpoint = new System.ServiceModel.EndpointAddress(endpointUri);

            // Create binding using HTTPS
            var securityElement = System.ServiceModel.Channels.SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            // This Web server requires timestamps
            securityElement.IncludeTimestamp = true;
            // This web server doens't return security header info with the response, so we have to set EnableUnsecuredResponse
            securityElement.EnableUnsecuredResponse = true;
            // other settings required by this web server to return valid data.
            securityElement.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            securityElement.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;

            // This Web Service only support SOAP 1.1
            var encodingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement(System.ServiceModel.Channels.MessageVersion.Soap11, System.Text.Encoding.UTF8);

            var transportElement = new System.ServiceModel.Channels.HttpsTransportBindingElement();
            transportElement.MaxReceivedMessageSize = 20000000; // 20 megs

            var binding = new System.ServiceModel.Channels.CustomBinding(securityElement, encodingElement, transportElement);

            var client = new OLOXWebSvcsClient(binding, endpoint);

            client.ClientCredentials.UserName.UserName = userName + "@" + companyLoginId;
            client.ClientCredentials.UserName.Password = password;

            return client;
        }
    }
}
