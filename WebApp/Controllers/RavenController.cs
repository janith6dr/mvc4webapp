﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client;
using WebApp.Helpers;
using WebApp.Tasks;
using System.Xml.Linq;
using WebApp.Extensions;

namespace WebApp.Controllers
{
	public abstract class RavenController : Controller
	{
		public static IDocumentStore DocumentStore
		{
			get { return DocumentStoreHolder.Store; }
		}

		public IDocumentSession RavenSession { get; protected set; }

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			RavenSession = DocumentStoreHolder.Store.OpenSession();
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if(filterContext.IsChildAction)
				return;

			using(RavenSession)
			{
				if(filterContext.Exception != null)
					return;

				if(RavenSession != null)
					RavenSession.SaveChanges();
			}

			TaskExecutor.StartExecuting();
		}

		protected HttpStatusCodeResult HttpNotModified()
		{
			return new HttpStatusCodeResult(304);
		}

		protected ActionResult Xml(XDocument xml, string etag)
		{
			return new XmlResult(xml, etag);
		}

		protected new JsonResult Json(object data)
		{
			return base.Json(data, JsonRequestBehavior.AllowGet);
		}
	}
}
