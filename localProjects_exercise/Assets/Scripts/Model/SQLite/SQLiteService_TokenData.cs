using UnityEngine;

using Mono.Data.SqliteClient;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Service {

	public class SQLiteService_TokenData {

		private SQLiteService service;

		public SQLiteService_TokenData() {
			service = (SQLiteService) SQLiteService.Instance;
		}

		// SQLiteService_TokenData Functions

		public string CreateTableCommand {

			get {
				return String.Format ("CREATE TABLE TokenData ({0} TEXT NOT NULL UNIQUE, {1} TEXT, {2} TEXT, {3} INTEGER, PRIMARY KEY({0}))",
					SQLiteFields.Name.TOKEN_ID,
					SQLiteFields.Name.TOKEN_TEXT,
					SQLiteFields.Name.TOKEN_COLOR,
					SQLiteFields.Name.TOKEN_SUM
				);
			}
		}

		public string ResetTableCommand {

			get {
				return String.Format ("DELETE FROM TokenData");
			}
		}

		private bool Exists_Internal(string tokenID) {
			string result = null;
			using (IDbConnection dbc = service.GetOpenConnection()) {
				using (IDbCommand dbcmd = dbc.CreateCommand()) {
					dbcmd.CommandText = String.Format("SELECT tokenID FROM TokenData WHERE {0}='{1}'", SQLiteFields.Name.TOKEN_ID, tokenID);
					using (IDataReader dbr = dbcmd.ExecuteReader()) {
						while (dbr.Read()) {
							result = dbr.GetString(0);
						} 
					}
				}
			}
			return result != null;
		}

		public void Exists(string tokenID, Action<bool> OnSeekModel) {
			string _tokenID = (string) tokenID.Clone();
			service.QueueTask (() => {
				bool result = Exists_Internal(_tokenID);
				return result as object;
			}, 
				(resultObject) => {
					if (OnSeekModel != null)
						OnSeekModel((bool)resultObject);
				});
		}

		public void GetModel(string tokenID, Action<TokenModel,RequestStatus> OnGetModel) {
			service.QueueTask (() => {
				TokenModel result = null;

				using (IDbConnection dbc = service.GetOpenConnection()) {
					using (IDbCommand dbcmd = dbc.CreateCommand()) {
						dbcmd.CommandText = String.Format("SELECT * FROM TokenData WHERE {0}='{1}'", SQLiteFields.Name.TOKEN_ID, tokenID);

						if (Exists_Internal(tokenID)) {
							result = new TokenModel ();

							using (IDataReader dbr = dbcmd.ExecuteReader ()) {
								while (dbr.Read ()) {

									int idx = dbr.GetOrdinal (SQLiteFields.Name.TOKEN_ID);
									if (!dbr.IsDBNull (idx)) {
										result.ID = dbr.GetString (idx);
									}

									idx = dbr.GetOrdinal (SQLiteFields.Name.TOKEN_TEXT);
									if (!dbr.IsDBNull (idx)) {
										result.Text = dbr.GetString (idx);
									}

									idx = dbr.GetOrdinal (SQLiteFields.Name.TOKEN_COLOR);
									if (!dbr.IsDBNull (idx)) {
										result.Color = dbr.GetString (idx);
									}

									idx = dbr.GetOrdinal (SQLiteFields.Name.TOKEN_SUM);
									if (!dbr.IsDBNull (idx)) {
										result.Sum = dbr.GetInt32(idx);
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
							OnGetModel(resultObject as TokenModel, RequestStatus.SUCCESS);
					}
				});
		}

		public void SaveModel(TokenModel data, Action<TokenModel, RequestStatus> callback) {
			service.QueueTask(() => {
				if (data != null) {
					using (IDbConnection dbc = service.GetOpenConnection ()) {
						using (IDbCommand dbcmd = dbc.CreateCommand ()) {
							dbcmd.CommandText = String.Format ("INSERT OR REPLACE INTO TokenData ({0},{2},{4},{6}) VALUES ('{1}','{3}','{5}',{7})",
								SQLiteFields.Name.TOKEN_ID, data.ID,
								SQLiteFields.Name.TOKEN_TEXT, data.Text,
								SQLiteFields.Name.TOKEN_COLOR, data.Color,
								SQLiteFields.Name.TOKEN_SUM, data.Sum
							);
							dbcmd.ExecuteNonQuery();
						}
					}
				} else {
					Debug.Log ("WARNING: insufficient data to save token model\n");
				}
				return null;
			}, (resultObject) => {
				if (callback != null)
					callback (data, RequestStatus.SUCCESS);
			});
		}

		public void DeleteModel(TokenModel data) {

			service.QueueTask (() => {
				using (IDbConnection dbc = service.GetOpenConnection ()) {
					using (IDbCommand dbcmd = dbc.CreateCommand ()) {
						dbcmd.CommandText = String.Format ("DELETE FROM TokenData WHERE TokenData.{0}='{1}'", SQLiteFields.Name.TOKEN_ID, data.ID);
						dbcmd.ExecuteNonQuery ();
					}
				}
				return null;
			}, null);
		}
	}
}
