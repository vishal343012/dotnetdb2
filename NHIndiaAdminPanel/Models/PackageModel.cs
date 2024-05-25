using Jitbit.Utils;
using NHIndiaAdminPanel.Controllers;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using System.Net.NetworkInformation;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;


namespace NHIndiaAdminPanel.Models
{
    public class PackageModel
    {
        public static PackageDescriptionEnt GetPackageDescription(int PackageId)
        {
            PackageDescriptionEnt packageDescription = new PackageDescriptionEnt
            {
                PackageId = PackageId,
                data = new List<ActivityCustomDescription>()
            };

            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_GetPackageDescription";

                    using (SqlCommand cmd = new SqlCommand(SqlString, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PackageId", PackageId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ActivityCustomDescription description = new ActivityCustomDescription
                                {
                                    DescriptionId = Convert.ToInt32(reader["DescriptionId"]),
                                    DescriptionName = reader["DescriptionName"].ToString(),
                                    Description = reader["Description"].ToString()
                                };

                                packageDescription.data.Add(description);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }

            return packageDescription;
        }

        public static MessageStatus SavePackageDescription(PackageDescriptionEnt data)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_SavePackageDescription";
                    var parameters = new DynamicParameters();
                    parameters.Add("@PackageId", data.PackageId);
                    parameters.Add("@data", ConvertToDataTable(data.data).AsTableValuedParameter("dbo.BBT_ACustomDescription"));

                    var returnedPackageId = cn.QuerySingleOrDefault<int>(SqlString, parameters, commandType: CommandType.StoredProcedure);

                    if (returnedPackageId == data.PackageId)
                    {
                        return new MessageStatus()
                        {
                            Status = returnedPackageId,
                            Message = "Package description saved successfully."
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = 0,
                            Message = "Something went wrong."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "An error occurred: " + ex.Message
                };
            }
        }

        private static DataTable ConvertToDataTable(List<ActivityCustomDescription> data)
        {
            var table = new DataTable();
            table.Columns.Add("DescriptionId", typeof(int));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("DescriptionName", typeof(string));

            foreach (var item in data)
            {
                table.Rows.Add(item.DescriptionId, item.Description, item.DescriptionName);
            }

            return table;
        }


        public static PackageDetailEnt GetPackage(int PackageId)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_GetPackageDetailById";
                    var package = cn.Query<PackageDetailEnt>(SqlString, new
                    {
                        PackageId = PackageId
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    return package;
                }
            }
            catch (Exception ex)
            {
                return new PackageDetailEnt();
            }
        }


        public static (List<PackageListItem> PackageList, int TotalPage) GetPackageList(SPackageList data)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string sqlString = "SPB_PacakgeList";

                    var parameters = new
                    {
                        PageSize = data.PageSize,
                        PageId = data.PageId,
                        SearchText = data.SearchText,
                        CountryId = data.CountryId,
                        CityId = data.CityId,
                        PackageType = data.PackageType
                    };

                    var result = cn.QueryMultiple(sqlString, parameters, commandType: CommandType.StoredProcedure);

                    var packages = result.Read<PackageListItem>().ToList();
                    var totalPage = result.Read<int>().Single();

