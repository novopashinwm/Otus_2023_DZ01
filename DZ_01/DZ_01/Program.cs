

using DZ_01.Entities;

using Npgsql;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DB1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Данная настройка нужна, чтобы postres корректно воспринял даты .net
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
  
            EFInsert();
            EFGetAll();
            EFInsertOneRowInEmploee();
            Console.WriteLine("Нажмите любую клавишу для завершения программы!");
            Console.ReadKey();
        }

        static void EFInsertOneRowInEmploee()
        {
            Console.WriteLine("Ввод одной строки в таблицу сотрудники");
            Console.WriteLine();
            using (DataContext db = new DataContext()) 
            {
                Console.Write("Введите ФИО: ");
                string fullName = Console.ReadLine();
                if (!string.IsNullOrEmpty(fullName)) 
                {
                    if (db.employees.Where(x => x.fullName == fullName).Count() > 0)
                    {
                        Console.WriteLine("Данный пользователь уже есть в базе данных!!!");
                    }
                    else 
                    {
                        Console.Write("Введите должность: ");
                        var position = Console.ReadLine();
                        Console.Write("Введите отдел: ");
                        var department = Console.ReadLine();
                        Console.Write("Введите зарплату: ");
                        decimal salary;
                        decimal.TryParse(Console.ReadLine(), out salary);
                        var employee = new Employee(db.employees.Count() + 1, fullName, position, department, salary);
                        db.employees.Add(employee);
                        db.SaveChanges();
                        Console.WriteLine("Запись успешно добавлена!!!");
                    }
                } 
            }
        }

        #region EFCore
        static void EFInsert()
        {
            using (DataContext db = new DataContext()) 
            {
                List<Employee> emploees = new List<Employee>();
                emploees.Add(new Employee(1, "Иванов Сергей", "Менеджер", "Продажи", 150_000.00m));
                emploees.Add(new Employee(2, "Балясникова Полина", "Разработчик", "IT", 200_000.00m));
                emploees.Add(new Employee(3, "Соседов Владимир", "Главный Бухгалтер", "Бухгалтерия", 300_000.00m));
                emploees.Add(new Employee(4, "Ключников Алексей", "Директор по продажам", "Маркетинг", 400_000.00m));
                emploees.Add(new Employee(5, "Бражникова Татьяна", "HR Специалист", "HR", 110_000.00m));
                db.employees.AddRange(emploees);

                List<Project> projects = new List<Project>();
                projects.Add(new Project(1, "Проект А", new DateTime(2023, 1, 1), new DateTime(2023, 3, 31), emploees[0]));
                projects.Add(new Project(2, "Проект Б", new DateTime(2023, 2, 15), new DateTime(2023, 6, 30), emploees[1]));
                projects.Add(new Project(3, "Проект В", new DateTime(2023, 3, 1), new DateTime(2023, 5, 31), emploees[3]));
                projects.Add(new Project(4, "Проект Г", new DateTime(2023, 4, 10), new DateTime(2023, 8, 31), emploees[2]));
                projects.Add(new Project(5, "Проект Д", new DateTime(2023, 5, 15), new DateTime(2023, 7, 30), emploees[0]));
                db.projects.AddRange(projects);

                List<Customer> customers = new List<Customer>();
                customers.Add(new Customer(1, "ООО 'Ромашка'", "125410,Москва, ул. Садовая-Ямская,4", emploees[0]));
                customers.Add(new Customer(2, "ООО 'Жажда'", "412431,Махачкала, ул. 13 бакинских коммисаров,10", emploees[2]));
                customers.Add(new Customer(3, "ООО 'Рымбыттехника'", "140410,Коломна, ул. Ленина,8", emploees[1]));
                customers.Add(new Customer(4, "ООО 'Золотая рыбка'", "140411,Коломна, ул. Фрунзе,52", emploees[3]));
                customers.Add(new Customer(5, "ЗАО 'МММ'", "121014,Москва, ул. Вавилова,12", emploees[0]));
                db.AddRange(customers);

                db.SaveChanges();
            }

        }

       
        static void EFGetAll()
        {
            var symbol = '=';
            var lenRow = 60;
            using (DataContext db = new DataContext()) 
            {
                Console.WriteLine($"{new string(symbol,lenRow/2)} Сотрудники {new string(symbol,lenRow/2)}");
                var emplyees = db.employees.ToList();
                foreach (var emploee in emplyees)
                {
                    Console.WriteLine(emploee);
                }  
                Console.WriteLine(new string(symbol,lenRow));
                Console.WriteLine();

                Console.WriteLine($"{new string(symbol,lenRow/2)} Проекты {new string(symbol, lenRow/2)}");
                var projects = db.projects.ToList();
                foreach (var project in projects)
                {
                    Console.WriteLine(project);
                }
                Console.WriteLine(new string(symbol, lenRow));
                Console.WriteLine();

                Console.WriteLine($"{new string(symbol, lenRow / 2)}  Клиенты  {new string(symbol, 10)}");
                var customers = db.customers.ToList();
                foreach (var customer in customers)
                {
                    Console.WriteLine(customer);
                }
                Console.WriteLine(new string(symbol, lenRow ));
                Console.WriteLine();

            }

        }


        #endregion
    }
}