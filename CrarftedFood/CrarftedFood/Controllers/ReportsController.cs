﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CrarftedFood.Models;
using CrarftedFood.Tests;
using Data.DTOs;
using Data.Entities;
using Rotativa.Options;
using Size = Rotativa.Options.Size;

namespace CrarftedFood.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //TODO permisija? delivery i allreports
        [AuthorizeUser(Permission = new int[] { ((int)Data.Enums.Permissions.SeeReports), (int)Data.Enums.Permissions.SeeDeliveryReport})]
        public ActionResult Index(bool delivery = false, bool order = false, bool invoice = false)
        {
            return View();
        }
        
        [AuthorizeUser(Permission = new int[] { ((int)Data.Enums.Permissions.SeeReports)})]
        public ActionResult Invoice(string fileName, int startDay = -1, int startMonth = -1, int startYear = -1, int endDay = -1, int endMonth = -1, int endYear = -1)

        {
            DateTime startTime, endTime;
                
            //startTime = new DateTime(startYear, startMonth, startDay);
            //endTime = new DateTime(endDay, endMonth, endDay);
            //if (startTime > endTime)L
            //{
            //    var t = startTime;
            //    startTime = endTime;
            //    endTime = t;
            //}

            //NOTE: temporary 


            startTime = new DateTime(2000, 1,1);
            endTime = new DateTime(2017, 1,1);

            var order = new ReportViewModel()
            {
                Orders = Reports.GetInvoiceReport(startTime, endTime)
            };
            
            CreatePDF(fileName, order);
            
            return View(order);
        }

        [AuthorizeUser(Permission = new int[] { ((int)Data.Enums.Permissions.SeeReports), (int)Data.Enums.Permissions.SeeDeliveryReport })]
        public ActionResult Delivery(string fileName, int day = -1, int month = -1, int year = -1)
        {
            DateTime Date = new DateTime(year,month,day);

            var delivery = new ReportViewModel()
            {
                Orders = Reports.GetDeliveryReport(Date)
            };

            CreatePDF(fileName, delivery);

            return View(delivery);
        }

        [AuthorizeUser(Permission = new int[] { ((int)Data.Enums.Permissions.SeeReports) })]
        public ActionResult Orders(string fileName, int day = -1, int month = -1, int year = -1)
        {
            DateTime Date = new DateTime(year, month, day);

            var order = new ReportViewModel()
            {//TODO pazi
                Orders = Reports.GetOrderReport(Date)
            };

            CreatePDF(fileName, order);

            return View(order);
        }

        private void CreatePDF(string fileName, object model )
        {
            var PDF = new Rotativa.ViewAsPdf("Invoice", model)
            {
                PageSize = Size.A4
            };
            byte[] bytePDF = PDF.BuildPdf(this.ControllerContext);
            var url = System.Web.HttpContext.Current.Server.MapPath("~") + "/RESOURCES/" +
                      fileName + ".pdf";
            var fileStream = new FileStream(url, FileMode.Create, FileAccess.Write);
            fileStream.Write(bytePDF, 0, bytePDF.Length);
            fileStream.Close();
        }
    }
}