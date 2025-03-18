// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

using System;
using System.Data.SQLite;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Data Source=sistema_servicos.db;Version=3;";
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            CriarTabelas(connection);

            while (true)
            {
                Console.WriteLine("\n=== Sistema de Prestação de Serviços ===");
                Console.WriteLine("1. Registrar Serviço");
                Console.WriteLine("2. Registrar Finança");
                Console.WriteLine("3. Consultar Serviços");
                Console.WriteLine("4. Consultar Finanças");
                Console.WriteLine("5. Sair");
                Console.Write("Escolha uma opção: ");
                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        RegistrarServico(connection);
                        break;
                    case "2":
                        RegistrarFinanca(connection);
                        break;
                    case "3":
                        ConsultarServicos(connection);
                        break;
                    case "4":
                        ConsultarFinancas(connection);
                        break;
                    case "5":
                        Console.WriteLine("Saindo...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
        }
    }

    static void CriarTabelas(SQLiteConnection connection)
    {
        string criarTabelaServicos = @"
            CREATE TABLE IF NOT EXISTS servicos (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                cliente TEXT NOT NULL,
                descricao TEXT NOT NULL,
                valor REAL NOT NULL,
                data TEXT NOT NULL
            )";
        string criarTabelaFinancas = @"
            CREATE TABLE IF NOT EXISTS financas (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                tipo TEXT NOT NULL, -- 'entrada' ou 'saida'
                valor REAL NOT NULL,
                descricao TEXT,
                data TEXT NOT NULL
            )";

        using (var command = new SQLiteCommand(criarTabelaServicos, connection))
        {
            command.ExecuteNonQuery();
        }

        using (var command = new SQLiteCommand(criarTabelaFinancas, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    static void RegistrarServico(SQLiteConnection connection)
    {
        Console.Write("Nome do cliente: ");
        string cliente = Console.ReadLine();
        Console.Write("Descrição do serviço: ");
        string descricao = Console.ReadLine();
        Console.Write("Valor do serviço: ");
        double valor = double.Parse(Console.ReadLine());
        string data = DateTime.Now.ToString("yyyy-MM-dd");

        string inserirServico = "INSERT INTO servicos (cliente, descricao, valor, data) VALUES (@cliente, @descricao, @valor, @data)";
        using (var command = new SQLiteCommand(inserirServico, connection))
        {
            command.Parameters.AddWithValue("@cliente", cliente);
            command.Parameters.AddWithValue("@descricao", descricao);
            command.Parameters.AddWithValue("@valor", valor);
            command.Parameters.AddWithValue("@data", data);
            command.ExecuteNonQuery();
        }

        string inserirFinanca = "INSERT INTO financas (tipo, valor, descricao, data) VALUES ('entrada', @valor, @descricao, @data)";
        using (var command = new SQLiteCommand(inserirFinanca, connection))
        {
            command.Parameters.AddWithValue("@valor", valor);
            command.Parameters.AddWithValue("@descricao", $"Serviço para {cliente}");
            command.Parameters.AddWithValue("@data", data);
            command.ExecuteNonQuery();
        }

        Console.WriteLine("Serviço registrado com sucesso!");
    }

    static void RegistrarFinanca(SQLiteConnection connection)
    {
        Console.Write("Tipo (entrada/saida): ");
        string tipo = Console.ReadLine().ToLower();
        Console.Write("Valor: ");
        double valor = double.Parse(Console.ReadLine());
        Console.Write("Descrição (opcional): ");
        string descricao = Console.ReadLine();
        string data = DateTime.Now.ToString("yyyy-MM-dd");

        string inserirFinanca = "INSERT INTO financas (tipo, valor, descricao, data) VALUES (@tipo, @valor, @descricao, @data)";
        using (var command = new SQLiteCommand(inserirFinanca, connection))
        {
            command.Parameters.AddWithValue("@tipo", tipo);
            command.Parameters.AddWithValue("@valor", valor);
            command.Parameters.AddWithValue("@descricao", descricao);
            command.Parameters.AddWithValue("@data", data);
            command.ExecuteNonQuery();
        }

        Console.WriteLine("Registro financeiro adicionado com sucesso!");
    }

    static void ConsultarServicos(SQLiteConnection connection)
    {
        Console.Write("Data (YYYY-MM-DD) ou deixe em branco para todas: ");
        string data = Console.ReadLine();

        string query = string.IsNullOrEmpty(data)
            ? "SELECT * FROM servicos"
            : "SELECT * FROM servicos WHERE data = @data";

        using (var command = new SQLiteCommand(query, connection))
        {
            if (!string.IsNullOrEmpty(data))
            {
                command.Parameters.AddWithValue("@data", data);
            }

            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("\n=== Serviços Registrados ===");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["id"]}, Cliente: {reader["cliente"]}, Descrição: {reader["descricao"]}, Valor: {reader["valor"]}, Data: {reader["data"]}");
                }
            }
        }
    }

    static void ConsultarFinancas(SQLiteConnection connection)
    {
        Console.Write("Data (YYYY-MM-DD) ou deixe em branco para todas: ");
        string data = Console.ReadLine();

        string query = string.IsNullOrEmpty(data)
            ? "SELECT * FROM financas"
            : "SELECT * FROM financas WHERE data = @data";

        using (var command = new SQLiteCommand(query, connection))
        {
            if (!string.IsNullOrEmpty(data))
            {
                command.Parameters.AddWithValue("@data", data);
            }

            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("\n=== Registros Financeiros ===");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["id"]}, Tipo: {reader["tipo"]}, Valor: {reader["valor"]}, Descrição: {reader["descricao"]}, Data: {reader["data"]}");
                }
            }
        }
