﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Data.DTOs;



namespace Data.Entities
{
    public static class Reports
    {
        public static List<OrderDto> GetOrderReport(DateTime date)
        {
            using (DataClassesDataContext dc = new DataClassesDataContext())
            {
                return dc.Requests.Where(a => a.DateRequested.Date == date.Date).Select(a => new OrderDto
                {
                    OrderId = a.RequestId,
                    Quantity = a.Quantity,
                    Price = a.Meal.Price * a.Quantity,
                    MealTitle = a.Meal.Title,
                    Note = a.Note
                }).ToList();
            }
        }

        public static List<OrderDto> GetDeliveryReport(DateTime date)
        {
            using (DataClassesDataContext dc = new DataClassesDataContext())
            {
                return dc.Requests.Where(a => a.DateRequested.Date == date.Date).Select(a => new OrderDto
                {
                    OrderId = a.RequestId,
                    EmployeeId = a.EmployeeId,
                    EmployeeName = a.Employee.Name,
                    Quantity = a.Quantity,
                    Price = a.Meal.Price * a.Quantity,
                    MealTitle = a.Meal.Title,
                    Note = a.Note
                }).ToList();
            }
        }

        public static List<OrderDto> GetInvoiceReport(DateTime start, DateTime end)
        {
            using (DataClassesDataContext dc = new DataClassesDataContext())
            {
                List<OrderDto> order = new List<OrderDto>();

                List<MenuMealItem> meals = Data.Entities.Meals.GetMenu();

                foreach (MenuMealItem meal in meals)
                {
                    float totalQuantity =
                        dc.Requests.Where(
                                a => a.MealId == meal.MealId && a.DateDelivered >= start && a.DateDelivered <= end)
                            .Select(a => a.Quantity)
                            .ToList()
                            .Sum();

                    if (totalQuantity != 0)
                    {
                        order.Add(new OrderDto()
                        {
                            MealTitle = meal.Title,
                            Quantity = totalQuantity,
                            Price = meal.Price,
                            TotalPrice = totalQuantity*meal.Price
                        });
                    }
                }

                if (order.Any())
                {
                    double totalBill = order.Sum(x => x.TotalPrice);
                    order.Add(new OrderDto()
                    {
                        MealTitle = "SALDO",
                        TotalPrice = totalBill
                    });
                }

                return order;
            }

        }

        public static List<OrderDto> GetOrdersOfEmployee(int empId, DateTime? start = null, DateTime? end = null)
        {
            using (DataClassesDataContext dc = new DataClassesDataContext())
            {
                if (start == null)
                {
                    start = new DateTime(1753, 1, 2);
                }
                if (end == null)
                {
                    end = new DateTime(9999, 12, 31);
                }
                return dc.Requests.Where(a => a.EmployeeId == empId && start.Value.Date <= a.DateRequested.Date && a.DateRequested.Date <= end.Value.Date)
                    .Select(a => new OrderDto
                    {
                        OrderId = a.RequestId,
                        Quantity = a.Quantity,
                        Price = a.Meal.Price * a.Quantity,
                        MealTitle = a.Meal.Title,
                        Note = a.Note
                    }).ToList();
            }
        }

    }
}
