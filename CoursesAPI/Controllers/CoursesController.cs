using System.Web.Http;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Services;

namespace CoursesAPI.Controllers
{
	[RoutePrefix("api/courses")]
	public class CoursesController : ApiController
	{
		private readonly CoursesServiceProvider _service;

		public CoursesController()
		{
			_service = new CoursesServiceProvider(new UnitOfWork<AppDataContext>());
		}

		// NOTE: we won't implement anything here!
		// We will only execute the business logic code in the context
		// of the unit tests.

	}
}
