using UnityEngine;

using Mono.Data.SqliteClient;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Service {
	public class SQLiteService : MonoBehaviour {

		//References
		protected static string DBFileLocation = "/LP_exercise.db";
		protected readonly object taskQueueLock = new object();
		protected Queue<object> taskQueue = null;

		public static SQLiteService Instance;

		private SQLiteService_QuestionData questionData;
		private SQLiteService_TokenData tokenData;

		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		void Awake() {
			Instance = this;
			path = UnityEngine.Application.persistentDataPath + DBFileLocation;

			taskQueue = new Queue<object>();

			questionData = new SQLiteService_QuestionData();
			tokenData = new SQLiteService_TokenData();

			this.LoadLocalSQLite();
		}

		void Update() {
			
			object nextTask = null;

			lock (taskQueueLock) {
				if (taskQueue.Count > 0) {
					nextTask = taskQueue.Dequeue ();
				}
			}

			// process next task
			if (nextTask != null) {
				object[] task = nextTask as object[];
				Func<object> function = task[0] as Func<object>;
				//Debug.Log ("Dequeue >>>>>> function: " + function.Method.ToString());
				Action<object> callback = task[1] as Action<object>;
				if (function != null) {
					object result = function();
					callback(result);
				}
			}
		}

		public void LoadLocalService() {
			this.LoadLocalSQLite();
		}

		public void ResetLocalService() {
			this.ResetLocalSQLite();
		}

		public void DeleteLocalService() {
			this.DeleteLocalSQLite();
		}

		// Accessors

		public void GetQuestionIDs(Action<string[]> OnGetQuestionIDs)
		{
			if (OnGetQuestionIDs != null) {
				questionData.GetIDs((IDs, status)=>{
					if(status.Equals(RequestStatus.SUCCESS)) {
						OnGetQuestionIDs(IDs);
					} else {
						OnGetQuestionIDs(null);
					}
				});
			}
		}
			
		public void GetQuestionModel(string questionID, Action<QuestionModel> OnGetQuestionModel) {
			if (OnGetQuestionModel != null) {
				questionData.GetModel(questionID, (model, status)=>{
					if(status.Equals(RequestStatus.SUCCESS)) {
						OnGetQuestionModel(model);
					} else {
						OnGetQuestionModel(null);
					}
				});
			}
		}

		// SQLiteService Functions

		string _path;
		/// <summary>
		/// Gets or sets the path, throws an error if the path is retrieved before being set.
		/// </summary>
		string path {
			get {
				if (string.IsNullOrEmpty(_path)) 
					throw new NullReferenceException("Path not set yet");
				return _path;
			}
			set { _path = value; }
		}

		internal IDbConnection GetOpenConnection() {
			IDbConnection connection = (IDbConnection) new SqliteConnection("URI=file:" + path);
			connection.Open();
			return connection;
		}

		private void LoadLocalSQLite() {
			
			bool dbExists = System.IO.File.Exists(path);

			Debug.Log (path);

			if (!dbExists) {
				
				using (IDbConnection dbc = this.GetOpenConnection()) {
					using (IDbCommand dbcmd = dbc.CreateCommand()) {

						dbcmd.CommandText = this.questionData.CreateTableCommand;
						dbcmd.ExecuteNonQuery();
					}
				}

				using (IDbConnection dbc = this.GetOpenConnection()) {
					using (IDbCommand dbcmd = dbc.CreateCommand()) {

						dbcmd.CommandText = this.tokenData.CreateTableCommand;
						dbcmd.ExecuteNonQuery();
					}
				}
			}
		}
	
		private void ResetLocalSQLite() {

			bool dbExists = System.IO.File.Exists(path);

			if (dbExists) {
				
				this.QueueTask (() => {
					using (IDbConnection dbc = this.GetOpenConnection ()) {
						using (IDbCommand dbcmd = dbc.CreateCommand ()) {
							dbcmd.CommandText = this.questionData.ResetTableCommand;
							dbcmd.ExecuteNonQuery ();
						}
					}
					return null;
				}, null);

				this.QueueTask (() => {
					using (IDbConnection dbc = this.GetOpenConnection ()) {
						using (IDbCommand dbcmd = dbc.CreateCommand ()) {
							dbcmd.CommandText = this.tokenData.ResetTableCommand;
							dbcmd.ExecuteNonQuery ();
						}
					}
					return null;
				}, null);

			} else {
				LoadLocalSQLite ();
			}
		}

		private void DeleteLocalSQLite() {

			bool dbExists = System.IO.File.Exists(path);

			if (dbExists) {
				System.IO.File.Delete(path);
			}
		}

		/// <summary>
		/// Either enqueue SQL tasks for processing
		/// </summary>
		public void QueueTask(Func<object> function, Action<object> callback)
		{
			object task = new object[] { function, callback };
			lock (taskQueueLock) {
				taskQueue.Enqueue (task);
			}
		}
	}
}
