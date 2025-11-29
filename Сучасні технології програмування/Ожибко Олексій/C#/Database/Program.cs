using Faker;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string connectionString = @"Server=localhost;Port=5432;User Id=postgres;Password=**;Database=**";

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

                // Заповнюємо таблицю Roles
                 PopulateRolesTable(connection);

                 // Заповнюємо таблицю ClassGroup
                 PopulateClassGroupTable(connection);

                 // Заповнюємо таблицю Users
                 PopulateUsersTable(connection);

                 // Заповнюємо таблицю Subjects
                 PopulateSubjectsTable(connection); 

                 // Заповнюємо таблицю TeacherSubjects
                 PopulateTeacherSubjectsTable(connection);

                 // Заповнюємо таблицю Schedule
                 PopulateScheduleTable(connection); 

                 // Заповнюємо таблицю StudentPerformance
                 PopulateStudentPerformanceTable(connection); 
 



            // Виведення даних з таблиці Roles.
            Console.WriteLine("Roles Table:");
            DisplayTable(connection, "SELECT * FROM Roles");

            // Виведення даних з таблиці ClassGroup.
            Console.WriteLine("\nClassGroup Table:");
            DisplayTable(connection, "SELECT * FROM ClassGroups");

            // Виведення даних з таблиці Users.
            Console.WriteLine("\nUsers Table:");
            DisplayTable(connection, "SELECT * FROM Users");

             // Виведення даних з таблиці Subjects.
             Console.WriteLine("\nSubjects Table:");
             DisplayTable(connection, "SELECT * FROM Subjects");

            // Виведення даних з таблиці TeacherSubjects.
            Console.WriteLine("\nTeacherSubjects Table:");
            DisplayTable(connection, "SELECT * FROM TeacherSubjects");

            // Виведення даних з таблиці Schedule.
            Console.WriteLine("\nSchedule Table:");
            DisplayTable(connection, "SELECT * FROM Schedule");

            // Виведення даних з таблиці StudentPerformance.
            Console.WriteLine("\nStudentPerformance Table:");
           DisplayTable(connection, "SELECT * FROM StudentPerformance");

            // Виведення даних з таблиці Materials.
            Console.WriteLine("\nMaterials Table:");
            DisplayTable(connection, "SELECT * FROM Materials");

        }
    }

    static void PopulateUsersTable(NpgsqlConnection connection)
    {
        Random random = new Random();
        SHA256 sha256 = SHA256.Create();
        for (int i = 1; i <= 40; i++) // Створюємо 40 школярів
        {
            string username = "student" + i;
            string password = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes("password" + i))).Replace("-", "").ToLower();
            string email = "student" + i + "@example.com";
            int roleId = 1; // Ідентифікатор ролі "школяр"
            string firstName = Name.First();
            string lastName = Name.Last();
            int classGroupId = (i % 22) + 1;


            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Users (username, password, email, role_id, first_name, last_name, class_group_id) " +
             "VALUES (@username, @password, @email, @role_id, @first_name, @last_name, @class_group_id)", connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@role_id", roleId);
                command.Parameters.AddWithValue("@first_name", firstName);
                command.Parameters.AddWithValue("@last_name", lastName);
                command.Parameters.AddWithValue("@class_group_id", classGroupId);
                command.ExecuteNonQuery();
            }
        }
        for (int i = 1; i <= 10; i++) // Створюємо 10 вчителів
        {
            string username = "teacher" + i;
            string password = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes("password" + i))).Replace("-", "").ToLower();
            string email = "teacher" + i + "@example.com";
            int roleId = 2; // Ідентифікатор ролі "вчитель"
            string firstName = Name.First();
            string lastName = Name.Last();


            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Users (username, password, email, role_id, first_name, last_name) " +
            "VALUES (@username, @password, @email, @role_id, @first_name, @last_name)", connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@role_id", roleId);
                command.Parameters.AddWithValue("@first_name", firstName);
                command.Parameters.AddWithValue("@last_name", lastName);

                command.ExecuteNonQuery();
            }
        }

        for (int i = 1; i <= 5; i++) // Створюємо 5 адміністраторів
        {
            string username = "admin" + i;
            string password = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes("password" + i))).Replace("-", "").ToLower();
            string email = "admin" + i + "@example.com";
            int roleId = 3; // Ідентифікатор ролі "адміністратор"
            string firstName = Name.First();
            string lastName = Name.Last();


            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Users (username, password, email, role_id, first_name, last_name) " +
             "VALUES (@username, @password, @email, @role_id, @first_name, @last_name)", connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@role_id", roleId);
                command.Parameters.AddWithValue("@first_name", firstName);
                command.Parameters.AddWithValue("@last_name", lastName);

                command.ExecuteNonQuery();
            }
        }
    }

    static void PopulateRolesTable(NpgsqlConnection connection)
    {
        // Масив ролей і їх ID
        string[] roleNames = { "Учень", "Вчитель", "Адмiнiстратор" };
        int[] roleIds = { 1, 2, 3 };

        for (int i = 0; i < roleNames.Length; i++)
        {
            string roleName = roleNames[i];
            int roleId = roleIds[i];

            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Roles (role_id, role_name) VALUES (@role_id, @role_name)", connection))
            {
                command.Parameters.AddWithValue("@role_id", roleId);
                command.Parameters.AddWithValue("@role_name", roleName);

                command.ExecuteNonQuery();
            }
        }
    }

    static void PopulateClassGroupTable(NpgsqlConnection connection)
    {
        string[] classGroupNames = { "1-А", "1-Б", "2-А", "2-Б", "3-А", "3-Б", "4-А", "4-Б", "5-А", "5-Б", "6-А", "6-Б", "7-А", "7-Б", "8-А", "8-Б", "9-А", "9-Б", "10-А", "10-Б", "11-А", "11-Б" };

        foreach (var className in classGroupNames)
        {
            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO ClassGroups (class_group_name) " +
                "VALUES (@class_group_name)", connection))
            {
                command.Parameters.AddWithValue("@class_group_name", className);

                command.ExecuteNonQuery();
            }
        }
    }

    static void PopulateSubjectsTable(NpgsqlConnection connection)
    {
        string[] subjects = { "Математика", "Iсторiя", "Фiзкультура", "Хiмiя", "Англiйська мова", "Фiзика", "Бiологiя", "Укр. мова", "Iнформатика", "Географiя"};

        foreach (var subjectName in subjects)
        {
            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Subjects (subject_name) VALUES (@subject_name)", connection))
            {
                command.Parameters.AddWithValue("@subject_name", subjectName);
                command.ExecuteNonQuery();
            }
        }
    }

    static void PopulateTeacherSubjectsTable(NpgsqlConnection connection)
    {
        // Fetch all teacher user_ids
        List<int> teacherIds = new List<int>();

        using (NpgsqlCommand command = new NpgsqlCommand("SELECT user_id FROM Users WHERE role_id = 2", connection))
        {
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int teacherId = reader.GetInt32(0);
                    teacherIds.Add(teacherId);
                }
            }
        }

        // Fetch all subject_ids
        List<int> subjectIds = new List<int>();

        using (NpgsqlCommand command = new NpgsqlCommand("SELECT subject_id FROM Subjects", connection))
        {
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int subjectId = reader.GetInt32(0);
                    subjectIds.Add(subjectId);
                }
            }
        }

        int subjectIndex = 0;
        foreach (var teacherId in teacherIds)
        {
            if (subjectIndex >= subjectIds.Count)
                subjectIndex = 0;

            using (NpgsqlCommand insertCommand = new NpgsqlCommand("INSERT INTO TeacherSubjects (teacher_id, subject_id) VALUES (@teacher_id, @subject_id)", connection))
            {
                insertCommand.Parameters.AddWithValue("@teacher_id", teacherId);
                insertCommand.Parameters.AddWithValue("@subject_id", subjectIds[subjectIndex]);
                insertCommand.ExecuteNonQuery();
            }
            subjectIndex++;
        }
    }

    static void PopulateScheduleTable(NpgsqlConnection connection)
    {
        Random random = new Random();
        string[] daysOfWeek = { "Понедiлок", "Вiвторок", "Середа", "Четвер", "П'ятниця" };
        string[] classTimes = { "08:30:00 - 09:10:00", "09:25:00 - 10:10:00", "10:20:00 - 11:05:00", "11:25:00 - 12:10:00", "12:20:00 - 13:05:00", "13:20:00 - 14:05:00", "14:15:00 - 15:00:00" };

        // Get all subject_ids
        List<int> subjectIds = new List<int>();
        using (NpgsqlCommand getSubjectsCommand = new NpgsqlCommand("SELECT subject_id FROM Subjects", connection))
        {
            using (NpgsqlDataReader reader = getSubjectsCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    int subjectId = reader.GetInt32(0);
                    subjectIds.Add(subjectId);
                }
            }
        }

        // Get TeacherSubjects data to determine teacher-subject relationships
        Dictionary<int, List<int>> teacherSubjectsMap = new Dictionary<int, List<int>>();
        using (NpgsqlCommand getTeacherSubjectsCommand = new NpgsqlCommand("SELECT teacher_id, subject_id FROM TeacherSubjects", connection))
        {
            using (NpgsqlDataReader reader = getTeacherSubjectsCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    int teacherId = reader.GetInt32(0);
                    int subjId = reader.GetInt32(1);

                    if (!teacherSubjectsMap.ContainsKey(teacherId))
                    {
                        teacherSubjectsMap[teacherId] = new List<int>();
                    }
                    teacherSubjectsMap[teacherId].Add(subjId);
                }
            }
        }

        int currentTeacherIndex = 0;
        for (int classGroupId = 1; classGroupId <= 22; classGroupId++) // Assuming you have 22 class groups indexed from 1 to 22
        {
            foreach (string dayOfWeek in daysOfWeek)
            {
                string classTime = classTimes[random.Next(classTimes.Length)];
                string[] timeParts = classTime.Split('-');

                string startTime = timeParts[0].Trim(); // Start time
                string endTime = timeParts[1].Trim();   // End time

                int subjectId = subjectIds[random.Next(subjectIds.Count)];
                int teacherId = 0;

                // Determine a teacher that is associated with this subject in TeacherSubjects
                foreach (var kv in teacherSubjectsMap)
                {
                    if (kv.Value.Contains(subjectId))
                    {
                        teacherId = kv.Key;
                        break;
                    }
                }

                if (teacherId == 0)
                {
                    teacherId = teacherSubjectsMap.ElementAt(currentTeacherIndex).Key;
                    currentTeacherIndex = (currentTeacherIndex + 1) % teacherSubjectsMap.Count;
                }

                using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Schedule (class_group_id, day_of_week, start_time, end_time, subject_id, teacher_id) " +
                    "VALUES (@class_group_id, @day_of_week, @start_time, @end_time, @subject_id, @teacher_id)", connection))
                {
                    command.Parameters.AddWithValue("@class_group_id", classGroupId);
                    command.Parameters.AddWithValue("@day_of_week", dayOfWeek);
                    command.Parameters.AddWithValue("@start_time", TimeSpan.Parse(startTime));
                    command.Parameters.AddWithValue("@end_time", TimeSpan.Parse(endTime));
                    command.Parameters.AddWithValue("@subject_id", subjectId);
                    command.Parameters.AddWithValue("@teacher_id", teacherId);

                    command.ExecuteNonQuery();
                }
            }
        }
    }

    static void PopulateStudentPerformanceTable(NpgsqlConnection connection)
    {
        Random random = new Random();

        // Retrieve all schedule_ids
        List<int> scheduleIds = new List<int>();
        using (NpgsqlCommand getScheduleCommand = new NpgsqlCommand("SELECT schedule_id, class_group_id FROM Schedule", connection))
        {
            using (NpgsqlDataReader reader = getScheduleCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    int scheduleId = reader.GetInt32(0);
                    int classGroupId = reader.GetInt32(1);
                    scheduleIds.Add(scheduleId);
                    Dictionary<int, int> scheduleClassGroups = new Dictionary<int, int>();
                    scheduleClassGroups[scheduleId] = classGroupId;
                }
            }
        }

        // Populate StudentPerformance for each schedule, its class, and corresponding students with respective subjects
        foreach (int scheduleId in scheduleIds)
        {
            int classGroupId = -1; // Variable to hold the class group for the current schedule

            // Retrieve the class group for the current schedule
            using (NpgsqlCommand getClassGroupForScheduleCommand = new NpgsqlCommand("SELECT class_group_id FROM Schedule WHERE schedule_id = @schedule_id", connection))
            {
                getClassGroupForScheduleCommand.Parameters.AddWithValue("@schedule_id", scheduleId);
                using (NpgsqlDataReader reader = getClassGroupForScheduleCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        classGroupId = reader.GetInt32(0);
                    }
                }
            }

            // Retrieve all student_ids for the current class group
            List<int> studentIds = new List<int>();
            using (NpgsqlCommand getStudentsForClassCommand = new NpgsqlCommand("SELECT user_id FROM Users WHERE class_group_id = @class_group_id AND role_id = 1", connection))
            {
                getStudentsForClassCommand.Parameters.AddWithValue("@class_group_id", classGroupId);
                using (NpgsqlDataReader reader = getStudentsForClassCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32(0);
                        studentIds.Add(studentId);
                    }
                }
            }

            // Retrieve all subject_ids for the current schedule
            List<int> subjectIds = new List<int>();
            using (NpgsqlCommand getSubjectsForScheduleCommand = new NpgsqlCommand("SELECT subject_id FROM Schedule WHERE schedule_id = @schedule_id", connection))
            {
                getSubjectsForScheduleCommand.Parameters.AddWithValue("@schedule_id", scheduleId);
                using (NpgsqlDataReader reader = getSubjectsForScheduleCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int subjectId = reader.GetInt32(0);
                        subjectIds.Add(subjectId);
                    }
                }
            }

            // Populate StudentPerformance for current schedule, class group students, and its subjects
            foreach (int studentId in studentIds)
            {
                foreach (int subjectId in subjectIds)
                {
                    DateTime date = DateTime.Now.AddDays(random.Next(-30, 0)); // Random date within the last month
                    string status = (random.Next(2) == 0) ? " " : " н "; // Random status
                    int grade = (status == " ") ? random.Next(1, 13) : 0; // Random grade if present

                    using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO StudentPerformance (student_id, subject_id, schedule_id, date, status, grade) " +
                        "VALUES (@student_id, @subject_id, @schedule_id, @date, @status, @grade)", connection))
                    {
                        command.Parameters.AddWithValue("@student_id", studentId);
                        command.Parameters.AddWithValue("@subject_id", subjectId);
                        command.Parameters.AddWithValue("@schedule_id", scheduleId);
                        command.Parameters.AddWithValue("@date", date);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@grade", grade);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    static void PopulateMaterialsTable(NpgsqlConnection connection)
    {
        Random random = new Random();
        string[] homeworks = { "ДЗ 1", "ДЗ 2", "ДЗ 3", "ДЗ 4", "ДЗ 5" };
        string[] topics = { "Тема 1", "Тема 2", "Тема 3", "Тема 4", "Тема 5" };
        string[] descriptions = { "Опис 1", "Опис 2", "Опис 3", "Опис 4", "Опис 5" };

        // Отримати всі schedule_ids
        List<int> scheduleIds = new List<int>();
        using (NpgsqlCommand getScheduleCommand = new NpgsqlCommand("SELECT schedule_id FROM Schedule", connection))
        {
            using (NpgsqlDataReader reader = getScheduleCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    int scheduleId = reader.GetInt32(0);
                    scheduleIds.Add(scheduleId);
                }
            }
        }

        foreach (int scheduleId in scheduleIds)
        {
            string homework = homeworks[random.Next(homeworks.Length)];
            string topic = topics[random.Next(topics.Length)];
            string description = descriptions[random.Next(descriptions.Length)];
            DateTime publishDate = DateTime.Now.AddDays(-random.Next(1, 30)); // Генеруємо дату від 1 до 30 днів назад.

            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Materials (schedule_id, homework, topic_of_the_lesson, description, publish_date) " +
                "VALUES (@schedule_id, @homework, @topic, @description, @publish_date)", connection))
            {
                command.Parameters.AddWithValue("@schedule_id", scheduleId);
                command.Parameters.AddWithValue("@homework", homework);
                command.Parameters.AddWithValue("@topic", topic);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@publish_date", publishDate);

                command.ExecuteNonQuery();
            }
        }
    }

    static void DisplayTable(NpgsqlConnection connection, string query)
    {
        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
        {
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            DataTable table = dataSet.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    Console.Write($"{col.ColumnName}: {row[col]} | ");
                }
                Console.WriteLine();
            }
        }
    }


}