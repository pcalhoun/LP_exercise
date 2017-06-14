using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Service {

	public enum RequestStatus {
		SUCCESS,
		SQLITE_ERROR
	}

	public class DataService : MonoBehaviour {

		private static SQLiteService _sqLiteService;
		private static DataService _dataService;

		// Methods

		public static DataService CreateGO() {

			GameObject dataServiceGO = new GameObject();
			_dataService = dataServiceGO.AddComponent<DataService>();
			dataServiceGO.name = "DataService";

			_sqLiteService = dataServiceGO.AddComponent<SQLiteService>();

			return _dataService;
		}

		public void GetQuestionIDs(Action<string[]> OnGetQuestionIDs) {
			_sqLiteService.GetQuestionIDs(OnGetQuestionIDs);
		}

		public void GetQuestionModel(string questionID, Action<QuestionModel> OnGetQuestionModel) {
			_sqLiteService.GetQuestionModel(questionID, OnGetQuestionModel);
		}
	}
}
