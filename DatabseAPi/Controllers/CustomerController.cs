using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DatabseAPi;
using Npgsql;

namespace DatabseAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Metoda do tworzenia nowego rekordu w tabeli "customer"
        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "INSERT INTO customer (name, email, phone) VALUES (@Name, @Email, @Phone)";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", customer.Name);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@Phone", customer.Phone);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return Ok();
        }

        // Metoda do odczytywania rekordu z tabeli "customer" na podstawie ID
        [HttpGet("{id}")]
        public IActionResult ReadCustomer(int id)
        {
            Customer customer = new Customer();

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM customer WHERE id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    customer.Id = (int)reader["id"];
                    customer.Name = (string)reader["name"];
                    customer.Email = (string)reader["email"];
                    customer.Phone = (string)reader["phone"];
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok(customer);
        }

        // Metoda do aktualizacji rekordu w tabeli "customer"
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, Customer customer)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "UPDATE customer SET name = @Name, email = @Email, phone = @Phone WHERE id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", customer.Name);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@Phone", customer.Phone);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }

            return Ok();
        }

        // Metoda do usuwania rekordu z tabeli "customer" na podstawie ID
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "DELETE FROM customer WHERE id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }

            return Ok();
        }
    }
}
