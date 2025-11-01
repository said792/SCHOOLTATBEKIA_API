using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    // ✅ تسجيل الدخول
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_AuthenticateUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", request.Username);
                cmd.Parameters.AddWithValue("@Password", request.Password);

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var user = new LoginResponse
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            Username = reader["Username"]?.ToString() ?? "",
                            Role = reader["Role"]?.ToString() ?? "",
                            StudentID = Convert.ToInt32(reader["StudentID"]),
                            StudentName = reader["ArbStudName"]?.ToString() ?? "",
                            EmploeID = Convert.ToInt32(reader["EmploeID"]),
                            EmploeName = reader["EmploeArName"]?.ToString() ?? "",
                            SchoolID = Convert.ToInt32(reader["SchoolID"]),
                            MrahelID = Convert.ToInt32(reader["MrahelID"]),
                            YerID = Convert.ToInt32(reader["YerID"])
                        };


                        // ✅ إنشاء JWT Token
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.Role, user.Role),
                            new Claim("UserID", user.UserID.ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: _configuration["Jwt:Issuer"],
                            audience: _configuration["Jwt:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: creds
                        );

                        user.Token = new JwtSecurityTokenHandler().WriteToken(token);

                        return Ok(user);
                    }
                    else
                    {
                        return Unauthorized("اسم المستخدم أو كلمة المرور غير صحيحة.");
                    }
                }
            }
        }
    }

    // ✅ دالة عامة تنفذ الإجراء المخزن scher2int وتعيد النتيجة JSON
    [HttpGet("scher2int")]
    public IActionResult GetScher2Int(int SCHER1, int SCHER2, int INPOT)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("scher2int", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SCHER1", SCHER1);
                cmd.Parameters.AddWithValue("@SCHER2", SCHER2);
                cmd.Parameters.AddWithValue("@INPOT", INPOT);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // ✅ تحويل الجدول إلى List<Dictionary<string, object>>
                var result = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    result.Add(dict);
                }

                return Ok(result); // ✅ JSON صالح وسهل التعامل
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "حدث خطأ أثناء تنفيذ scher2int",
                error = ex.Message
            });
        }
    }

    [HttpGet("scher3int")]
    public IActionResult GetScher3Int(int SCHER1, int SCHER2, int SCHER3, int INPOT)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("scher3int", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SCHER1", SCHER1);
                cmd.Parameters.AddWithValue("@SCHER2", SCHER2);
                cmd.Parameters.AddWithValue("@SCHER3", SCHER3);
                cmd.Parameters.AddWithValue("@INPOT", INPOT);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // ✅ تحويل الجدول إلى List<Dictionary<string, object>>
                var result = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    result.Add(dict);
                }

                return Ok(result); // ✅ JSON صالح وسهل التعامل
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "حدث خطأ أثناء تنفيذ scher2int",
                error = ex.Message
            });
        }
    }
    [HttpGet("scher5int")]
    public IActionResult GetScher3Int(int SCHER1, int SCHER2, int SCHER3, int SCHER4, int SCHER5, int INPOT)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("scher5int", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SCHER1", SCHER1);
                cmd.Parameters.AddWithValue("@SCHER2", SCHER2);
                cmd.Parameters.AddWithValue("@SCHER3", SCHER3);
                cmd.Parameters.AddWithValue("@SCHER4", SCHER4);
                cmd.Parameters.AddWithValue("@SCHER5", SCHER5);
                cmd.Parameters.AddWithValue("@INPOT", INPOT);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // ✅ تحويل الجدول إلى List<Dictionary<string, object>>
                var result = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        dict[col.ColumnName] = row[col];
                    }
                    result.Add(dict);
                }

                return Ok(result); // ✅ JSON صالح وسهل التعامل
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "حدث خطأ أثناء تنفيذ scher2int",
                error = ex.Message
            });
        }
    }
    // ✅ نموذج استقبال بيانات الطالب من الموبايل
    public class StudentAbsenceDto
    {
        public int StudentID { get; set; }
        public DateTime AbsentDate { get; set; }
        public string AbsentType { get; set; } = string.Empty;
    }

    // ✅ استقبال قائمة الطلاب (Batch)
    [HttpPost("SaveAbsencesBatch")]
    public async Task<IActionResult> SaveAbsencesBatch([FromBody] List<StudentAbsenceDto> absences)
    {
        if (absences == null || absences.Count == 0)
            return BadRequest(new { success = false, message = "القائمة فارغة" });

        try
        {
            int inserted = 0, updated = 0;

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await conn.OpenAsync();

                foreach (var item in absences)
                {
                    // 🟢 أولاً: تنفيذ الإجراء الخاص بالحفظ
                    using (SqlCommand cmd = new SqlCommand("INSER_UPDAT_DELETTAB_sch_dat", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@sch1", item.StudentID);
                        cmd.Parameters.AddWithValue("@sch2", item.AbsentDate.Date);
                        cmd.Parameters.AddWithValue("@sch3", item.AbsentType ?? "غائب بدون عذر");
                        cmd.Parameters.AddWithValue("@INPOT", 3); // النسخة الجديدة

                        // 🧩 نجهّز باراميترات الإخراج من الإجراء
                        var studentNameParam = new SqlParameter("@StudentName", SqlDbType.NVarChar, 100)
                        {
                            Direction = ParameterDirection.Output
                        };
                        var schoolNameParam = new SqlParameter("@SchoolName", SqlDbType.NVarChar, 100)
                        {
                            Direction = ParameterDirection.Output
                        };

                        cmd.Parameters.Add(studentNameParam);
                        cmd.Parameters.Add(schoolNameParam);

                        var returnParam = new SqlParameter
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        cmd.Parameters.Add(returnParam);

                        await cmd.ExecuteNonQueryAsync();

                        int result = Convert.ToInt32(returnParam.Value);
                        string studentName = studentNameParam.Value?.ToString() ?? "طالب غير معروف";
                        string schoolName = schoolNameParam.Value?.ToString() ?? "مدرسة غير محددة";

                        if (result == 1) inserted++;
                        else if (result == 2) updated++;

                        // 🟣 ثانياً: إرسال الإشعار باستخدام الإجراء الجديد AddNotification
                        using (SqlCommand notifCmd = new SqlCommand("AddNotification", conn))
                        {
                            notifCmd.CommandType = CommandType.StoredProcedure;

                            notifCmd.Parameters.AddWithValue("@UserID", item.StudentID);
                            notifCmd.Parameters.AddWithValue("@MessageID", 2); // 2 = إشعار غياب
                            notifCmd.Parameters.AddWithValue("@RelatedID", 0);

                            // 🧩 تمرير المتغيرات الديناميكية
                            notifCmd.Parameters.AddWithValue("@StudentName", studentName);
                            notifCmd.Parameters.AddWithValue("@SchoolName", schoolName);
                            notifCmd.Parameters.AddWithValue("@AbsentDate", item.AbsentDate);

                            // النص المخصص (اختياري)
                            notifCmd.Parameters.AddWithValue("@CustomText", DBNull.Value);

                            await notifCmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                await conn.CloseAsync();
            }

            return Ok(new
            {
                success = true,
                inserted,
                updated,
                message = $"تم إدخال {inserted} وتحديث {updated} وإرسال الإشعارات بنجاح ✅"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }


    [HttpPost("ManagePermissions")]
    public async Task<IActionResult> ManagePermissions([FromBody] PermissionDto dto)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // 🟢 تنفيذ إجراء حفظ الإذن
                using (SqlCommand cmd = new SqlCommand("ManagePermissions", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PermissionID", dto.PermissionID);
                    cmd.Parameters.AddWithValue("@EmploeID", dto.EmploeID);
                    cmd.Parameters.AddWithValue("@PermissionType", dto.PermissionType);
                    cmd.Parameters.AddWithValue("@PermissionDate", dto.PermissionDate);

                    // ⏰ معالجة الوقت لقبول "HH:mm" أو "HH:mm:ss"
                    if (!TimeSpan.TryParse(dto.StartTime?.ToString(), out TimeSpan startTime))
                        startTime = TimeSpan.Parse(dto.StartTime?.ToString() + ":00");

                    if (!TimeSpan.TryParse(dto.EndTime?.ToString(), out TimeSpan endTime))
                        endTime = TimeSpan.Parse(dto.EndTime?.ToString() + ":00");

                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);

                    cmd.Parameters.AddWithValue("@PermissionDuration", dto.PermissionDuration);
                    cmd.Parameters.AddWithValue("@PermissionStatus", dto.PermissionStatus);
                    cmd.Parameters.AddWithValue("@YerID", dto.YerID);
                    cmd.Parameters.AddWithValue("@INPOT", dto.INPOT);

                    await cmd.ExecuteNonQueryAsync();
                }

                // 🟣 جلب بيانات المعلم والمدرسة
                string teacherName = "";
                string schoolName = "";
                int schoolId = 0;

                using (SqlCommand infoCmd = new SqlCommand(@"
                SELECT 
                    E.EmploeArName AS TeacherName,
                    S.SchoolNam AS SchoolName,
                    S.SchoolID
                FROM Emplwes E
                LEFT JOIN SCHOOL S ON E.SchoolID = S.SchoolID
                WHERE E.EmploeID = @EmpID", conn))
                {
                    infoCmd.Parameters.AddWithValue("@EmpID", dto.EmploeID);

                    using (var reader = await infoCmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            teacherName = reader["TeacherName"]?.ToString() ?? "موظف غير معروف";
                            schoolName = reader["SchoolName"]?.ToString() ?? "مدرسة غير محددة";
                            schoolId = Convert.ToInt32(reader["SchoolID"]);
                        }
                    }
                }

                // 🧩 جلب كل المدراء في نفس المدرسة
                List<int> managerIds = new List<int>();

                using (SqlCommand mgrCmd = new SqlCommand(@"
                SELECT U.UserID
                FROM Users1 U
                WHERE (U.Role = N'مدير' )
                AND U.SchoolID = @SchoolID", conn))
                {
                    mgrCmd.Parameters.AddWithValue("@SchoolID", schoolId);

                    using (var reader = await mgrCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            managerIds.Add(Convert.ToInt32(reader["UserID"]));
                        }
                    }
                }

                // 📨 إرسال الإشعار لكل مدير في نفس المدرسة
                foreach (var managerId in managerIds)
                {
                    using (SqlCommand notifCmd = new SqlCommand("AddNotification", conn))
                    {
                        notifCmd.CommandType = CommandType.StoredProcedure;

                        notifCmd.Parameters.AddWithValue("@UserID", managerId); // 👤 كل مدير
                        notifCmd.Parameters.AddWithValue("@MessageID", 4); // 💬 إذن خروج
                        notifCmd.Parameters.AddWithValue("@RelatedID", dto.PermissionID);
                        notifCmd.Parameters.AddWithValue("@TeacherName", teacherName);
                        notifCmd.Parameters.AddWithValue("@SchoolName", schoolName);
                        notifCmd.Parameters.AddWithValue("@AbsentDate", dto.PermissionDate);
                        notifCmd.Parameters.AddWithValue("@CustomText",
                            $"إذن خروج لمدة {dto.PermissionDuration} من {dto.StartTime} إلى {dto.EndTime}");

                        await notifCmd.ExecuteNonQueryAsync();
                    }
                }

                await conn.CloseAsync();

                return Ok(new
                {
                    success = true,
                    message = $"تم حفظ إذن الخروج وإرسال الإشعار إلى {managerIds.Count} مدير(ين) في نفس المدرسة ✅"
                });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("RequestVacation")]
    public async Task<IActionResult> RequestVacation([FromBody] VacationDto dto)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // 🟢 تنفيذ الإجراء المخزن لحفظ الإجازة
                using (SqlCommand cmd = new SqlCommand("RequestVacation", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TlabAgazaID", dto.TlabAgazaID);
                    cmd.Parameters.AddWithValue("@EmploeID", dto.EmploeID);
                    cmd.Parameters.Add("@AgazaNo", SqlDbType.NVarChar).Value = dto.AgazaNo ?? (object)DBNull.Value;
                    cmd.Parameters.AddWithValue("@dtpStartDate", dto.DtpStartDate);
                    cmd.Parameters.AddWithValue("@dtpEndDate", dto.DtpEndDate);
                    cmd.Parameters.AddWithValue("@txtDuration", dto.TxtDuration);
                    cmd.Parameters.Add("@AgazaType", SqlDbType.NVarChar).Value =
                        string.IsNullOrWhiteSpace(dto.AgazaType) ? "منتظرة" : dto.AgazaType;

                    cmd.Parameters.AddWithValue("@YerID", dto.YerID);
                    cmd.Parameters.AddWithValue("@INPOT", dto.INPOT);

                    await cmd.ExecuteNonQueryAsync();
                }

                // 🟣 جلب بيانات الموظف والمدرسة
                string teacherName = "";
                string schoolName = "";
                int schoolId = 0;

                using (SqlCommand infoCmd = new SqlCommand(@"
                SELECT 
                    E.EmploeArName AS TeacherName,
                    S.SchoolNam AS SchoolName,
                    S.SchoolID
                FROM Emplwes E
                LEFT JOIN SCHOOL S ON E.SchoolID = S.SchoolID
                WHERE E.EmploeID = @EmpID", conn))
                {
                    infoCmd.Parameters.AddWithValue("@EmpID", dto.EmploeID);

                    using (var reader = await infoCmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            teacherName = reader["TeacherName"]?.ToString() ?? "موظف غير معروف";
                            schoolName = reader["SchoolName"]?.ToString() ?? "مدرسة غير محددة";
                            schoolId = Convert.ToInt32(reader["SchoolID"]);
                        }
                    }
                }

                // 🧩 جلب كل المدراء في نفس المدرسة
                List<int> managerIds = new List<int>();

                using (SqlCommand mgrCmd = new SqlCommand(@"
                SELECT U.UserID
                FROM Users1 U
                WHERE (U.Role = N'مدير' )
                AND U.SchoolID = @SchoolID", conn))
                {
                    mgrCmd.Parameters.AddWithValue("@SchoolID", schoolId);

                    using (var reader = await mgrCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            managerIds.Add(Convert.ToInt32(reader["UserID"]));
                        }
                    }
                }

                // 📨 إرسال الإشعار لكل مدير في نفس المدرسة
                foreach (var managerId in managerIds)
                {
                    using (SqlCommand notifCmd = new SqlCommand("AddNotification", conn))
                    {
                        notifCmd.CommandType = CommandType.StoredProcedure;

                        notifCmd.Parameters.AddWithValue("@UserID", managerId);
                        notifCmd.Parameters.AddWithValue("@MessageID", 5);
                        notifCmd.Parameters.AddWithValue("@RelatedID", dto.TlabAgazaID);

                        notifCmd.Parameters.Add("@TeacherName", SqlDbType.NVarChar).Value = teacherName;
                        notifCmd.Parameters.Add("@SchoolName", SqlDbType.NVarChar).Value = schoolName;
                        notifCmd.Parameters.AddWithValue("@AbsentDate", dto.DtpStartDate);
                        notifCmd.Parameters.Add("@Duration", SqlDbType.NVarChar).Value = dto.TxtDuration + " يوم";

                        notifCmd.Parameters.Add("@CustomText", SqlDbType.NVarChar).Value =
                            $"طلب إجازة ({dto.AgazaNo}) من {dto.DtpStartDate:yyyy-MM-dd} إلى {dto.DtpEndDate:yyyy-MM-dd} لمدة {dto.TxtDuration} يوم";

                        await notifCmd.ExecuteNonQueryAsync();
                    }
                }

                await conn.CloseAsync();

                return Ok(new
                {
                    success = true,
                    message = $"تم تسجيل طلب الإجازة وإرسال الإشعار إلى {managerIds.Count} مدير(ين) في نفس المدرسة ✅"
                });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
    [HttpGet("GetNotifications")]
    public async Task<IActionResult> GetNotifications(int userId, int? notificationId = 0)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("scher2int", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SCHER1", userId);
                cmd.Parameters.AddWithValue("@SCHER2", notificationId.HasValue ? notificationId.Value : 0);
                cmd.Parameters.AddWithValue("@INPOT", 19);

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();

                var notifications = new List<NotificationItem>();

                while (await reader.ReadAsync())
                {
                    var item = new NotificationItem
                    {
                        Id = reader["NotificationID"] is DBNull ? 0 : Convert.ToInt32(reader["NotificationID"]),
                        Title = reader["Title"] is DBNull ? string.Empty : reader["Title"].ToString(),
                        Date = reader["CreatedAt"] is DBNull
                               ? string.Empty
                               : Convert.ToDateTime(reader["CreatedAt"]).ToString("yyyy-MM-dd HH:mm"),
                        IsRead = reader["IsRead"] is DBNull ? false : Convert.ToBoolean(reader["IsRead"]),
                        // نملأ Body بس لو جاي إشعار محدد
                        Body = (notificationId.HasValue && notificationId.Value > 0)
                               ? (reader["MessageContent"] is DBNull ? string.Empty : reader["MessageContent"].ToString())
                               : string.Empty
                    };

                    notifications.Add(item);
                }

                await conn.CloseAsync();
                return Ok(notifications);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // نموذج بيانات الإشعار
    public class NotificationItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public string Date { get; set; } = "";
        public bool IsRead { get; set; }
    }

    [HttpPost("MarkAsRead")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SELCT_MAX_SUMTAB_sch_mrehl_yer", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sch", notificationId);
                cmd.Parameters.AddWithValue("@INPOT", 40); // قيمة INPOT للتحديث

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await conn.CloseAsync();

                return Ok(true);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("GetUnreadCount")]
    public async Task<IActionResult> GetUnreadCount(int sch, int INPOT)
    {
        int count = 0;

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand("SELCT_MAX_SUMTAB_sch_mrehl_yer", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@sch", sch);
            cmd.Parameters.AddWithValue("@INPOT", 39);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            count = Convert.ToInt32(result);
        }

        return Ok(count);
    }


    public class VacationDto
    {
        public int TlabAgazaID { get; set; }
        public int EmploeID { get; set; }
        public string AgazaNo { get; set; } // نوع الإجازة
        public DateTime DtpStartDate { get; set; }
        public DateTime DtpEndDate { get; set; }
        public int TxtDuration { get; set; }
        public string AgazaType { get; set; } // حالة الإجازة
        public int YerID { get; set; }
        public int INPOT { get; set; }
    }  

    [HttpGet("RunStoredProcedure")]
    public async Task<IActionResult> RunStoredProcedure(string procName, int INPOT)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@INPOT", INPOT);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                await conn.CloseAsync();

                return Ok(new { success = true, result });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"حدث خطأ أثناء تنفيذ {procName}",
                error = ex.Message
            });
        }
    }

    public class PermissionDto
    {
        public int PermissionID { get; set; }
        public int EmploeID { get; set; }
        public string PermissionType { get; set; }
        public DateTime PermissionDate { get; set; }

        // 👇 خليه نص عشان نقدر نتحكم في التحويل جوه الكنترولر
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public string PermissionDuration { get; set; }
        public string PermissionStatus { get; set; }
        public int YerID { get; set; }
        public int INPOT { get; set; }
    }


    // ✅ الموديلات
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public int UserID { get; set; }
        public string Username { get; set; } = "";
        public string Role { get; set; } = "";
        public int StudentID { get; set; }
        public string StudentName { get; set; } = "";
        public int EmploeID { get; set; }
        public string EmploeName { get; set; } = "";
        public int SchoolID { get; set; }
        public int MrahelID { get; set; }
        public int YerID { get; set; }
        public string Token { get; set; } = "";
    }
}


