

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
            GetVersion();

            //CreateClientsTable();
            //CreateDepositsTable();
            //InsertClientsSimple();
            //InsertClientsWithParams();
            //InsertClientsMultipleCommands();
            //Transaction();

            //NHibernateInsertClient();
            //NHibernateInsertDeposit();
            //NHibernateInsertClientThenDeposit();

            //NHibernateInsertDepositWithClient();
            //NHibernateNPlus1();

            //DapperUpsert();

            //JoinDapper();
            //JoinNHibernate();

            //GroupByNHibernate();

            //EFInsert();
            //EFUpdate();
            //EFGetAll();
            //EFGetOne();
            //EFDelete();

            Console.ReadKey();
        }

        const string connectionString = "Host=localhost;Username=habrpguser;Password=pgpwd4habr;Database=habrdb";

        #region ADONET
        /// <summary>
        /// Подключение к БД и получение версии
        /// </summary>
        static void GetVersion()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var sql = "SELECT version()";

                using var cmd = new NpgsqlCommand(sql, connection);

                var version = cmd.ExecuteScalar().ToString();

                Console.WriteLine($"PostgreSQL version: {version}");
            }
        }


        /// <summary>
        /// Создание таблицы
        /// </summary>
        static void CreateClientsTable()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
CREATE SEQUENCE clients_id_seq;

CREATE TABLE clients
(
    id              BIGINT                      NOT NULL    DEFAULT NEXTVAL('clients_id_seq'),
    first_name      CHARACTER VARYING(255)      NOT NULL,
    last_name       CHARACTER VARYING(255)      NOT NULL,
    middle_name     CHARACTER VARYING(255),
    email           CHARACTER VARYING(255)      NOT NULL,
  
    CONSTRAINT clients_pkey PRIMARY KEY (id),
    CONSTRAINT clients_email_unique UNIQUE (email)
);

CREATE INDEX clients_last_name_idx ON clients(last_name);
CREATE UNIQUE INDEX clients_email_unq_idx ON clients(lower(email));
";

            using var cmd = new NpgsqlCommand(sql, connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Created CLIENTS table. Affected rows count: {affectedRowsCount}");
        }

        /// <summary>
        /// Создание таблицы с внешним ключом
        /// </summary>
        static void CreateDepositsTable()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
CREATE SEQUENCE deposits_id_seq;

CREATE TABLE deposits
(
    id              BIGINT                      NOT NULL    DEFAULT NEXTVAL('deposits_id_seq'),
    client_id       BIGINT                      NOT NULL,
    created_at      TIMESTAMP WITH TIME ZONE    NOT NULL,    
  
    CONSTRAINT deposits_pkey PRIMARY KEY (id),
    CONSTRAINT deposits_fk_client_id FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE
);
";

            using var cmd = new NpgsqlCommand(sql, connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Created DEPOSITS table. Affected rows count: {affectedRowsCount}");
        }

        /// <summary>
        /// Вставка без параметров
        /// </summary>
        static void InsertClientsSimple()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var firstName = "Иван";
            var lastName = "Иванов";
            var sql = $@"
INSERT INTO clients(first_name, last_name, middle_name, email) 
VALUES ('{firstName}', '{lastName}', 'Иванович', 'ivan@mail.ru');
";

            using var cmd = new NpgsqlCommand(sql, connection);

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Insert into CLIENTS table. Affected rows count: {affectedRowsCount}");
        }

        /// <summary>
        /// Вставка с параметрами
        /// </summary>
        static void InsertClientsWithParams()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
INSERT INTO clients(first_name, last_name, middle_name, email) 
VALUES (:first_name, :last_name, :middle_name, :email);
";

            using var cmd = new NpgsqlCommand(sql, connection);
            var parameters = cmd.Parameters;
            parameters.Add(new NpgsqlParameter("first_name", "Константин"));
            parameters.Add(new NpgsqlParameter("last_name", "Константинов"));
            parameters.Add(new NpgsqlParameter("middle_name", "Константинович"));
            parameters.Add(new NpgsqlParameter("email", "konst@rambler.ru"));

            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();

            Console.WriteLine($"Insert into CLIENTS table. Affected rows count: {affectedRowsCount}");
        }

        /// <summary>
        /// Несколько команд в одном соединение + выборка из таблицы
        /// </summary>
        static void InsertClientsMultipleCommands()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
