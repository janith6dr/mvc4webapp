﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
	public class DataLoadController : AsyncController
	{
		public ActionResult Index()
		{
			return View();
		}

		public void SomeTaskAsync(int id)
		{
			AsyncManager.OutstandingOperations.Increment();
			Task.Factory.StartNew(taskId =>
			{
				for(int i = 0; i < 100; i++)
				{
					Thread.Sleep(200);
					HttpContext.Application["task" + taskId] = i;
				}
				var result = "result";
				AsyncManager.OutstandingOperations.Decrement();
				AsyncManager.Parameters["result"] = result;
				return result;
			}, id);
		}

		public ActionResult SomeTaskCompleted(string result)
		{
			return Content(result, "text/plain");
		}

		public ActionResult SomeTaskProgress(int id)
		{
			return Json(new
			{
				Progress = HttpContext.Application["task" + id]
			}, JsonRequestBehavior.AllowGet);
		}
    }
}
