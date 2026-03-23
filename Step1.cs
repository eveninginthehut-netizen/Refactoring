using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internetShop_до_рефакторинга_
{
    internal class Step1
    {
        public class UserWithOrdersDto
        {
            //исключительно пример
            public int UserId { get; set; }
            public string Name { get; set; }
            public List<OrderDto> Orders { get; set; }
            //тут реализация класса
        }
        public class OrderDto
        {
            //исключительно пример
            public int orderId { get; set; }
            public decimal Total { get; set; }

        }
        string _connectionString;


        public List<UserWithOrdersDto> GetUserWithOrders()

        {
            var result = new List<UserWithOrdersDto>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //защищаем от sql инекций 
                const string userSql = "SELECT ID,Name  FROM Users";
                using (var UserCmd = new SqlCommand(userSql, connection))
                using (var userReader = UserCmd.ExecuteReader())
                {
                    while (userReader.Read())
                    {
                        var userId = userReader.GetInt32(userReader.GetOrdinal("id"));
                        var userName = userReader.GetString(userReader.GetOrdinal("name"));
                        const string orderSQl = "SELECT Id, Total from Orders where UserId=@UserID";
                        using (var orderCMD = new SqlCommand(orderSQl, connection))
                        {
                            orderCMD.Parameters.AddWithValue("userid", userId);
                            using (var orderReader = orderCMD.ExecuteReader())
                            {
                                var orders = new List<OrderDto>();
                                while (orderReader.Read())
                                {
                                    orders.Add(new OrderDto
                                    {
                                        orderId = orderReader.GetInt32(orderReader.GetOrdinal("id")),
                                        Total = orderReader.GetDecimal(orderReader.GetOrdinal("Total")),
                                    });
                                }
                                result.Add(new UserWithOrdersDto
                                {
                                    UserId = userId,
                                    Name = userName,
                                    Orders = orders
                                });
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}