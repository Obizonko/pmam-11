using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        [Route("registration")]
        public IActionResult Registration(WebApplication3.Models.Registration registration)
        {
            registration.IsActive = 1; // Set IsActive to 1

            using (NpgsqlConnection con = new NpgsqlConnection(_configuration.GetConnectionString("Ef_Postgres_Db").ToString()))
            {
                con.Open();

                // Перевірка наявності логіну або електронної пошти у базі даних перед вставкою нового запису
                using (NpgsqlCommand checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM registration WHERE Email = @Email OR UserName = @UserName", con))
                {
                    checkCmd.Parameters.AddWithValue("Email", registration.Email);
                    checkCmd.Parameters.AddWithValue("UserName", registration.UserName);
                    int existingRecordsCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (existingRecordsCount > 0)
                    {
                        // Повернути повідомлення про існуючий логін або електронну пошту відповідно
                        return BadRequest(new { message = "This username or email already exists in the database" });
                    }
                }

                // Виконання вставки запису, якщо такий логін або електронна пошта ще не існує в базі
                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO registration (UserName, PASS, Email, IsActive) VALUES (@UserName, @PASS, @Email, @IsActive)", con))
                {
                    cmd.Parameters.AddWithValue("UserName", registration.UserName);
                    cmd.Parameters.AddWithValue("PASS", registration.PASS);
                    cmd.Parameters.AddWithValue("Email", registration.Email);
                    cmd.Parameters.AddWithValue("IsActive", registration.IsActive);
                    int i = cmd.ExecuteNonQuery();

                    if (i > 0)
                    {
                        // Повернення повідомлення про успішну вставку в відповідь
                        return Ok(new { message = "Data Inserted" });
                    }
                    else
                    {
                        // Повернення повідомлення про помилку в відповідь
                        return BadRequest(new { message = "Error" });
                    }
                }
            }
        }



        [HttpPost]
        [Route("login")]
        public IActionResult login(WebApplication3.Models.Registration registration)
        {
            var connectionString = _configuration.GetConnectionString("Ef_Postgres_Db");

            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM registration WHERE email = @Email AND pass = @Password AND isactive = 1", con))
                {
                    cmd.Parameters.AddWithValue("Email", registration.Email);
                    cmd.Parameters.AddWithValue("Password", registration.PASS);

                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            // Retrieving the username from the query results
                            string username = Convert.ToString(dt.Rows[0]["username"]);

                            // Returning additional data along with the response
                            return Ok(new { message = "Valid User", username = username /* Additional data you want to return */ });

                        }
                        else
                        {
                            return Ok(new { message = "Invalid User" });
                        }

                    }   
                }
            }
        }


        [Route("api/[controller]")]
        [ApiController]
        public class StaticFileController : ControllerBase
        {
            [HttpGet]
            [Route("index")]
            public IActionResult GetIndexHtml()
            {
                // Use the content root path to find the "wwwroot" folder
                var contentRootPath = System.IO.Directory.GetCurrentDirectory();
                var filePath = System.IO.Path.Combine(contentRootPath, "wwwroot");

                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Serve the HTML file
                    var fileStream = System.IO.File.OpenRead(filePath);
                    return File(fileStream, "text/html");
                }
                else
                {
                    return NotFound(); // Return a 404 Not Found response if the file doesn't exist
                }
            }
        }


    }


}
