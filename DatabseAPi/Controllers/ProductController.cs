using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DatabseAPi;
using Npgsql;

namespace DatabaseAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Metoda do tworzenia nowego rekordu w tabeli "product"
        [HttpPost]
        public IActionResult CreateProduct(Product product)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "INSERT INTO product (name, price) VALUES (@Name, @Price)";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Price", product.Price);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return Ok();
        }

        // Metoda do odczytywania rekordu z tabeli "product" na podstawie ID
        [HttpGet("{id}")]
        public IActionResult ReadProduct(int id)
        {
            DatabseAPi.Product product = new Product();

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM product WHERE id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    product.Id = (int)reader["id"];
                    product.Name = (string)reader["name"];
                    product.Price = (decimal)reader["price"];
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok(product);
        }

        // Metoda do aktualizacji rekordu w tabeli "product"
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "UPDATE product SET name = @Name, price = @Price WHERE id = @Id";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Price", product.Price);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return NotFound();
                }
            }

            return Ok();
        }

        // Metoda do usuwania rekordu z tabeli "product" na podstawie ID
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "DELETE FROM product WHERE id = @Id";
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
