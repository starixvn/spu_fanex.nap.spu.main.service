namespace Fanex.NAP.SPU.Main.Features.JobSchedulers.Models
{
    using System.Collections.Generic;
    using System.Linq;    

    public class GroupJob
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string HealthCheck { get; set; }        

        public IEnumerable<Job> Jobs { get; set; }

        public IEnumerable<Job> ExistingJobs { get; set; }

        public IEnumerable<Job> NewJobs { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as GroupJob;

            if (other == null) return false;

            if(other.Name != Name || other.HealthCheck != HealthCheck || !EqualJobs(Jobs, other.Jobs))
            {
                
                return false;
            }

            return true;
        }

        private bool EqualJobs(IEnumerable<Job> jobs1, IEnumerable<Job> jobs2)
        {          
            return jobs1.All(jobs2.Contains);
        }

        public override int GetHashCode() => (Name, HealthCheck, Jobs).GetHashCode();        
    }
}
