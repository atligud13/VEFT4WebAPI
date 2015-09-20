namespace CoursesAPI.Models
{
	/// <summary>
	/// 
	/// </summary>
	public class CourseInstanceDTO
	{
		/// <summary>
		/// The database-generated ID of the course instance.
		/// </summary>
		public int    CourseInstanceID { get; set; }

		/// <summary>
		/// The string identifier of the course template this
		/// course belongs to. Example: "T-514-VEFT"
		/// </summary>
		public string TemplateID       { get; set; }

		/// <summary>
		/// The name of the course.
		/// </summary>
		public string Name             { get; set; }

		/// <summary>
		/// The full name of the main teacher in the course.
		/// If no teacher is defined, the function will
		/// return an empty string.
		/// </summary>
		public string MainTeacher      { get; set; }
	}
}
