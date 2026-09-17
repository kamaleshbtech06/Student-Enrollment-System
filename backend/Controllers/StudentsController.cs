using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using StudentEnrollmentAPI.Data;
using StudentEnrollmentAPI.Models;

namespace StudentEnrollmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly DbConnection _db;

        public StudentsController(DbConnection db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            try
            {
                var students = new List<Student>();
                using var conn = _db.GetConnection();
                conn.Open();
                using var cmd = new MySqlCommand("SELECT id, name, department FROM students ORDER BY id", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new Student
                    {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Department = reader.GetString("department")
                    });
                }
                return Ok(students);
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { error = "Database connection failed. Please ensure MySQL server is running.", details = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddStudent([FromBody] Student student)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                using var cmd = new MySqlCommand("INSERT INTO students (id, name, department) VALUES (@id, @name, @department)", conn);
                cmd.Parameters.AddWithValue("@id", student.Id);
                cmd.Parameters.AddWithValue("@name", student.Name);
                cmd.Parameters.AddWithValue("@department", student.Department);
                cmd.ExecuteNonQuery();
                return Ok(new { message = "Student saved" });
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { error = "Failed to save student to database.", details = ex.Message });
            }
        }
    }
}
