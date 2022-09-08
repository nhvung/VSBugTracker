using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using VSBugTracker.ADO;
using VSBugTracker.Models;

namespace VSBugTracker
{
    public class BugTrackerProcess
    {
        
        public BugTrackerProcess()
        {
        }

        #region Private Functions
        string ToDBValues<TADO>(string[] insertedFields, params TADO[] dbObjs)
        {
            if (dbObjs == null || dbObjs.Length == 0) return "";
            Type tADO = typeof(TADO);
            var props = tADO.GetProperties().ToDictionary(ite=>ite.Name, ite=>ite, StringComparer.InvariantCultureIgnoreCase);
            try
            {
                List<string> values = new List<string>();
                foreach (TADO dbObj in dbObjs)
                {
                    string[] objValues = insertedFields.Select(ite => {
                        if (props.ContainsKey(ite))
                        {
                            object value = props[ite].GetValue(dbObj, null);
                            if (props[ite].PropertyType == typeof(byte[]))
                            {
                                string sValue = "0x" + BitConverter.ToString((byte[])value).Replace("-","");
                                return sValue;
                            }
                            else if (props[ite].PropertyType == typeof(string))
                            {
                                string sValue = value.ToString().Replace("\\","\\\\").Replace("'", "''");
                                return sValue == null ? "null" : string.Format("'{0}'", sValue);
                            }
                            else
                                return value == null ? "null" : string.Format("'{0}'", value);
                        }
                        else
                        {
                            return "null";
                        }
                        
                    }).ToArray();
                    values.Add("(" + string.Join(",", objValues) + ")");
                }
                string result = string.Join(",", values);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        List<TResult> ExecuteReader<TResult>(string query) where TResult : class
        {
            if (string.IsNullOrEmpty(GlobalVariables.ConnectionString)) return new List<TResult>();
            OdbcConnection cn = new OdbcConnection(GlobalVariables.ConnectionString);
            try
            {
                List<TResult> result = new List<TResult>();
                cn.Open();
                OdbcCommand cmd = cn.CreateCommand();
                cmd.CommandText = query;
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {

                    Type tResult = typeof(TResult);
                    var props = tResult.GetProperties();
                    while (reader.Read())
                    {
                        TResult res = Activator.CreateInstance<TResult>();
                        foreach (var prop in props)
                        {
                            try
                            {
                                object value = reader[prop.Name];
                                if (value.Equals(DBNull.Value)) value = null;
                                if (prop.PropertyType == typeof(string))
                                {
                                    prop.SetValue(res, value ?? "", null);
                                }
                                else if (prop.PropertyType == typeof(int))
                                {
                                    prop.SetValue(res, value, null);
                                }
                                else if (prop.PropertyType == typeof(byte[]))
                                {
                                    prop.SetValue(res, value, null);
                                }
                            }
                            catch
                            {

                            }
                        }
                        result.Add(res);
                    }
                }
                reader.Close();
                cmd.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
        }
        List<TResult> ExecuteReader<TResult>(string query, string[] propertyNames) where TResult : class
        {
            if (string.IsNullOrEmpty(GlobalVariables.ConnectionString)) return new List<TResult>();
            OdbcConnection cn = new OdbcConnection(GlobalVariables.ConnectionString);
            try
            {
                List<TResult> result = new List<TResult>();
                cn.Open();
                OdbcCommand cmd = cn.CreateCommand();
                cmd.CommandText = query;
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {

                    Type tResult = typeof(TResult);
                    var props = tResult.GetProperties().Where(ite => propertyNames.Contains(ite.Name, StringComparer.InvariantCultureIgnoreCase)).ToArray();
                    while (reader.Read())
                    {
                        TResult res = Activator.CreateInstance<TResult>();
                        foreach (var prop in props)
                        {
                            try
                            {
                                object value = reader[prop.Name];
                                if (value.Equals(DBNull.Value)) value = null;
                                if (prop.PropertyType == typeof(string))
                                {
                                    prop.SetValue(res, value ?? "", null);
                                }
                                else if (prop.PropertyType == typeof(int))
                                {
                                    prop.SetValue(res, value, null);
                                }
                                else if (prop.PropertyType == typeof(byte[]))
                                {
                                    prop.SetValue(res, value, null);
                                }
                            }
                            catch
                            {

                            }
                        }
                        result.Add(res);
                    }
                }
                reader.Close();
                cmd.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
        }
        int ExecuteNonQuery(string query)
        {
            if (string.IsNullOrEmpty(GlobalVariables.ConnectionString)) return 0;
            OdbcConnection cn = new OdbcConnection(GlobalVariables.ConnectionString);
            try
            {
                cn.Open();
                OdbcCommand cmd = cn.CreateCommand();
                cmd.CommandText = query;
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
        }
        object ExecuteScalar(string query)
        {
            if (string.IsNullOrEmpty(GlobalVariables.ConnectionString)) return null;
            OdbcConnection cn = new OdbcConnection(GlobalVariables.ConnectionString);
            try
            {
                cn.Open();
                OdbcCommand cmd = cn.CreateCommand();
                cmd.CommandText = query;
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
        }
        int ExecuteInsert<TADO>(string tableName, string[] insertedFields, params TADO[] dbObjs)
        {
            try
            {
                string values = ToDBValues(insertedFields, dbObjs);
                string query = string.Format("insert into {0}({1}) values {2}", tableName, string.Join(", ", insertedFields), values);
                return ExecuteNonQuery(query);
            }
            catch ( Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region TAccount
        int GetMaxAccountID()
        {
            try
            {
                string query = string.Format("select max(AccountID) from TAccount");
                object objValue = ExecuteScalar(query);
                if (objValue.Equals(DBNull.Value)) objValue = null;
                return Convert.ToInt32(objValue ?? 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AddAccount(params TAccountADO[] accounts)
        {
            try
            {
                if (accounts == null || accounts.Length == 0) return 0;
                string[] loginNames = accounts.Select(ite => ite.AccountLogin.Replace("\\", "\\\\").Replace("'", "''")).Select(ite=>"'" + ite + "'").ToArray();
                string query = "select * from TAccount where AccountLogin in (" + string.Join(",", loginNames) + ")";
                loginNames = ExecuteReader<TAccountADO>(query)?.Select(ite => ite.AccountLogin).ToArray();

                accounts = accounts?.Where(ite => !loginNames.Contains(ite.AccountLogin, StringComparer.InvariantCultureIgnoreCase)).ToArray();
                if (accounts == null || accounts.Length == 0) return 0;

                int maxAccountID = GetMaxAccountID();
                accounts = accounts.Select((ite, idx) => { ite.AccountID = maxAccountID + idx + 1; return ite; }).ToArray();
                string[] insertedFields = new string[] { "AccountID", "AccountLogin", "AccountName", "EncryptedPassword", "AccountType", "AccountRight" };
                int exec = ExecuteInsert("TAccount", insertedFields, accounts);
                return exec;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TAccountADO> GetAllAccounts()
        {
            try
            {
                string query = "select * from TAccount";
                List<TAccountADO> result = ExecuteReader<TAccountADO>(query);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int AssignRight(int accountID, int right)
        {
            try
            {
                string query = "update TAccount set AccountRight = " + right + " where AccountID = " + accountID;
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AssignRight(int[] accountIDs, int right)
        {
            try
            {
                if (accountIDs?.Length > 0)
                {
                    string query = "update TAccount set AccountRight = " + right + " where AccountID in (" + string.Join(", ", accountIDs) + ")";
                    return ExecuteNonQuery(query);
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateAccount(TAccountADO account)
        {
            try
            {
                string query = string.Format("update TAccount set AccountLogin = '{0}', AccountName = '{1}', EncryptedPassword = '{2}', AccountType = {3}, AccountRight = {4} where AccountID = {5}",
                    account.AccountLogin, account.AccountName, account.EncryptedPassword, account.AccountType, account.AccountRight, account.AccountID);
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteAccounts(params int[] accountIDs)
        {
            try
            {
                if (accountIDs == null || accountIDs.Length == 0 || accountIDs[0] == 0) return 0;
                string query = "delete from TAccount where AccountID in (" + string.Join(", ", accountIDs) + ")";
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteAccounts(params string[] loginNames)
        {
            try
            {
                if (loginNames == null || loginNames.Length == 0 || loginNames[0].Equals("admin", StringComparison.InvariantCultureIgnoreCase)) return 0;
                string query = "delete from TAccount where AccountLogin in (" + string.Join(", ", loginNames.Select(ite=>"'" + ite + "'")) + ")";
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TAccountADO Login(string loginName, string encryptedPassword)
        {
            try
            {
                string query = string.Format("select * from TAccount where AccountLogin = '{0}' and EncryptedPassword = '{1}' limit 1", loginName, encryptedPassword);
                var accounts = ExecuteReader<TAccountADO>(query);
                return accounts == null ? null : accounts.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TAccountADO> GetAllCoders()
        {
            try
            {
                string query = "select * from TAccount where AccountType = 1";
                List<TAccountADO> result = ExecuteReader<TAccountADO>(query);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<TAccountADO> GetAllTesters()
        {
            try
            {
                string query = "select * from TAccount where AccountType = 2";
                List<TAccountADO> result = ExecuteReader<TAccountADO>(query);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<TBugVersion> GetAllBugVersions()
        {
            try
            {
                string query = "select distinct Version, ResolveVersion from TBug";
                List<TBugVersion> result = ExecuteReader<TBugVersion>(query);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<TBugVersion> GetAllBugVersions(int sprintID)
        {
            try
            {
                string query = "select distinct Version, ResolveVersion from TBug where SprintID = " + sprintID;
                List<TBugVersion> result = ExecuteReader<TBugVersion>(query);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int ChangePassword(string loginName, string encryptedPassword)
        {
            try
            {
                string query = string.Format("update TAccount set EncryptedPassword = '{0}' where AccountLogin = '{1}'", encryptedPassword, loginName);
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region TAttachment
        int GetMaxAttachmentID()
        {
            try
            {
                string query = string.Format("select max(AttachmentID) from TAttachment");
                object objValue = ExecuteScalar(query);
                if (objValue.Equals(DBNull.Value)) objValue = null;
                return Convert.ToInt32(objValue ?? 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TAttachmentADO GetAttachment(int attachmentID)
        {
            try
            {
                string query = "select * from TAttachment where AttachmentID = " + attachmentID;
                List<TAttachmentADO> result = ExecuteReader<TAttachmentADO>(query);
                return result?.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TAttachmentADO> GetAttachments(int bugID, bool includeData = true)
        {
            try
            {
                string selectFields = includeData ? "*" : "AttachmentID, BugID, FileName, FileExtension";
                string query = "select " + selectFields + " from TAttachment where BugID = " + bugID;
                List<TAttachmentADO> result = ExecuteReader<TAttachmentADO>(query);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TAttachmentADO> GetAttachments(int[] bugIDs, bool includeData)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return new List<TAttachmentADO>();
                string selectFields = includeData ? "*" : "AttachmentID, BugID, FileName, FileExtension";
                string query = "select " + selectFields + " from TAttachment where BugID in (" + string.Join(", ", bugIDs) + ")";
                List<TAttachmentADO> result = includeData ? ExecuteReader<TAttachmentADO>(query) : ExecuteReader<TAttachmentADO>(query, new string[] { "AttachmentID", "BugID", "FileName", "FileExtension" });
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AddAttachment(params TAttachmentADO[] attachments)
        {
            try
            {
                if (attachments == null || attachments.Length == 0) return 0;

                int maxAttachmentID = GetMaxAttachmentID();
                attachments = attachments.Select((ite, idx) => { ite.AttachmentID = maxAttachmentID + idx + 1; return ite; }).ToArray();

                string[] insertedFields = new string[] { "AttachmentID", "BugID", "FileName", "FileExtension", "FileData" };
                int exec = ExecuteInsert("TAttachment", insertedFields, attachments);
                return exec;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DeleteAttachmentsFollowBug(params int[] bugIDs)
        {
            if (bugIDs == null || bugIDs.Length == 0) return 0;
            string query = string.Format("delete from TAttachment where BugID in ({0})", string.Join(",", bugIDs));
            return ExecuteNonQuery(query);
        }
        public int DeleteAttachments(params int[] attachmentIDs)
        {
            if (attachmentIDs == null || attachmentIDs.Length == 0) return 0;
            string query = string.Format("delete from TAttachment where AttachmentID in ({0})", string.Join(",", attachmentIDs));
            return ExecuteNonQuery(query);
        }
        #endregion

        #region TBug
        int GetMaxBugID()
        {
            try
            {
                string query = string.Format("select max(BugID) from TBug");
                object objValue = ExecuteScalar(query);
                if (objValue.Equals(DBNull.Value)) objValue = null;
                return Convert.ToInt32(objValue ?? 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AddBugs(params TBugFullInfo[] bugs)
        {
            try
            {
                if (bugs == null || bugs.Length == 0) return 0;
                int maxBugID = GetMaxBugID();

                int createdDate = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
                bugs = bugs.Select((ite, idx) =>
                {
                    ite.BugID = maxBugID + idx + 1;
                    if (ite.Attachments != null && ite.Attachments.Count > 0)
                        ite.Attachments.ForEach(ite1 => ite1.BugID = ite.BugID);
                    ite.CreatedDate = createdDate;
                    return ite;
                }).ToArray();
                string[] insertFields = new string[] { "BugID", "Description", "TaskID", "SprintID", "AddedAccountID", "OpenedAccountID", "VerifiedAccountID", "AssignedAccountID", "PriorityID", "StatusID", "CreatedDate", "ResolvedDate", "OpenedDate", "VerifiedDate", "Comment", "Version", "ResolveVersion" };
                int exec = ExecuteInsert("TBug", insertFields, bugs);                
                TAttachmentADO[] attachments = bugs.Where(ite => ite.Attachments != null && ite.Attachments.Count > 0).SelectMany(ite => ite.Attachments).ToArray();
                if (attachments.Length > 0)                
                    AddAttachment(attachments);                
                return exec;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public int MoveBugs(int sprintID, int taskID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                string query = string.Format("update TBug set SprintID = {0}, TaskID = {1} where BugID in ({2})", sprintID, taskID, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DeleteBugs(params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                DeleteAttachmentsFollowBug(bugIDs);
                string query = string.Format("delete from TBug where BugID in ({0})", string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TBugFullInfo> GetBugs(int sprintID, int taskID, int[] statusIDs, int[] priorityIDs, int assigneeID)
        {
            try
            {
                List<string> filters = new List<string>();
                if (sprintID > -1) filters.Add(string.Format("SprintID = {0}", sprintID));
                if (taskID > -1) filters.Add(string.Format("TaskID = {0}", taskID));
                if (assigneeID > -1) filters.Add(string.Format("AssignedAccountID = {0}", assigneeID));
                if (statusIDs !=null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && priorityIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                    int[] bugIds = dbBugs.Select(ite => ite.BugID).ToArray(), exeBugIDs;
                    while (bugIds.Length > 0)
                    {
                        exeBugIDs = bugIds.Take(20).ToArray();
                        var attachments = GetAttachments(exeBugIDs, false);
                        if (attachments != null && attachments.Count > 0)
                        {
                            var m_attachment = attachments.GroupBy(ite => ite.BugID).ToDictionary(ite => ite.Key, ite => ite.ToList());
                            result.ForEach(ite => { if (m_attachment.ContainsKey(ite.BugID)) ite.Attachments = m_attachment[ite.BugID]; });
                        }
                        bugIds = bugIds.Skip(20).ToArray();
                    }
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TBugFullInfo> GetBugsForStatistic(int sprintID, int taskID, int[] statusIDs, int[] priorityIDs, int assigneeID)
        {
            try
            {
                List<string> filters = new List<string>();
                if (sprintID > -1) filters.Add(string.Format("SprintID = {0}", sprintID));
                if (taskID > -1) filters.Add(string.Format("TaskID = {0}", taskID));
                if (assigneeID > -1) filters.Add(string.Format("AssignedAccountID = {0}", assigneeID));
                if (statusIDs != null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && priorityIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TBugFullInfo> GetBugs(int sprintID, int taskID, int[] statusIDs, int[] priorityIDs, int assigneeID, string[] reportVersions, string[] resolveVersions)
        {
            try
            {
                List<string> filters = new List<string>();
                if (sprintID > -1) filters.Add(string.Format("SprintID = {0}", sprintID));
                if (taskID > -1) filters.Add(string.Format("TaskID = {0}", taskID));
                if (assigneeID > -1) filters.Add(string.Format("AssignedAccountID = {0}", assigneeID));
                if (statusIDs != null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && priorityIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));
                if (reportVersions != null && reportVersions.Length > 0) filters.Add(string.Format("Version in ({0})", string.Join(", ", reportVersions.Select(ite => "'" + ite + "'"))));
                if (resolveVersions != null && resolveVersions.Length > 0) filters.Add(string.Format("ResolveVersion in ({0})", string.Join(", ", resolveVersions.Select(ite => "'" + ite + "'"))));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                    int[] bugIds = dbBugs.Select(ite => ite.BugID).ToArray(), exeBugIDs;
                    while (bugIds.Length > 0)
                    {
                        exeBugIDs = bugIds.Take(20).ToArray();
                        var attachments = GetAttachments(exeBugIDs, false);
                        if (attachments != null && attachments.Count > 0)
                        {
                            var m_attachment = attachments.GroupBy(ite => ite.BugID).ToDictionary(ite => ite.Key, ite => ite.ToList());
                            result.ForEach(ite => { if (m_attachment.ContainsKey(ite.BugID)) ite.Attachments = m_attachment[ite.BugID]; });
                        }
                        bugIds = bugIds.Skip(20).ToArray();
                    }
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TBugFullInfo> GetBugsForStatistic(int sprintID, int taskID, int[] statusIDs, int[] priorityIDs, int assigneeID, string[] reportVersions, string[] resolveVersions)
        {
            try
            {
                if (reportVersions == null || reportVersions.Length == 0) return new List<TBugFullInfo>();
                if (resolveVersions == null || resolveVersions.Length == 0) return new List<TBugFullInfo>();

                List<string> filters = new List<string>();
                if (sprintID > -1) filters.Add(string.Format("SprintID = {0}", sprintID));
                if (taskID > -1) filters.Add(string.Format("TaskID = {0}", taskID));
                if (assigneeID > -1) filters.Add(string.Format("AssignedAccountID = {0}", assigneeID));
                if (statusIDs != null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && priorityIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));
                if (reportVersions != null && reportVersions.Length > 0) filters.Add(string.Format("Version in ({0})", string.Join(", ", reportVersions.Select(ite => "'" + ite + "'"))));
                if (resolveVersions != null && resolveVersions.Length > 0) filters.Add(string.Format("ResolveVersion in ({0})", string.Join(", ", resolveVersions.Select(ite => "'" + ite + "'"))));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();                    
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TBugFullInfo> GetBugs(int sprintID, int taskID, int[] statusIDs, int[] priorityIDs, int[] assigneeIDs)
        {
            try
            {
                List<string> filters = new List<string>();
                if (sprintID > -1) filters.Add(string.Format("SprintID = {0}", sprintID));
                if (taskID > -1) filters.Add(string.Format("TaskID = {0}", taskID));
                if (assigneeIDs != null && assigneeIDs.Length > 0) filters.Add(string.Format("AssignedAccountID in ({0})", string.Join(", ", assigneeIDs)));
                if (statusIDs != null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && priorityIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                    int[] bugIds = dbBugs.Select(ite => ite.BugID).ToArray(), exeBugIDs;
                    while (bugIds.Length > 0)
                    {
                        exeBugIDs = bugIds.Take(20).ToArray();
                        var attachments = GetAttachments(exeBugIDs, false);
                        if (attachments != null && attachments.Count > 0)
                        {
                            var m_attachment = attachments.GroupBy(ite => ite.BugID).ToDictionary(ite => ite.Key, ite => ite.ToList());
                            result.ForEach(ite => { if (m_attachment.ContainsKey(ite.BugID)) ite.Attachments = m_attachment[ite.BugID]; });
                        }
                        bugIds = bugIds.Skip(20).ToArray();
                    }
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TBugFullInfo> GetBugs(int sprintID, int taskID, int[] statusIDs, int[] priorityIDs, int[] assigneeIDs, int[] reportIDs)
        {
            try
            {
                List<string> filters = new List<string>();
                if (sprintID > -1) filters.Add(string.Format("SprintID = {0}", sprintID));
                if (taskID > -1) filters.Add(string.Format("TaskID = {0}", taskID));
                if (assigneeIDs != null && assigneeIDs.Length > 0) filters.Add(string.Format("AssignedAccountID in ({0})", string.Join(", ", assigneeIDs)));
                if (reportIDs != null && reportIDs.Length > 0) filters.Add(string.Format("AddedAccountID in ({0})", string.Join(", ", reportIDs)));
                if (statusIDs != null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && priorityIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                    int[] bugIds = dbBugs.Select(ite => ite.BugID).ToArray(), exeBugIDs;
                    while (bugIds.Length > 0)
                    {
                        exeBugIDs = bugIds.Take(20).ToArray();
                        var attachments = GetAttachments(exeBugIDs, false);
                        if (attachments != null && attachments.Count > 0)
                        {
                            var m_attachment = attachments.GroupBy(ite => ite.BugID).ToDictionary(ite => ite.Key, ite => ite.ToList());
                            result.ForEach(ite => { if (m_attachment.ContainsKey(ite.BugID)) ite.Attachments = m_attachment[ite.BugID]; });
                        }
                        bugIds = bugIds.Skip(20).ToArray();
                    }
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TBugFullInfo> GetBugs(int sprintID, int taskID, int[] statusIDs, int[] priorityIDs, int[] assigneeIDs, int[] reportIDs, string[] reportVersions, string[] resolveVersions)
        {
            try
            {
                List<string> filters = new List<string>();
                if (sprintID > -1) filters.Add(string.Format("SprintID = {0}", sprintID));
                if (taskID > -1) filters.Add(string.Format("TaskID = {0}", taskID));
                if (assigneeIDs != null && assigneeIDs.Length > 0) filters.Add(string.Format("AssignedAccountID in ({0})", string.Join(", ", assigneeIDs)));
                if (reportIDs != null && reportIDs.Length > 0) filters.Add(string.Format("AddedAccountID in ({0})", string.Join(", ", reportIDs)));
                if (statusIDs != null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && priorityIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));
                if (reportVersions != null && reportVersions.Length > 0) filters.Add(string.Format("Version in ({0})", string.Join(", ", reportVersions.Select(ite=> "'" + ite + "'"))));
                if (resolveVersions != null && resolveVersions.Length > 0) filters.Add(string.Format("ResolveVersion in ({0})", string.Join(", ", resolveVersions.Select(ite => "'" + ite + "'"))));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                    int[] bugIds = dbBugs.Select(ite => ite.BugID).ToArray(), exeBugIDs;
                    while (bugIds.Length > 0)
                    {
                        exeBugIDs = bugIds.Take(20).ToArray();
                        var attachments = GetAttachments(exeBugIDs, false);
                        if (attachments != null && attachments.Count > 0)
                        {
                            var m_attachment = attachments.GroupBy(ite => ite.BugID).ToDictionary(ite => ite.Key, ite => ite.ToList());
                            result.ForEach(ite => { if (m_attachment.ContainsKey(ite.BugID)) ite.Attachments = m_attachment[ite.BugID]; });
                        }
                        bugIds = bugIds.Skip(20).ToArray();
                    }
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TBugFullInfo> GetBugsOfCoder(int sprintID, int taskID, int[] statusIDs, int[] priorityIDs, int assigneeID)
        {
            try
            {
                List<string> filters = new List<string>();
                if (sprintID > -1) filters.Add(string.Format("SprintID = {0}", sprintID));
                if (taskID > -1) filters.Add(string.Format("TaskID = {0}", taskID));
                if (assigneeID > -1) filters.Add(string.Format("AssignedAccountID = {0}", assigneeID));
                if (statusIDs != null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && statusIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                    int[] bugIds = dbBugs.Select(ite => ite.BugID).ToArray(), exeBugIDs;
                    while (bugIds.Length > 0)
                    {
                        exeBugIDs = bugIds.Take(20).ToArray();
                        var attachments = GetAttachments(exeBugIDs, false);
                        if (attachments != null && attachments.Count > 0)
                        {
                            var m_attachment = attachments.GroupBy(ite => ite.BugID).ToDictionary(ite => ite.Key, ite => ite.ToList());
                            result.ForEach(ite => { if (m_attachment.ContainsKey(ite.BugID)) ite.Attachments = m_attachment[ite.BugID]; });
                        }
                        bugIds = bugIds.Skip(20).ToArray();
                    }
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TBugFullInfo> GetBugsOfTester(int sprintID, int taskID, int[] statusIDs, int[] priorityIDs, int assigneeID)
        {
            try
            {
                List<string> filters = new List<string>();
                if (sprintID > -1) filters.Add(string.Format("SprintID = {0}", sprintID));
                if (taskID > -1) filters.Add(string.Format("TaskID = {0}", taskID));
                if (assigneeID > -1) filters.Add(string.Format("AssignedAccountID = {0}", assigneeID));
                if (statusIDs != null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && statusIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                    int[] bugIds = dbBugs.Select(ite => ite.BugID).ToArray(), exeBugIDs;
                    while (bugIds.Length > 0)
                    {
                        exeBugIDs = bugIds.Take(20).ToArray();
                        var attachments = GetAttachments(exeBugIDs, false);
                        if (attachments != null && attachments.Count > 0)
                        {
                            var m_attachment = attachments.GroupBy(ite => ite.BugID).ToDictionary(ite => ite.Key, ite => ite.ToList());
                            result.ForEach(ite => { if (m_attachment.ContainsKey(ite.BugID)) ite.Attachments = m_attachment[ite.BugID]; });
                        }
                        bugIds = bugIds.Skip(20).ToArray();
                    }
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TBugFullInfo> GetBugs(DateTime beginDate, DateTime endDate)
        {
            try
            {
                List<string> filters = new List<string>();
                if (beginDate != DateTime.MinValue) filters.Add(string.Format("CreatedDate >= {0:yyyyMMdd}", beginDate));
                if (endDate != DateTime.MinValue) filters.Add(string.Format("CreatedDate <= {0:yyyyMMdd}", endDate));


                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                   
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<TBugFullInfo> GetBugsForStatistic(DateTime beginDate, DateTime endDate, int[] statusIDs, int[] priorityIDs, string[] reportVersions, string[] resolveVersions)
        {
            try
            {
                if (reportVersions == null || reportVersions.Length == 0) return new List<TBugFullInfo>();
                if (resolveVersions == null || resolveVersions.Length == 0) return new List<TBugFullInfo>();
                List<string> filters = new List<string>();
                if (beginDate != DateTime.MinValue) filters.Add(string.Format("CreatedDate >= {0:yyyyMMdd}", beginDate));
                if (endDate != DateTime.MinValue) filters.Add(string.Format("CreatedDate <= {0:yyyyMMdd}", endDate));

                if (statusIDs != null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && priorityIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));
                if (reportVersions != null && reportVersions.Length > 0) filters.Add(string.Format("Version in ({0})", string.Join(", ", reportVersions.Select(ite => "'" + ite + "'"))));
                if (resolveVersions != null && resolveVersions.Length > 0) filters.Add(string.Format("ResolveVersion in ({0})", string.Join(", ", resolveVersions.Select(ite => "'" + ite + "'"))));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<TBugFullInfo> GetBugsForStatistic(DateTime beginDate, DateTime endDate, int[] statusIDs, int[] priorityIDs)
        {
            try
            {
                List<string> filters = new List<string>();
                if (beginDate != DateTime.MinValue) filters.Add(string.Format("CreatedDate >= {0:yyyyMMdd}", beginDate));
                if (endDate != DateTime.MinValue) filters.Add(string.Format("CreatedDate <= {0:yyyyMMdd}", endDate));

                if (statusIDs != null && statusIDs.Length > 0) filters.Add(string.Format("StatusID in ({0})", string.Join(", ", statusIDs)));
                if (priorityIDs != null && priorityIDs.Length > 0) filters.Add(string.Format("PriorityID in ({0})", string.Join(", ", priorityIDs)));

                string filter = filters.Count == 0 ? "" : " where " + string.Join(" and ", filters);
                string query = "select * from TBug " + filter + " order by BugID desc";
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                if (dbBugs != null && dbBugs.Count > 0)
                {
                    List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                    return result;
                }
                return new List<TBugFullInfo>();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public TBugFullInfo GetBug(int bugID, bool includeData = false)
        {

            try
            {
                string query = "select * from TBug where BugID = " + bugID;
                List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);
                if(dbBugs?.Count > 0)
                {
                    TBugFullInfo result = dbBugs.Select(ite => new TBugFullInfo(ite)).FirstOrDefault();
                    var attachments = GetAttachments(bugID, includeData);
                    result.Attachments = attachments;
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }
        public List<TBugFullInfo> GetBugs(params int[] bugIds)
        {
            try
            {
                if(bugIds?.Length > 0)
                {
                    string query = "select * from TBug where BugID in (" + string.Join(", ", bugIds)  + ") order by BugID desc";
                    List<TBugADO> dbBugs = ExecuteReader<TBugADO>(query);

                    if (dbBugs != null && dbBugs.Count > 0)
                    {
                        List<TBugFullInfo> result = dbBugs.Select(ite => new TBugFullInfo(ite)).ToList();
                        bugIds = dbBugs.Select(ite => ite.BugID).ToArray();
                        int[] exeBugIDs;
                        while (bugIds.Length > 0)
                        {
                            exeBugIDs = bugIds.Take(20).ToArray();
                            var attachments = GetAttachments(exeBugIDs, false);
                            if (attachments != null && attachments.Count > 0)
                            {
                                var m_attachment = attachments.GroupBy(ite => ite.BugID).ToDictionary(ite => ite.Key, ite => ite.ToList());
                                result.ForEach(ite => { if (m_attachment.ContainsKey(ite.BugID)) ite.Attachments = m_attachment[ite.BugID]; });
                            }
                            bugIds = bugIds.Skip(20).ToArray();
                        }
                        return result;
                    }
                }
                
                return new List<TBugFullInfo>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AssignBug(int assigneeID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                string query = string.Format("update TBug set AssignedAccountID = {0}, StatusID = 1 where BugID in ({1})", assigneeID, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int AssignBugToMe(int assigneeID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                string query = string.Format("update TBug set AssignedAccountID = {0}, StatusID = 2 where BugID in ({1})", assigneeID, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int ReopenBugs(int accountID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                DateTime utcNow = DateTime.UtcNow;
                string query = string.Format("update TBug set OpenedAccountID = {0}, StatusID = {1}, OpenedDate = {2:yyyyMMdd} where BugID in ({3})", accountID, 8, utcNow, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int VerifyBugs(int accountID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                DateTime utcNow = DateTime.UtcNow;
                string query = string.Format("update TBug set VerifiedAccountID = {0}, StatusID = {1}, VerifiedDate = {2:yyyyMMdd} where BugID in ({3})", accountID, 16, utcNow, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int ResolveBugs(int accountID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                DateTime utcNow = DateTime.UtcNow;
                string query = string.Format("update TBug set AssignedAccountID = {0}, StatusID = {1}, ResolvedDate = {2:yyyyMMdd} where BugID in ({3})", accountID, 4, utcNow, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int ByDesignBugs(int accountID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                DateTime utcNow = DateTime.UtcNow;
                string query = string.Format("update TBug set AssignedAccountID = {0}, StatusID = {1}, ResolvedDate = {2:yyyyMMdd} where BugID in ({3})", accountID, 64, utcNow, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int NotFixBugs(int accountID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                DateTime utcNow = DateTime.UtcNow;
                string query = string.Format("update TBug set AssignedAccountID = {0}, StatusID = {1}, ResolvedDate = {2:yyyyMMdd} where BugID in ({3})", accountID, 32, utcNow, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int FixedBugs(int accountID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                DateTime utcNow = DateTime.UtcNow;
                string query = string.Format("update TBug set AssignedAccountID = {0}, StatusID = {1}, ResolvedDate = {2:yyyyMMdd} where BugID in ({3})", accountID, 128, utcNow, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int PleaseWaitBugs(int accountID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                DateTime utcNow = DateTime.UtcNow;
                string query = string.Format("update TBug set AssignedAccountID = {0}, StatusID = {1}, ResolvedDate = {2:yyyyMMdd} where BugID in ({3})", accountID, 256, utcNow, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdatePriorityBugs(int priorityID, params int[] bugIDs)
        {
            try
            {
                if (bugIDs == null || bugIDs.Length == 0) return 0;
                string query = string.Format("update TBug set PriorityID = {0} where BugID in ({1})", priorityID, string.Join(", ", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int UpdateBugComment(int bugID, string commment)
        {
            try
            {
                int createdDate = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
                string query = string.Format("update TBug set Comment = '{0}' where BugID = {1}", commment.Replace("\\","\\\\").Replace("'", "''"), bugID);
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int UpdateBugVersion(int bugID, string version)
        {
            try
            {
                int createdDate = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
                string query = string.Format("update TBug set Version = '{0}' where BugID = {1}", version.Replace("\\", "\\\\").Replace("'", "''"), bugID);
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int UpdateBugResolveVersion(int bugID, string resolveVersion)
        {
            try
            {
                int createdDate = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
                string query = string.Format("update TBug set ResolveVersion = '{0}' where BugID = {1}", resolveVersion.Replace("\\", "\\\\").Replace("'", "''"), bugID);
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int UpdateBugResolveVersion(int[] bugIDs, string resolveVersion)
        {
            try
            {
                int createdDate = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
                string query = string.Format("update TBug set ResolveVersion = '{0}' where BugID in ({1})", resolveVersion.Replace("\\", "\\\\").Replace("'", "''"), string.Join(",", bugIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int UpdateBugDescription(int bugID, string description)
        {
            try
            {
                int createdDate = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
                string query = string.Format("update TBug set Description = '{0}' where BugID = {1}", description.Replace("\\", "\\\\").Replace("'", "''"), bugID);
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region TDocument
        int GetMaxDocumentID()
        {
            try
            {
                string query = string.Format("select max(DocumentID) from TDocument");
                object objValue = ExecuteScalar(query);
                if (objValue.Equals(DBNull.Value)) objValue = null;
                return Convert.ToInt32(objValue ?? 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AddDocuments(params TDocumentADO[] documents)
        {
            try
            {
                if (documents == null || documents.Length == 0) return 0;
                int maxDocumentID = GetMaxDocumentID();
                documents = documents.Select((ite, idx) => { ite.DocumentID = maxDocumentID + idx + 1; return ite; }).ToArray();
                string[] insertFields = new string[] { "DocumentID", "SprintID", "TaskID", "AddedAccountID", "FileName", "FileExtension", "FileData" };
                return ExecuteInsert("TDocument", insertFields, documents);
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int DeleteDocuments(params int[] documentIDs)
        {
            if (documentIDs == null || documentIDs.Length == 0) return 0;
            string query = string.Format("delete from TDocument where DocumentID in ({0})", string.Join(",", documentIDs));
            return ExecuteNonQuery(query);
        }
        public List<TDocumentADO> GetDocuments(int sprintID, int taskID, bool includeData)
        {
            try
            {
                List<string> filters = new List<string>();
                if (sprintID > -1) filters.Add("SprintID = " + sprintID);
                if (taskID > -1) filters.Add("TaskID = " + taskID);
                string filter = filters.Count > 0 ? " where " + string.Join(" and ", filters) : "";
                string[] selectFields = includeData ? new string[] { "DocumentID", "SprintID", "TaskID", "AddedAccountID", "FileName", "FileExtension", "FileData" } : new string[] { "DocumentID", "SprintID", "TaskID", "AddedAccountID", "FileName", "FileExtension" };
                string query = string.Format("select {0} from TDocument {1}", string.Join(", ", selectFields), filter);
                List<TDocumentADO> result = ExecuteReader<TDocumentADO>(query, selectFields);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TDocumentADO> GetDocuments(params int[] documentIDs)
        {
            try
            {
                if (documentIDs == null || documentIDs.Length == 0) return new List<TDocumentADO>();               
                
                string query = string.Format("select * from TDocument where DocumentID in ({0})", string.Join(", ", documentIDs));
                List<TDocumentADO> result = ExecuteReader<TDocumentADO>(query);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TDocumentADO GetDocument(int documentID)
        {
            try
            {
                string query = string.Format("select * from TDocument where DocumentID = {0}", documentID);
                List<TDocumentADO> result = ExecuteReader<TDocumentADO>(query);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion        

        #region TPriority
        public List<TPriorityADO> GetAllPriorities()
        {
            string query = "select * from TPriority";
            List<TPriorityADO> result = ExecuteReader<TPriorityADO>(query);
            return result;
        }
        #endregion

        #region TSprint
        int GetMaxSprintID()
        {
            try
            {
                string query = string.Format("select max(SprintID) from TSprint");
                object objValue = ExecuteScalar(query);
                if (objValue.Equals(DBNull.Value)) objValue = null;
                return Convert.ToInt32(objValue ?? 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AddSprint(params TSprintADO[] sprints)
        {
            try
            {
                if (sprints == null || sprints.Length == 0) return 0;
                int maxSprintID = GetMaxSprintID();
                sprints = sprints.Select((ite, idx) => { ite.SprintID = maxSprintID + idx + 1; return ite; }).ToArray();
                return ExecuteInsert("TSprint", new string[] { "SprintID", "SprintName", "StartDate", "EndDate" }, sprints);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int DeleteSprint(params int[] sprintIDs)
        {
            try
            {
                sprintIDs = sprintIDs.Where(ite => ite != 0).ToArray();
                if (sprintIDs == null || sprintIDs.Length == 0) return 0;
                string query = "update TBug set SprintID = 0 where SprintID in (" + string.Join(", ", sprintIDs) + ")";
                ExecuteNonQuery(query);
                query = "update TTask set SprintID = 0 where SprintID in (" + string.Join(", ", sprintIDs) + ")";
                ExecuteNonQuery(query);
                query = "update TDocument set SprintID = 0 where SprintID in (" + string.Join(", ", sprintIDs) + ")";
                ExecuteNonQuery(query);
                query = "delete from TSprint where SprintID in (" + string.Join(", ", sprintIDs) + ")";
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<TSprintInfo> GetAllSprints()
        {
            try
            {
                string query = "select * from TSprint order by SprintID desc";
                List<TSprintADO> sprintsADO = ExecuteReader<TSprintADO>(query);
                if (sprintsADO != null && sprintsADO.Count > 0)
                {
                    List<TSprintInfo> result = sprintsADO.Select(ite => new TSprintInfo(ite)).ToList();
                    query = "select * from TTask where SprintID in (" + string.Join(", ", sprintsADO.Select(ite=>ite.SprintID)) + ") order by Summary";
                    var tasks = ExecuteReader<TTaskADO>(query);
                    if (tasks != null && tasks.Count > 0)
                    {
                        var m_task = tasks.GroupBy(ite => ite.SprintID).ToDictionary(ite => ite.Key, ite => ite.ToList());
                        result.ForEach(ite => { if (m_task.ContainsKey(ite.SprintID)) ite.Tasks = m_task[ite.SprintID]; });
                    }
                    
                    return result;
                }
                return new List<TSprintInfo>();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region TStatus
        public List<TStatusADO> GetAllStatuses()
        {
            string query = "select * from TStatus";
            List<TStatusADO> result = ExecuteReader<TStatusADO>(query);
            return result;
        }
        #endregion

        #region TTask
        int GetMaxTaskID()
        {
            try
            {
                string query = string.Format("select max(TaskID) from TTask");
                object objValue = ExecuteScalar(query);
                if (objValue.Equals(DBNull.Value)) objValue = null;
                return Convert.ToInt32(objValue ?? 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AddTasks(params TTaskADO[] tasks)
        {
            try
            {
                if (tasks == null || tasks.Length == 0) return 0;
                int maxTaskID = GetMaxTaskID();
                tasks = tasks.Select((ite, idx) => { ite.TaskID = maxTaskID + idx + 1; return ite; }).ToArray();
                string[] insertFields = new string[] { "TaskID", "SprintID", "Summary" };
                return ExecuteInsert("TTask", insertFields, tasks);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int UpdateTaskName(int taskID, string taskName)
        {
            try
            {
                string query = string.Format("update TTask set Summary = '{0}' where TaskID = {1}", taskName, taskID);
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int DeleteTasks(params int[] taskIDs)
        {
            try
            {
                taskIDs = taskIDs.Where(ite => ite != 0).ToArray();
                if (taskIDs == null || taskIDs.Length == 0) return 0;
                string query = string.Format("update TBug set SprintID = 0, TaskID = 0 where TaskID in ({0})", string.Join(", ", taskIDs));
                ExecuteNonQuery(query);
                query = string.Format("update TDocument set SprintID = 0, TaskID = 0 where TaskID in ({0})", string.Join(", ", taskIDs));
                ExecuteNonQuery(query);
                query = string.Format("delete from TTask where TaskID in ({0})", string.Join(", ", taskIDs));
                return ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region TRight
        public List<TRightADO> GetAllRights()
        {
            string query = "select * from TRight";
            List<TRightADO> result = ExecuteReader<TRightADO>(query);
            return result;
        }
        #endregion

        #region TNotify

        int GetMaxNotifyID()
        {
            try
            {
                string query = string.Format("select max(NotifyID) from TNotify");
                object objValue = ExecuteScalar(query);
                if (objValue.Equals(DBNull.Value)) objValue = null;
                return Convert.ToInt32(objValue ?? 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        int AddNotify(TNotifyADO notify)
        {
            try
            {
                string query = string.Format("select * TNotify where AccountID = " + notify.AccountID);
                var existNotify = ExecuteReader<TNotifyADO>(query);
                if (existNotify?.Count > 0) return 0;
                int maxID = GetMaxNotifyID();
                notify.NotifyID = maxID + 1;
                int exec = ExecuteInsert("TNotify", new string[] { "NotifyID", "AccountID", "Email", "SkypeID", "ScheduleTime", "Status" }, notify);
                return exec;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int AddNotify(TNotifyFullInfo notifyInfo)
        {
            try
            {
                string query = string.Format("select * from TNotify where AccountID = " + notifyInfo.AccountID);
                var existNotify = ExecuteReader<TNotifyADO>(query);
                if (existNotify?.Count > 0)
                {
                    query = string.Format("delete from TNotify where AccountID = " + notifyInfo.AccountID);
                    ExecuteNonQuery(query);
                }
                int maxID = GetMaxNotifyID();
                TNotifyADO notifyADO = new TNotifyADO()
                {
                    AccountID = notifyInfo.AccountID,
                    Email = notifyInfo.Email,
                    SkypeID = notifyInfo.SkypeID,
                    ScheduleTime = (int)notifyInfo.RepeatDays * 1000000 + notifyInfo.RepeatTime,
                    Status = notifyInfo.Status,
                    BugStatus = notifyInfo.BugStatus
                };
                notifyADO.NotifyID = maxID + 1;
                int exec = ExecuteInsert("TNotify", new string[] { "NotifyID", "AccountID", "Email", "SkypeID", "ScheduleTime", "BugStatus", "Status" }, notifyADO);
                return exec;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TNotifyFullInfo GetNotify(int accountID)
        {
            try
            {
                string query = string.Format("select * from TNotify where AccountID = " + accountID);
                List<TNotifyADO> notifyADOs = ExecuteReader<TNotifyADO>(query);
                if (notifyADOs?.Count > 0)
                {
                    TNotifyFullInfo result = notifyADOs.Select(ite => new TNotifyFullInfo() { AccountID = ite.AccountID, BugStatus = ite.BugStatus,
                    Email = ite.Email, SkypeID = ite.SkypeID, Status = ite.Status, RepeatDays =  (TNotifyFullInfo.RepeatDayOfWeek)(ite.ScheduleTime / 1000000), RepeatTime = ite.ScheduleTime % 1000000 }).FirstOrDefault();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }
        public List<TNotifyFullInfo> GetAllNotifies()
        {
            try
            {
                string query = string.Format("select * from TNotify");
                List<TNotifyADO> notifyADOs = ExecuteReader<TNotifyADO>(query);
                if (notifyADOs?.Count > 0)
                {
                    List<TNotifyFullInfo> result = notifyADOs.Select(ite => new TNotifyFullInfo()
                    {
                        AccountID = ite.AccountID,
                        BugStatus = ite.BugStatus,
                        Email = ite.Email,
                        SkypeID = ite.SkypeID,
                        Status = ite.Status,
                        RepeatDays = (TNotifyFullInfo.RepeatDayOfWeek)(ite.ScheduleTime / 1000000),
                        RepeatTime = ite.ScheduleTime % 1000000
                    }).ToList();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        #endregion
    }
}