                    return (packages, totalPage);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle accordingly
                throw new Exception("Error fetching package list", ex);
            }
        }

        public static MessageStatus UpdatePackageStatus(int PackageId, int NewStatus)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "UpdatePackageStatusFlag";
                    var parameters = new DynamicParameters();
                    parameters.Add("@PackageId", PackageId);
                    parameters.Add("@NewStatus", NewStatus);

                    cn.Execute(SqlString, parameters, commandType: CommandType.StoredProcedure);

                    return new MessageStatus()
                    {
                        Status = 1,
                        Message = "Package status updated to " + (NewStatus == 1 ? "Active" : "Inactive")
                    };
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Something went wrong."
                };
            }
        }



        public static MessageStatus SavePackageDetails(PackageDetailEnt data)
        {

            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_CreateUpdatePackage";
                    var status = cn.Query<int>(SqlString, new
                    {
                        @PackageId = data.PackageId,
                        @PackageName = data.PackageName,
                        @ShortDescription = data.ShortDescription,
                        @IsFlightIncluded = data.IsFlightIncluded,
                        @IsHotelIncluded = data.IsHotelIncluded,
                        @IsTransferIncluded = data.IsTransferIncluded,
                        @TypeId = data.TypeId,
                        @CancellationPolicyId = data.CancellationPolicyId,
                        @Tnc = data.Tnc,
                        @IsDynamicPackage = data.IsDynamicPackage,
                        @Nights = data.Nights,
                        @EntryType = data.EntryType,
                        @IsSoldOut = data.IsSoldOut,
                        @Slug = data.Slug,
                        @InclusionPdf = data.InclusionPdf,
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (status == -1)
                    {
                        return new MessageStatus()
                        {
                            Status = 0,
                            Message = "Something went wrong."
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {

                            Status = status,
                            Message = "Package saved successfully."
                        };
                    }

                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Something went wrong."
                };
            }
        }
        public static List<PackageTypeEnt> GetPackageType()
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_GetPackageType";
                    IEnumerable<dynamic> data = cn.Query<dynamic>(SqlString, commandType: System.Data.CommandType.StoredProcedure);

                    List<PackageTypeEnt> packageTypes = new List<PackageTypeEnt>();
                    foreach (var item in data)
                    {
                        PackageTypeEnt packageType = new PackageTypeEnt();
                        packageType.PackageTypeId = item.PackageTypeId;
                        packageType.PackageTypeName = item.PackageTypeName;
                        packageTypes.Add(packageType);
                    }
                    return packageTypes;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                // Returning an empty list in case of an exception
                return new List<PackageTypeEnt>();
            }
        }
        public static MessageStatus SavePackageType(PackageTypeEnt data)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_CreatePackageType";
                    var status = cn.Query<int>(SqlString, new
                    {
                        @PackageTypeId = data.PackageTypeId,
                        @PackageTypeName = data.PackageTypeName,
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault(); // Retrieve the first value from the result set

                    if (status == 1)
                    {
                        return new MessageStatus()
                        {
                            Status = 1,
                            Message = "Package Type saved successfully."
                        };
                    }
                    else if (status == 2)
                    {
                        return new MessageStatus()
                        {
                            Status = 1,
                            Message = "Package Type uodated successfully."
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = 0,
                            Message = "Sorry, PackageType already exists with the name: " + data.PackageTypeName + "!"
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Something went wrong."
                };
            }
        }
        public static List<CancellationPolicyEnt> GetCancellationPolicyList()
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "GetCancellationPolicyList";
                    IEnumerable<dynamic> data = cn.Query<dynamic>(SqlString, commandType: System.Data.CommandType.StoredProcedure);

                    List<CancellationPolicyEnt> PolicysList = new List<CancellationPolicyEnt>();
                    foreach (var item in data)
                    {
                        CancellationPolicyEnt policy = new CancellationPolicyEnt();
                        policy.CancellationPolicyId = item.CancellationPolicyId;
                        policy.CancellationPolicyName = item.CancellationPolicyName;

                        PolicysList.Add(policy);
                    }
                    return PolicysList;
                }
            }
            catch (Exception ex)
            {

                return new List<CancellationPolicyEnt>();
            }
        }


        public static bool IsPdf(IFormFile file)
        {
            return file.ContentType.ToLower() == "application/pdf";
        }
        public static List<PackageCityMapEnt> GetPackageCityMap(int PackageId)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "GetPackageCityMap";
                    var parameters = new { PackageId };
                    IEnumerable<dynamic> data = cn.Query<dynamic>(SqlString, parameters, commandType: System.Data.CommandType.StoredProcedure);

                    List<PackageCityMapEnt> packageCityMapList = new List<PackageCityMapEnt>();
                    foreach (var item in data)
                    {
                        PackageCityMapEnt packageCityMap = new PackageCityMapEnt();
                        packageCityMap.CityName = item.CityName;
                        packageCityMap.Nights = item.Nights;
                        packageCityMap.HotelName = item.HotelName;
                        packageCityMap.CityId = item.CityId;
                        packageCityMap.PackageId = PackageId;
                        packageCityMap.CityMapId = item.CityMapId;

                        packageCityMapList.Add(packageCityMap);
                    }
                    return packageCityMapList;
                }
            }
            catch (Exception ex)
            {
                return new List<PackageCityMapEnt>();
            }
        }
        public static MessageStatus SavePackageCityMap(PackageCityMapEnt data)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SaveUpdatePackageCityMap";
                    var status = cn.Query<int>(SqlString, new
                    {
                        @PackageId = data.PackageId,
                        @CityId = data.CityId,
                        @Nights = data.Nights,
                        @HotelName = data.HotelName,
                        @CityMapId = data.CityMapId,
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (status == data.PackageId)
                    {
                        return new MessageStatus()
                        {
                            Status = data.PackageId,
                            Message = "City and Night Updated."
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = data.PackageId,
                            Message = "Sorry City & Night not Updated!"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Something went wrong."
                };
            }
        }
        public static MessageStatus RemoveCityMap(PackageCityMapEnt data)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "RemoveCityMap";
                    var status = cn.Query<int>(SqlString, new
                    {
                        @PackageId = data.PackageId,
                        @CityMapId = data.CityMapId,
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (status == data.PackageId)
                    {
                        return new MessageStatus()
                        {
                            Status = data.PackageId,
                            Message = "City Removed."
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = data.PackageId,
                            Message = "Something went wrong"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Something went wrong."
                };
            }
        }
        public static List<PackageInclusionEnt> GetPackageInclusion(int PackageId)
        {
            List<PackageInclusionEnt> packageInclusions = new List<PackageInclusionEnt>();
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_GetPackageInclusion";
                    var parameters = new { PackageId };
                    IEnumerable<dynamic> data = cn.Query<dynamic>(SqlString, parameters, commandType: System.Data.CommandType.StoredProcedure);


                    foreach (var item in data)
                    {
                        PackageInclusionEnt packageInc = new PackageInclusionEnt();
                        packageInc.InclusionId = item.InclusionId;
                        packageInc.Day = item.Day;
                        packageInc.Title = item.Title;
                        packageInc.Description = item.Description;
                        packageInc.PackageId = PackageId;
                        packageInclusions.Add(packageInc);
                    }

                }
                return packageInclusions;

            }
            catch (Exception ex)
            {
                return packageInclusions;
            }
        }
        public static MessageStatus SavePackageInclusion(List<PackageInclusionEnt> data)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    // Convert List<PackageInclusionEnt> to DataTable
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("InclusionId", typeof(int));
                    dataTable.Columns.Add("Day", typeof(int)); // Change data type to match SQL table type
                    dataTable.Columns.Add("Title", typeof(string));
                    dataTable.Columns.Add("Description", typeof(string));
                    dataTable.Columns.Add("PackageId", typeof(int));

                    foreach (var item in data)
                    {
                        dataTable.Rows.Add(item.InclusionId, item.Day, item.Title, item.Description, item.PackageId);
                    }

                    // Declare output parameter for PackageId
                    var parameters = new DynamicParameters();
                    parameters.Add("@data", dataTable.AsTableValuedParameter("[dbo].[BBT_PackageInclusion]"));
                    parameters.Add("@PackageId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    // Execute stored procedure
                    cn.Execute("SPB_SavePackageInclusion", parameters, commandType: CommandType.StoredProcedure);

                    // Get the output parameter value for PackageId
                    int packageId = parameters.Get<int>("@PackageId");

                    if (packageId != 0)
                    {
                        return new MessageStatus()
                        {
                            Status = packageId,
                            Message = "Package Inclusion saved successfully."
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = packageId,
                            Message = "Failed to save Package Inclusion."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "An error occurred: " + ex.Message
                };
            }
        }
        public static PackagePriceRootEnt GetPackagePrice(int PackageId, int SlabId)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_GetPackagesOptions";
                    var parameters = new { PackageId = PackageId, SlabId = SlabId };
                    var result = cn.QueryMultiple(SqlString, parameters, commandType: CommandType.StoredProcedure);

                    // Create a new PackagePriceRootEnt object
                    PackagePriceRootEnt packagePrice = new PackagePriceRootEnt();

                    // Package Settings
                    packagePrice.Setting = result.Read<PackageSettingsEnt>().FirstOrDefault();

                    // Package Prices
                    packagePrice.Price = result.Read<PackagePriceEnt>().ToList();

                    // Package Occupancy
                    packagePrice.Occupancy = result.Read<PackageOccupancyEnt>().ToList();

                    // Package Details
                    packagePrice.Detail = result.Read<PackageDaEnt>().FirstOrDefault();

                    packagePrice.SlabId = SlabId;

                    return packagePrice;
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle it appropriately
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static List<Dictionary<string, object>> GetPackageSlabs(int PackageId)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_PackageSlab";
                    var parameters = new { PackageId = PackageId };
                    var result = cn.Query(SqlString, parameters, commandType: CommandType.StoredProcedure);

                    var slabsList = new List<Dictionary<string, object>>();

                    foreach (var row in result)
                    {
                        var slabDict = new Dictionary<string, object>();
                        foreach (var prop in row)
                        {
                            slabDict.Add(prop.Key, prop.Value);
                        }
                        slabsList.Add(slabDict);
                    }

                    return slabsList;
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle it appropriately
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static MessageStatus SaveSlab(PackageSlab data)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SavePackageSlab";
                    var status = cn.Query<int>(SqlString, new
                    {
                        @DateId = data.DateId,
                        @Date = data.Date,
                        @PackageId = data.PackageId
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (status == data.PackageId)
                    {
                        return new MessageStatus()
                        {
                            Status = status,
                            Message = "Save successfully.."
                        };
                    }
                    else if (status == 0)
                    {
                        return new MessageStatus()
                        {
                            Status = status,
                            Message = "Failed to save slab."
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = status,
                            Message = "Something went wrong."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "An error occurred: " + ex.Message
                };
            }
        }
        public static MessageStatus DeleteSlab(int SlabId)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "DeleteSlab_Package";
                    int status = cn.QueryFirstOrDefault<int>(SqlString, new { SlabId }, commandType: CommandType.StoredProcedure);

                    if (status == 1)
                    {
                        return new MessageStatus()
                        {
                            Status = 1,
                            Message = "Date Deleted."
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = 0,
                            Message = "Something went wrong."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Something went wrong: " + ex.Message
                };
            }
        }
        public static MessageStatus SaveDynamicSlab(PackageDynamicSlab data)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SavePackageDynamicSlab";
                    var status = cn.Query<int>(SqlString, new
                    {
                        @SlabId = data.SlabId,
                        @SlabName = data.SlabName,
                        @FromDate = data.FromDate,
                        @ToDate = data.ToDate,
                        @PackageId = data.PackageId


                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (status == data.PackageId)
                    {
                        return new MessageStatus()
                        {
                            Status = status,
                            Message = "Save successfully..."
                        };
                    }
                    else if (status == 0)
                    {
                        return new MessageStatus()
                        {
                            Status = status,
                            Message = "Failed to save slab."
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = status,
                            Message = "Something went wrong."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "An error occurred: " + ex.Message
                };
            }

        }
        public static List<Dictionary<string, object>> GetPackageDynamicSlabs(int PackageId)
        {
            try
            {

                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_PackageDynamicSlab";
                    var parameters = new { PackageId = PackageId };
                    var result = cn.Query(SqlString, parameters, commandType: CommandType.StoredProcedure);

                    var slabsList = new List<Dictionary<string, object>>();

                    foreach (var row in result)
                    {
                        var slabDict = new Dictionary<string, object>();
                        foreach (var prop in row)
                        {
                            slabDict.Add(prop.Key, prop.Value);
                        }
                        slabsList.Add(slabDict);
                    }

                    return slabsList;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static MessageStatus DeleteDynamicSlab(int SlabId)
        {
            try
            {

                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "DeleteDynamicSlab_Package";
                    int status = cn.QueryFirstOrDefault<int>(SqlString, new { SlabId }, commandType: CommandType.StoredProcedure);

                    if (status == 1)
                    {
                        return new MessageStatus()
                        {
                            Status = 1,
                            Message = "Date Deleted."
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = 0,
                            Message = "Something went wrong."
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 1,
                    Message = "Something went wrong."
                };
            }
        }

        public static MessageStatus SavePackagePrice(PackagePriceRootEnt data)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_SavePackagePrice";
                    var status = cn.Query<int>(SqlString, new
                    {
                        @PackageId = data.Setting.PackageId,
                        @Child1From = data.Setting.Child1From,
                        @Child1To = data.Setting.Child1To,
                        @Child2From = data.Setting.Child2From,
                        @Child2To = data.Setting.Child2To,
                        @InfantFrom = data.Setting.InfantFrom,
                        @InfantTo = data.Setting.InfantTo,
                        @Child1Name = data.Setting.Child1Name,
                        @Child2Name = data.Setting.Child2Name,
                        @InfantName = data.Setting.InfantName,
                        @Child1Price = data.Setting.Child1Price,
                        @Child2Price = data.Setting.Child2Price,
                        @InfantPrice = data.Setting.InfantPrice,
                        @SlabId = data.SlabId,
                        @Options = ToDataTable(data.Price),
                        @Occupancy = ToDataTable(data.Occupancy)
                    }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                    if (status == data.Setting.PackageId)
                    {
                        return new MessageStatus()
                        {
                            Status = status,
                            Message = "Save successfully..."
                        };
                    }
                    else if (status == 0)
                    {
                        return new MessageStatus()
                        {
                            Status = status,
                            Message = "Failed to save!"
                        };
                    }
                    else
                    {
                        return new MessageStatus()
                        {
                            Status = status,
                            Message = "Something went wrong."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Something went wrong."
                };
            }
        }

        // Convert List to DataTable
        private static DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (var item in items)
            {
                var values = new object[properties.Length];
                for (var i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public static List<Dictionary<string, object>> GetPackagesImages(int PackageId)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_GetPackagesImages";
                    var parameters = new { PackageId };
                    IEnumerable<dynamic> data = cn.Query<dynamic>(SqlString, parameters, commandType: System.Data.CommandType.StoredProcedure);

                    // Create a list to hold the dictionaries representing image data
                    List<Dictionary<string, object>> imagesList = new List<Dictionary<string, object>>();

                    // Iterate over the result set and convert each row into a dictionary
                    foreach (var row in data)
                    {
                        Dictionary<string, object> imageDict = new Dictionary<string, object>();
                        imageDict["ImageId"] = row.ImageId;
                        imageDict["PackageId"] = row.PackageId;
                        imageDict["Path"] = row.Path;
                        imageDict["StatusFlag"] = row.StatusFlag;
                        imageDict["IsFrontImage"] = row.IsFrontImage;
                        imageDict["AlternateText"] = row.AlternateText;

                        imagesList.Add(imageDict);
                    }

                    // Return the list of image dictionaries
                    return imagesList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int SaveImage(int PackageId, string Path, string alternatetext)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SavePackagesImage";
                    var parameters = new
                    {
                        PackageId,
                        Path,
                        alternatetext
                    };
                    cn.Execute(SqlString, parameters, commandType: System.Data.CommandType.StoredProcedure);

                    return PackageId;
                }
            }
            catch (Exception ex)
            {
                return 0; // Return 0 or handle error as needed
            }
        }

        public static bool UpdateAlternateText(int imageId, string alternateText)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_AlternateText_Package";
                    var parameters = new
                    {
                        ImageId = imageId,
                        AlternateText = alternateText
                    };
                    cn.Execute(SqlString, parameters, commandType: System.Data.CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception)
            {
                return false; // Return false or handle error as needed
            }
        }

        public static MessageStatus ImageAction(int ImageId, string Action)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPB_ImageAction_Package";
                    var parameters = new
                    {
                        ImageId,
                        Action
                    };

                    // Execute the stored procedure
                    cn.Execute(SqlString, parameters, commandType: System.Data.CommandType.StoredProcedure);

                    // Return a success message
                    return new MessageStatus()
                    {
                        Status = 1,
                        Message = "Updated Successfully."
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("Error performing image action: " + ex.Message);

                // Return an error message
                return new MessageStatus()
                {
                    Status = 0,
                    Message = "Something went wrong."
                };
            }
        }


    }


    public class PackageDynamicSlab
    {
        public int SlabId { get; set; }
        public string SlabName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int PackageId { get; set; }
    }
    public class PackageSlab
    {
        public int DateId { get; set; }
        public string Date { get; set; }
        public int PackageId { get; set; }
    }
    public class PackagePriceRootEnt
    {
        public PackageSettingsEnt Setting { get; set; }
        public List<PackagePriceEnt> Price { get; set; }
        public List<PackageOccupancyEnt> Occupancy { get; set; }
        public PackageDaEnt Detail { get; set; }
        public int SlabId { get; set; }
    }
    public class PackageDaEnt
    {
        public int IsDynamicPackage { get; set; }
    }
    public class PackageOccupancyEnt
    {
        public int OccupancyId { get; set; }
        public int Adult { get; set; }
        public int Child1 { get; set; }
        public int Child2 { get; set; }
        public int Infant { get; set; }
        public int MaxPax { get; set; }
    }
    public class PackagePriceEnt
    {
        public int PriceId { get; set; }
        public string OptionName { get; set; }
        public int FromAdult { get; set; }
        public int ToAdult { get; set; }
        public decimal Price { get; set; }
    }
    public class PackageSettingsEnt
    {
        public int Child1From { get; set; }
        public int Child1To { get; set; }
        public int Child2From { get; set; }
        public int Child2To { get; set; }
        public int InfantFrom { get; set; }
        public int InfantTo { get; set; }
        public int PackageId { get; set; }
        public string Child1Name { get; set; }
        public string Child2Name { get; set; }
        public string InfantName { get; set; }
        public decimal Child1Price { get; set; }
        public decimal Child2Price { get; set; }
        public decimal InfantPrice { get; set; }
    }
    public class PackageInclusionEnt
    {
        public int InclusionId { get; set; }
        public int Day { get; set; } // Change data type to string
        public string Title { get; set; }
        public string Description { get; set; }
        public int PackageId { get; set; }
    }

    public class PackageDescriptionEnt
    {
        public int PackageId { get; set; }
        public List<ActivityCustomDescription> data { get; set; }
    }
    public class ActivityCustomDescription
    {
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public string DescriptionName { get; set; }
    }
    public class SPackageList
    {
        public string SearchText { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int PackageType { get; set; }
        public int PageSize { get; set; }
        public int PageId { get; set; }
    }

    public class PackageListItem
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string PackageTypeName { get; set; }
        public string Status { get; set; }
        public string SupplierName { get; set; }
    }

    public class CancellationPolicyEnt
    {
        public int CancellationPolicyId { get; set; }
        public string CancellationPolicyName { get; set; }
        public string CancellationPolicyDescription { get; set; }

        public int StatusFlag { get; set; }

    }
    public class PackageTypeEnt
    {
        public int PackageTypeId { get; set; }

        [Required(ErrorMessage = "PackageType Name is required.")]
        public string PackageTypeName { get; set; }
    }

    public class PackageDetailEnt
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public string ShortDescription { get; set; }
        public int IsFlightIncluded { get; set; }
        public int IsHotelIncluded { get; set; }
        public int IsTransferIncluded { get; set; }
        public int TypeId { get; set; }
        public int CancellationPolicyId { get; set; }
        public string Tnc { get; set; }
        public int IsDynamicPackage { get; set; }
        public int Nights { get; set; }
        public int EntryType { get; set; }
        public int IsSoldOut { get; set; }
        public string Slug { get; set; }
        public string InclusionPdf { get; set; }
    }

    public class PackageCityMapEnt
    {
        public string CityName { get; set; }
        public int Nights { get; set; }
        public string HotelName { get; set; }
        public int CityId { get; set; }
        public int PackageId { get; set; }
        public int CityMapId { get; set; }

    }
    public class CityMapEnt
    {
        public int PackageId { get; set; }
        public int CityMapId { get; set; }
    }



}


