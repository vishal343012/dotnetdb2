using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using NHIndiaAdminPanel.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHIndiaAdminPanel.Controllers
{
    public class PackageController : Controller
    {
        public IActionResult List()
        {
            ViewBag.PackageTypes = GetPackageType();
            ViewBag.CountryList = ActivitiesExtraController.GetCountry();
            return View();
        }
        public IActionResult Create(int PackageId = 0)
        {
            ViewBag.PackageId = PackageId;
            ViewBag.PackageTypes = GetPackageType();
            ViewBag.CancellationPolicies = GetCancellationPolicyList();

            return View();
        }
        public IActionResult PackageType()
        {
            return View();
        }

        public IActionResult CityMap(int PackageId)
        {
            if (PackageId <= 0)
            {
                return RedirectToAction("List");
            }
            ViewBag.PackageId = PackageId;
            ViewBag.CityList = GetPackageCityMap(PackageId);
            return View();
        }

        public IActionResult Description(int PackageId)
        {
            if (PackageId <= 0)
            {
                return RedirectToAction("List");
            }
            ViewBag.PackageId = PackageId;
            return View();
        }

        public IActionResult Inclusion(int PackageId)
        {
            if (PackageId <= 0)
            {
                return RedirectToAction("List");
            }
            ViewBag.PackageId = PackageId;
            ViewBag.PackageInclusions = GetPackageInclusion(PackageId);
            return View();
        }
        public IActionResult Price(int PackageId)
        {

            try
            {
                if (PackageId <= 0)
                {
                    return RedirectToAction("List");
                }
                PackagePriceRootEnt packagePrice = PackageModel.GetPackagePrice(PackageId, 0);

                if (packagePrice != null)
                {
                    ViewBag.PackageId = PackageId;
                    ViewBag.PackageSettings = packagePrice.Setting;
                    ViewBag.PackageOccupancy = packagePrice.Occupancy;
                    ViewBag.PackageDetails = packagePrice.Detail;
                }
                else
                {
                    TempData["Message"] = "Failed to retrieve package price or slabs data.";
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle it appropriately
                TempData["Message"] = "An error occurred while retrieving package price or slabs data.";
                Console.WriteLine(ex.Message);
            }

            // Return the view
            return View();
        }

        public IActionResult Images(int PackageId)
        {
            if (PackageId <= 0)
            {
                return RedirectToAction("List");
            }
            ViewBag.PackageId = PackageId;
            ViewBag.PackageImages = GetPackagesImages(PackageId);
            return View();
        }

        [HttpGet]
        public static PackageDetailEnt GetPackage(int PackageId)
        {
            return PackageModel.GetPackage(PackageId);

        }

        [HttpPost]
        public IActionResult SavePackageDetails(PackageDetailEnt package)
        {

            if (package == null)
            {
                TempData["Message"] = "Please fill all ditels.";
                return RedirectToAction("Create");
            }
            if (package.CancellationPolicyId == 0)
            {
                TempData["Message"] = "Please select Cancellation Policy.";
                if (package.PackageId == 0)
                {
                    return RedirectToAction("Create");
                }
                return RedirectToAction("Create", new { PackageId = package.PackageId });
            }
            if (package.TypeId == 0)
            {
                TempData["Message"] = "Please select Package Type.";
                if (package.PackageId == 0)
                {
                    return RedirectToAction("Create");
                }
                return RedirectToAction("Create", new { PackageId = package.PackageId });
            }
            var result = PackageModel.SavePackageDetails(package);

            if (result.Status != -1 && result.Status != 0)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("Create", new { PackageId = result.Status });
            }
            else
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        public static List<PackageTypeEnt> GetPackageType()
        {
            return PackageModel.GetPackageType();
        }

        [HttpPost]
        public IActionResult SavePackageType(PackageTypeEnt packageType)
        {
            if (string.IsNullOrEmpty(packageType.PackageTypeName))
            {
                TempData["Message"] = "PackageType Name is required";
                return RedirectToAction("PackageType");
            }

            if (ModelState.IsValid)
            {

                var result = PackageModel.SavePackageType(packageType);


                if (result.Status == 1)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction("PackageType");
                }
                else
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction("PackageType");
                }
            }


            return RedirectToAction("PackageType");
        }

        [HttpGet]
        public static List<CancellationPolicyEnt> GetCancellationPolicyList()
        {
            return PackageModel.GetCancellationPolicyList();
        }

        [HttpPost]
        public IActionResult UploadPdfs(IFormCollection form)
        {
            try
            {
                var files = form.Files;
                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (!PackageModel.IsPdf(file))
                        {
                            return Json("Not a Valid PDF");
                        }


                        string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);

                        // Define base path and file path
                        string basePath = @"C:\NH\TP_Project";
                        string filePath = Path.Combine("PdfFiles", form["PackageId"]);

                        Directory.CreateDirectory(Path.Combine(basePath, filePath));

                        string fullPath = Path.Combine(basePath, filePath, fileName);

                        // Save the file
                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        string finalPath = Path.Combine(filePath, fileName);
                        return Json(new MessageStatus { Status = 1, Message = "File Uploaded Successfully!~" + finalPath.Replace('\\', '/') });

                    }
                }
                else
                {
                    return Json(new MessageStatus { Status = 0, Message = "No files selected." });
                }
            }
            catch (Exception ex)
            {
                return Json(new MessageStatus { Status = -1, Message = "Error details: " + ex.Message });
            }
            return Json(new MessageStatus { Status = -1, Message = "Error: Something went wrong please try again" });
        }

        [HttpGet]
        public IActionResult GetPackageDescription(int PackageId)
        {
            var packageDescription = PackageModel.GetPackageDescription(PackageId);
            return Ok(packageDescription);
        }

        [HttpPost]
        public async Task<IActionResult> SavePackageDescription()
        {
            string requestBody;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Manually deserialize the JSON string into PackageDescriptionEnt
            PackageDescriptionEnt sData;
            try
            {
                sData = JsonConvert.DeserializeObject<PackageDescriptionEnt>(requestBody);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                TempData["Message"] = "Something went wrong!";
                return Json(new { success = false, message = "Something went wrong!" });
            }

            if (sData == null || sData.PackageId == 0)
            {
                TempData["Message"] = "Invalid PackageId or missing data!";
                return Json(new { success = false, message = "Invalid PackageId or missing data!" });
            }

            var result = PackageModel.SavePackageDescription(sData);

            if (result.Status == sData.PackageId)
            {
                TempData["Message"] = result.Message;
                return Json(new { success = true, message = result.Message });
            }
            else
            {
                TempData["Message"] = result.Message;
                return Json(new { success = false, message = result.Message });
            }
        }


        [HttpGet]
        public List<PackageCityMapEnt> GetPackageCityMap(int PackageId)
        {
            return PackageModel.GetPackageCityMap(PackageId);
        }

        [HttpPost]
        public IActionResult SavePackageCityMap(PackageCityMapEnt data)
        {
            if (data == null)
            {
                TempData["Message"] = "Something went wrong!";
                return RedirectToAction("Create");

            }
            else if (data.Nights == 0)
            {
                TempData["Message"] = "Nights not be Zero(0)";
                return RedirectToAction("CityMap", new { PackageId = data.PackageId });
            }
            else if (data.HotelName == "" || data.HotelName == null)
            {
                TempData["Message"] = "please enter Hotel name!";
                return RedirectToAction("CityMap", new { PackageId = data.PackageId });
            }

            if (data.CityId == 0)
            {
                TempData["Message"] = "please select city from list!";
                return RedirectToAction("CityMap", new { PackageId = data.PackageId });
            }
            var result = PackageModel.SavePackageCityMap(data);
            if (result.Status == data.PackageId)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("CityMap", new { PackageId = data.PackageId });
            }
            else
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("CityMap", new { PackageId = data.PackageId });
            }
        }

        [HttpPost]
        public IActionResult RemoveCityMap(PackageCityMapEnt data)
        {

            if (data.PackageId == 0 || data.CityMapId == 0 || data == null)
            {
                TempData["Message"] = "Something went wrong!";
                return RedirectToAction("Create");
            }
            var result = PackageModel.RemoveCityMap(data);
            if (result.Status == data.PackageId)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("CityMap", new { PackageId = data.PackageId });
            }
            else
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("CityMap", new { PackageId = data.PackageId });
            }
        }

        [HttpGet]
        public List<PackageInclusionEnt> GetPackageInclusion(int PackageId)
        {
            return PackageModel.GetPackageInclusion(PackageId);
        }
        [HttpPost]
        public async Task<IActionResult> SavePackageInclusion()
        {
            // Read the request body asynchronously
            string requestBody;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Manually deserialize the JSON string into List<PackageInclusionEnt>
            List<PackageInclusionEnt> inclusions;
            try
            {
                inclusions = JsonConvert.DeserializeObject<List<PackageInclusionEnt>>(requestBody);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                TempData["Message"] = "Something went wrong!";
                return Json(new { success = false, message = "Something went wrong!" });
            }

            if (inclusions == null || inclusions.Count == 0)
            {
                TempData["Message"] = "Something went wrong!";
                return Json(new { success = false, message = "Something went wrong!" });
            }

            var result = PackageModel.SavePackageInclusion(inclusions);
            if (result.Status == inclusions[0].PackageId)
            {
                TempData["Message"] = result.Message;
                return Json(new { success = true, message = result.Message });
            }
            else
            {
                TempData["Message"] = result.Message;
                return Json(new { success = false, message = result.Message });
            }
        }

        [HttpGet]
        public PackagePriceRootEnt GetPackagePrice(int PackageId, int SlabId)
        {
            return PackageModel.GetPackagePrice(PackageId, SlabId);
        }


        [HttpGet]
        public List<Dictionary<string, object>> GetPackageSlabs(int PackageId)
        {
            return PackageModel.GetPackageSlabs(PackageId);
        }


        [HttpPost]
        public async Task<IActionResult> SaveSlab()
        {
            string requestBody;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Manually deserialize the JSON string into List<PackageInclusionEnt>
            PackageSlab data;
            try
            {
                data = JsonConvert.DeserializeObject<PackageSlab>(requestBody);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                TempData["Message"] = "Something went wrong!";
                return Json(new { success = false, message = "Something went wrong!" });
            }

            if (data == null || data.Date == "")
            {
                TempData["Message"] = "Something went wrong!";
                return Json(new { success = false, message = "Something went wrong!" });
            }

            var result = PackageModel.SaveSlab(data);

            if (result.Status == data.PackageId)
            {
                TempData["Message"] = result.Message;
                return Json(new { success = true, message = result.Message });
            }
            else
            {
                TempData["Message"] = result.Message;
                return Json(new { success = false, message = result.Message });
            }
        }

        [HttpGet]
        public MessageStatus DeleteSlab(int SlabId)
        {
            return PackageModel.DeleteSlab(SlabId);
        }


        [HttpPost]
        public async Task<IActionResult> SaveDynamicSlab()
        {
            string requestBody;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Manually deserialize the JSON string into List<PackageInclusionEnt>
            PackageDynamicSlab data;
            try
            {
                data = JsonConvert.DeserializeObject<PackageDynamicSlab>(requestBody);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                TempData["Message"] = "Something went wrong!";
                return Json(new { success = false, message = "Something went wrong!" });
            }

            if (data == null || data.SlabName == "")
            {
                TempData["Message"] = "Something went wrong!";
                return Json(new { success = false, message = "Something went wrong!" });
            }

            var result = PackageModel.SaveDynamicSlab(data);

            if (result.Status == data.PackageId)
            {
                TempData["Message"] = result.Message;
                return Json(new { success = true, message = result.Message });
            }
            else
            {
                TempData["Message"] = result.Message;
                return Json(new { success = false, message = result.Message });
            }
        }


        [HttpGet]
        public List<Dictionary<string, object>> GetPackageDynamicSlabs(int PackageId)
        {
            return PackageModel.GetPackageDynamicSlabs(PackageId);
        }

        [HttpGet]
        public MessageStatus DeleteDynamicSlab(int SlabId)
        {
            return PackageModel.DeleteDynamicSlab(SlabId);
        }


        [HttpPost]
        public async Task<IActionResult> SavePackagePrice()
        {
            string requestBody;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Manually deserialize the JSON string into PackagePriceRootEnt
            PackagePriceRootEnt data;
            try
            {
                data = JsonConvert.DeserializeObject<PackagePriceRootEnt>(requestBody);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                TempData["Message"] = "Something went wrong!";
                return Json(new { success = false, message = "Something went wrong!" });
            }

            var result = PackageModel.SavePackagePrice(data);

            if (result.Status == data.Setting.PackageId)
            {
                TempData["Message"] = result.Message;
                return Json(new { success = true, message = result.Message });
            }
            else
            {
                TempData["Message"] = result.Message;
                return Json(new { success = false, message = result.Message });
            }
        }

        [HttpGet]
        public List<Dictionary<string, object>> GetPackagesImages(int PackageId)
        {
            return PackageModel.GetPackagesImages(PackageId);
        }

        [HttpPost]
        public ActionResult UploadFiles(List<IFormFile> files, string packageid, string alternatetext)
        {
            if (files.Count > 0)
            {
                try
                {
                    foreach (var file in files)
                    {
                        if (!file.ContentType.StartsWith("image/"))
                        {
                            return Json(new { status = 0, message = "Not a valid image file." });
                        }

                        //string basePath = @"C:\NH\TP_Project";


                        //string basePath = @"C:\NH\NHIndia_new_project\NHIndiaAdminPanel\NHIndiaAdminPanel\wwwroot\img";
                        string basePath = @"";
                        string relativeDirectoryPath = Path.Combine("packageimages", packageid);
                        string fullDirectoryPath = Path.Combine(basePath, relativeDirectoryPath);

                        if (!Directory.Exists(fullDirectoryPath))
                        {
                            Directory.CreateDirectory(fullDirectoryPath);
                        }

                        // Generate a unique file name
                        string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);

                        // Combine the full directory path and file name to get the full file path
                        string fullPath = Path.Combine(fullDirectoryPath, fileName);

                        // Use a file stream to save the file
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // Construct the relative file path to be saved in the database
                        string finalPath = Path.Combine(relativeDirectoryPath, fileName).Replace("\\", "/");

                        // Save the image information in the database
                        var status = PackageModel.SaveImage(Convert.ToInt32(packageid), finalPath, alternatetext);
                    }

                    return Json(new { status = 1, message = "File(s) uploaded successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { status = 0, message = $"An error occurred: {ex.Message}" });
                }
            }
            else
            {
                return Json(new { status = 0, message = "No files selected." });
            }
        }


        [HttpPost]
        public ActionResult UpdateAlternateText(int imageId, string alternateText)
        {
            try
            {
                var status = PackageModel.UpdateAlternateText(imageId, alternateText);
                return Json(new { status = status ? 1 : 0 });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult ImageAction(int ImageId, string imageAction)
        {
            var result = PackageModel.ImageAction(ImageId, imageAction);
            return Json(result);
        }

        [HttpPost]
        public ActionResult<(List<PackageListItem> PackageList, int TotalPage)> GetPackageList([FromBody] SPackageList data)
        {
            try
            {
                var (packages, totalPage) = PackageModel.GetPackageList(data);
                return Ok(new { packages, totalPage });
            }
            catch (Exception ex)
            {
                // Log the exception or handle accordingly
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult UpdatePackageStatus(int PackageId, int NewStatus)
        {
            var result = PackageModel.UpdatePackageStatus(PackageId, NewStatus);
            return Json(result);
        }





    }
}
