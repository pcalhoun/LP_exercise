using UnityEngine;

using Mono.Data.SqliteClient;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Service {

	public class SQLiteService_QuestionData {

		private SQLiteService service;

		public SQLiteService_QuestionData() {
			service = (SQLiteService) SQLiteService.Instance;
		}

		// Accessors

		public void GetIDs(Action<string[],RequestStatus> OnGetIDs) {
			service.QueueTask (() => {
				
				List<string> result = null;

				using (IDbConnection dbc = service.GetOpenConnection()) {
					using (IDbCommand dbcmd = dbc.CreateCommand()) {
						dbcmd.CommandText = String.Format("SELECT {0} FROM QuestionData", SQLiteFields.Name.QUESTION_ID);

						result = new List<string>();

						using (IDataReader dbr = dbcmd.ExecuteReader ()) {
							while (dbr.Read ()) {

								int idx = dbr.GetOrdinal (SQLiteFields.Name.QUESTION_ID);
								if (!dbr.IsDBNull (idx)) {
									result.Add(dbr.GetString (idx));
								}
							} 
						}
					}
				}

				return result as object;
			}, 
				(resultObject) => {
					if (OnGetIDs != null) {
						if (resultObject == null) // doesn't exist
							OnGetIDs(null, RequestStatus.SQLITE_ERROR);
						else
							OnGetIDs((resultObject as List<string>).ToArray(), RequestStatus.SUCCESS);
					}
				});
		}

		// SQLiteService_QuestionData Functions

		public string CreateTableCommand {

			get {
				return String.Format ("CREATE TABLE QuestionData ({0} TEXT NOT NULL UNIQUE, {1} TEXT, {2} TEXT, PRIMARY KEY({0}))",
					SQLiteFields.Name.QUESTION_ID,
					SQLiteFields.Name.QUESTION_PROMPT,
					SQLiteFields.Name.QUESTION_BLOB
				);
			}
		}

		public string ResetTableCommand {

			get {
				return String.Format ("DELETE FROM QuestionData");
			}
		}

		private bool Exists_Internal(string questionID) {
			string result = null;
			using (IDbConnection dbc = service.GetOpenConnection()) {
				using (IDbCommand dbcmd = dbc.CreateCommand()) {
					dbcmd.CommandText = String.Format("SELECT questionID FROM QuestionData WHERE {0}='{1}'", SQLiteFields.Name.QUESTION_ID, questionID);
					using (IDataReader dbr = dbcmd.ExecuteReader()) {
						while (dbr.Read()) {
							result = dbr.GetString(0);
						} 
					}
				}
			}
			return result != null;
		}

		public void Exists(string questionID, Action<bool> OnSeekModel) {
			string _questionID = (string) questionID.Clone();
			service.QueueTask (() => {
				bool result = Exists_Internal(_questionID);
				return result as object;
			}, 
				(resultObject) => {
					if (OnSeekModel != null)
						OnSeekModel((bool)resultObject);
				});
		}

		public void GetModel(string questionID, Action<QuestionModel,RequestStatus> OnGetModel) {
			service.QueueTask (() => {
				QuestionModel result = null;

				using (IDbConnection dbc = service.GetOpenConnection()) {
					using (IDbCommand dbcmd = dbc.CreateCommand()) {
						dbcmd.CommandText = String.Format("SELECT * FROM QuestionData WHERE {0}='{1}'", SQLiteFields.Name.QUESTION_ID, questionID);

						if (Exists_Internal(questionID)) {
							result = new QuestionModel ();

							using (IDataReader dbr = dbcmd.ExecuteReader ()) {
								while (dbr.Read ()) {

									int idx = dbr.GetOrdinal (SQLiteFields.Name.QUESTION_ID);
									if (!dbr.IsDBNull (idx)) {
										result.ID = dbr.GetString (idx);
									}

									idx = dbr.GetOrdinal (SQLiteFields.Name.QUESTION_PROMPT);
									if (!dbr.IsDBNull (idx)) {
										result.Prompt = dbr.GetString (idx);
									}

									idx = dbr.GetOrdinal (SQLiteFields.Name.QUESTION_BLOB);
									if (!dbr.IsDBNull (idx)) {
										result.Blob = dbr.GetString (idx);
									}
								} 
							}
						}
					}
				}

				return result as object;
			}, 
				(resultObject) => {
					if (OnGetModel != null) {
						if (resultObject == null) // doesn't exist
							OnGetModel(null, RequestStatus.SQLITE_ERROR);
						else
							OnGetModel(resultObject as QuestionModel, RequestStatus.SUCCESS);
					}
				});
		}

		public void SaveModel(QuestionModel data, Action<QuestionModel, RequestStatus> callback) {
			service.QueueTask(() => {
				if (data != null) {
					using (IDbConnection dbc = service.GetOpenConnection ()) {
						using (IDbCommand dbcmd = dbc.CreateCommand ()) {
							dbcmd.CommandText = String.Format ("INSERT OR REPLACE INTO QuestionData ({0},{2},{4}) VALUES ('{1}','{3}','{5}')",
								SQLiteFields.Name.QUESTION_ID, data.ID,
								SQLiteFields.Name.QUESTION_PROMPT, data.Prompt,
								SQLiteFields.Name.QUESTION_BLOB, data.Blob
							);
							dbcmd.ExecuteNonQuery();
						}
					}
				} else {
					Debug.Log ("WARNING: insufficient data to save question model\n");
				}
				return null;
			}, (resultObject) => {
				if (callback != null)
					callback (data, RequestStatus.SUCCESS);
			});
		}

		public void DeleteModel(QuestionModel data) {

			service.QueueTask (() => {
				using (IDbConnection dbc = service.GetOpenConnection ()) {
					using (IDbCommand dbcmd = dbc.CreateCommand ()) {
						dbcmd.CommandText = String.Format ("DELETE FROM QuestionData WHERE QuestionData.{0}='{1}'", SQLiteFields.Name.QUESTION_ID, data.ID);
						dbcmd.ExecuteNonQuery ();
					}
				}
				return null;
			}, null);
		}
	}
}
