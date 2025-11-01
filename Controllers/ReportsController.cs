using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using QuestPDF.Fluent;

namespace SCHOOLTATBEKIA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ReportsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("Report")]
        [Produces("application/pdf")]
        public IActionResult GetDynamicReport(int yearId, int schoolId, int employeeId, int reportType,
                                       string headerTitle = "ملخص عام",
                                       string detailsTitle = "تفاصيل التقرير")
        {
            try
            {
                // 🟢 1. استدعاء نفس الإجراء المخزن
                var (summary, details) = GetReportData("scher3int", yearId, schoolId, employeeId, reportType);

                // 🏫 2. بيانات المدرسة أو المؤسسة
                var company = GetSchoolInfo(schoolId);

                // 📋 3. تجهيز البيانات
                var summaryData = summary.Count > 0 ? summary[0] : new Dictionary<string, object>();

                // 🧾 4. توليد التقرير PDF
                var report = new ReportTemplate(company, summaryData, details, headerTitle, detailsTitle);
                var pdf = report.GeneratePdf();

                // 💾 5. عرض مباشر داخل المتصفح أو التطبيق
                Response.Headers.Add("Content-Disposition", $"inline; filename=Report_{reportType}.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        // ======================================================
        // 🟢 دالة مشتركة لاستدعاء أي إجراء مخزن يعيد جدولين
        private (List<Dictionary<string, object>> header, List<Dictionary<string, object>> details)
            GetReportData(string procName, int scher1, int scher2, int scher3, int inpot)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@SCHER1", scher1);
            cmd.Parameters.AddWithValue("@SCHER2", scher2);
            cmd.Parameters.AddWithValue("@SCHER3", scher3);
            cmd.Parameters.AddWithValue("@INPOT", inpot);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            var header = new List<Dictionary<string, object>>();
            var details = new List<Dictionary<string, object>>();

            while (reader.Read())
                header.Add(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));

            if (reader.NextResult())
                while (reader.Read())
                    details.Add(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));

            return (header, details);
        }
        // ======================================================
        // 🟣 تقرير الغياب للطلاب
        [HttpGet("AbsenceReport")]
        [Produces("application/pdf")]
        public IActionResult GetAbsenceReport(int yearId, int mrahelId, int studentId, int reportType,
                                              string headerTitle = "ملخص الغياب",
                                              string detailsTitle = "تفاصيل الغياب")
        {
            try
            {
                // reportType:
                // 8 = الكل, 81 = يومي, 82 = أسبوعي, 83 = شهري, 84 = سنوي
                var (summary, details) = GetReportData("scher3int", yearId, mrahelId, studentId, reportType);
                var company = GetSchoolInfo(mrahelId); // نفس الدالة بتجيب بيانات المدرسة أو المرحلة
                var summaryData = summary.Count > 0 ? summary[0] : new Dictionary<string, object>();

                var report = new ReportTemplate(company, summaryData, details, headerTitle, detailsTitle);
                var pdf = report.GeneratePdf();

                Response.Headers.Add("Content-Disposition", "inline; filename=AbsenceReport.pdf");
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ======================================================
        // 🏫 دالة تجيب بيانات المدرسة
        private Dictionary<string, object> GetSchoolInfo(int schoolId)
        {
            var result = new Dictionary<string, object>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELCT_MAX_SUMTAB_sch_mrehl_yer", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@sch", schoolId);
            cmd.Parameters.AddWithValue("@INPOT", 36);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                result["المكتب"] = reader["المدرسة"]?.ToString();
                result["العنوان"] = $"{reader["المديرية"]} - {reader["الادارة"]}";
                if (reader["اللوجو"] != DBNull.Value && ((byte[])reader["اللوجو"]).Length > 0)
                    result["اللوجو"] = $"data:image/png;base64,{Convert.ToBase64String((byte[])reader["اللوجو"])}";
                else
                    result["اللوجو"] = "";
            }
            else
            {
                result["المكتب"] = "مدرسة غير معروفة";
                result["العنوان"] = "لم يتم العثور على بيانات";
                result["اللوجو"] = "";
            }

            return result;
        }
    }
}
