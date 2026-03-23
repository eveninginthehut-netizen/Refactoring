using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Интернет_шоп
{
    internal class Program
    {
        public class UserService
        {
            private readonly SqlConnection _connection;
            public UserService(SqlConnection connection)
            {
                _connection = connection;
            }
            //Проблемный метод: Получение пользователей с их заказами 
            public List<object>GetUsersWithOrders()
            {
                var resulte= new List<object>();
                //Уязвимость SQL инъекции
                var usersCmd = new SqlCommand("SELECT * FROM Users", _connection);
                var usersReader=usersCmd.ExecuteReader();
                while(usersReader.Read())
                {
                    var userId = usersReader.GetInt32(0);
                    var userName=usersReader.GetString(1);
                    //Проблемма N+1: запрос в цикле
                    var ordersCmd = new SqlCommand($"SELECT * FROM Orders where UserId={userId}", _connection);
                    var ordersReader=ordersCmd.ExecuteReader();
                    var orders=new List<object>();
                    while(ordersReader.Read())
                    {
                        orders.Add(new { OrderId = ordersReader.GetInt32(0), Total = ordersReader.GetDecimal(2) });
                    }
                    ordersReader.Close();
                    resulte.Add(new
                    {
                        userId = userId,
                        Name = userName,
                        Orders = orders
                    });
                }
                usersReader.Close();
                return resulte;
            }
        }
        static void Main(string[] args)
        {

        }
    }
}