INSERT INTO clients(first_name, last_name, middle_name, email) 
VALUES (:first_name, :last_name, :middle_name, :email);
";

            using var cmd1 = new NpgsqlCommand(sql, connection);
            var parameters = cmd1.Parameters;
            parameters.Add(new NpgsqlParameter("first_name", "Иван"));
            parameters.Add(new NpgsqlParameter("last_name", "Петров"));
            parameters.Add(new NpgsqlParameter("middle_name", "Петрович"));
            parameters.Add(new NpgsqlParameter("email", "petr@yandex.ru"));

            var affectedRowsCount = cmd1.ExecuteNonQuery().ToString();

            Console.WriteLine($"Insert into CLIENTS table. Affected rows count: {affectedRowsCount}");

            sql = @"
SELECT first_name, last_name, middle_name, email FROM clients
WHERE first_name<>:first_name
";

            using var cmd2 = new NpgsqlCommand(sql, connection);
            parameters = cmd2.Parameters;
            parameters.Add(new NpgsqlParameter("first_name", "Иван"));

            var reader = cmd2.ExecuteReader();
            while (reader.Read())
            {
                var firstName = reader.GetString(0);
                var lastName = reader.GetString(1);
                var middleName = reader.GetString(2);
                var email = reader.GetString(3);

                Console.WriteLine($"Read: [firstName={firstName},lastName={lastName},middleName={middleName},email={email}]");
            }
        }

        /// <summary>
        /// Транзакция + возврат идентификатора вставленной записи
        /// </summary>
        static void Transaction()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                var sql = @"
INSERT INTO clients(first_name, last_name, middle_name, email) 
VALUES (:first_name, :last_name, :middle_name, :email)
RETURNING id;
";

                using var cmd1 = new NpgsqlCommand(sql, connection);
                var parameters = cmd1.Parameters;
                parameters.Add(new NpgsqlParameter("first_name", "Александр"));
                parameters.Add(new NpgsqlParameter("last_name", "Александров"));
                parameters.Add(new NpgsqlParameter("middle_name", "Александрович"));
                parameters.Add(new NpgsqlParameter("email", "alex@yandex.ru"));

                var clientId = (long)cmd1.ExecuteScalar();
                Console.WriteLine($"Insert into CLIENTS table. ClientId = {clientId}");

                // Специально кидаем исключение
                //throw new ApplicationException("Deliberate exception");

                sql = @"
INSERT INTO deposits(client_id, created_at) 
VALUES (:client_id, :created_at);
";

                using var cmd2 = new NpgsqlCommand(sql, connection);
                parameters = cmd2.Parameters;
                parameters.Add(new NpgsqlParameter("client_id", clientId));
                parameters.Add(new NpgsqlParameter("created_at", DateTime.Now));

                var affectedRowsCount = cmd2.ExecuteNonQuery().ToString();

                Console.WriteLine($"Insert into DEPOSITS table. Affected rows count: {affectedRowsCount}");

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Console.WriteLine($"Rolled back the transaction");
                return;
            }
        }
        #endregion



        #region EFCore
        static void EFInsert()
        {
            //using (DataContext db = new DataContext())
            //{
            //    var client = new Client
            //    {
            //        id = 90,
            //        first_name = "Test",
            //        last_name = "Test",
            //        email = "Test",
            //    };
            //    db.clients.Add(client);

            //    db.SaveChanges();
            //    Console.WriteLine("Record Added");
            //}
        }

        static void EFUpdate()
        {
            //using (DataContext db = new DataContext())
            //{
            //    var client = db.clients.FirstOrDefault(x => x.id == 1);
            //    if (client != null)
            //    {
            //        client.first_name = "Вася";
            //        db.Update(client);
            //        db.SaveChanges();
            //    }
            //    Console.WriteLine("Record Updated");
            //}
        }

        static void EFGetAll()
        {
            //using (DataContext db = new DataContext())
            //{
            //    var objects = db.clients.ToArray();
            //    Console.WriteLine("count:" + objects.Count());
            //}
        }

        static void EFGetOne()
        {
            //using (DataContext db = new DataContext())
            //{
            //    var objects = db.clients.FirstOrDefault(x => x.id == 1);
            //    Console.WriteLine(objects.first_name);
            //}
        }

        static void EFDelete()
        {
            //using (DataContext db = new DataContext())
            //{
            //    var obj = db.clients.FirstOrDefault(x => x.id == 1);
            //    if (obj != null)
            //    {
            //        db.clients.Remove(obj);
            //        db.SaveChanges();
            //        Console.WriteLine("объект удалён");
            //    }
            //    else
            //        Console.WriteLine("объект пустой");
            //}
        }

        #endregion
    }
}