using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using StudentEnrollmentAPI.Data;
using StudentEnrollmentAPI.Models;

namespace StudentEnrollmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly DbConnection _db;

        public EnrollmentsController(DbConnection db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetEnrollments()
        {
            try
            {
                var enrollments = new List<Enrollment>();
                using var conn = _db.GetConnection();
                conn.Open();
                using var cmd = new MySqlCommand("SELECT enrollment_id, student_id, course_id FROM enrollments ORDER BY enrollment_id", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    enrollments.Add(new Enrollment
                    {
                        EnrollmentId = reader.GetInt32("enrollment_id"),
                        StudentId = reader.GetInt32("student_id"),
                        CourseId = reader.GetInt32("course_id")
                    });
                }
                return Ok(enrollments);
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { error = "Database connection failed. Please ensure MySQL server is running.", details = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddEnrollment([FromBody] Enrollment enrollment)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                using var cmd = new MySqlCommand("INSERT INTO enrollments (student_id, course_id) VALUES (@studentId, @courseId)", conn);
                cmd.Parameters.AddWithValue("@studentId", enrollment.StudentId);
                cmd.Parameters.AddWithValue("@courseId", enrollment.CourseId);
                cmd.ExecuteNonQuery();
                return Ok(new { message = "Student enrolled" });
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { error = "Failed to enroll student.", details = ex.Message });
            }
        }
    }
}
