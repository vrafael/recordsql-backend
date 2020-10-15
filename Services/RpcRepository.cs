using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RecordBackend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace RecordBackend.Services
{
    public interface IRpcRepository
    {
        Task<RpcResponseContainer> ExecAsync(RpcRequestContainer requestContainer);
    }

    public class RpcRepository : IRpcRepository
    {
        string connectionString = string.Empty;

        public RpcRepository(string conn)
        {
            connectionString = conn;
        }

        public RpcRepository(IOptions<ConfigureOptions> optionsAccessor)
        {
            connectionString = optionsAccessor.Value.ConnectionStringUserAPI;
        }

        public async Task<RpcResponseContainer> ExecAsync(RpcRequestContainer requestContainer)
        {
            RpcResponseContainer responseContainer = new RpcResponseContainer();
            responseContainer.RpcResponse.ID = requestContainer.RpcRequest.ID;

            using (SqlConnection conn = new SqlConnection(connectionString))
            //startup using (SqlCommand cmdAuth = new SqlCommand(@"Auth.Startup", conn))
            using (SqlCommand cmdQuery = requestContainer.RpcRequest.Method != null ? new SqlCommand(requestContainer.RpcRequest.Method, conn) : null) //создаем если имя процедуры указано
            {
                /*startup
                cmdAuth.CommandType = CommandType.StoredProcedure;

                cmdAuth.Parameters.AddRange(new SqlParameter[] {
                    new SqlParameter() {ParameterName = @"UserAgent", Direction = ParameterDirection.Input, SqlDbType = SqlDbType.NVarChar, Size = 512, Value = requestContainer.Identity.UserAgent },
                    new SqlParameter() {ParameterName = @"IPAddress", Direction = ParameterDirection.Input, SqlDbType = SqlDbType.NVarChar, Size = 512, Value = requestContainer.Identity.IPAddress},
                    new SqlParameter() {ParameterName = @"SessionKey", Direction = ParameterDirection.InputOutput, SqlDbType = SqlDbType.NVarChar, Size = 512, Value = requestContainer.Identity.SessionKey },
                    new SqlParameter() {ParameterName = @"Message", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.NVarChar, Size = 512 },
                    //new SqlParameter() {ParameterName = @"ExpirationDate", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.DateTime2 },
                });*/

                if (cmdQuery != null)
                {
                    cmdQuery.CommandType = CommandType.StoredProcedure;

                    if (requestContainer.RpcRequest.Params != null)
                    {
                        var prms = requestContainer.RpcRequest.Params as IDictionary<string, object>;

                        foreach (string key in prms.Keys)
                        {
                            if (prms[key] is ExpandoObject)
                            {
                                cmdQuery.Parameters.Add(new SqlParameter()
                                {
                                    ParameterName = key,
                                    Value = JsonConvert.SerializeObject(prms[key]),
                                });
                            }
                            else
                            {
                                cmdQuery.Parameters.Add(new SqlParameter()
                                {
                                    ParameterName = key,
                                    Value = prms[key],
                                });
                            }
                        }
                    }
                }

                try
                {
                    await conn.OpenAsync();
                    //cmd.Prepare();

                    /*startup 
                    await cmdAuth.ExecuteNonQueryAsync();
                    responseContainer.SessionKey = cmdAuth.Parameters[@"SessionKey"].Value.ToString();*/
                    ////var expiarationDate = cmdAuth.Parameters[@"ExpirationDate"].Value;
                    ////responseContainer.ExpirationDate = expiarationDate == DBNull.Value ? null : (DateTime?)expiarationDate;

                    /*startup 
                    var message = cmdAuth.Parameters[@"Message"].Value.ToString();
                    if (!string.IsNullOrEmpty(message)) //если есть сообщение - возвращаем ошибку авторизации
                    {
                        responseContainer.RpcResponse.Error = new RpcError(401, message);
                        return responseContainer;
                    }*/

                    if (cmdQuery != null)
                    {
                        using (var datareader = await cmdQuery.ExecuteJsonReaderAsync())
                        {
                            responseContainer.RpcResponse.Result = datareader.ReadAll();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = ex.Message.Replace("\nThe transaction ended in the trigger. The batch has been aborted.", @""); //вырезаем из ошибки текст об откате батча
                    responseContainer.RpcResponse.Error = new RpcError(500, message);
                }
                finally
                {
                    conn.Close();
                }
            }

            return responseContainer;
        }
    }

    //https://stackoverflow.com/questions/50750143/executexmlreader-fails-on-specific-json-from-sql-server
    static class SqlJsonUtils
    {
        public async static Task<SqlJSONReader> ExecuteJsonReaderAsync(this SqlCommand cmd)
        {
            var rdr = await cmd.ExecuteReaderAsync();
            return new SqlJSONReader(rdr);
        }

        public class SqlJSONReader : TextReader
        {
            private SqlDataReader SqlReader { get; set; }
            private string CurrentLine { get; set; }
            private int CurrentPostion { get; set; }

            public SqlJSONReader(SqlDataReader reader)
            {
                CurrentLine = "";
                CurrentPostion = 0;
                this.SqlReader = reader;
            }
            public override int Peek()
            {
                return GetChar(false);
            }
            public override int Read()
            {
                return GetChar(true);
            }
            public int GetChar(bool Advance)
            {
                while (CurrentLine.Length == CurrentPostion)
                {
                    if (!SqlReader.Read())
                    {
                        return -1;
                    }
                    CurrentLine = SqlReader.GetString(0);
                    CurrentPostion = 0;
                }
                var rv = CurrentLine[CurrentPostion];
                if (Advance)
                    CurrentPostion += 1;

                return rv;
            }

            public object ReadAll()
            {
                var sbResult = new StringBuilder();

                if (SqlReader.HasRows)
                {
                    while (SqlReader.Read())
                        sbResult.Append(SqlReader.GetString(0));
                }
                else
                    return string.Empty;

                // Clean up any JSON escapes before returning
                return JsonConvert.DeserializeObject(sbResult.ToString());
            }

            public override void Close()
            {
                SqlReader.Close();
            }
        }
    }
}