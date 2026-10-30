using Dapper;
using CCS.Core;
using CCS.Data;
namespace CCS.Services
{
    public class CoachFormService
    {
        public void Save(long coachId,CoachForm form)
        {
            using(var connection=Database.Open())
            {
                var existing=connection.ExecuteScalar<long>("SELECT COUNT(*) FROM CoachForms WHERE CoachId=@CoachId",new{CoachId=coachId});
                if(existing>0)
                {
                    connection.Execute("UPDATE CoachForms SET CoachLevel=@CoachLevel,CoachBase=@CoachBase,PhysicalFileNumber=@PhysicalFileNumber,CollaborationStatus=@CollaborationStatus WHERE CoachId=@CoachId",new{CoachId=coachId,CoachLevel=form.CoachLevel,CoachBase=form.CoachBase,PhysicalFileNumber=form.PhysicalFileNumber,CollaborationStatus=form.CollaborationStatus});
                    return;
                }
                connection.Execute("INSERT INTO CoachForms(CoachId,CoachLevel,CoachBase,PhysicalFileNumber,CollaborationStatus) VALUES(@CoachId,@CoachLevel,@CoachBase,@PhysicalFileNumber,@CollaborationStatus)",new{CoachId=coachId,CoachLevel=form.CoachLevel,CoachBase=form.CoachBase,PhysicalFileNumber=form.PhysicalFileNumber,CollaborationStatus=form.CollaborationStatus});
            }
        }
        public CoachForm GetByCoachId(long coachId)
        {
            using(var connection=Database.Open())
            {
                return connection.QueryFirstOrDefault<CoachForm>("SELECT * FROM CoachForms WHERE CoachId=@CoachId",new{CoachId=coachId});
            }
        }
    }
}