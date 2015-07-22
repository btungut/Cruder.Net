using Cruder.Core.Configuration;
using Cruder.Core.ExceptionHandling;
using Cruder.Core.Model;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Cruder.Core.Module
{
    public static class Logger
    {
        public static Result<int> Log(string description)
        {
            return LogInternal(LogType.Info, Priority.None, description, null, null, LogModule.Unknown);
        }

        public static Result<int> Log(string description, Enum module)
        {
            return LogInternal(LogType.Info, Priority.None, description, null, null, module);
        }

        public static Result<int> Log(string description, string details)
        {
            return LogInternal(LogType.Info, Priority.None, description, details, null, LogModule.Unknown);
        }

        public static Result<int> Log(string description, string details, Enum module)
        {
            return LogInternal(LogType.Info, Priority.None, description, details, null, module);
        }

        public static Result<int> Log(LogType logType, Priority priority, string description, Enum module)
        {
            return LogInternal(logType, priority, description, null, null, module);
        }

        public static Result<int> Log(LogType logType, Priority priority, string description, string details, Enum module)
        {
            return LogInternal(logType, priority, description, details, null, module);
        }

        public static Result<int> Log(LogType logType, Priority priority, string description, Result result, Enum module)
        {
            if (result.HasError && result.Exception != null)
            {
                return Log(logType, priority, description, result.Exception, null, module);
            }
            else
            {
                return LogInternal(logType, priority, description, result.Message, null, module);
            }
        }

        public static Result<int> Log(LogType logType, Priority priority, string description, Exception exception, Enum module)
        {
            return Log(logType, priority, description, exception, null, module);
        }

        public static Result<int> Log(LogType logType, Priority priority, string description, Exception exception, Request request, Enum module)
        {
            string detailsString = exception == null ? null : JsonHelper.Serialize(exception);

            var log = LogInternal(logType, priority, description, detailsString, request, module);

            return log;
        }

        private static Result<int> LogInternal(LogType logType, Priority priority, string description, string details, Request request, Enum module)
        {
            if (request == null)
            {
                request = Request.GetCurrent();
            }

            string requestString = request == null ? null : JsonHelper.Serialize(request);

            return MssqlLog(logType, priority, description, details, requestString, module);
        }

        private static Result<int> MssqlLog(LogType logType, Priority priority, string description, string details, string request, Enum module)
        {
            Result<int> result = new Result<int>();

            if (ConfigurationFactory.Logger.LoggingLevel != LoggingLevel.NoLog)
            {
                string connString = System.Configuration.ConfigurationManager.ConnectionStrings[ConfigurationFactory.Logger.ConnectionStringKey].ConnectionString;
                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand();

                try
                {
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("Insert into {0} (Type,Priority,Description,details,Request,Module,CreatedOn,UpdatedOn,CreatedBy, UpdatedBy) output INSERTED.ID VALUES (@LogType,@Priority,@Description,@details,@Request,@Module,@CreatedOn,@UpdatedOn,@CreatedBy,@UpdatedBy)", ConfigurationFactory.Logger.TableName);

                    cmd.Parameters.AddWithValue("@LogType", (int)logType);
                    cmd.Parameters.AddWithValue("@Priority", (int)priority);
                    cmd.Parameters.AddWithValue("@Description", description.ToString());

                    object detailsVal;
                    if (string.IsNullOrEmpty(details))
                    {
                        detailsVal = DBNull.Value;
                    }
                    else
                    {
                        detailsVal = details.ToString();
                    }

                    object requestVal;
                    if (string.IsNullOrEmpty(request))
                    {
                        requestVal = DBNull.Value;
                    }
                    else
                    {
                        requestVal = request.ToString();
                    }

                    cmd.Parameters.AddWithValue("@details", detailsVal);
                    cmd.Parameters.AddWithValue("@Request", requestVal);
                    cmd.Parameters.AddWithValue("@Module", module.ToString());
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@CreatedBy", 1);
                    cmd.Parameters.AddWithValue("@UpdatedBy", 1);

                    conn.Open();
                    result.Data = (int)cmd.ExecuteScalar();
                }
                catch (Exception e)
                {
                    result.Exception = new FrameworkException("Logger.MssqlLog()", "An error occurred while DbLogging", e);
                    result.HasError = true;

                    try
                    {
                        string path = Path.GetDirectoryName(typeof(Logger).Assembly.CodeBase).Replace(@"file:\", string.Empty) + "\\" + ConfigurationFactory.Logger.FilePath;
                        FileInfo file = new FileInfo(path);

                        if (file.Exists && file.Length > 10485760) // hardcoded 10mb
                        {
                            string newPath = string.Concat(
                                Path.GetDirectoryName(path),
                                Path.DirectorySeparatorChar,
                                Path.GetFileNameWithoutExtension(path),
                                "_",
                                DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm"), //hardcoded format
                                Path.GetExtension(path));


                            file.MoveTo(newPath);
                        }
                        else if(!Directory.Exists(Path.GetDirectoryName(path)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                        }

                        StringBuilder builder = new StringBuilder();

                        builder.AppendLine(JsonHelper.Serialize(new
                                {
                                    @CreatedOn = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm"), //hardcoded format
                                    @LogType = logType,
                                    @Priority = priority,
                                    @description = description,
                                    @details = details,
                                    @request = request,
                                    @module = module
                                }));
                        builder.AppendLine("==============================================");

                        File.AppendAllText(path, builder.ToString());

                    }
                    catch (Exception) { }
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn.Dispose();   
                    }
                }
            }

            return result;
        }
    }
}
