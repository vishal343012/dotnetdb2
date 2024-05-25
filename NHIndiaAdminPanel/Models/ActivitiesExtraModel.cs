using Backend.Models;
using Dapper;
using Jitbit.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace NHIndiaAdminPanel.Models
{
    public class ActivitiesExtraModel
    {
        public static List<CountryList> GetCountry()
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "TP_GetCountryCity";

                    var parameters = new DynamicParameters();
                    parameters.Add("@Operation", "Country");

                    // Ensure using Dapper's Query method correctly
                    var queryResult = cn.Query<CountryList>(SqlString, parameters, commandType: CommandType.StoredProcedure);

                    // Convert to List explicitly
                    return queryResult.ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                // Returning an empty list in case of an exception
                return new List<CountryList>();
            }
        }

        public static List<CityList> GetCity(int CountryId)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "TP_GetCountryCity";

                    var parameters = new DynamicParameters();
                    parameters.Add("@Operation", "City");
                    parameters.Add("@CountryId", CountryId);

                    // Ensure using Dapper's Query method correctly
                    var queryResult = cn.Query<CityList>(SqlString, parameters, commandType: CommandType.StoredProcedure);

                    // Convert to List explicitly
                    return queryResult.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<CityList>();
            }
        }

        public static List<CityCountryPrefix> GetCityCountryPrefix(string Prefix)
        {
            List<CityCountryPrefix> cityCountryList = new List<CityCountryPrefix>();

            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Prefix", Prefix);

                    // Ensure using Dapper's Query method correctly
                    cityCountryList = cn.Query<CityCountryPrefix>("GetCityCountryPrefix_Package", parameters, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }

            return cityCountryList;
        }
    }

    public class CountryList
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MobileCode { get; set; }
    }

    public class CityList
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class CityCountryPrefix
    {
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }
    }
}
