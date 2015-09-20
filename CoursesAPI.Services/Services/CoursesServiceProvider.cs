using System.Collections.Generic;
using System.Linq;
using CoursesAPI.Models;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Exceptions;
using CoursesAPI.Services.Models.Entities;

namespace CoursesAPI.Services.Services
{
	public class CoursesServiceProvider
	{
		private readonly IUnitOfWork _uow;

		private readonly IRepository<CourseInstance> _courseInstances;
		private readonly IRepository<TeacherRegistration> _teacherRegistrations;
		private readonly IRepository<CourseTemplate> _courseTemplates; 
		private readonly IRepository<Person> _persons;

		public CoursesServiceProvider(IUnitOfWork uow)
		{
			_uow = uow;

			_courseInstances      = _uow.GetRepository<CourseInstance>();
			_courseTemplates      = _uow.GetRepository<CourseTemplate>();
			_teacherRegistrations = _uow.GetRepository<TeacherRegistration>();
			_persons              = _uow.GetRepository<Person>();
		}

		/// <summary>
		/// You should implement this function, such that all tests will pass.
		/// </summary>
		/// <param name="courseInstanceID">The ID of the course instance which the teacher will be registered to.</param>
		/// <param name="model">The data which indicates which person should be added as a teacher, and in what role.</param>
		/// <returns>Should return basic information about the person.</returns>
		public PersonDTO AddTeacherToCourse(int courseInstanceID, AddTeacherViewModel model)
		{
            // 1. Validation
            var courseInstance = _courseInstances.All().SingleOrDefault(x => x.ID == courseInstanceID);

            if (courseInstance == null)
            {
                throw new AppObjectNotFoundException();
            }

            var person = _persons.All().SingleOrDefault(x => x.SSN == model.SSN);

            if(person == null)
            {
                throw new AppObjectNotFoundException();
            }

            var registration = _teacherRegistrations.All().SingleOrDefault(x => x.SSN == model.SSN);

            if(registration != null)
            {
                throw new AppValidationException("TEACHER_ALREADY_ASSIGNED_TO_COURSE");
            }

            // Check if model's type is a main teacher, if so and there already is a main teacher
            // we throw an error.
            if(model.Type == TeacherType.MainTeacher)
            {
                var teacherRegistrations = _teacherRegistrations.All().Where(x => x.CourseInstanceID == courseInstance.ID).ToList();

                foreach(TeacherRegistration teacher in teacherRegistrations)
                {
                    if(teacher.Type == TeacherType.MainTeacher)
                    {
                        throw new AppValidationException("COURSE_ALREADY_HAS_MAIN_TEACHER");
                    }
                }
            }
            
            // Everything seems to be in order, create the entity model and 
            // assign the teacher to the course
            var registrationEntity = new TeacherRegistration
            {
                CourseInstanceID = courseInstanceID,
                SSN = model.SSN,
                Type = model.Type
            };

            _teacherRegistrations.Add(registrationEntity);
            _uow.Save();

            // Creating the return object
            var personDTO = new PersonDTO
            {
                Name = person.Name,
                SSN = person.SSN
            };

            return personDTO;
		}

		/// <summary>
		/// You should write tests for this function. You will also need to
		/// modify it, such that it will correctly return the name of the main
		/// teacher of each course.
		/// </summary>
		/// <param name="semester"></param>
		/// <returns></returns>
		public List<CourseInstanceDTO> GetCourseInstancesBySemester(string semester = null)
		{
			if (string.IsNullOrEmpty(semester))
			{
				semester = "20153";
			}


            var coursesLeftQuery = from c in _courseInstances.All()
                                   join ct in _courseTemplates.All() on c.CourseID equals ct.CourseID
                                   select new { c, ct };

            var teachersRightQuery = from tr in _teacherRegistrations.All()
                                     join p in _persons.All() on tr.SSN equals p.SSN
                                     where tr.Type == TeacherType.MainTeacher
                                     select new { tr, p };

            // Run through courses and search for the main teacher for each corresponding course
            // If he's not found his name will be set as empty.
            var result = (from course in coursesLeftQuery
                          join reg in teachersRightQuery on course.c.ID equals reg.tr.CourseInstanceID into teachers
                          from teacher in teachers.DefaultIfEmpty()
                          where course.c.SemesterID == semester
                          select new CourseInstanceDTO
                          {
                              CourseInstanceID = course.c.ID,
                              Name = course.ct.Name,
                              TemplateID = course.ct.CourseID,
                              MainTeacher = (teacher == null ? "" : teacher.p.Name)
                          }).ToList();

            return result;
        }
	}
}
