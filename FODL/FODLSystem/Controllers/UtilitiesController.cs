using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DNTBreadCrumb.Core;
using EQList;
using FODLSystem.Models;
using FODLSystem.Models.View_Model;
using LinqToExcel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NavCU;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
//all codes modified by JRV
namespace FODLSystem.Controllers
{
    public class UtilitiesController : Controller
    {
        private readonly FODLSystemContext _context;
        private static string url = "http://Bacchus.semiraramining.net:7047/bc2019_smpc/WS/Semirara/";
       // private static string url = "http://APRODITE.semiraramining.net:7057/BC130_SMPC_TEST/WS/Semirara/";
        public UtilitiesController(FODLSystemContext context)
        {
            _context = context;
        }
        [BreadCrumb(Title = "Synchronize", Order = 1, IgnoreAjaxRequests = true)]
        public IActionResult Synchronize()
        {
            this.SetCurrentBreadCrumbTitle("Synchronize");
            return View();
        }
        public IActionResult DownloadExcel()
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "syncdata_" + DateTime.Now.ToString("MMddyyyy") + ".xlsx";
            try
            {

                DateTime _syncdata= DateTime.Now;
                var _sync= _context.SynchronizeInformations.FirstOrDefault();
                if (_sync != null)
                {
                    _syncdata = _sync.LastDownloaded.GetValueOrDefault();
                }

                var depts = _context.Departments.ToList(); //done
                var users = _context.Users.ToList(); //done
                var items = _context.Items.ToList();
                var components = _context.Components.ToList();
                var dispensers = _context.Dispensers.ToList();
                var equipments = _context.Equipments.ToList();
                var lubetrucks = _context.LubeTrucks.ToList();
                var drivers = _context.Drivers.ToList();

                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet =
                    workbook.Worksheets.Add("Department");
                    worksheet.Cell(1, 1).Value = "ID";
                    worksheet.Cell(1, 2).Value = "Code";
                    worksheet.Cell(1, 3).Value = "Name";
                    worksheet.Cell(1, 4).Value = "Status";
                    worksheet.Cell(1, 5).Value = "CompanyId";
                    int index = 1;

                    foreach (var item in depts)
                    {
                        worksheet.Cell(index + 1, 1).Value = item.ID;
                        worksheet.Cell(index + 1, 2).Value = "'" + item.Code;
                        worksheet.Cell(index + 1, 3).Value = item.Name;
                        worksheet.Cell(index + 1, 4).Value = item.Status;
                        worksheet.Cell(index + 1, 5).Value = item.CompanyId;
                        index++;
                    }


                    IXLWorksheet worksheet2 =
                     workbook.Worksheets.Add("Users");
                    worksheet2.Cell(1, 1).Value = "Id";
                    worksheet2.Cell(1, 2).Value = "Username";
                    worksheet2.Cell(1, 3).Value = "RoleId";
                    worksheet2.Cell(1, 4).Value = "Password";
                    worksheet2.Cell(1, 5).Value = "FirstName";
                    worksheet2.Cell(1, 6).Value = "LastName";
                    worksheet2.Cell(1, 7).Value = "Name";
                    worksheet2.Cell(1, 8).Value = "Status";
                    worksheet2.Cell(1, 9).Value = "Email";
                    worksheet2.Cell(1, 10).Value = "Domain";
                    worksheet2.Cell(1, 11).Value = "CompanyAccess";
                    worksheet2.Cell(1, 12).Value = "UserType";
                    worksheet2.Cell(1, 13).Value = "DepartmentId";
                    worksheet2.Cell(1, 14).Value = "DispenserAccess";
                    worksheet2.Cell(1, 15).Value = "LubeAccess";
                    worksheet2.Cell(1, 16).Value = "DateModified";
                    index = 1;

                    foreach (var item in users)
                    {
                        worksheet2.Cell(index + 1, 1).Value = item.Id;
                        worksheet2.Cell(index + 1, 2).Value = "'" + item.Username;
                        worksheet2.Cell(index + 1, 3).Value = item.RoleId;
                        worksheet2.Cell(index + 1, 4).Value = "'" + item.Password;
                        worksheet2.Cell(index + 1, 5).Value = "'" + item.FirstName;
                        worksheet2.Cell(index + 1, 6).Value = "'" + item.LastName;
                        worksheet2.Cell(index + 1, 7).Value = "'" + item.Name;
                        worksheet2.Cell(index + 1, 8).Value = "'" + item.Status;
                        worksheet2.Cell(index + 1, 9).Value = "'" + item.Email;
                        worksheet2.Cell(index + 1, 10).Value = "'" + item.Domain;
                        worksheet2.Cell(index + 1, 11).Value = "'" + item.CompanyAccess;
                        worksheet2.Cell(index + 1, 12).Value = "'" + item.UserType;
                        worksheet2.Cell(index + 1, 13).Value = "'" + item.DepartmentCode;
                        worksheet2.Cell(index + 1, 14).Value = "'" + item.DispenserAccess;
                        worksheet2.Cell(index + 1, 15).Value = "'" + item.LubeAccess;
                        worksheet2.Cell(index + 1, 16).Value =  item.DateModified;
                        index++;
                    }

                    IXLWorksheet wsItem =
                     workbook.Worksheets.Add("Items");
                    wsItem.Cell(1, 1).Value = "Id";
                    wsItem.Cell(1, 2).Value = "No";
                    wsItem.Cell(1, 3).Value = "Description";
                    wsItem.Cell(1, 4).Value = "Description2";
                    wsItem.Cell(1, 5).Value = "TypeFuel";
                    wsItem.Cell(1, 6).Value = "DescriptionLiquidation";
                    wsItem.Cell(1, 7).Value = "Status";
                    wsItem.Cell(1, 8).Value = "DateModified";
                   

                    index = 1;

                    foreach (var item in items)
                    {
                        wsItem.Cell(index + 1, 1).Value = item.Id;
                        wsItem.Cell(index + 1, 2).Value = "'" + item.No;
                        wsItem.Cell(index + 1, 3).Value = "'" + item.Description;
                        wsItem.Cell(index + 1, 4).Value = "'" + item.Description2;
                        wsItem.Cell(index + 1, 5).Value = "'" + item.TypeFuel;
                        wsItem.Cell(index + 1, 6).Value = "'" + item.DescriptionLiquidation;
                        wsItem.Cell(index + 1, 7).Value = "'" + item.Status;
                        wsItem.Cell(index + 1, 8).Value =item.DateModified;
                        index++;
                    }

                    IXLWorksheet wsComponents =
                     workbook.Worksheets.Add("Components");
                    wsComponents.Cell(1, 1).Value = "Id";
                    wsComponents.Cell(1, 2).Value = "Code";
                    wsComponents.Cell(1, 3).Value = "Description";
                    wsComponents.Cell(1, 4).Value = "Status";
                    wsComponents.Cell(1, 5).Value = "DateModified";
                   


                    index = 1;

                    foreach (var item in components)
                    {
                        wsComponents.Cell(index + 1, 1).Value = item.Id;
                        wsComponents.Cell(index + 1, 2).Value = "'" + item.Code;
                        wsComponents.Cell(index + 1, 3).Value = item.Description;
                        wsComponents.Cell(index + 1, 4).Value = item.Status;
                        wsComponents.Cell(index + 1, 5).Value = item.DateModified;
                       
                        index++;
                    }

                    IXLWorksheet wsDispensers =
                    workbook.Worksheets.Add("Dispensers");
                    wsDispensers.Cell(1, 1).Value = "Id";
                    wsDispensers.Cell(1, 2).Value = "Name";
                    wsDispensers.Cell(1, 3).Value = "Status";
                    wsDispensers.Cell(1, 5).Value = "No";
                    wsDispensers.Cell(1, 4).Value = "DateModified";

                    index = 1;


                    foreach (var item in dispensers)
                    {
                        wsDispensers.Cell(index + 1, 1).Value = item.Id;
                        wsDispensers.Cell(index + 1, 2).Value = "'" + item.Name;
                        wsDispensers.Cell(index + 1, 3).Value = "'" + item.Status;
                        wsDispensers.Cell(index + 1, 4).Value = "'" + item.No;
                        wsDispensers.Cell(index + 1, 5).Value = item.DateModified;
                        index++;
                    }




                    IXLWorksheet wsEquipments =
                    workbook.Worksheets.Add("Equipments");
                    wsEquipments.Cell(1, 1).Value = "Id";
                    wsEquipments.Cell(1, 2).Value = "No";
                    wsEquipments.Cell(1, 3).Value = "Name";
                    wsEquipments.Cell(1, 4).Value = "ModelNo";
                    wsEquipments.Cell(1, 5).Value = "Status";
                    wsEquipments.Cell(1, 6).Value = "DepartmentCode";
                    wsEquipments.Cell(1, 7).Value = "FuelCodeDiesel";
                    wsEquipments.Cell(1, 8).Value = "FuelCodeOil";
                    wsEquipments.Cell(1, 9).Value = "DateModified";

                    index = 1;

                    foreach (var item in equipments)
                    {
                        wsEquipments.Cell(index + 1, 1).Value = item.Id;
                        wsEquipments.Cell(index + 1, 2).Value = "'" + item.No;
                        wsEquipments.Cell(index + 1, 3).Value = "'" + item.Name;
                        wsEquipments.Cell(index + 1, 4).Value = "'" + item.ModelNo;
                        wsEquipments.Cell(index + 1, 5).Value = "'" + item.Status;
                        wsEquipments.Cell(index + 1, 6).Value = "'" + item.DepartmentCode;
                        wsEquipments.Cell(index + 1, 7).Value = "'" + item.FuelCodeDiesel;
                        wsEquipments.Cell(index + 1, 8).Value = "'" + item.FuelCodeOil;
                        wsEquipments.Cell(index + 1, 9).Value =  item.DateModified;
                        index++;
                    }


                    IXLWorksheet wsLubetrucks =
                    workbook.Worksheets.Add("Lubetrucks");
                    wsLubetrucks.Cell(1, 1).Value = "Id";
                    wsLubetrucks.Cell(1, 2).Value = "No";
                    wsLubetrucks.Cell(1, 3).Value = "OldId";
                    wsLubetrucks.Cell(1, 4).Value = "Description";
                    wsLubetrucks.Cell(1, 5).Value = "Status";
                    wsLubetrucks.Cell(1, 6).Value = "DateModified";
                    index = 1;

                    foreach (var item in lubetrucks)
                    {
                        wsLubetrucks.Cell(index + 1, 1).Value = item.Id;
                        wsLubetrucks.Cell(index + 1, 2).Value = "'" + item.No;
                        wsLubetrucks.Cell(index + 1, 3).Value = "'" + item.OldId;
                        wsLubetrucks.Cell(index + 1, 4).Value = "'" + item.Description;
                        wsLubetrucks.Cell(index + 1, 5).Value = "'" + item.Status;
                        wsLubetrucks.Cell(index + 1, 6).Value = item.DateModified;
                        index++;
                    }


                    IXLWorksheet wsDrivers =
                    workbook.Worksheets.Add("Drivers");
                    wsDrivers.Cell(1, 1).Value = "ID";
                    wsDrivers.Cell(1, 2).Value = "IdNumber";
                    wsDrivers.Cell(1, 3).Value = "Name";
                    wsDrivers.Cell(1, 4).Value = "Position";
                    wsDrivers.Cell(1, 5).Value = "Status";
                    wsDrivers.Cell(1, 6).Value = "DateModified";
                    index = 1;

                    foreach (var item in drivers)
                    {
                        wsDrivers.Cell(index + 1, 1).Value = item.ID;
                        wsDrivers.Cell(index + 1, 2).Value = "'" + item.IdNumber;
                        wsDrivers.Cell(index + 1, 3).Value = "'" + item.Name;
                        wsDrivers.Cell(index + 1, 4).Value = "'" + item.Position;
                        wsDrivers.Cell(index + 1, 5).Value = "'" + item.Status;
                        wsDrivers.Cell(index + 1, 6).Value =  item.DateModified;
                        index++;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        if (_sync != null)
                        {
                            _sync.LastDownloaded = DateTime.Now;
                            _context.Entry(_sync).State = EntityState.Modified;
                            _context.SaveChanges();
                        }

                        return File(content, contentType, fileName);
                    }

                    

                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IActionResult>UploadExcel()
        {
            string filePath = "";
            string message = "";
            string status = "";
            IFormFile file = Request.Form.Files[0];
            string transferExcel;

            try
            {

                string strFilename = Path.GetFileNameWithoutExtension(file.FileName);

                int cntRec = _context.FileUploads.Where(a => a.FileName == strFilename).Count();
                if (cntRec > 0)
                {
                    var err = new
                    {
                        status = "failed",
                        message = "File already uploaded"
                    };

                   
                    return Json(err);
                }


                StringBuilder sb = new StringBuilder();
                if (file.Length > 0)
                {
                    string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                    ISheet sheet;
                    ISheet sheet2;
                    ISheet sheet3;
                    ISheet sheet4;
                    ISheet sheet5;
                    ISheet sheet6;
                    ISheet sheet7;
                    ISheet sheet8;



                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\fileuploads\", file.FileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        stream.Position = 0;
                        if (sFileExtension == ".xls")
                        {
                            HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                           
                            sheet = hssfwb.GetSheet("Department"); //get first sheet from workbook 
                            sheet2 = hssfwb.GetSheet("Users");
                            sheet3 = hssfwb.GetSheet("Items");
                            sheet4 = hssfwb.GetSheet("Components");
                            sheet5 = hssfwb.GetSheet("Dispensers");
                            sheet6 = hssfwb.GetSheet("Equipments");
                            sheet7 = hssfwb.GetSheet("Lubetrucks");
                            sheet8 = hssfwb.GetSheet("Drivers");


                        }
                        else
                        {
                            XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                           
                            sheet = hssfwb.GetSheet("Department"); //get first sheet from workbook 
                            sheet2 = hssfwb.GetSheet("Users");
                            sheet3 = hssfwb.GetSheet("Items");
                            sheet4 = hssfwb.GetSheet("Components");
                            sheet5 = hssfwb.GetSheet("Dispensers");
                            sheet6 = hssfwb.GetSheet("Equipments");
                            sheet7 = hssfwb.GetSheet("Lubetrucks");
                            sheet8 = hssfwb.GetSheet("Drivers");

                        }
                        
                    }
                    transferExcel = await UploadExcelFinal(sheet, sheet2, sheet3, sheet4, sheet5, sheet6, sheet7, sheet8, fullPath);
                    if (transferExcel == "success")
                    {
                        status = "success";
                        message = "Uploaded successfully!";
                    }
                    else
                    {
                        status = "failed";
                        message = transferExcel;
                    }                 

                }
                
            }
            catch (Exception e)
            {
                status = "failed";
                message = e.Message.ToString();
            }

            var model = new
            {
                status,
                message
            };

            filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\fileuploads\" + file.FileName);
            System.IO.File.Delete(filePath);

            return new JsonResult(model);
        }
        public async Task<string> UploadExcelFinal(ISheet sheet, ISheet sheet2, ISheet sheet3, ISheet sheet4, ISheet sheet5, ISheet sheet6, ISheet sheet7, ISheet sheet8, string fileName)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int rowCount = sheet.LastRowNum;
                    //DEPARTMENT
                    //List<Department> svm = new List<Department>();
                    if (rowCount >= 0)
                    {
                        for (int i = 1; i <= rowCount; i++)
                        {
                            IRow headerRow = sheet.GetRow(i); //Get Header Row

                            var dept = _context.Departments.FirstOrDefault(r => r.Code == headerRow.Cells[1].StringCellValue);
                            if (dept == null)
                            {
                                Department sv = new Department
                                {
                                    Code = headerRow.Cells[1].StringCellValue,
                                    Name = headerRow.Cells[2].StringCellValue,
                                    Status = headerRow.Cells[3].StringCellValue,
                                    CompanyId = Convert.ToInt32(headerRow.Cells[4].NumericCellValue)
                                };
                                _context.Departments.Add(sv);
                            }
                            else
                            {
                                dept.Name = headerRow.Cells[2].StringCellValue;
                                dept.Status = headerRow.Cells[3].StringCellValue;
                                dept.CompanyId = Convert.ToInt32(headerRow.Cells[4].NumericCellValue);
                                _context.Entry(dept).State = EntityState.Modified;
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    //USER
                    rowCount = sheet2.LastRowNum;
                    if (rowCount > 0)
                    {
                        for (int i = 1; i <= rowCount; i++)
                        {

                            IRow headerRow = sheet2.GetRow(i); //Get Header Row
                            var _user = _context.Users.FirstOrDefault(r => r.Domain == headerRow.Cells[9].StringCellValue && r.Username == headerRow.Cells[1].StringCellValue);
                            if (_user == null)
                            {
                                User sv = new User
                                {
                                    //Id = Convert.ToInt32(clc[0]),
                                    Username = headerRow.Cells[1].StringCellValue,
                                    RoleId = Convert.ToInt32(headerRow.Cells[2].NumericCellValue),
                                    Password = headerRow.Cells[3].StringCellValue,
                                    FirstName = headerRow.Cells[4].StringCellValue,
                                    LastName = headerRow.Cells[5].StringCellValue,
                                    Name = headerRow.Cells[6].StringCellValue,
                                    Status = headerRow.Cells[7].StringCellValue,
                                    Email = headerRow.Cells[8].StringCellValue,
                                    Domain = headerRow.Cells[9].StringCellValue,
                                    CompanyAccess = headerRow.Cells[10].StringCellValue,
                                    UserType = headerRow.Cells[11].StringCellValue,
                                    DepartmentCode = string.IsNullOrEmpty(headerRow.Cells[12].StringCellValue) ? null : headerRow.Cells[12].StringCellValue,
                                    DispenserAccess = headerRow.Cells[13].StringCellValue,
                                    LubeAccess = headerRow.Cells[14].StringCellValue,
                                    DateModified = DateTime.Now
                                };

                                _context.Users.Add(sv);
                            }
                            else
                            {
                                _user.FirstName = headerRow.Cells[4].StringCellValue;
                                _user.LastName = headerRow.Cells[5].StringCellValue;
                                _user.Name = headerRow.Cells[6].StringCellValue;
                                _user.Status = headerRow.Cells[7].StringCellValue;
                                _user.Email = headerRow.Cells[8].StringCellValue;
                                _user.Domain = headerRow.Cells[9].StringCellValue;
                                _user.CompanyAccess = headerRow.Cells[10].StringCellValue;
                                _user.UserType = headerRow.Cells[11].StringCellValue;
                                _user.DepartmentCode = string.IsNullOrEmpty(headerRow.Cells[12].StringCellValue)?null: headerRow.Cells[12].StringCellValue;
                                _user.DispenserAccess = headerRow.Cells[13].StringCellValue;
                                _user.LubeAccess = headerRow.Cells[14].StringCellValue;
                                _context.Entry(_user).State = EntityState.Modified;
                            }
                        }

                        await _context.SaveChangesAsync();
                    }

                    //ITEMS
                    rowCount = sheet3.LastRowNum;
                    if (rowCount > 0)
                    {
                        for (int i = 1; i <= rowCount; i++)
                        {
                            IRow headerRow = sheet3.GetRow(i); //Get Header Row
                            var _item = _context.Items.FirstOrDefault(r => r.No == headerRow.Cells[1].StringCellValue);
                            if (_item == null)
                            {
                                Item sv = new Item
                                {
                                    //Id = Convert.ToInt32(clc[0]),
                                    No = headerRow.Cells[1].StringCellValue,
                                    Description = headerRow.Cells[2].StringCellValue,
                                    Description2 = headerRow.Cells[3].StringCellValue,
                                    TypeFuel = headerRow.Cells[4].StringCellValue,
                                    DescriptionLiquidation = headerRow.Cells[5].StringCellValue,
                                    Status = headerRow.Cells[6].StringCellValue,
                                    DateModified = DateTime.Now
                                };
                                _context.Items.Add(sv);
                            }
                            else
                            {
                                _item.Description = headerRow.Cells[2].StringCellValue;
                                _item.Description2 = headerRow.Cells[3].StringCellValue;
                                _item.TypeFuel = headerRow.Cells[4].StringCellValue;
                                _item.DescriptionLiquidation = headerRow.Cells[5].StringCellValue;
                                _item.Status = headerRow.Cells[6].StringCellValue;
                                _item.DateModified = DateTime.Now;
                                _context.Entry(_item).State = EntityState.Modified;
                            }
                        }

                        await _context.SaveChangesAsync();
                    }

                    //Components
                    rowCount = sheet4.LastRowNum;
                    if (rowCount > 0)
                    {
                        for (int i = 1; i <= rowCount; i++)
                        {
                            IRow headerRow = sheet4.GetRow(i); //Get Header Row

                            var _comp = _context.Components.FirstOrDefault(r => r.Code == headerRow.Cells[1].StringCellValue);
                            if (_comp == null)
                            {
                                Component sv = new Component
                                {
                                    //Id = Convert.ToInt32(clc[0]),
                                    Code = headerRow.Cells[1].StringCellValue,
                                    Description = headerRow.Cells[2].StringCellValue,
                                    Status = headerRow.Cells[3].StringCellValue,
                                };

                                _context.Components.Add(sv);
                            }
                            else
                            {
                                _comp.Description = headerRow.Cells[2].StringCellValue;
                                _comp.Status = headerRow.Cells[3].StringCellValue;
                                _context.Entry(_comp).State = EntityState.Modified;
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    //Dispensers
                    rowCount = sheet5.LastRowNum;
                    if (rowCount > 0)
                    {
                        for (int i = 1; i <= rowCount; i++)
                        {
                            IRow headerRow = sheet5.GetRow(i); //Get Header Row

                            var _disp = _context.Dispensers.FirstOrDefault(r => r.No == headerRow.Cells[3].StringCellValue);
                            if (_disp == null)
                            {
                                Dispenser sv = new Dispenser
                                {
                                    No = headerRow.Cells[3].StringCellValue,
                                    Name = headerRow.Cells[1].StringCellValue,
                                    Status = headerRow.Cells[2].StringCellValue,
                                    DateModified = DateTime.Now
                                };

                                _context.Dispensers.Add(sv);
                            }
                            else
                            {
                                _disp.Name = headerRow.Cells[1].StringCellValue;
                                _disp.Status = headerRow.Cells[2].StringCellValue;
                                _disp.DateModified = DateTime.Now;
                                _context.Entry(_disp).State = EntityState.Modified;
                            }

                        }
                        await _context.SaveChangesAsync();
                    }

                    //Equipments
                    rowCount = sheet6.LastRowNum;
                    if (rowCount > 0)
                    {
                        for (int i = 1; i <= rowCount; i++)
                        {
                            IRow headerRow = sheet6.GetRow(i); //Get Header Row

                            var _eq = _context.Equipments.FirstOrDefault(r => r.No == headerRow.Cells[1].StringCellValue);
                            if (_eq == null)
                            {
                                Equipment sv = new Equipment
                                {
                                    //Id = Convert.ToInt32(clc[0]),
                                    No = headerRow.Cells[1].StringCellValue,
                                    Name = headerRow.Cells[2].StringCellValue,
                                    ModelNo = headerRow.Cells[3].StringCellValue,
                                    Status = headerRow.Cells[4].StringCellValue,
                                    DepartmentCode = headerRow.Cells[5].StringCellValue,
                                    FuelCodeDiesel = headerRow.Cells[6].StringCellValue,
                                    FuelCodeOil = headerRow.Cells[7].StringCellValue,
                                };

                                _context.Equipments.Add(sv);
                            }
                            else
                            {
                                _eq.Name = headerRow.Cells[2].StringCellValue;
                                _eq.ModelNo = headerRow.Cells[3].StringCellValue;
                                _eq.Status = headerRow.Cells[4].StringCellValue;
                                _eq.DepartmentCode = headerRow.Cells[5].StringCellValue;
                                _eq.FuelCodeDiesel = headerRow.Cells[6].StringCellValue;
                                _eq.FuelCodeOil = headerRow.Cells[7].StringCellValue;
                                _context.Entry(_eq).State = EntityState.Modified;
                            }
                        }
                        await _context.SaveChangesAsync();
                    }


                    //Lubetrucks
                    rowCount = sheet7.LastRowNum;
                    if (rowCount > 0)
                    {
                        for (int i = 1; i <= rowCount; i++)
                        {
                            IRow headerRow = sheet7.GetRow(i); //Get Header Row
                            var _lub = _context.LubeTrucks.FirstOrDefault(r => r.No == headerRow.Cells[1].StringCellValue);
                            if (_lub == null)
                            {
                                LubeTruck sv = new LubeTruck
                                {
                                    No = headerRow.Cells[1].StringCellValue,
                                    OldId = headerRow.Cells[2].StringCellValue,
                                    Description = headerRow.Cells[3].StringCellValue,
                                    Status = headerRow.Cells[4].StringCellValue
                                };

                                _context.LubeTrucks.Add(sv);
                            }
                            else
                            {
                                _lub.Description = headerRow.Cells[3].StringCellValue;
                                _lub.Status = headerRow.Cells[4].StringCellValue;
                                _context.Entry(_lub).State = EntityState.Modified;
                            }

                        }

                        await _context.SaveChangesAsync();
                    }

                    //Drivers
                    rowCount = sheet8.LastRowNum;
                    if (rowCount > 0)
                    {
                        for (int i = 1; i <= rowCount; i++)
                        {
                            IRow headerRow = sheet8.GetRow(i); //Get Header Row
                            var _driver = _context.Drivers.FirstOrDefault(r => r.IdNumber == headerRow.Cells[1].StringCellValue);
                            if (_driver == null)
                            {
                                Driver sv = new Driver
                                {
                                    //Id = Convert.ToInt32(clc[0]),
                                    IdNumber = headerRow.Cells[1].StringCellValue,
                                    Name = headerRow.Cells[2].StringCellValue,
                                    Position = headerRow.Cells[3].StringCellValue,
                                    Status = headerRow.Cells[4].StringCellValue,
                                    DateModified = DateTime.Now,
                                };

                                _context.Drivers.Add(sv);
                            }
                            else
                            {
                                _driver.Name = headerRow.Cells[2].StringCellValue;
                                _driver.Position = headerRow.Cells[3].StringCellValue;
                                _driver.Status = headerRow.Cells[4].StringCellValue;
                                _driver.DateModified = DateTime.Now;
                                _context.Entry(_driver).State = EntityState.Modified;
                            }
                        }

                        await _context.SaveChangesAsync();
                    }

                    DateTime lastDateModified = DateTime.Now;

                    var si = _context.SynchronizeInformations.Find(1);
                    if (si != null)
                    {
                        lastDateModified = si.LastModifiedDate;
                        si.LastModifiedDate = DateTime.Now;
                        si.ModifiedBy = User.Identity.GetFullName();
                        _context.SynchronizeInformations.Update(si);
                    }
                    else
                    {
                        SynchronizeInformation sIn = new SynchronizeInformation
                        {
                            LastModifiedDate = DateTime.Now,
                            ModifiedBy = User.Identity.GetFullName()
                        };
                        _context.Add(sIn);
                        _context.SaveChanges();

                        lastDateModified = si.LastModifiedDate;
                    }


                    //var userStatus = UpdateUsers(fileName, "Users", lastDateModified); //Users


                    //var itemStatus = UpdateItems(fileName, "Items", lastDateModified); //Items


                    //var componentsStatus = UpdateComponents(fileName, "Components", lastDateModified); //Components

                    //var dispensersStatus = UpdateDispensers(fileName, "Dispensers", lastDateModified); //Dispensers

                    //var equipmentsStatus = UpdateEquipments(fileName, "Equipments", lastDateModified); //Equipments
                    //var lubetrucksStatus = UpdateLubeTrucks(fileName, "Lubetrucks", lastDateModified); //Lubetrucks
                    //var driversStatus = UpdateDrivers(fileName, "Drivers", lastDateModified); //Lubetrucks

                    Log log = new Log
                    {
                        Action = "Upload",
                        CreatedDate = DateTime.Now,
                        Descriptions = "Upload Excel File Synchronize. Users , Dispensers , Equipments and Lubetrucks",
                        Status = "success",
                        UserId = User.Identity.GetUserId().ToString()
                    };

                    _context.Add(log);
                   await  _context.SaveChangesAsync();

                    transaction.Commit();
                    return "success";
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return e.Message;
                }
            }
        }
        string UpdateUsers(string fileName, string sheetName, DateTime LastDateModified)
        {
            string.Format("Update users started..");
            try
            {
                FileInfo fs = new FileInfo(fileName);
                ExcelPackage package = new ExcelPackage(fs);
                DataTable dtexcel = new DataTable();
                dtexcel = ExcelToDataTable(package, sheetName);
                DateTime defdDate = new DateTime(1900, 01, 01);

                int dtRows = dtexcel.Rows.Count;
                List<User> items = new List<User>();
                for (int i = 0; i < dtexcel.Rows.Count; i++)
                {
                    var dmodified = string.IsNullOrEmpty(dtexcel.Rows[i]["DateModified"].ToString()) ? defdDate : Convert.ToDateTime(dtexcel.Rows[i]["DateModified"]);
                    string.Format("Row : " + (i+1) + " DateModified " + dmodified);

                    User item = new User();
                    item.Id = Convert.ToInt32(dtexcel.Rows[i]["Id"]);
                    item.Username = dtexcel.Rows[i]["Username"].ToString();
                    item.RoleId = Convert.ToInt32(dtexcel.Rows[i]["RoleId"]);
                    item.Password = dtexcel.Rows[i]["Password"].ToString();
                    item.FirstName = dtexcel.Rows[i]["FirstName"].ToString();
                    item.LastName = dtexcel.Rows[i]["LastName"].ToString();
                    item.Name = dtexcel.Rows[i]["Name"].ToString();
                    item.Status = dtexcel.Rows[i]["Status"].ToString();
                    item.Email = dtexcel.Rows[i]["Email"].ToString();
                    item.Domain = dtexcel.Rows[i]["Domain"].ToString();
                    item.CompanyAccess = dtexcel.Rows[i]["CompanyAccess"].ToString();
                    item.UserType = dtexcel.Rows[i]["UserType"].ToString();
                    item.DepartmentCode = dtexcel.Rows[i]["DepartmentCode"].ToString();
                    item.DispenserAccess = dtexcel.Rows[i]["DispenserAccess"].ToString();
                    item.LubeAccess = dtexcel.Rows[i]["LubeAccess"].ToString();

                    item.DateModified = dmodified;
                   
                    items.Add(item);
                }
              
                var itemsToUpdate = items.Where(a => a.DateModified >= LastDateModified);

                int itemCount = itemsToUpdate.Count();


                string.Format("User to be modified Count " + itemCount);


                foreach (var item in itemsToUpdate)
                {
                    int _id = item.Id;
                    string.Format("User Id : " + _id);

                    //if (item.Id == 10)
                    //{
                    //     _id = item.Id;
                    //}

                    try
                    {
                        //var it = _context.Users.Find(item.Id);
                        var it = _context.Users.Where(a=>a.Username == item.Username).FirstOrDefault();
                        it.Username = item.Username;
                        it.RoleId = item.RoleId;
                        it.Password = item.Password;
                        it.FirstName = item.FirstName;
                        it.LastName = item.LastName;
                        it.Name = item.Name;
                        it.Status = item.Status;
                        it.Email = item.Email;
                        it.Domain = item.Domain;
                        it.CompanyAccess = item.CompanyAccess;
                        it.UserType = item.UserType;
                        it.DepartmentCode = item.DepartmentCode;
                        it.DispenserAccess = item.DispenserAccess;
                        it.LubeAccess = item.LubeAccess;
                        it.DateModified = item.DateModified;
                        //_context.Update(it);
                        _context.Entry(it).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    catch (Exception)
                    {

                        var err = _id;
                        string s;
                    }
                    
                }

                return "Ok";

            }
            catch (Exception e)
            {
                string.Format("User update error " + e.Message);
                return e.Message;
            }
        }
        
        string UpdateItems(string fileName,string sheetName,DateTime LastDateModified) {

            try
            {
                FileInfo fs = new FileInfo(fileName);
                ExcelPackage package = new ExcelPackage(fs);
                DataTable dtexcel = new DataTable();
                dtexcel = ExcelToDataTable(package, sheetName);
                DateTime defdDate = new DateTime(1900, 01, 01);

                int dtRows = dtexcel.Rows.Count;
                List<Item> items = new List<Item>();
                for (int i = 0; i < dtexcel.Rows.Count; i++)
                {
                    Item item = new Item();
                    item.Id = Convert.ToInt32(dtexcel.Rows[i]["Id"]);
                    item.No = dtexcel.Rows[i]["No"].ToString();
                    item.Description = dtexcel.Rows[i]["Description"].ToString();
                    item.Description2 = dtexcel.Rows[i]["Description2"].ToString();
                    item.TypeFuel = dtexcel.Rows[i]["TypeFuel"].ToString();
                    item.DescriptionLiquidation = dtexcel.Rows[i]["DescriptionLiquidation"].ToString();
                    item.Status = dtexcel.Rows[i]["Status"].ToString();
                    item.DateModified = dtexcel.Rows[i]["DateModified"].ToString() == "" ? defdDate : Convert.ToDateTime(dtexcel.Rows[i]["DateModified"]);

                    items.Add(item);
                }
                var itemsToUpdate = items.Where(a=>a.DateModified >= LastDateModified);
                int itemCount = itemsToUpdate.Count();
                foreach (var item in itemsToUpdate)
                {
                    var it = _context.Items.Find(item.Id);
                    it.Description = item.Description;
                    it.Description2 = item.Description2;
                    it.DescriptionLiquidation = item.DescriptionLiquidation;
                    it.TypeFuel = item.TypeFuel;
                    it.No = item.No;
                    it.Status = item.Status;
                    //_context.Update(it);
                    _context.Entry(it).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                return "Ok";

            }
            catch (Exception e)
            {
                
                return e.Message;
            }
        }

        string UpdateComponents(string fileName, string sheetName, DateTime LastDateModified)
        {

            try
            {
                FileInfo fs = new FileInfo(fileName);
                ExcelPackage package = new ExcelPackage(fs);
                DataTable dtexcel = new DataTable();
                dtexcel = ExcelToDataTable(package, sheetName);
                DateTime defdDate = new DateTime(1900, 01, 01);

                int dtRows = dtexcel.Rows.Count;
                List<Component> items = new List<Component>();
                for (int i = 0; i < dtexcel.Rows.Count; i++)
                {
                    Component item = new Component();
                    item.Id = Convert.ToInt32(dtexcel.Rows[i]["Id"]);
                    item.Code = dtexcel.Rows[i]["Name"].ToString();
                    item.DateModified = dtexcel.Rows[i]["DateModified"].ToString() == "" ? defdDate : Convert.ToDateTime(dtexcel.Rows[i]["DateModified"]);
                    item.Status = dtexcel.Rows[i]["Status"].ToString();
                    items.Add(item);
                }
                var itemsToUpdate = items.Where(a => a.DateModified >= LastDateModified);
                int itemCount = itemsToUpdate.Count();
                foreach (var item in itemsToUpdate)
                {
                    var it = _context.Components.Find(item.Id);
                    it.Code = item.Code;
                    it.Status = item.Status;
                    //_context.Update(it);
                    _context.Entry(it).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                return "Ok";

            }
            catch (Exception e)
            {

                return e.Message;
            }
        }

        string UpdateDispensers(string fileName, string sheetName, DateTime LastDateModified)
        {   

            try
            {
               

                FileInfo fs = new FileInfo(fileName);
                ExcelPackage package = new ExcelPackage(fs);
                DataTable dtexcel = new DataTable();
                dtexcel = ExcelToDataTable(package, sheetName);
                DateTime defdDate = new DateTime(1900, 01, 01);

                int dtRows = dtexcel.Rows.Count;
                

                List<Dispenser> items = new List<Dispenser>();
                for (int i = 0; i < dtexcel.Rows.Count; i++)
                {
                   
                    var dm =  dtexcel.Rows[i]["DateModified"].ToString() == "" ? defdDate.ToString() : Convert.ToDateTime(dtexcel.Rows[i]["DateModified"].ToString()).ToString();
                    string.Format("Date Formatted : " + dm).WriteLog();
                    Dispenser item = new Dispenser();
                    item.Id = Convert.ToInt32(dtexcel.Rows[i]["Id"]);
                    item.No = dtexcel.Rows[i]["No"].ToString();
                    item.Name = dtexcel.Rows[i]["Name"].ToString();
                    item.DateModified = Convert.ToDateTime(dm);
                    item.Status = dtexcel.Rows[i]["Status"].ToString();
                    items.Add(item);
                }

              
                var itemsToUpdate = items.Where(a => a.DateModified >= LastDateModified);
                int itemCount = itemsToUpdate.Count();
             

                foreach (var item in itemsToUpdate)
                {
                    var it = _context.Dispensers.Find(item.Id);
                    it.No = item.No;
                    it.Name = item.Name;
                    it.Status = item.Status;
                    it.DateModified = DateTime.Now;
                    _context.Entry(it).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                return "Ok";

            }
            catch (Exception e)
            {
                string.Format("Error Dispenser : " + e.Message).WriteLog();
                return e.Message;
            }
        }
        string UpdateEquipments(string fileName, string sheetName, DateTime LastDateModified)
        {

            try
            {
                FileInfo fs = new FileInfo(fileName);
                ExcelPackage package = new ExcelPackage(fs);
                DataTable dtexcel = new DataTable();
                dtexcel = ExcelToDataTable(package, sheetName);
                DateTime defdDate = new DateTime(1900, 01, 01);

                int dtRows = dtexcel.Rows.Count;
                List<Equipment> items = new List<Equipment>();
                for (int i = 0; i < dtexcel.Rows.Count; i++)
                {
                    Equipment item = new Equipment();
                    item.Id = Convert.ToInt32(dtexcel.Rows[i]["Id"]);
                    item.No = dtexcel.Rows[i]["No"].ToString();
                    item.Name = dtexcel.Rows[i]["Name"].ToString();
                    item.ModelNo = dtexcel.Rows[i]["ModelNo"].ToString();
                    item.DepartmentCode = dtexcel.Rows[i]["DepartmentCode"].ToString();
                    item.FuelCodeDiesel = dtexcel.Rows[i]["FuelCodeDiesel"].ToString();
                    item.FuelCodeOil = dtexcel.Rows[i]["FuelCodeOil"].ToString();
                    item.Status = dtexcel.Rows[i]["Status"].ToString();
                    item.DateModified = dtexcel.Rows[i]["DateModified"].ToString() == "" ? defdDate : Convert.ToDateTime(dtexcel.Rows[i]["DateModified"]);

                    items.Add(item);
                }
                var itemsToUpdate = items.Where(a => a.DateModified >= LastDateModified);
                int itemCount = itemsToUpdate.Count();
                foreach (var item in itemsToUpdate)
                {
                    var it = _context.Equipments.Find(item.Id);
                    it.Name = item.Name;
                    it.ModelNo = item.ModelNo;
                    it.DepartmentCode = item.DepartmentCode;
                    it.FuelCodeDiesel = item.FuelCodeDiesel;
                    it.FuelCodeOil = item.FuelCodeOil;
                    it.No = item.No;
                    it.Status = item.Status;
                    //_context.Update(it);
                    _context.Entry(it).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                return "Ok";

            }
            catch (Exception e)
            {

                return e.Message;
            }
        }

        string UpdateLubeTrucks(string fileName, string sheetName, DateTime LastDateModified)
        {

            try
            {
                FileInfo fs = new FileInfo(fileName);
                ExcelPackage package = new ExcelPackage(fs);
                DataTable dtexcel = new DataTable();
                dtexcel = ExcelToDataTable(package, sheetName);
                DateTime defdDate = new DateTime(1900, 01, 01);

                int dtRows = dtexcel.Rows.Count;
                List<LubeTruck> items = new List<LubeTruck>();
                for (int i = 0; i < dtexcel.Rows.Count; i++)
                {
                    LubeTruck item = new LubeTruck();
                    item.Id = Convert.ToInt32(dtexcel.Rows[i]["Id"]);
                    item.No = dtexcel.Rows[i]["No"].ToString();
                    item.OldId = dtexcel.Rows[i]["OldId"].ToString();
                    item.Description = dtexcel.Rows[i]["Description"].ToString();
                    item.DateModified = dtexcel.Rows[i]["DateModified"].ToString() == "" ? defdDate : Convert.ToDateTime(dtexcel.Rows[i]["DateModified"]);
                    item.Status = dtexcel.Rows[i]["Status"].ToString();
                    items.Add(item);
                }
                var itemsToUpdate = items.Where(a => a.DateModified >= LastDateModified);
                int itemCount = itemsToUpdate.Count();
                foreach (var item in itemsToUpdate)
                {
                    var it = _context.LubeTrucks.Find(item.Id);
                    it.No = item.No;
                    it.OldId = item.OldId;
                    it.Description = item.Description;
                    it.Status = item.Status;
                    //_context.Update(it);
                    _context.Entry(it).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                return "Ok";

            }
            catch (Exception e)
            {

                return e.Message;
            }
        }
        string UpdateDrivers(string fileName, string sheetName, DateTime LastDateModified)
        {

            try
            {
                FileInfo fs = new FileInfo(fileName);
                ExcelPackage package = new ExcelPackage(fs);
                DataTable dtexcel = new DataTable();
                dtexcel = ExcelToDataTable(package, sheetName);
                DateTime defdDate = new DateTime(1900, 01, 01);

                int dtRows = dtexcel.Rows.Count;
                List<Driver> items = new List<Driver>();
                for (int i = 0; i < dtexcel.Rows.Count; i++)
                {
                    Driver item = new Driver();
                    item.ID = Convert.ToInt32(dtexcel.Rows[i]["ID"]);
                    item.IdNumber = dtexcel.Rows[i]["IdNumber"].ToString();
                    item.Name = dtexcel.Rows[i]["Name"].ToString();
                    item.Position = dtexcel.Rows[i]["Position"].ToString();
                    item.DateModified = dtexcel.Rows[i]["DateModified"].ToString() == "" ? defdDate : Convert.ToDateTime(dtexcel.Rows[i]["DateModified"]);
                    item.Status = dtexcel.Rows[i]["Status"].ToString();
                    items.Add(item);
                }
                var itemsToUpdate = items.Where(a => a.DateModified >= LastDateModified);
                int itemCount = itemsToUpdate.Count();
                foreach (var item in itemsToUpdate)
                {
                    var it = _context.Drivers.Find(item.ID);
                    it.IdNumber = item.IdNumber;
                    it.Name = item.Name;
                    it.Position = item.Position;
                    it.Status = item.Status;
                    it.DateModified = item.DateModified;
                    //_context.Update(it);
                    _context.Entry(it).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                return "Ok";

            }
            catch (Exception e)
            {

                return e.Message;
            }
        }
        static DataTable ExcelToDataTable(ExcelPackage package,string sheetName)
        {
            ExcelWorksheet workSheet = package.Workbook.Worksheets[sheetName];
            DataTable table = new DataTable();
            foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
            {
                table.Columns.Add(firstRowCell.Text);
            }

            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                var newRow = table.NewRow();
                foreach (var cell in row)
                {
                    newRow[cell.Start.Column - 1] = cell.Text;
                }
                table.Rows.Add(newRow);
            }
            return table;
        }

        static DataTable ConvertToDatatable(ISheet sheet)
        {
            DataTable dtTable = new DataTable();
            List<string> rowList = new List<string>();
            //ISheet sheet;
            //using (var stream = new FileStream("TestData.xlsx", FileMode.Open))
            //{
            //    stream.Position = 0;
            //    XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
            //   shtItem = xssWorkbook.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                for (int j = 0; j < cellCount; j++)
                {
                    ICell cell = headerRow.GetCell(j);
                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                    {
                        dtTable.Columns.Add(cell.ToString());
                    }
                }
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            if (!string.IsNullOrEmpty(row.GetCell(j).ToString()) && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                            {
                                rowList.Add(row.GetCell(j).ToString());
                            }
                        }
                    }
                    if (rowList.Count > 0)
                        dtTable.Rows.Add(rowList.ToArray());
                    rowList.Clear();
                }
            //}
            return dtTable;
        }

        public async Task<JsonResult> uploadNavision(string batchno,string refid)
        {
            string status = "";
            string message = "";
            int saveCount = 0;
            #region OLD CODE  //JRV IBAHIN KO CODE MASALIMUOT 
            //try
            //{
            //   
            //    //string apiUrl = @"http://192.168.0.199/FODLApi/api/"; //SMPC DEV
            //     string apiUrl = @"http://localhost/fodlapi/api/"; //SMPC Live
            //    //string apiUrl = @"http://sodium2/FODLApi/api/"; //SMPC DEV
            //   // string apiUrl = @"http://localhost:59455/api/"; //LOCAL

            //    NavisionViewModel nvm = null;
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(apiUrl);
            //        var responseTask = client.GetAsync("uploadnav?batchno=" + batchno + "&referenceno=" + refid);
            //        responseTask.Wait();

            //        var response = responseTask.Result;
            //        if (response.IsSuccessStatusCode)
            //        {


            //            var readTask = response.Content.ReadAsAsync<NavisionViewModel>();



            //            try
            //            {
            //                readTask.Wait();

            //                nvm = readTask.Result;
            //                //if (nvm.message != "success")
            //                //{
            //                //    status = "failed";
            //                //    message = nvm.message;
            //                //}
            //                //else
            //                //{
            //                //    status = "success";
            //                //    message = "Uploaded to Navision Successfully";
            //                //}
            //                if (nvm.status == "success")
            //                {
            //                    status = "success";
            //                    message = "Uploaded to Navision Successfully";

            //                }
            //                else if (nvm.status == "partial_success")
            //                {
            //                    status = "success";
            //                    message = nvm.message;

            //                }
            //                else
            //                {
            //                    status = "failed";
            //                    message = nvm.message;
            //                }


            //            }
            //            catch (Exception e)
            //            {

            //                status = "failed";
            //                message = e.Message.ToString();

            //            }




            //        }
            //        else
            //        {
            //            status = "failed";
            //            message = response.ReasonPhrase;
            //        }
            //    }


            //    Log log = new Log();
            //    log.Action = "Save";
            //    log.CreatedDate = DateTime.Now;
            //    log.Descriptions = "Send data to API";
            //    log.Status = status + " " + message;
            //    _context.Add(log);
            //    _context.SaveChanges();


            //}
            //catch (Exception e)
            //{

            //    status = "failed";
            //    message = e.Message.ToString();
            //}
            #endregion

            #region //jrv new code for nav integration
            string NAVUserName = @"Semiraramining\HANDSHAKE";
            string NAVPassword = "M1ntch0c0l@t3";
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly)
            {
                MaxReceivedMessageSize = 10485760
            };

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
            var endpoint = new EndpointAddress(new Uri(url + "Codeunit/FODL_Web_Service"));

            FODL_Web_Service_PortClient _PortClient = new FODL_Web_Service_PortClient(binding, endpoint);
            _PortClient.ClientCredentials.UserName.UserName = NAVUserName;
            _PortClient.ClientCredentials.UserName.Password = NAVPassword;
            int FuelId = 0;
            try
            {
                var fuelid = refid.Split(',').ToList();
                if (fuelid!=null && fuelid.Count() > 0)
                {
                    NavCU.GetDocumentNo_Result getDocumentNo_ = new GetDocumentNo_Result();
                    getDocumentNo_ = await _PortClient.GetDocumentNoAsync(batchno);


                    foreach (var f in fuelid)
                    {
                        saveCount = 0;
                        FuelId = Convert.ToInt32(f);
                        var _fuelOils = _context.FuelOils.FirstOrDefault(r => r.Id == FuelId);
                        if (_fuelOils != null)
                        {

                            var details = _context.FuelOilSubDetails
                          .Where(a => a.FuelOilDetails.FuelOilId == FuelId)
                          .Where(a => a.Status == "Active")
                          .Where(a => a.FuelOilDetails.Status == "Active")
                          .Select(a => new
                          {
                              EntryType = "Negative Adjmt.",
                              ItemNo = a.Items.No,
                              PostingDate = a.FuelOilDetails.FuelOils.TransactionDate,
                              DocumentDate = a.FuelOilDetails.FuelOils.CreatedDate,
                              Qty = a.VolumeQty,
                              EquipmentCode = a.FuelOilDetails.Equipments.No,
                              a.FuelOilDetails.Locations.OfficeCode,
                              FuelCode = a.Items.TypeFuel == "OIL-LUBE" ? a.FuelOilDetails.Equipments.FuelCodeOil : a.FuelOilDetails.Equipments.FuelCodeDiesel,
                              LocationCode = "SMPC-SITE",
                              DepartmentCode = a.Items.TypeFuel == "OIL-LUBE" ? "345" : a.FuelOilDetails.Equipments.DepartmentCode,
                              a.Id,
                              a.Status,
                              a.FuelOilDetailId,
                              FuelOilSubDetailsId = a.Id,
                              DocumentNo = a.FuelOilDetails.FuelOils.ReferenceNo,
                              a.FuelOilDetails.JobCardNo
                          }).ToList();

                            int i = 0;
                            bool allTransferred = true;
                            if (details != null)
                            {

                                foreach (var p in details)
                                {
                                    i += 1;
                                    NavCU.UploadToNavision_Result sendtonav = new UploadToNavision_Result();
                                    sendtonav = await _PortClient.UploadToNavisionAsync(batchno, (i * 10000), p.ItemNo, Convert.ToDateTime(p.PostingDate), Convert.ToDateTime(p.DocumentDate), Convert.ToInt32(p.Qty), p.EquipmentCode,
                                       p.OfficeCode, p.FuelCode, p.LocationCode, p.DepartmentCode, getDocumentNo_.return_value, p.JobCardNo ?? "");

                                    if (sendtonav.return_value == "Success")
                                    {
                                        var det = _context.FuelOilSubDetails.FirstOrDefault(r => r.Id == p.Id);
                                        if (det != null)
                                        {
                                            det.Status = "Transferred";
                                            saveCount++;
                                            _context.Entry(det).State = EntityState.Modified;
                                            _context.SaveChanges();

                                            _fuelOils.Status = "Partially Transferred";
                                            _fuelOils.BatchName = _fuelOils.BatchName == null ? batchno : _fuelOils.BatchName.Contains(batchno) ? _fuelOils.BatchName : (_fuelOils.BatchName + ", " + batchno).Trim();
                                            _context.Entry(_fuelOils).State = EntityState.Modified;
                                            _context.SaveChanges();

                                        }
                                    }
                                    else
                                    {
                                        allTransferred = false;
                                    }
                                }
                            }

                            if (allTransferred==true)
                            {
                                _fuelOils.Status = "Transferred";
                                _fuelOils.BatchName = _fuelOils.BatchName == null ? batchno : _fuelOils.BatchName.Contains(batchno) ? _fuelOils.BatchName : (_fuelOils.BatchName + ", " + batchno).Trim();
                                _context.Entry(_fuelOils).State = EntityState.Modified;
                                _context.SaveChanges();

                                message = "Record has been successfully Transferred to NAVISION";
                                status = "success";
                            }
                            else
                            {
                                if (saveCount > 0)
                                {
                                    _fuelOils.Status = "Partially Transferred";
                                    _fuelOils.BatchName = _fuelOils.BatchName == null ? batchno : _fuelOils.BatchName.Contains(batchno) ? _fuelOils.BatchName : (_fuelOils.BatchName + ", " + batchno).Trim();
                                    _context.Entry(_fuelOils).State = EntityState.Modified;
                                    _context.SaveChanges();
                                    status = "warning";
                                    message = "Record has been successfully Partially Transferred to NAVISION";
                                }
                                else
                                {
                                    status = "failed";
                                    message = "Uanble to transfer Record to NAVISION";
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception x)
            {
                //_context.ChangeTracker.Entries()
                //      .Where(e => e.Entity != null).ToList()
                //      .ForEach(e => e.State = EntityState.Detached);
                var _fuelOils = _context.FuelOils.FirstOrDefault(r => r.Id == FuelId);
                if (_fuelOils != null && saveCount > 0)
                {
                    _fuelOils.Status = "Partially Transferred";
                    _fuelOils.BatchName = _fuelOils.BatchName == null ? batchno : _fuelOils.BatchName.Contains(batchno) ? _fuelOils.BatchName : (_fuelOils.BatchName + ", " + batchno).Trim();
                    _context.Entry(_fuelOils).State = EntityState.Modified;
                    _context.SaveChanges();
                    status = "warning";

                }
                var model2 = new
                {
                    status,
                    message  = x.Message
                };

                return Json(model2);
            }


            NavCU.NewBatchName_Result NewBatch = new NewBatchName_Result();
            NewBatch = await _PortClient.NewBatchNameAsync(batchno);


            var model = new
            {
                status,
                message
            };

            #endregion
            return Json(model);
        }

        #region JRV NO SERIES
        public PartialViewResult NoSeries()
        {

            ViewData["PageStatus"] = "NOT OK";
            var noseries = _context.NoSeries.ToList();
            return PartialView(noseries);
        }
        [HttpPost]
        public async Task<PartialViewResult> NoSeries(List<NoSeries> _noSeries)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_noSeries != null)
                    {
  
                        foreach (var NSeries in _noSeries)
                        {

                            if (NSeries.Id == 0)
                            {
                                _context.Add(NSeries);
                            }
                            else
                            {
                                _context.Entry(NSeries).State = EntityState.Modified;
                            }
                        }
                        await _context.SaveChangesAsync();
                        var _noseries = _context.NoSeries.ToList();
                        ViewData["PageStatus"] = "OK";
                        return PartialView(_noseries);
                    }

                }
                catch (DbUpdateException ex)
                {
                    string innerMessage = ex.InnerException.Message ?? ex.Message;
                    ModelState.AddModelError("", innerMessage);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        string innerMessage = ex.InnerException.Message ?? ex.Message;
                        ModelState.AddModelError("", innerMessage);
                    }
                    else
                    {
                        ModelState.AddModelError("", ex.Message);
                    }
                }
            }
            //else
            //{
            //    var message = string.Join(" | ", ModelState.Values
            //    .SelectMany(v => v.Errors)
            //    .Select(e => e.ErrorMessage));
            //    ModelState.AddModelError("", message);
            //}

            ViewData["PageStatus"] = "NOT OK";
            return PartialView(_noSeries);
        }
        #endregion



    }
}