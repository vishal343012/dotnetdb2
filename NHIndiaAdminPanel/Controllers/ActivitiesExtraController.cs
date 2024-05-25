using Microsoft.AspNetCore.Mvc;
using NHIndiaAdminPanel.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Collections.Generic;

namespace NHIndiaAdminPanel.Controllers
{
    public class ActivitiesExtraController : Controller
    {


        [HttpGet]
        public static List<CountryList> GetCountry()
        {
            return ActivitiesExtraModel.GetCountry();
        }


        [HttpGet]
        public IActionResult GetCity(int countryId)
        {
            var cities = ActivitiesExtraModel.GetCity(countryId);
            return Json(cities);
        }


        [HttpGet]
        public IActionResult GetCityCountryPrefix(string Prefix)
        {
            var cityCountry= ActivitiesExtraModel.GetCityCountryPrefix(Prefix);
            return Json(cityCountry);
        }

    }

    
}
