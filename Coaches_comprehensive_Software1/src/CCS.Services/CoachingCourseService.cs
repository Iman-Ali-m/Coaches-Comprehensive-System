using System.Collections.Generic;
using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class CoachingCourseService
    {
        public long Create(CoachingCourse course)
        {
            using(var connection=Database.Open())
            {
                course.CreatedAt=Clock.Now();
                return connection.ExecuteScalar<long>("INSERT INTO CoachingCourses(CoachId,CourseType,CourseName,StartDate,EndDate,DaysCount,Description,CreatedAt) VALUES(@CoachId,@CourseType,@CourseName,@StartDate,@EndDate,@DaysCount,@Description,@CreatedAt);SELECT last_insert_rowid();",course);
            }
        }
        public IEnumerable<CoachingCourse> GetByCoachId(long coachId)
        {
            using(var connection=Database.Open())
            {
                return connection.Query<CoachingCourse>("SELECT * FROM CoachingCourses WHERE CoachId=@coachId",new{coachId});
            }
        }
        public void Delete(long id)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("DELETE FROM CourseFiles WHERE CourseId=@id",new{id});
                connection.Execute("DELETE FROM CoachingCourses WHERE Id=@id",new{id});
            }
        }
        public void AddFile(long courseId,string originalFileName,string storedFileName,long fileSize)
        {
            using(var connection=Database.Open())
            {
                connection.Execute("INSERT INTO CourseFiles(CourseId,OriginalFileName,StoredFileName,FileSize,UploadedAt) VALUES(@courseId,@originalFileName,@storedFileName,@fileSize,@uploadedAt)",new{courseId,originalFileName,storedFileName,fileSize,uploadedAt=Clock.Now()});
            }
        }
        public CourseFile GetFirstFileByCourseId(long courseId)
        {
            using(var connection=Database.Open())
            {
                return connection.QueryFirstOrDefault<CourseFile>("SELECT * FROM CourseFiles WHERE CourseId=@courseId ORDER BY Id DESC LIMIT 1",new{courseId});
            }
        }
    }
}