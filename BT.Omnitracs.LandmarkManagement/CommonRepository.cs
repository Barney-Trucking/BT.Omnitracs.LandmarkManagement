using BT.Omnitracs.LandmarkManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT.Omnitracs.LandmarkManagement
{
    public class CommonRepository
    {
        private string connApp = ConfigurationManager.ConnectionStrings["BTApp"].ConnectionString;

        public ObservableCollection<TmwGeofence> GetTmwGeofences()
        {
            ObservableCollection<TmwGeofence> result = new ObservableCollection<TmwGeofence>();
            using (SqlConnection conn = new SqlConnection(connApp))
            {
                SqlCommand cmd = new SqlCommand("TMW_Geofences_GetAll", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    TmwGeofence geo = new TmwGeofence();
                    geo.Name = rdr[0].ToString();
                    geo.Lat = Convert.ToDouble(rdr[1]);
                    geo.Lon = Convert.ToDouble(rdr[2]);
                    if (rdr[3] != DBNull.Value)
                    {
                        geo.Street = rdr[3].ToString();
                    }
                    else
                    {
                        geo.Street = "";
                    }
                    geo.City = rdr[4].ToString();
                    geo.State = rdr[5].ToString();
                    if (rdr[6] != DBNull.Value)
                    {
                        geo.Zip = rdr[6].ToString();
                    }
                    else
                    {
                        geo.Zip = "";
                    }                    
                    if (rdr[7] != DBNull.Value)
                    {
                        if (Convert.ToDecimal(rdr[7]) < Convert.ToDecimal(.01))
                        {
                            geo.Radius = 0;
                        }
                        else
                        {
                            geo.Radius = Convert.ToDouble(rdr[7]);
                        }
                    }
                    else
                    {
                        geo.Radius = 0;
                    }                   
                    geo.Id = rdr[8].ToString();
                    geo.AtHomeLoc = rdr[9].ToString();

                    result.Add(geo);
                }
                conn.Close();
            }
            return result;
        }
    }
}
